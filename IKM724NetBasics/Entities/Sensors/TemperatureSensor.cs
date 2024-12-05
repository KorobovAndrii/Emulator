using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKM724NetBasics.Entities.Sensors
{
    public class TemperatureSensor : Sensor
    {
        public TemperatureSensor(string name, string description, string apiBaseUrl)
            : base(name, description, apiBaseUrl) { }
        
        public override string GenerateValue()
        {
            var randomTemperature = new Random().NextDouble() * 50;
            return randomTemperature.ToString("F2");
        }
    }
}