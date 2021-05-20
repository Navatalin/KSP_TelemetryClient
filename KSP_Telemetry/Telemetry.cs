using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;


namespace KSP_Telemetry
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Telemetry : MonoBehaviour
    {
        private DateTime startTime;
        private NetworkClient _networkClient;
        private HashSet<TelemtryResource> telemtryResources;
        
        public void Start()
        {
            Debug.Log("Starting Telemetry");
            _networkClient = new NetworkClient();
            _networkClient.Connect();
            startTime = DateTime.Now;
            telemtryResources = new HashSet<TelemtryResource>();

            
        }
        public void FixedUpdate()
        {
            var vessel = FlightGlobals.ActiveVessel;
            var altitude = vessel.altitude;
            var velocity = vessel.speed;
            var accel = vessel.acceleration.magnitude;
            var latitude = vessel.latitude;
            var longitude = vessel.longitude;
            var dynamicPressure = vessel.dynamicPressurekPa;
            var staticPressure = vessel.staticPressurekPa;


            TelemtryMessage message = new TelemtryMessage();
            message.Altitude = altitude;
            message.Velocity = velocity;
            message.Acceleration = accel;
            message.Latitude = latitude;
            message.Longitude = longitude;
            message.DynamicPressure = dynamicPressure;
            message.StaticPressure = staticPressure;


            try
            {
                var delay = message.Altitude / (3 * Mathf.Pow(10,8));
                message.Delay = delay;
                if ((DateTime.Now - startTime).Seconds > delay)
                {
                    SendData(message);
                    startTime = DateTime.Now;
                }
            }
            catch(Exception ex)
            {
                Debug.Log("KSP Telemetry Error: " + ex.Message + " Stack: " + ex.StackTrace);
            }


        }

        private void SendData(TelemtryMessage message)
        {
            var messageStr = $"alt:{message.Altitude};vel:{message.Velocity};acc:{message.Acceleration};lat:{message.Latitude};lng:{message.Longitude};dpr:{message.DynamicPressure};spr:{message.StaticPressure};";
            _networkClient.Send(messageStr);
        }
    }
    public class TelemtryMessage
    {
        public double Altitude { get; set; }
        public double Velocity { get; set; }
        public double Acceleration { get; set; }
        public List<string> Resources { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double DynamicPressure { get; set; }
        public double StaticPressure { get; set; }
        public double Delay { get; set; }
    }
    public class TelemtryResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public double maxAmount { get; set; }
    }
}
