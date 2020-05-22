using System;
using System.Net.Http;
using Newtonsoft.Json;
using SoundByte.Core.Extension;

namespace SoundByte.App.Uwp.Extensions.Definitions
{
    public class ExtensionNetwork : IExtensionNetwork
    {
        public string GetString(string url)
        {
            try
            {
                using (var client = new HttpClient())
                using (var request = client.GetAsync(url).Result)
                {
                    var str = request.Content.ReadAsStringAsync().Result;
                    return str;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public dynamic Get(string url)
        {
            try
            {
                using (var client = new HttpClient())
                using (var request = client.GetAsync(url).Result)
                {
                    var str = request.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<dynamic>(str);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}