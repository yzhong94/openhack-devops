package openHackDevOps

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"
	"strconv"
)

type TripPoint struct {
	Id                           string
	TripId                       string
	Latitude                     float64
	Longitude                    float64
	Speed                        float64
	RecordedTimeStamp            string
	Sequence                     int
	RPM                          float64
	ShortTermFuelBank            float64
	LongTermFuelBank             float64
	ThrottlePosition             float64
	RelativeThrottlePosition     float64
	Runtime                      float64
	DistanceWithMalfunctionLight float64
	EngineLoad                   float64
	MassFlowRate                 float64
	EngineFuelRate               float64
	VIN                          sql.NullString
	HasOBDData                   bool
	HasSimulatedOBDData          bool
}

// TripPoint Service Methods

func GetAllTripPoints(w http.ResponseWriter, r *http.Request) {
	query := "SELECT [Id], [TripId], [Latitude], [Longitude], [Speed], [RecordedTimeStamp], [Sequence], [RPM], [ShortTermFuelBank], [LongTermFuelBank], [ThrottlePosition], [RelativeThrottlePosition], [Runtime], [DistanceWithMalfunctionLight], [EngineLoad], [EngineFuelRate], [VIN] FROM [dbo].[TripPoints] WHERE Deleted = 0"

	statement, err := ExecuteQuery(query)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while retrieving trip points from database"))
		return
	}

	got := []TripPoint{}

	for statement.Next() {
		var r TripPoint
		err := statement.Scan(&r.Id, &r.TripId, &r.Latitude, &r.Longitude, &r.Speed, &r.RecordedTimeStamp, &r.Sequence, &r.RPM, &r.ShortTermFuelBank, &r.LongTermFuelBank, &r.ThrottlePosition, &r.RelativeThrottlePosition, &r.Runtime, &r.DistanceWithMalfunctionLight, &r.EngineLoad, &r.EngineFuelRate, &r.VIN)

		if err != nil {
			fmt.Fprintf(w, SerializeError(err, "Error scanning Trip Points"))
			return
		}

		got = append(got, r)
	}

	serializedReturn, _ := json.Marshal(got)

	fmt.Fprintf(w, string(serializedReturn))
}

func GetTripPoint(w http.ResponseWriter, r *http.Request) {
	tripPointId := r.FormValue("id")

	query := "SELECT [Id], [TripId], [Latitude], [Longitude], [Speed], [RecordedTimeStamp], [Sequence], [RPM], [ShortTermFuelBank], [LongTermFuelBank], [ThrottlePosition], [RelativeThrottlePosition], [Runtime], [DistanceWithMalfunctionLight], [EngineLoad], [EngineFuelRate], [VIN] FROM TripPoints WHERE Id = '" + tripPointId + "' AND Deleted = 0"

	row, err := FirstOrDefault(query)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while retrieving trip point from database"))
		return
	}

	var tripPoint TripPoint

	err = row.Scan(&tripPoint.Id, &tripPoint.TripId, &tripPoint.Latitude, &tripPoint.Longitude, &tripPoint.Speed, &tripPoint.RecordedTimeStamp, &tripPoint.Sequence, &tripPoint.RPM, &tripPoint.ShortTermFuelBank, &tripPoint.LongTermFuelBank, &tripPoint.ThrottlePosition, &tripPoint.RelativeThrottlePosition, &tripPoint.Runtime, &tripPoint.DistanceWithMalfunctionLight, &tripPoint.EngineLoad, &tripPoint.EngineFuelRate, &tripPoint.VIN)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Failed to scan a trip point"))
		return
	}

	serializedTripPoint, _ := json.Marshal(tripPoint)

	fmt.Fprintf(w, string(serializedTripPoint))
}

func PostTripPoint(w http.ResponseWriter, r *http.Request) {
	tripId := r.FormValue("tripId")

	body, err := ioutil.ReadAll(r.Body)

	var tripPoint TripPoint

	err = json.Unmarshal(body, &tripPoint)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while decoding json"))
		return
	}

	tripPoint.TripId = tripId

	insertQuery := fmt.Sprintf("DECLARE @tempReturn TABLE (TripPointId NVARCHAR(128)); INSERT INTO TripPoints ([TripId], [Latitude], [Longitude], [Speed], [RecordedTimeStamp], [Sequence], [RPM], [ShortTermFuelBank], [LongTermFuelBank], [ThrottlePosition], [RelativeThrottlePosition], [Runtime], [DistanceWithMalfunctionLight], [EngineLoad], [EngineFuelRate], [MassFlowRate], [HasOBDData], [HasSimulatedOBDData], [VIN], [Deleted]) OUTPUT Inserted.ID INTO @tempReturn VALUES ('%s', '%s', '%s', '%s', '%s', %d, '%s', '%s', '%s', '%s', '%s', '%s', '%s', '%s', '%s', '%s', '%s', '%s', '%s', 'false'); SELECT TripPointId FROM @tempReturn",
		tripPoint.TripId,
		strconv.FormatFloat(tripPoint.Latitude, 'f', -1, 64),
		strconv.FormatFloat(tripPoint.Longitude, 'f', -1, 64),
		strconv.FormatFloat(tripPoint.Speed, 'f', -1, 64),
		tripPoint.RecordedTimeStamp,
		tripPoint.Sequence,
		strconv.FormatFloat(tripPoint.RPM, 'f', -1, 64),
		strconv.FormatFloat(tripPoint.ShortTermFuelBank, 'f', -1, 64),
		strconv.FormatFloat(tripPoint.LongTermFuelBank, 'f', -1, 64),
		strconv.FormatFloat(tripPoint.ThrottlePosition, 'f', -1, 64),
		strconv.FormatFloat(tripPoint.RelativeThrottlePosition, 'f', -1, 64),
		strconv.FormatFloat(tripPoint.Runtime, 'f', -1, 64),
		strconv.FormatFloat(tripPoint.DistanceWithMalfunctionLight, 'f', -1, 64),
		strconv.FormatFloat(tripPoint.EngineLoad, 'f', -1, 64),
		strconv.FormatFloat(tripPoint.MassFlowRate, 'f', -1, 64),
		strconv.FormatFloat(tripPoint.EngineFuelRate, 'f', -1, 64),
		strconv.FormatBool(tripPoint.HasOBDData),
		strconv.FormatBool(tripPoint.HasSimulatedOBDData),
		tripPoint.VIN.String)

	fmt.Fprintf(w, insertQuery)

	// var newTripPoint NewTripPoint

	// result, err := ExecuteQuery(insertQuery)

	// if err != nil {
	// 	fmt.Fprintf(w, SerializeError(err, "Error while inserting Trip Point onto database"))
	// 	return
	// }

	// for result.Next() {
	// 	err = result.Scan(&newTripPoint.Id)

	// 	if err != nil {
	// 		fmt.Fprintf(w, SerializeError(err, "Error while retrieving last id"))
	// 	}
	// }

	// serializedTripPoint, _ := json.Marshal(newTripPoint)

	// fmt.Fprintf(w, string(serializedTripPoint))
}

func PatchTripPoint(w http.ResponseWriter, r *http.Request) {
	// tripPointId := r.FormValue("id")

	// body, err := ioutil.ReadAll(r.Body)
}

func DeleteTripPoint(w http.ResponseWriter, r *http.Request) {
	// tripPointId := r.FormValue("id")
}

func GetMaxSequence(w http.ResponseWriter, r *http.Request) {
	// tripId = r.FormValue("id")
}

// End of Trip Point Service Methods

type NewTripPoint struct {
	Id string
}
