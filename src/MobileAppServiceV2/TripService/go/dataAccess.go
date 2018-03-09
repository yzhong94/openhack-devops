package openHackDevOps

import (
	"database/sql"
	"encoding/json"
	"flag"
	"fmt"
)

var (
	debug    = flag.Bool("debug", false, "enable debugging")
	password = flag.String("password", "MyComplex-Passw0rd", "the database password")
	port     = flag.Int("port", 1433, "the database port")
	server   = flag.String("server", "mydrivingdbserver-vpwupcazgfita.database.windows.net", "the database server")
	user     = flag.String("user", "YourUserName", "the database user")
	database = flag.String("d", "myDrivingDB", "db_name")
)

func ExecuteNonQuery(query string) (string, error) {
	connString := fmt.Sprintf("server=%s;database=%s;user id=%s;password=%s;port=%d", *server, *database, *user, *password, *port)

	if *debug {
		fmt.Printf("connString:%s\n", connString)
	}

	conn, err := sql.Open("mssql", connString)

	if err != nil {
		return "", err
		// log.Fatal("Failed to connect to the database: ", err.Error())
	}

	defer conn.Close()

	statement, err := conn.Prepare(query)

	if err != nil {
		return "", err
		// log.Fatal("Failed to query a trip: ", err.Error())
	}

	defer statement.Close()

	result, err := statement.Query()

	if err != nil {
		return "", err
		// log.Fatal("Error while running the query: ", err.Error())
	}

	serializedResult, _ := json.Marshal(result)

	return string(serializedResult), nil
}

func ExecuteQuery(query string) (*sql.Rows, error) {
	connString := fmt.Sprintf("server=%s;database=%s;user id=%s;password=%s;port=%d", *server, *database, *user, *password, *port)

	if *debug {
		fmt.Printf("connString:%s\n", connString)
	}

	conn, err := sql.Open("mssql", connString)

	if err != nil {
		return nil, err
		// log.Fatal("Failed to connect to the database: ", err.Error())
	}

	defer conn.Close()

	statement, err := conn.Prepare(query)

	if err != nil {
		return nil, err
		// log.Fatal("Failed to query a trip: ", err.Error())
	}

	defer statement.Close()

	rows, err := statement.Query()

	if err != nil {
		return nil, err
		// log.Fatal("Error while running the query: ", err.Error())
	}

	return rows, nil
}

func FirstOrDefault(query string) (*sql.Row, error) {
	connString := fmt.Sprintf("server=%s;database=%s;user id=%s;password=%s;port=%d", *server, *database, *user, *password, *port)

	if *debug {
		fmt.Printf("connString:%s\n", connString)
	}

	conn, err := sql.Open("mssql", connString)

	if err != nil {
		return nil, err
		// log.Fatal("Failed to connect to the database: ", err.Error())
	}

	defer conn.Close()

	statement, err := conn.Prepare(query)

	if err != nil {
		return nil, err
		// log.Fatal("Failed to query a trip: ", err.Error())
	}

	defer statement.Close()

	row := statement.QueryRow()

	return row, nil
}
