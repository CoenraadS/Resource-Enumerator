using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Resource_Enumerator
{
    public partial class Form1 : Form
    {
        MenuStrip menuStrip1;

        public Form1()
        {
            initializeMenu();
            InitializeComponent();
        }

        private void initializeMenu()
        {
            menuStrip1 = new MenuStrip();
            ToolStripMenuItem fileItem = new ToolStripMenuItem("File");
            ToolStripMenuItem openSubItem = new ToolStripMenuItem("Open", null, openToolStripMenuItem_Click);
            ToolStripMenuItem recentSubItem = new ToolStripMenuItem("Recent");

            if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent1) ||
                !String.IsNullOrEmpty(Properties.Settings.Default.Recent2) ||
                !String.IsNullOrEmpty(Properties.Settings.Default.Recent3))
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent1))
                {
                    ToolStripMenuItem recentSubItem1 = new ToolStripMenuItem(Properties.Settings.Default.Recent1);
                    recentSubItem.DropDownItems.Add(recentSubItem1);
                }

                if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent2))
                {
                    ToolStripMenuItem recentSubItem2 = new ToolStripMenuItem(Properties.Settings.Default.Recent2);
                    recentSubItem.DropDownItems.Add(recentSubItem2);
                }

                if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent3))
                {
                    ToolStripMenuItem recentSubItem3 = new ToolStripMenuItem(Properties.Settings.Default.Recent3);
                    recentSubItem.DropDownItems.Add(recentSubItem3);
                }

                fileItem.DropDownItems.Add(recentSubItem);
            }

            fileItem.DropDownItems.Add(openSubItem);

            ToolStripMenuItem viewItem = new ToolStripMenuItem("View");
            ToolStripMenuItem largeIconItem = new ToolStripMenuItem("Large Icons", null, largeIconToolStripMenuItem_Click);
            ToolStripMenuItem smallIconItem = new ToolStripMenuItem("Small Icons", null, smallIconToolStripMenuItem_Click);
            ToolStripMenuItem detailItem = new ToolStripMenuItem("Details", null, detailsToolStripMenuItem_Click);
            ToolStripMenuItem listItem = new ToolStripMenuItem("List", null, listToolStripMenuItem_Click);
            ToolStripMenuItem tileItem = new ToolStripMenuItem("Tile", null, tileToolStripMenuItem_Click);

            viewItem.DropDownItems.Add(largeIconItem);
            viewItem.DropDownItems.Add(smallIconItem);
            viewItem.DropDownItems.Add(detailItem);
            viewItem.DropDownItems.Add(listItem);
            viewItem.DropDownItems.Add(tileItem);

            menuStrip1.Items.Add(fileItem);
            menuStrip1.Items.Add(viewItem);

            this.Controls.Add(menuStrip1);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                enumerateResources(openFileDialog1.FileName);
            }
        }

        private void enumerateResources(string fileName)
        {

        }

        private void largeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.LargeIcon;
        }

        private void smallIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.SmallIcon;
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Details;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.List;
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Tile;
        }
    }
}
