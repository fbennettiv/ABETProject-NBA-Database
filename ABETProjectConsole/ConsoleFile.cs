using System;
using System.Data.SQLite;
using System.IO;

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

class ConsoleClass
{
    static LibraryClass players; //Declaring LibraryClass object and it created in Main
    static string[] Menuitems = { "New Record", "Open Record", "Display All Records", "Exit" };
    static string[] Record; // Holds individual Player records
    static string[][] thePlayers; // Holds the Players (Master) table data
    static string[][] Teams; // Holds the Teams (support) table data
    static int LongestFieldLabel; // for display formatting
    static string PlayerNum;    // Master table primary key

    /*********************************************************************************************************************************************************************************************************/

    /*** Non Database display methods ***/

    /***********************************************************************/
    /* Method to display the program screen heading.                       */
    /***********************************************************************/
    static void heading()
    {
        Console.Clear();
        Console.Write("NBA Player Maintenance                 ");
        Console.WriteLine(DateTime.Now.ToLongDateString());
        Console.WriteLine();
    }

    /***********************************************************************/
    /* Method to display the program menu.                                 */
    /***********************************************************************/
    static void DisplayMenu()
    {
        int i;

        for (i = 0; i < Menuitems.Length; i++)
            Console.WriteLine($"{i + 1}. {Menuitems[i]}\n");
    }

    /*********************************************************************************************************************************************************************************************************/

    /*** Database Support Table methods ***/

    /***********************************************************************/
    /* Method to get a database View (an array of records where each       */
    /* record is an array of strings) of the support table.                */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The Drivers object (drivers)                            */
    /*    Output:	The Manufacturers View (Manufacturers)                  */
    /***********************************************************************/
    static void GetSupportData()
    {
        try
        {
            Teams = players.GetTeams();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }
    }

    /*********************************************************************************************************************************************************************************************************/

    /*** Database New & Open Record methods ***/

    /***********************************************************************/
    /* Method to display a record.                                         */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	An array of Field Headings (FieldHeads)                 */
    /*             The length of the longest Field Heading                 */
    /*                (LongestFieldLabel)                                  */
    /*             The Manufacturers View (Manufacturers)                  */
    /*             The record (Record)                                     */
    /***********************************************************************/
    static void DisplayRecord()
    {
        int i;

        for (i = 0; i < LibraryClass.PlayerHeaders.Length; i++)
        {
            // We use the older numbered formatting because the newer $ formatting
            // requires a constant for field display width
            Console.Write("{0}. {1," + LongestFieldLabel + "}: ", i + 1, LibraryClass.PlayerHeaders[i]);
            Console.Write(Record[i]);
            if (i == 5) // foreign key Field
            {
                Console.Write("   (");
                Console.Write(LibraryClass.FindValue(Record[i], Teams, 1));
                Console.Write(")");
            }
            Console.WriteLine();
        }
    }

    /***********************************************************************/
    /* Method to Open a record.                                            */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The Drivers object (drivers)                            */
    /*    Output:	The Record, array of strings (Record)                   */
    /*             A Driver# (DriverNo)                                    */
    /***********************************************************************/
    static void OpenRecord()
    {
        heading();  // clear the screen

        Console.WriteLine();

        Console.Write("Enter a driver #: ");
        PlayerNum = Console.ReadLine();
        //DriverNo = DriverNo.PadLeft(2); // Right justify the Driver#

        try
        {
            Record = players.GetRecord(PlayerNum);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }

        Console.WriteLine();
        if (Record[0] == "no record found") // The DLL set this if record not found
        {
            Console.WriteLine("Record for Driver " + PlayerNum + " not found");
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }
        else
        {
            DisplayRecord();
            RecordOptions(); // Display the Record Options
        }
    }

    /***********************************************************************/
    /* Method to Save a record.                                            */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The Drivers object (drivers)                            */
    /*           	The Record, array of strings (Record)                   */
    /***********************************************************************/
    static void SaveRecord()
    {
        try
        {
            players.SaveRecord(Record);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }
    }

    /***********************************************************************/
    /* Method to Delete a record.                                          */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The Drivers object (drivers)                            */
    /***********************************************************************/
    static void DeleteRecord()
    {
        try
        {
            players.DeleteRecord(PlayerNum);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }
    }

    /***********************************************************************/
    /* Method to get a New record.                                         */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	An array of Field Headings (FieldHeads)                 */
    /*    Output:	The Record, array of strings (Record)                   */
    /***********************************************************************/
    static void NewRecord()
    {
        int i;
        string value;
        bool OK;
        int number;

        heading();    // clear the screen

        Record = new string[LibraryClass.PlayerHeaders.Length];

        Console.WriteLine();

        // ask the uesr to enter a value for each field
        for (i = 0; i < LibraryClass.PlayerHeaders.Length; i++)
        {
            Console.Write($"Enter value for {LibraryClass.PlayerHeaders[i]}: ");
            value = Console.ReadLine();
            if (i == 0)
            {
                //value = value.PadLeft(2);  // Right justify the Driver#
                if (players.RecordExists(players.tableName, players.keyName, value))
                {
                    Console.WriteLine("Driver #  " + value + " already exists");
                    Console.WriteLine("Press any key to return to main menu: ");
                    Console.ReadLine();
                    return;
                }
            }
            if (i == LibraryClass.PlayerHeaders.Length - 1) // Manufacturers foreign key
            {
                OK = false;
                do
                {
                    OK = int.TryParse(value, out number);
                    if (!OK)
                    {
                        Console.Write($"Enter value for {LibraryClass.PlayerHeaders[i]} (Must be an integer): ");
                        value = Console.ReadLine();
                    }
                }
                while (!OK);
                //value = $"{number:d2}"; //Zero Fill to 2 digits
                value = $"{number}";
            }
            Record[i] = value;
        }
        Console.WriteLine();

        PlayerNum = Record[0];

        SaveRecord();
        DisplayRecord();
        RecordOptions();  // Display the Record Options
    }

    /***********************************************************************/
    /* Method to display and process the record options.                   */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	An array of Field Headings (FieldHeads)                 */
    /***********************************************************************/
    static void RecordOptions()
    {
        int choice;
        string value;
        bool OK;
        int number;

        Console.WriteLine();
        Console.WriteLine("1 = Modify record    2 = Delete record   3 = Return to main menu");
        Console.WriteLine();
        Console.Write("Enter you choice: ");
        value = Console.ReadLine();
        OK = false;
        do
        {
            OK = int.TryParse(value, out number);
            if (!OK)
            {
                Console.Write("Enter you choice (Must be an integer): ");
                value = Console.ReadLine();
            }
        }
        while (!OK);
        choice = number;

        if (choice == 1)  // Modify record
        {
            Console.WriteLine();
            Console.Write("Which field do you want to modify? ");
            value = Console.ReadLine();
            OK = false;
            do
            {
                OK = int.TryParse(value, out number);
                if (!OK)
                {
                    Console.Write("Which field do you want to modify? (Must be an integer)");
                    value = Console.ReadLine();
                }
            }
            while (!OK);
            choice = number;
            if (choice >= 1 && choice <= LibraryClass.PlayerHeaders.Length)
            {
                Console.Write($"Enter new value for field {choice}: ");
                value = Console.ReadLine();
                // Manufacturers foreign key
                if (choice == 6)
                {
                    OK = false;
                    do
                    {
                        OK = int.TryParse(value, out number);
                        if (!OK)
                        {
                            Console.Write($"Enter new value for field {choice} (Must be an integer): ");
                            value = Console.ReadLine();
                        }
                    }
                    while (!OK);
                    //value = $"{number:d2}"; //Zero Fill to 2 digits
                    value = $"{number}";
                }
                Record[choice - 1] = value;
                SaveRecord();
                DisplayRecord();
            }
        }
        else if (choice == 2) // Delete record
        {
            DeleteRecord();
        }

        // Nothing is needed for choice 3 because we return to the main loop
    }

    /*********************************************************************************************************************************************************************************************************/

    /*** Database Display All Records methods ***/

    /***********************************************************************/
    /* Method to display all master table records with the supporting data */
    /*                                                                     */
    /* Globals:                                                            */
    /*    Input:	The Drivers object (drivers)                            */
    /*             An array of Field Lengths (FieldLens)                   */
    /*             An array of Field Headings (FieldHeads)                 */
    /*             The Drivers View, an array of records where each record */
    /*             is an array of strings (theDrivers)                     */
    /***********************************************************************/
    static void DisplayAllRecords()
    {
        int i, k;

        heading();

        Console.WriteLine();

        // We use the older numbered formatting because the newer $ formatting
        // requires a constant for field display width
        for (i = 0; i < LibraryClass.PlayerHeaders.Length; i++)
            Console.Write("{0,-" + players.PlayerLength[i] + "} ", LibraryClass.PlayerHeaders[i]);
        Console.WriteLine();

        for (i = 0; i < LibraryClass.PlayerHeaders.Length; i++)
            Console.Write($"{new string('=', players.PlayerLength[i])} ");
        Console.WriteLine();

        try
        {
            thePlayers = players.GetAllPlayers();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to return to main menu: ");
            Console.ReadLine();
        }

        for (i = 0; i < thePlayers.Length; i++)
        {
            // We use the older numbered formatting because the newer $ formatting
            // requires a constant for field display width
            for (k = 0; k < thePlayers[i].Length; k++)
                Console.Write("{0,-" + players.PlayerLength[k] + "} ", thePlayers[i][k]);
            Console.WriteLine();
        }
        Console.WriteLine();

        Console.WriteLine("Press any key to return to main menu: ");
        Console.ReadLine();
    }
    static void Main(string[] args)
    {
        int choice;
        bool OK;
        int number;
        string value;

        try
        {
            players = new LibraryClass();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

        LongestFieldLabel = LibraryClass.GetLongestFieldLabel();
        GetSupportData();

        do
        {
            heading();
            DisplayMenu();

            Console.Write("Enter your choice: ");
            value = Console.ReadLine();
            OK = false;
            do
            {
                OK = int.TryParse(value, out number);
                if (!OK)
                {
                    Console.WriteLine("Enter your choice (Must be an integer): ");
                    value = Console.ReadLine();
                }
            }
            while (!OK);
            choice = number;

            switch (choice)
            {
                case 1:
                    NewRecord();
                    break;
                case 2:
                    OpenRecord();
                    break;
                case 3:
                    DisplayAllRecords();
                    break;
            }
        }
        while (choice != Menuitems.Length);  // We set the last menu item to exit
    }
}


