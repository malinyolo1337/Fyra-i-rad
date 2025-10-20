CREATE TABLE [Speldeltagare] (
    [SpelID] INT,
    [SpelarID] INT,
    [SpelarRoll] VARCHAR(20), 

    PRIMARY KEY (SpelID, SpelarID),
    FOREIGN KEY (SpelID) REFERENCES Spel(SpelID),
    FOREIGN KEY (SpelarID) REFERENCES Spelare(SpelarID)
);