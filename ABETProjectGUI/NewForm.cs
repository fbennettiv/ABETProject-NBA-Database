using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABETProjectGUI
{
    public partial class NewForm : Form
    {
        const int KeyLength = 2;
        string PlayerKey;
        public static bool OK = false;

        public NewForm()
        {
            InitializeComponent();
        }

        /***********************************************************************/
        /* Method to determine if a string is an integer.                      */
        /*                                                                     */
        /* Input:	A string (s)                                               */
        /*                                                                     */
        /* Returns: True or False.                                             */
        /***********************************************************************/
        private bool IsInteger(string s)
        {
            int i;

            for (i = 0; i < s.Length; i++)
                if (s[i] < '0' || s[i] > '9') return false;

            return true;
        }

        private void okButton_Click_1(object sender, EventArgs e)
        {
            if (!IsInteger(playerNumTextBox.Text))
            {
                MessageBox.Show("Player # must be an integer");
                playerNumTextBox.Focus();
                playerNumTextBox.SelectAll();
            }
            else if (playerNumTextBox.Text.Length > KeyLength)
            {
                MessageBox.Show("Player # cannot be longer than " + KeyLength + " digits");
                playerNumTextBox.Focus();
                playerNumTextBox.Select();
            }
            else
            {
                //PlayerKey = playerNumTextBox.Text.PadLeft(KeyLength);
                PlayerKey = playerNumTextBox.Text;
                if (NBADatabase.players.RecordExists(NBADatabase.players.tableName, NBADatabase.players.keyName, PlayerKey))
                {
                    MessageBox.Show("Player #  " + PlayerKey + " already exists");
                    playerNumTextBox.Focus();
                    playerNumTextBox.SelectAll();
                }
                else
                {
                    NBADatabase.KeyText.Text = PlayerKey;
                    OK = true;
                    Close();
                }
            }
        }

        private void cancelButton_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void NewForm_Load_1(object sender, EventArgs e)
        {
            SetupPlayerListBox();
        }

        /************************************************************************/
        /* Method to Setup the DriverListBox.                                   */
        /*                                                                      */
        /* Globals:                                                             */
        /*    Input:	The Players object (players)                            */
        /*    Output:	The Player ListBox (PlayerListBox)                      */
        /************************************************************************/
        void SetupPlayerListBox()
        {
            int i, n;
            string[][] Players;
            string item;

            try
            {
                Players = NBADatabase.players.GetCurrentPlayers();
                for (i = 0; i < Players.Length; i++)
                {
                    item = String.Empty;

                    for (n = 0; n < Players[i].Length; n++)
                    {
                        item += Players[i][n];
                        if (n < Players[i].Length - 1) item += ' ';
                    }
                    playerListBox.Items.Add(item);
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message + " SetupPlayerListBox"); }
        }

        private void playerNumTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void playerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

