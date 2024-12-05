using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IKM724NetBasics.Entities.Sensors;
using IKM724NetBasics.Entities;

namespace IKM724NetBasics
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var apiBaseUrl = "https://localhost:7045/";

            var sensors = await LoadSensorsFromServer(apiBaseUrl);

            Console.WriteLine("Starting Emulator...");

            while (true)
            {
                var updatedSensors = await LoadSensorsFromServer(apiBaseUrl);

                foreach (var sensor in updatedSensors)
                {
                    if (!sensors.Any(s => s.Id == sensor.Id))
                    {
                        sensors.Add(sensor);
                        Console.WriteLine($"New sensor added: {sensor.Name} ({sensor.Id})");
                    }
                }

                foreach (var sensor in sensors)
                {
                    await sensor.SendValueAsync();
                }

                await Task.Delay(1000);
            }
        }

        private static async Task<List<Sensor>> LoadSensorsFromServer(string apiBaseUrl)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };

            var response = await httpClient.GetAsync("api/Indicator");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Server response: " + responseContent);

            if (!responseContent.Contains("\"type\""))
            {
                Console.WriteLine("Warning: Server response does not contain 'type' field.");
            }

            var indicators = JsonSerializer.Deserialize<List<IndicatorModel>>(responseContent);

            if (indicators == null || indicators.Count == 0)
            {
                Console.WriteLine("No indicators found.");
                return new List<Sensor>();
            }

            foreach (var indicator in indicators)
            {
                Console.WriteLine($"Indicator: Name={indicator.Name}, ID={indicator.Id}, Type={indicator.Type}");
            }

            var sensors = new List<Sensor>();
            foreach (var indicator in indicators)
            {
                Console.WriteLine($"Loaded sensor: {indicator.Name}, ID: {indicator.Id}, Type: {indicator.Type}");
                Console.WriteLine($"Raw Type Value: '{indicator.Type}' (Length: {indicator.Type?.Length ?? 0})");

                if (string.IsNullOrWhiteSpace(indicator.Type))
                {
                    Console.WriteLine($"Invalid or empty Type for indicator: {indicator.Name}");
                    continue;
                }

                Sensor sensor = indicator.Type.Trim().ToLowerInvariant() switch
                {
                    "temperature" => new TemperatureSensor(indicator.Name ?? "Unnamed Sensor", indicator.Description ?? "No description provided", apiBaseUrl),
                    "presence" => new PresenceSensor(indicator.Name ?? "Unnamed Sensor", indicator.Description ?? "No description provided", apiBaseUrl),
                    _ => throw new Exception($"Unknown indicator type: {indicator.Type}")
                };

                sensor.Id = indicator.Id;
                sensors.Add(sensor);
            }

            return sensors;
        }
    }
}
