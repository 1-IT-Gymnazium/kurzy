using System;
using System.Net;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

/* 
########################################################

Funkcionalitu celeho kodu popisuju v readme.md na gitu
Autor: Tomas F. /1102/

########################################################
*/


namespace ProduktDataDoCSV
{
    public class Program
    {
        static void Main(string[] args)
        {
            DateTime userInputDate;
            DateTime exchangeRateDate;

            while (true)
            {
                Console.WriteLine("Zadejte datum pro kurzovní lístek (DD.MM.YYYY) nebo 'exit' pro ukončení:");
                string inputDate = Console.ReadLine();

                if (inputDate.ToLower() == "exit")
                {
                    break;
                }

                if (TryParseInputDate(inputDate, out userInputDate, out exchangeRateDate))
                {
                    double usdToCzkRate = FetchExchangeRate(exchangeRateDate);

                    if (usdToCzkRate == 0)
                    {
                        Console.WriteLine("Neplatné datum nebo nelze získat kurzovní lístek. Zkuste to prosím znovu.");
                        continue;
                    }

                    // vypis kurzu CZK-USD ze dne ktery zada uzivatel
                    Console.WriteLine($"Kurz CZK-USD k datu {exchangeRateDate:dd.MM.yyyy} je: 1 USD = {usdToCzkRate:N0} CZK");

                    using (SqlConnection conn = new SqlConnection("Server=stbechyn-sql.database.windows.net;Database=AdventureWorksDW2020;User Id=prvniit;Password=P@ssW0rd!;"))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT EnglishProductName, DealerPrice FROM DimProduct", conn))
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            try
                            {
                                using (StreamWriter writer = new StreamWriter($"{exchangeRateDate:dd.MM.yyyy}_adventureworks.csv"))
                                {
                                    writer.WriteLine("Date;ProductName;PriceUSD;PriceCZK");

                                    while (reader.Read())
                                    {
                                        string productName = reader.GetString(0);
                                        decimal priceUSD = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                                        decimal priceCZK = priceUSD * (decimal)usdToCzkRate;
                                        writer.WriteLine($"{exchangeRateDate:dd.MM.yyyy};{productName};{priceUSD};{priceCZK}");
                                    }
                                }
                                Console.WriteLine($"Data byla úspěšně zapsána do souboru: {exchangeRateDate:dd.MM.yyyy}_adventureworks.csv");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Chyba při psaní do souboru: {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Neplatné datum. Zkuste to prosím znovu.");
                }
            }
        }

        static bool TryParseInputDate(string dateStr, out DateTime userInputDate, out DateTime exchangeRateDate)
        {
            if (!DateTime.TryParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out userInputDate) || userInputDate > DateTime.Today)
            {
                userInputDate = DateTime.MinValue;
                exchangeRateDate = DateTime.MinValue;
                return false;
            }

            exchangeRateDate = userInputDate;
            switch (exchangeRateDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    exchangeRateDate = exchangeRateDate.AddDays(-1);
                    break;
                case DayOfWeek.Sunday:
                    exchangeRateDate = exchangeRateDate.AddDays(-2);
                    break;
            }

            return true;
        }

        static double FetchExchangeRate(DateTime date)
        {
            WebClient client = new WebClient();
            string data = client.DownloadString($"https://www.cnb.cz/cs/financni-trhy/devizovy-trh/kurzy-devizoveho-trhu/kurzy-devizoveho-trhu/rok.txt?rok={date.Year}");
            string[] lines = data.Split('\n');
            string[] header = lines[0].Split('|');
            double usdToCzkRate = 0;

            foreach (string line in lines)
            {
                string[] columns = line.Split('|');
                if (columns[0] == date.ToString("dd.MM.yyyy"))
                {
                    if (double.TryParse(columns[Array.IndexOf(header, "1 USD")], out usdToCzkRate))
                    {
                        break;
                    }
                }
            }

            return usdToCzkRate;
        }
    }
}
