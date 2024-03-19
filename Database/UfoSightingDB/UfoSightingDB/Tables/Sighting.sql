CREATE TABLE Sighting (
    SightingID INT PRIMARY KEY IDENTITY(1,1),
    ReportedBy INT NOT NULL,
    Reported DATETIME DEFAULT GETDATE(), 
    Occurred DATETIME NOT NULL,
    Latitude DECIMAL(9,6),
    Longitude DECIMAL(9,6),
    EstimatedDurationInSeconds INT,
    Description VARCHAR(MAX) NOT NULL,
    WitnessCount INT NOT NULL,
    FOREIGN KEY (ReportedBy) REFERENCES Member(MemberID)
);