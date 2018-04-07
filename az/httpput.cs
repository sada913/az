using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace AzureFunk
{
    public static class httpput
    {
        [FunctionName("httpput")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "put")]Person person, [Table("person", Connection = "")]CloudTable outTable, TraceWriter log)
        {
            if (person.id == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("A non-empty id must be specified.")
                };
            };

            person.PartitionKey = "Per";
            person.RowKey = person.id.ToString();
            //こいつらは絶対いるプライマリキー
            //これがかぶっているやつを更新してるはず？


            //このへんで多分更新処理してるはず
            TableOperation updateOperation = TableOperation.InsertOrReplace(person);
            TableResult result = outTable.Execute(updateOperation);
            return new HttpResponseMessage((HttpStatusCode)result.HttpStatusCode);
        }

        public class Person : AzureFunk.httppost.Person
        {
        }
    }
}
