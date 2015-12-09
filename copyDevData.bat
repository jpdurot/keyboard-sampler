mkdir Sampler\bin\Debug\www
mkdir Sampler\bin\Debug\Sounds
xcopy /y /s /e Sampler\www\*.* Sampler\bin\Debug\www
xcopy /y Sampler\Configuration.xml Sampler\bin\Debug\
xcopy /y /s Sampler\Sounds\*.* Sampler\bin\Debug\Sounds
pause