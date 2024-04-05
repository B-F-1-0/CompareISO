using Microsoft.Dism;
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
    public partial class Form4 : Form
    {
        public int indexwim1 { get; set; }
        public int indexwim2 { get; set; }
        public Form4()
        {
            InitializeComponent();
        }

        public void addEntries(DismImageInfoCollection indexList, int iso)
        {
            foreach (DismImageInfo imageInfo in indexList)
            {
                if (iso == 1)
                {
                    iso1IndexList.Items.Add("Index " + imageInfo.ImageIndex + ": " + imageInfo.ImageName);
                }
                else
                {
                    iso2IndexList.Items.Add("Index " + imageInfo.ImageIndex + ": " + imageInfo.ImageName);
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (iso1IndexList.SelectedIndex == -1 || iso2IndexList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an index for both install.wim.");
                return;
            }
            String wim1;
            String wim2;
            try
            {
                wim1 = iso1IndexList.SelectedItem as String;
                wim2 = iso2IndexList.SelectedItem as String;
                wim1 = wim1.Split(" ")[1];
                wim2 = wim2.Split(" ")[1];
                wim1 = wim1.Replace(":", "");
                wim2 = wim2.Replace(":", "");
                indexwim1 = Int16.Parse(wim1);
                indexwim2 = Int16.Parse(wim2);
                this.Close();
            }
            catch
            {
                MessageBox.Show("Please select an index for both install.wim.");
                return;
            }
        }
    }
}
