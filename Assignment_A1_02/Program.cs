using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_02.Models;
using Assignment_A1_02.Services;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Diagnostics;
using System.ComponentModel.Design;

namespace Assignment_A1_02
{
    class Program
    {
        public static bool CompleteorFault { get; set; }
        static async Task Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();


            //Register the event
            service.WeatherForecastAvailable += WeatherForecastAvailable;
            
            //Your Code

            Task<Forecast>[] tasks = { null, null };
            Exception exception = null;
            try
            {

                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;

                //Create the two tasks and wait for comletion
                tasks[0] = service.GetForecastAsync(latitude, longitude);
                tasks[1] = service.GetForecastAsync("elvis");
       

                Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);
                Console.WriteLine($"Weather forecast for {forecast.City}");
                var gruppList = forecast.Items.GroupBy(item => item.DateTime.Date);
                foreach (var item in gruppList)
                {
                    Console.WriteLine($" {item.Key:d}");
                    foreach (var i in item)
                    {
                        Console.WriteLine($"\t{i.DateTime:t}: {i.Description}, Temperature {i.Temperature} Degc, Wind: {i.WindSpeed} m/s");
                    }

                }

                Forecast forecast1 = await new OpenWeatherService().GetForecastAsync("elvis");
                Console.WriteLine("_________________________");
                Console.WriteLine($"Weather forecast for {forecast1.City}");
                var gruppList1 = forecast1.Items.GroupBy(item => item.DateTime.Date);

                foreach (var item in gruppList1)
                {
                    Console.WriteLine($" {item.Key:d}");
                    foreach (var i in item)
                    {
                        Console.WriteLine($"\t{i.DateTime:t}: {i.Description}, Temperature {i.Temperature}, Wind: {i.WindSpeed} m/s");
                    }
                }



                Task.WaitAll(tasks[0], tasks[1]);


            }
            catch (Exception ex)
            {
                exception = ex;
                //How to handle an exception

                Console.WriteLine($"City weather service error!");
                Console.WriteLine($"{exception.Message}");

                    
                CompleteorFault = true; 
        

            }

            foreach (var task in tasks)
            {
               

                if (CompleteorFault == false)
                {
                    Console.WriteLine("Program success");
                    return;

                }


               
            }

        
        }
        //Event handler declaration
        static public void WeatherForecastAvailable(object sender, string message)
        {
            Console.WriteLine($"Event message from weather service {message}");
        }

    }

}      




 