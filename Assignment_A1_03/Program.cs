using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;
using Assignment_A1_03.Services;
using System.Collections;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Dynamic;

namespace Assignment_A1_03
{
    class Program
    {
        // Deklarerar en bool-egenskap (CompleteorFault) som kan användas för att ange om uppgiften har slutförts eller misslyckats
        public static bool CompleteorFault { get; set; }

        static async Task  Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();

            //Register the event
            //Your Code
          // Registrerar en händelsehanterare för WeatherForecastAvailable-händelsen från OpenWeatherService
            service.WeatherForecastAvailable += WeatherForecastAvailable;


            // Skapar en array av uppgifter (tasks) för att lagra uppgifterna för att hämta väderprognoser
            Task<Forecast>[] tasks = { null, null, null, null };
            Exception exception = null;
            try
            {

                // Ange latitud och longitud för platsen att hämta väderprognoser för
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;

                //Create the two tasks and wait for comletion
                // Skapar uppgifter för att hämta väderprognoser för den angivna platsen (Stockholm)
                tasks[0] = service.GetForecastAsync(latitude, longitude); 
                tasks[1] = service.GetForecastAsync("Miami");


               

                tasks[2] = service.GetForecastAsync(latitude, longitude);
                tasks[3] = service.GetForecastAsync("Miami");



               
                

                Forecast forecast1 = await new OpenWeatherService().GetForecastAsync(latitude, longitude);
                var forecastsort1 = forecast1.Items.GroupBy(item => item.DateTime.Date);
                Console.WriteLine($"weather forecast for {forecast1.City}");
                foreach (var item in forecastsort1)
                {
                    Console.WriteLine($" {item.Key:d}");
                    foreach (var i in item)
                    {
                        Console.WriteLine($"\t{i.DateTime:t}: {i.Description}, temperature {i.Temperature} degc, wind: {i.WindSpeed} m/s");
                    }



                }


                Forecast forecast2 = await new OpenWeatherService().GetForecastAsync("Miami");
                var forecastsort2 = forecast2.Items.GroupBy(item => item.DateTime.Date);
                Console.WriteLine($"weather forecast for {forecast2.City}");
                foreach (var item in forecastsort2)
                {
                    Console.WriteLine($" {item.Key:d}");
                    foreach (var i in item)
                    {
                        Console.WriteLine($"\t{i.DateTime:t}: {i.Description}, temperature {i.Temperature} degc, wind: {i.WindSpeed} m/s");
                    }



                }
                // Väntar på att alla uppgifter ska slutföras (för Stockholm och Miami)
                Task.WaitAll(tasks[0], tasks[1]);

                // Hämtar och behandlar väderprognosen för Stockholm igen
                Forecast forecast3 = await new OpenWeatherService().GetForecastAsync(latitude, longitude);
                var forecastsort3 = forecast3.Items.GroupBy(item => item.DateTime.Date);
                Console.WriteLine($"weather forecast for {forecast3.City}");
                foreach (var item in forecastsort3)
                {
                    Console.WriteLine($" {item.Key:d}");
                    foreach (var i in item)
                    {
                        Console.WriteLine($"\t{i.DateTime:t}: {i.Description}, temperature {i.Temperature} degc, wind: {i.WindSpeed} m/s");
                    }



                }

                // Hämtar och behandlar väderprognosen för Miami igen
                Forecast forecast4 = await new OpenWeatherService().GetForecastAsync("Miami");
                var forecastsort4 = forecast4.Items.GroupBy(item => item.DateTime.Date);
                Console.WriteLine($"weather forecast for {forecast4.City}");
                foreach (var item in forecastsort4)
                {
                    Console.WriteLine($" {item.Key:d}");
                    foreach (var i in item)
                    {
                        Console.WriteLine($"\t{i.DateTime:t}: {i.Description}, temperature {i.Temperature} degc, wind: {i.WindSpeed} m/s");
                    }



                }


                // Väntar på att de återstående uppgifterna ska slutföras (för Stockholm och Miami)
                Task.WaitAll(tasks[2], tasks[3]);










            }
            catch (Exception ex)
            {
                exception = ex;
               

                Console.WriteLine($"City weather service error!");
           
                CompleteorFault = true;

            }
            
            foreach (var task in tasks)
            {


                if (CompleteorFault == false)
                {
                    Console.WriteLine("Program success");
                    return;

                }
                else if (CompleteorFault == true)
                {

                    Console.WriteLine("error: " + exception.Message);
                    return;
                }



            }
                

        }

        static public void WeatherForecastAvailable(object sender, string message)
        {
            Console.WriteLine($"Event message from weather service {message}");
        }
        
       
       

    }

}

