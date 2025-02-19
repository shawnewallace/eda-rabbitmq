IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220419145755_first'
)
BEGIN
    CREATE TABLE [dbo].[log_entries] (
        [Id] int NOT NULL IDENTITY,
        [OrderId] uniqueidentifier NOT NULL,
        [RoutingKey] nvarchar(max) NULL,
        [Content] nvarchar(max) NULL,
        [WhenReceived] datetime2 NOT NULL,
        CONSTRAINT [PK_log_entries] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220419145755_first'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220419145755_first', N'9.0.2');
END;

COMMIT;
GO

