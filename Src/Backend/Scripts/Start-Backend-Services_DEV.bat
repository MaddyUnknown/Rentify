@echo off

PUSHD ..
dotnet build || (
	echo [ERROR] Build failed
	POPD
	exit /b 1
)

POPD

start "Web API" cmd /k "cd /d ../Rentify.API && dotnet run --no-build --environement Development"
start "Dispatcher Worker" cmd /k "cd /d ../Rentify.Event.Dispatcher.Worker && dotnet run --no-build --environement Development"
start "Processor Worker" cmd /k "cd /d ../Rentify.Event.Processor.Worker && dotnet run --no-build --environement Development"