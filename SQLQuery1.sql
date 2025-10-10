CREATE TABLE [Spelare] (
    [SpelarID] INT IDENTITY (1,1),
    [Username] VARCHAR(50) NOT NULL,
    [Password] VARCHAR(50) NOT NULL,
    [Markör] VARCHAR(10), 
    [AntalVinster] INT DEFAULT 0,
    [AntalFörluster] INT DEFAULT 0

	CONSTRAINT [PK_Tbl_Spelare] PRIMARY KEY CLUSTERED ([SpelarID] ASC)
);