package openHackDevOps

import (
	"encoding/json"
)

func SerializeError(e error) string {
	var errorMessage struct {
		Message string
	}

	errorMessage.Message = e.Error()

	serializedError, _ := json.Marshal(errorMessage)

	return string(serializedError)
}
