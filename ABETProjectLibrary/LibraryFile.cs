using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;

/*************************************************************************/
/* Program Name:     LibraryFile.cs                                      */
/* Date:             12/6/2022                                           */
/* Programmer:       Fred Bennett IV                                     */
/* Class:            CSCI 234                                            */
/*                                                                       */
/* Program Description:  Database Library for the ABET. Builds a DLL     */
/*                       (Dynamic Link Library) file to be referenced by */
/*                       the User Inerface executable projects for the   */
/*                       Console and GUI (Windows Forms Application)     */
/*                                                                       */
/* Testing Data: The NBA database NBA.db                                 */
/* Initial data:                                                         */
/*                                                                       */
/* SQLite[NBA]> select * from Player;                                    */
/* +-----------+-----------+----------+-----+----------+---------+-----+-----+------+-----+-----+-----+-----+-------+ */
/* | PlayerNum | FirstName | LastName | Age | Position | Jersey# | Pts | FG% | 3PT% | Reb | Ast | Stl | Blk | Team# | */
/* +-----------+-----------+----------+-----+----------+---------+-----+-----+------+-----+-----+-----+-----+-------+ */
/* |  1        | Ja        | Morant   | 23  | PG       | 12      | 28  | 46  | 37   | 06  | 07  | 01  | 00  | 01    | */
/* |  2        | Desmond   | Bane     | 24  | SF       | 22      | 24  | 46  | 45   | 04  | 04  | 00  | 00  | 01    | */
/* +-----------+-----------+----------+-----+----------+---------+-----+-----+------+-----+-----+-----+-----+-------+ */
/* 14 rows in set(0.001 sec)                                             */
/*                                                                       */
/* SQLite[NBA]> select * from Team;                                      */
/* +---------+-----------------------+---------------+--------------+----------------+----------------+-----------+ */
/* | TeamNum |        TeamName       |      City     |  ArenaName   | Owner          |    HeadCoach   |   Mascot  | */
/* +---------+-----------------------+---------------+--------------+----------------+----------------+-----------+ */
/* | 01      | Memphis Grizzlies     | Memphis       | FedExForum   | Robert J. Pera | Taylor Jenkins | Grizz     | */
/* | 02      | Golden State Warriors | San Francisco | Chase Center | Peter Guber    | Steve Kerr     | Berserker | */
/* +---------+-----------------------+---------------+--------------+----------------+----------------+-----------+ */
/* 7 rows in set(0.000 sec)                                              */
/* See Define.txt for database setup                                     */
/*************************************************************************/

public class LibraryClass
{
    static string connectString;
    static SQLiteConnection connection = null; // null -> not open
    static SQLiteDataReader reader = null;
    static SQLiteCommand sqlCmd;

    // For Display
    static string[] TeamHeaders = { "TeamNum", "TeamName", "City", "ArenaName", "Owner", "HeadCoach", "Mascot" };
    static int[] TeamLength = { 5, 25, 20, 50, 30, 30, 30 };
    public static string[] PlayerHeaders = { "PlayerNum", "FirstName", "LastName", "Age", "Position", "Jersey#", "Pts",
                                             "FG%", "3PT%", "Reb", "Ast", "Stl", "Blk", "Team#" };
    public int[] PlayerLength = { 9, 20, 20, 3, 20, 7, 5, 3, 3, 4, 3, 3, 3, 3};

    //Actual Database Header
    public static string[] PlayerNames = { "PlayerNum", "FirstName", "LastName", "Age", "Position", "JerseyNum", "Points",
                        "FieldGoalPercentage", "ThreePointPercentage", "Rebounds", "Assists", "Steals", "Blocks", "TeamNum"};

    // For SQL Statements
    static int[] TDataTypes = { 0, 1, 1, 1, 1, 1, 1 }; //Tells whether it should put quotes for the library to read
    static int[] PDataTypes = { 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0}; // 0 -> No quotes, 1 -> Quotes needed
    public string keyName = PlayerNames[0];
    public string tableName = "Player";

    //Constructor to connect to library
    public LibraryClass()
    {
        DBConnect();
    }
    //Destructor to close library
    ~LibraryClass()
    {
        DBClose();
    }

    /*********************************************************************************************************************************************************************************************************/

    /*** Database Connection methods ***/

    /*************************************/
    /* Method to Connect to the database */
    /* Global input: connectString       */
    /* Global output: connection         */
    /*************************************/
    static void DBConnect()
    {
        connectString = "Data Source=NBA.db";

        try
        {
            connection = new SQLiteConnection(connectString);
            connection.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }
    }

    /*************************************/
    /* Method to Close the database      */
    /* Global input: connection          */
    /*************************************/
    static void DBClose()
    {
        if (connection != null) connection.Close();
    }

    /*********************************************************************************************************************************************************************************************************/

    /*** Database collection methods ***/

    /***********************************************************************/
    /* Method to get a database View (an array of records where each       */
    /* record is an array of strings). Used to get any View of the         */
    /* database depending on the SQL SELECT Command.                       */
    /*                                                                     */
    /* Input:	A SQL SELECT Command (commandString) to create the View    */
    /*                                                                     */
    /* Globals: A DataReader (reader)                                      */
    /*          A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*                                                                     */
    /* Returns: The View (Array of array of strings)                       */
    /***********************************************************************/
    private string[][] GetData(string commandString)
    {
        string[][] data = null;
        int col;
        int row = 0;
        int records;

        Debug.Print("fred commandString = " + commandString);
        reader = null;

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            // Make one pass over the data to count the records
            // to know what size to make the array
            records = 0;
            reader = sqlCmd.ExecuteReader();
            while (reader.Read()) records++;

            reader.Close();

            // Allocate the array of records
            data = new string[records][];

            // Make another pass over the data to fill the arrays
            reader = sqlCmd.ExecuteReader();
            while (reader.Read())
            {
                // Allocate each record in the array of records
                data[row] = new string[reader.FieldCount];
                for (col = 0; col < reader.FieldCount; col++)
                    data[row][col] = reader.GetValue(col).ToString();
                row++;
            }
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in GetData");
        }

        finally
        {
            if (reader != null) reader.Close();
        }

        return data;
    }

    /***********************************************************************/
    /* Method to get the All Drivers View (an array of records where each  */
    /* record is an array of strings). Uses a join of the Drivers and      */
    /* Manufacturers tables.                                               */
    /*                                                                     */
    /* Input:	None                                                       */
    /*                                                                     */
    /* Returns: The All Drivers View for the Display All Records User      */
    /*          Interface.                                                  */
    /***********************************************************************/
    public string[][] GetAllPlayers()
    {
        string[][] data = null;
        string commandString;

        // Note: This is an inner join so drivers without matching
        //       Manufacturers will not be included. To include all
        //       drivers, even when they do not include a matching
        //       Manufacturer we would need to use a left outer join.
        commandString = "select PlayerNum,FirstName,LastName,Age,";
        commandString += "Position,JerseyNum,Player.TeamNum,Points,FieldGoalPercentage,ThreePointPercentage,";
        commandString += "Rebounds,Assists,Steals,Blocks from Player,Team";
        commandString += " where Player.TeamNum = Team.TeamNum";
        commandString += " order by Player.TeamNum";

        //Console.WriteLine(commandString);

        data = GetData(commandString);

        return data;
    }

    /***********************************************************************/
    /* Method to get the Manufacturers View (an array of records where each*/
    /* record is an array of strings).                                     */
    /*                                                                     */
    /* Input:	None                                                       */
    /*                                                                     */
    /* Returns: The Manufacturers View                                     */
    /***********************************************************************/
    public string[][] GetTeams()
    {
        string[][] data = null;
        string commandString;

        commandString = "select * from Team";

        data = GetData(commandString);

        return data;
    }

    /***********************************************************************/
    /* Method to get a single record (array of strings) from the database. */
    /*                                                                     */
    /* Input:	The table (tableName)                                      */
    /*          The Name of the primary key of the table (keyName)         */
    /*          The key's value (key)                                      */
    /*                                                                     */
    /* Globals: A DataReader (reader)                                      */
    /*          A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*                                                                     */
    /* Returns: The record.                                                */
    /***********************************************************************/
    public string[] GetARecord(string tableName, string keyName, string key)
    {
        string commandString;
        string[] Record = null;
        int col;

        commandString = "select * from " + tableName;
        commandString += " where " + keyName + " = ";
        commandString += "'";
        commandString += key;
        commandString += "'";

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            reader = sqlCmd.ExecuteReader();
            if (reader.Read())
            {
                Record = new string[reader.FieldCount];
                for (col = 0; col < reader.FieldCount; col++)
                    Record[col] = reader.GetValue(col).ToString();
            }
            else
            {
                Record = new string[1];
                Record[0] = "no record found";
            }
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in GetARecord");
        }

        finally
        {
            if (reader != null) reader.Close();
        }

        return Record;
    }

    /***********************************************************************/
    /* Method to get a single record (array of strings) from the database  */
    /* using global names.                                                 */
    /*                                                                     */
    /* Input:	The primary key's value (key)                              */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The table (tableName)                                   */
    /*             The Name of the primary key of the table (keyName)      */
    /*                                                                     */
    /* Returns: The record.                                                */
    /***********************************************************************/
    public string[] GetRecord(string key)
    {
        return GetARecord(tableName, keyName, key);
    }

    /***********************************************************************/
    /* Method to delete a single record from the database.                 */
    /*                                                                     */
    /* Input:	The table (tableName)                                      */
    /*          The Name of the primary key of the table (keyName)         */
    /*          The  key's value (key)                                     */
    /*                                                                     */
    /* Globals: A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*                                                                     */
    /* Returns: Nothing.                                                   */
    /***********************************************************************/
    public void DeleteARecord(string tableName, string keyName, string key)
    {
        string commandString;

        commandString = "delete from " + tableName;
        commandString += " where " + keyName + " = ";
        commandString += "'";
        commandString += key;
        commandString += "'";

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            sqlCmd.ExecuteNonQuery();
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in DeleteARecord");
        }
    }

    /***********************************************************************/
    /* Method to delete a single record from the database                  */
    /* using global names.                                                 */
    /*                                                                     */
    /* Input:	The primary key's value (key)                              */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The table (tableName)                                   */
    /*             The Name of the primary key of the table (keyName)      */
    /*                                                                     */
    /* Returns: Nothing.                                                   */
    /***********************************************************************/
    public void DeleteRecord(string key)
    {
        DeleteARecord(tableName, keyName, key);
    }

    /***********************************************************************/
    /* Method to determine if a record exists in the database.             */
    /*                                                                     */
    /* Input:	The table (tableName)                                      */
    /*          The Name of the primary key of the table (keyName)         */
    /*          The key's value (key)                                      */
    /*                                                                     */
    /* Globals: A DataReader (reader)                                      */
    /*          A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*                                                                     */
    /* Returns: True or False.                                             */
    /***********************************************************************/
    public bool RecordExists(string tableName, string keyName, string key)
    {
        string commandString;
        bool Exists = false;

        commandString = "select * from " + tableName;
        commandString += " where " + keyName + " = ";
        commandString += "'"; commandString += key;
        commandString += "'";

        Debug.Print(" RecordExists commandString = " + commandString);

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            reader = sqlCmd.ExecuteReader();
            Exists = reader.Read();
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in RecordExists");
        }

        finally
        {
            if (reader != null) reader.Close();
        }

        return Exists;
    }

    /***********************************************************************/
    /* Method to insert a single record into the database.                 */
    /*                                                                     */
    /* Input:	The table (tableName)                                      */
    /*          The record (array of strings)                              */
    /*                                                                     */
    /* Globals: A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*             An array of Data Types (DataTypes)                      */
    /*             0 -> No quotes, 1 -> Quotes needed                      */
    /*                                                                     */
    /* Returns: Nothing.                                                   */
    /***********************************************************************/
    void InsertRecord(string tableName, string[] Record)
    {
        string commandString;
        int i;

        commandString = "insert into " + tableName + " values (";
        for (i = 0; i < Record.Length; i++)
        {
            if (PDataTypes[i] == 1) commandString += "'";
            commandString += Record[i];
            if (PDataTypes[i] == 1) commandString += "'";
            if (i < Record.Length - 1) commandString += ",";
        }
        commandString += ")";

        Debug.Print("InsertRecord commandString = " + commandString);

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            sqlCmd.ExecuteNonQuery();
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in InsertRecord");
        }
    }

    /***********************************************************************/
    /* Method to update a single record into the database.                 */
    /*                                                                     */
    /* Input:	The table (tableName)                                      */
    /*          The record (array of strings)                              */
    /*                                                                     */
    /* Globals: A Command (sqlCmd)                                         */
    /*    Input:	A Connection (connection)                               */
    /*             An array of Data Types (DataTypes)                      */
    /*             0 -> No quotes, 1 -> Quotes needed                      */
    /*             An array of Field Names (FieldNames)                    */
    /*                                                                     */
    /* Returns: Nothing.                                                   */
    /*                                                                     */
    /* for simplicity we do not track which fields have changed but update */
    /* all fields                                                          */
    /***********************************************************************/
    public void UpdateRecord(string tableName, string keyName, string[] Record)
    {
        string commandString;
        string key;
        int i;

        key = Record[0];

        commandString = "update " + tableName + " set ";
        for (i = 0; i < Record.Length; i++)
        {
            commandString += PlayerHeaders[i];
            commandString += " = '";
            commandString += Record[i];
            commandString += "'";
            if (i < Record.Length - 1) commandString += ",";
        }
        commandString += " where " + keyName + " = '";
        commandString += key;
        commandString += "'";

        Debug.Print("UpdateRecord commandString = " + commandString);

        try
        {
            sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = commandString;
            sqlCmd.Connection = connection;

            sqlCmd.ExecuteNonQuery();
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\nOccurred in UpdateRecord");
        }
    }


    /***********************************************************************/
    /* Method to Save a single record to the database.                     */
    /* We assume the first field in the record is the primary key.         */
    /*                                                                     */
    /* Input:	The record (array of strings)                              */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The table (tableName)                                   */
    /*             The Name of the primary key of the table (keyName)      */
    /*                                                                     */
    /* Returns: Nothing.                                                   */
    /***********************************************************************/
    public void SaveRecord(string[] Record)
    {
        string key;

        key = Record[0];
        if (RecordExists(tableName, keyName, key))
            UpdateRecord(tableName, keyName, Record);
        else
            InsertRecord(tableName, Record);
    }

    /***********************************************************************/
   /* Method to get a View of the Drivers #, First Name and Last Name     */
   /* (an array of records where each record is an array of strings)      */
   /* ordered by Driver #.                                                */
   /* Used by the GUI new record list box in the dialog to see existing   */
   /* driver #s.                                                          */
   /*                                                                     */
   /* Input:	None                                                       */
   /*                                                                     */
   /* Returns: The View                                                   */
   /***********************************************************************/
   public string[][] GetCurrentPlayers()
   {
      string[][] data = null;
      string commandString;

      commandString = "select PlayerNum,FirstName,LastName from Player";
      commandString += " order by PlayerNum";

      data = GetData(commandString);

      return data;
   }

    /***********************************************************************/
    /* Method to get a View of the Drivers #, First Name and Last Name     */
    /* (an array of records where each record is an array of strings)      */
    /* ordered by Last Name then First Name.                               */
    /* Used by the GUI open record drop down in the dialog box to pick a   */
    /* driver.                                                             */
    /*                                                                     */
    /* Input:	None                                                       */
    /*                                                                     */
    /* Returns: The View                                                   */
    /***********************************************************************/
    public string[][] GetPlayers()
    {
        string[][] data = null;
        string commandString;

        commandString = "select PlayerNum,FirstName,LastName from Player";
        commandString += " order by PlayerNum";

        data = GetData(commandString);

        return data;
    }

    /*********************************************************************************************************************************************************************************************************/

    /*** Database Utility methods ***/

    /***********************************************************************/
    /* Method to find the Longest Field Label                              */
    /* Used by the console user interface for dispaly.                     */
    /*                                                                     */
    /* Input:	None                                                       */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	An array of Field Headings (FieldHeads)                 */
    /*                                                                     */
    /* Returns: Longest Field Label Length                                 */
    /***********************************************************************/
    public static int GetLongestFieldLabel()
    {
        int i;
        int Longest = 0;

        for (i = 0; i < PlayerHeaders.Length; i++)
            if (PlayerHeaders[i].Length > Longest)
                Longest = PlayerHeaders[i].Length;

        return Longest;
    }

    /***********************************************************************/
    /* Method to Find a Value in the support table given the support table */
    /* primary key (from the Master table foreign key).                    */
    /*                                                                     */
    /* Input:	the support table primary key (TheKey)                     */
    /*          the support table data (values)                            */
    /*          the index (location) of the support table                  */
    /*              field (Value) we are looking for (index)               */
    /*                                                                     */
    /* Returns: The Value or "Not Found"                                   */
    /***********************************************************************/
    public static string FindValue(string TheKey, string[][] values, int index)
    {
        int i;
        string key;
        string value;

        for (i = 0; i < values.Length; i++)
        {
            key = values[i][0];
            value = values[i][index];
            if (TheKey == key) return value;
        }

        return "Not Found";
    }
}

