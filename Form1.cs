using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace Resource_Enumerator
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);

        MenuStrip menuStrip1;
        List<List<string>> resourceList;
        string fileName;
        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        IntPtr dataFilePointer;

        public Form1()
        {
            dataFilePointer = IntPtr.Zero;
            resourceList = new List<List<string>>();
            initializeMenu();
            InitializeComponent();
            checkedListBox1.DataSource = Enum.GetNames(typeof(Resource));
        }

        private void initializeMenu()
        {
            menuStrip1 = new MenuStrip();
            ToolStripMenuItem fileItem = new ToolStripMenuItem("File");
            ToolStripMenuItem openSubItem = new ToolStripMenuItem("Open", null, openToolStripMenuItem_Click);
            ToolStripMenuItem recentSubItem = new ToolStripMenuItem("Recent");
            ToolStripMenuItem runItem = new ToolStripMenuItem("Run", null, runToolStripMenuItem_Click);

            if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent1) ||
                !String.IsNullOrEmpty(Properties.Settings.Default.Recent2) ||
                !String.IsNullOrEmpty(Properties.Settings.Default.Recent3))
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent1))
                {
                    ToolStripMenuItem recentSubItem1 = new ToolStripMenuItem(Properties.Settings.Default.Recent1, null, recentSubItem1_Click);
                    recentSubItem.DropDownItems.Add(recentSubItem1);
                }

                if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent2))
                {
                    ToolStripMenuItem recentSubItem2 = new ToolStripMenuItem(Properties.Settings.Default.Recent2, null, recentSubItem2_Click);
                    recentSubItem.DropDownItems.Add(recentSubItem2);
                }

                if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent3))
                {
                    ToolStripMenuItem recentSubItem3 = new ToolStripMenuItem(Properties.Settings.Default.Recent3, null, recentSubItem2_Click);
                    recentSubItem.DropDownItems.Add(recentSubItem3);
                }

                fileItem.DropDownItems.Add(recentSubItem);
            }

            fileItem.DropDownItems.Add(openSubItem);           

            menuStrip1.Items.Add(fileItem);
            menuStrip1.Items.Add(runItem);

            this.Controls.Add(menuStrip1);
        }

        private void enumerateResources()
        {
            if (dataFilePointer != IntPtr.Zero)
            {
                try
                {
                    FreeLibrary(dataFilePointer);
                }
                catch {}
            }
            dataFilePointer = LoadLibraryEx(fileName, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);

            listBox1.Items.Clear();

            if (File.Exists(fileName))
            {
                foreach (Resource resource in (Resource[])Enum.GetValues(typeof(Resource)))
                {
                    resourceList.Add(EnumerateType.getResourceList(dataFilePointer, Resource.CURSOR));
                }

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.GetItemChecked(i))
                    {
                        if (resourceList[i].Count > 0)
                        {
                            listBox1.Items.Add((Resource)Enum.GetValues(typeof(Resource)).GetValue(i));
                            foreach (String item in resourceList[i])
                            {
                                listBox1.Items.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("File does not exist");
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;

                this.Text = openFileDialog1.SafeFileName;

                Properties.Settings.Default.Recent3 = Properties.Settings.Default.Recent2;
                Properties.Settings.Default.Recent2 = Properties.Settings.Default.Recent1;
                Properties.Settings.Default.Recent1 = fileName;
            }
        }

        private void recentSubItem1_Click(object sender, EventArgs e)
        {
            fileName = Properties.Settings.Default.Recent1;
            this.Text = fileName;
        }

        private void recentSubItem2_Click(object sender, EventArgs e)
        {
            fileName = Properties.Settings.Default.Recent2;
            this.Text = fileName;

        }

        private void recentSubItem3_Click(object sender, EventArgs e)
        {
            fileName = Properties.Settings.Default.Recent3;
            this.Text = fileName;
        }

        private void checkBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAll.Checked)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enumerateResources();
        }
    }
}
