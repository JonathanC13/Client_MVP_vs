USE [master]
GO
/****** Object:  Database [floor_collection]    Script Date: 11/02/2018 17:02:29 ******/
CREATE DATABASE [floor_collection] ON  PRIMARY 
( NAME = N'floor_collection', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\floor_collection.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'floor_collection_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\floor_collection_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [floor_collection] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [floor_collection].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [floor_collection] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [floor_collection] SET ANSI_NULLS OFF
GO
ALTER DATABASE [floor_collection] SET ANSI_PADDING OFF
GO
ALTER DATABASE [floor_collection] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [floor_collection] SET ARITHABORT OFF
GO
ALTER DATABASE [floor_collection] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [floor_collection] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [floor_collection] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [floor_collection] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [floor_collection] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [floor_collection] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [floor_collection] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [floor_collection] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [floor_collection] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [floor_collection] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [floor_collection] SET  DISABLE_BROKER
GO
ALTER DATABASE [floor_collection] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [floor_collection] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [floor_collection] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [floor_collection] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [floor_collection] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [floor_collection] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [floor_collection] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [floor_collection] SET  READ_WRITE
GO
ALTER DATABASE [floor_collection] SET RECOVERY FULL
GO
ALTER DATABASE [floor_collection] SET  MULTI_USER
GO
ALTER DATABASE [floor_collection] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [floor_collection] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'floor_collection', N'ON'
GO
USE [floor_collection]
GO
/****** Object:  Table [dbo].[Images_door]    Script Date: 11/02/2018 17:02:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Images_door](
	[idImage] [int] IDENTITY(0,1) NOT NULL,
	[image_name] [varchar](50) NOT NULL,
	[image_saved] [varbinary](max) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[floor_collection_0]    Script Date: 11/02/2018 17:02:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[floor_collection_0](
	[idFloor] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[FloorOrder] [int] NOT NULL,
	[Image_floor] [varbinary](max) NOT NULL,
	[DoorID] [int] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[door_collection_0]    Script Date: 11/02/2018 17:02:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[door_collection_0](
	[idDoor] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[DoorID] [int] NOT NULL,
	[MarginLeft] [decimal](5, 2) NOT NULL,
	[MarginTop] [decimal](5, 2) NOT NULL,
	[MarginRight] [decimal](5, 2) NOT NULL,
	[MarginBot] [decimal](5, 2) NOT NULL,
	[DoorIP] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
