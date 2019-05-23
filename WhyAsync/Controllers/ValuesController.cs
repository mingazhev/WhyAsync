using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WhyAsync.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ValuesController : ControllerBase
  {
    private static Task<bool> task;

    [HttpGet("sync")]
    public IEnumerable<string> Get()
    {
      // Getting values, sync waiting for work done.
      return this.GetValues().Result;
    }

    [HttpGet("async")]
    public async Task<IEnumerable<string>> GetAsync()
    {
      // Getting values, async waiting for result 
      return await this.GetValues();
    }

    [HttpGet("isresponding")]
    public Task<bool> IsResponding()
    {
      return task ?? (task = Task.FromResult(true));
    }

    private async Task<IEnumerable<string>> GetValues()
    {
      ThreadPool.GetAvailableThreads(out var workerThreads, out var completionPortThreads);
      Console.WriteLine($"Available thread count: {workerThreads}");

      // This is a mock for long running IO operation
      await Task.Delay(TimeSpan.FromSeconds(30));
      return new string[] { "value1", "value2" };
    }
  }
}
