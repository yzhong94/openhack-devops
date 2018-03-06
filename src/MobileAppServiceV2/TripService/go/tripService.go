package openHackDevOps

import (
	"database/sql"
	"flag"
	"fmt"
	"log"
	"net/http"

	_ "github.com/denisenkom/go-mssqldb"
)

var (
	debug         = flag.Bool("debug", false, "enable debugging")
	password      = flag.String("password", "@windowsPhone10", "the database password")
	port     *int = flag.Int("port", 1433, "the database port")
	server        = flag.String("server", "mydrivingdbserver-fxw5u47lzepqy.database.windows.net", "the database server")
	user          = flag.String("user", "YourUserName", "the database user")
	database      = flag.String("d", "myDrivingDB", "db_name")
)

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

	// query := fmt.Sprintf("SELECT Id, Name, RecordedTimeStamp FROM Trips WHERE UserId LIKE '%%s'", userId)

	query := "SELECT Id, Name, RecordedTimeStamp FROM Trips WHERE UserId LIKE '%" + userId + "'"

	statement, err := conn.Prepare(query)

	if err != nil {
		log.Fatal("Failed to create the statement query: ", err.Error())
	}

	defer statement.Close()

	var (
		ID                string
		Name              string
		RecordedTimeStamp string
	)

	row := statement.QueryRow()

	err = row.Scan(&ID, &Name, &RecordedTimeStamp)

	if err != nil {
		log.Fatal("Scan failed: ", err.Error())
	}

	returnMessage := fmt.Sprintf("The Id is %s", ID)

	fmt.Fprintf(w, returnMessage)

	// fmt.Fprintf(w, userId)
}
