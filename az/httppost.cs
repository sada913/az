using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace AzureFunk
{
    public static class httppost
    {
        [FunctionName("httppost")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequestMessage req, [Table("person", Connection = "")]ICollector<Person> outTable, [Table("person2", Connection = "")]ICollector<Person> outTable2, TraceWriter log)
        {
            dynamic data = await req.Content.ReadAsAsync<object>();
            string name = data?.name;
            //json��name�� name��


            Friends re = JsonConvert.DeserializeObject<Friends>( req.Content.ReadAsStringAsync().Result);
            //req.Content.ReadAsStringAsync() �Ƀ��N�G�X�g�̃{�f�B�������Ă�
            name = re.name;


            
            Random r = new Random();

            if (name == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name in the request body");
            }

            /*

            var friends = new List<Friends>();
            friends.Add((new Friends() { name = "Akasaka", id = r.Next() }));
            friends.Add((new Friends() { name = "Kaito", id = r.Next() }));
            var j_frinds = JsonConvert.SerializeObject(friends,Formatting.Indented);

            */

            //List�\����string�ɂ��Ȃ���_��

            Person p = new Person()
            {
                PartitionKey = "Functions",
                RowKey = Guid.NewGuid().ToString(),
                name = name,
                id = r.Next(),
                //jfrinds = j_frinds
            };
            outTable.Add(p);
            //table�X�g���[�W�Ɋi�[
            outTable2.Add(p);


            return req.CreateResponse(HttpStatusCode.Created,p);
            //���N�G�X�g�ɏ��������json�ŕԂ�class���\
        }

        public class Person : TableEntity
        {
            public string name { get; set; }
            public int id { get; set; }
            public string jfrinds { get; set; }
        }

        public class Friends
        {
            public string name;
            public int id;
        }
    }
}
