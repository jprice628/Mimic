cd /build/Test/UnitTests/
dotnet test
result=$?
[ $result -ne 0 ] && exit $result

cd /build/Source/Mimic/
dotnet publish -o /publish/
exit $?