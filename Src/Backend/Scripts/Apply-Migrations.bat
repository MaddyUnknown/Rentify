@echo off
SET context=%1

IF /I "%context%" NEQ "data" IF /I "%context%" NEQ "auth" goto notvalid

REM Data Access Context
IF "%context%"=="data" (
	dotnet ef database update --context RentifyDbContext --project ../Rentify.DataAccess.SqlServer --startup-project ../Rentify.API
)

REM Auth Access Context
IF "%context%"=="auth" (
	dotnet ef database update --context RentifyAuthDbContext --project ../Rentify.DataAccess.SqlServer --startup-project ../Rentify.API
)

exit /b 0


:notvalid
echo [ERROR] Migration context type is required and can only be 'auth' or 'data'.
echo Usage: add-migration.bat ContextType MigrationName
exit /b 1