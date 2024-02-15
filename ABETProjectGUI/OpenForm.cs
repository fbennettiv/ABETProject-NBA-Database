using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ABETProjectGUI
{
    public partial class OpenForm : Form
    {
        const int KeyLength = 2;
        string PlayerKey;

        public OpenForm()
        {
            InitializeComponent();
        }

        private void cancelButton_Click_1(object sender, EventArgs e)

        {
            NBADatabase.OpenDriverAccepted = false;
            Close();
        }

        private void okButton_Click_1(object sender, EventArgs e)
        {
            int KeyLen;

            if (playerNumComboBox.Text.Length < KeyLength)
                KeyLen = playerNumComboBox.Text.Length;
            else
                KeyLen = KeyLength;

            NBADatabase.OpenDriverAccepted = true;

            Debug.WriteLine("fred playerNumComboBox.Text = " + playerNumComboBox.Text);

            PlayerKey = playerNumComboBox.Text.Substring(0, KeyLen);
            Debug.WriteLine("fred PlayerKey = " + PlayerKey);
            Debug.WriteLine("fred PlayerKey.Length = " + PlayerKey.Length);
            // PlayerKey = PlayerKey.Trim().PadLeft(KeyLength);
            PlayerKey = PlayerKey.Trim();
            Debug.WriteLine("fred PlayerKey.Length = " + PlayerKey.Length);
            Debug.WriteLine("fred PlayerKey = " + PlayerKey);

            NBADatabase.KeyText.Text = PlayerKey;

            NBADatabase.OpenRecord(PlayerKey);

            Close();
        }

        private void OpenForm_Load_1(object sender, EventArgs e)
        {
            SetupPlayerDropDown();
        }

        /***********************************************************************/
        /* Method to Setup the Driver # ComboBox.                              */
        /*                                                                     */
        /* Globals:                                                            */
        /*    Input:  The Players object (players)                             */
        /*            The Players # ComboBox (playerNumComboBox)               */
        /***********************************************************************/
        void SetupPlayerDropDown()
        {
            int i, n;
            string[][] Players;
            string item;

            try
            {
                Players = NBADatabase.players.GetPlayers();
                for (i = 0; i < Players.Length; i++)
                {
                    item = String.Empty;
                    Debug.WriteLine("fred Players[{0}].Length = {1}", i, Players[i].Length);

                    for (n = 0; n < Players[i].Length; n++)
                    {
                        item += Players[i][n];
                        if (n < Players[i].Length - 1) item += ' ';
                    }
                    playerNumComboBox.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
