# Client_MVP_old

USE [floor_collection]
GO

Create doot collection
/****** Object:  Table [dbo].[door_collection_0]    Script Date: 11/02/2018 16:52:06 ******/
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


// create floor collection
USE [floor_collection]
GO

/****** Object:  Table [dbo].[floor_collection_0]    Script Date: 11/02/2018 16:52:43 ******/
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


// create image table
USE [floor_collection]
GO

/****** Object:  Table [dbo].[Images_door]    Script Date: 11/02/2018 16:52:57 ******/
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


