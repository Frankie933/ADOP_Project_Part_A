using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            double latitude = 59.5086798659495;
            double longitude = 18.2654625932976;

            Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

            //Your Code to present each forecast item in a grouped list
            Console.WriteLine($"Weather forecast for {forecast.City}");
            
          

            var GroupForecast = forecast.Items.GroupBy(item => item.DateTime.Date);


            foreach (var group in GroupForecast)
            {


                Console.WriteLine($"{group.Key:d}");

                foreach (var i in group)
                {

                    Console.WriteLine($"Time: {i.DateTime:t}, Temperature: {i.Temperature}, Description: {i.Description}. Wind: {i.WindSpeed}m/s");
                }



            }
        


        }
    }
}
