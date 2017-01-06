#!/usr/bin/env ruby
# encoding: utf-8

require "rubygems"
require "bunny"

conn = Bunny.new("amqp://dev:password@equinox.local:5672")
#conn = Bunny.new("amqp://guest:guest@localhost:5672")
conn.start

ch  = conn.create_channel
x   = ch.direct("retail_system", :durable => true)
q   = ch.queue("crm_q", :durable => true, :auto_delete => false)
q.bind(x, :routing_key => "new_customer")
q.bind(x, :routing_key => "updated_customer")

ARGV.each do |severity|
  q.bind(x, :routing_key => severity)
end

puts " [*] Waiting for logs. To exit press CTRL+C"

begin
  q.subscribe(:block => true, :manual_ack => true) do |delivery_info, properties, body|
    puts " [x] #{delivery_info.routing_key}:#{body}"
    ch.acknowledge(delivery_info.delivery_tag, true)    
    sleep(10)
  end
rescue Interrupt => _
  ch.close
  conn.close
end