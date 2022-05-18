package com.nrkei.microservices.car_rental_offer

import com.nrkei.microservices.rapids_rivers.Packet
import com.nrkei.microservices.rapids_rivers.River
import com.nrkei.microservices.rapids_rivers.PacketProblems
import com.nrkei.microservices.rapids_rivers.RapidsConnection
import com.nrkei.microservices.rapids_rivers.rabbit_mq.RabbitMqRapids
import java.util.Random

@Suppress("unused") // json-encoding will make sure these props are used
private class CarOffer(
    val offer: String,
    val offer_message: String,
    val likelihood: Double,
    val value: Double
)

// Understands the messages on an event bus
class RandomOffer : River.PacketListener {
    companion object {
        @JvmStatic
        fun main(args: Array<String>) {
            val host = args[0]
            val port = args[1]
            val rapidsConnection = RabbitMqRapids("monitor_in_kotlin", host, port)
            val river = River(rapidsConnection)
            river.requireValue("need", "car_rental_offer")
            river.forbid("solution")
            river.register(RandomOffer())
        }
    }

    private var cars: List<CarOffer> = listOf(
        CarOffer("extra_rent_day", "Extra day", 0.8, 2000.0),
        CarOffer("clean_car", "Clean car", 0.1, 20000.0),
        CarOffer("clean_car", "Clean car", 0.5, 15000.0),
        CarOffer("cheap_coffee", "Cheap coffee", 1.0, 5.0),
        CarOffer("cheap_coffee", "Cheap coffee", 0.8, 10.0)
    )
    private var random = Random()

    override fun packet(connection: RapidsConnection, packet: Packet, warnings: PacketProblems) {
        val offers = random.nextInt(4)
        for (i in 0..offers) {
            packet.put("solution", cars[random.nextInt(cars.size)])
            connection.publish(packet.toJson())
        }
    }

    override fun onError(connection: RapidsConnection, errors: PacketProblems) {
        println(String.format(" [x] %s", errors))
    }
}