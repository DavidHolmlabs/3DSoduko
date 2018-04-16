using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Soduko;

namespace SodukoAzure
{
    public static class GetInitialBoard
    {
        [FunctionName("GetInitialBoard")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string emptySquaresString = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "emptySquares", true) == 0)
                .Value;

            if (emptySquaresString == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                emptySquaresString = data?.name;
            }

            int emptySquares = 100;

            if(emptySquaresString != null)
            {
                if (int.TryParse(emptySquaresString, out int res))
                    emptySquares = res;
            }

            var mainGame = new MainGame();

            await mainGame.FetchSolutionAsync(emptySquares);

            var json = mainGame.Serialize();

            return req.CreateResponse(HttpStatusCode.OK, json);
        }
    }
}
