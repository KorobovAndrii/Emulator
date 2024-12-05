using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace IKM724NetBasics.Entities.Sensors
{
    public abstract class Sensor
    {
        public readonly HttpClient _httpClient;

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ApiBaseUrl { get; set; }

        protected Sensor(string name, string description, string apiBaseUrl) 
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            ApiBaseUrl = apiBaseUrl;
            _httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
        }

        public abstract string GenerateValue();

        public async Task SendValueAsync()
        {
            var value = GenerateValue();

            var payload = new
            {
                Id = Id,
                Value = value
            };

            var response = await _httpClient.PutAsJsonAsync("api/Indicator", payload);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to send value for {Name}: {response.ReasonPhrase}");
            }
            else
            {
                Console.WriteLine($"Sent value for {Name}: {value}");
            }
        }
    }
}