@echo off
SET name=%1

IF "%name%"=="" (
    echo [ERROR] Migration name is required.
    echo Usage: add-migration.bat MigrationName
    exit /b 1
)

dotnet ef migrations add %name% --project ../Rentify.DataAccess.SqlServer --startup-project ../Rentify.API