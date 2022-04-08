using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace convert
{
    
    class Program
    {

        public static string targetCurencyValue;
        public static string exchaneUrl;


        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("Invalid args");
                return;
            }

            
            for (int i = 0; i < args.Count(); i++)
            {
                if (!TestArgument(args[i]))
                {
                    return;
                }
            }


            var baseCurency = args[0];
            var targetCurency = args[1];


            Root myDeserializedClass = GetCurrency(baseCurency);

            if (myDeserializedClass != null)
            {
                foreach (PropertyInfo prop in typeof(Rates).GetProperties())
                {
                    if (prop.Name == targetCurency)
                    {
                        targetCurencyValue = prop.GetValue(myDeserializedClass.rates, null).ToString();
                        break;
                    }

                }


                if (targetCurencyValue != "")
                {
                    Console.WriteLine();
                    Console.WriteLine("********************************************");
                    Console.WriteLine($"Course {myDeserializedClass.base_code} to {targetCurency} = {targetCurencyValue}");
                    Console.WriteLine("Update on : " + myDeserializedClass.time_last_update_utc.ToLocalTime());
                    Console.WriteLine("********************************************");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"Error : Target Currency = {targetCurency} not exist.");
                }

            }
            else
            {
                Console.WriteLine($"Error : Base Currency = {baseCurency} not exist. ");
            }
            
            

        }



        /// <summary>
        /// Ziskaj Konverzny list
        /// </summary>
        /// <param name="_baseCurency"></param>
        /// <returns>Trieda Root</returns>
        public static Root GetCurrency(string _baseCurency)
        {

            try
            {
                exchaneUrl = @"https://open.er-api.com/v6/latest/" + _baseCurency;
                var json = new WebClient().DownloadString(exchaneUrl);


                var jsonData = JObject.Parse(json);


                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);

                if (myDeserializedClass.result == "success")
                {
                    return myDeserializedClass;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Missing internet connection ? " + e);
                return null;
                
            }

        }




        static bool TestArgument(string input)
        {
            string pattern = @"[A-Z]{3}";
            Regex rg = new Regex(pattern);

            if (!rg.IsMatch(input))
            {
                Console.WriteLine("Invalid args");
                return false;
            }

            return true;

        }
    }
}

