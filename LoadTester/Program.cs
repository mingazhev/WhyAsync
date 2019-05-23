using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoadTester
{
  class Program
  {
    static void Main(string[] args)
    {
      while (true)
      {
        Console.WriteLine("Type 'S' for sync way, 'A' for async way. Type 'E' for exit");
        var choose = Console.ReadLine();
        string apiMethod = string.Empty;
        if (choose.Equals("s", StringComparison.OrdinalIgnoreCase))
          apiMethod = "sync";
        else if (choose.Equals("a", StringComparison.OrdinalIgnoreCase))
          apiMethod = "async";
        else if(choose.Equals("e", StringComparison.OrdinalIgnoreCase))
          return;
        else
          continue;


        var methodUrl = $"http://localhost:5000/api/values/{apiMethod}";
        Console.WriteLine($"Executing {methodUrl}");

        // ServicePointManager.DefaultConnectionLimit = 200;

        var requestCount = 50;
        using (var httpClient = new HttpClient())
        {
          httpClient.Timeout = TimeSpan.FromMinutes(2);

          Parallel.For(0, requestCount, ((i, state) =>
          {
            httpClient.GetAsync(methodUrl);
          }));

          var sw = Stopwatch.StartNew();
          Console.WriteLine($"{requestCount} requests created. Waiting server for respond");

          // Waiting server to serve another request
          var isAlive = httpClient.GetAsync($"http://localhost:5000/api/values/isresponding").Result.Content;
          Console.WriteLine($"{sw.Elapsed.TotalSeconds} sec.");

          Console.WriteLine("Finished. Type any key to continue");
          Console.ReadKey();
        }
      }
    }
  }
}
