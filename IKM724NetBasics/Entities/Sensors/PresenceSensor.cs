using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKM724NetBasics.Entities.Sensors
{
    public class PresenceSensor : Sensor
    {
        public PresenceSensor(string name, string description, string apiBaseUrl)
            : base (name, description, apiBaseUrl){}

        public override string GenerateValue()
        {
            return new Random().Next(0, 2) == 0 ? "No" : "Yes";
        }
    }
}