using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        DateTime StartTime;
        String TaskWorking;
        DateTime EndTime;
        Boolean isTaskStarted = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(isTaskStarted)
            {
                TimeSpan duration = DateTime.Now - StartTime;
                lblTimeSpent.Text = duration.ToString(@"hh\:mm\:ss");
            }
        }

        /* Stopped */
        private void button2_Click(object sender, EventArgs e)
        {
            whenStopped();
        }

        private void addTimeLog(String task, DateTime start, DateTime end)
        {
            TimeSpan duration = DateTime.Now - StartTime;
            String durationStr = duration.ToString(@"hh\:mm\:ss");
            int taskStringLength = task.Length;
            String log = "[" + task + "] ";
            
            int gap = 100 - taskStringLength;
            if (gap < 0) gap = 10;

            for (int i = 0; i < gap; i++)
            {
                log += "-";
            }
            log += " ";
            
            log+= durationStr;

            lbTimeLogs.Items.Add(log);
            isTaskStarted = false;
        }

        /* Started */
        private void button1_Click(object sender, EventArgs e)
        {
            whenStarted();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tbNewTask.Text != "")
            {
                for(int i=0; i<cmbTaskList.Items.Count)
                addLine(tbNewTask.Text);
                cmbTaskList.Items.Add(tbNewTask.Text);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                String line;

                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("tasks.txt");


                line = sr.ReadLine();
                
                //Continue to read until you reach end of file
                while (line != null)
                {
                    //write the lie to console window
                    Console.WriteLine(line);
                    //Read the next line
                    cmbTaskList.Items.Add(line);
                    cmbTaskList.Text = cmbTaskList.Items[cmbTaskList.Items.Count - 1].ToString();
                    line = sr.ReadLine();
                }

                //close the file
                sr.Close();
                Console.ReadLine();
            }
            catch (Exception err)
            {
                MessageBox.Show("No task file is available");
                addLine("Test Task");

            }

            try
            {
                timer1.Start();
            }
            catch (Exception err)
            {
             
            }

            this.FormClosing += new FormClosingEventHandler(Inicio_FormClosing_1);
	
        }
        private void Inicio_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            whenStopped();

            //Open the File
            String fileName = "Logs_"+ DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            StreamWriter sw = new StreamWriter(fileName, true, Encoding.ASCII);

            for (int i = 0; i < lbTimeLogs.Items.Count; i++)
            {
                sw.WriteLine(lbTimeLogs.Items[i]);
            }
                

            //close the file
            sw.Close();

        }

        private void addLine(String line)
        {
            try 
            {
                //Open the File
                StreamWriter sw = new StreamWriter("tasks.txt", true, Encoding.ASCII);

                sw.WriteLine(line);

                //close the file
                sw.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }            
        }

        private void btnSelectTask_Click(object sender, System.EventArgs e)
        {
            if (cmbTaskList.Text != "" )
            {
                lblSelectedTask.Text = cmbTaskList.Text;
            }
            else
            {
                MessageBox.Show("Please Select a task");
            }
        }

        private void whenStarted()
        {
            if (lblSelectedTask.Text != "Please Select a Task")
            {
                cmbTaskList.Enabled = false;
                btnSelectTask.Enabled = false;
                btnStart.Enabled = false;
                btnStop.Enabled = true;

                StartTime = DateTime.Now;
                TaskWorking = lblSelectedTask.Text;
                isTaskStarted = true;
            }
            else
            {
                MessageBox.Show("Please Select a task");
            }

        }

        private void whenStopped()
        {
            if (isTaskStarted == true)
            {
                cmbTaskList.Enabled = true;
                btnSelectTask.Enabled = true;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                EndTime = DateTime.Now;

                addTimeLog(TaskWorking, StartTime, EndTime);
            }
        }
    }
}
