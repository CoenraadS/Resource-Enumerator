using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Resource_Enumerator
{
    public partial class StringViewer : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);

        List<string> stringList;
        List<string> stringOutput;

        public StringViewer(IntPtr dataFilePointer)
        {
            InitializeComponent();

            stringList = EnumerateType.getResourceList(dataFilePointer, Resource.STRING);
            stringOutput = new List<string>();

            listBox1.DataSource = stringList;

            uint pointer;
            StringBuilder resource;
            foreach (string myString in stringList)
            {
                resource = new StringBuilder(255);
                pointer = Convert.ToUInt32(GET_RESOURCE_NAME((IntPtr)Convert.ToInt32(myString)));
                LoadString(dataFilePointer, pointer, resource, resource.Capacity + 1);

                stringOutput.Add(resource.ToString());
            }
            listBox2.DataSource = stringOutput;
        }

        private bool IS_INTRESOURCE(IntPtr value)
        {
            if (((uint)value) > ushort.MaxValue)
                return false;
            return true;
        }

        private string GET_RESOURCE_NAME(IntPtr value)
        {
            if (IS_INTRESOURCE(value) == true)
                return value.ToString();
            return Marshal.PtrToStringUni((IntPtr)value);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = listBox2.SelectedIndex;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.DataSource != null)
            listBox2.SelectedIndex = listBox1.SelectedIndex;
        }

        private void removeEmptyStringsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < stringList.Count;)
            {
                if (string.IsNullOrWhiteSpace(stringOutput[i]))
                {
                    stringOutput.RemoveAt(i);
                    stringList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            listBox1.DataSource = null;
            listBox1.DataSource = stringList;
            listBox2.DataSource = null;
            listBox2.DataSource = stringOutput;
        }
    }
}
