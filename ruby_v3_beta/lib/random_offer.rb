#!/usr/bin/env ruby
# encoding: utf-8
require 'securerandom'
require 'rapids_rivers'

# Understands the complete stream of messages on an event bus
class RandomOffer
  attr_reader :service_name

  def initialize(host_ip, port)
    rapids_connection = RapidsRivers::RabbitMqRapids.new(host_ip, port)
    @river = RapidsRivers::RabbitMqRiver.new(rapids_connection)
    @river.require_values 'need' => 'car_rental_offer'
    @river.forbid 'solution'
    @service_name = 'random_offer_ruby_' + SecureRandom.uuid

    @cars = Array.new {
      {
        offer: "extra_rent_day",
        offer_message: "Extra day",
        likelihood: 0.8,
        value: 2000,
      }
      {
        offer: "clean_car",
        offer_message: "Clean car",
        likelihood: 0.1,
        value: 20000,
      }
      {
        offer: "clean_car",
        offer_message: "Clean car",
        likelihood: 0.5,
        value: 15000,
      }
      {
        offer: "cheap_coffee",
        offer_message: "Cheap coffee",
        likelihood: 1.0,
        value: 5,
      }
      {
        offer: "cheap_coffee",
        offer_message: "Cheap coffee",
        likelihood: 0.8,
        value: 10,
      }
    }
  end

  def start
    puts " [*] #{@service_name} waiting for traffic on RabbitMQ event bus ... To exit press CTRL+C"
    @river.register(self)
  end

  # @param [RapidsRivers::RabbitMqRapids] rapids_connection
  # @param [RapidsRivers::Packet] packet
  def packet(rapids_connection, packet, warnings)
    offers = rand(4)
    i = 0
    while i < offers
      ++i

      # don't really like this way of doing it though
      packet.used_key 'solution'
      packet.instance_variable_set :@solution, @cars[rand(@cars.length - 1)]

      rapids_connection.publish packet.to_json
    end
  end

  def on_error(rapids_connection, errors)
    puts " [x] #{errors}"
  end
end

RandomOffer.new(ARGV.shift, ARGV.shift).start
