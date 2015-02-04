protoc --lua_out=./ CommsMessages.proto
protoc --cpp_out=./ CommsMessages.proto
ruby-protobuf/bin/rprotoc CommsMessages.proto
