
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/11/2017 01:22:11
-- Generated from EDMX file: E:\Test3Server\resources\TecoRP_Website\Models\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [TecoRP];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Answers_Applications]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Answers] DROP CONSTRAINT [FK_Answers_Applications];
GO
IF OBJECT_ID(N'[dbo].[FK_Answers_Questions]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Answers] DROP CONSTRAINT [FK_Answers_Questions];
GO
IF OBJECT_ID(N'[dbo].[FK_Blogs_Categories]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Blogs] DROP CONSTRAINT [FK_Blogs_Categories];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserClaims] DROP CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserLogins] DROP CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_QuestionToApplications_Questions]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QuestionToApplications] DROP CONSTRAINT [FK_QuestionToApplications_Questions];
GO
IF OBJECT_ID(N'[dbo].[FK_TradeOffers_AspNetUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TradeOffers] DROP CONSTRAINT [FK_TradeOffers_AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[FK_TradeOffers_TradeCategories]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TradeOffers] DROP CONSTRAINT [FK_TradeOffers_TradeCategories];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[__MigrationHistory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[__MigrationHistory];
GO
IF OBJECT_ID(N'[dbo].[Answers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Answers];
GO
IF OBJECT_ID(N'[dbo].[Applications]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Applications];
GO
IF OBJECT_ID(N'[dbo].[AspNetRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserClaims]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserClaims];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserLogins]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserLogins];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[Blogs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Blogs];
GO
IF OBJECT_ID(N'[dbo].[Categories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Categories];
GO
IF OBJECT_ID(N'[dbo].[Questions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Questions];
GO
IF OBJECT_ID(N'[dbo].[QuestionToApplications]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QuestionToApplications];
GO
IF OBJECT_ID(N'[dbo].[Subscribtions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Subscribtions];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[TradeCategories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TradeCategories];
GO
IF OBJECT_ID(N'[dbo].[TradeOffers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TradeOffers];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'C__MigrationHistory'
CREATE TABLE [dbo].[C__MigrationHistory] (
    [MigrationId] nvarchar(150)  NOT NULL,
    [ContextKey] nvarchar(300)  NOT NULL,
    [Model] varbinary(max)  NOT NULL,
    [ProductVersion] nvarchar(32)  NOT NULL
);
GO

-- Creating table 'Answers'
CREATE TABLE [dbo].[Answers] (
    [AnswerId] int IDENTITY(1,1) NOT NULL,
    [QuestionId] int  NOT NULL,
    [ApplicationId] int  NULL,
    [Answer] tinyint  NULL,
    [AnswerText] nvarchar(2000)  NULL
);
GO

-- Creating table 'Applications'
CREATE TABLE [dbo].[Applications] (
    [ApplicationId] int IDENTITY(1,1) NOT NULL,
    [IsApproved] bit  NULL,
    [Name] nvarchar(100)  NULL,
    [SocialClubName] nvarchar(250)  NULL,
    [Contact] nvarchar(100)  NULL,
    [UserID] nvarchar(128)  NULL,
    [RegisterDate] datetime  NULL
);
GO

-- Creating table 'AspNetRoles'
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(128)  NOT NULL,
    [Name] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'AspNetUserClaims'
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [ClaimType] nvarchar(max)  NULL,
    [ClaimValue] nvarchar(max)  NULL
);
GO

-- Creating table 'AspNetUserLogins'
CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] nvarchar(128)  NOT NULL,
    [ProviderKey] nvarchar(128)  NOT NULL,
    [UserId] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'AspNetUsers'
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(128)  NOT NULL,
    [Email] nvarchar(256)  NULL,
    [EmailConfirmed] bit  NOT NULL,
    [PasswordHash] nvarchar(max)  NULL,
    [SecurityStamp] nvarchar(max)  NULL,
    [PhoneNumber] nvarchar(max)  NULL,
    [PhoneNumberConfirmed] bit  NOT NULL,
    [TwoFactorEnabled] bit  NOT NULL,
    [LockoutEndDateUtc] datetime  NULL,
    [LockoutEnabled] bit  NOT NULL,
    [AccessFailedCount] int  NOT NULL,
    [UserName] nvarchar(256)  NOT NULL,
    [SocialClubName] nvarchar(250)  NULL,
    [ImageURL] varchar(200)  NULL
);
GO

-- Creating table 'Blogs'
CREATE TABLE [dbo].[Blogs] (
    [BlogId] int IDENTITY(1,1) NOT NULL,
    [URL] varchar(250)  NULL,
    [ImageURL] varchar(250)  NULL,
    [IsEnabled] bit  NULL,
    [CategoryId] tinyint  NULL,
    [Title] nvarchar(400)  NULL,
    [Text] varchar(max)  NULL,
    [RegisterDate] datetime  NULL
);
GO

-- Creating table 'Categories'
CREATE TABLE [dbo].[Categories] (
    [CategoryId] tinyint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(70)  NULL
);
GO

-- Creating table 'Questions'
CREATE TABLE [dbo].[Questions] (
    [QuestionID] int IDENTITY(1,1) NOT NULL,
    [QuestionText] nvarchar(500)  NULL,
    [IsTextArea] bit  NOT NULL,
    [Selection_A] nvarchar(250)  NULL,
    [Selection_B] nvarchar(250)  NULL,
    [Selection_C] nvarchar(250)  NULL,
    [RightAnswer] tinyint  NULL,
    [RegisterDate] datetime  NULL
);
GO

-- Creating table 'QuestionToApplications'
CREATE TABLE [dbo].[QuestionToApplications] (
    [QuestionID] int  NOT NULL,
    [ApplicationID] int  NOT NULL
);
GO

-- Creating table 'Subscribtions'
CREATE TABLE [dbo].[Subscribtions] (
    [SubscribeId] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(100)  NULL,
    [Email] nvarchar(150)  NULL,
    [Message] nvarchar(750)  NULL,
    [RegisterDate] datetime  NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'TradeCategories'
CREATE TABLE [dbo].[TradeCategories] (
    [TradeCategoryId] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(150)  NULL
);
GO

-- Creating table 'TradeOffers'
CREATE TABLE [dbo].[TradeOffers] (
    [OfferId] int  NOT NULL,
    [TradeCategoryId] int  NOT NULL,
    [OfferOwnerUserID] nvarchar(128)  NULL,
    [OfferOwnerSocialClubID] nvarchar(200)  NULL,
    [IsActive] bit  NULL,
    [Title] nvarchar(150)  NULL,
    [Message] nvarchar(1000)  NULL,
    [ImageURL] varchar(200)  NULL,
    [RegisterDate] datetime  NULL
);
GO

-- Creating table 'AspNetUserRoles'
CREATE TABLE [dbo].[AspNetUserRoles] (
    [AspNetRoles_Id] nvarchar(128)  NOT NULL,
    [AspNetUsers_Id] nvarchar(128)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [MigrationId], [ContextKey] in table 'C__MigrationHistory'
ALTER TABLE [dbo].[C__MigrationHistory]
ADD CONSTRAINT [PK_C__MigrationHistory]
    PRIMARY KEY CLUSTERED ([MigrationId], [ContextKey] ASC);
GO

-- Creating primary key on [AnswerId] in table 'Answers'
ALTER TABLE [dbo].[Answers]
ADD CONSTRAINT [PK_Answers]
    PRIMARY KEY CLUSTERED ([AnswerId] ASC);
GO

-- Creating primary key on [ApplicationId] in table 'Applications'
ALTER TABLE [dbo].[Applications]
ADD CONSTRAINT [PK_Applications]
    PRIMARY KEY CLUSTERED ([ApplicationId] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetRoles'
ALTER TABLE [dbo].[AspNetRoles]
ADD CONSTRAINT [PK_AspNetRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [PK_AspNetUserClaims]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [LoginProvider], [ProviderKey], [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [PK_AspNetUserLogins]
    PRIMARY KEY CLUSTERED ([LoginProvider], [ProviderKey], [UserId] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUsers'
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [PK_AspNetUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [BlogId] in table 'Blogs'
ALTER TABLE [dbo].[Blogs]
ADD CONSTRAINT [PK_Blogs]
    PRIMARY KEY CLUSTERED ([BlogId] ASC);
GO

-- Creating primary key on [CategoryId] in table 'Categories'
ALTER TABLE [dbo].[Categories]
ADD CONSTRAINT [PK_Categories]
    PRIMARY KEY CLUSTERED ([CategoryId] ASC);
GO

-- Creating primary key on [QuestionID] in table 'Questions'
ALTER TABLE [dbo].[Questions]
ADD CONSTRAINT [PK_Questions]
    PRIMARY KEY CLUSTERED ([QuestionID] ASC);
GO

-- Creating primary key on [QuestionID], [ApplicationID] in table 'QuestionToApplications'
ALTER TABLE [dbo].[QuestionToApplications]
ADD CONSTRAINT [PK_QuestionToApplications]
    PRIMARY KEY CLUSTERED ([QuestionID], [ApplicationID] ASC);
GO

-- Creating primary key on [SubscribeId] in table 'Subscribtions'
ALTER TABLE [dbo].[Subscribtions]
ADD CONSTRAINT [PK_Subscribtions]
    PRIMARY KEY CLUSTERED ([SubscribeId] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [TradeCategoryId] in table 'TradeCategories'
ALTER TABLE [dbo].[TradeCategories]
ADD CONSTRAINT [PK_TradeCategories]
    PRIMARY KEY CLUSTERED ([TradeCategoryId] ASC);
GO

-- Creating primary key on [OfferId] in table 'TradeOffers'
ALTER TABLE [dbo].[TradeOffers]
ADD CONSTRAINT [PK_TradeOffers]
    PRIMARY KEY CLUSTERED ([OfferId] ASC);
GO

-- Creating primary key on [AspNetRoles_Id], [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [PK_AspNetUserRoles]
    PRIMARY KEY CLUSTERED ([AspNetRoles_Id], [AspNetUsers_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ApplicationId] in table 'Answers'
ALTER TABLE [dbo].[Answers]
ADD CONSTRAINT [FK_Answers_Applications]
    FOREIGN KEY ([ApplicationId])
    REFERENCES [dbo].[Applications]
        ([ApplicationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Answers_Applications'
CREATE INDEX [IX_FK_Answers_Applications]
ON [dbo].[Answers]
    ([ApplicationId]);
GO

-- Creating foreign key on [QuestionId] in table 'Answers'
ALTER TABLE [dbo].[Answers]
ADD CONSTRAINT [FK_Answers_Questions]
    FOREIGN KEY ([QuestionId])
    REFERENCES [dbo].[Questions]
        ([QuestionID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Answers_Questions'
CREATE INDEX [IX_FK_Answers_Questions]
ON [dbo].[Answers]
    ([QuestionId]);
GO

-- Creating foreign key on [UserId] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserClaims]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserLogins]
    ([UserId]);
GO

-- Creating foreign key on [OfferOwnerUserID] in table 'TradeOffers'
ALTER TABLE [dbo].[TradeOffers]
ADD CONSTRAINT [FK_TradeOffers_AspNetUsers]
    FOREIGN KEY ([OfferOwnerUserID])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TradeOffers_AspNetUsers'
CREATE INDEX [IX_FK_TradeOffers_AspNetUsers]
ON [dbo].[TradeOffers]
    ([OfferOwnerUserID]);
GO

-- Creating foreign key on [CategoryId] in table 'Blogs'
ALTER TABLE [dbo].[Blogs]
ADD CONSTRAINT [FK_Blogs_Categories]
    FOREIGN KEY ([CategoryId])
    REFERENCES [dbo].[Categories]
        ([CategoryId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Blogs_Categories'
CREATE INDEX [IX_FK_Blogs_Categories]
ON [dbo].[Blogs]
    ([CategoryId]);
GO

-- Creating foreign key on [QuestionID] in table 'QuestionToApplications'
ALTER TABLE [dbo].[QuestionToApplications]
ADD CONSTRAINT [FK_QuestionToApplications_Questions]
    FOREIGN KEY ([QuestionID])
    REFERENCES [dbo].[Questions]
        ([QuestionID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [TradeCategoryId] in table 'TradeOffers'
ALTER TABLE [dbo].[TradeOffers]
ADD CONSTRAINT [FK_TradeOffers_TradeCategories]
    FOREIGN KEY ([TradeCategoryId])
    REFERENCES [dbo].[TradeCategories]
        ([TradeCategoryId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TradeOffers_TradeCategories'
CREATE INDEX [IX_FK_TradeOffers_TradeCategories]
ON [dbo].[TradeOffers]
    ([TradeCategoryId]);
GO

-- Creating foreign key on [AspNetRoles_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRoles]
    FOREIGN KEY ([AspNetRoles_Id])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUsers]
    FOREIGN KEY ([AspNetUsers_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AspNetUserRoles_AspNetUsers'
CREATE INDEX [IX_FK_AspNetUserRoles_AspNetUsers]
ON [dbo].[AspNetUserRoles]
    ([AspNetUsers_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------