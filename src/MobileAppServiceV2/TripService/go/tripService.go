package openHackDevOps

import (
	"database/sql"
	"encoding/json"
	"flag"
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

var (
	debug         = flag.Bool("debug", false, "enable debugging")
	password      = flag.String("password", "MyComplex-Passw0rd", "the database password")
	port     *int = flag.Int("port", 1433, "the database port")
	server        = flag.String("server", "mydrivingdbserver-vpwupcazgfita.database.windows.net", "the database server")
	user          = flag.String("user", "YourUserName", "the database user")
	database      = flag.String("d", "myDrivingDB", "db_name")
)

func GetTrip(w http.ResponseWriter, r *http.Request) {
	tripId := r.FormValue("id")

	connString := fmt.Sprintf("server=%s;database=%s;user id=%s;password=%s;port=%d", *server, *database, *user, *password, *port)

	if *debug {
		fmt.Printf("connString:%s\n", connString)
	}

	conn, err := sql.Open("mssql", connString)

	if err != nil {
		log.Fatal("Failed to connect to the database: ", err.Error())
	}

	defer conn.Close()

	query := "SELECT Id, Name, UserId, RecordedTimeStamp, EndTimeStamp, Rating, IsComplete, HasSimulatedOBDData, AverageSpeed, FuelUsed, HardStops, HardAccelerations, MainPhotoUrl, Distance FROM Trips WHERE Id = '" + tripId + "'"

	statement, err := conn.Prepare(query)

	if err != nil {
		log.Fatal("Failed to query a trip: ", err.Error())
	}

	defer statement.Close()

	row := statement.QueryRow()

	var trip Trip

	err = row.Scan(&trip.Id, &trip.Name, &trip.UserId, &trip.RecordedTimeStamp, &trip.EndTimeStamp, &trip.Rating, &trip.IsComplete, &trip.HasSimulatedOBDData, &trip.AverageSpeed, &trip.FuelUsed, &trip.HardStops, &trip.HardAccelerations, &trip.MainPhotoUrl, &trip.Distance)

	if err != nil {
		log.Fatal("Failed to scan a trip: ", err.Error())
	}

	serializedTrip, _ := json.Marshal(trip)

	fmt.Fprintf(w, string(serializedTrip))
}

func GetAllTrips(w http.ResponseWriter, r *http.Request) {
	userId := r.FormValue("Id")

	connString := fmt.Sprintf("server=%s;database=%s;user id=%s;password=%s;port=%d", *server, *database, *user, *password, *port)

	if *debug {
		fmt.Printf("connString:%s\n", connString)
	}

	conn, err := sql.Open("mssql", connString)

	if err != nil {
		log.Fatal("Failed to connect to the database: ", err.Error())
	}

	defer conn.Close()

	query := "SELECT Id, Name, UserId, RecordedTimeStamp, EndTimeStamp, Rating, IsComplete, HasSimulatedOBDData, AverageSpeed, FuelUsed, HardStops, HardAccelerations, MainPhotoUrl, Distance FROM Trips WHERE UserId LIKE '%" + userId + "' and Deleted = 0"

	statement, err := conn.Query(query)

	if err != nil {
		log.Fatal("Failed to create the statement query: ", err.Error())
	}

	defer statement.Close()

	got := []Trip{}

	for statement.Next() {
		var r Trip
		err = statement.Scan(&r.Id, &r.Name, &r.UserId, &r.RecordedTimeStamp, &r.EndTimeStamp, &r.Rating, &r.IsComplete, &r.HasSimulatedOBDData, &r.AverageSpeed, &r.FuelUsed, &r.HardStops, &r.HardAccelerations, &r.MainPhotoUrl, &r.Distance)

		if err != nil {
			log.Fatal("Error scanning:", err.Error())
		}

		got = append(got, r)
	}

	serializedReturn, _ := json.Marshal(got)

	fmt.Fprintf(w, string(serializedReturn))
}

func DeleteTrip(w http.ResponseWriter, r *http.Request) {
	tripId := r.FormValue("id")

	connString := fmt.Sprintf("server=%s;database=%s;user id=%s;password=%s;port=%d", *server, *database, *user, *password, *port)

	if *debug {
		fmt.Printf("connString:%s\n", connString)
	}

	conn, err := sql.Open("mssql", connString)

	if err != nil {
		log.Fatal("Failed to connect to the database: ", err.Error())
	}

	defer conn.Close()

	deleteTripPointsQuery := fmt.Sprintf("DELETE FROM TripPoints WHERE TripId = '%s'", tripId)
	deleteTripsQuery := fmt.Sprintf("DELETE FROM Trips WHERE Id = '%s'", tripId)

	deleteTripPointsStatement, err := conn.Prepare(deleteTripPointsQuery)

	if err != nil {
		log.Fatal("Error preparing to delete a Trip point: ", err.Error())
	}

	result, err := deleteTripPointsStatement.Exec()

	if err != nil {
		log.Fatal("Error while deleting a trip point: ", err.Error())
	}

	deleteTripsStatement, err := conn.Prepare(deleteTripsQuery)

	if err != nil {
		log.Fatal("Error while preparing to delete the trip:", err.Error())
	}

	result, err = deleteTripsStatement.Exec()

	if err != nil {
		log.Fatal("Error while deleting the trip: ", err.Error())
	}

	serializedResult, _ := json.Marshal(result)

	fmt.Fprintf(w, string(serializedResult))
}

func PatchTrip(w http.ResponseWriter, r *http.Request) {
	tripId := r.FormValue("id")
	body, err := ioutil.ReadAll(r.Body)

	defer r.Body.Close()

	if err != nil {
		log.Fatal("Error while reading request body ", err.Error())
	}

	var trip Trip

	err = json.Unmarshal(body, &trip)

	if err != nil {
		log.Fatal("Error while decoding json ", err.Error())
	}

	updateQuery := fmt.Sprintf("UPDATE Trips SET Name = '%s', UserId = '%s', RecordedTimeStamp = '%s', EndTimeStamp = '%s', Rating = %d, IsComplete = '%s', HasSimulatedOBDData = '%s', AverageSpeed = %f, FuelUsed = %s, HardStops = %s, HardAccelerations = %s, MainPhotoUrl = '%s', Distance = %f, UpdatedAt = GETDATE() WHERE Id = '%s'", trip.Name, trip.UserId, trip.RecordedTimeStamp, trip.EndTimeStamp, trip.Rating, strconv.FormatBool(trip.IsComplete), strconv.FormatBool(trip.HasSimulatedOBDData), trip.AverageSpeed, strconv.FormatFloat(trip.FuelUsed, 'f', -1, 64), strconv.FormatFloat(trip.HardStops, 'f', -1, 64), strconv.FormatFloat(trip.HardAccelerations, 'f', -1, 64), trip.MainPhotoUrl, trip.Distance, tripId)

	connString := fmt.Sprintf("server=%s;database=%s;user id=%s;password=%s;port=%d", *server, *database, *user, *password, *port)

	if *debug {
		fmt.Printf("connString:%s\n", connString)
	}

	conn, err := sql.Open("mssql", connString)

	if err != nil {
		log.Fatal("Failed to connect to the database: ", err.Error())
	}

	defer conn.Close()

	patchTripStatement, err := conn.Prepare(updateQuery)

	if err != nil {
		log.Fatal("Error while preparing to patch the Trip: ", err.Error())
	}

	result, err := patchTripStatement.Exec()

	if err != nil {
		log.Fatal("Error while updating the Trip: ", err.Error())
	}

	serializedResult, _ := json.Marshal(result)

	fmt.Fprintf(w, string(serializedResult))

	// output := fmt.Sprintf("This is the Fuel Used by %s: %f", tripId, trip.FuelUsed)

	// fmt.Fprintf(w, output)
}

func PostTrip(w http.ResponseWriter, r *http.Request) {
	userId := r.FormValue("userId")

	body, err := ioutil.ReadAll(r.Body)

	var trip Trip

	err = json.Unmarshal(body, &trip)

	if err != nil {
		log.Fatal("Error while decoding json ", err.Error())
	}

	trip.UserId = userId

	insertQuery := fmt.Sprintf("DECLARE @tempReturn TABLE (TripId NVARCHAR(128)); INSERT INTO Trips (Name, UserId, RecordedTimeStamp, EndTimeStamp, Rating, IsComplete, HasSimulatedOBDData, AverageSpeed, FuelUsed, HardStops, HardAccelerations, MainPhotoUrl, Distance, Deleted) OUTPUT Inserted.ID INTO @tempReturn VALUES ('%s', '%s', '%s', '%s', %d, '%s', '%s', %f, '%s', '%s', '%s', '%s', %f, 'false'); SELECT TripId FROM @tempReturn", trip.Name, trip.UserId, trip.RecordedTimeStamp, trip.EndTimeStamp, trip.Rating, strconv.FormatBool(trip.IsComplete), strconv.FormatBool(trip.HasSimulatedOBDData), trip.AverageSpeed, strconv.FormatFloat(trip.FuelUsed, 'f', -1, 64), strconv.FormatFloat(trip.HardStops, 'f', -1, 64), strconv.FormatFloat(trip.HardAccelerations, 'f', -1, 64), trip.MainPhotoUrl, trip.Distance)

	connString := fmt.Sprintf("server=%s;database=%s;user id=%s;password=%s;port=%d", *server, *database, *user, *password, *port)

	conn, err := sql.Open("mssql", connString)

	if err != nil {
		log.Fatal("Failed to connect to the database: ", err.Error())
	}

	defer conn.Close()

	insertStatement, err := conn.Prepare(insertQuery)

	if err != nil {
		log.Fatal("Error while preparing the insert: ", err.Error())
	}

	result := insertStatement.QueryRow()

	var newTrip NewTrip

	err = result.Scan(&newTrip.Id)

	if err != nil {
		log.Fatal("Error while retrieving last id: ", err.Error())
	}

	serializedTrip, _ := json.Marshal(newTrip)

	fmt.Fprintf(w, string(serializedTrip))
}

type NewTrip struct {
	Id string
}
