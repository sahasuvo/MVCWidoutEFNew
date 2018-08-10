CREATE TABLE [dbo].[StudentReg] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [Name]    VARCHAR (100)  NULL,
    [City]    VARCHAR (100)  NULL,
    [Address] NVARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE PROCEDURE [dbo].[AddNewStudent]
@Name VARCHAR (100), @City VARCHAR (100), @Address NVARCHAR (500)
AS
BEGIN
    INSERT  INTO StudentReg
    VALUES (@Name, @City, @Address);
    RETURN 1;
END

GO
CREATE PROCEDURE [dbo].[DeleteStudent]
@Id INT
AS
BEGIN
    DELETE StudentReg
    WHERE  Id = @Id;
END
RETURN 1;

GO
CREATE PROCEDURE [dbo].[GetStudentDetails]
AS
SELECT *
FROM   StudentReg;

GO
CREATE PROCEDURE [dbo].[UpdateStudent]
@ID INT=0, @Name VARCHAR (100), @City VARCHAR (100), @Address NVARCHAR (500)
AS
BEGIN
    UPDATE StudentReg
    SET    Name    = @Name,
           City    = @City,
           Address = @Address
    WHERE  Id = @ID;
END
RETURN 1;

GO
