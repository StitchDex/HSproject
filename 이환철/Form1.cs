using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;

namespace 이환철
{
    public partial class Form1 : Form
    {
        static string connstr = "server=165.246.104.245;port=3306;uid=user;pwd=Rktmxjqls1!;database=logon_db_alter";
        MySqlConnection conn = new MySqlConnection(connstr);

        public ThreadStart ts1;
        public Thread th1;

        delegate void SetFormStateCallback(bool isTrue);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            Parameter.isLooping = true;
            if (th1 == null || th1.IsAlive == false)//
            {
                ts1 = new ThreadStart(this.CheckMinimized);
                th1 = new Thread(ts1);
                th1.Start();//최소화 확인쓰레드 시작
            }

            this.WindowState = FormWindowState.Maximized;
            try
            {
                conn.Open();
                
            }
            catch
            {
                MessageBox.Show("연결실패");
            }


        }

        private void CheckMinimized()//폼의 크기상태를 확인해 최소화 등 방지
        {

            while (Parameter.isLooping)
            {
                Thread.Sleep(1);//0.001초 간격으로 확인
                if (this.InvokeRequired)
                {
                    SetFormStateCallback d = new SetFormStateCallback(CheckMinimized_MT);
                    this.Invoke(d, new object[] { true });
                }
                else
                {
                    CheckMinimized_MT(true);
                }
            }
        }

        private void CheckMinimized_MT(bool isTrue)//최소화 확인 및 최대화 메소드
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;//최대화
            }
            this.BringToFront();
            this.SetTopLevel(true);
            if (this.TopMost == false) { this.TopMost = true; }//맨 앞으로 가져오기


        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                MySqlDataAdapter adpt = new MySqlDataAdapter();
                string sql = "select * from student_tb";
                adpt.SelectCommand = new MySqlCommand(sql, conn);
                adpt.Fill(ds, "Student");

                foreach(DataRow r in ds.Tables["Student"].Rows)
                {
                    if(txtNum.Text == r[0].ToString() && txtName.Text == r[1].ToString())
                    {
                        Parameter.isLooping = false;

                        while (th1 != null && th1.IsAlive == true)//
                        {
                            th1.Abort();
                        }

                        Parameter.UserNum = txtNum.Text;

                        this.Hide();
                        Form2 form2 = new Form2();
                        form2.Show();
                        conn.Close();

                        
                        return;
                    }
                    
                }
                MessageBox.Show("Log on ERROR \r\r 학번 및 이름을 확인해 주세요.", "LogOn EROOR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch { }
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Parameter.isLooping = false;
            while (th1 != null && th1.IsAlive == true)//
            {
                th1.Abort();
            }

            if (txtName.Text == Parameter.adminID && txtNum.Text == Parameter.adminPW)
            {
                Parameter.isForcedClose = true;
            }
            else
            {
                MessageBox.Show("정상적으로 종료해주세요!", "강제 종료 의심", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Parameter.isLooping = false;
            while (th1 != null && th1.IsAlive == true)//
            {
                th1.Abort();
            }
            Parameter.isForcedClose = true;
            System.Diagnostics.Process.Start("shutdown.exe", "/r /f /t 0");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Parameter.isLooping = false;
            while (th1 != null && th1.IsAlive == true)//
            {
                th1.Abort();
            }
            Parameter.isForcedClose = true;
            System.Diagnostics.Process.Start("shutdown.exe", "/p /f");
        }

        
    }

    class Parameter
    {
        static public string UserNum = "";
        static public string adminID = "1234";
        static public string adminPW = "1234";

        static public bool isForcedClose = false;
        static public bool isLooping = false;

    }

    class ProgramList
    {
        List<string[]> ProList = new List<string[]>();
        public List<string[]> NewList()//현재 사용내역 저장
        {
            
            System.Diagnostics.Process[] pro = System.Diagnostics.Process.GetProcesses();//사용내역 불러오기

            for (int i = 0; i < pro.Length; i++)
            {
                if (pro[i].MainWindowHandle != IntPtr.Zero)
                {
                    if (pro[i].MainWindowTitle == "") continue;
                    string[] temp = { pro[i].ProcessName, pro[i].MainWindowTitle };
                    ProList.Add(temp);//각 열에 프로세스명과 캡션명을 순서대로 저장
                }
            }
            
            return ProList;
        }
        public string[] ProReturn()//프로세스명 받기
        {
            string[] ProOut = new string[ProList.Count];
            for (int i = 0; i < ProList.Count; i++)
            {
                ProOut[i] = ProList.ElementAt<string[]>(i)[0];
            }
            Array.Resize(ref ProOut, ProList.Count);
            return ProOut;
        }
        public string[] CapReturn()//캡션명 받기
        {
            string[] CapOut = new string[ProList.Count];
            for (int i = 0; i < ProList.Count; i++)
            {
                CapOut[i] = ProList.ElementAt<string[]>(i)[1];
            }
            Array.Resize(ref CapOut, ProList.Count);
            return CapOut;
        }
    }
}

