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
    public partial class DisplayForm : Form
    {
        static int[] ColWidths = { 65, 100, 100, 35, 100, 100, 35, 35, 35, 35, 35, 35, 35, 40 };
        static string[][] thePlayers;
        static string[] TableData = new string[LibraryClass.PlayerHeaders.Length];

        public DisplayForm()
        {
            InitializeComponent();
        }

        private void DisplayForm_Load_1(object sender, EventArgs e)
        {
            int i, k;

            try
            {
                thePlayers = NBADatabase.players.GetAllPlayers();
                dataGridView.ColumnCount = LibraryClass.PlayerHeaders.Length;
                for (i = 0; i < LibraryClass.PlayerHeaders.Length; i++)
                {
                    dataGridView.Columns[i].Name = LibraryClass.PlayerHeaders[i];
                    dataGridView.Columns[i].Width = ColWidths[i];
                }

                for (i = 0; i < thePlayers.Length; i++)
                {
                    for (k = 0; k < thePlayers[i].Length - 1; k++)
                        TableData[k] = thePlayers[i][k];
                    TableData[k] = thePlayers[i][k];

                    dataGridView.Rows.Add(TableData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void exitButton_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
