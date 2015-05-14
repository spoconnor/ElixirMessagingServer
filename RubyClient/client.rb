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

  RESPONSE = 1
  PING = 2
  PONG = 3
  NEWUSER = 4
  LOGIN = 5
  SAY = 6


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
      header.from = msg.username
      header.dest = ""
      headerStr = header.to_s
      client.send(headerStr.length.chr + headerStr + msg.to_s)
      data = client.receive()
      printf("Received [%p]\n", data)
      headerSize = data[0].ord
      header = Header.new
      header.parse_from_string(data[1,headerSize])
      if (header.msgtype != RESPONSE) 
        puts("Unexcpected message type '#{header.msgtype}'")
        exit()
      end
      reply = Response.new
      reply.parse_from_string(data[headerSize+1,9999])
      puts("Response '#{reply.code}' #{reply.message}")
      if (reply.code = 1) 
        loggedin = true
      else
        puts("Login failed")
      end
    end
  end

  Thread.new() do
    while data = client.receive()
      printf("Received [%p]\n", data)
      headerSize = data[0].ord
      header = Header.new
      puts("Received msgSize #{headerSize}")
      header.parse_from_string(data[1,headerSize])
      puts("Received msgtype #{header.msgtype}")
      case header.msgtype
      when RESPONSE
        resmsg = Response.new
        resmsg.parse_from_string(data[headerSize+1,9999])
        puts("Response: (#{resmsg.code}) #{resmsg.message}")
      when SAY
        #puts("Say")
        resmsg = Say.new
        resmsg.parse_from_string(data[headerSize+1,9999])
        puts("#{header.from}: #{resmsg.text}")
      else
        puts("Unknown")
      end
    end
    printf("Closing reader")
    exit()
  end


  while (1) do
    header = Header.new
    header.msgtype = SAY
    msg = Say.new
    header.from = username
    parsed = gets.chomp.split(/:/)
    if (parsed[1] == nil)
      header.dest = ""
      msg.text = parsed[0]
    else
      header.dest = parsed[0]
      msg.text = parsed[1]
    end
    
    headerStr = header.to_s
    client.send(headerStr.length.chr + headerStr + msg.to_s)
  end

puts("Client closing")
client.close()

#rescue Exception => e
# puts "Exception #{e}"
#end
