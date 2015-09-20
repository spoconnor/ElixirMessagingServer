# Copyright: Hiroshi Ichikawa <http://gimite.net/en/>
# Lincense: New BSD Lincense

$LOAD_PATH << File.dirname(__FILE__) + "/lib"
require "web_socket"

$LOAD_PATH << File.dirname(__FILE__) + "/../ProtoBufs"
$LOAD_PATH << File.dirname(__FILE__) + "/../ProtoBufs/ruby-protobuf/lib"
require "CommsMessages.pb.rb"

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

EResponse = 1
EPing = 2
EPong = 3
ENewUser = 4
ELogin = 5
ESay = 6
EMapRequest = 7
EMapIgnore = 8
EMap = 9
EQueryServer = 10
EQueryServerResponse = 11

# Login
  loggedin = false
  username = ""
  while (!loggedin) do
    puts("[1] new user")
    puts("[2] login")
    selection = gets.chomp
    message = CommsMessages::Message.new
    case selection
    when "1"
      puts "New User"
      message.msgtype = ENewUser
      message.newUser = CommsMessages::NewUser.new
      printf("FullName:")
      message.newUser.name = gets.chomp
      printf("UserName:")
      username = gets.chomp
      message.newUser.username = username
      printf("Password:")
      message.newUser.password = gets.chomp
      send = true
    when "2"
      puts "Login"
      printf("Name:")
      message.msgtype = ELogin
      message.login = CommsMessages::Login.new
      printf("UserName:")
      username = gets.chomp
      message.login.username = username
      printf("Password:")
      message.login.password = gets.chomp
      send = true
    else
      puts "Unknown option"
      send = false
    end
    if (send)
      message.from = message.login.username
      message.dest = ""
      messageStr = message.to_s
      client.send(messageStr.length.chr + messageStr)
      data = client.receive()
      printf("Received [%p]\n", data)
      messageSize = data[0].ord
      message = CommsMessages::Message.new
      message.parse_from_string(data[1,messageSize])
      if (message.msgtype != EResponse) 
        puts("Unexcpected message type '#{message.msgtype}'")
        exit()
      end
      puts("Response '#{message.response.code}' #{message.response.message}")
      if (message.response.code = 1) 
        loggedin = true
      else
        puts("Login failed")
      end
    end
  end

  Thread.new() do
    while data = client.receive()
      printf("Received [%p]\n", data)
      messageSize = data[0].ord
      message = CommsMessages::Message.new
      puts("Received msgSize #{messageSize}")
      message.parse_from_string(data[1,messageSize])
      puts("Received msgtype #{message.msgtype}")
      case message.msgtype
      when EResponse
        puts("Response: (#{message.response.code}) #{message.response.message}")
      when ESay
        #puts("Say")
        puts("#{message.say.from}: #{message.say.text}")
      when EMap
        puts("Map message received for #{message.map.mapChunkX},#{message.map.mapChunkY}")
      else
        puts("Unknown")
      end
    end
    printf("Closing reader")
    exit()
  end

  # Request map
  message = CommsMessages::Message.new
  message.msgtype = EMapRequest
  message.from = username
  message.dest = "MapServer"
  message.mapRequest = CommsMessages::MapRequest.new
  message.mapRequest.mapChunkX = 1
  message.mapRequest.mapChunkY = 1
  messageStr = message.to_s
  client.send(messageStr.length.chr + messageStr)

  while (1) do
    message = CommsMessages::Message.new
    message.msgtype = ESay
    message.say = CommsMessages::Say.new
    message.from = username
    parsed = gets.chomp.split(/:/)
    if (parsed[1] == nil)
      message.dest = ""
      message.say.text = parsed[0]
    else
      message.dest = parsed[0]
      message.say.text = parsed[1]
    end
    
    messageStr = message.to_s
    client.send(messageStr.length.chr + messageStr)
  end

puts("Client closing")
client.close()

#rescue Exception => e
# puts "Exception #{e}"
#end
