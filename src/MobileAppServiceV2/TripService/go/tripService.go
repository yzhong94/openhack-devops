package openHackDevOps

import (
	"fmt"
	"net/http"
)

func GetAllTrips(w http.ResponseWriter, r *http.Request) {
	fmt.Fprintf(w, r.URL.RawQuery)
}
