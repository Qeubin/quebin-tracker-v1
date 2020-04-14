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
using Tulpep.NotificationWindow;
using System.Reflection;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        DateTime StartTime;
        String TaskWorking;
        DateTime EndTime;
        Boolean isTaskStarted = false;
        DateTime lastNofiedTime;
        int notificationMode = 2; //1 Notification Ballon; 2 Custom Notification
        int notifTimeOut_noTracking = 10;
        int notifTimeOut_tracking = 30;


        public Form1()
        {
            lastNofiedTime = DateTime.Now;
            InitializeComponent();
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            if(isTaskStarted)
            {
                TimeSpan duration = DateTime.Now - StartTime;
                lblTimeSpent.Text = duration.ToString(@"hh\:mm\:ss");

                TimeSpan dTime = DateTime.Now - lastNofiedTime;
                //if (dTime.TotalSeconds > 30 * 60 )
                //{
                //    PopupNotifier popup = new PopupNotifier();
                //    popup.TitleText = "Quebin Tracker Reminder";
                //    popup.ContentText = "Hope you are tracking the correct task...";
                //    popup.Popup();
                //    lastNofiedTime = DateTime.Now;
                //}                
                if (dTime.TotalSeconds > notifTimeOut_tracking * 60)
                //if (dTime.TotalSeconds > 0.1 * 60)
                {
                    if (notificationMode == 1)
                    {
                        notifyIcon1.ShowBalloonTip(1000, "Qeubin Reminder",
                            "Hope you are tracking correct task ;-) ....", ToolTipIcon.Info);
                        lastNofiedTime = DateTime.Now;
                    }
                    else if (notificationMode == 2)
                    {
                        PopupNotifier popup = new PopupNotifier();
                        popup.TitleText = "Quebin Tracker Reminder";
                        popup.ContentText = "You are working on \n" + TaskWorking;
                        ;
                        //Font font = new Font(new FontFamily("Comic Sans MS"), 15);
                        Font font = new Font(new FontFamily("Tahoma"), 15);
                        popup.TitleFont = font;
                        Font fontBody = new Font(new FontFamily("Comic Sans MS"), 12);
                        popup.ContentFont = fontBody;
                        popup.BodyColor = Color.LightCyan;
                        popup.Popup();
                        lastNofiedTime = DateTime.Now;
                    }
                }
            }
            //else
            //{                
            //    TimeSpan dTime = DateTime.Now - lastNofiedTime;
            //    if (dTime.TotalSeconds > 0.1 * 60)
            //    {
            //        PopupNotifier popup = new PopupNotifier();
            //        popup.TitleText = "Quebin Tracker Reminder";
            //        popup.BodyColor = Color.LightCyan;
                    
            //        popup.ContentText = "Do you want to track the work you're currently working on...";
            //        popup.Popup();
            //        lastNofiedTime = DateTime.Now;
            //    }
            //}
            else
            {
                TimeSpan dTime = DateTime.Now - lastNofiedTime;
                if (dTime.TotalSeconds > notifTimeOut_noTracking * 60)
                {
                    if (notificationMode == 1)
                    {
                        notifyIcon1.ShowBalloonTip(1000, "Qeubin Reminder",
                            "You are not started task tracking....", ToolTipIcon.Warning);
                        lastNofiedTime = DateTime.Now;
                    }
                    else if(notificationMode == 2)
                    {

                        PopupNotifier popup = new PopupNotifier();
                        popup.TitleText = "Quebin Tracker Reminder";
                        Font font = new Font(new FontFamily("Tahoma"), 15);
                        popup.TitleFont = font;
                        popup.BodyColor = Color.LightCyan;
                        popup.ContentText = "Do you want to track the work you're currently working on...";
                        Font fontBody = new Font(new FontFamily("Comic Sans MS"), 12);
                        popup.ContentFont = fontBody;
                        popup.Popup();
                        lastNofiedTime = DateTime.Now;
                    }
                }
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

            addEntryToCSV(task, start, end);
        }

        private void addEntryToCSV(String task, DateTime start, DateTime end)
        {
            //Open the File
            try
            {
                String fileName = "TimeLogs.csv";
                StreamWriter sw = new StreamWriter(fileName, true, Encoding.ASCII);

                TimeSpan duration = DateTime.Now - StartTime;
                String durationStr = duration.ToString(@"hh\:mm\:ss");

                String entry = "";
                entry += DateTime.Now.ToString("MM/dd/yy H:mm:ss.fff");
                entry += ",";
                entry += task.ToString();
                entry += ",";
                entry += start.ToString();
                entry += ",";
                entry += end.ToString();
                entry += ",";
                entry += durationStr;
                entry += ",";
                entry += duration.TotalMilliseconds.ToString();
                //entry += ",";


                sw.WriteLine(entry);
                //close the file
                sw.Close();
            }
            catch (System.Exception)
            {                
                //throw;
            }
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
                for (int i = 0; i < cmbTaskList.Items.Count; i++)
                {
                    if (tbNewTask.Text == cmbTaskList.Items[i].ToString())
                    {
                        MessageBox.Show("Task Already added");
                        return;
                    }
                }
                
                addLine(tbNewTask.Text);
                cmbTaskList.Items.Add(tbNewTask.Text);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            //FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            //string version = fvi.FileVersion;

            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //string assemblyVersion1 = Assembly.LoadFile("your assembly file").GetName().Version.ToString();
            //string fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            //string productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

            this.Text += "  v" + assemblyVersion;            

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

            loadConfigurations();
	
        }

        private void loadConfigurations()
        {
            try
            {
                String line;
                int tmpIntValue;

                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("config.conf");

                line = sr.ReadLine();

                //Continue to read until you reach end of file
                while (line != null)
                {
                    line = line.Replace(" ", String.Empty);
                    line = line.Replace("\t", String.Empty);

                    String[] config = line.Split('=');
                    if (config.Count() != 2) continue;

                    switch (config[0])
                    {
                        case "notifTimeOut_noTracking":
                            if (config[1] == "") break;
                            tmpIntValue = (int)Convert.ToInt64(config[1]);
                            if (tmpIntValue < 0 || tmpIntValue > 60) break;
                            notifTimeOut_noTracking = tmpIntValue;
                            break;
                        case "notifTimeOut_tracking":
                            if(config[1] == "") break;
                            tmpIntValue = (int)Convert.ToInt64(config[1]);
                            if (tmpIntValue < 0 || tmpIntValue > 60) break;
                            notifTimeOut_tracking = tmpIntValue;
                            break;
                        case "notificationMode":
                            if (config[1] == "") break;
                            tmpIntValue = (int)Convert.ToInt64(config[1]);
                            if (tmpIntValue < 1 || tmpIntValue > 2) break;
                            notificationMode = tmpIntValue;
                            break;
                        default:
                            break;
                    }
                    line = sr.ReadLine();
                }
            }
            catch (Exception err)
            {

            }
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

        private void Form1_Move(object sender, System.EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void showToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.TopMost = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.TopMost = true;
        }
    }
}
