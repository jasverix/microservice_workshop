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
            const string host = "toyota";
            const string port = "5672";
            var rapidsConnection = new RabbitMqRapids("monitor_in_csharp", host, port);
            var river = new River(rapidsConnection);
            river.RequireValue("need", "car_rental_offer"); // Reject packet unless key exists and has expected value
            river.Forbid("solution"); // Reject packet if it does have key1 or key2
            river.Register(new RandomOffer()); // Hook up to the river to start receiving traffic
        }

        private readonly List<object> _cars = new List<object>() {
            new {
                offer = "extra_rent_day",
                offer_message = "Extra day",
                likelihood = 0.8,
                value = 2000,
            },
            new {
                offer = "clean_car",
                offer_message = "Clean car",
                likelihood = 0.1,
                value = 20000,
            },
            new {
                offer = "clean_car",
                offer_message = "Clean car",
                likelihood = 0.5,
                value = 15000,
            },
            new {
                offer = "cheap_coffee",
                offer_message = "Cheap coffee",
                likelihood = 1d,
                value = 5,
            },
            new {
                offer = "cheap_coffee",
                offer_message = "Cheap coffee",
                likelihood = 0.8,
                value = 10,
            },
        };

        public void ProcessPacket(RapidsConnection connection, JObject jsonPacket, PacketProblems warnings) {
            var amountOfOffers = _random.Next(0, 4);
            for (var i = 0; i < amountOfOffers; ++i) {
                jsonPacket["solution"] = JToken.FromObject(_cars[_random.Next(_cars.Count) - 1]);
                connection.Publish(JsonConvert.SerializeObject(jsonPacket));
            }
        }

        public void ProcessError(RapidsConnection connection, PacketProblems errors)
            => Console.WriteLine(" [x] {0}", errors);
    }
}
