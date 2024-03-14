using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_01.Models;

namespace Assignment_A1_01.Services
{
    public class OpenWeatherService

    {
        // Skapar en ny instans av HttpClient-klassen.
        // HttpClient är en klass som används för att skicka HTTP-requests och ta emot HTTP-responses från en webbserver.
        // Det används ofta för att göra HTTP-begäranden i en klientapplikation, som till

        HttpClient httpClient = new HttpClient();

        // Your API Key
        readonly string apiKey = "2c4b136042f0d448c152ee1a9ea61886"; 

        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            //Read the response from the WebApi
            HttpResponseMessage response = await httpClient.GetAsync(uri); 
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            //Convert WeatherApiData to Forecast using Linq.
            //Your code


            var forecast = new Forecast
            {
                // Tilldelar värdet av stadsnamnet från wd till egenskapen "City" i den nya Forecast-instansen
                City = wd.city.name,

                // Fyller egenskapen "Items" i den nya Forecast-instansen med en lista av "ForecastItem"-instanser
                // Denna lista skapas genom att använda Select-metoden på listan wd.list och för varje element i listan skapas en ny ForecastItem-instans
                // Denna nya instans fylls med data från det aktuella elementet i wd.list
                // Sedan konverteras resultatet av Select-metoden till en lista med ToList-metoden
                Items = wd.list.Select(item => new ForecastItem
                {
                    // Konverterar tidsstämpeln (dt) från Unix-tid till DateTime och tilldelar resultatet till "DateTime" i den nya ForecastItem-instansen
                    DateTime = UnixTimeStampToDateTime(item.dt),

                    // Tilldelar temperaturen från det aktuella elementet i wd.list till "Temperature" i den nya ForecastItem-instansen
                    Temperature = item.main.temp,

                    // Tilldelar vindhastigheten från det aktuella elementet i wd.list till "WindSpeed" i den nya ForecastItem-instansen
                    WindSpeed = item.wind.speed,

                    // Tilldelar väderbeskrivningen från det första väderobjektet (weather) i det aktuella elementet i wd.list till "Description" i den nya ForecastItem-instansen
                    Description = item.weather.First().description,
                }).ToList() // Konverterar resultatet av Select-metoden till en lista
            };

            // Returnerar den nya Forecast-instansen som resultatet av funktionen
            return forecast;






            //Hint: you will find 
            //City: wd.city.name
            //Daily forecast in wd.list, in an item in the list
            //      Date and time in Unix timestamp: dt 
            //      Temperature: main.temp
            //      WindSpeed: wind.speed
            //      Description:  first item in weather[].description
            //      Icon:  $"http://openweathermap.org/img/w/{wdle.weather.First().icon}.png"   //NOTE: Not necessary, only if you like to use an icon

            //var forecast = new Forecast(); //dummy to compile, replaced by your own code
            //return forecast;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
