using FusionCacheWebApi.Models;
using FusionCacheWebApi.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZiggyCreatures.Caching.Fusion;

namespace FusionCacheWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IDataService _dataService;

        public CacheController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var data = await _dataService.GetExpensiveDataAsync(key);
            return Ok(new
            {
                Data = data,
                Message = "Notice subsequent calls are faster due to caching",
                Tip = "Try calling multiple instances with a Redis backplane to see distributed caching"
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetValue(string key)
        {
            var data = await _dataService.GetDataAsync(key);

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> SetValue(CacheRequest request)
        {
            var data = await _dataService.SetDataAsync(request);

            return Ok(data);

        }


        [HttpGet("expire/{key}")]
        public async Task<IActionResult> Expire(string key, [FromServices] IFusionCache cache)
        {
            await cache.RemoveAsync($"data:{key}");
            return Ok(new { Message = $"Cache entry for key '{key}' has been expired" });
        }
    }
}
