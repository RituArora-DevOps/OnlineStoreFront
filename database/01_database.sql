-- Create the database with explicit files if it doesn't exist
IF DB_ID('OnlineStore') IS NULL
BEGIN
    CREATE DATABASE OnlineStore;
    -- ON PRIMARY
    -- (
    --     NAME = OnlineStore_Data,                     -- logical name
    --     FILENAME = 'C:\Users\jsrri\FSD\Web_Application_Development_II\Assignments\TeamsProject\SQLData\OnlineStore_Data.mdf',-- physical file path
    --     SIZE = 20MB,                                   -- initial size
    --     MAXSIZE = 1GB,                                 -- maximum size
    --     FILEGROWTH = 5MB                               -- growth increment
    -- )
    -- LOG ON
    -- (
    --     NAME = OnlineStore_Log,
    --     FILENAME = 'C:\Users\jsrri\FSD\Web_Application_Development_II\Assignments\TeamsProject\SQLData\OnlineStore_Log.ldf',
    --     SIZE = 10MB,
    --     MAXSIZE = 512MB,
    --     FILEGROWTH = 5MB
    -- );
END
GO

-- Switch to the database
USE OnlineStore;
GO
