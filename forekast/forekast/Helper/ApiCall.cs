using System.Net.Http;
using System.Threading.Tasks;

namespace WeCast.Helper
{
    public class ApiCall
    {
        public static async Task<ApiResponse> GetApi(string url, string authorizationId = null)
        {
            using (var  client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(authorizationId))
                    client.DefaultRequestHeaders.Add("Authorization", authorizationId);

                var request = await client.GetAsync(url);
                if (request.IsSuccessStatusCode)
                {
                    return new ApiResponse { Response = await request.Content.ReadAsStringAsync() };
                }
                else
                {
                    return new ApiResponse { ErrorMessage = request.ReasonPhrase };
                }
            }
        }
    }

    public class ApiResponse
    {
        public bool IsSuccessful => ErrorMessage == null;
        public string ErrorMessage { get; set; }

        public string Response { get; set; }
    }
}
