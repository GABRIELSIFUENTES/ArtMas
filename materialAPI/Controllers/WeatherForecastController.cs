using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Extensions;
using RestSharp.Serialization;
using RestSharp.Serializers;
using RestSharp.Validation;
using ArticulosSAP;

namespace materialAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ArtMATHEAD_EncController : ControllerBase
    {
        //Conexion con el bus
        static readonly string queueOrTopicUrl = "https://materials.servicebus.windows.net/articulos/messages"; // Format: "https://<service bus namespace>.servicebus.windows.net/<topic name or queue>/messages";
        static readonly string signatureKeyName = "articulosSAS";
        static readonly string signatureKey = "Bdp+LIoo2AzXw4Mp+ef8kP1oLwklNCu1tQKm9thF+iA=";
        static readonly TimeSpan timeToLive = TimeSpan.FromDays(1);
        static readonly string token = GetSasToken(queueOrTopicUrl, signatureKeyName, signatureKey, timeToLive);
        public static string responseServer;

        public static string GetSasToken(string resourceUri, string keyName, string key, TimeSpan ttl)
        {
            var expiry = GetExpiry(ttl);
            string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
            HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);
            return sasToken;
        }

        private static string GetExpiry(TimeSpan ttl)
        {
            TimeSpan expirySinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1) + ttl;
            return Convert.ToString((int)expirySinceEpoch.TotalSeconds);
        }


        //metodo resAPI
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        
        private readonly ILogger<ArtMATHEAD_EncController> _logger;

        public ArtMATHEAD_EncController(ILogger<ArtMATHEAD_EncController> logger)
        {
            _logger = logger;
        }
        //Console.WriteLine("Authorization: " + token);
        [HttpGet]
        public IEnumerable<ArtMATHEAD_Enc> Get()
        {

            //metodo get
            string hardcore = "[{\"date\":\"2021-05-24T14:51:22.920365-05:00\",\"temperatureC\":-12,\"" +
                "temperatureF\":11,\"summary\":\"datasourceBalmy\"},{\"date\":\"2021-05-25T14:51:22.932372-05:00\"," +
                "\"temperatureC\":6,\"temperatureF\":42,\"summary\":\"datasourceCool\"},{\"date\":\"2021-05-26T14:51:22.932377-05:00\"," +
                "\"temperatureC\":28,\"temperatureF\":82,\"summary\":\"datasourceChilly\"},{\"date\":" +
                "\"2021-05-27T14:51:22.932378-05:00\",\"temperatureC\":-10,\"temperatureF\":15,\"summary\":" +
                "\"datasourceBalmy\"},{\"date\":\"2021-05-28T14:51:22.932379-05:00\",\"temperatureC\":44," +
                "\"temperatureF\":111,\"summary\":\"datasourceScorching\"}]";
           // PostBus(hardcore);
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new ArtMATHEAD_Enc
            {
               /* Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                TokenBus = token,*/
                Material_MATNR="MATERIAL PRUEBA",
                TipoMaterial_MATL_TYPE = "TIPO MATERIAL PRUEBA",
                GpoArt_MATL_GROUP = "GRUPO DE ARTICULOS PRUEBA",
                CategMat_MATL_CAT= "CATEGORIA MATERIAL PRUEBA",
                UltiDoc_NUMDOC= "DOCUMENTO",
                Fec_Movto= DateTime.Now.AddDays(index),
            })
            .ToArray();
            
        }
        public static void PostBus(string data)
        {
            var client = new RestClient("https://materials.servicebus.windows.net/articulos/messages");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", token);
            request.AddHeader("Content-Type", "application/atom+xml;type=entry;charset=utf-8");
            request.AddParameter("application/atom+xml;type=entry;charset=utf-8", data, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);

            responseServer = response.Content;
          
        }

    }   
}
