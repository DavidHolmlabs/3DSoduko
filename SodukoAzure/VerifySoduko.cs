using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Soduko;

namespace SodukoAzure
{
    public static class VerifySoduko
    {
        [FunctionName("VerifySoduko")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("VerifySoduko");

            dynamic data = await req.Content.ReadAsAsync<object>();

            try
            {
                string resultJson = (new MainGame()).VerifyJson(data.ToString());

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        resultJson,
                        Encoding.UTF8,
                        "application/json")
                };
            }
            catch (System.Exception)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Bad Json");
            }
        }
    }
}
