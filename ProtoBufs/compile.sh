protoc --lua_out=./ CommsMessages.proto
protoc --cpp_out=./ CommsMessages.proto
ruby-protobuf/bin/rprotoc CommsMessages.proto
cp CommsMessages.pb.cc ../EclipseWorkspace/WorldServer/src/CommsMessages.pb.cc
cp CommsMessages.pb.h ../EclipseWorkspace/WorldServer/include/CommsMessages.pb.h
cp CommsMessages.proto ../ElixirMessagingServer/priv/CommsMessages.proto
