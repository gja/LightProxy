echo off
cd LightProxy
msbuild /p:Configuration=Release /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=D:\Documents\sgKey.snk 
cd ..
echo Remember to Remove InternalsVisibleTo line from Properties/AssemblyInfo.cs
copy LightProxy\bin\Release\LightProxy.dll .
