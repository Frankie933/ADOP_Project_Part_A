using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;

namespace Assignment_A1_03.Services
{
    public class OpenWeatherService
    {
        // Skapar en ny instans av HttpClient för att göra HTTP-anrop till en webbserver.
        HttpClient httpClient = new HttpClient();

        // Deklarerar en samling av cache-objekt för geografiska väderprognoser med hjälp av en ConcurrentDictionary.
        ConcurrentDictionary<(double, double, string), Forecast> cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), Forecast>();

        // Deklarerar en samling av cache-objekt för stadsväderprognoser med hjälp av en ConcurrentDictionary.
        ConcurrentDictionary<(string, string), Forecast> cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();

        // Deklarerar en sträng för API-nyckeln som används för att göra anrop till OpenWeatherMap API.
        readonly string apiKey = "2c4b136042f0d448c152ee1a9ea61886";

        // Deklarerar ett händelseobjekt för att indikera att en ny väderprognos är tillgänglig.
        public event EventHandler<string> WeatherForecastAvailable;

        // Metod för att utlösa händelsehanteraren när en ny väderprognos är tillgänglig.
        protected virtual void OnWeatherForecastAvailable(string message)
        {
            WeatherForecastAvailable?.Invoke(this, message);
        }

        // Metod för att hämta en väderprognos för en given stad asynkront.
        public async Task<Forecast> GetForecastAsync(string City)
        {
            // Variabel för att lagra väderprognosen.
            Forecast forecast;

            // Hämtar aktuellt datum och tid.
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            // Kontrollerar om en cache för stadsväderprognosen finns och om den är giltig.
            if (cachedCityForecasts.TryGetValue((City, date), out forecast))
            {
                // Om det finns en giltig cache, utlöses händelsen för att indikera att en ny väderprognos är tillgänglig.
                OnWeatherForecastAvailable($"New Message from weather service: New Cached weather forecast for {City} available");
                return forecast;
            }

            // Om ingen giltig cache finns, försöker vi hämta väderprognosen från OpenWeatherMap API.

            // Lägger till väderprognosen i cachen med aktuellt datum och tid.
            cachedCityForecasts[(City, DateTime.Now.ToString(date))] = forecast;

            // Utlöser händelsen för att indikera att en ny väderprognos är tillgänglig.
            OnWeatherForecastAvailable($"New message from weather service: New weather forecast for {City} available");

            // Skapar URL för att göra en förfrågan till OpenWeatherMap API med angiven stad.
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            // Hämtar väderprognosen från OpenWeatherMap API.
            forecast = await ReadWebApiAsync(uri);

            // Returnerar väderprognosen.
            return forecast;
        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            Forecast forecast;
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            if (cachedGeoForecasts.TryGetValue((latitude, longitude, date), out  forecast))
            {
                OnWeatherForecastAvailable($"New Message from weather service: New Cached weather forecast for {latitude},{longitude} avialible");
                return forecast;


            }

           cachedGeoForecasts[(latitude, longitude, DateTime.Now.ToString(date))] = forecast;  
           OnWeatherForecastAvailable($"New message from weather service: New weather forecast for {latitude},{longitude} available");


            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            forecast = await ReadWebApiAsync(uri);



            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            //Read the response from the WebApi
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();


            //Convert WeatherApiData to Forecast using Linq.
            //Your code
            var forecast = new Forecast()
            {
                City = wd.city.name,
                Items = wd.list.Select(item => 
                
                
                
                new ForecastItem
                {
                    DateTime = UnixTimeStampToDateTime(item.dt),
                    Temperature = item.main.temp,
                    WindSpeed = item.wind.speed,
                    Description = item.weather.First().description,

                }).ToList()

            };
            return forecast;


     
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0,  DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}


