using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace mapa.Classes
{
    class http
    {
        internal async Task<string> getAnsv(string artName, string apikey)
        {
            HttpClient httpClient = new HttpClient();
            string request = "https://rest.bandsintown.com/v4/artists/" + artName + @"/events/?app_id=" + apikey;
            HttpResponseMessage response =
                (await httpClient.GetAsync(request)).EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
