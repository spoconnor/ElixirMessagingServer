### Generated by rprotoc. DO NOT EDIT!
### <proto file: CommsMessages.proto>
# package CommsMessages;
#  
# //import "google/protobuf/csharp_options.proto";
# //
# //option (google.protobuf.csharp_file_options).namespace = "Onewheel.Interface";
# //option (google.protobuf.csharp_file_options).umbrella_classname = "WorldEventsProtos";
# 
# option optimize_for = SPEED;
# 
#   enum MsgType {
#     eResponse = 1;
#     ePing = 2;
#     ePong = 3;
#     eNewUser = 4;
#     eLogin = 5;
#     eSay = 6;
#     eMapRequestUpdates = 7;
#     eMapIgnoreUpdates = 8;
#     eMap = 9;
#     eQueryServer = 10;
#     eQueryServerResponse = 11;
#   }
# 
#   message Message {
#     required int32 msgtype = 1;
#     required string from = 2;
#     required string dest = 3;
# 
#     optional Response response = 4;
#     optional Ping ping = 5;
#     optional Pong pong = 6;
#     optional NewUser newUser = 7;
#     optional Login login = 8;
#     optional Say say = 9;
#     optional MapRequestUpdates mapRequestUpdates = 10;
#     optional MapIgnoreUpdates mapIgnoreUpdates = 11;
#     optional Map map = 12;
#     optional QueryServer queryServer = 13;
#     optional QueryServerResponse queryServerResponse = 14;
#   }
# 
#   message Ping
#   {
#     required int32 count = 1;
#   }
#   message Pong
#   {
#     required int32 count = 1;
#   }
# 
#   message Response
#   {
#     required int32 code = 1;
#     optional string message = 2;
#   }
# 
#   message NewUser
#   {
#     required string username = 1;
#     required string password = 2;
#     required string name = 3;
#   }
#   
#   message Login
#   {
#     required string username = 1;
#     required string password = 2;
#   }
#   
#   message Say 
#   {
#     required string text = 1;
#   }
# 
#   message MapRequestUpdates
#   {
#     required int32 mapChunkX = 1;
#     required int32 mapChunkY = 2;
#   }
# 
#   message MapIgnoreUpdates
#   {
#     required int32 mapChunkX = 1;
#     required int32 mapChunkY = 2;
#   }
# 
#   message Map
#   {
#     required int32 mapChunkX = 1;
#     required int32 mapChunkY = 2;
#     required int32 dataSize = 3;
#     // binary data follows message
#   }
# 
#   message QueryServer 
#   {
#   }
# 
#   message QueryServerResponse 
#   {
#     required int32 minMapChunkX = 1;
#     required int32 minMapChunkY = 2;
#     required int32 maxMapChunkX = 3;
#     required int32 maxMapChunkY = 4;
#   }

require 'protobuf/message/message'
require 'protobuf/message/enum'
require 'protobuf/message/service'
require 'protobuf/message/extend'

module CommsMessages
  ::Protobuf::OPTIONS[:"optimize_for"] = :SPEED
  class MsgType < ::Protobuf::Enum
    defined_in __FILE__
    EResponse = value(:eResponse, 1)
    EPing = value(:ePing, 2)
    EPong = value(:ePong, 3)
    ENewUser = value(:eNewUser, 4)
    ELogin = value(:eLogin, 5)
    ESay = value(:eSay, 6)
    EMapRequestUpdates = value(:eMapRequestUpdates, 7)
    EMapIgnoreUpdates = value(:eMapIgnoreUpdates, 8)
    EMap = value(:eMap, 9)
    EQueryServer = value(:eQueryServer, 10)
    EQueryServerResponse = value(:eQueryServerResponse, 11)
  end
  class Message < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :msgtype, 1
    required :string, :from, 2
    required :string, :dest, 3
    optional :Response, :response, 4
    optional :Ping, :ping, 5
    optional :Pong, :pong, 6
    optional :NewUser, :newUser, 7
    optional :Login, :login, 8
    optional :Say, :say, 9
    optional :MapRequestUpdates, :mapRequestUpdates, 10
    optional :MapIgnoreUpdates, :mapIgnoreUpdates, 11
    optional :Map, :map, 12
    optional :QueryServer, :queryServer, 13
    optional :QueryServerResponse, :queryServerResponse, 14
  end
  class Ping < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :count, 1
  end
  class Pong < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :count, 1
  end
  class Response < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :code, 1
    optional :string, :message, 2
  end
  class NewUser < ::Protobuf::Message
    defined_in __FILE__
    required :string, :username, 1
    required :string, :password, 2
    required :string, :name, 3
  end
  class Login < ::Protobuf::Message
    defined_in __FILE__
    required :string, :username, 1
    required :string, :password, 2
  end
  class Say < ::Protobuf::Message
    defined_in __FILE__
    required :string, :text, 1
  end
  class MapRequestUpdates < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :mapChunkX, 1
    required :int32, :mapChunkY, 2
  end
  class MapIgnoreUpdates < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :mapChunkX, 1
    required :int32, :mapChunkY, 2
  end
  class Map < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :mapChunkX, 1
    required :int32, :mapChunkY, 2
    required :int32, :dataSize, 3
  end
  class QueryServer < ::Protobuf::Message
    defined_in __FILE__
  end
  class QueryServerResponse < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :minMapChunkX, 1
    required :int32, :minMapChunkY, 2
    required :int32, :maxMapChunkX, 3
    required :int32, :maxMapChunkY, 4
  end
end