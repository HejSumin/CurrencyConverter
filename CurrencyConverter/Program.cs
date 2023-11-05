using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Currency Converter");

        Console.Write("Enter the amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount entered.");
            return;
        }

        Console.Write("Enter the source currency code (e.g., USD): ");
        string sourceCurrency = Console.ReadLine().ToUpper();

        Console.Write("Enter the target currency code (e.g., EUR): ");
        string targetCurrency = Console.ReadLine().ToUpper();

        decimal convertedAmount = await ConvertCurrency(amount, sourceCurrency, targetCurrency);

        Console.WriteLine($"{amount} {sourceCurrency} is equivalent to {convertedAmount} {targetCurrency}");
    }

    static async Task<decimal> ConvertCurrency(decimal amount, string sourceCurrency, string targetCurrency)
    {
        using (HttpClient client = new HttpClient())
        {
            string apiKey = "YOUR_API_KEY";
            string apiUrl = $"https://api.apilayer.com/exchangerates_data/latest?base={sourceCurrency}&symbols={targetCurrency}";

            client.DefaultRequestHeaders.Add("User-Agent", "CurrencyConverter");

            string responseString = await client.GetStringAsync(apiUrl);

            var response = JsonSerializer.Deserialize<ExchangeRateResponse>(responseString);

            if (response != null && response.Success)
            {
                if (response.Rates.TryGetValue(targetCurrency, out decimal rate))
                {
                    return amount * rate;
                }
                else
                {
                    Console.WriteLine("Invalid target currency code.");
                }
            }
            else
            {
                Console.WriteLine("Failed to retrieve exchange rates.");
            }
        }

        return 0;
    }
}

class ExchangeRateResponse
{
    public bool Success { get; set; }
    public DateTime Timestamp { get; set; }
    public string Base { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
}