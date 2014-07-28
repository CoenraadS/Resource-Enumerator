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
        Resource[] resourceArray;
        DataTable table;
        DataRow row;
        ToolStripMenuItem fileItem;
        ToolStripMenuItem recentSubItem;
        ToolStripMenuItem recentSubItem1;
        ToolStripMenuItem recentSubItem2;
        ToolStripMenuItem recentSubItem3;

        public Form1()
        {
            dataFilePointer = IntPtr.Zero;
            resourceArray = (Resource[])Enum.GetValues(typeof(Resource));
            initializeMenu();
            InitializeComponent();
            checkedListBox1.DataSource = Enum.GetNames(typeof(Resource));
        }

        private void initializeMenu()
        {
            menuStrip1 = new MenuStrip();
            fileItem = new ToolStripMenuItem("File");
            ToolStripMenuItem openSubItem = new ToolStripMenuItem("Open", null, openToolStripMenuItem_Click);
            recentSubItem = new ToolStripMenuItem("Recent");
            ToolStripMenuItem runItem = new ToolStripMenuItem("Run", null, runToolStripMenuItem_Click);
            ToolStripMenuItem toolItem = new ToolStripMenuItem("Tools");
            ToolStripMenuItem stringToolItem = new ToolStripMenuItem("String Viewer", null, stringToolToolStripMenuItem_Click);
            ToolStripMenuItem imageToolItem = new ToolStripMenuItem("Image Viewer", null, imageToolToolStripMenuItem_Click);

            fileItem.DropDownItems.Add(openSubItem);

            #region Recent files
            if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent1) ||
                !String.IsNullOrEmpty(Properties.Settings.Default.Recent2) ||
                !String.IsNullOrEmpty(Properties.Settings.Default.Recent3))
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent1))
                {
                    recentSubItem1 = new ToolStripMenuItem(Properties.Settings.Default.Recent1, null, recentSubItem1_Click);
                    recentSubItem.DropDownItems.Add(recentSubItem1);
                }

                if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent2))
                {
                    recentSubItem2 = new ToolStripMenuItem(Properties.Settings.Default.Recent2, null, recentSubItem2_Click);
                    recentSubItem.DropDownItems.Add(recentSubItem2);
                }

                if (!String.IsNullOrEmpty(Properties.Settings.Default.Recent3))
                {
                    recentSubItem3 = new ToolStripMenuItem(Properties.Settings.Default.Recent3, null, recentSubItem2_Click);
                    recentSubItem.DropDownItems.Add(recentSubItem3);
                }
                fileItem.DropDownItems.Add(recentSubItem);
            }
            #endregion

            toolItem.DropDownItems.Add(imageToolItem);
            toolItem.DropDownItems.Add(stringToolItem);
            menuStrip1.Items.Add(fileItem);
            menuStrip1.Items.Add(runItem);
            menuStrip1.Items.Add(toolItem);


            this.Controls.Add(menuStrip1);
        }

        private void loadFile()
        {
            if (dataFilePointer != IntPtr.Zero)
            {
                FreeLibrary(dataFilePointer);
            }

            dataFilePointer = LoadLibraryEx(fileName, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
            this.Text = fileName;
        }

        private void enumerateResources()
        {
            resourceList = new List<List<string>>();

            if (File.Exists(fileName))
            {
                int checkboxCounter = 0;
                foreach (Resource resource in resourceArray)
                {
                    if (checkedListBox1.GetItemChecked(checkboxCounter))
                    {
                        List<string> tempList = EnumerateType.getResourceList(dataFilePointer, resource);
                        resourceList.Add(tempList);

                    }
                    checkboxCounter++;
                }

                table = new DataTable();
                dataGridView1.DataSource = null;

                if (resourceList.Count > 0)
                {
                    int length = 0;
                    /* The Datatable accepts input as rows. 
                     So we must arrange all columns to have same amount of rows (MAX). */
                    foreach (List<string> item in resourceList)
                    {
                        if (item.Count > length)
                        {
                            length = item.Count;
                        }
                    }

                    //Add columns

                    checkboxCounter = 0;
                    foreach (Resource resource in resourceArray)
                    {
                        if (checkedListBox1.GetItemChecked(checkboxCounter))
                        {
                            table.Columns.Add(resource.ToString());
                        }
                        checkboxCounter++;
                    }

                    //Populate via rows
                    for (int i = 0; i < length; i++) //For each row.
                    {
                        row = table.NewRow();

                        for (int x = 0; x < resourceList.Count; x++) //For each column index.
                        {
                            if (i < resourceList[x].Count)
                            {
                                row[x] = resourceList[x][i];
                            }
                        }
                        table.Rows.Add(row);
                    }
                    dataGridView1.DataSource = table;
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
                loadFile();

                //If item is new, add it to recent list and move other items down.
                if (Properties.Settings.Default.Recent1 != fileName &&
                    Properties.Settings.Default.Recent2 != fileName &&
                    Properties.Settings.Default.Recent3 != fileName)
                {
                    Properties.Settings.Default.Recent3 = Properties.Settings.Default.Recent2;
                    Properties.Settings.Default.Recent2 = Properties.Settings.Default.Recent1;
                    Properties.Settings.Default.Recent1 = fileName;
                    Properties.Settings.Default.Save();
                }

                //Add recent menu option.
                for (int i = 0; i < recentSubItem.DropDownItems.Count;)
                {
                    if (recentSubItem.DropDownItems[i] != null)
                    {
                        recentSubItem.DropDownItems.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                if (!string.IsNullOrEmpty(Properties.Settings.Default.Recent1) && recentSubItem1 != null)
                {
                    recentSubItem1 = new ToolStripMenuItem(Properties.Settings.Default.Recent1, null, recentSubItem1_Click);
                    recentSubItem.DropDownItems.Add(recentSubItem1);
                }

                if (!string.IsNullOrEmpty(Properties.Settings.Default.Recent2) && recentSubItem2 != null)
                {
                    recentSubItem2 = new ToolStripMenuItem(Properties.Settings.Default.Recent2, null, recentSubItem1_Click);
                    recentSubItem.DropDownItems.Add(recentSubItem2);
                }

                if (!string.IsNullOrEmpty(Properties.Settings.Default.Recent3) && recentSubItem3 != null)
                {
                    recentSubItem3 = new ToolStripMenuItem(Properties.Settings.Default.Recent3, null, recentSubItem1_Click);
                    recentSubItem.DropDownItems.Add(recentSubItem3);
                }

                if (recentSubItem.DropDownItems.Count > 0 && recentSubItem1 != null || recentSubItem2 != null || recentSubItem3 != null)
                {
                    fileItem.DropDownItems.Add(recentSubItem);
                }
                
            }
        }

        private void recentSubItem1_Click(object sender, EventArgs e)
        {
            fileName = Properties.Settings.Default.Recent1;
            loadFile();
        }

        private void recentSubItem2_Click(object sender, EventArgs e)
        {
            fileName = Properties.Settings.Default.Recent2;
            loadFile();

        }

        private void recentSubItem3_Click(object sender, EventArgs e)
        {
            fileName = Properties.Settings.Default.Recent3;
            loadFile();
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

        private void stringToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataFilePointer != IntPtr.Zero)
            {
                StringViewer stringViewer = new StringViewer(dataFilePointer);
                stringViewer.Show();
            }
        }

        private void imageToolToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
