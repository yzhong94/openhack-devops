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
    using System.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.EnvironmentVariables;

    public class Program
    {

        //Setup Device Connection Information
        private static  string IotHubUri = "mydriving-vpwupcazgfita.azure-devices.net";
        private static string DeviceKey = "IimjJsuK6H19V1iPTbRfGT5afcDRKZhVT89W4c0xFBg=";//"JYalviYVlMt6+jXkgJgyuN3exevWjbYbTrxNevCJsV4=";
        private static string DeviceId = "B6E360B7924542EF8611C648772163DB";//"MyDriving-DevOpsSim1";
        private static string AzureMobileServiceUrl = "https://mydriving-vpwupcazgfita.azurewebsites.net";
        private static DirectoryInfo fileDirectory;

        //IOTHUB Vars
        private static DeviceClient _deviceClient;
        //private static int _messageId = 1;

        //Service Configuration to Different Services Requried
        static IStoreManager _storeManager;
        public static IStoreManager StoreManager => _storeManager ?? (_storeManager = ServiceLocator.Instance.Resolve<IStoreManager>());
        public Settings Settings => Settings.Current;


        private static void Main(string[] args)
        {
            IConfiguration funcConfiguration;
            var builder = new ConfigurationBuilder().AddEnvironmentVariables();
            funcConfiguration = builder.Build();
            //Environmental Variables to be Passed to Container
            IotHubUri = funcConfiguration.GetSection("IOT_HUB_URI").Value ?? ("mydriving-vpwupcazgfita.azure-devices.net");
            DeviceKey = funcConfiguration.GetSection("DEVICE_KEY").Value ?? ("IimjJsuK6H19V1iPTbRfGT5afcDRKZhVT89W4c0xFBg=");// ("JYalviYVlMt6 +jXkgJgyuN3exevWjbYbTrxNevCJsV4=");
            DeviceId = funcConfiguration.GetSection("DEVICE_ID").Value ?? ("B6E360B7924542EF8611C648772163DB");// ("MyDriving -DevOpsSim1");
            AzureMobileServiceUrl = funcConfiguration.GetSection("AZURE_MOBILE_SERVICE").Value ?? ("https://mydriving-vpwupcazgfita.azurewebsites.net");
            fileDirectory = new DirectoryInfo(funcConfiguration.GetSection("FILE_VOLUME").Value ?? (@"C:\Users\brents\source\repos\openhack-devops\src\DeviceSim\SimulatedDevice\TripFiles\"));



            InitializeServices();
            StartSimulator();
            //TODO: Add Authentication
            //LoginViewController login = new LoginViewController();
            //login.AttempLogin();



            SendDeviceToCloudMessagesAsync();
            //Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();


            Console.ReadLine();
        }

        private static void StartSimulator()
        {
            Console.WriteLine($"Simulated device : {DeviceId} \n Starting Trip Broadcast");
            _deviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey), TransportType.Mqtt);
            _deviceClient.ProductInfo = "CarTripInfo - Simulator";
        }

        private static void InitializeServices()
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

        private static async void SendDeviceToCloudMessagesAsync()
        {
            foreach (var fileToProc in fileDirectory.GetFiles())
            {
            Console.WriteLine($"Start Processing File : {fileToProc.FullName}");
            
            // AzureClient.AzureClient.CheckIsAuthTokenValid();
            StreamReader file = new StreamReader(fileToProc.FullName);

            List<string[]> _toProcess = new List<string[]>();
            string line;
            Trip trip = new Trip();
            List<TripPoint> _tripPoints = new List<TripPoint>();

            //Pick up File and strip out useable content
            while ((line = file.ReadLine()) != null)
            {
                if ((line != "") && (!line.Contains("tripid"))) { _toProcess.Add(line.Split(',')); }
               
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
                //await Task.Delay(100);

            }

            trip.EndTimeStamp = trip.Points.Last<TripPoint>().RecordedTimeStamp;

            Console.WriteLine($"Finished Processing File : {fileToProc.FullName} at {trip.EndTimeStamp}");
            
            //await AzureClient.AzureClient.CheckIsAuthTokenValid();

            //await StoreManager.TripStore.InsertAsync(trip);
            }

        }

        private static void UpdateTripPointTimeStamps(Trip trip)
        {
            //Sort Trip Points By Sequence Number
            trip.Points = trip.Points.OrderBy(p => p.Sequence).ToList();

            List<timeInfo> timeToAdd = new List<timeInfo>();
            System.TimeSpan tDiff;

            //Create a Variable to Track the Time Range as it Changes
            System.DateTime runningTime = trip.RecordedTimeStamp;


            //Calculate the Difference in time between Each Sequence Item 
            for (int currentTripPoint = (trip.Points.Count - 1); currentTripPoint > -1; currentTripPoint--)
            {
                if (currentTripPoint > 0)
                {
                    tDiff = trip.Points[currentTripPoint].RecordedTimeStamp - trip.Points[currentTripPoint - 1].RecordedTimeStamp;
                    timeToAdd.Add(new timeInfo() { evtSeq = trip.Points[currentTripPoint].Sequence, tSpan = tDiff });
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

    public struct timeInfo
    {
        public int evtSeq;
        public TimeSpan tSpan;
    }
    public class LoginController
    {

        LoginModel viewModel;
        public async void AttempLogin()
        {
            viewModel = new LoginModel();
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
                await Services.OBDDataProcessor.GetProcessor().Initialize(ViewModel.ModelBase.StoreManager);

                //NavigateToTabs();
            }
        }

    }
}
