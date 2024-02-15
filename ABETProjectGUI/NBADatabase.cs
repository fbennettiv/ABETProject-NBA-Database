using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
/* +-----------+-----------+----------+-----+----------+---------+-------+-----+-----+------+-----+-----+-----+-----+ */
/* | PlayerNum | FirstName | LastName | Age | Position | Jersey# | Team# | Pts | FG% | 3PT% | Reb | Ast | Stl | Blk | */
/* +-----------+-----------+----------+-----+----------+---------+-------+-----+-----+------+-----+-----+-----+-----+ */
/* |  1        | Ja        | Morant   | 23  | PG       | 12      | 01    | 28  | 46  | 37   | 06  | 07  | 01  | 00  | */
/* |  2        | Desmond   | Bane     | 24  | SF       | 22      | 01    | 24  | 46  | 45   | 04  | 04  | 00  | 00  | */
/* +-----------+-----------+----------+-----+----------+---------+-------+-----+-----+------+-----+-----+-----+-----+ */
/* 14 rows in set(0.001 sec)                                              */
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


namespace ABETProjectGUI
{
    public partial class NBADatabase : Form
    {
        public static LibraryClass players; //Declare the LibraryClass object
        static int FontWidth;
        static int LeftMargin = 6;
        static int TopMargin = 35;
        static int ExtraWidth = 120;
        int FieldTop;
        int LongestFieldLabel; //For formatting the display
        static int VerticalKeySep = 10;
        static int VerticalFieldSep = 10;
        static int VerticalFieldPad = 6;
        Graphics g;
        Label KeyLabel = new Label();
        public static Label KeyText = new Label();
        public static bool OpenDriverAccepted;
        static string[] Record; // Holds individual Player records
        static string[] CleanRecord; // Holds a Clean version of the player records (After a save)
        static string[][] Teams; // Holds the Team support table data
        static int NumCombo = 1; // Number of foriegn key fields
        static int NumText; // Number of non foriegn key fields
        public static TextBox[] FieldText; // Array of TextBoxes for non foreign key fields
        public static ComboBox[] FieldCombo; // Array of ComboBoxes for foreign key fields
        Label[] FieldLabels; // Array of Labels for Label fields
        int[] FieldSize = { 10, 10, 10, 3, 10, 3, 3, 3, 3, 3, 3, 3, 3 }; // Array of non key field sizes
        static bool RecordLoaded = false;

        // Constructor
        public NBADatabase()
        {
            int i;

            Debug.Print("elmo Before InitializeComponent");
            InitializeComponent();
            Debug.Print("fred after InitializeComponent");
            try
            {
                players = new LibraryClass();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(1);
            }


            NumText = LibraryClass.PlayerHeaders.Length - NumCombo;

            // This section creates the GUI controls dynamically
            // to improve the reusability of the code
            // You may create them statically with drag and drop
            // if you prefer
            FieldLabels = new Label[LibraryClass.PlayerHeaders.Length];
            FieldText = new TextBox[NumText];
            FieldCombo = new ComboBox[NumCombo];
            LongestFieldLabel = LibraryClass.GetLongestFieldLabel();

            SuspendLayout();

            // Font set in properties window
            g = CreateGraphics();
            FontHeight = (int)(Font.SizeInPoints / 72 * g.DpiX);
            FontWidth = (FontHeight * 8) / 10;

            FieldTop = TopMargin + FontHeight + VerticalFieldPad + VerticalFieldSep + VerticalKeySep;
            
            KeyLabel = new Label();
            KeyLabel.AutoSize = false;
            KeyLabel.Location = new Point(LeftMargin, TopMargin);
            KeyLabel.Name = "KeyLabel";
            KeyLabel.Text = "Player #";
            KeyLabel.Size = new Size(FontWidth * KeyLabel.Text.Length + 2, FontHeight + VerticalFieldPad);
            Controls.Add(KeyLabel);

            KeyText = new Label();
            KeyLabel.AutoSize = false;
            KeyLabel.Location = new Point(LeftMargin, TopMargin);
            KeyText.Name = "KeyText";
            KeyText.Text = String.Empty;
            KeyText.Size = new Size(FontWidth * KeyLabel.Text.Length + 2, FontHeight + VerticalFieldPad);
            KeyText.BorderStyle = BorderStyle.Fixed3D;
            KeyText.BackColor = Color.White;
            Controls.Add(KeyText);


            // We are starting a 1 instead of 0 because the
            // first control is done separately with KeyLabel and KeyText
            for (i = 1; i < LibraryClass.PlayerHeaders.Length; i++)
            {
                FieldLabels[i] = new Label();
                FieldLabels[i].AutoSize = false;
                FieldLabels[i].Location = new Point(LeftMargin, FieldTop + i * (FontHeight + VerticalFieldPad + VerticalFieldSep));
                FieldLabels[i].Name = "FieldLabel" + i;
                FieldLabels[i].Text = LibraryClass.PlayerHeaders[i];
                FieldLabels[i].Size = new Size(FontWidth * LongestFieldLabel + 2, FontHeight + VerticalFieldPad);
                FieldLabels[i].TextAlign = ContentAlignment.MiddleLeft;
                Controls.Add(FieldLabels[i]);

                // We are assuming the foreign key Fields are 
                // at the end of the record
                if (i < NumText)
                {
                    FieldText[i] = new TextBox();
                    FieldText[i].AutoSize = false;
                    FieldText[i].MinimumSize = new System.Drawing.Size(0, 0);
                    FieldText[i].Location = new Point(FieldLabels[i].Location.X + FieldLabels[i].Size.Width + 5, FieldTop + i * (FontHeight + VerticalFieldPad + VerticalFieldSep));
                    FieldText[i].Name = "FieldText" + i;
                    FieldText[i].Text = String.Empty;
                    FieldText[i].Size = new Size((FontWidth * FieldSize[i]) + ExtraWidth, FontHeight + VerticalFieldPad);
                    Controls.Add(FieldText[i]);
                }
                else
                {
                    FieldCombo[i - NumText] = new ComboBox();
                    FieldCombo[i - NumText].AutoSize = false;
                    FieldCombo[i - NumText].MinimumSize = new System.Drawing.Size(0, 0);
                    FieldCombo[i - NumText].Location = new Point(FieldLabels[i].Location.X + FieldLabels[i].Size.Width + 5, FieldTop + i * (FontHeight + VerticalFieldPad + VerticalFieldSep));

                    FieldCombo[i - NumText].Name = "FieldCombo" + i;
                    FieldCombo[i - NumText].Text = String.Empty;
                    FieldCombo[i - NumText].Size = new Size(FontWidth * FieldSize[i - NumText] + ExtraWidth, FontHeight + VerticalFieldPad);
                    Controls.Add(FieldCombo[i - NumText]);
                }
            }

            ResumeLayout(false);

            SetupTeamDropDown();
        }

        private void displayAllMenuItem_Click(object sender, EventArgs e)
        {
            DisplayForm objDisplayForm = new DisplayForm(); //This is for another gui

            objDisplayForm.ShowDialog();
        }

        private void openFileMenuItem_Click(object sender, EventArgs e)
        {
            OpenForm objOpenForm = new OpenForm();

            if (RecordLoaded) ResetRecord();
            CheckRecordChanged();

            objOpenForm.ShowDialog();
        }

        private void exitFileMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutHelpMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("\tNBA Data Maintenance\n\n\nAuthor: Fred Bennett IV\n\nData Made: 12/10/22");
        }

        private void saveFileMenuItem_Click_1(object sender, EventArgs e)
        {
            SaveRecord();
        }

        private void newFileMenuItem_Click(object sender, EventArgs e)
        {
            NewForm objNewForm = new NewForm();

            if (RecordLoaded) ResetRecord();
            CheckRecordChanged();

            objNewForm.ShowDialog();

            if (NewForm.OK)
            {
                Record = new string[LibraryClass.PlayerHeaders.Length];
                Record[0] = KeyText.Text;
                ClearDataEntry();
                ResetRecord();
                RecordLoaded = true;
                Copy2CleanRecord();
            }
        }

        private void deleteFileMenuItem_Click(object sender, EventArgs e)
        {
            string key;

            if (RecordLoaded)
            {
                key = Record[0];

                try
                {
                    players.DeleteRecord(key);
                    RecordLoaded = false;
                    KeyText.Text = String.Empty;
                    ClearDataEntry();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " deleteFileMenuItem_Click");
                }
            }
            else
                MessageBox.Show("No record loaded to delete");
        }

        private void undeleteFileMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void purgeFileMenuItem_Click(object sender, EventArgs e)
        {

        }

        public static void OpenRecord(string key)
        {
            int i;

            try
            {
                Debug.Print("fred RecordLoaded = " + RecordLoaded);
                Record = players.GetRecord(key);
                for (i = 0; i < Record.Length; i++)
                    Debug.Print("fred Record[" + i + "] = " + Record[i]);
                if (Record[0] == "no record found")
                {
                    KeyText.Text = String.Empty;
                    ClearDataEntry();
                    RecordLoaded = false;
                    MessageBox.Show("Record for Player " + key + " not found");
                }
                else
                {
                    // We are starting a 1 instead of 0 because the
                    // first control is done separately with KeyLabel and KeyText
                    for (i = 1; i < LibraryClass.PlayerHeaders.Length; i++)
                    {
                        // We are assuming the foreign key Fields are 
                        // at the end of the record
                        if (i < NumText) //Text Field
                            FieldText[i].Text = Record[i];
                        else
                        {
                            Debug.Print("fred i = " + i);
                            Debug.Print("fred NumText = " + NumText);
                            Debug.Print("fred FieldCombo.Length = " + FieldCombo.Length);
                            Debug.Print("fred Record.Length = " + Record.Length);
                            FieldCombo[i - NumText].Text = LibraryClass.FindValue(Record[i], Teams, 1);
                            Debug.Print("fred Record[" + i + "] = " + Record[i]);
                            Debug.Print("fred FieldCombo[" + (i - NumText) + "].Text = " + FieldCombo[i - NumText].Text);
                        }
                    }
                    RecordLoaded = true;
                    Copy2CleanRecord();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " OpenRecord");
            }
        }

        /***********************************************************************/
        /* Method to Save (Copy) the contents of the record into the           */
        /* CleanRecord after a Save                                            */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	The Record, array of strings (Record)                  */
        /*    Output:	The Clean Record, array of strings (CleanRecord)       */
        /***********************************************************************/
        private static void Copy2CleanRecord()
        {
            int i;

            CleanRecord = new string[Record.Length];
            for (i = 0; i < Record.Length; i++)
                CleanRecord[i] = Record[i];
        }

        /***********************************************************************/
        /* Method to fill the Drop Down list for the Manufacturers             */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:   The Teams View (Teams)                                  */
        /*             The Players object (players)                            */
        /*             An array of Field ComboBoxes (FieldCombo) for           */
        /*                foreign key fields                                   */
        /***********************************************************************/
        void SetupTeamDropDown()
        {
            int i;

            try
            {
                Teams = players.GetTeams();

                for (i = 0; i < Teams.Length; i++)
                    FieldCombo[0].Items.Add(Teams[i][1]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " SetupTeamDropDown");
            }
        }

        /***********************************************************************/
        /* Method to Copy the contents of the GUI fields to the Record in      */
        /* memory so they can be saved to the database                         */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	Number of non foreign key fields (NumText)             */
        /*             An array of Field ComboBoxes (FieldCombo) for           */
        /*                foreign key fields                                   */
        /*             Array of TextBoxes for non foreign key fields(FieldText)*/
        /*    Output:	The Record, array of strings (Record)                  */
        /***********************************************************************/
        private static void ResetRecord()
        {
            int i;
            int index;

            Debug.Print("fred NumText = " + NumText);

            // We are starting a 1 instead of 0 because the
            // first control is done separately with KeyLabel and KeyText
            for (i = 1; i < LibraryClass.PlayerHeaders.Length; i++)
            {
                if (i < NumText)
                    Record[i] = FieldText[i].Text;
                else
                {
                    try
                    {
                        index = FieldCombo[i - NumText].SelectedIndex;
                        Record[i] = Teams[index][0];
                        Debug.Print("fred ResetRecord Record[" + i + "] = " + Record[i]);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("fred ResetRecord Exception");
                        Debug.Print("fred " + ex.Message);
                        Debug.Print("fred i = " + i);
                        Debug.Print("fred players.PlayerHeaders.Length = " + LibraryClass.PlayerHeaders.Length);
                        Debug.Print("fred Record.Length = " + Record.Length);

                        Record[i] = String.Empty;
                    }
                }
            }
        }

        /***********************************************************************/
        /* Method to check if the contents of the record are dirty             */
        /* i.e. has any thing changed since the last save                      */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	The Record, array of strings (Record)                  */
        /*             The Clean Record, array of strings (CleanRecord)        */
        /***********************************************************************/
        private static bool RecordChanged()
        {
            int i;

            for (i = 0; i < Record.Length; i++)
                if (CleanRecord[i] != Record[i]) return true;

            return false;
        }

        /***********************************************************************/
        /* Method to check if the contents of the record are dirty             */
        /* i.e. has any changed since the last save.                           */
        /* If it is dirty alert the user and give them an opportunity to       */
        /* save it.                                                            */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	Boolean to track when a record is loaded (RecordLoaded)*/
        /***********************************************************************/
        private static void CheckRecordChanged()
        {
            DialogResult result;
            
            if (RecordLoaded && RecordChanged())
            {
                result = MessageBox.Show("Record changed. Do you want to save these changes?",
                    "NBA Maintenance", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    SaveRecord();
                }
            }
        }

        /***********************************************************************/
        /* Method to save a loaded record to the database                      */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	Boolean to track when a record is loaded (RecordLoaded)*/
        /*             The Drivers object (drivers)                            */
        /*    Output:	The Record, array of strings (Record)                  */
        /***********************************************************************/
        private static void SaveRecord()
        {
            if (RecordLoaded)
            {
                ResetRecord();
                Record[0] = KeyText.Text;

                if (RecordChanged())
                {
                    try
                    {
                        players.SaveRecord(Record);
                        Copy2CleanRecord();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "SaveRecord");
                    }
                }
                else
                    Copy2CleanRecord();
            }
            else
                MessageBox.Show("No record loaded to save");
        }

        /***********************************************************************/
        /* Method to Clear the Data on the screen                              */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:	Number of non foreign key fields (NumText)              */
        /*             An array of Field ComboBoxes (FieldCombo) for           */
        /*                foreign key fields                                   */
        /*             Array of TextBoxes for non foreign key fields(FieldText)*/
        /*             An array of Field Headings (FieldHeads)                 */
        /***********************************************************************/
        private static void ClearDataEntry()
        {
            int i;

            // We are starting a 1 instead of 0 because the
            // first control is done separately with KeyLabel and KeyText
            for (i = 1; i < LibraryClass.PlayerHeaders.Length; i++)
            {
                if (i < NumText)
                    FieldText[i].Text = String.Empty;
                else
                    FieldCombo[i - NumText].SelectedIndex = -1; //No selection
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
