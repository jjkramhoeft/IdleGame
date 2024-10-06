
namespace Storage
{
    public static class LocalCreateTableSql
    {
        public const string DebugActivityInfo = @"
        CREATE TABLE IF NOT EXISTS debugactivityinfo (
            id INTEGER NOT NULL PRIMARY KEY,
            activitytext TEXT );
        DELETE FROM debugactivityinfo; 
        INSERT INTO debugactivityinfo (id)
            VALUES ( 1 ) 
        ";

        public const string Persons = @"
        CREATE TABLE IF NOT EXISTS persons (
            id INTEGER NOT NULL PRIMARY KEY,
            generativeState INTEGER NOT NULL,
            details TEXT NOT NULL
        );            
        ";

        public const string Locations = @"
        CREATE TABLE IF NOT EXISTS locations (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            generativeState INTEGER NOT NULL,
            boxLowerLeftLongitude LONG NOT NULL,
            boxLowerLeftLatitude LONG NOT NULL,
            details TEXT NOT NULL
        );
        CREATE UNIQUE INDEX IF NOT EXISTS idx_locations_long_lati 
        ON locations( boxLowerLeftLongitude, boxLowerLeftLatitude );
        ";

        public const string Regions = @"
        CREATE TABLE IF NOT EXISTS regions (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            generativeState INTEGER NOT NULL,
            boxLowerLeftLongitude LONG NOT NULL,
            boxLowerLeftLatitude LONG NOT NULL,
            details TEXT NOT NULL
        );
        CREATE UNIQUE INDEX IF NOT EXISTS idx_regions_long_lati 
        ON regions( boxLowerLeftLongitude, boxLowerLeftLatitude );
        ";

        public const string RegionRequests = @"
        CREATE TABLE IF NOT EXISTS regionrequests (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            picked INTEGER NOT NULL,
            generativeState INTEGER NOT NULL,
            requestTime DATETIME NOT NULL,
            boxLowerLeftLongitude LONG NOT NULL,
            boxLowerLeftLatitude LONG NOT NULL
        );
        CREATE UNIQUE INDEX IF NOT EXISTS idx_regionrequests_long_lati 
        ON regionrequests( boxLowerLeftLongitude, boxLowerLeftLatitude );
        ";

        public const string LocationRequests = @"
        CREATE TABLE IF NOT EXISTS locationrequests (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            picked INTEGER NOT NULL,
            generativeState INTEGER NOT NULL,
            requestTime DATETIME NOT NULL,
            boxLowerLeftLongitude LONG NOT NULL,
            boxLowerLeftLatitude LONG NOT NULL
        );
        CREATE UNIQUE INDEX IF NOT EXISTS idx_locationrequests_long_lati 
        ON locationrequests( boxLowerLeftLongitude, boxLowerLeftLatitude );
        ";

        public const string PersonRequests = @"
        CREATE TABLE IF NOT EXISTS personrequests (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            picked INTEGER NOT NULL,
            generativeState INTEGER NOT NULL,
            requestTime DATETIME NOT NULL,
            boxLowerLeftLongitude LONG NOT NULL,
            boxLowerLeftLatitude LONG NOT NULL,
            details TEXT NOT NULL
        );
        INSERT INTO personrequests ( picked, boxLowerLeftLongitude, boxLowerLeftLatitude, generativeState,requestTime, details)
            VALUES ( 0, 1, 1, 0, '2024-01-01','foo' );
        DELETE FROM personrequests; ";//insert and delete is to make sure ID AUTOINCREMENT is free for player


        public const string CharacterLocationLogs = @"
            CREATE TABLE IF NOT EXISTS characterlocationlogs (
                id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                characterId INTEGER NOT NULL,
                boxLowerLeftLongitude LONG NOT NULL,
                boxLowerLeftLatitude LONG NOT NULL,
                pointLongitude LONG NOT NULL,
                pointLatitude LONG NOT NULL,
                status INTEGER NOT NULL,
                changedAt DATETIME NOT NULL,
                firstAt DATETIME NULL,
                visitCount INTEGER NULL
            );
            CREATE UNIQUE INDEX IF NOT EXISTS idx_characterlocationlogs_charid
            ON characterlocationlogs( characterId, boxLowerLeftLongitude, boxLowerLeftLatitude );
        ";

    }
}