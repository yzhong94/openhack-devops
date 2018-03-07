package openHackDevOps

import (
	"fmt"
	"net/http"

	"github.com/gorilla/mux"
)

type Route struct {
	Name        string
	Method      string
	Pattern     string
	HandlerFunc http.HandlerFunc
}

type Routes []Route

func NewRouter() *mux.Router {
	router := mux.NewRouter().StrictSlash(true)
	for _, route := range routes {
		var handler http.Handler
		handler = route.HandlerFunc
		handler = Logger(handler, route.Name)

		router.
			Methods(route.Method).
			Path(route.Pattern).
			Name(route.Name).
			Handler(handler)
	}

	return router
}

func HealthCheck(w http.ResponseWriter, r *http.Request) {
	fmt.Fprintf(w, "APIs working!")
}

var routes = Routes{
	Route{
		"HealthCheck",
		"GET",
		"/v1/HealthCheck",
		HealthCheck,
	},

	Route{
		"GetTrip",
		"GET",
		"/v1/GetTrip",
		GetTrip,
	},

	Route{
		"DeleteTrip",
		"DELETE",
		"/v1/DeleteTrip",
		DeleteTrip,
	},

	Route{
		"GetAllTrips",
		"GET",
		"/v1/GetAllTrips",
		GetAllTrips,
	},

	Route{
		"PatchTrip",
		"PATCH",
		"/v1/PatchTrip",
		PatchTrip,
	},

	Route{
		"Tenantsget",
		"GET",
		"/v1/tenants",
		Tenantsget,
	},
}
