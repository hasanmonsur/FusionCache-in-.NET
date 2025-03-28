namespace FusionCacheWebApi.Models
{
  
    public class CacheRequest
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public TimeSpan Expiration { get; set; }
    }

}
