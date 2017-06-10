
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/26/2017 19:48:06
-- Generated from EDMX file: C:\Users\aban20\Desktop\copy5\PortfolioManager\Model\DataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [MFM5092];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_SecurityTypeDBInstrumentsDB]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[InstrumentsDBs] DROP CONSTRAINT [FK_SecurityTypeDBInstrumentsDB];
GO
IF OBJECT_ID(N'[dbo].[FK_InstrumentsDBOrderBookDB]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderBookDBs] DROP CONSTRAINT [FK_InstrumentsDBOrderBookDB];
GO
IF OBJECT_ID(N'[dbo].[FK_StockDBOptionsDB]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OptionsDBs] DROP CONSTRAINT [FK_StockDBOptionsDB];
GO
IF OBJECT_ID(N'[dbo].[FK_OptionsDBOptionKindDB]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OptionsDBs] DROP CONSTRAINT [FK_OptionsDBOptionKindDB];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[SecurityTypeDBs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SecurityTypeDBs];
GO
IF OBJECT_ID(N'[dbo].[InstrumentsDBs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InstrumentsDBs];
GO
IF OBJECT_ID(N'[dbo].[OrderBookDBs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderBookDBs];
GO
IF OBJECT_ID(N'[dbo].[OptionKindDBs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OptionKindDBs];
GO
IF OBJECT_ID(N'[dbo].[OptionsDBs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OptionsDBs];
GO
IF OBJECT_ID(N'[dbo].[StockDBs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StockDBs];
GO
IF OBJECT_ID(N'[dbo].[InterestRateDBs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InterestRateDBs];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'SecurityTypeDBs'
CREATE TABLE [dbo].[SecurityTypeDBs] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [TypeName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'InstrumentsDBs'
CREATE TABLE [dbo].[InstrumentsDBs] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Symbol] nvarchar(max)  NOT NULL,
    [SecurityTypeId] bigint  NOT NULL
);
GO

-- Creating table 'OrderBookDBs'
CREATE TABLE [dbo].[OrderBookDBs] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Position] nvarchar(max)  NOT NULL,
    [Quantity] bigint  NOT NULL,
    [TimeStamp] datetime  NOT NULL,
    [Price] float  NOT NULL,
    [InstrumentsId] bigint  NOT NULL,
    [FairPrice] float  NULL,
    [ProfitLoss] float  NULL,
    [Delta] float  NULL,
    [Theta] float  NULL,
    [Gamma] float  NULL,
    [Vega] float  NULL,
    [Rho] float  NULL
);
GO

-- Creating table 'OptionKindDBs'
CREATE TABLE [dbo].[OptionKindDBs] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [OptionKindName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'OptionsDBs'
CREATE TABLE [dbo].[OptionsDBs] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Issuer] nvarchar(max)  NOT NULL,
    [Symbol] nvarchar(max)  NOT NULL,
    [OptionType] nvarchar(max)  NOT NULL,
    [IsTradable] bit  NOT NULL,
    [LastTradedPrice] float  NOT NULL,
    [StrikePrice] float  NULL,
    [MaturityDate] datetime  NOT NULL,
    [ISIN] nvarchar(max)  NOT NULL,
    [Rebate] float  NULL,
    [Barrier] float  NULL,
    [BarrierOptionType] nvarchar(max)  NULL,
    [UnderlyingID] bigint  NOT NULL,
    [OptionKindID] bigint  NOT NULL
);
GO

-- Creating table 'StockDBs'
CREATE TABLE [dbo].[StockDBs] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Issuer] nvarchar(max)  NOT NULL,
    [Symbol] nvarchar(max)  NOT NULL,
    [LastTradedPrice] float  NOT NULL,
    [HistoricalVolatility] float  NOT NULL,
    [IsTradable] bit  NOT NULL,
    [ISIN] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'InterestRateDBs'
CREATE TABLE [dbo].[InterestRateDBs] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Tenor] datetime  NOT NULL,
    [Rate] float  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'SecurityTypeDBs'
ALTER TABLE [dbo].[SecurityTypeDBs]
ADD CONSTRAINT [PK_SecurityTypeDBs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'InstrumentsDBs'
ALTER TABLE [dbo].[InstrumentsDBs]
ADD CONSTRAINT [PK_InstrumentsDBs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderBookDBs'
ALTER TABLE [dbo].[OrderBookDBs]
ADD CONSTRAINT [PK_OrderBookDBs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OptionKindDBs'
ALTER TABLE [dbo].[OptionKindDBs]
ADD CONSTRAINT [PK_OptionKindDBs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OptionsDBs'
ALTER TABLE [dbo].[OptionsDBs]
ADD CONSTRAINT [PK_OptionsDBs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'StockDBs'
ALTER TABLE [dbo].[StockDBs]
ADD CONSTRAINT [PK_StockDBs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'InterestRateDBs'
ALTER TABLE [dbo].[InterestRateDBs]
ADD CONSTRAINT [PK_InterestRateDBs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [SecurityTypeId] in table 'InstrumentsDBs'
ALTER TABLE [dbo].[InstrumentsDBs]
ADD CONSTRAINT [FK_SecurityTypeDBInstrumentsDB]
    FOREIGN KEY ([SecurityTypeId])
    REFERENCES [dbo].[SecurityTypeDBs]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SecurityTypeDBInstrumentsDB'
CREATE INDEX [IX_FK_SecurityTypeDBInstrumentsDB]
ON [dbo].[InstrumentsDBs]
    ([SecurityTypeId]);
GO

-- Creating foreign key on [InstrumentsId] in table 'OrderBookDBs'
ALTER TABLE [dbo].[OrderBookDBs]
ADD CONSTRAINT [FK_InstrumentsDBOrderBookDB]
    FOREIGN KEY ([InstrumentsId])
    REFERENCES [dbo].[InstrumentsDBs]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_InstrumentsDBOrderBookDB'
CREATE INDEX [IX_FK_InstrumentsDBOrderBookDB]
ON [dbo].[OrderBookDBs]
    ([InstrumentsId]);
GO

-- Creating foreign key on [UnderlyingID] in table 'OptionsDBs'
ALTER TABLE [dbo].[OptionsDBs]
ADD CONSTRAINT [FK_StockDBOptionsDB]
    FOREIGN KEY ([UnderlyingID])
    REFERENCES [dbo].[StockDBs]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StockDBOptionsDB'
CREATE INDEX [IX_FK_StockDBOptionsDB]
ON [dbo].[OptionsDBs]
    ([UnderlyingID]);
GO

-- Creating foreign key on [OptionKindID] in table 'OptionsDBs'
ALTER TABLE [dbo].[OptionsDBs]
ADD CONSTRAINT [FK_OptionsDBOptionKindDB]
    FOREIGN KEY ([OptionKindID])
    REFERENCES [dbo].[OptionKindDBs]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OptionsDBOptionKindDB'
CREATE INDEX [IX_FK_OptionsDBOptionKindDB]
ON [dbo].[OptionsDBs]
    ([OptionKindID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------