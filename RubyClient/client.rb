# Copyright: Hiroshi Ichikawa <http://gimite.net/en/>
# Lincense: New BSD Lincense

$LOAD_PATH << File.dirname(__FILE__) + "/lib"
require "web_socket"

$LOAD_PATH << File.dirname(__FILE__) + "/../ProtoBufs"
require "CommsMessages.pb.rb"

#if ARGV.size != 1
#  $stderr.puts("Usage: ruby samples/stdio_client.rb ws://HOST:PORT/")
#  exit(1)
#end

#begin

serverurl = "ws://localhost:8081"

#client = WebSocket.new(ARGV[0])
client = WebSocket.new(serverurl)
puts("Connected")
objectid = 0

  RESPONSE = 1
  PING = 2
  PONG = 3
  NEWUSER = 4
  LOGIN = 5
  SAY = 6

def StartReceiveThread()
  Thread.new() do
    while data = client.receive()
      printf("Received [%p]\n", data)
      header = Header.new
      header.parse_from_string(data[0..1])
      case header.msgtype
      when RESPONSE
        msg = Response.new
        msg.parse_from_string(data[2..9999])
        puts("Response: (#{msg.code}) #{msg.message}")
      when SAY
        puts("Say")
        msg = Say.new
        msg.parse_from_string(data[2..9999])
        puts("From: #{msg.from}")
        puts("Target: #{msg.target}")
        puts("Text: #{msg.text}")
      else
        puts("Unknown")
      end
    end
    printf("Closing reader")
    exit()
  end
end


# Ping server
puts("Pinging server '#{serverurl}'")
header = Header.new
header.msgtype = PING
msg = Ping.new
msg.count = 1
client.send(header.to_s + msg.to_s)

data = client.receive()
header = Header.new
header.parse_from_string(data[2..9999])
if (header.msgtype != PONG) 
  puts("Unexcpected message type '#{header.msgtype}'")
  exit()
end
reply = PONG.new
puts("Received pong '#{reply.count}'")

# Login
  loggedin = false
  username = ""
  while (!loggedin) do
    puts("[1] new user")
    puts("[2] login")
    selection = gets.chomp
    header = Header.new
    case selection
    when "1"
      puts "New User"
      header.msgtype = NEWUSER
      msg = NewUser.new
      printf("FullName:")
      msg.name = gets.chomp
      printf("UserName:")
      username = gets.chomp
      msg.username = username
      printf("Password:")
      msg.password = gets.chomp
      send = true
    when "2"
      puts "Login"
      printf("Name:")
      header.msgtype = LOGIN
      msg = Login.new
      printf("UserName:")
      username = gets.chomp
      msg.username = username
      printf("Password:")
      msg.password = gets.chomp
      send = true
    else
      puts "Unknown option"
      send = false
    end
    if (send)
      client.send(header.to_s + msg.to_s)
      data = client.receive()
      header = Header.new
      header.parse_from_string(data[2..9999])
      if (header.msgtype != RESPONSE) 
        puts("Unexcpected message type '#{header.msgtype}'")
        exit()
      end
      reply = Response.new
      puts("Received response '#{response.code}' #{response.message}")
      if (response.code = 1) 
        loggedin = true
      end
    end
  end

  StartReceiveThread()

  while (1) do
    printf(">")
    header = Header.new
    header.msgtype = SAY
    msg = Say.new
    msg.from = username
    msg.target = ""
    msg.text = gets.chomp
    
    client.send(header.to_s + msg.to_s)
  end

puts("Client closing")
client.close()

#rescue Exception => e
# puts "Exception #{e}"
#end
