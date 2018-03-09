package openHackDevOps

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"net/http"
)

type TripPoint struct {
	Id                           string
	TripId                       string
	Latitude                     float32
	Longitude                    float32
	Speed                        float32
	RecordedTimeStamp            string
	Sequence                     int
	RPM                          float32
	ShortTermFuelBank            float32
	LongTermFuelBank             float32
	ThrottlePosition             float32
	RelativeThrottlePosition     float32
	Runtime                      float32
	DistanceWithMalfunctionLight float32
	EngineLoad                   float32
	MassFlowRate                 float32
	EngineFuelRate               float32
	VIN                          sql.NullString
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
	// userId := r.FormValue("userId")

	// body, err := ioutil.ReadAll(r.Body)
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
