cd /build/Test/UnitTests/
dotnet test
result=$?
[ $result -ne 0 ] && exit $result

cd /build/Source/VirtualService/
dotnet publish -o /publish/
exit $?