using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompareISO
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        public Form3(List<String> addedFiles, List<String> removedFiles)
        {
            InitializeComponent();
            // append the added header first
            try
            {
                if (addedFiles.Any())
                {
                    headerText("Added");
                    foreach (String file in addedFiles)
                    {
                        String[] entry = file.Split(',');
                        tableRichTextBox.AppendText("\n|-\n|" + entry[0] + "\n|" + entry[1] + "\n|" + entry[2]);
                    }
                    endText();
                    tableRichTextBox.AppendText("\n");
                }
                if (removedFiles.Any())
                {
                    headerText("Removed");
                    foreach (String file in removedFiles)
                    {
                        String[] entry = file.Split(',');
                        tableRichTextBox.AppendText("\n|-\n|" + entry[0] + "\n|" + entry[1] + "\n|" + entry[2]);
                    }
                    endText();
                }
                tableRichTextBox.SelectionStart = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred trying to render the table. " + ex.Message);
                this.Close();
                return;
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void headerText(String text)
        {
            tableRichTextBox.AppendText("{| class=\"wikitable\"\r\n<caption>" + text + " files</caption>\r\n!Name\r\n!Description\r\n!Version");
        }

        private void endText()
        {
            tableRichTextBox.AppendText("\n|}");
        }
    }
}
