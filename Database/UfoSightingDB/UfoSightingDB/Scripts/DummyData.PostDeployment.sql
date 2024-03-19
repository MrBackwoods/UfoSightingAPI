-- Adding dummy data 

SET IDENTITY_INSERT Member ON;

IF NOT EXISTS (SELECT 1 FROM Member)
BEGIN
    INSERT INTO Member (MemberID, FirstName, LastName, ApiKey, IsAdmin, Email)
    VALUES (1, 'John', 'Doe', 'UDETYTuDbDAtScf8z7YVCVenHjqsHM98X8MkYVKFkvA=', 1, 'test1@ufosightings.fi'), -- dummy-key-1
           (2, 'Jane', 'Smith','MyRhy0ZUmHpWJL+Q5FYTy7SD58RVJwJV1QjWlMstfX0=', 0, 'test2@ufosightings.fi'),  --dummy-key-2
           (3, 'Alice', 'Johnson', '????', 0, 'test3@ufosightings.fi')
END;

SET IDENTITY_INSERT Member OFF;

IF NOT EXISTS (SELECT 1 FROM Sighting)
BEGIN
    INSERT INTO Sighting (ReportedBy, Occurred, Latitude, Longitude, EstimatedDurationInSeconds, Description, WitnessCount)
    VALUES (1, '2024-03-15 18:30:00', 34.052235, -118.243683, 3600, 'Bright light spotted in the sky', 2),
           (2, '2024-03-16 22:15:00', 40.712776, -74.005974, 1800, 'Strange object flying at high speed', 1),
           (3, '2024-03-17 09:00:00', 51.507351, -0.127758, 2700, 'Multiple lights moving erratically', 3);
END;