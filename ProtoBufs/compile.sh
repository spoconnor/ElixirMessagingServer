protoc --lua_out=./ CommsMessages.proto
protoc --cpp_out=./ CommsMessages.proto
ruby-protobuf/bin/rprotoc CommsMessages.proto

# DotNet
protoc CommsMessages.proto -o CommsMessages.bin
mono DotNet/ProtoGen.exe CommsMessages.bin 

cp CommsMessages.pb.cc ../EclipseWorkspace/WorldServer/src/CommsMessages.pb.cc
cp CommsMessages.pb.h ../EclipseWorkspace/WorldServer/include/CommsMessages.pb.h
cp CommsMessages.proto ../ElixirMessagingServer/priv/CommsMessages.proto
cp CommsMessages.cs ../FSharp/WorldServer/CommsMessages.cs
