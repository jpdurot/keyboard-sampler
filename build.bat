C:\Windows\Microsoft.Net\Framework\v4.0.30319\MSBuild.exe Sampler\Sampler.csproj /t:build /p:Configuration="Debug" /p:Platform="AnyCPU"
rem C:\Windows\Microsoft.Net\Framework64\v4.0.30319\MSBuild.exe Sampler.sln /t:rebuild /p:Configuration="Debug" /p:Platform="Any Cpu"
mkdir output
del /f  /s /q output\*.*
xcopy /s /e Sampler\bin\Debug\*.exe .\output\
xcopy /s /e Sampler\bin\Debug\*.dll .\output\
xcopy /s /e Sampler\bin\Debug\Sampler.exe.config .\output\
xcopy /s /e Sampler\bin\Debug\www\*.* .\output\www\