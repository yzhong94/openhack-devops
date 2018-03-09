package openHackDevOps

type TripPoint struct {
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
	VIN                          string
	HasOBDData                   bool
	HasSimulatedOBDData          bool
}
