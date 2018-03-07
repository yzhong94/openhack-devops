package openHackDevOps

import (
	"database/sql"
	"encoding/json"
	"flag"
	"fmt"
	"log"
	"net/http"

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
	HardStops           float32
	HardAccelerations   float32
	MainPhotoUrl        string
	Distance            float64
}

var (
	debug         = flag.Bool("debug", false, "enable debugging")
	password      = flag.String("password", "@windowsPhone10", "the database password")
	port     *int = flag.Int("port", 1433, "the database port")
	server        = flag.String("server", "mydrivingdbserver-fxw5u47lzepqy.database.windows.net", "the database server")
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

	query := "SELECT Id, Name, UserId, RecordedTimeStamp, EndTimeStamp, Rating, IsComplete, HasSimulatedOBDData, AverageSpeed, FuelUsed, HardStops, HardAccelerations, MainPhotoUrl, Distance FROM Trips WHERE UserId LIKE '%" + userId + "'"

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
