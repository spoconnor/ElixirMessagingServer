mkdir MyApp
cp start_*.sh MyApp
cp ../ElixirMessagingServer/ElixirMessagingServer MyApp 
cp ../ElixirMessagingServer/priv MyApp -R
cp ../EclipseWorkspace/WorldServer/Debug/WorldServer MyApp
cp ../EclipseWorkspace/WorldServer/include/SimpleAmqpClient/libSimpleAmqpClient.so.2.4.0 MyApp

today=`date +"%Y%m%d-%H%M"`
tar -czvf MyApp.$today.tgz MyApp
aws s3 cp MyApp.$today.tgz s3://onewheel/installation/ --acl public-read

echo Uploaded
