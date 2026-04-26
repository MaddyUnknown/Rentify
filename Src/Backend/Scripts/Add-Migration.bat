@echo off
SET context=%1
SET name=%2

IF "%name%"=="" (
    echo [ERROR] Migration name is required.
    echo Usage: add-migration.bat ContextType MigrationName
    exit /b 1
)

IF /I "%context%" NEQ "data" IF /I "%context%" NEQ "auth" goto notvalid

REM Data Access Context
IF "%context%"=="data" (
	dotnet ef migrations add %name% --context RentifyDbContext --project ../Rentify.DataAccess.SqlServer --startup-project ../Rentify.API
)

REM Auth Access Context
IF "%context%"=="auth" (
	dotnet ef migrations add %name% --context RentifyAuthDbContext --project ../Rentify.Auth.Identity --startup-project ../Rentify.API
)

exit /b 0


:notvalid
echo [ERROR] Migration context type is required and can only be 'auth' or 'data'.
echo Usage: add-migration.bat ContextType MigrationName
exit /b 1