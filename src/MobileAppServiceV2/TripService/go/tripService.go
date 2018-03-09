package openHackDevOps

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"strconv"

	_ "github.com/denisenkom/go-mssqldb"
)

type Trip struct {
	Id                  string
	Name                string
	UserId              string
	RecordedTimeStamp   string
	EndTimeStamp        string
	Rating              int
	IsComplete          bool
	HasSimulatedOBDData bool
	AverageSpeed        float64
	FuelUsed            float64
	HardStops           float64
	HardAccelerations   float64
	MainPhotoUrl        string
	Distance            float64
}

func GetTrip(w http.ResponseWriter, r *http.Request) {
	tripId := r.FormValue("id")

	query := "SELECT Id, Name, UserId, RecordedTimeStamp, EndTimeStamp, Rating, IsComplete, HasSimulatedOBDData, AverageSpeed, FuelUsed, HardStops, HardAccelerations, MainPhotoUrl, Distance FROM Trips WHERE Id = '" + tripId + "' AND Deleted = 0"

	row, err := FirstOrDefault(query)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while retrieving trip from database"))
		return
	}

	var trip Trip

	err = row.Scan(&trip.Id, &trip.Name, &trip.UserId, &trip.RecordedTimeStamp, &trip.EndTimeStamp, &trip.Rating, &trip.IsComplete, &trip.HasSimulatedOBDData, &trip.AverageSpeed, &trip.FuelUsed, &trip.HardStops, &trip.HardAccelerations, &trip.MainPhotoUrl, &trip.Distance)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Failed to scan a trip"))
		return
	}

	serializedTrip, _ := json.Marshal(trip)

	fmt.Fprintf(w, string(serializedTrip))
}

func GetAllTrips(w http.ResponseWriter, r *http.Request) {
	userId := r.FormValue("Id")

	query := "SELECT Id, Name, UserId, RecordedTimeStamp, EndTimeStamp, Rating, IsComplete, HasSimulatedOBDData, AverageSpeed, FuelUsed, HardStops, HardAccelerations, MainPhotoUrl, Distance FROM Trips WHERE UserId LIKE '%" + userId + "' and Deleted = 0"

	statement, err := ExecuteQuery(query)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while retrieving trips from database"))
		return
	}

	got := []Trip{}

	for statement.Next() {
		var r Trip
		err := statement.Scan(&r.Id, &r.Name, &r.UserId, &r.RecordedTimeStamp, &r.EndTimeStamp, &r.Rating, &r.IsComplete, &r.HasSimulatedOBDData, &r.AverageSpeed, &r.FuelUsed, &r.HardStops, &r.HardAccelerations, &r.MainPhotoUrl, &r.Distance)

		if err != nil {
			fmt.Fprintf(w, SerializeError(err, "Error scanning Trips"))
			return
		}

		got = append(got, r)
	}

	serializedReturn, _ := json.Marshal(got)

	fmt.Fprintf(w, string(serializedReturn))
}

func DeleteTrip(w http.ResponseWriter, r *http.Request) {
	tripId := r.FormValue("id")

	deleteTripPointsQuery := fmt.Sprintf("UPDATE TripPoints SET Deleted = 1 WHERE TripId = '%s'", tripId)
	deleteTripsQuery := fmt.Sprintf("UPDAte Trips SET Deleted = 1 WHERE Id = '%s'", tripId)

	log.Println(deleteTripPointsQuery)
	log.Println(deleteTripsQuery)

	result, err := ExecuteNonQuery(deleteTripPointsQuery)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while deleting trip points from database"))
		return
	}

	result, err = ExecuteNonQuery(deleteTripsQuery)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while deleting trip from database"))
		return
	}

	serializedResult, _ := json.Marshal(result)

	fmt.Fprintf(w, string(serializedResult))
}

func PatchTrip(w http.ResponseWriter, r *http.Request) {
	tripId := r.FormValue("id")
	body, err := ioutil.ReadAll(r.Body)

	defer r.Body.Close()

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while reading request body"))
		return
	}

	var trip Trip

	err = json.Unmarshal(body, &trip)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while decoding json"))
		return
	}

	updateQuery := fmt.Sprintf("UPDATE Trips SET Name = '%s', UserId = '%s', RecordedTimeStamp = '%s', EndTimeStamp = '%s', Rating = %d, IsComplete = '%s', HasSimulatedOBDData = '%s', AverageSpeed = %f, FuelUsed = %s, HardStops = %s, HardAccelerations = %s, MainPhotoUrl = '%s', Distance = %f, UpdatedAt = GETDATE() WHERE Id = '%s'", trip.Name, trip.UserId, trip.RecordedTimeStamp, trip.EndTimeStamp, trip.Rating, strconv.FormatBool(trip.IsComplete), strconv.FormatBool(trip.HasSimulatedOBDData), trip.AverageSpeed, strconv.FormatFloat(trip.FuelUsed, 'f', -1, 64), strconv.FormatFloat(trip.HardStops, 'f', -1, 64), strconv.FormatFloat(trip.HardAccelerations, 'f', -1, 64), trip.MainPhotoUrl, trip.Distance, tripId)

	result, err := ExecuteNonQuery(updateQuery)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while patching trip on the database"))
		return
	}

	fmt.Fprintf(w, string(result))
}

func PostTrip(w http.ResponseWriter, r *http.Request) {
	userId := r.FormValue("userId")

	body, err := ioutil.ReadAll(r.Body)

	var trip Trip

	err = json.Unmarshal(body, &trip)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while decoding json"))
		return
	}

	trip.UserId = userId

	insertQuery := fmt.Sprintf("DECLARE @tempReturn TABLE (TripId NVARCHAR(128)); INSERT INTO Trips (Name, UserId, RecordedTimeStamp, EndTimeStamp, Rating, IsComplete, HasSimulatedOBDData, AverageSpeed, FuelUsed, HardStops, HardAccelerations, MainPhotoUrl, Distance, Deleted) OUTPUT Inserted.ID INTO @tempReturn VALUES ('%s', '%s', '%s', '%s', %d, '%s', '%s', %f, '%s', '%s', '%s', '%s', %f, 'false'); SELECT TripId FROM @tempReturn", trip.Name, trip.UserId, trip.RecordedTimeStamp, trip.EndTimeStamp, trip.Rating, strconv.FormatBool(trip.IsComplete), strconv.FormatBool(trip.HasSimulatedOBDData), trip.AverageSpeed, strconv.FormatFloat(trip.FuelUsed, 'f', -1, 64), strconv.FormatFloat(trip.HardStops, 'f', -1, 64), strconv.FormatFloat(trip.HardAccelerations, 'f', -1, 64), trip.MainPhotoUrl, trip.Distance)

	var newTrip NewTrip

	result, err := ExecuteQuery(insertQuery)

	if err != nil {
		fmt.Fprintf(w, SerializeError(err, "Error while inserting trip onto database"))
		return
	}

	for result.Next() {
		err = result.Scan(&newTrip.Id)

		if err != nil {
			fmt.Fprintf(w, SerializeError(err, "Error while retrieving last id"))
		}
	}

	serializedTrip, _ := json.Marshal(newTrip)

	fmt.Fprintf(w, string(serializedTrip))
}

type NewTrip struct {
	Id string
}
