using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AzureFunk
{
    public static class httpget
    {
        [FunctionName("httpget")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get")]HttpRequestMessage req, [Table("person", Connection = "")]IQueryable<Person> inTable, TraceWriter log)
        {
            dynamic data = req.Content.ReadAsAsync<object>();
            string name = req.GetQueryNameValuePairs()
                             .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                             .Value;
            //http://hoge.com/api/hoge ?name={なんたら} のなんたらを取得
            //複数は&でつなぐ

            if (string.IsNullOrEmpty(name))
                return req.CreateResponse(HttpStatusCode.BadRequest, "Argument Not found");
            //おかしかったらエラー

            var searchlist = inTable.Where(Person => Person.name == name).ToList();
            //検索
            if (searchlist.Count == 0)
                return req.CreateErrorResponse(HttpStatusCode.NotFound, "Data Not Found");
            //なかったら404

            return req.CreateResponse(HttpStatusCode.OK, searchlist);
            //レスポンス
        }

        public class Person : AzureFunk.httppost.Person
        {
        }

    }
}
