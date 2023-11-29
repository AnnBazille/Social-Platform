CREATE DATABASE [SocialPlatform];
GO

USE [SocialPlatform];
GO

CREATE TABLE [Users] (
    [Id]              CHAR(36)         NOT NULL,
	[Handle]          VARCHAR(64)      NOT NULL,
	[DisplayName]     NVARCHAR(128)    NOT NULL,
	[Email]           VARCHAR(320)     NOT NULL,
	[Salt]            CHAR(36)         NOT NULL,
	[PasswordHash]    CHAR(64)         NOT NULL,
	[SessionId]       CHAR(36)         NULL,

	CONSTRAINT    [PK_Users]           PRIMARY KEY    ([Id]),
	CONSTRAINT    [UQ_Users_Handle]    UNIQUE         ([Handle]),
	CONSTRAINT    [UQ_Users_Email]     UNIQUE         ([Email])
);
GO

CREATE TABLE [Followings] (
    [Id]            CHAR(36)    NOT NULL,
	[UserId]        CHAR(36)    NOT NULL,
	[FollowerId]    CHAR(36)    NOT NULL,

	CONSTRAINT    [PK_Followings]               PRIMARY KEY    ([Id]),
	CONSTRAINT    [FK_Followings_UserId]        FOREIGN KEY    ([UserId])        REFERENCES    [Users]    ([Id]),
	CONSTRAINT    [FK_Followings_FollowerId]    FOREIGN KEY    ([FollowerId])    REFERENCES    [Users]    ([Id])
);
GO

CREATE TABLE [Posts] (
    [Id]          CHAR(36)         NOT NULL,
	[UserId]      CHAR(36)         NOT NULL,
	[ParentId]    CHAR(36)         NULL,
	[Message]     NVARCHAR(256)    NULL,
	[Date]        DATETIME         NOT NULL,

	CONSTRAINT    [PK_Posts]             PRIMARY KEY    ([Id]),
	CONSTRAINT    [FK_Posts_UserId]      FOREIGN KEY    ([UserId])      REFERENCES    [Users]    ([Id]),
	CONSTRAINT    [FK_Posts_ParentId]    FOREIGN KEY    ([ParentId])    REFERENCES    [Posts]    ([Id])
);
GO

CREATE TABLE [Likes] (
    [Id]        CHAR(36)    NOT NULL,
	[UserId]    CHAR(36)    NOT NULL,
	[PostId]    CHAR(36)    NOT NULL,

	CONSTRAINT    [PK_Likes]           PRIMARY KEY    ([Id]),
	CONSTRAINT    [FK_Likes_UserId]    FOREIGN KEY    ([UserId])    REFERENCES    [Users]    ([Id]),
	CONSTRAINT    [FK_Likes_PostId]    FOREIGN KEY    ([PostId])    REFERENCES    [Posts]    ([Id])
);
GO

CREATE TABLE [Feeds] (
    [Id]            CHAR(36)    NOT NULL,
	[ReceiverId]    CHAR(36)    NOT NULL,
	[PostId]        CHAR(36)    NOT NULL,

	CONSTRAINT    [PK_Feeds]               PRIMARY KEY    ([Id]),
	CONSTRAINT    [FK_Feeds_ReceiverId]    FOREIGN KEY    ([ReceiverId])    REFERENCES    [Users]    ([Id]),
	CONSTRAINT    [FK_Feeds_PostId]        FOREIGN KEY    ([PostId])        REFERENCES    [Posts]    ([Id])
);
GO
