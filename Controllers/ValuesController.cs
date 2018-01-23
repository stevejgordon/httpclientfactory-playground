using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HttpClientFactoryTesting.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ValuesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var client = _httpClientFactory.CreateClient();

            var result = await client.GetStringAsync("http://www.google.com");

            var client2 = _httpClientFactory.CreateClient("Name"); // asking for a named client

            var result2 = await client.GetStringAsync("/"); // will add this path to the base url

            return Ok(result);
        }
    }

    [Route("api/[controller]")]
    public class AnotherController : Controller
    {
        private readonly SomeService _someService;

        public AnotherController(SomeService service)
        {
            _someService = service;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _someService.GetRawData();

            return Ok(result);
        }
    }

    public class SomeService
    {
        private readonly HttpClient _client;

        public SomeService(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("http://www.google.com");
        }

        public async Task<string> GetRawData()
        {
            return await _client.GetStringAsync("");
        }
    }
}
