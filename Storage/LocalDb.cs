using Model;
using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace Storage
{
    public class LocalDb(string dbName) : IWorldStore
    {
        private readonly string _dbFullName = $"D:\\VisualStudioProjects\\IdleGame\\Storage\\Data\\{dbName}.db";
        private readonly JsonSerializerOptions jsonOption = new() { IncludeFields = true };
        public void InitStore()
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = LocalCreateTableSql.DebugActivityInfo;
            command.ExecuteNonQuery();
            command.CommandText = LocalCreateTableSql.Persons;
            command.ExecuteNonQuery();
            command.CommandText = LocalCreateTableSql.Locations;
            command.ExecuteNonQuery();
            command.CommandText = LocalCreateTableSql.Regions;
            command.ExecuteNonQuery();
            command.CommandText = LocalCreateTableSql.RegionRequests;
            command.ExecuteNonQuery();
            command.CommandText = LocalCreateTableSql.LocationRequests;
            command.ExecuteNonQuery();
            command.CommandText = LocalCreateTableSql.CharacterLocationLogs;
            command.ExecuteNonQuery();
            command.CommandText = LocalCreateTableSql.PersonRequests;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Truncate all tables
        /// </summary>
        public void EmptyStore()
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            List<string> tables = [
                "persons",
                "locations",
                "regions",
                "regionrequests",
                "locationrequests",
                "personrequests",
                "characterlocationlogs" ];
            var command = connection.CreateCommand();
            foreach (var tableName in tables)
            {
                command.CommandText = "DELETE FROM " + tableName;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Truncate all player tables - but leave locations and regions
        /// </summary>
        public void ResetStore()
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            List<string> tables = [
                "characterlocationlogs" ];
            var command = connection.CreateCommand();
            foreach (var tableName in tables)
            {
                command.CommandText = "DELETE FROM " + tableName;
                command.ExecuteNonQuery();
            }

        }

        ///
        public Region? GetRegion(Box regionBox)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"  
            SELECT generativeState, details, id
            FROM regions
            WHERE 
                boxLowerLeftLatitude = $latitude AND 
                boxLowerLeftLongitude = $longitude ";
            command.Parameters.AddWithValue("$latitude", regionBox.LowerLeftPoint.Latitude);
            command.Parameters.AddWithValue("$longitude", regionBox.LowerLeftPoint.Longitude);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int state = reader.GetInt32(0);
                var detailsJson = reader.GetString(1);
                int id = reader.GetInt32(2);
                if (state > 0)
                {
                    var region = JsonSerializer.Deserialize<Region>(detailsJson, jsonOption) ?? throw new Exception("Region with a 0<generativeState was null!");
                    region.Id = id;
                    if (region.Box.SameBox(regionBox))
                        return region;
                    else
                        throw new Exception($"Error! Region box mismatch obj:({region.Box.LowerLeftPoint.Longitude},{region.Box.LowerLeftPoint.Latitude}) size:{region.Box.Size}  db:({regionBox.LowerLeftPoint.Longitude},{regionBox.LowerLeftPoint.Latitude}) size:{regionBox.Size}");
                }
                else
                    throw new Exception($"Error! Db region has state:{state}.");
            }
            return null;// did not find the rigion in database
        }

        public bool InsertRegion(Region region)
        {
            string details = JsonSerializer.Serialize(region, jsonOption);
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"  
            INSERT INTO regions ( generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude, details)
            VALUES ( $state,$longitude, $latitude,  $details ) ";
            command.Parameters.AddWithValue("$details", details);
            command.Parameters.AddWithValue("$state", (int)region.State);
            command.Parameters.AddWithValue("$longitude", region.Box.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", region.Box.LowerLeftPoint.Latitude);
            if (command.ExecuteNonQuery() == 1)
                return true;
            else
                return false;
        }

        public bool UpdateRegion(Region region)
        {
            string details = JsonSerializer.Serialize(region, jsonOption);
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"  
            UPDATE regions SET generativeState = $state, details = $details 
            WHERE 
                boxLowerLeftLatitude = $latitude AND 
                boxLowerLeftLongitude = $longitude ";
            command.Parameters.AddWithValue("$longitude", region.Box.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", region.Box.LowerLeftPoint.Latitude);
            command.Parameters.AddWithValue("$details", details);
            command.Parameters.AddWithValue("$state", (int)region.State);
            if (command.ExecuteNonQuery() == 1)
                return true;
            else
                return false;
        }

        ///
        public bool RequestRegion(Box regionBox)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            // test if a request allready exist
            var command = connection.CreateCommand();
            command.CommandText = @"  
            SELECT generativeState
            FROM regionrequests
            WHERE 
                boxLowerLeftLatitude = $latitude AND 
                boxLowerLeftLongitude = $longitude ";
            command.Parameters.AddWithValue("$longitude", regionBox.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", regionBox.LowerLeftPoint.Latitude);
            using var readerTest = command.ExecuteReader();
            while (readerTest.Read())
                if (-1 < readerTest.GetInt32(0))
                    return false;
            readerTest.Close();
            // Test if the region is allready created - and it's generativeState
            command.CommandText = @"  
            SELECT generativeState
            FROM regions
            WHERE 
                boxLowerLeftLatitude = $latitude AND 
                boxLowerLeftLongitude = $longitude ";
            int state = -1;
            using var readerRegions = command.ExecuteReader();
            while (readerRegions.Read())
                state = readerRegions.GetInt32(0);
            readerRegions.Close();
            if ((int)GenerativeState.Done == state)
                return false; // Allready created and done
            // Do it! Create/insert the regionRequest
            command.CommandText = @"
            INSERT INTO regionrequests ( picked, generativeState, requestTime, boxLowerLeftLongitude, boxLowerLeftLatitude )
            VALUES ( 0, $state, $reqtime, $longitude, $latitude )";
            command.Parameters.AddWithValue("$state", (int)GenerativeState.None);
            command.Parameters.AddWithValue("$reqtime", DateTime.Now);
            if (command.ExecuteNonQuery() == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Look up any request for regions, default max one is returned
        /// Initial request are return first
        /// </summary>
        public List<RegionRequest> GetRegionRequest(bool onlyTop = true)
        {
            List<RegionRequest> resultRegionrequests = [];
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            int limit = 1;
            if (!onlyTop)
                limit = 20;

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude
            FROM regionrequests
            WHERE 
                picked = 0 
            ORDER BY generativeState, requestTime DESC
            LIMIT {limit}";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int state = reader.GetInt32(0);
                long longitude = reader.GetInt64(1);
                long latitude = reader.GetInt64(2);
                Box box = new(new(longitude, latitude), Box.regionBoxSize);
                RegionRequest req = new() { Box = box, CurrentGenerativeState = (GenerativeState)state };
                resultRegionrequests.Add(req);
            }
            reader.Close();
            return resultRegionrequests;
        }

        public List<RegionRequest> GetPickedRegionRequest()
        {
            List<RegionRequest> resultRegionrequests = [];
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude
            FROM regionrequests
            WHERE 
                picked = 1 ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int state = reader.GetInt32(0);
                long longitude = reader.GetInt64(1);
                long latitude = reader.GetInt64(2);
                Box box = new(new(longitude, latitude), Box.regionBoxSize);
                RegionRequest req = new() { Box = box, CurrentGenerativeState = (GenerativeState)state };
                resultRegionrequests.Add(req);
            }
            reader.Close();
            return resultRegionrequests;
        }

        public bool UpdateRegionRequestState(RegionRequest regionRequest)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"  
            UPDATE regionrequests SET generativeState = $state 
            WHERE
                boxLowerLeftLongitude = $logitude AND
                boxLowerLeftLatitude = $latitude ";
            command.Parameters.AddWithValue("$state", (int)regionRequest.CurrentGenerativeState);
            command.Parameters.AddWithValue("$logitude", regionRequest.Box.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", regionRequest.Box.LowerLeftPoint.Latitude);
            int result = command.ExecuteNonQuery();
            if (result == 1)
                return true;
            else
                return false;
        }

        public bool UpdateRegionRequestPicked(RegionRequest regionRequest, bool picked)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"  
            UPDATE regionrequests SET picked = $picked 
            WHERE
                boxLowerLeftLongitude = $logitude AND
                boxLowerLeftLatitude = $latitude ";
            if (picked)
                command.Parameters.AddWithValue("$picked", 1);
            else
                command.Parameters.AddWithValue("$picked", 0);
            command.Parameters.AddWithValue("$logitude", regionRequest.Box.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", regionRequest.Box.LowerLeftPoint.Latitude);
            int result = command.ExecuteNonQuery();
            if (result == 1)
                return true;
            else
                return false;
        }

        ///
        public Location? GetLocation(Box locationBox)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"  
            SELECT generativeState, details, id
            FROM locations
            WHERE 
                boxLowerLeftLongitude = $logitude AND
                boxLowerLeftLatitude = $latitude ";
            command.Parameters.AddWithValue("$logitude", locationBox.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", locationBox.LowerLeftPoint.Latitude);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int state = reader.GetInt32(0);
                var detailsJson = reader.GetString(1);
                int id = reader.GetInt32(2);
                if (state > 0)
                {
                    var location = JsonSerializer.Deserialize<Location>(detailsJson, jsonOption) ?? throw new Exception("Could not deserialize location");
                    location.Id = id;
                    if (location.Box.SameBox(locationBox))
                        return location;
                    else
                        throw new Exception($"Error! Location box mismatch obj:({location.Box.LowerLeftPoint.Longitude},{location.Box.LowerLeftPoint.Latitude}) size:{location.Box.Size} db:({locationBox.LowerLeftPoint.Longitude},{locationBox.LowerLeftPoint.Latitude}) size:{locationBox.Size}");
                }
                else
                    throw new Exception($"Error! Location has state {state}.");
            }
            return null;// did not find the location
        }

        ///        
        public Location? GetLocation(int id)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"  
            SELECT generativeState, details
            FROM locations
            WHERE 
                id = $id ";
            command.Parameters.AddWithValue("$id", id);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int state = reader.GetInt32(0);
                var detailsJson = reader.GetString(1);
                if (state > 0)
                {
                    var location = JsonSerializer.Deserialize<Location>(detailsJson, jsonOption) ?? throw new Exception("Could not deserialize location");
                    location.Id = id;
                    return location;
                }
                else
                    throw new Exception($"Error! Location has state {state}.");
            }
            return null;// did not find the location
        }


        /// When the Id is known the person is already created
        public Person? GetPerson(int id)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"  
            SELECT details
            FROM persons
            WHERE 
                id = $id ";
            command.Parameters.AddWithValue("$id", id);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var detailsJson = reader.GetString(0);
                var dbP = JsonSerializer.Deserialize<Person>(detailsJson, jsonOption);
                return dbP;
            }

            return null;
        }

        public bool SavePerson(Person person)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"  
                INSERT INTO persons ( id, generativeState, details )
                VALUES ( $id, $state, $details ) ";
            command.Parameters.AddWithValue("$id", person.Id);
            command.Parameters.AddWithValue("$state", (int)person.State);
            string details = JsonSerializer.Serialize(person, jsonOption);
            command.Parameters.AddWithValue("$details", details);
            int result = command.ExecuteNonQuery();
            if (result == 1)
                return true;
            else
                return false;
        }

        public bool UpdatePerson(Person person)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = @"  
                UPDATE persons SET generativeState = $state, details = $details
                WHERE
                    id = $id   ";
            command.Parameters.AddWithValue("$id", person.Id);
            command.Parameters.AddWithValue("$state", (int)person.State);
            string details = JsonSerializer.Serialize(person, jsonOption);
            command.Parameters.AddWithValue("$details", details);
            int result = command.ExecuteNonQuery();
            if (result == 1)
                return true;
            else
                return false;
        }

        public bool InsertLocation(Location location)
        {
            string details = JsonSerializer.Serialize(location, jsonOption);
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"  
            INSERT INTO locations (  generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude, details)
            VALUES ( $state, $longitude, $latitude, $details ) ";
            command.Parameters.AddWithValue("$details", details);
            command.Parameters.AddWithValue("$state", (int)location.State);
            command.Parameters.AddWithValue("$longitude", location.Box.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", location.Box.LowerLeftPoint.Latitude);
            if (command.ExecuteNonQuery() == 1)
                return true;
            else
                return false;
        }

        public bool UpdateLocation(Location location)
        {
            string details = JsonSerializer.Serialize(location, jsonOption);
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"  
            UPDATE locations SET generativeState = $state, details = $details 
            WHERE 
                boxLowerLeftLatitude = $latitude AND 
                boxLowerLeftLongitude = $longitude ";
            command.Parameters.AddWithValue("$longitude", location.Box.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", location.Box.LowerLeftPoint.Latitude);
            command.Parameters.AddWithValue("$details", details);
            command.Parameters.AddWithValue("$state", (int)location.State);
            command.ExecuteNonQuery();
            if (command.ExecuteNonQuery() == 1)
                return true;
            else
                return false;
        }

        public CharacterLocationLog? GetCurrentCharacterLocationLog(int characterId)
        {
            CharacterLocationLog? result = null;
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT id, pointLongitude, pointLatitude, boxLowerLeftLongitude, boxLowerLeftLatitude, status, changedAt, firstAt, visitCount
            FROM characterlocationlogs
            WHERE 
                characterId = $characterId AND
                ( status = {(int)CharacterLocationLog.Status.Current} OR 
                    status = {(int)CharacterLocationLog.Status.Departed}   ) ";
            command.Parameters.AddWithValue("$characterId", characterId);
            int resultCounter = 0;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                resultCounter++;
                var id = reader.GetInt64(0);
                long pointLongitude = reader.GetInt64(1);
                long pointLatitude = reader.GetInt64(2);
                long boxLongitude = reader.GetInt64(3);
                long boxLatitude = reader.GetInt64(4);
                int status = reader.GetInt32(5);
                DateTime changedAt = reader.GetDateTime(6);
                DateTime? firstAt = null;
                if (!reader.IsDBNull(7))
                    firstAt = reader.GetDateTime(7);
                int visitCount = 0;
                if (!reader.IsDBNull(8))
                    visitCount = reader.GetInt32(8);
                Point p = new(pointLongitude, pointLatitude);
                Box box = new(new(boxLongitude, boxLatitude), Box.pointBoxSize);
                result = new()
                {
                    Id = id,
                    LocationPoint = p,
                    LocationBox = box,
                    CharacterLocationStatus = (CharacterLocationLog.Status)status,
                    ChangedAt = changedAt,
                    FirstAt = firstAt,
                    VisitCount = visitCount
                };
            }
            if (resultCounter > 1)
                throw new Exception($"To many current character locations! count:{resultCounter} characterId:{characterId}");
            else
                return result;
        }

        public CharacterLocationLog? GetCharacterDestinationLocationLog(int characterId)
        {
            CharacterLocationLog? result = null;
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT id, pointLongitude, pointLatitude, boxLowerLeftLongitude, boxLowerLeftLatitude, changedAt, visitCount
            FROM characterlocationlogs
            WHERE 
                characterId = $characterId AND  
                status = {(int)CharacterLocationLog.Status.OnRoute} ";
            command.Parameters.AddWithValue("$characterId", characterId);
            int resultCounter = 0;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                resultCounter++;
                var id = reader.GetInt64(0);
                long pointLongitude = reader.GetInt64(1);
                long pointLatitude = reader.GetInt64(2);
                long boxLongitude = reader.GetInt64(3);
                long boxLatitude = reader.GetInt64(4);
                DateTime changedAt = reader.GetDateTime(5);
                int visitCount = reader.GetInt32(6);
                Point p = new(pointLongitude, pointLatitude);
                Box box = new(new(boxLongitude, boxLatitude), Box.pointBoxSize);
                result = new()
                {
                    Id = id,
                    LocationPoint = p,
                    LocationBox = box,
                    CharacterLocationStatus = CharacterLocationLog.Status.OnRoute,
                    ChangedAt = changedAt,
                    VisitCount = visitCount
                };
            }
            if (resultCounter > 1)
                throw new Exception($"To many on route character locations! count:{resultCounter} characterId:{characterId}");
            else
                return result;
        }

        /// <summary>
        /// Update/insert characterlocationlogs to reflect the arrival
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="arrivalPoint"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool Arrive(int characterId, Box arrivalBox, Point arrivalPoint)
        {
            if (!arrivalBox.SameBox(Box.GetLocationBox(arrivalPoint))) // Test that box and point match
                throw new Exception("Can not arrive! Box and point do not fit together!");

            int onRouteId = -1;  // this is the one expected normal one
            List<long> depatureIds = []; // there should normaly also be one of these, extras will be set to historic
            List<long> failedOnRouteIds = []; // there should normaly be none of these, extras will be deleted
            List<long> failedOnRouteIdsWithHistory = [];// there should normaly be none of these, extras will be set to historic
            List<long> currentIds = []; // there should normaly be none of these, extras will be set to historic
            List<long> historicIds = []; // normaly these will be skipped
            int previousVisitCountAtDestination = 0;
            DateTime changedAt = DateTime.Now;

            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();

            // Find current,departure and onroute ids (if there is none - then it is the start of the game)
            command.CommandText = @$" 
            SELECT id, status, visitCount, boxLowerLeftLongitude, boxLowerLeftLatitude
            FROM characterlocationlogs
            WHERE 
                characterId = $characterId ";
            command.Parameters.AddWithValue("$characterId", characterId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                // read record
                int id = reader.GetInt32(0);
                int status = reader.GetInt32(1);
                int visitCout = reader.GetInt32(2);
                long boxLongitude = reader.GetInt64(3);
                long boxLatitude = reader.GetInt64(4);
                Box box = new(new(boxLongitude, boxLatitude), Box.pointBoxSize);
                if (box.SameBox(arrivalBox))
                {
                    previousVisitCountAtDestination = visitCout;
                    onRouteId = id;
                }
                // sort/organize
                if (status == (int)CharacterLocationLog.Status.Current)
                    currentIds.Add(id);
                else if (status == (int)CharacterLocationLog.Status.Departed)
                    depatureIds.Add(id);
                else if (status == (int)CharacterLocationLog.Status.OnRoute && 0 < visitCout)
                    failedOnRouteIdsWithHistory.Add(id);
                else if (status == (int)CharacterLocationLog.Status.OnRoute && visitCout <= 0)
                    failedOnRouteIds.Add(id);
                else if (status == (int)CharacterLocationLog.Status.Historic)
                    historicIds.Add(id);
                else
                    throw new Exception("Fix code! Found a wrong characterlocationlogs when arriving");
            }
            reader.Close();
            int debugExecuteNonQuery = 0;
            // Now update/delete them all
            command.Parameters.Clear();
            foreach (var id in failedOnRouteIds)
            {
                if (id == onRouteId)
                    continue;//it will be handled last
                command.CommandText = @"  
                    DELETE FROM characterlocationlogs 
                    WHERE
                        id = $failedOnRouteId ";
                command.Parameters.AddWithValue("$failedOnRouteId", id);
                debugExecuteNonQuery = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            command.CommandText = @$" 
                    UPDATE characterlocationlogs 
                    SET status = $status, changedAt = $changedAt 
                    WHERE 
                        id = $id ";
            foreach (var id in failedOnRouteIdsWithHistory)
            {
                if (id == onRouteId)
                    continue;//it will be handled last
                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$changedAt", changedAt);
                command.Parameters.AddWithValue("$status", (int)CharacterLocationLog.Status.Historic);
                debugExecuteNonQuery = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            foreach (var id in depatureIds)
            {
                if (id == onRouteId)
                    continue;//it will be handled last
                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$changedAt", changedAt);
                command.Parameters.AddWithValue("$status", (int)CharacterLocationLog.Status.Historic);
                debugExecuteNonQuery = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            foreach (var id in currentIds)
            {
                if (id == onRouteId)
                    continue;//it will be handled last
                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$changedAt", changedAt);
                command.Parameters.AddWithValue("$status", (int)CharacterLocationLog.Status.Historic);
                debugExecuteNonQuery = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            if (0 < onRouteId)
            {
                command.CommandText = @$" 
                UPDATE characterlocationlogs 
                SET status = $status, changedAt = $changedAt, visitCount = $count 
                WHERE 
                    id = $onrouteId ";
                command.Parameters.AddWithValue("$onrouteId", onRouteId);
                command.Parameters.AddWithValue("$changedAt", changedAt);
                command.Parameters.AddWithValue("$status", (int)CharacterLocationLog.Status.Current);
                command.Parameters.AddWithValue("$count", previousVisitCountAtDestination + 1);
                debugExecuteNonQuery = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            else
            {
                command.CommandText = @$" 
                INSERT INTO characterlocationlogs ( characterId, pointLongitude, pointLatitude, boxLowerLeftLongitude, boxLowerLeftLatitude, status, changedAt, firstAt, visitCount ) 
                VALUES ( $characterId, $pointLongitude, $pointLatitude, $boxLongitude, $boxLatitude,$status, $changedAt, $changedAt, $vistitCount )   ";
                command.Parameters.AddWithValue("$characterId", characterId);
                command.Parameters.AddWithValue("$pointLongitude", arrivalPoint.Longitude);
                command.Parameters.AddWithValue("$pointLatitude", arrivalPoint.Latitude);
                command.Parameters.AddWithValue("$boxLongitude", arrivalBox.LowerLeftPoint.Longitude);
                command.Parameters.AddWithValue("$boxLatitude", arrivalBox.LowerLeftPoint.Latitude);
                command.Parameters.AddWithValue("$status", (int)CharacterLocationLog.Status.Current);
                command.Parameters.AddWithValue("$changedAt", changedAt);
                command.Parameters.AddWithValue("$vistitCount", 1);
                debugExecuteNonQuery = command.ExecuteNonQuery();
            }
            _ = debugExecuteNonQuery;
            return true;
        }

        public bool Depart(int characterId, Box destinationBox, Point destinationPoint)
        {
            if (!destinationBox.SameBox(Box.GetLocationBox(destinationPoint))) // Test that box and point match
                throw new Exception("Can not depart! The box and point do not fit together!");
            int destinationId = -1;  // this is the expected normal one
            List<long> currentIds = []; // one of these is the expected normal case, extras will be set to historic
            List<long> depatureIds = [];// none of these is expected, extras will be set to historic
            List<long> failedOnRouteIds = [];// none of these is expected, extras will be deleted
            List<long> failedOnRouteIdsWithHistory = [];// none of these is expected, extras will be set to historic            
            List<long> historicIds = []; // normaly these will be skipped
            DateTime changedAt = DateTime.Now;

            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();

            // Find current,departure and onroute ids (if there is none - then it is the start of the game)
            command.CommandText = @$" 
            SELECT id, status, visitCount, boxLowerLeftLongitude, boxLowerLeftLatitude
            FROM characterlocationlogs
            WHERE 
                characterId = $characterId ";
            command.Parameters.AddWithValue("$characterId", characterId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                // read record
                int id = reader.GetInt32(0);
                int status = reader.GetInt32(1);
                int visitCout = reader.GetInt32(2);
                long boxLongitude = reader.GetInt64(3);
                long boxLatitude = reader.GetInt64(4);
                Box box = new(new(boxLongitude, boxLatitude), Box.pointBoxSize);
                if (box.SameBox(destinationBox))
                    destinationId = id;
                // sort/organize
                if (status == (int)CharacterLocationLog.Status.Current)
                    currentIds.Add(id);
                else if (status == (int)CharacterLocationLog.Status.Departed)
                    depatureIds.Add(id);
                else if (status == (int)CharacterLocationLog.Status.OnRoute && 0 < visitCout)
                    failedOnRouteIdsWithHistory.Add(id);
                else if (status == (int)CharacterLocationLog.Status.OnRoute && visitCout <= 0)
                    failedOnRouteIds.Add(id);
                else if (status == (int)CharacterLocationLog.Status.Historic)
                    historicIds.Add(id);
                else
                    throw new Exception("Fix code! Found a wrong characterlocationlogs when detarting");
            }
            reader.Close();
            int debugExecuteNonQuery = 0;
            // Now update/delete them all
            command.Parameters.Clear();
            foreach (var id in failedOnRouteIds)
            {
                if (id == destinationId)
                    continue;//it will be handled last
                command.CommandText = @"  
                DELETE FROM characterlocationlogs 
                WHERE
                    id = $failedOnRouteId ";
                command.Parameters.AddWithValue("$failedOnRouteId", id);
                debugExecuteNonQuery = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            command.CommandText = @$" 
            UPDATE characterlocationlogs 
            SET status = $status, changedAt = $changedAt 
            WHERE 
                id = $id ";
            foreach (var id in failedOnRouteIdsWithHistory)
            {
                if (id == destinationId)
                    continue;//it will be handled last
                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$changedAt", changedAt);
                command.Parameters.AddWithValue("$status", (int)CharacterLocationLog.Status.Historic);
                debugExecuteNonQuery = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            foreach (var id in depatureIds)
            {
                if (id == destinationId)
                    continue;//it will be handled last
                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$changedAt", changedAt);
                command.Parameters.AddWithValue("$status", (int)CharacterLocationLog.Status.Historic);
                debugExecuteNonQuery = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            int statusPlural = (int)CharacterLocationLog.Status.Departed;
            foreach (var id in currentIds)
            {
                if (id == destinationId)
                    continue;//it will be handled last
                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$changedAt", changedAt);
                command.Parameters.AddWithValue("$status", statusPlural);
                debugExecuteNonQuery = command.ExecuteNonQuery();
                command.Parameters.Clear();
                statusPlural = (int)CharacterLocationLog.Status.Historic; // if more than one current the extra are set to historic
            }

            if (0 < destinationId)
            {
                command.CommandText = @$" 
                UPDATE characterlocationlogs 
                SET status = $status , changedAt = $changedAt 
                WHERE 
                    id = $id ";
                command.Parameters.AddWithValue("$id", destinationId);
                command.Parameters.AddWithValue("$status", (int)CharacterLocationLog.Status.OnRoute);
                command.Parameters.AddWithValue("$changedAt", changedAt);
                debugExecuteNonQuery = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            else
            {
                command.CommandText = @$" 
                INSERT INTO characterlocationlogs ( characterId, pointLongitude, pointLatitude, boxLowerLeftLongitude, boxLowerLeftLatitude, status, changedAt, firstAt, visitCount ) 
                VALUES ( $characterId, $pointLongitude, $pointLatitude, $boxLongitude, $boxLatitude,$status, $changedAt, $changedAt, $vistitCount )   
                ";
                command.Parameters.AddWithValue("$characterId", characterId);
                command.Parameters.AddWithValue("$pointLongitude", destinationPoint.Longitude);
                command.Parameters.AddWithValue("$pointLatitude", destinationPoint.Latitude);
                command.Parameters.AddWithValue("$boxLongitude", destinationBox.LowerLeftPoint.Longitude);
                command.Parameters.AddWithValue("$boxLatitude", destinationBox.LowerLeftPoint.Latitude);
                command.Parameters.AddWithValue("$status", (int)CharacterLocationLog.Status.OnRoute);
                command.Parameters.AddWithValue("$changedAt", changedAt);
                command.Parameters.AddWithValue("$vistitCount", 0);
                debugExecuteNonQuery = command.ExecuteNonQuery();
            }
            _ = debugExecuteNonQuery;
            return true;
        }


        public CharacterLocationLog? GetCharacterLocationLog(int characterId, Box locationBox)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @$" 
                SELECT id, pointLongitude, pointLatitude, status, changedAt, visitCount
                FROM characterlocationlogs
                WHERE 
                    characterId = $characterId AND  
                    boxLowerLeftLongitude = $boxLongitude AND
                    boxLowerLeftLatitude = $boxLatitude ";
            command.Parameters.AddWithValue("$characterId", characterId);
            command.Parameters.AddWithValue("$boxLongitude", locationBox.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$boxLatitude", locationBox.LowerLeftPoint.Latitude);
            int resultCounter = 0;

            CharacterLocationLog? result = null;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                resultCounter++;
                var id = reader.GetInt64(0);
                long pointLongitude = reader.GetInt64(1);
                long pointLatitude = reader.GetInt64(2);
                int status = reader.GetInt32(3);
                DateTime changedAt = reader.GetDateTime(4);
                int visitCount = reader.GetInt32(5);
                Point p = new(pointLongitude, pointLatitude);
                result = new()
                {
                    Id = id,
                    LocationPoint = p,
                    LocationBox = locationBox,
                    CharacterLocationStatus = (CharacterLocationLog.Status)status,
                    ChangedAt = changedAt,
                    VisitCount = visitCount
                };
            }
            if (resultCounter > 1)
                throw new Exception($"To many departure character locations! count:{resultCounter} characterId:{characterId}");
            reader.Close();
            return result;
        }


        public List<CharacterLocationLog> GetCharacterLocationLog(int characterId)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT id, pointLongitude, pointLatitude, status, changedAt, visitCount, boxLowerLeftLongitude, boxLowerLeftLatitude
            FROM characterlocationlogs
            WHERE 
                characterId = $characterId ";
            command.Parameters.AddWithValue("$characterId", characterId);

            List<CharacterLocationLog> result = [];
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetInt64(0);
                long pointLongitude = reader.GetInt64(1);
                long pointLatitude = reader.GetInt64(2);
                int status = reader.GetInt32(3);
                DateTime changedAt = reader.GetDateTime(4);
                int visitCount = reader.GetInt32(5);
                Point p = new(pointLongitude, pointLatitude);
                long boxLongitude = reader.GetInt64(6);
                long boxLatitude = reader.GetInt64(7);
                Box locationBox = new(new(boxLongitude, boxLatitude), Box.pointBoxSize);
                CharacterLocationLog item = new()
                {
                    Id = id,
                    LocationPoint = p,
                    LocationBox = locationBox,
                    CharacterLocationStatus = (CharacterLocationLog.Status)status,
                    ChangedAt = changedAt,
                    VisitCount = visitCount
                };
                result.Add(item);
            }
            reader.Close();
            return result;
        }
        
        public List<Person> GetPersonsAtLocation(Box locationBox)
        {
            List<Person> result = [];
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT persons.details
            FROM characterlocationlogs
            INNER JOIN persons ON persons.id = characterlocationlogs.characterId
            WHERE 
                characterlocationlogs.status = {(int)CharacterLocationLog.Status.Current} AND
                characterlocationlogs.boxLowerLeftLongitude  = $longitude AND
                characterlocationlogs.boxLowerLeftLatitude = $latitude ";
            command.Parameters.AddWithValue("$longitude", locationBox.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", locationBox.LowerLeftPoint.Latitude);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var detailsJson = reader.GetString(0);
                var dbP = JsonSerializer.Deserialize<Person>(detailsJson, jsonOption);
                if(dbP is not null)
                    result.Add(dbP);
            }
            reader.Close();
            return result;
        }

        public bool RequestLocation(Box locationBox)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            // Test if the location is allready requested
            var command = connection.CreateCommand();
            command.CommandText = @"  
            SELECT generativeState
            FROM locationrequests
            WHERE 
                boxLowerLeftLatitude = $latitude AND 
                boxLowerLeftLongitude = $longitude ";
            command.Parameters.AddWithValue("$longitude", locationBox.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", locationBox.LowerLeftPoint.Latitude);
            using var readerTest = command.ExecuteReader();
            while (readerTest.Read())
                if (-1 < readerTest.GetInt32(0))
                    return false;
            readerTest.Close();
            // Test if the location is allready created - and it's generativeState
            command.CommandText = @"  
            SELECT generativeState
            FROM locations
            WHERE 
                boxLowerLeftLatitude = $latitude AND 
                boxLowerLeftLongitude = $longitude ";
            int state = -1;
            using var readerLocations = command.ExecuteReader();
            while (readerLocations.Read())
                state = readerLocations.GetInt32(0);
            readerLocations.Close();
            if ((int)GenerativeState.Done == state)
                return false; // Allready created and done

            // Do it! Create/insert the locationRequest
            command.CommandText = @"
            INSERT INTO locationrequests ( picked, generativeState, requestTime, boxLowerLeftLongitude, boxLowerLeftLatitude )
            VALUES ( 0, $state, $reqtime, $longitude, $latitude )";
            command.Parameters.AddWithValue("$state", (int)GenerativeState.None);
            command.Parameters.AddWithValue("$reqtime", DateTime.Now);
            if (command.ExecuteNonQuery() == 1)
                return true;
            else
                return false;
        }

        public bool UpdateLocationRequestState(LocationRequest locationRequest)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"  
            UPDATE locationrequests SET generativeState = $state 
            WHERE
                boxLowerLeftLongitude = $logitude AND
                boxLowerLeftLatitude = $latitude ";
            command.Parameters.AddWithValue("$state", (int)locationRequest.CurrentGenerativeState);
            command.Parameters.AddWithValue("$logitude", locationRequest.Box.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", locationRequest.Box.LowerLeftPoint.Latitude);
            int result = command.ExecuteNonQuery();
            if (result == 1)
                return true;
            else
                return false;
        }

        public bool UpdateLocationRequestPicked(LocationRequest locationRequest, bool picked)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"  
            UPDATE locationrequests SET picked = $picked 
            WHERE
                boxLowerLeftLongitude = $logitude AND
                boxLowerLeftLatitude = $latitude ";
            if (picked)
                command.Parameters.AddWithValue("$picked", 1);
            else
                command.Parameters.AddWithValue("$picked", 0);
            command.Parameters.AddWithValue("$logitude", locationRequest.Box.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", locationRequest.Box.LowerLeftPoint.Latitude);
            int result = command.ExecuteNonQuery();
            if (result == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Look up any request for locations, default max one is returned
        /// Initial request are return first
        /// </summary>
        public List<LocationRequest> GetLocationRequest(bool onlyTopOne = true)
        {
            int limit = 1;
            if (!onlyTopOne)
                limit = 20;
            List<LocationRequest> resultLocationRequests = [];
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude, requestTime
            FROM locationrequests
            WHERE 
                picked = 0  
            ORDER BY generativeState, requestTime DESC
            LIMIT {limit}";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int state = reader.GetInt32(0);
                long longitude = reader.GetInt64(1);
                long latitude = reader.GetInt64(2);
                DateTime requestTime = reader.GetDateTime(3);
                Box box = new(new(longitude, latitude), Box.pointBoxSize);
                LocationRequest req = new() { Box = box, CurrentGenerativeState = (GenerativeState)state, RequestTime = requestTime };
                resultLocationRequests.Add(req);
            }
            reader.Close();
            return resultLocationRequests;
        }


        public LocationRequest? GetLocationRequest(Box locationBox)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT generativeState, requestTime
            FROM locationrequests
            WHERE 
                boxLowerLeftLongitude = $longitude AND
                boxLowerLeftLatitude = $latitude
            ";
            command.Parameters.AddWithValue("$longitude", locationBox.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", locationBox.LowerLeftPoint.Latitude);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int state = reader.GetInt32(0);
                DateTime requestTime = reader.GetDateTime(1);
                return new() { Box = locationBox, CurrentGenerativeState = (GenerativeState)state, RequestTime = requestTime };
            }            
            reader.Close();
            return null;
        }


        public List<LocationRequest> GetPickedLocationRequest()
        {
            List<LocationRequest> resultLocationRequests = [];
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude, requestTime
            FROM locationrequests
            WHERE 
                picked = 1 ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int state = reader.GetInt32(0);
                long longitude = reader.GetInt64(1);
                long latitude = reader.GetInt64(2);
                DateTime requestTime = reader.GetDateTime(3);
                Box box = new(new(longitude, latitude), Box.pointBoxSize);
                LocationRequest req = new() { Box = box, CurrentGenerativeState = (GenerativeState)state, RequestTime = requestTime };
                resultLocationRequests.Add(req);
            }
            reader.Close();
            return resultLocationRequests;
        }

        public int RequestPerson(PersonRequest personRequest)
        {
            string details = JsonSerializer.Serialize(personRequest, jsonOption);
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO personrequests ( picked, generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude, requestTime, details )
            VALUES ( 0, $state, $longitude, $latitude, $reqtime, $details );
            select last_insert_rowid();";
            command.Parameters.AddWithValue("$details", details);
            command.Parameters.AddWithValue("$longitude", personRequest.LocationBox.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", personRequest.LocationBox.LowerLeftPoint.Latitude);
            command.Parameters.AddWithValue("$state", (int)GenerativeState.None);
            command.Parameters.AddWithValue("$reqtime", DateTime.Now);
            int lastId = Convert.ToInt32(command.ExecuteScalar());
            return lastId;
        }

        /// <summary>
        /// Look up any request for persons, default max one is returned
        /// Initial request are return first
        /// </summary>
        public List<PersonRequest> GetPersonRequest(bool onlyTopOne = true)
        {
            int limit = 1;
            if (!onlyTopOne)
                limit = 20;
            List<PersonRequest> resultPersonRequests = [];
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT id, generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude, requestTime , details 
            FROM personrequests
            WHERE 
                picked = 0 
            ORDER BY generativeState, requestTime DESC
            LIMIT {limit}";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int state = reader.GetInt32(1);
                long longitude = reader.GetInt64(2);
                long latitude = reader.GetInt64(3);
                DateTime requestTime = reader.GetDateTime(4);
                string details = reader.GetString(5);

                Box box = new(new(longitude, latitude), Box.regionBoxSize);
                var personRequest = JsonSerializer.Deserialize<PersonRequest>(details, jsonOption) ?? throw new Exception("Missing details in Person request!");
                personRequest.Id = id;
                personRequest.CurrentGenerativeState = (GenerativeState)state;
                resultPersonRequests.Add(personRequest);
            }
            reader.Close();
            return resultPersonRequests;
        }

        public PersonRequest? GetPersonRequest(int id)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude, requestTime , details 
            FROM personrequests
            WHERE 
                id = $id";
            command.Parameters.AddWithValue("$id", id);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int state = reader.GetInt32(0);
                long longitude = reader.GetInt64(1);
                long latitude = reader.GetInt64(2);
                DateTime requestTime = reader.GetDateTime(3);
                string details = reader.GetString(4);

                Box box = new(new(longitude, latitude), Box.regionBoxSize);
                var personRequest = JsonSerializer.Deserialize<PersonRequest>(details, jsonOption) ?? throw new Exception("Missing details in Person request!");
                personRequest.Id = id;
                personRequest.CurrentGenerativeState = (GenerativeState)state;
                return personRequest;
            }
            reader.Close();
            return null;
        }

        public List<PersonRequest> GetPickedPersonRequest()
        {
            List<PersonRequest> resultPersonRequests = [];
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT id, generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude, requestTime , details 
            FROM personrequests
            WHERE 
                picked = 0 ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int state = reader.GetInt32(1);
                long longitude = reader.GetInt64(2);
                long latitude = reader.GetInt64(3);
                DateTime requestTime = reader.GetDateTime(4);
                string details = reader.GetString(5);

                Box box = new(new(longitude, latitude), Box.regionBoxSize);
                var personRequest = JsonSerializer.Deserialize<PersonRequest>(details, jsonOption) ?? throw new Exception("Missing details in Person request!");
                personRequest.Id = id;
                personRequest.CurrentGenerativeState = (GenerativeState)state;
                resultPersonRequests.Add(personRequest);
            }
            reader.Close();
            return resultPersonRequests;
        }

        public bool UpdatePersonRequestState(PersonRequest personRequest)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"  
            UPDATE personrequests SET generativeState = $state 
            WHERE
                id = $id ";
            command.Parameters.AddWithValue("$state", (int)personRequest.CurrentGenerativeState);
            command.Parameters.AddWithValue("$id", personRequest.Id);
            int result = command.ExecuteNonQuery();
            if (result == 1)
                return true;
            else
                return false;
        }

        public bool UpdatePersonRequestPicked(PersonRequest personRequest, bool picked)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"  
            UPDATE personrequests SET picked = $picked 
            WHERE
                id = $id ";
            if (picked)
                command.Parameters.AddWithValue("$picked", 1);
            else
                command.Parameters.AddWithValue("$picked", 0);
            command.Parameters.AddWithValue("$id", personRequest.Id);
            int result = command.ExecuteNonQuery();
            if (result == 1)
                return true;
            else
                return false;
        }

        public List<Metadata> GetPersonMetadata()
        {
            int count = 0;
            List<Metadata> result = [];
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT id, generativeState, details
            FROM persons                
            ORDER BY generativeState, id
            LIMIT 50 ";

            Box box = new(new(0, 0), 0);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                count++;
                int dbId = reader.GetInt32(0);
                int dbState = reader.GetInt32(1);
                string details = reader.GetString(2);
                var person = JsonSerializer.Deserialize<Person>(details, jsonOption);
                Metadata metadata = new(
                    count,
                    person?.Id ?? -1,
                    dbId,
                    person?.Name ?? "",
                    person?.Description ?? "",
                    box,
                    (GenerativeState)dbState,
                    person?.State ?? GenerativeState.None,
                    person?.DescriptionPromptByLLM ?? "");
                result.Add(metadata);
            }
            reader.Close();
            return result;
        }

        public List<Metadata> GetRegionMetadataList()
        {
            int count = 0;
            List<Metadata> result = [];
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT id, generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude, details
            FROM regions                
            ORDER BY generativeState, id ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                count++;
                int dbId = reader.GetInt32(0);
                int dbState = reader.GetInt32(1);
                long longitude = reader.GetInt64(2);
                long latitude = reader.GetInt64(3);
                string details = reader.GetString(4);
                var region = JsonSerializer.Deserialize<Region>(details, jsonOption);
                Box box = new(new(longitude, latitude), Box.regionBoxSize);
                Metadata metadata = new(
                    count,
                    region?.Id ?? -1,
                    dbId,
                    region?.Name ?? "",
                    "",
                    box,
                    (GenerativeState)dbState,
                    region?.State ?? GenerativeState.None,
                    "");
                result.Add(metadata);
            }
            reader.Close();
            return result;
        }

        public List<Metadata> GetLocationMetadata()
        {
            int count = 0;
            List<Metadata> result = [];
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT id, generativeState, boxLowerLeftLongitude, boxLowerLeftLatitude, details
            FROM locations                
            ORDER BY generativeState, id
            LIMIT 50 ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                count++;
                int dbId = reader.GetInt32(0);
                int dbState = reader.GetInt32(1);
                long longitude = reader.GetInt64(2);
                long latitude = reader.GetInt64(3);
                string details = reader.GetString(4);
                var location = JsonSerializer.Deserialize<Location>(details, jsonOption);
                Box box = new(new(longitude, latitude), Box.regionBoxSize);
                Metadata metadata = new(
                    count,
                    location?.Id ?? -1,
                    dbId,
                    location?.Name ?? "",
                    location?.Description ?? "",
                    box,
                    (GenerativeState)dbState,
                    location?.State ?? GenerativeState.None,
                    location?.DescriptionPromptByLLM ?? "");
                result.Add(metadata);
            }
            reader.Close();
            return result;
        }

        public bool DeleteLocationRequest(Box box)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"  
            DELETE FROM locationrequests 
            WHERE
                boxLowerLeftLongitude = $longitude AND
                boxLowerLeftLatitude = $latitude ";
            command.Parameters.AddWithValue("$longitude", box.LowerLeftPoint.Longitude);
            command.Parameters.AddWithValue("$latitude", box.LowerLeftPoint.Latitude);
            int result = command.ExecuteNonQuery();
            if (result == 1)
                return true;
            else
                return false;
        }

        public int UnpickAll()
        {
            int count = 0;
            foreach (var pickedRegionRequest in GetPickedRegionRequest())
                if (UpdateRegionRequestPicked(pickedRegionRequest, false))
                    count++;
                else
                    throw new Exception("Could not unpick region request at start up clean!");
            foreach (var pickedLocationRequest in GetPickedLocationRequest())
                if (UpdateLocationRequestPicked(pickedLocationRequest, false))
                    count++;
                else
                    throw new Exception("Could not unpick location request at start up clean!");
            foreach (var pickedPersonRequest in GetPickedPersonRequest())
                if (UpdatePersonRequestPicked(pickedPersonRequest, false))
                    count++;
                else
                    throw new Exception("Could not unpick person request at start up clean!");
            return count;
        }

        public void SetDebugActivityInfo(DebugActivityInfo debugActivityInfo)
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"  
            UPDATE debugactivityinfo SET activitytext = $activitytext
            WHERE id = 1 ";
            command.Parameters.AddWithValue("$activitytext", debugActivityInfo.Text);
            command.ExecuteNonQuery();
        }

        public DebugActivityInfo GetDebugActivityInfo()
        {
            using var connection = new SqliteConnection($"Data Source={_dbFullName}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @$" 
            SELECT id, activitytext
            FROM debugactivityinfo                
            WHERE id = 1 ";

            DebugActivityInfo result = new("");
            using var reader = command.ExecuteReader();
            while (reader.Read())
                result.Text = reader.GetString(1);
            reader.Close();

            return result;
        }
    }
}