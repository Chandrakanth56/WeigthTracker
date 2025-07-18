USE [WightList]
GO
/****** Object:  Table [dbo].[Farmer]    Script Date: 13-07-2025 6.05.08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Farmer](
	[FarmerID] [int] IDENTITY(1,1001) NOT NULL,
	[VendorID] [int] NULL,
	[FarmerName] [varchar](100) NOT NULL,
	[FarmerEmail] [varchar](100) NULL,
	[PassswordHAsh] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[FarmerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[FarmerEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vendor]    Script Date: 13-07-2025 6.05.08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendor](
	[VendorId] [int] IDENTITY(1,101) NOT NULL,
	[VendorName] [varchar](100) NOT NULL,
	[VendorEmail] [varchar](100) NULL,
	[PasswordHash] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[VendorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[VendorEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Weights]    Script Date: 13-07-2025 6.05.08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Weights](
	[WeightID] [int] IDENTITY(1,1) NOT NULL,
	[FarmerID] [int] NULL,
	[Weights] [float] NOT NULL,
	[Timestamp] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[WeightID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Weights] ADD  DEFAULT (getdate()) FOR [Timestamp]
GO
ALTER TABLE [dbo].[Farmer]  WITH CHECK ADD FOREIGN KEY([VendorID])
REFERENCES [dbo].[Vendor] ([VendorId])
GO
ALTER TABLE [dbo].[Weights]  WITH CHECK ADD FOREIGN KEY([FarmerID])
REFERENCES [dbo].[Farmer] ([FarmerID])
GO
/****** Object:  StoredProcedure [dbo].[sp_AddFarmer]    Script Date: 13-07-2025 6.05.08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_AddFarmer]
(
@VendorID int,
@FarmerName varchar(50),
@FarmerEmail VARCHAR(100),
@PasswordHash VARCHAR(50)
)
AS 
BEGIN
IF NOT EXISTS(SELECT 1 FROM Vendor WHERE VendorId=@VendorID)
RETURN;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AddWeight]    Script Date: 13-07-2025 6.05.08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_AddWeight]
(
@FarmerId INT, 
@Weight FLOAT
)
as 
begin 
	INSERT INTO Weights(FarmerId,Weights) VALUES (@FarmerId,@Weight)
END
GO
/****** Object:  StoredProcedure [dbo].[spEachFarmerRecordforVendor]    Script Date: 13-07-2025 6.05.08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[spEachFarmerRecordforVendor]
(
    @FarmerID INT,
    @VendorId INT
)
AS
BEGIN 
    SELECT 
        F.FarmerName, 
        W.Weights, 
        W.Timestamp 
    FROM 
        Farmer F
    INNER JOIN Weights W ON F.FarmerID = W.FarmerID
    INNER JOIN Vendor V ON F.VendorID = V.VendorID
    WHERE 
        F.FarmerID = @FarmerID 
        AND F.VendorID = @VendorId
END
GO
/****** Object:  StoredProcedure [dbo].[spGetNameOfFarmer]    Script Date: 13-07-2025 6.05.08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[spGetNameOfFarmer]
(
@VendorId INT
)
AS 
BEGIN
SELECT F.FarmerID ,F.FarmerName
FROM Farmer F JOIN Vendor V 
ON V.VendorId=F.VendorID
WHERE @VendorId=V.VendorId
END
GO
/****** Object:  StoredProcedure [dbo].[spShowFarmer]    Script Date: 13-07-2025 6.05.08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[spShowFarmer]
AS
BEGIN 
SELECT * FROM Farmer
END;
GO
