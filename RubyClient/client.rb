# Copyright: Hiroshi Ichikawa <http://gimite.net/en/>
# Lincense: New BSD Lincense

$LOAD_PATH << File.dirname(__FILE__) + "/lib"
require "web_socket"

require 'json'

# $LOAD_PATH << File.dirname(__FILE__) + "/../ProtoBufs"
# require "CommsMessages.pb.rb"

#if ARGV.size != 1
#  $stderr.puts("Usage: ruby samples/stdio_client.rb ws://HOST:PORT/")
#  exit(1)
#end

#begin

if (ARGV.size > 0)
 hostname = ARGV.shift
else
 hostname = "localhost"
end
serverurl = "ws://#{hostname}:8081"

puts "Connecting to #{serverurl}"

#client = WebSocket.new(ARGV[0])
client = WebSocket.new(serverurl)
puts("Connected")
objectid = 0

# Login
  loggedin = false
  username = ""
  while (!loggedin) do
    puts("[1] new user")
    puts("[2] login")
    selection = gets.chomp
    msg = Hash.new()
    header = Hash.new()
    case selection
    when "1"
      puts "New User"
      printf("FullName:")
      name = gets.chomp
      printf("UserName:")
      username = gets.chomp
      printf("Password:")
      password = gets.chomp
      header[:msgtype] = 'NewUser'
      msg[:name] = name
      msg[:username] = username
      msg[:password] = password
      send = true
    when "2"
      puts "Login"
    else
      puts "Unknown option"
      send = false
    end
    if (send)
      header[:from] = msg[:username]
      header[:dest] = ""
      client.send(JSON.generate(header) + "\x00" + JSON.generate(msg))
      data = client.receive()
      printf("Received [%p]\n", data)
      printf("Received [%p]\n", data)
      #msg = JSON.parse(data)
      loggedin = true  # TODO
    end
  end

  Thread.new() do
    while data = client.receive()
      printf("Received [%p]\n", data)
      #puts("Received msgtype #{header.msgtype}")
      #case header.msgtype
      #when RESPONSE
      #  resmsg = Response.new
      #  resmsg.parse_from_string(data[headerSize+1,9999])
      #  puts("Response: (#{resmsg.code}) #{resmsg.message}")
      #when SAY
      #  #puts("Say")
      #  resmsg = Say.new
      #  resmsg.parse_from_string(data[headerSize+1,9999])
      #  puts("#{header.from}: #{resmsg.text}")
      #else
      #  puts("Unknown")
      #end
    end
    printf("Closing reader")
    exit()
  end


  while (1) do
    header = Hash.new()
    header[:from] = username
    header[:msgtype] = "Say"
    msg = Hash.new()
    parsed = gets.chomp.split(/:/)
    if (parsed[1] == nil)
      header[:dest] = ""
      msg[:text] = parsed[0]
    else
      header[:dest] = parsed[0]
      msg[:text] = parsed[1]
    end
    client.send(JSON.generate(header) + "\x00" + JSON.generate(msg))
  end

puts("Client closing")
client.close()

#rescue Exception => e
# puts "Exception #{e}"
#end
