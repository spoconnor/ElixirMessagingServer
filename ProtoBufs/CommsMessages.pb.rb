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
#     eMapRequest = 7;
#     eMapIgnore = 8;
#     eMap = 9;
#     eMapUpdate = 10;
#     eMapCharacterUpdate = 11;
#     eQueryServer = 12;
#     eQueryServerResponse = 13;
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
#     optional MapRequest mapRequest = 10;
#     optional MapIgnore mapIgnore = 11;
#     optional Map map = 12;
#     optional MapUpdate mapUpdate = 13;
#     optional MapCharacterUpdate mapCharacterUpdate = 14;
#     optional QueryServer queryServer = 15;
#     optional QueryServerResponse queryServerResponse = 16;
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
#   message MapRequest
#   {
#     required int32 x = 1;
#     required int32 y = 2;
#   }
# 
#   message MapIgnore
#   {
#     required int32 x = 1;
#     required int32 y = 2;
#   }
# 
#   message Map
#   {
#     required int32 minX = 1;
#     required int32 minY = 2;
#     required int32 maxX = 3;
#     required int32 maxY = 4;
#     required int32 dataSize = 5;
#     // binary data follows message
#   }
# 
#   message MapUpdate 
#   {
#     required int32 x = 1;
#     required int32 y = 2;
#     required int32 z = 3;
#     required int32 newBlock = 4;
#   }
# 
#   message MapCharacterUpdate 
#   {
#     required int32 id = 1;
#     required int32 x = 2;
#     required int32 y = 3;
#     required int32 z = 4;
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
    EMapRequest = value(:eMapRequest, 7)
    EMapIgnore = value(:eMapIgnore, 8)
    EMap = value(:eMap, 9)
    EMapUpdate = value(:eMapUpdate, 10)
    EMapCharacterUpdate = value(:eMapCharacterUpdate, 11)
    EQueryServer = value(:eQueryServer, 12)
    EQueryServerResponse = value(:eQueryServerResponse, 13)
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
    optional :MapRequest, :mapRequest, 10
    optional :MapIgnore, :mapIgnore, 11
    optional :Map, :map, 12
    optional :MapUpdate, :mapUpdate, 13
    optional :MapCharacterUpdate, :mapCharacterUpdate, 14
    optional :QueryServer, :queryServer, 15
    optional :QueryServerResponse, :queryServerResponse, 16
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
  class MapRequest < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :x, 1
    required :int32, :y, 2
  end
  class MapIgnore < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :x, 1
    required :int32, :y, 2
  end
  class Map < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :minX, 1
    required :int32, :minY, 2
    required :int32, :maxX, 3
    required :int32, :maxY, 4
    required :int32, :dataSize, 5
  end
  class MapUpdate < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :x, 1
    required :int32, :y, 2
    required :int32, :z, 3
    required :int32, :newBlock, 4
  end
  class MapCharacterUpdate < ::Protobuf::Message
    defined_in __FILE__
    required :int32, :id, 1
    required :int32, :x, 2
    required :int32, :y, 3
    required :int32, :z, 4
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