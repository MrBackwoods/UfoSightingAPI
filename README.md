# UfoSightingAPI
.NET Core web API that utilizes MS SQL database and Entity Framework. 
The solution contains three projects:
- Database project
- Web API project
- Web API test project

The API currently has these endpoints. Some are public, some are private requiring an API key and some require API key that is linked to admin rights.
- GET Sighting/byYear/{year} (public, used to get sightings by year with limited details)
- GET Sighting/byId/{id} (public, used to get sighting by id with limited details)
- GET Sighting/withDetails/byYear/{year} (private, used to get sightings by year with all details)
- GET Sightingw/ithDetails/byId/{id} (private, used to a sighting by id with all details)
- POST Sighting (private, used to post a new sighting by id)
- DELETE Sighting{id} (private admin, used to delete a sighting by id)
- PUT Sighting{id} (private admin, used to update a sighting)

This project definetely is still work <strong>in progress</strong>. List of things to do include:
- Logging
- Member handling
- More test cases
- Commenting
- Refactoring
- Improved authentication
