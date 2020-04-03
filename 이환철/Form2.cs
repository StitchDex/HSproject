using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace 이환철
{
    public partial class Form2 : Form
    {
        DateTime dt = DateTime.Now;

        public Form2()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true) { this.TopMost = true; }
            else { this.TopMost = false; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Parameter.isForcedClose = true;
            this.Close();
            Form1 f1 = new Form1();
            f1.Show();
            
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Parameter.isForcedClose == false)
            {                      
                MessageBox.Show("정상적으로 종료해주세요!", "강제 종료 의심", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            timer1.Stop();
            

            
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
            label2.Text = dt.ToString("yyyy-MM-dd HH:mm:ss.000");
            label1.Text = Parameter.UserNum;

            timer1.Start();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ProgramList PL = new ProgramList();
            PL.NewList();
            string[] proList = PL.CapReturn();
            lstbxTaskArray.Items.Clear();

            for (int i = 0; i < proList.Length; i++)
            {
                lstbxTaskArray.Items.Add(proList[i]);
                
            }
        }
    }
}
