


namespace SimulatedDevice
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Newtonsoft.Json;
    using System.IO;
    using System.Collections.Generic;

    using SimulatedDevice.Utils;
    using SimulatedDevice.DataObjects;
    using SimulatedDevice.DataStore;
    using SimulatedDevice.DataStore.Azure;
    using SimulatedDevice.DataStore.Abstractions;
    using SimulatedDevice.Services;
    using SimulatedDevice.ViewModel;
    using SimulatedDevice.Interfaces;
    using SimulatedDevice.Utils.Interfaces;
    using SimulatedDevice.Shared;
    using SimulatedDevice.Helpers;
    using SimulatedDevice.AzureClient;

    using System.Linq;

    public class Program
    {

        

        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                SimulationContext ctx = new SimulationContext(args[0], args[1], args[2], args[3]);
                ctx.SendDeviceToCloudMessagesAsync();
            }
            //Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            //TODO : How Do we Keep the Container Continuously running the code ?
           
            //Console.ReadLine();
        }

    }

    public struct TimeInfo
    {
        public int evtSeq;
        public TimeSpan tSpan;
    }
    public class LoginController
    {

        LoginViewModel viewModel;
        public async void AttempLogin()
        {
            viewModel = new LoginViewModel();
            await LoginAsync(LoginAccount.Microsoft);

            //await login.ExecuteLoginMicrosoftCommandAsync();

        }
        async Task LoginAsync(LoginAccount account)
        {
            switch (account)
            {
                case LoginAccount.Facebook:
                    await viewModel.ExecuteLoginFacebookCommandAsync();
                    break;
                case LoginAccount.Microsoft:
                    await viewModel.ExecuteLoginMicrosoftCommandAsync();
                    break;
                case LoginAccount.Twitter:
                    await viewModel.ExecuteLoginTwitterCommandAsync();
                    break;
            }

            if (viewModel.IsLoggedIn)
            {
                //When the first screen of the app is launched after user has logged in, initialize the processor that manages connection to OBD Device and to the IOT Hub
                await Services.OBDDataProcessor.GetProcessor().Initialize(ViewModel.ViewModelBase.StoreManager);

                //NavigateToTabs();
            }
        }

    }

    public class SimulationContext
    {
        //Setup Device Connection Information with some default values
        private string IotHubUri = String.Empty;                //"mydriving-vpwupcazgfita.azure-devices.net";
        private string DeviceKey = String.Empty;                //"JYalviYVlMt6+jXkgJgyuN3exevWjbYbTrxNevCJsV4=";
        private string DeviceId = String.Empty;                 //"MyDriving-DevOpsSim1";
        private string AzureMobileServiceUrl = String.Empty;    //"https://mydriving-vpwupcazgfita.azurewebsites.net";

        //IOTHUB Vars
        private static DeviceClient _deviceClient;
        private static int _messageId = 1;

        public SimulationContext(string iotHubUri, string deviceKey, string deviceId, string azureMobileServiceUrl)
        {
            this.IotHubUri = iotHubUri;
            this.DeviceKey = deviceKey;
            this.DeviceId = deviceId;
            this.AzureMobileServiceUrl = azureMobileServiceUrl;


            InitializeServices();
            StartSimulator();

            //TODO: Add Authentication
            //LoginController login = new LoginController();
            //login.AttempLogin();
        }


        //Service Configuration to Different Services Requried
        private IStoreManager _storeManager;
        public IStoreManager StoreManager => _storeManager ?? (_storeManager = ServiceLocator.Instance.Resolve<IStoreManager>());
        public Settings Settings => Settings.Current;

        private void StartSimulator()
        {
            Console.WriteLine($"Simulated device : {DeviceId} \n Starting Trip Broadcast");
            _deviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey), TransportType.Mqtt);
            _deviceClient.ProductInfo = "CarTripInfo - Simulator";
        }

        private void InitializeServices()
        {


            ServiceLocator.Instance.Add<IAuthentication, Authentication>();
            ServiceLocator.Instance.Add<ILogger, PlatformLogger>();
            ServiceLocator.Instance.Add<IAzureClient, AzureClient.AzureClient>();
            ServiceLocator.Instance.Add<ITripStore, DataStore.Azure.Stores.TripStore>();
            ServiceLocator.Instance.Add<ITripPointStore, DataStore.Azure.Stores.TripPointStore>();
            ServiceLocator.Instance.Add<IPhotoStore, DataStore.Azure.Stores.PhotoStore>();
            ServiceLocator.Instance.Add<IUserStore, DataStore.Azure.Stores.UserStore>();
            ServiceLocator.Instance.Add<IHubIOTStore, DataStore.Azure.Stores.IOTHubStore>();
            ServiceLocator.Instance.Add<IPOIStore, DataStore.Azure.Stores.POIStore>();
            ServiceLocator.Instance.Add<IStoreManager, DataStore.Azure.StoreManager>();
            //ServiceLocator.Instance.Add<IOBDDevice, OBDDevice>(); //No Need as a Simulator will not be used
        }

        public async void SendDeviceToCloudMessagesAsync()
        {
            // AzureClient.AzureClient.CheckIsAuthTokenValid();

            List<string[]> _toProcess = new List<string[]>();
            string line;
            int _counter = 0;
            Trip trip = new Trip();
            List<TripPoint> _tripPoints = new List<TripPoint>();

            //Pick up File and strip out useable content
            StreamReader file = new StreamReader(@"C:\Users\brents\source\repos\SimulatedDevice\SimulatedDevice\TripFiles\trip1.csv");

            while ((line = file.ReadLine()) != null)
            {
                if ((line != "") && (!line.Contains("tripid"))) { _toProcess.Add(line.Split(',')); }
                _counter++;
            }
            file.Close();

            //Process File Contents


            if (_toProcess.Count > 0)
            {

                trip.RecordedTimeStamp = DateTime.UtcNow;
                trip.Name = "Trip 1";
                trip.Id = Guid.NewGuid().ToString(); //Create trip ID
                trip.UserId = _toProcess[0][1]; //"MicrosoftAccount:cd3744e78c2d3d2d" //TODO: Make this so that once Authenticated we use the Login Information from the JWT Token not the file


                foreach (string[] _point in _toProcess)
                {
                    TripPoint _tripPoint = new TripPoint()
                    {
                        TripId = trip.Id,
                        Id = Guid.NewGuid().ToString(),
                        Latitude = Convert.ToDouble(_point[4]),
                        Longitude = Convert.ToDouble(_point[5]),
                        Speed = Convert.ToDouble(_point[6]),
                        RecordedTimeStamp = Convert.ToDateTime(_point[7]),
                        Sequence = Convert.ToInt32(_point[8]),
                        RPM = Convert.ToDouble(_point[9]),
                        ShortTermFuelBank = Convert.ToDouble(_point[10]),
                        LongTermFuelBank = Convert.ToDouble(_point[11]),
                        ThrottlePosition = Convert.ToDouble(_point[12]),
                        RelativeThrottlePosition = Convert.ToDouble(_point[13]),
                        Runtime = Convert.ToDouble(_point[14]),
                        DistanceWithMalfunctionLight = Convert.ToDouble(_point[16]),
                        EngineLoad = Convert.ToDouble(_point[16]),
                        MassFlowRate = Convert.ToDouble(_point[17]),
                        EngineFuelRate = Convert.ToDouble(_point[19])

                    };
                    trip.Points.Add(_tripPoint);
                }

                //Update Time Stamps to current date and times before sending to IOT Hub
                UpdateTripPointTimeStamps(trip);

            }

            //Start Streaming Trip Points to IOT Hub

            foreach (TripPoint IOTHubTripPoint in trip.Points)
            {


                var settings = new JsonSerializerSettings { ContractResolver = new CustomContractResolver() };
                var tripDataPointBlob = JsonConvert.SerializeObject(IOTHubTripPoint, settings);
                var tripBlob = JsonConvert.SerializeObject(new { TripId = IOTHubTripPoint.TripId, UserId = trip.UserId });
                tripBlob = tripBlob.TrimEnd('}');
                string packagedBlob = $"{tripBlob},\"TripDataPoint\":{tripDataPointBlob}}}";

                //Encode Message for IOTHUB
                var message = new Message(Encoding.ASCII.GetBytes(packagedBlob));
                await _deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, packagedBlob);
                //Add some delay 
                await Task.Delay(100);

            }

            trip.EndTimeStamp = trip.Points.Last<TripPoint>().RecordedTimeStamp;

            //TODO: Need to fix/ Refactor MobileService Auth Code for Console not Web/Mobile Environment
            //await StoreManager.TripStore.InsertAsync(trip);
        }

        private void UpdateTripPointTimeStamps(Trip trip)
        {
            //Sort Trip Points By Sequence Number
            trip.Points = trip.Points.OrderBy(p => p.Sequence).ToList();

            List<TimeInfo> timeToAdd = new List<TimeInfo>();
            System.TimeSpan tDiff;

            //Create a Variable to Track the Time Range as it Changes
            System.DateTime runningTime = trip.RecordedTimeStamp;


            //Calculate the Difference in time between Each Sequence Item 
            for (int currentTripPoint = (trip.Points.Count - 1); currentTripPoint > -1; currentTripPoint--)
            {
                if (currentTripPoint > 0)
                {
                    tDiff = trip.Points[currentTripPoint].RecordedTimeStamp - trip.Points[currentTripPoint - 1].RecordedTimeStamp;
                    timeToAdd.Add(new TimeInfo() { evtSeq = trip.Points[currentTripPoint].Sequence, tSpan = tDiff });
                }

            }

            //Sort List in order to Add time to Trip Points
            timeToAdd = timeToAdd.OrderBy(s => s.evtSeq).ToList();
            //Update Trip Points

            for (int currentTripPoint = 1, timeToAddCollIdx = 0; currentTripPoint < trip.Points.Count; currentTripPoint++, timeToAddCollIdx++)
            {
                runningTime = runningTime.Add(timeToAdd[timeToAddCollIdx].tSpan);
                trip.Points[currentTripPoint].RecordedTimeStamp = runningTime;
            }

            // Update Initial Trip Point
            trip.Points[0].RecordedTimeStamp = trip.RecordedTimeStamp;
        }








    }
}















//using System;

//namespace SimulatedDevice
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.WriteLine("Hello World!");
//        }
//    }
//}


