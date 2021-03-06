USE [master]
GO
/****** Object:  Database [MANvFATFootball]    Script Date: 22/08/2016 14:25:21 ******/
CREATE DATABASE [MANvFATFootball]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MANvFATFootball', FILENAME = N'D:\SQLDBs\MANvFATFootball_data.mdf' , SIZE = 18688KB , MAXSIZE = 512000KB , FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'MANvFATFootball_log', FILENAME = N'D:\SQLDBs\MANvFATFootball_log.ldf' , SIZE = 4672KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [MANvFATFootball] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MANvFATFootball].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MANvFATFootball] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MANvFATFootball] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MANvFATFootball] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MANvFATFootball] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MANvFATFootball] SET ARITHABORT OFF 
GO
ALTER DATABASE [MANvFATFootball] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MANvFATFootball] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MANvFATFootball] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MANvFATFootball] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MANvFATFootball] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MANvFATFootball] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MANvFATFootball] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MANvFATFootball] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MANvFATFootball] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MANvFATFootball] SET  ENABLE_BROKER 
GO
ALTER DATABASE [MANvFATFootball] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MANvFATFootball] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MANvFATFootball] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MANvFATFootball] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MANvFATFootball] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MANvFATFootball] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MANvFATFootball] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MANvFATFootball] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [MANvFATFootball] SET  MULTI_USER 
GO
ALTER DATABASE [MANvFATFootball] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MANvFATFootball] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MANvFATFootball] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MANvFATFootball] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [MANvFATFootball]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CalculateBMI]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <29-05-2016>
-- Description:	<Calculate BMI>
-- =============================================
CREATE FUNCTION [dbo].[fn_CalculateBMI]
(
	-- Add the parameters for the function here
	@Weight decimal(8,2),
	@HeightID bigint
)
RETURNS decimal(8,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @BMI decimal(8,2),
			@Height_InFeet decimal(8,2)

	-- Add the T-SQL statements to compute the return value here
	SELECT @Height_InFeet = (Height_Value/100.00) from Heights where HeightID = @HeightID

	set @BMI = (@Weight / @Height_InFeet) / @Height_InFeet

	-- Return the result of the function
	RETURN @BMI

END


GO
/****** Object:  UserDefinedFunction [dbo].[fn_CalculateBodyFat]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <29-05-2016>
-- Description:	<Calculate BodyFAT>
-- =============================================
CREATE FUNCTION [dbo].[fn_CalculateBodyFat]
(
	-- Add the parameters for the function here
	@Weight decimal(8,2),
	@HeightID bigint,
	@DOB datetime2

)
RETURNS decimal(8,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE 

	@BMI decimal(8,2),
			@Height_InFeet decimal(8,2),
			@Age int,
			@BodyFat decimal(8,2)

	-- Add the T-SQL statements to compute the return value here
	SELECT @Height_InFeet = (Height_Value/100.00) from Heights where HeightID = @HeightID

	set @BMI = (@Weight / @Height_InFeet) / @Height_InFeet

	set @Age = DATEDIFF(YY,@DOB,GetDAte())

	set @BodyFat = (1.2 * @BMI) + (0.23 * @Age) - 16.2

	-- Return the result of the function
	RETURN @BodyFat

END


GO
/****** Object:  UserDefinedFunction [dbo].[fn_CalculateNonTeamedPlayersPerLeague]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <20-06-2016>
-- Description:	<-e-scription, ->
-- =============================================
CREATE FUNCTION [dbo].[fn_CalculateNonTeamedPlayersPerLeague]
(
	-- Add the parameters for the function here
	@LeagueID bigint
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @TotalNonTeamedPlayers int

	-- Add the T-SQL statements to compute the return value here
Declare @tblNonPaymentPlayers table
(
PlayerID bigint
)	

INSERT INTO @tblNonPaymentPlayers
	select Distinct p.PlayerID
	from Players as p
LEFT Join PlayerPayments  as pp on p.PlayerID = pp.PlayerID
Left Join NoTeamPlayers as ntp on p.PlayerID = ntp.PlayerID
LEFT JOIN Leagues as L on ntp.LeagueID = L.LeagueID
where pp.PlayerPaymentID IS NULL OR pp.GoCardless_PaymentID IS NULL
AND pp.ManualPaymentReceived = 0
AND pp.PaidWithPayPal <> 1


select @TotalNonTeamedPlayers = count(*) from NoTeamPlayers where LeagueID = @LeagueID AND PlayerID NOT IN (select PlayerID from @tblNonPaymentPlayers)

	-- Return the result of the function
	RETURN @TotalNonTeamedPlayers

END

GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetPlayerRegWeight]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <05-05-2016>
-- Description:	<Get Player Starting/Registration Weight>
-- =============================================
CREATE FUNCTION [dbo].[fn_GetPlayerRegWeight]
(
	-- Add the parameters for the function here
	@PlayerID bigint
)
RETURNS decimal(8,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @PlayerRegWeight decimal(8,2)

	-- Add the T-SQL statements to compute the return value here
	select @PlayerRegWeight = [Weight] from Players where PlayerID = @PlayerID

	-- Return the result of the function
	RETURN @PlayerRegWeight

END


GO
/****** Object:  UserDefinedFunction [dbo].[fn_LeagueLossPercent]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <30-06-2016>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fn_LeagueLossPercent]
(
	-- Add the parameters for the function here
	@LeagueID bigint
)
RETURNS decimal(10,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @LeagueLossPercent as decimal(10,2)

	Declare @tblLeague as table
	(
	LeagueID bigint,
	PlayerID bigint,
	TotalLost decimal(10,2)

	)

	-- Add the T-SQL statements to compute the return value here
	Insert into @tblLeague
	select @LeagueID, pww.PlayerID,pww.TotalLost from PlayerWeightWeeks as pww
where PlayerID IN (Select p.PlayerID from Players as p
		INNER JOIN TeamPlayers as tp on p.PlayerID = tp.PlayerID
		INNER JOIN LeagueTeams as lt on tp.TeamID = lt.TeamID
		INNER JOIN Leagues as l on lt.LeagueID = l.LeagueID
		Where l.LeagueID = @LeagueID)

Declare @NumLostPlayers as int,
@TotalPlayers as int
select @NumLostPlayers = count(*) from @tblLeague
where TotalLost > 0


select @TotalPlayers = count(*) from @tblLeague
--where TotalLost <= 0

--select @NumLostPlayers as NumLostPlayers, @TotalPlayers as NumNotLostPlayers

if @NumLostPlayers = 0 
Begin
	set @LeagueLossPercent = 0.00
	End
else 
	Begin
	select @LeagueLossPercent = (Convert(decimal(10,2), Convert(decimal(10,2),@NumLostPlayers)/Convert(decimal(10,2),@TotalPlayers))*100.00)

--select @LeagueLossPercent as LeagueLossPercent
	End
	-- Return the result of the function
	RETURN @LeagueLossPercent

END

GO
/****** Object:  UserDefinedFunction [dbo].[fn_LeagueTotalLossKG]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <02/07/2016>
-- Description:	<Item # 4 Admin Dashboard>
-- =============================================
CREATE FUNCTION [dbo].[fn_LeagueTotalLossKG]
(
	-- Add the parameters for the function here
	@LeagueID bigint
)
RETURNS decimal(10,2)
AS
BEGIN
Declare @LeagueTotalLossKG decimal(10,2)
	-- Declare the return variable here
	
	select  @LeagueTotalLossKG = SUM(pww.TotalLost) from PlayerWeightWeeks as pww
where PlayerID IN (Select p.PlayerID from Players as p
		INNER JOIN TeamPlayers as tp on p.PlayerID = tp.PlayerID
		INNER JOIN LeagueTeams as lt on tp.TeamID = lt.TeamID
		INNER JOIN Leagues as l on lt.LeagueID = l.LeagueID
		Where l.LeagueID = @LeagueID)


	-- Return the result of the function
	RETURN ISNULL(@LeagueTotalLossKG,0.00)

END

GO
/****** Object:  UserDefinedFunction [dbo].[fn_LossPercentage]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fn_LossPercentage]
(
	-- Add the parameters for the function here
	@LossWk1 decimal(8,2)
)
RETURNS decimal(8,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @LossPercent decimal(8,2)

	-- Add the T-SQL statements to compute the return value here
	SELECT @LossPercent = @LossWk1+2

	-- Return the result of the function
	RETURN @LossPercent

END


GO
/****** Object:  UserDefinedFunction [dbo].[fn_PlayerTotalLossWeeks]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <20/07/2016>
-- Description:	<To Calculate Hatrick this function will calculate total weeks player have loss weight>
-- =============================================
CREATE FUNCTION [dbo].[fn_PlayerTotalLossWeeks]
(
	-- Add the parameters for the function here
	@PlayerID bigint
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	Declare @TotalLossWeeks int
select  @TotalLossWeeks = sum(t.valcount)
  from (
         select pww.PlayerID AS d
			,   CASE WHEN pww.LossWk1 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk2 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk3 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk4 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk5 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk6 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk7 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk8 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk9 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk10 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk11 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk12 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk13 > 0 THEN 1 ELSE 0 END
				+ CASE WHEN pww.LossWk14 > 0 THEN 1 ELSE 0 END
                       AS valcount
       from PlayerWeightWeeks as pww
	   where pww.PlayerID = @PlayerID
       ) t
group by t.d
     ;

--select @TotalLossWeeks as TotalLossWeeks

	-- Return the result of the function
	RETURN @TotalLossWeeks

END

GO
/****** Object:  UserDefinedFunction [dbo].[GenerateFullName]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Usman Akram>
-- Create date: <04-04-2016>
-- Description:	<Generate FullName as [Title]+[FirstName] [LastName]>
-- =============================================
CREATE FUNCTION [dbo].[GenerateFullName]
(
	-- Add the parameters for the function here
	@TitleID bigint,
	@FirstName varchar(50),
	@LastName varchar(50)
)
RETURNS varchar(255)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Fullname varchar(255),
	@Title varchar(50)


	-- Add the T-SQL statements to compute the return value here
	select @Title = Title from Titles where TitleID = @TitleID

	set @Fullname = @Title+' '+@FirstName+' '+@LastName

	-- Return the result of the function
	RETURN @Fullname

END



GO
/****** Object:  UserDefinedFunction [dbo].[TeamTotalDraw]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-05-2016>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[TeamTotalDraw]
(
	-- Add the parameters for the function here
	@TeamID bigint,
	@TypeID int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	Declare @TotalDraw int
set @TotalDraw  = 0

if @TypeID = 1 -- 1=> Weight and Goals using Totals
Begin
	 
	 select @TotalDraw = @TotalDraw + Count(*)
	 from LeagueFixtures as LF
	 where HomeTeamID = @TeamID
	 AND HomeTeamTotal = AwayTeamTotal

	 select @TotalDraw = @TotalDraw + Count(*)
	 from LeagueFixtures as LF
	 where AwayTeamID = @TeamID
	 AND AwayTeamTotal = HomeTeamTotal

 End
 Else If @TypeID = 2 -- 2 => Pitch using Scores Only
 Begin
  
	select @TotalDraw = @TotalDraw + Count(*)
	 from LeagueFixtures as LF
	 where HomeTeamID = @TeamID
	 AND HomeTeamScore = AwayTeamScore

	 select @TotalDraw = @TotalDraw + Count(*)
	 from LeagueFixtures as LF
	 where AwayTeamID = @TeamID
	 AND AwayTeamScore = HomeTeamScore

 End

  Else If @TypeID = 3 -- 3 => Scalse using Weight Only
 Begin
  
	select @TotalDraw = @TotalDraw + Count(*)
	 from LeagueFixtures as LF
	 where HomeTeamID = @TeamID
	 AND HomeTeamWeight = AwayTeamWeight

	 select @TotalDraw = @TotalDraw + Count(*)
	 from LeagueFixtures as LF
	 where AwayTeamID = @TeamID
	 AND AwayTeamWeight = HomeTeamWeight

 End
 --select @TotalWon as TotalWon


	-- Return the result of the function
	RETURN @TotalDraw

END


GO
/****** Object:  UserDefinedFunction [dbo].[TeamTotalGoalsAgainst]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-05-2016>
-- Description:	<When Other Team Make Goals Against Current Team>
-- =============================================
CREATE FUNCTION [dbo].[TeamTotalGoalsAgainst]
(
	-- Add the parameters for the function here
	@TeamID bigint,
	@TypeID int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
Declare @TotalGoalsAgainst int
set @TotalGoalsAgainst  = 0

if @TypeID = 1 -- 1=> Weight and Goals using Totals
Begin
	select @TotalGoalsAgainst = @TotalGoalsAgainst + ISNULL(SUM(ISNULL(AwayTeamTotal,0)),0)
	 from LeagueFixtures as LF
	 where HomeTeamID = @TeamID
	 --AND HomeTeamTotal = AwayTeamTotal

	 select @TotalGoalsAgainst = @TotalGoalsAgainst + ISNULL(SUM(ISNULL(HomeTeamTotal,0)),0)
	 from LeagueFixtures as LF
	 where AwayTeamID = @TeamID

 End
 Else If @TypeID = 2 -- 2 => Pitch using Scores Only
 Begin

		select @TotalGoalsAgainst = @TotalGoalsAgainst + ISNULL(SUM(ISNULL(AwayTeamScore,0)),0)
	 from LeagueFixtures as LF
	 where HomeTeamID = @TeamID
	 --AND HomeTeamTotal = AwayTeamTotal

	 select @TotalGoalsAgainst = @TotalGoalsAgainst + ISNULL(SUM(ISNULL(HomeTeamScore,0)),0)
	 from LeagueFixtures as LF
	 where AwayTeamID = @TeamID
 End

  Else If @TypeID = 3 -- 3 => Scalse using Weight Only
 Begin

		select @TotalGoalsAgainst = @TotalGoalsAgainst + ISNULL(SUM(ISNULL(AwayTeamWeight,0)),0)
	 from LeagueFixtures as LF
	 where HomeTeamID = @TeamID
	 --AND HomeTeamTotal = AwayTeamTotal

	 select @TotalGoalsAgainst = @TotalGoalsAgainst + ISNULL(SUM(ISNULL(HomeTeamWeight,0)),0)
	 from LeagueFixtures as LF
	 where AwayTeamID = @TeamID
 End


 
	-- Return the result of the function
	RETURN @TotalGoalsAgainst

END


GO
/****** Object:  UserDefinedFunction [dbo].[TeamTotalGoalsDifference]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-05-2016>
-- Description:	<Total Goal Difference GD>
-- =============================================
CREATE FUNCTION [dbo].[TeamTotalGoalsDifference]
(
	-- Add the parameters for the function here
	@TeamID bigint,
	@TypeID int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
Declare @TotalGoalsDifference int
set @TotalGoalsDifference  = 0

select @TotalGoalsDifference = dbo.TeamTotalGoalsFor(@TeamID,@TypeID) - dbo.TeamTotalGoalsAgainst(@TeamID,@TypeID)

	-- Return the result of the function
	RETURN @TotalGoalsDifference

END


GO
/****** Object:  UserDefinedFunction [dbo].[TeamTotalGoalsFor]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-05-2016>
-- Description:	<When Team Make Goals for other Teams>
-- =============================================
CREATE FUNCTION [dbo].[TeamTotalGoalsFor]
(
	-- Add the parameters for the function here
	@TeamID bigint,
	@TypeID int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
Declare @TotalGoalsFor int
set @TotalGoalsFor  = 0


if @TypeID = 1 -- 1=> Weight and Goals using Totals
Begin
select @TotalGoalsFor = @TotalGoalsFor + ISNULL(SUM(ISNULL(HomeTeamTotal,0)),0)
 from LeagueFixtures as LF
 where HomeTeamID = @TeamID
 --AND HomeTeamTotal = AwayTeamTotal

 select @TotalGoalsFor = @TotalGoalsFor + ISNULL(SUM(ISNULL(AwayTeamTotal,0)),0)
 from LeagueFixtures as LF
 where AwayTeamID = @TeamID
 
 End
 Else If @TypeID = 2 -- 2 => Pitch using Scores Only
 Begin

 select @TotalGoalsFor = @TotalGoalsFor + ISNULL(SUM(ISNULL(HomeTeamScore,0)),0)
 from LeagueFixtures as LF
 where HomeTeamID = @TeamID
 --AND HomeTeamTotal = AwayTeamTotal

 select @TotalGoalsFor = @TotalGoalsFor + ISNULL(SUM(ISNULL(AwayTeamScore,0)),0)
 from LeagueFixtures as LF
 where AwayTeamID = @TeamID

End

  Else If @TypeID = 3 -- 3 => Scalse using Weight Only
 Begin
  select @TotalGoalsFor = @TotalGoalsFor + ISNULL(SUM(ISNULL(HomeTeamWeight,0)),0)
 from LeagueFixtures as LF
 where HomeTeamID = @TeamID
 --AND HomeTeamTotal = AwayTeamTotal

 select @TotalGoalsFor = @TotalGoalsFor + ISNULL(SUM(ISNULL(AwayTeamWeight,0)),0)
 from LeagueFixtures as LF
 where AwayTeamID = @TeamID
 End
	-- Return the result of the function
	RETURN @TotalGoalsFor

END


GO
/****** Object:  UserDefinedFunction [dbo].[TeamTotalLoss]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-05-2016>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[TeamTotalLoss]
(
	-- Add the parameters for the function here
	@TeamID bigint,
	@TypeID int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	Declare @TotalLoss int
set @TotalLoss  = 0


if @TypeID = 1 -- 1=> Weight and Goals using Totals
Begin

select @TotalLoss = @TotalLoss + Count(*)
 from LeagueFixtures as LF
 where HomeTeamID = @TeamID
 AND HomeTeamTotal < AwayTeamTotal

 select @TotalLoss = @TotalLoss + Count(*)
 from LeagueFixtures as LF
 where AwayTeamID = @TeamID
 AND AwayTeamTotal < HomeTeamTotal
 
 End
 Else If @TypeID = 2 -- 2 => Pitch using Scores Only
 Begin

 select @TotalLoss = @TotalLoss + Count(*)
 from LeagueFixtures as LF
 where HomeTeamID = @TeamID
 AND HomeTeamScore < AwayTeamScore

 select @TotalLoss = @TotalLoss + Count(*)
 from LeagueFixtures as LF
 where AwayTeamID = @TeamID
 AND AwayTeamScore < HomeTeamScore

End

  Else If @TypeID = 3 -- 3 => Scalse using Weight Only
 Begin

 select @TotalLoss = @TotalLoss + Count(*)
 from LeagueFixtures as LF
 where HomeTeamID = @TeamID
 AND HomeTeamWeight < AwayTeamWeight

 select @TotalLoss = @TotalLoss + Count(*)
 from LeagueFixtures as LF
 where AwayTeamID = @TeamID
 AND AwayTeamWeight < HomeTeamWeight

 End
	-- Return the result of the function
	RETURN @TotalLoss

END


GO
/****** Object:  UserDefinedFunction [dbo].[TeamTotalNumPlayed]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-05-2016>
-- Description:	<Team total times played>
-- =============================================
CREATE FUNCTION [dbo].[TeamTotalNumPlayed]
(
	-- Add the parameters for the function here
	@TeamID bigint,
	@TypeID int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @TotalPlayed int

if @TypeID = 1 -- 1=> Weight and Goals using Totals
Begin
		select @TotalPlayed = Count(*)
	 from LeagueFixtures as LF
	 where HomeTeamID = @TeamID OR AwayTeamID = @TeamID

 End
 Else If @TypeID = 2 OR @TypeID = 3-- 2 => Pitch using Scores Only, in Excel Sheet it uses pitch P in sacles P
 Begin
  
  select @TotalPlayed = dbo.TeamTotalWon(@TeamID,2) + dbo.TeamTotalDraw(@TeamID,2) + dbo.TeamTotalLoss(@TeamID,2)
  
 End


	-- Return the result of the function
	RETURN @TotalPlayed

END


GO
/****** Object:  UserDefinedFunction [dbo].[TeamTotalPts]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-05-2016>
-- Description:	<Total Pts = (Total Won * 2) + Total Draw Draw>
-- =============================================
CREATE FUNCTION [dbo].[TeamTotalPts]
(
	-- Add the parameters for the function here
	@TeamID bigint,
	@TypeID int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
Declare @TotalPts int
set @TotalPts  = 0

select @TotalPts = (dbo.TeamTotalWon(@TeamID,@TypeID)*2) + dbo.TeamTotalDraw(@TeamID,@TypeID)

	-- Return the result of the function
	RETURN @TotalPts

END


GO
/****** Object:  UserDefinedFunction [dbo].[TeamTotalWon]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-05-2016>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[TeamTotalWon]
(
	-- Add the parameters for the function here
	@TeamID bigint,
	@TypeID int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	Declare @TotalWon int
set @TotalWon  = 0


if @TypeID = 1 -- 1=> Weight and Goals using Totals
Begin


		select @TotalWon = @TotalWon + Count(*)
		 from LeagueFixtures as LF
		 where HomeTeamID = @TeamID
		 AND HomeTeamTotal > AwayTeamTotal

		 select @TotalWon = @TotalWon + Count(*)
		 from LeagueFixtures as LF
		 where AwayTeamID = @TeamID
		 AND AwayTeamTotal > HomeTeamTotal
 
 End
 Else If @TypeID = 2 -- 2 => Pitch using Scores Only
 Begin

		 select @TotalWon = @TotalWon + Count(*)
		 from LeagueFixtures as LF
		 where HomeTeamID = @TeamID
		 AND HomeTeamScore > AwayTeamScore

		 select @TotalWon = @TotalWon + Count(*)
		 from LeagueFixtures as LF
		 where AwayTeamID = @TeamID
		 AND AwayTeamScore > HomeTeamScore

End

  Else If @TypeID = 3 -- 3 => Scalse using Weight Only
 Begin
 		 select @TotalWon = @TotalWon + Count(*)
		 from LeagueFixtures as LF
		 where HomeTeamID = @TeamID
		 AND HomeTeamWeight > AwayTeamWeight

		 select @TotalWon = @TotalWon + Count(*)
		 from LeagueFixtures as LF
		 where AwayTeamID = @TeamID
		 AND AwayTeamWeight > HomeTeamWeight

End

	-- Return the result of the function
	RETURN @TotalWon

END


GO
/****** Object:  Table [dbo].[Advertisements]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Advertisements](
	[AdvertisementID] [bigint] IDENTITY(1,1) NOT NULL,
	[Advertisement] [varchar](50) NOT NULL,
	[SortBy] [int] NOT NULL CONSTRAINT [DF_Advertisements_SortBy]  DEFAULT ((1)),
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Advertisements] PRIMARY KEY CLUSTERED 
(
	[AdvertisementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AuditLogs]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AuditLogs](
	[AuditLogID] [bigint] IDENTITY(1,1) NOT NULL,
	[LogDate] [datetime2](7) NOT NULL,
	[AuditLogShortDesc] [varchar](255) NOT NULL,
	[AuditLogLongDesc] [varchar](max) NOT NULL,
	[UserID] [bigint] NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED 
(
	[AuditLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Cities]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Cities](
	[CityID] [bigint] IDENTITY(1,1) NOT NULL,
	[RegionID] [bigint] NOT NULL,
	[CityName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED 
(
	[CityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Countries]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Countries](
	[CountryID] [bigint] IDENTITY(1,1) NOT NULL,
	[CountryName] [varchar](50) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[CountryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ELMAH_Error]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ELMAH_Error](
	[ErrorId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_ELMAH_Error_ErrorId]  DEFAULT (newid()),
	[Application] [nvarchar](60) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[AllXml] [ntext] NOT NULL,
 CONSTRAINT [PK_ELMAH_Error] PRIMARY KEY NONCLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Heights]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Heights](
	[HeightID] [bigint] IDENTITY(1,1) NOT NULL,
	[Height_Value] [decimal](10, 2) NOT NULL,
	[Height_Display] [varchar](50) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Heights] PRIMARY KEY CLUSTERED 
(
	[HeightID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HowActives]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HowActives](
	[HowActiveID] [bigint] IDENTITY(1,1) NOT NULL,
	[HowActive] [varchar](100) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_HowActives] PRIMARY KEY CLUSTERED 
(
	[HowActiveID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LeagueFixtures]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LeagueFixtures](
	[LeagueFixtureID] [bigint] IDENTITY(1,1) NOT NULL,
	[LeagueID] [bigint] NOT NULL,
	[MatchNum] [int] NULL,
	[WeekNum] [int] NOT NULL,
	[FixtureDateTime] [datetime2](7) NOT NULL,
	[HomeTeamID] [bigint] NOT NULL,
	[HomeTeamScore] [int] NULL,
	[HomeTeamWeight] [decimal](8, 2) NULL,
	[HomeTeamTotal]  AS ([HomeTeamScore]+[HomeTeamWeight]),
	[AwayTeamID] [bigint] NOT NULL,
	[AwayTeamScore] [int] NULL,
	[AwayTeamWeight] [decimal](8, 2) NULL,
	[AwayTeamTotal]  AS ([AwayTeamScore]+[AwayTeamWeight]),
	[ManOfTheMatchPlayerID] [bigint] NULL,
	[RowVersion] [timestamp] NOT NULL,
	[AutoCalcTeamWeight] [bit] NOT NULL CONSTRAINT [DF_LeagueFixtures_AutoCalcTeamWeight]  DEFAULT ((1)),
 CONSTRAINT [PK_LeagueFixtures] PRIMARY KEY CLUSTERED 
(
	[LeagueFixtureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Leagues]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Leagues](
	[LeagueID] [bigint] IDENTITY(1,1) NOT NULL,
	[CityID] [bigint] NOT NULL,
	[LeagueName] [varchar](500) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[Display] [bit] NOT NULL DEFAULT ((1)),
	[DateTimeFrom] [datetime2](7) NULL,
	[DateTimeTo] [datetime2](7) NULL,
	[IsLocalAuthorityScheme] [bit] NOT NULL CONSTRAINT [DF_Leagues_IsLocalAuthorityScheme]  DEFAULT ((0)),
	[NewsTag] [varchar](255) NULL,
 CONSTRAINT [PK_Leagues] PRIMARY KEY CLUSTERED 
(
	[LeagueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LeagueTeams]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LeagueTeams](
	[LeagueTeamID] [bigint] IDENTITY(1,1) NOT NULL,
	[LeagueID] [bigint] NOT NULL,
	[TeamID] [bigint] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_LeagueTeams] PRIMARY KEY CLUSTERED 
(
	[LeagueTeamID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[News]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[News](
	[NewsID] [bigint] IDENTITY(1,1) NOT NULL,
	[NewsDateTime] [datetime2](7) NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[ShortDesc] [varchar](200) NOT NULL,
	[LongDesc] [varchar](max) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_News_Active]  DEFAULT ((1)),
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_News] PRIMARY KEY CLUSTERED 
(
	[NewsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NoTeamPlayers]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoTeamPlayers](
	[NoTeamPlayerID] [bigint] IDENTITY(1,1) NOT NULL,
	[PlayerID] [bigint] NOT NULL,
	[LeagueID] [bigint] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_NoTeamPlayers] PRIMARY KEY CLUSTERED 
(
	[NoTeamPlayerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PageContents]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PageContents](
	[PageContentID] [bigint] IDENTITY(1,1) NOT NULL,
	[PageName] [varchar](50) NOT NULL,
	[PageContent] [varchar](max) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_PageContents] PRIMARY KEY CLUSTERED 
(
	[PageContentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[payments]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[payments](
	[id] [nvarchar](255) NULL,
	[created_at] [nvarchar](255) NULL,
	[charge_date] [datetime] NULL,
	[amount] [float] NULL,
	[description] [nvarchar](255) NULL,
	[currency] [nvarchar](255) NULL,
	[status] [nvarchar](255) NULL,
	[amount_refunded] [float] NULL,
	[reference] [nvarchar](255) NULL,
	[transaction_fee] [float] NULL,
	[payout_date] [datetime] NULL,
	[app_fee] [float] NULL,
	[links#mandate] [nvarchar](255) NULL,
	[links#creditor] [nvarchar](255) NULL,
	[links#customer] [nvarchar](255) NULL,
	[customer#id] [nvarchar](255) NULL,
	[customer#created_at] [nvarchar](255) NULL,
	[customer#email] [nvarchar](255) NULL,
	[customer#given_name] [nvarchar](255) NULL,
	[customer#family_name] [nvarchar](255) NULL,
	[customer#company_name] [nvarchar](255) NULL,
	[customer#address_line1] [nvarchar](255) NULL,
	[customer#address_line2] [nvarchar](255) NULL,
	[customer#address_line3] [nvarchar](255) NULL,
	[customer#city] [nvarchar](255) NULL,
	[customer#region] [nvarchar](255) NULL,
	[customer#postal_code] [nvarchar](255) NULL,
	[customer#country_code] [nvarchar](255) NULL,
	[customer#language] [nvarchar](255) NULL,
	[customer#swedish_identity_number] [nvarchar](255) NULL,
	[customer#active_mandates] [bit] NOT NULL,
	[links#subscription] [nvarchar](255) NULL,
	[links#payout] [nvarchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PaymentStatuses]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PaymentStatuses](
	[PaymentStatusID] [bigint] IDENTITY(1,1) NOT NULL,
	[PaymentStatus] [varchar](50) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_PaymentStatuses] PRIMARY KEY CLUSTERED 
(
	[PaymentStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PayPalResponses]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PayPalResponses](
	[ResponseID] [bigint] IDENTITY(1,1) NOT NULL,
	[ResponseDateTime] [datetime] NOT NULL,
	[PaypalResponse] [varchar](max) NULL,
 CONSTRAINT [PK_PayPalResponses] PRIMARY KEY CLUSTERED 
(
	[ResponseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PayPalTransactions]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PayPalTransactions](
	[TransactionID] [bigint] IDENTITY(1,1) NOT NULL,
	[TransactionDateTime] [datetime] NOT NULL,
	[PlayerID] [bigint] NULL,
	[PaylinkID] [varchar](50) NULL,
	[CurrencyCode] [varchar](4) NULL,
	[AmountInCurrency] [varchar](50) NULL,
	[PayPalTransactionID] [varchar](max) NULL,
	[PayPalAmount] [varchar](50) NULL,
	[PayPalPaymentStatus] [varchar](50) NULL,
	[PayPalBuyerEmail] [varchar](50) NULL,
	[PayPalBuyerFirstname] [varchar](50) NULL,
	[PayPalLastname] [varchar](50) NULL,
	[PayPalBuyerAddress] [varchar](max) NULL,
	[PayPalBuyerAddressStatus] [varchar](50) NULL,
	[PayPalPaymentDate] [varchar](50) NULL,
	[PayPalBuyerStatus] [varchar](50) NULL,
	[PayPalBuyerID] [varchar](max) NULL,
	[PayPalVerifySign] [varchar](max) NULL,
	[PayPalIPNTrackID] [varchar](max) NULL,
	[PayPalBuyerNotes] [varchar](max) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_PayPalTransactions] PRIMARY KEY CLUSTERED 
(
	[TransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlayerImages]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlayerImages](
	[PlayerImageID] [bigint] IDENTITY(1,1) NOT NULL,
	[PlayerID] [bigint] NOT NULL,
	[FileName] [varchar](200) NOT NULL,
	[IsAnimated] [bit] NOT NULL CONSTRAINT [DF_PlayerImages_IsAnimated]  DEFAULT ((0)),
	[Display] [bit] NOT NULL CONSTRAINT [DF_PlayerImages_Display]  DEFAULT ((1)),
	[DefaultImage] [bit] NOT NULL CONSTRAINT [DF_PlayerImages_DefaultImage]  DEFAULT ((0)),
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_PlayerImages] PRIMARY KEY CLUSTERED 
(
	[PlayerImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlayerPayments]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlayerPayments](
	[PlayerPaymentID] [bigint] IDENTITY(1,1) NOT NULL,
	[PlayerID] [bigint] NOT NULL,
	[Amount] [decimal](8, 2) NOT NULL,
	[PaymentDateTime] [datetime] NOT NULL,
	[PaymentStatusID] [bigint] NOT NULL,
	[GoCardless_PaymentID] [varchar](100) NULL,
	[GoCardless_PaymentCancelled] [bit] NOT NULL CONSTRAINT [DF_PlayerPayments_GoCardless_PaymentCancelled]  DEFAULT ((0)),
	[PaymentCancellation_DateTime] [datetime2](7) NULL,
	[PaymentCancellation_Reason] [varchar](max) NULL,
	[PaymentCancellation_ByUserID] [bigint] NULL,
	[GoCardless_CustomerID] [varchar](100) NULL,
	[GoCardless_BankAccountID] [varchar](100) NULL,
	[GoCardless_Token] [varchar](max) NULL,
	[BookRequested] [bit] NOT NULL CONSTRAINT [DF_PlayerPayments_BookRequested]  DEFAULT ((0)),
	[ManualPaymentReceived] [bit] NOT NULL CONSTRAINT [DF_PlayerPayments_ManualPaymentReceived]  DEFAULT ((0)),
	[PaidWithPayPal] [bit] NOT NULL CONSTRAINT [DF_PlayerPayments_PaidWithPayPal]  DEFAULT ((0)),
	[PaylinkID] [varchar](50) NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[PlayerPaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Players]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Players](
	[PlayerID] [bigint] IDENTITY(1,1) NOT NULL,
	[TitleID] [bigint] NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[FullName]  AS ([dbo].[GenerateFullName]([TitleID],[FirstName],[LastName])),
	[EmailAddress] [varchar](50) NOT NULL,
	[DOB] [datetime2](7) NULL,
	[Age]  AS (datediff(year,[DOB],getdate())),
	[Weight] [decimal](10, 2) NOT NULL,
	[WorkPhone] [varchar](20) NULL,
	[Mobile] [varchar](20) NULL,
	[Landline] [varchar](20) NULL,
	[HeightID] [bigint] NOT NULL CONSTRAINT [DF_Players_HeightID]  DEFAULT ((1)),
	[BMI]  AS ([dbo].[fn_CalculateBMI]([Weight],[HeightID])),
	[BodyFat]  AS ([dbo].[fn_CalculateBodyFat]([Weight],[HeightID],[DOB])),
	[HowActiveID] [bigint] NOT NULL CONSTRAINT [DF_Players_HowActiveID]  DEFAULT ((1)),
	[PositionID] [bigint] NOT NULL CONSTRAINT [DF_Players_PositionID]  DEFAULT ((1)),
	[AddressLine1] [varchar](100) NULL,
	[AddressLine2] [varchar](100) NULL,
	[Country] [varchar](50) NULL,
	[City] [varchar](50) NULL,
	[PostCode] [varchar](50) NULL,
	[Notes] [varchar](max) NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_Players_Active]  DEFAULT ((1)),
	[Featured] [bit] NOT NULL CONSTRAINT [DF_Players_Featured]  DEFAULT ((0)),
	[RowVersion] [timestamp] NOT NULL,
	[RegistrationDate] [datetime2](7) NOT NULL CONSTRAINT [DF_Players_RegistrationDate]  DEFAULT (getdate()),
	[AdvertisementID] [bigint] NULL,
	[AdvertisementOther] [bit] NOT NULL CONSTRAINT [DF_Players_AdvertisementOther]  DEFAULT ((0)),
	[AdvertisementOtherDetails] [varchar](max) NULL,
 CONSTRAINT [PK_Players] PRIMARY KEY CLUSTERED 
(
	[PlayerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlayerWeights]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayerWeights](
	[PlayerWeightID] [bigint] IDENTITY(1,1) NOT NULL,
	[PlayerID] [bigint] NOT NULL,
	[RecordDate] [date] NOT NULL,
	[WeekNum] [int] NOT NULL,
	[Weight] [decimal](8, 2) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_PlayerWeights] PRIMARY KEY CLUSTERED 
(
	[PlayerWeightID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlayerWeightWeeks]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayerWeightWeeks](
	[PlayerWeightWeekID] [bigint] IDENTITY(1,1) NOT NULL,
	[PlayerID] [bigint] NOT NULL,
	[Wk1] [decimal](8, 2) NULL,
	[LossWk1]  AS ([dbo].[fn_GetPlayerRegWeight]([PlayerID])-[Wk1]),
	[Wk2] [decimal](8, 2) NULL,
	[LossWk2]  AS ([Wk1]-[Wk2]),
	[Wk3] [decimal](8, 2) NULL,
	[LossWk3]  AS ([Wk2]-[Wk3]),
	[Wk4] [decimal](8, 2) NULL,
	[LossWk4]  AS ([Wk3]-[Wk4]),
	[Wk5] [decimal](8, 2) NULL,
	[LossWk5]  AS ([Wk4]-[Wk5]),
	[Wk6] [decimal](8, 2) NULL,
	[LossWk6]  AS ([Wk5]-[Wk6]),
	[Wk7] [decimal](8, 2) NULL,
	[LossWk7]  AS ([Wk6]-[Wk7]),
	[Wk8] [decimal](8, 2) NULL,
	[LossWk8]  AS ([Wk7]-[Wk8]),
	[Wk9] [decimal](8, 2) NULL,
	[LossWk9]  AS ([Wk8]-[Wk9]),
	[Wk10] [decimal](8, 2) NULL,
	[LossWk10]  AS ([Wk9]-[Wk10]),
	[Wk11] [decimal](8, 2) NULL,
	[LossWk11]  AS ([Wk10]-[Wk11]),
	[Wk12] [decimal](8, 2) NULL,
	[LossWk12]  AS ([Wk11]-[Wk12]),
	[Wk13] [decimal](8, 2) NULL,
	[LossWk13]  AS ([Wk12]-[Wk13]),
	[Wk14] [decimal](8, 2) NULL,
	[LossWk14]  AS ([Wk13]-[Wk14]),
	[LossPercent] [decimal](8, 2) NULL,
	[Five_BWV]  AS ([dbo].[fn_GetPlayerRegWeight]([PlayerID])*(0.05)),
	[Ten_BWV]  AS ([dbo].[fn_GetPlayerRegWeight]([PlayerID])*(0.1)),
	[TotalLost] [decimal](8, 2) NULL,
	[CurrentBMI] [decimal](8, 2) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_PlayerWeightWeeks] PRIMARY KEY CLUSTERED 
(
	[PlayerWeightWeekID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Positions]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Positions](
	[PositionID] [bigint] IDENTITY(1,1) NOT NULL,
	[Position] [varchar](100) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Positions] PRIMARY KEY CLUSTERED 
(
	[PositionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Regions]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Regions](
	[RegionID] [bigint] IDENTITY(1,1) NOT NULL,
	[CountryID] [bigint] NOT NULL,
	[RegionName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED 
(
	[RegionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [bigint] IDENTITY(1,1) NOT NULL,
	[Role] [varchar](50) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SystemSettings]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SystemSettings](
	[SystemSettingID] [bigint] IDENTITY(1,1) NOT NULL,
	[EmailsEnabled] [bit] NOT NULL CONSTRAINT [DF_SystemSettings_EmailsEnabled]  DEFAULT ((0)),
	[SendTempEmail] [bit] NOT NULL CONSTRAINT [DF_SystemSettings_SendTempEmail]  DEFAULT ((0)),
	[SupportEmailAddress] [varchar](100) NOT NULL,
	[TempEmailAddress] [varchar](100) NOT NULL,
	[CurrentDomain] [varchar](100) NOT NULL,
	[AdminEmailAddress] [varchar](100) NOT NULL,
	[GoCardless_Mode] [varchar](50) NOT NULL,
	[GoCardless_APIUrlLive] [varchar](max) NOT NULL,
	[GoCardless_TokenUrlLive] [varchar](max) NOT NULL,
	[GoCardless_ClientIDLive] [varchar](max) NOT NULL,
	[GoCardless_ClientSecretLive] [varchar](max) NOT NULL,
	[GoCardless_TokenLive] [varchar](max) NOT NULL,
	[GoCardless_APIUrlSandbox] [varchar](max) NOT NULL,
	[GoCardless_TokenUrlSandbox] [varchar](max) NOT NULL,
	[GoCardless_ClientIDSandbox] [varchar](max) NOT NULL,
	[GoCardless_ClientSecretSandbox] [varchar](max) NOT NULL,
	[GoCardless_TokenSandbox] [varchar](max) NOT NULL,
	[RegFeeWithBook] [decimal](8, 2) NOT NULL CONSTRAINT [DF_SystemSettings_FeeWithBook]  DEFAULT ((0)),
	[RegFeeWithoutBook] [decimal](8, 2) NOT NULL CONSTRAINT [DF_SystemSettings_FeeWithoutBook]  DEFAULT ((0)),
	[PayPal_Mode] [varchar](50) NOT NULL,
	[PayPalServerURL_Live] [varchar](max) NOT NULL,
	[BusinessEmail_Live] [varchar](max) NOT NULL,
	[NotifyURL_IPN_Live] [varchar](max) NOT NULL,
	[ReturnURL_Live] [varchar](max) NOT NULL,
	[PayPalServerURL_SandBox] [varchar](max) NOT NULL,
	[BusinessEmail_SandBox] [varchar](max) NOT NULL,
	[NotifyURL_IPN_SandBox] [varchar](max) NOT NULL,
	[ReturnURL_SandBox] [varchar](max) NOT NULL,
 CONSTRAINT [PK_SystemSettings] PRIMARY KEY CLUSTERED 
(
	[SystemSettingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TeamImages]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TeamImages](
	[TeamImageID] [bigint] IDENTITY(1,1) NOT NULL,
	[TeamID] [bigint] NOT NULL,
	[FileName] [varchar](200) NOT NULL,
	[Display] [bit] NOT NULL CONSTRAINT [DF_TeamImages_Display]  DEFAULT ((1)),
	[DefaultImage] [bit] NOT NULL CONSTRAINT [DF_TeamImages_DefaultImage]  DEFAULT ((0)),
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_TeamImages] PRIMARY KEY CLUSTERED 
(
	[TeamImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TeamPlayers]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeamPlayers](
	[TeamPlayerID] [bigint] IDENTITY(1,1) NOT NULL,
	[TeamID] [bigint] NOT NULL,
	[PlayerID] [bigint] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_TeamPlayers] PRIMARY KEY CLUSTERED 
(
	[TeamPlayerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Teams]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Teams](
	[TeamID] [bigint] IDENTITY(1,1) NOT NULL,
	[TeamName] [varchar](50) NOT NULL,
	[CoachID] [bigint] NOT NULL,
	[Featured] [bit] NOT NULL CONSTRAINT [DF_Teams_Featured]  DEFAULT ((0)),
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Teams] PRIMARY KEY CLUSTERED 
(
	[TeamID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Titles]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Titles](
	[TitleID] [bigint] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](50) NOT NULL,
	[SortBy] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Titles] PRIMARY KEY CLUSTERED 
(
	[TitleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleID] [bigint] NOT NULL,
	[FullName] [varchar](50) NOT NULL,
	[EmailAddress] [varchar](100) NOT NULL,
	[Password] [varchar](max) NOT NULL,
	[Locked] [bit] NOT NULL CONSTRAINT [DF_Users_Locked]  DEFAULT ((0)),
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_Users_Deleted]  DEFAULT ((0)),
	[PasswordResetCode] [varchar](max) NULL,
	[ResetCodeExpiry] [datetime2](7) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Unique_CityName]    Script Date: 22/08/2016 14:25:21 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Unique_CityName] ON [dbo].[Cities]
(
	[CityName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Unique_CountryName]    Script Date: 22/08/2016 14:25:21 ******/
CREATE NONCLUSTERED INDEX [Unique_CountryName] ON [dbo].[Countries]
(
	[CountryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ELMAH_Error_App_Time_Seq]    Script Date: 22/08/2016 14:25:21 ******/
CREATE NONCLUSTERED INDEX [IX_ELMAH_Error_App_Time_Seq] ON [dbo].[ELMAH_Error]
(
	[Application] ASC,
	[TimeUtc] DESC,
	[Sequence] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Unique_LeagueName]    Script Date: 22/08/2016 14:25:21 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Unique_LeagueName] ON [dbo].[Leagues]
(
	[LeagueName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Unique_EmailAddress]    Script Date: 22/08/2016 14:25:21 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Unique_EmailAddress] ON [dbo].[Players]
(
	[EmailAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Unique_Region]    Script Date: 22/08/2016 14:25:21 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Unique_Region] ON [dbo].[Regions]
(
	[CountryID] ASC,
	[RegionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Unique_TeamPlayer]    Script Date: 22/08/2016 14:25:21 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Unique_TeamPlayer] ON [dbo].[TeamPlayers]
(
	[TeamID] ASC,
	[PlayerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [Unique_TeamName]    Script Date: 22/08/2016 14:25:21 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Unique_TeamName] ON [dbo].[Teams]
(
	[TeamName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuditLogs]  WITH CHECK ADD  CONSTRAINT [FK_AuditLogs_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[AuditLogs] CHECK CONSTRAINT [FK_AuditLogs_Users]
GO
ALTER TABLE [dbo].[Cities]  WITH CHECK ADD  CONSTRAINT [FK_Cities_Regions] FOREIGN KEY([RegionID])
REFERENCES [dbo].[Regions] ([RegionID])
GO
ALTER TABLE [dbo].[Cities] CHECK CONSTRAINT [FK_Cities_Regions]
GO
ALTER TABLE [dbo].[LeagueFixtures]  WITH CHECK ADD  CONSTRAINT [FK_LeagueFixtures_Leagues] FOREIGN KEY([LeagueID])
REFERENCES [dbo].[Leagues] ([LeagueID])
GO
ALTER TABLE [dbo].[LeagueFixtures] CHECK CONSTRAINT [FK_LeagueFixtures_Leagues]
GO
ALTER TABLE [dbo].[LeagueFixtures]  WITH CHECK ADD  CONSTRAINT [FK_LeagueFixtures_Players] FOREIGN KEY([ManOfTheMatchPlayerID])
REFERENCES [dbo].[Players] ([PlayerID])
GO
ALTER TABLE [dbo].[LeagueFixtures] CHECK CONSTRAINT [FK_LeagueFixtures_Players]
GO
ALTER TABLE [dbo].[LeagueFixtures]  WITH CHECK ADD  CONSTRAINT [FK_LeagueFixtures_Teams] FOREIGN KEY([HomeTeamID])
REFERENCES [dbo].[Teams] ([TeamID])
GO
ALTER TABLE [dbo].[LeagueFixtures] CHECK CONSTRAINT [FK_LeagueFixtures_Teams]
GO
ALTER TABLE [dbo].[LeagueFixtures]  WITH CHECK ADD  CONSTRAINT [FK_LeagueFixtures_Teams1] FOREIGN KEY([AwayTeamID])
REFERENCES [dbo].[Teams] ([TeamID])
GO
ALTER TABLE [dbo].[LeagueFixtures] CHECK CONSTRAINT [FK_LeagueFixtures_Teams1]
GO
ALTER TABLE [dbo].[Leagues]  WITH CHECK ADD  CONSTRAINT [FK_Leagues_Cities] FOREIGN KEY([CityID])
REFERENCES [dbo].[Cities] ([CityID])
GO
ALTER TABLE [dbo].[Leagues] CHECK CONSTRAINT [FK_Leagues_Cities]
GO
ALTER TABLE [dbo].[LeagueTeams]  WITH CHECK ADD  CONSTRAINT [FK_LeagueTeams_Leagues] FOREIGN KEY([LeagueID])
REFERENCES [dbo].[Leagues] ([LeagueID])
GO
ALTER TABLE [dbo].[LeagueTeams] CHECK CONSTRAINT [FK_LeagueTeams_Leagues]
GO
ALTER TABLE [dbo].[LeagueTeams]  WITH CHECK ADD  CONSTRAINT [FK_LeagueTeams_Teams] FOREIGN KEY([TeamID])
REFERENCES [dbo].[Teams] ([TeamID])
GO
ALTER TABLE [dbo].[LeagueTeams] CHECK CONSTRAINT [FK_LeagueTeams_Teams]
GO
ALTER TABLE [dbo].[NoTeamPlayers]  WITH CHECK ADD  CONSTRAINT [FK_NoTeamPlayers_Leagues] FOREIGN KEY([LeagueID])
REFERENCES [dbo].[Leagues] ([LeagueID])
GO
ALTER TABLE [dbo].[NoTeamPlayers] CHECK CONSTRAINT [FK_NoTeamPlayers_Leagues]
GO
ALTER TABLE [dbo].[NoTeamPlayers]  WITH CHECK ADD  CONSTRAINT [FK_NoTeamPlayers_Players] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([PlayerID])
GO
ALTER TABLE [dbo].[NoTeamPlayers] CHECK CONSTRAINT [FK_NoTeamPlayers_Players]
GO
ALTER TABLE [dbo].[PlayerImages]  WITH CHECK ADD  CONSTRAINT [FK_PlayerImages_Players] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([PlayerID])
GO
ALTER TABLE [dbo].[PlayerImages] CHECK CONSTRAINT [FK_PlayerImages_Players]
GO
ALTER TABLE [dbo].[PlayerPayments]  WITH CHECK ADD  CONSTRAINT [FK_PlayerPayments_PaymentStatuses] FOREIGN KEY([PaymentStatusID])
REFERENCES [dbo].[PaymentStatuses] ([PaymentStatusID])
GO
ALTER TABLE [dbo].[PlayerPayments] CHECK CONSTRAINT [FK_PlayerPayments_PaymentStatuses]
GO
ALTER TABLE [dbo].[PlayerPayments]  WITH CHECK ADD  CONSTRAINT [FK_PlayerPayments_Players] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([PlayerID])
GO
ALTER TABLE [dbo].[PlayerPayments] CHECK CONSTRAINT [FK_PlayerPayments_Players]
GO
ALTER TABLE [dbo].[Players]  WITH CHECK ADD  CONSTRAINT [FK_Players_Advertisements] FOREIGN KEY([AdvertisementID])
REFERENCES [dbo].[Advertisements] ([AdvertisementID])
GO
ALTER TABLE [dbo].[Players] CHECK CONSTRAINT [FK_Players_Advertisements]
GO
ALTER TABLE [dbo].[Players]  WITH CHECK ADD  CONSTRAINT [FK_Players_Heights] FOREIGN KEY([HeightID])
REFERENCES [dbo].[Heights] ([HeightID])
GO
ALTER TABLE [dbo].[Players] CHECK CONSTRAINT [FK_Players_Heights]
GO
ALTER TABLE [dbo].[Players]  WITH CHECK ADD  CONSTRAINT [FK_Players_HowActives] FOREIGN KEY([HowActiveID])
REFERENCES [dbo].[HowActives] ([HowActiveID])
GO
ALTER TABLE [dbo].[Players] CHECK CONSTRAINT [FK_Players_HowActives]
GO
ALTER TABLE [dbo].[Players]  WITH CHECK ADD  CONSTRAINT [FK_Players_Positions] FOREIGN KEY([PositionID])
REFERENCES [dbo].[Positions] ([PositionID])
GO
ALTER TABLE [dbo].[Players] CHECK CONSTRAINT [FK_Players_Positions]
GO
ALTER TABLE [dbo].[Players]  WITH CHECK ADD  CONSTRAINT [FK_Players_Titles] FOREIGN KEY([TitleID])
REFERENCES [dbo].[Titles] ([TitleID])
GO
ALTER TABLE [dbo].[Players] CHECK CONSTRAINT [FK_Players_Titles]
GO
ALTER TABLE [dbo].[PlayerWeights]  WITH CHECK ADD  CONSTRAINT [FK_PlayerWeights_Players] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([PlayerID])
GO
ALTER TABLE [dbo].[PlayerWeights] CHECK CONSTRAINT [FK_PlayerWeights_Players]
GO
ALTER TABLE [dbo].[PlayerWeightWeeks]  WITH CHECK ADD  CONSTRAINT [FK_PlayerWeightWeeks_Players] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([PlayerID])
GO
ALTER TABLE [dbo].[PlayerWeightWeeks] CHECK CONSTRAINT [FK_PlayerWeightWeeks_Players]
GO
ALTER TABLE [dbo].[Regions]  WITH CHECK ADD  CONSTRAINT [FK_Regions_Countries] FOREIGN KEY([CountryID])
REFERENCES [dbo].[Countries] ([CountryID])
GO
ALTER TABLE [dbo].[Regions] CHECK CONSTRAINT [FK_Regions_Countries]
GO
ALTER TABLE [dbo].[TeamImages]  WITH CHECK ADD  CONSTRAINT [FK_TeamImages_Teams] FOREIGN KEY([TeamID])
REFERENCES [dbo].[Teams] ([TeamID])
GO
ALTER TABLE [dbo].[TeamImages] CHECK CONSTRAINT [FK_TeamImages_Teams]
GO
ALTER TABLE [dbo].[TeamPlayers]  WITH CHECK ADD  CONSTRAINT [FK_TeamPlayers_Players] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([PlayerID])
GO
ALTER TABLE [dbo].[TeamPlayers] CHECK CONSTRAINT [FK_TeamPlayers_Players]
GO
ALTER TABLE [dbo].[TeamPlayers]  WITH CHECK ADD  CONSTRAINT [FK_TeamPlayers_Teams] FOREIGN KEY([TeamID])
REFERENCES [dbo].[Teams] ([TeamID])
GO
ALTER TABLE [dbo].[TeamPlayers] CHECK CONSTRAINT [FK_TeamPlayers_Teams]
GO
ALTER TABLE [dbo].[Teams]  WITH CHECK ADD  CONSTRAINT [FK_Teams_Users] FOREIGN KEY([CoachID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Teams] CHECK CONSTRAINT [FK_Teams_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles]
GO
/****** Object:  StoredProcedure [dbo].[DeleteLeague]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <03-06-2016
-- Description:	<Delete a League and foreign table entries>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteLeague]
	-- Add the parameters for the stored procedure here
	@LeagueID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE from LeagueFixtures where LeagueID = @LeagueID
	DELETE from LeagueTeams where LeagueID = @LeagueID
	DELETE from NoTeamPlayers where LeagueID = @LeagueID
	DELETE from Leagues where LeagueID = @LeagueID

END


GO
/****** Object:  StoredProcedure [dbo].[DeletePlayer]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <01-06-2016>
-- Description:	<Delete Complete Record from System for Player>
-- =============================================
CREATE PROCEDURE [dbo].[DeletePlayer]
	-- Add the parameters for the stored procedure here
	@PlayerID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Delete from NoTeamPlayers where PlayerID = @PlayerID

	Delete from PlayerImages where PlayerID = @PlayerID

	Delete from PlayerPayments where PlayerID = @PlayerID

	Delete from PlayerWeights  where PlayerID = @PlayerID

	Delete from PlayerWeightWeeks  where PlayerID = @PlayerID

	Delete from TeamPlayers where PlayerID = @PlayerID

	delete from Players  where PlayerID = @PlayerID

END


GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorsXml]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_GetErrorsXml]
(
    @Application NVARCHAR(60),
    @PageIndex INT = 0,
    @PageSize INT = 15,
    @TotalCount INT OUTPUT
)
AS 

    SET NOCOUNT ON

    DECLARE @FirstTimeUTC DATETIME
    DECLARE @FirstSequence INT
    DECLARE @StartRow INT
    DECLARE @StartRowIndex INT

    SELECT 
        @TotalCount = COUNT(1) 
    FROM 
        [ELMAH_Error]
    WHERE 
        [Application] = @Application

    -- Get the ID of the first error for the requested page

    SET @StartRowIndex = @PageIndex * @PageSize + 1

    IF @StartRowIndex <= @TotalCount
    BEGIN

        SET ROWCOUNT @StartRowIndex

        SELECT  
            @FirstTimeUTC = [TimeUtc],
            @FirstSequence = [Sequence]
        FROM 
            [ELMAH_Error]
        WHERE   
            [Application] = @Application
        ORDER BY 
            [TimeUtc] DESC, 
            [Sequence] DESC

    END
    ELSE
    BEGIN

        SET @PageSize = 0

    END

    -- Now set the row count to the requested page size and get
    -- all records below it for the pertaining application.

    SET ROWCOUNT @PageSize

    SELECT 
        errorId     = [ErrorId], 
        application = [Application],
        host        = [Host], 
        type        = [Type],
        source      = [Source],
        message     = [Message],
        [user]      = [User],
        statusCode  = [StatusCode], 
        time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
    FROM 
        [ELMAH_Error] error
    WHERE
        [Application] = @Application
    AND
        [TimeUtc] <= @FirstTimeUTC
    AND 
        [Sequence] <= @FirstSequence
    ORDER BY
        [TimeUtc] DESC, 
        [Sequence] DESC
    FOR
        XML AUTO



GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorXml]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_GetErrorXml]
(
    @Application NVARCHAR(60),
    @ErrorId UNIQUEIDENTIFIER
)
AS

    SET NOCOUNT ON

    SELECT 
        [AllXml]
    FROM 
        [ELMAH_Error]
    WHERE
        [ErrorId] = @ErrorId
    AND
        [Application] = @Application



GO
/****** Object:  StoredProcedure [dbo].[ELMAH_LogError]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_LogError]
(
    @ErrorId UNIQUEIDENTIFIER,
    @Application NVARCHAR(60),
    @Host NVARCHAR(30),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @AllXml NTEXT,
    @StatusCode INT,
    @TimeUtc DATETIME
)
AS

    SET NOCOUNT ON

    INSERT
    INTO
        [ELMAH_Error]
        (
            [ErrorId],
            [Application],
            [Host],
            [Type],
            [Source],
            [Message],
            [User],
            [AllXml],
            [StatusCode],
            [TimeUtc]
        )
    VALUES
        (
            @ErrorId,
            @Application,
            @Host,
            @Type,
            @Source,
            @Message,
            @User,
            @AllXml,
            @StatusCode,
            DateAdd(Hour,1, GETUTCDATE())
        )

GO
/****** Object:  StoredProcedure [dbo].[PlayersPaidForBook_ALL]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-07-2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[PlayersPaidForBook_ALL]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select Distinct p.PlayerID, p.FullName, p.Mobile, p.EmailAddress, 
CASE WHEN p.AddressLine1 IS NULL THEN 
SUBSTRING(p.Notes,CHARINDEX('PayPal Address', p.Notes)+16 ,LEN(p.Notes))
ELSE
(ISNULL(p.AddressLine1,'')+' '+ ISNULL(p.AddressLine2,'')+' '+ISNULL(p.PostCode,'')+' '+ISNULL(p.City,'')+' '+ISNULL(p.Country,''))
END
 as [Address],
pp.Amount 
from players as p
INNER Join PlayerPayments as pp on p.PlayerID = pp.PlayerID
where pp.Amount = 15.49 and pp.PaidWithPayPal = 1
and PaymentStatusID <>  6
--and CONVERT(date,pp.PaymentDateTime) = Convert(date,GetDAte())


UNION

select p.PlayerID, p.FullName, p.Mobile, p.EmailAddress, 
(ISNULL(p.AddressLine1,'')+' '+ ISNULL(p.AddressLine2,'')+' '+ISNULL(p.PostCode,'')+' '+ISNULL(p.City,'')+' '+ISNULL(p.Country,'')) as [Address],
pp.Amount 
from players as p
INNER Join PlayerPayments as pp on p.PlayerID = pp.PlayerID
where pp.Amount = 17.99 and pp.GoCardless_PaymentID IS NOT NULL
and PaymentStatusID <>  6
--and CONVERT(date,pp.PaymentDateTime) = Convert(date,GetDAte())
order by p.FullName
END

GO
/****** Object:  StoredProcedure [dbo].[PlayersPaidForBook_TodayOnly]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-07-2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[PlayersPaidForBook_TodayOnly]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		select Distinct p.PlayerID, p.FullName, p.Mobile, p.EmailAddress, 
CASE WHEN p.AddressLine1 IS NULL THEN 
SUBSTRING(p.Notes,CHARINDEX('PayPal Address', p.Notes)+16 ,LEN(p.Notes))
ELSE
(ISNULL(p.AddressLine1,'')+' '+ ISNULL(p.AddressLine2,'')+' '+ISNULL(p.PostCode,'')+' '+ISNULL(p.City,'')+' '+ISNULL(p.Country,''))
END
 as [Address],
pp.Amount 
from players as p
INNER Join PlayerPayments as pp on p.PlayerID = pp.PlayerID
where pp.Amount = 15.49 and pp.PaidWithPayPal = 1
and PaymentStatusID <>  6
and CONVERT(date,pp.PaymentDateTime) = Convert(date,GetDAte())


UNION

select p.PlayerID, p.FullName, p.Mobile, p.EmailAddress, 
(ISNULL(p.AddressLine1,'')+' '+ ISNULL(p.AddressLine2,'')+' '+ISNULL(p.PostCode,'')+' '+ISNULL(p.City,'')+' '+ISNULL(p.Country,'')) as [Address],
pp.Amount 
from players as p
INNER Join PlayerPayments as pp on p.PlayerID = pp.PlayerID
where pp.Amount = 17.99 and pp.GoCardless_PaymentID IS NOT NULL
and PaymentStatusID <>  6
and CONVERT(date,pp.PaymentDateTime) = Convert(date,GetDAte())
order by p.FullName
END

GO
/****** Object:  StoredProcedure [dbo].[SelectAdvertisementPercentAge]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <02/08/2016>
-- Description:	<Dashboard Advertisement with Percentage of usage>
-- =============================================
CREATE PROCEDURE [dbo].[SelectAdvertisementPercentAge]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		DECLARE @TotalPlayers bigint
DECLARE @TotalADV_Selection bigint

Declare @tblPlayers as table(
PlayerID bigint,
AdvertisementID bigint
)
INSERT INTO @tblPlayers
select PlayerID,AdvertisementID from Players

Declare @tblAdvertisements as table(
AdvertisementID bigint,
Advertisement varchar(50)
)
INSERT INTO @tblAdvertisements
select AdvertisementID,Advertisement from Advertisements

select @TotalPlayers = count(*)  from @tblPlayers

--select @TotalPlayers as TotalPlayers
Declare @tblAdvertisement_PercentAge as table(
AdvertisementID bigint,
Advertisement varchar(50),
TotalSelections int,
PercentAge decimal(8,2)
)

DECLARE @MyCursor CURSOR;
DECLARE @MyField bigint;
BEGIN
    SET @MyCursor = CURSOR FOR
   select ADV.AdvertisementID from @tblAdvertisements as ADV  

    OPEN @MyCursor 
    FETCH NEXT FROM @MyCursor 
    INTO @MyField

    WHILE @@FETCH_STATUS = 0
    BEGIN


DECLARE @PercentAge_ADV decimal(8,2)
DECLARE @Advertisement varchar(50)
 select @Advertisement = ADV.Advertisement from @tblAdvertisements as ADV  where ADV.AdvertisementID = @MyField


select @TotalADV_Selection = count(*)  from @tblPlayers as p
where p.AdvertisementID = @MyField

Declare @Step1 decimal(8,2);


if @TotalADV_Selection = 0
	Begin
		set @PercentAge_ADV = 0.00
	End
Else
	Begin
	set @Step1 = Convert(decimal(8,2), @TotalPlayers)/Convert(decimal(8,2), @TotalADV_Selection)
		set @PercentAge_ADV = 100.00/@Step1--(Convert(decimal(8,2), @TotalADV_Selection)/100.00)*Convert(decimal(8,2), @TotalPlayers)
	End
--select @PercentAge_ADV as PercentAge_ADV

Insert Into @tblAdvertisement_PercentAge([AdvertisementID],[Advertisement],[TotalSelections],[PercentAge])
Values(@MyField,@Advertisement,@TotalADV_Selection,@PercentAge_ADV)

    --YOUR ALGORITHM GOES HERE 
      FETCH NEXT FROM @MyCursor 
      INTO @MyField 
    END; 

    CLOSE @MyCursor ;
    DEALLOCATE @MyCursor;
END;


select * from @tblAdvertisement_PercentAge

END
GO
/****** Object:  StoredProcedure [dbo].[SelectGoCardlessExport]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman AKram>
-- Create date: <14-07-2016>
-- Description:	<Export Customer Details From GoCArdless and sync with MVFF DB>
-- =============================================
CREATE PROCEDURE [dbo].[SelectGoCardlessExport]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
select 
pl.PlayerID,pl.EmailAddress,p.amount,p.id as GoCardless_PaymentID, p.links#customer as GoCardless_CustomerID,p.created_at,
p.customer#address_line1 as AddressLine1, p.customer#address_line2 as AddressLine2,  p.customer#city as City, 
p.customer#postal_code as PostCode, p.customer#country_code as CoutnryCode,p.status,p.description as GoCardless_Description
from payments as p
INNER JOIN Players as pl on p.customer#email = pl.EmailAddress
END

GO
/****** Object:  StoredProcedure [dbo].[SelectLeagueFixtureStats]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-05-2016>
-- Description:	<Select League Fixture Stats 1 = Weight & Goals , 2 = Pitch and 3 = Scales>
-- =============================================
CREATE PROCEDURE [dbo].[SelectLeagueFixtureStats] 
	-- Add the parameters for the stored procedure here
	@LeagueID bigint,
	@TypeID int --1 = Weight & Goals , 2 = Pitch and 3 = Scales
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select 
			LT.TeamID, T.TeamName, dbo.TeamTotalNumPlayed(LT.TeamID,@TypeID) as TeamTotalPlayed_P,
			dbo.TeamTotalWon(LT.TeamID,@TypeID) as TeamTotalWon_W,
			dbo.TeamTotalDraw(LT.TeamID,@TypeID) as TeamTotalDraw_D,
			dbo.TeamTotalLoss(LT.TeamID,@TypeID) as TeamTotalLoss_L,
			dbo.TeamTotalGoalsFor(LT.TeamID,@TypeID) as TeamTotalGoalsFor_GF,
			dbo.TeamTotalGoalsAgainst(LT.TeamID,@TypeID) as TeamTotalGoalsAgainst_GA,
			dbo.TeamTotalGoalsDifference(LT.TeamID,@TypeID) as TeamTotalGoalsDifference_GD,
			dbo.TeamTotalPts(LT.TeamID,@TypeID) as TeamTotalPts_Pts 
	from LeagueTeams as LT
	INNER JOIN Teams as T on LT.TeamID = T.TeamID
	where LeagueID = @LeagueID


END


GO
/****** Object:  StoredProcedure [dbo].[SelectLeagueStats]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <30-06-2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectLeagueStats] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select LeagueID, Leagues.LeagueName, 
	dbo.[fn_LeagueLossPercent](LeagueID) as LeagueLossPercent,
	dbo.[fn_LeagueTotalLossKG](LeagueID) as LeagueTotalLossKG
		 from Leagues
END

GO
/****** Object:  StoredProcedure [dbo].[SelectNonPaymentPlayers]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <20-06-2016>
-- Description:	<Thos Playres who didn't paid for registration>
-- =============================================
CREATE PROCEDURE [dbo].[SelectNonPaymentPlayers]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select Distinct p.PlayerID, pp.PlayerPaymentID, p.FullName,
	L.LeagueName,pp.ManualPaymentReceived,pp.PaidWithPayPal,
	 p.Mobile, p.EmailAddress,pp.PaymentStatusID ,
(ISNULL(p.AddressLine1,'')+' '+ ISNULL(p.AddressLine2,'')+' '+ISNULL(p.PostCode,'')+' '+ISNULL(p.City,'')+' '+ISNULL(p.Country,'')) as [Address]
 from Players as p
LEFT Join PlayerPayments  as pp on p.PlayerID = pp.PlayerID
Left Join NoTeamPlayers as ntp on p.PlayerID = ntp.PlayerID
LEFT JOIN Leagues as L on ntp.LeagueID = L.LeagueID
where pp.PlayerPaymentID IS NULL OR pp.GoCardless_PaymentID IS NULL
AND pp.ManualPaymentReceived = 0
AND pp.PaidWithPayPal <> 1
AND (pp.PaymentStatusID <> 1 AND  pp.PaymentStatusID <> 3 AND pp.PaymentStatusID <> 4 AND pp.PaymentStatusID <> 5)
Order by p.FullName

--select * from PaymentStatuses
END

GO
/****** Object:  StoredProcedure [dbo].[SelectNonTeamedPlayers]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <30-05-2016>
-- Description:	<Select No Team Players , those players which registered but not added to any team>
-- =============================================
CREATE PROCEDURE [dbo].[SelectNonTeamedPlayers]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select p.PlayerID, p.FullName, p.EmailAddress,tp.TeamID, p.City as PlayerCity ,
	L.LeagueName
	from Players as p
		LEFT JOIN TeamPlayers as tp on p.PlayerID = tp.PlayerID
		Left Join NoTeamPlayers as ntp on p.PlayerID = ntp.PlayerID
LEFT JOIN Leagues as L on ntp.LeagueID = L.LeagueID
		where tp.PlayerID IS NULL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectPlayerIDs_ByLeagueID]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <05-05-2016>
-- Description:	<Get Player IDs by LeagueID>
-- =============================================
CREATE PROCEDURE [dbo].[SelectPlayerIDs_ByLeagueID]
	-- Add the parameters for the stored procedure here
	@LeagueID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select p.PlayerID from Players as p
		INNER JOIN TeamPlayers as tp on p.PlayerID = tp.PlayerID
		INNER JOIN LeagueTeams as lt on tp.TeamID = lt.TeamID
		INNER JOIN Leagues as l on lt.LeagueID = l.LeagueID
		Where l.LeagueID = @LeagueID
END


GO
/****** Object:  StoredProcedure [dbo].[SelectPlayers]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <29-06-2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectPlayers]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select 
Distinct p.PlayerID, p.FullName, p.EmailAddress, p.DOB, p.Weight, p.BMI, 
pos.Position,p.Mobile,
CASE WHEN L.LeagueName IS NULL THEN LNTP.LeagueName ELSE L.LeagueName END as LeagueName,
 p.City, p.RegistrationDate, 
p.Active,p.Featured,
ADVR.Advertisement, p.AdvertisementOther, p.AdvertisementOtherDetails
 from Players as p
INNER JOIN Positions as pos on p.PositionID = pos.PositionID
LEFT JOIN TeamPlayers as tp on p.PlayerID = tp.PlayerID
LEFT JOIN LeagueTeams as lt on tp.TeamID = lt.TeamID
LEFT JOIN Leagues as L on lt.LeagueID = L.LeagueID
LEFT JOIN NoTeamPlayers as NTP on p.PlayerID = NTP.PlayerID
LEFT JOIN Leagues as LNTP on NTP.LeagueID = LNTP.LeagueID
LEFT JOIN Advertisements as ADVR on p.AdvertisementID = ADVR.AdvertisementID

END

GO
/****** Object:  StoredProcedure [dbo].[SelectPlayersBMIPositions]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <04-08-2016>
-- Description:	<This Stored Procedure will be used to Get Players BMI Positions of those Player who have paid for the League and we can use these players to assign them to Teams for that League>
-- =============================================
CREATE PROCEDURE [dbo].[SelectPlayersBMIPositions]
	-- Add the parameters for the stored procedure here
	@LeagueID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
--Players from Non Teamed Players
select 
p.PlayerID, p.FullName, p.BMI, p.PositionID , pos.Position,
CASE WHEN p.PositionID = 1 THEN 
	CASE WHEN p.BMI <= 35 THEN 'A1'  ELSE 'A2' END
 WHEN p.PositionID = 2 THEN 
	CASE WHEN p.BMI <= 35 THEN 'B1'  ELSE 'B2' END
 WHEN p.PositionID = 3 THEN 
	CASE WHEN p.BMI <= 35 THEN 'C1'  ELSE 'C2' END
 WHEN p.PositionID = 5 THEN  
	CASE WHEN p.BMI <= 35 THEN 'E1'  ELSE 'E2' END
 ELSE
	CASE WHEN p.BMI <= 35 THEN 'D1'  ELSE 'D2' END 
 END as BMIPosition

 from Players as p
		INNER Join NoTeamPlayers as ntp on p.PlayerID = ntp.PlayerID
		INNER JOIN Leagues as L on ntp.LeagueID = L.LeagueID
		INNER JOIN Positions as pos on p.PositionID = pos.PositionID
		where L.LeagueID = @LeagueID
		AND p.PlayerID NOT IN (
				select Distinct p.PlayerID
				 from Players as p
				LEFT Join PlayerPayments  as pp on p.PlayerID = pp.PlayerID
				Left Join NoTeamPlayers as ntp on p.PlayerID = ntp.PlayerID
				LEFT JOIN Leagues as L on ntp.LeagueID = L.LeagueID
				where pp.PlayerPaymentID IS NULL OR pp.GoCardless_PaymentID IS NULL
				AND pp.ManualPaymentReceived = 0
				AND pp.PaidWithPayPal <> 1
				AND (pp.PaymentStatusID <> 1 AND  pp.PaymentStatusID <> 3 AND pp.PaymentStatusID <> 4 AND pp.PaymentStatusID <> 5)
				)

UNION

--Players from League TeamPlayers who are already added to League Teams
--it will help to add new Non-Teamed players according to BMI Position
 select 
p.PlayerID, p.FullName, p.BMI, p.PositionID , pos.Position
,
CASE WHEN p.PositionID = 1 THEN 
	CASE WHEN p.BMI <= 35 THEN 'A1'  ELSE 'A2' END
 WHEN p.PositionID = 2 THEN 
	CASE WHEN p.BMI <= 35 THEN 'B1'  ELSE 'B2' END
 WHEN p.PositionID = 3 THEN 
	CASE WHEN p.BMI <= 35 THEN 'C1'  ELSE 'C2' END
 WHEN p.PositionID = 5 THEN  
	CASE WHEN p.BMI <= 35 THEN 'E1'  ELSE 'E2' END
 ELSE
	CASE WHEN p.BMI <= 35 THEN 'D1'  ELSE 'D2' END 
 END as BMIPosition

--p.FullName as PlayerFullname, t.TeamName, h.Height_Value, p.Weight, pos.Position, p.BMI
--tp.TeamID, t.TeamName
from Players as p
INNER JOIN Heights as h on p.HeightID = h.HeightID
INNER JOIN Positions as pos on p.PositionID = pos.PositionID
INNER JOIN TeamPlayers as tp on p.PlayerID  = tp.PlayerID
INNER JOIN Teams as t on tp.TeamID = t.TeamID
INNER JOIN LeagueTeams as lt on tp.TeamID = lt.TeamID
where lt.LeagueID = @LeagueID

Order by BMIPosition

END

GO
/****** Object:  StoredProcedure [dbo].[SelectPlayersPaid1499]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <16-06-2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectPlayersPaid1499]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
select p.PlayerID, p.FullName, p.Mobile, p.EmailAddress, 
(ISNULL(p.AddressLine1,'')+' '+ ISNULL(p.AddressLine2,'')+' '+ISNULL(p.PostCode,'')+' '+ISNULL(p.City,'')+' '+ISNULL(p.Country,'')) as [Address],
pp.Amount 
from players as p
INNER Join PlayerPayments as pp on p.PlayerID = pp.PlayerID
where pp.Amount >= 17.99 and (pp.GoCardless_PaymentID IS NOT NULL  OR pp.PaidWithPayPal = 1)
--and PaymentStatusID =  4
and CONVERT(date,pp.PaymentDateTime) = Convert(date,GetDAte())
order by p.FullName

END


GO
/****** Object:  StoredProcedure [dbo].[SelectPlayersPaid1499Confirmed]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <16-06-2016>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[SelectPlayersPaid1499Confirmed]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
select p.PlayerID, p.FullName, p.Mobile, p.EmailAddress, 
(ISNULL(p.AddressLine1,'')+' '+ ISNULL(p.AddressLine2,'')+' '+ISNULL(p.PostCode,'')+' '+ISNULL(p.City,'')+' '+ISNULL(p.Country,'')) as [Address],
pp.Amount 
from players as p
INNER Join PlayerPayments as pp on p.PlayerID = pp.PlayerID
where pp.Amount = 17.99 and pp.GoCardless_PaymentID IS NOT NULL
and PaymentStatusID =  4
and CONVERT(date,pp.PaymentDateTime) = Convert(date,GetDAte())
order by p.FullName

END


GO
/****** Object:  StoredProcedure [dbo].[SelectPlayersPerLeague]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <17-06-2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectPlayersPerLeague]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	select L.LeagueID, L.LeagueName, Count(tp.PlayerID) as TotalPlayers, dbo.[fn_CalculateNonTeamedPlayersPerLeague](L.LeagueID) as NonTeamedPlayers 
	From Leagues as L
left JOIN LeagueTeams as lt on L.LeagueID = lt.LeagueID
Left JOIN TeamPlayers as tp on lt.TeamID = tp.TeamID
Group by L.LeagueID, L.LeagueName

order by L.LeagueName



--select 
--		L.LeagueID, L.LeagueName, T.TeamID, T.TeamName, P.PlayerID, P.FullName as PlayerFullName 
--from 
--		Leagues as L
--		INNER JOIN LeagueTeams as lt on L.LeagueID = lt.LeagueID
--		INNER JOIN TeamPlayers as tp on lt.TeamID = tp.TeamID
--		INNER JOIN Teams as T on tp.TeamID = T.TeamID
--		INNER JOIN Players as P on tp.PlayerID = P.PlayerID
END

GO
/****** Object:  StoredProcedure [dbo].[SelectPlayersTotalNumWeeksLoss_AwayTeam]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <20/07/2016>
-- Description:	<This Stored Procedure will be used to Return total number of weeks Player has loss weight and we'll use this to calculate his Weigth goal for his Team>
-- =============================================
CREATE PROCEDURE [dbo].[SelectPlayersTotalNumWeeksLoss_AwayTeam]
	-- Add the parameters for the stored procedure here
	@LeagueFixtureID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		select Distinct pww.PlayerID, dbo.[fn_PlayerTotalLossWeeks](pww.PlayerID) as TotalLossWeeks from PlayerWeightWeeks as pww
INNER JOIN TeamPlayers as tp on pww.PlayerID = tp.PlayerID
INNER JOIN LeagueFixtures as lf on tp.TeamID = lf.AwayTeamID
where lf.LeagueFixtureID = @LeagueFixtureID
END

GO
/****** Object:  StoredProcedure [dbo].[SelectPlayersTotalNumWeeksLoss_HomeTeam]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <20/07/2016>
-- Description:	<This Stored Procedure will be used to Return total number of weeks Player has loss weight and we'll use this to calculate his Weigth goal for his Team>
-- =============================================
CREATE PROCEDURE [dbo].[SelectPlayersTotalNumWeeksLoss_HomeTeam]
	-- Add the parameters for the stored procedure here
	@LeagueFixtureID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		select Distinct pww.PlayerID, dbo.[fn_PlayerTotalLossWeeks](pww.PlayerID) as TotalLossWeeks from PlayerWeightWeeks as pww
INNER JOIN TeamPlayers as tp on pww.PlayerID = tp.PlayerID
INNER JOIN LeagueFixtures as lf on tp.TeamID = lf.HomeTeamID
where lf.LeagueFixtureID = @LeagueFixtureID
END

GO
/****** Object:  StoredProcedure [dbo].[SelectPlayerWeightWeeks_AwayTeam]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <02/07/2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectPlayerWeightWeeks_AwayTeam]
	-- Add the parameters for the stored procedure here
	@LeagueFixtureID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select pww.* from PlayerWeightWeeks as pww
INNER JOIN TeamPlayers as tp on pww.PlayerID = tp.PlayerID
INNER JOIN LeagueFixtures as lf on tp.TeamID = lf.AwayTeamID
where lf.LeagueFixtureID = @LeagueFixtureID

END

GO
/****** Object:  StoredProcedure [dbo].[SelectPlayerWeightWeeks_HomeTeam]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <02/07/2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectPlayerWeightWeeks_HomeTeam]
	-- Add the parameters for the stored procedure here
	@LeagueFixtureID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select pww.* from PlayerWeightWeeks as pww
INNER JOIN TeamPlayers as tp on pww.PlayerID = tp.PlayerID
INNER JOIN LeagueFixtures as lf on tp.TeamID = lf.HomeTeamID
where lf.LeagueFixtureID = @LeagueFixtureID

END

GO
/****** Object:  StoredProcedure [dbo].[SelectPlayerWeightWeeks_PositionByAwayTeam]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <02/07/2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectPlayerWeightWeeks_PositionByAwayTeam]
	-- Add the parameters for the stored procedure here
	@LeagueID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
declare @tblPlayerPositions as Table
(
PlayerID bigint,
LossPercent decimal(8,2)
)

Insert into @tblPlayerPositions
select Distinct pww.PlayerID, pww.LossPercent from PlayerWeightWeeks as pww
INNER JOIN TeamPlayers as tp on pww.PlayerID = tp.PlayerID
INNER JOIN LeagueFixtures as lf on tp.TeamID = lf.AwayTeamID
where lf.LeagueID = @LeagueID 
order by pww.LossPercent Desc


select ROW_NUMBER() 
        OVER (ORDER BY LossPercent Desc) AS PositionNum,* from @tblPlayerPositions

END

GO
/****** Object:  StoredProcedure [dbo].[SelectPlayerWeightWeeks_PositionByHomeTeam]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <02/07/2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectPlayerWeightWeeks_PositionByHomeTeam]
	-- Add the parameters for the stored procedure here
	@LeagueID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
declare @tblPlayerPositions as Table
(
PlayerID bigint,
LossPercent decimal(8,2)
)

Insert into @tblPlayerPositions
select Distinct pww.PlayerID, pww.LossPercent from PlayerWeightWeeks as pww
INNER JOIN TeamPlayers as tp on pww.PlayerID = tp.PlayerID
INNER JOIN LeagueFixtures as lf on tp.TeamID = lf.HomeTeamID
where lf.LeagueID = @LeagueID 
order by pww.LossPercent Desc


select ROW_NUMBER() 
        OVER (ORDER BY LossPercent Desc) AS PositionNum,* from @tblPlayerPositions

END

GO
/****** Object:  StoredProcedure [dbo].[SelectTeamWeightByLeague]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <12-05-2016>
-- Description:	<Get Player weight of Teams assigned to League>
-- =============================================
CREATE PROCEDURE [dbo].[SelectTeamWeightByLeague]
	-- Add the parameters for the stored procedure here
	@LeagueID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select L.LeagueName, t.TeamName, SUM(pww.TotalLost) as TeamTotalWeightLost from Leagues as L
Inner join LeagueTeams as LT on L.LeagueID = LT.LeagueID
INNER JOIN Teams as T on lt.TeamID = t.TeamID
INNER JOIN TeamPlayers as TP on t.TeamID = tp.TeamID
INNER JOIN PlayerWeightWeeks as pww on tp.PlayerID = pww.PlayerID
INNER JOIN Players as P on TP.PlayerID = P.PlayerID
Where L.LeagueID = @LeagueID
group by L.LeagueName, t.TeamName






END


GO
/****** Object:  StoredProcedure [dbo].[test]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[test]
	-- Add the parameters for the stored procedure here
	as
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
select L.LeagueID, L.LeagueName, Count(tp.PlayerID) as TotalPlayers from Leagues as L
left JOIN LeagueTeams as lt on L.LeagueID = lt.LeagueID
Left JOIN TeamPlayers as tp on lt.TeamID = tp.TeamID
Group by L.LeagueID, L.LeagueName

END

GO
/****** Object:  StoredProcedure [dbo].[tmpSendEmail]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[tmpSendEmail] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select PlayerID from players

where
 Convert(date, RegistrationDate ,102) >= Convert(date, '2016-07-26' ,102)
 AND
  Convert(date, RegistrationDate ,102) <= Convert(date, '2016-08-01' ,102)
END

GO
/****** Object:  StoredProcedure [dbo].[tmpSynchPaypal]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[tmpSynchPaypal]
	-- Add the partmpameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
select pt.TransactionID, pt.TransactionDateTime, pt.PayLinkID,pt.PlayerID as PayPAl_PlayerID, pt.PayPalAmount,
p.PlayerID, p.Fullname as PlayerFullname, p.EmailAddress, pp.PaymentDateTime, pp.Amount, pp.BookRequested, pp.PAidWithPAyPal,
pp.Paymentstatusid, pp.PlayerPaymentID
 from [dbo].[PayPalTransactions] as pt
INNER JOIN Players as p on pt.PlayerID = p.PlayerID
LEFT JOIN PlayerPayments as pp on p.PlayerID = pp.PlayerID
END

GO
/****** Object:  StoredProcedure [dbo].[UpdatePlayerImageAsDefault]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <05-04-2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdatePlayerImageAsDefault]
	-- Add the parameters for the stored procedure here
	@PlayerImageID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @PlayerID bigint

	select @PlayerID = PlayerID from PlayerImages where PlayerImageID  = @PlayerImageID

	--Unseelct All the Player Default Images
	update PlayerImages
	set DefaultImage = 0
	where PlayerID = @PlayerID


	--Set the Requested Image as default
	Update PlayerImages
	set DefaultImage = 1
	where PlayerImageID  = @PlayerImageID
	
END


GO
/****** Object:  StoredProcedure [dbo].[UpdateTeamImageAsDefault]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <14-04-2016>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateTeamImageAsDefault]
	-- Add the parameters for the stored procedure here
	@TeamImageID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @TeamID bigint

	select @TeamID = TeamID from TeamImages where TeamImageID  = @TeamImageID

	--Unseelct All the Team Default Images
	update TeamImages
	set DefaultImage = 0
	where TeamID = @TeamID


	--Set the Requested Image as default
	Update TeamImages
	set DefaultImage = 1
	where TeamImageID  = @TeamImageID
	
END


GO
/****** Object:  Trigger [dbo].[Insert_LeagueFixture]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[Insert_LeagueFixture]
   ON  [dbo].[LeagueFixtures]
   AFTER Insert
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Declare @MaxMatchNum		int,
			@CurrentMatchNum	int,
			@LeagueFixtureID bigint,
			@LeagueID bigint

	select  @LeagueFixtureID = LeagueFixtureID, @LeagueID = LeagueID from inserted
	
		select @CurrentMatchNum = MAX(MatchNum) from LeagueFixtures where LeagueID = @LeagueID

	if @CurrentMatchNum IS NOT NULL 
	Begin

	

		set @MaxMatchNum = @CurrentMatchNum + 1

		Update LeagueFixtures
		Set MatchNum = @MaxMatchNum
		Where LeagueFixtureID = @LeagueFixtureID

		
	End
	Else
		Begin
		Update LeagueFixtures
		Set MatchNum = 1
		Where LeagueFixtureID = @LeagueFixtureID

		End




END


GO
/****** Object:  Trigger [dbo].[Delete_Player]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <05-05-2016>
-- Description:	<Delete Player Record in PlayerWeightWeeks when Any Player Deleted>
-- =============================================
CREATE TRIGGER [dbo].[Delete_Player]
   ON  [dbo].[Players]
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	Declare @PlayerID bigint

	select @PlayerID = PlayerID from deleted

	Delete from [dbo].[PlayerWeightWeeks]
	Where PlayerID = @PlayerID


END


GO
/****** Object:  Trigger [dbo].[Insert_Player]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <05-05-2016>
-- Description:	<Add Player Record in PlayerWeightWeeks when Any Player Inserted here>
-- =============================================
CREATE TRIGGER [dbo].[Insert_Player]
   ON  [dbo].[Players]
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	Declare @PlayerID bigint

	select @PlayerID = PlayerID from inserted

	INSERT INTO [dbo].[PlayerWeightWeeks]
           ([PlayerID])
     VALUES
           (@PlayerID)


END


GO
/****** Object:  Trigger [dbo].[Insert_PlayerWeightWeeks]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <05-05-2016>
-- Description:	<Trigger after INSERT of PlayerWeightWeeks record>
-- =============================================
CREATE TRIGGER [dbo].[Insert_PlayerWeightWeeks]
   ON  [dbo].[PlayerWeightWeeks]
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here

Declare @PlayerWeightWeekID bigint,
	@PlayerID bigint,
	@LossWk1 decimal(8,2),
	@LossWk2 decimal(8,2),
	@LossWk3 decimal(8,2),
	@LossWk4 decimal(8,2),
	@LossWk5 decimal(8,2),
	@LossWk6 decimal(8,2),
	@LossWk7 decimal(8,2),
	@LossWk8 decimal(8,2),
	@LossWk9 decimal(8,2),
	@LossWk10 decimal(8,2),
	@LossWk11 decimal(8,2),
	@LossWk12 decimal(8,2),
	@LossWk13 decimal(8,2),
	@LossWk14 decimal(8,2),
	@LossPercent decimal(8,2),
	@TotalLoss decimal(8,2)

	select @LossWk1 = ISNULL(LossWk1,0.00), 
	@LossWk2 = ISNULL(LossWk2,0.00), 
	@LossWk3 = ISNULL(LossWk3,0.00), 
	@LossWk4 = ISNULL(LossWk4,0.00), 
	@LossWk5 = ISNULL(LossWk5,0.00), 
	@LossWk6 = ISNULL(LossWk6,0.00), 
	@LossWk7 = ISNULL(LossWk7,0.00), 
	@LossWk8 = ISNULL(LossWk8,0.00), 
	@LossWk9 = ISNULL(LossWk9,0.00), 
	@LossWk10 = ISNULL(LossWk10,0.00), 
	@LossWk11 = ISNULL(LossWk11,0.00), 
	@LossWk12 = ISNULL(LossWk12,0.00), 
	@LossWk13 = ISNULL(LossWk13,0.00),
	@LossWk14 = ISNULL(LossWk14,0.00),
	@PlayerWeightWeekID = PlayerWeightWeekID,
	@PlayerID = PlayerID from inserted

	DECLARE @PlayerRegWeight decimal(8,2)

	-- Add the T-SQL statements to compute the return value here
	Select @PlayerRegWeight = [Weight] from Players where PlayerID = @PlayerID

	Select @TotalLoss =(@LossWk1+@LossWk2+@LossWk3+@LossWk4+@LossWk5+@LossWk6+@LossWk7+@LossWk8+@LossWk9+@LossWk10+@LossWk11+@LossWk12+@LossWk13+@LossWk14)
	Select @LossPercent = (@TotalLoss/@PlayerRegWeight)*100.00

	Update  
			PlayerWeightWeeks
	Set 
			TotalLost = @TotalLoss,
			LossPercent = Convert(decimal(8,2), @LossPercent)
	Where 
			PlayerWeightWeekID = @PlayerWeightWeekID

END

GO
/****** Object:  Trigger [dbo].[Update_PlayerWeightWeeks]    Script Date: 22/08/2016 14:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Usman Akram>
-- Create date: <05-05-2016>
-- Description:	<Trigger after Update of PlayerWeightWeeks record>
-- =============================================
CREATE TRIGGER [dbo].[Update_PlayerWeightWeeks]
   ON  [dbo].[PlayerWeightWeeks]
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here

	Declare @PlayerWeightWeekID bigint,
	@PlayerID bigint,
	@LossWk1 decimal(8,2),
	@LossWk2 decimal(8,2),
	@LossWk3 decimal(8,2),
	@LossWk4 decimal(8,2),
	@LossWk5 decimal(8,2),
	@LossWk6 decimal(8,2),
	@LossWk7 decimal(8,2),
	@LossWk8 decimal(8,2),
	@LossWk9 decimal(8,2),
	@LossWk10 decimal(8,2),
	@LossWk11 decimal(8,2),
	@LossWk12 decimal(8,2),
	@LossWk13 decimal(8,2),
	@LossWk14 decimal(8,2),
	@LossPercent decimal(8,2),
	@TotalLoss decimal(8,2)

	select @LossWk1 = ISNULL(LossWk1,0.00), 
	@LossWk2 = ISNULL(LossWk2,0.00), 
	@LossWk3 = ISNULL(LossWk3,0.00), 
	@LossWk4 = ISNULL(LossWk4,0.00), 
	@LossWk5 = ISNULL(LossWk5,0.00), 
	@LossWk6 = ISNULL(LossWk6,0.00), 
	@LossWk7 = ISNULL(LossWk7,0.00), 
	@LossWk8 = ISNULL(LossWk8,0.00), 
	@LossWk9 = ISNULL(LossWk9,0.00), 
	@LossWk10 = ISNULL(LossWk10,0.00), 
	@LossWk11 = ISNULL(LossWk11,0.00), 
	@LossWk12 = ISNULL(LossWk12,0.00), 
	@LossWk13 = ISNULL(LossWk13,0.00),
	@LossWk14 = ISNULL(LossWk14,0.00),
	@PlayerWeightWeekID = PlayerWeightWeekID,
	@PlayerID = PlayerID from inserted

	DECLARE @PlayerRegWeight decimal(8,2),
	 @CurrentBMI decimal(8,2),
	 @CurrentWeight decimal(8,2)

	DECLARE @PlayerHeightID bigint

	-- Add the T-SQL statements to compute the return value here
	Select @PlayerRegWeight = [Weight] from Players where PlayerID = @PlayerID

	Select @TotalLoss =(@LossWk1+@LossWk2+@LossWk3+@LossWk4+@LossWk5+@LossWk6+@LossWk7+@LossWk8+@LossWk9+@LossWk10+@LossWk11+@LossWk12+@LossWk13+@LossWk14)
	Select @LossPercent = (@TotalLoss/@PlayerRegWeight)*100.00

	select @CurrentWeight = (@PlayerRegWeight - @TotalLoss)

	select @PlayerHeightID= HeightID from Players where PlayerID = @PlayerID

	select @CurrentBMI = [dbo].[fn_CalculateBMI](@CurrentWeight,@PlayerHeightID)

	Update  
			PlayerWeightWeeks
	Set 
			TotalLost = @TotalLoss,
			LossPercent = Convert(decimal(8,2), @LossPercent),
			CurrentBMI =@CurrentBMI
	Where 
			PlayerWeightWeekID = @PlayerWeightWeekID

END

GO
USE [master]
GO
ALTER DATABASE [MANvFATFootball] SET  READ_WRITE 
GO
