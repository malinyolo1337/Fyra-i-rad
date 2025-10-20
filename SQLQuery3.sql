﻿CREATE TABLE Spelrunda (
    SpelRundaID INT PRIMARY KEY IDENTITY(1,1),
    SpelID INT NOT NULL,
    SpelarID INT NOT NULL,
    DragSekvens INT NOT NULL,
    Kolumn INT CHECK (Kolumn BETWEEN 1 AND 7) NOT NULL,

    FOREIGN KEY (SpelID) REFERENCES Spel(SpelID),
    FOREIGN KEY (SpelarID) REFERENCES Spelare(SpelarID)
);