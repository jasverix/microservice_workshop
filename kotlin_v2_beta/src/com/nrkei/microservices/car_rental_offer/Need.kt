/*
 * Copyright (c) 2019 by Fred George
 * May be used freely except for training; license required for training.
 * @author Fred George  fredgeorge@acm.org
 */

package com.nrkei.microservices.car_rental_offer

import com.nrkei.microservices.rapids_rivers.Packet
import com.nrkei.microservices.rapids_rivers.RapidsConnection
import com.nrkei.microservices.rapids_rivers.rabbit_mq.RabbitMqRapids

// Understands the requirement for advertising on a site
class Need {

    companion object {
        @JvmStatic
        fun main(args: Array<String>) {
            val host = args[0]
            val port = args[1]

            val rapidsConnection = RabbitMqRapids("car_rental_need_kotlin", host, port)
            publish(rapidsConnection)
        }


        private fun publish(rapidsConnection: RapidsConnection) {
            try {
                while (true) {
                    val jsonMessage = needPacket().toJson()
                    println(String.format(" [<] %s", jsonMessage))
                    rapidsConnection.publish(jsonMessage)
                    Thread.sleep(5000)
                }
            } catch (e: Exception) {
                throw RuntimeException("Could not publish message:", e)
            }

        }

        private fun needPacket(): Packet = Packet(hashMapOf("need" to "car_rental_offer") as Map<String, Any>?)
    }
}