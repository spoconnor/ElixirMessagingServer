protoc --lua_out=./ CommsMessages.proto
protoc --cpp_out=./ CommsMessages.proto
ruby-protobuf/bin/rprotoc CommsMessages.proto

# DotNet
protoc CommsMessages.proto -o CommsMessages.bin
mono DotNet/ProtoGen.exe CommsMessages.bin 

# Scala
cd ScalaBuff
sbt "project scalabuff-compiler" "run --proto_path=.. --scala_out=.."
cd ..
mv CommsMessages/* .

cp CommsMessages.pb.cc ../EclipseWorkspace/WorldServer/src/CommsMessages.pb.cc
cp CommsMessages.pb.h ../EclipseWorkspace/WorldServer/include/CommsMessages.pb.h
cp CommsMessages.proto ../ElixirMessagingServer/priv/CommsMessages.proto
cp CommsMessages.cs ../DotNet/WorldServer/CommsMessages.cs
cp CommsMessages.proto ../Html5Client/src/CommsMessages.proto
cp CommsMessages.scala ../ScalaServer/src/main/scala/CommsMessages.scala

echo "Done!"
