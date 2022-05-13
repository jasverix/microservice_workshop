using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MicroServiceWorkshop.RapidsRivers;
using MicroServiceWorkshop.RapidsRivers.RabbitMQ;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RentalOffer.RandomOffer {
    internal class RandomOffer : River.IPacketListener {
        private readonly Random _random = new Random();
        
        public static void Main() {
            string host = "172.27.242.200";
            string port = "5672";

            var rapidsConnection = new RabbitMqRapids("monitor_in_csharp", host, port);
            var river = new River(rapidsConnection);
            // See RiverTest for various functions River supports to aid in filtering, like:
            river.RequireValue("need", "car_rental_offer"); // Reject packet unless key exists and has expected value
            //river.Require("key1", "key2");       // Reject packet unless it has key1 and key2
            river.Forbid("solution");        // Reject packet if it does have key1 or key2
            river.Register(new RandomOffer()); // Hook up to the river to start receiving traffic
        }

        public void ProcessPacket(RapidsConnection connection, JObject jsonPacket, PacketProblems warnings) {
            var cars = new List<object>() {
                new {
                    offer = "Extra day",
                    likelihood = 0.8,
                    value = 2000,
                },
                new {
                    offer = "Clean car",
                    likelihood = 0.1,
                    value = 20000,
                },
                new {
                    offer = "Clean car",
                    likelihood = 0.5,
                    value = 15000,
                },
                new {
                    offer = "Cheap coffee",
                    likelihood = 1d,
                    value = 5,
                },
                new {
                    offer = "Cheap coffee",
                    likelihood = 0.8,
                    value = 10,
                },
            };
            
           var index = _random.Next(cars.Count);

           jsonPacket["solution"] = JToken.FromObject(cars[index - 1]);

           connection.Publish(JsonConvert.SerializeObject(jsonPacket));
        }

        public void ProcessError(RapidsConnection connection, PacketProblems errors) { }
    }
}