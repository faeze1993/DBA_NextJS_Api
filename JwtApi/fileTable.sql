SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO
CREATE OR ALTER FUNCTION dbo.GetNewPathLocator (@parent hierarchyid = null) RETURNS varchar(max) AS
BEGIN       
    DECLARE @result varchar(max), @newid uniqueidentifier  -- declare new path locator, newid placeholder       
    SELECT @newid = guid FROM dbo.vguid; -- retrieve new GUID      
    SELECT @result = ISNULL(@parent.ToString(), '/') + -- append parent if present, otherwise assume root
                     convert(varchar(20), convert(bigint, substring(convert(binary(16), @newid), 1, 6))) + '.' +
                     convert(varchar(20), convert(bigint, substring(convert(binary(16), @newid), 7, 6))) + '.' +
                     convert(varchar(20), convert(bigint, substring(convert(binary(16), @newid), 13, 4))) + '/'     
    RETURN @result -- return new path locator     
END


GO

--=======================================================================================

GO 
-- =============================================
-- Author:		<محمدرضا>
-- Create date: <13971226>
-- Description:	<ذخیره فایل>
-- =============================================
CREATE OR ALTER   PROCEDURE [dbo].[SaveFileInfo] 
			@stream_id				UNIQUEIDENTIFIER,
			@filename				NVARCHAR(255),
			@file_stream			VARBINARY(max),
			@DirPath				NVARCHAR(2000)
			
AS 
BEGIN

	DECLARE @path HIERARCHYID
	DECLARE @FolderStream_id UNIQUEIDENTIFIER	
	DECLARE @New_path_locator HIERARCHYID
	DECLARE @Dir_path_locator HIERARCHYID
	---------------------------------------------------------------
	EXEC dbo.MkdirInFileTable @DirPath =@DirPath, -- nvarchar(2000)
	    @Path_locator =@Dir_path_locator OUTPUT -- hierarchyid
	---------------------------------------------------------------
	---------------------اگر فایل از قبل وجود داشت . اطلاعات را اپدیت کن 
	if (select count(1) from dbo.Fileinfo where stream_id = @stream_id) > 0
	begin
			
		Update dbo.Fileinfo
		set file_stream	= @file_stream,
			[name] = @filename,
			path_locator=dbo.GetNewPathLocator(@Dir_path_locator)
		where stream_id = @stream_id
		
	end
	else
	begin
	--------------------------------------------------------------
		--اگر فایلی هم نام در این دایرکتوری هست خطا بده
		if(select count(1) from dbo.Fileinfo where name = @filename and parent_path_locator = @Dir_path_locator ) > 0
		begin
			--select N'نام فایل تکراری میباشد،لطفا تغییر دهید' as errorMessage
			 RAISERROR(N'نام فایل تکراری میباشد',16,1,N'خطا')
		end 
			
		INSERT INTO dbo.Fileinfo
								(stream_id	
								, file_stream	
								, name		
								,path_locator) 
							Select 
								@stream_id 
								, @file_stream  
								, @filename
								, dbo.GetNewPathLocator(@Dir_path_locator)-- cast and path_locator
	end

END
GO

--======================================================
GO
--===========================================
-- Author:		<محمدرضا>
-- Create date: <13971226>
-- Description:	ایجاد مسیر
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[MkdirInFileTable]
				@DirPath NVARCHAR(2000) ,--D1\D2\D3\D4\
				@Path_locator HIERARCHYID OUT	
AS
BEGIN
	SET NOCOUNT ON
	-------------------------------------------------------------------------متغیر
	DECLARE @cnt INT=1;
	DECLARE @parentdir table(path hierarchyid not null);
	DECLARE @subdir_locator HIERARCHYID;
	DECLARE @SubDirName NVARCHAR(255);
	DECLARE @FirstPath_locator HIERARCHYID=NULL;
	DECLARE @PreviousParent HIERARCHYID;
	-------------------------------------------------------------------------- گرفتن اولین دایرکتوری و گذاشتن بک اسلش اول هر ادرس
	SET @DirPath =CASE WHEN RIGHT(@DirPath,1)='\' THEN @DirPath ELSE @DirPath+'\' END
	SELECT @FirstPath_locator=path_locator FROM dbo.Fileinfo WHERE path_locator.GetLevel()=1 AND name=SUBSTRING(@DirPath,0, CHARINDEX('\',@DirPath))
	-------------------------------------------------------------------------- در صورت وجود دایرکتوری اول پس_لوکیتور در تمپ تیبل ذخیره میشود برای مراحل بعد
	IF @FirstPath_locator IS NULL
	BEGIN
		INSERT INTO Fileinfo (name,is_directory,is_archive) 
		OUTPUT INSERTED.path_locator into @parentdir
		SELECT Item, 1, 0
		FROM dbo.Split(@DirPath,'\') WHERE ListID=@cnt
	END
	ELSE
	BEGIN
	    INSERT @parentdir
	            ( path )
	    VALUES  ( @FirstPath_locator  -- path - hierarchyid
	              )
	END
	------------------------------------------------------------------------
	WHILE 1=1
	BEGIN
	    SET @cnt=@cnt+1
		SET @SubDirName=null
		SELECT @SubDirName=Item FROM dbo.Split(@DirPath,'\') WHERE ListID=@cnt
		IF @SubDirName is null
		BEGIN
		    SET @Path_locator=(SELECT path FROM @parentdir)
			BREAK;
		END	

		

		SELECT @subdir_locator = dbo.GetNewPathLocator(path) from @parentdir

		IF EXISTS(SELECT TOP 1 1 FROM dbo.Fileinfo JOIN @parentdir parentdir ON parent_path_locator=parentdir.path   where name = @SubDirName )--WHERE path_locator=@subdir_locator
		BEGIN
			   SET @PreviousParent =(SELECT path FROM @parentdir)
			   DELETE @parentdir
			   INSERT @parentdir
			           ( path )
			   SELECT TOP 1 path_locator FROM dbo.Fileinfo where parent_path_locator=@PreviousParent AND  name = @SubDirName

				CONTINUE;
		END


		DELETE @parentdir 

		INSERT INTO dbo.Fileinfo (name,path_locator,is_directory,is_archive)
		OUTPUT INSERTED.path_locator into @parentdir 
		VALUES (@SubDirName, @subdir_locator, 1, 0);

	END

END

GO