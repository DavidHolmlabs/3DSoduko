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
    public static class GetInitialBoard
    {
        [FunctionName("GetInitialBoard")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("GetInitialBoard");

            dynamic data = await req.Content.ReadAsAsync<object>();
            var emptySquaresString = data?.name;

            int emptySquares = 100;

            if (emptySquaresString != null)
            {
                if (int.TryParse(emptySquaresString, out int res))
                    emptySquares = res;
            }

            var mainGame = new MainGame();

            await mainGame.FetchSolutionAsync(emptySquares);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    mainGame.Serialize(),
                    Encoding.UTF8,
                    "application/json")
            };
        }
    }
}
