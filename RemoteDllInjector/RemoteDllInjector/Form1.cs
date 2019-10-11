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

namespace RemoteDllInjector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

   

        private void textBox_dllpath_DragEnter(object sender, DragEventArgs e)
        {

            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            this.textBox_dllpath.Text = path;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if(String.Empty == this.textBox_dllpath.Text) {
                MessageBox.Show("dll full path is empty");
                return;
            }

            int pid = 0;
            if ( String.Empty != this.textBox_name.Text)
            {
                Process[] targetProcess = Process.GetProcessesByName(this.textBox_name.Text);
                if (targetProcess.Length != 0)
                {
                    pid = targetProcess[0].Id;
                }
            }

            if(String.Empty != this.textBox_pid.Text) {
                try
                {
                    pid = Int32.Parse(this.textBox_pid.Text);
                }
                catch (Exception ae) { 
                    MessageBox.Show("pid should be number");
                    return;
                }
            }

            if( 0 == pid){
                MessageBox.Show("ProcessPid or processName At least one");
                return;
            }


            String res = "inject fail";
            LoadLibrary.LoadNativeLibrary(pid, this.textBox_dllpath.Text,out res);
            MessageBox.Show(res);


        }


    }
}
