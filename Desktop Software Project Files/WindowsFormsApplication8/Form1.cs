﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        //Declare variables for servos
        int servo1 = 0;
        int servo2 = 0;
        int servo3 = 0;
        int servo4 = 0;
        int servo5 = 0;
        private void rotationTrackBar_Scroll(object sender, EventArgs e)//On Rotation Track Bar Update
        {
            servo1 = TrackBar1.Value;
            
        }

        private void extensionTrackBar_Scroll(object sender, EventArgs e) //On Extension Track Bar Update
        {
            servo2 = TrackBar2.Value;
            
        }

        private void elevationTrackBar_Scroll(object sender, EventArgs e)//On Elevation Track Bar Update
        {
            servo3 = TrackBar3.Value;
           
        }

        private void gripperTrackBar_Scroll(object sender, EventArgs e)//On Gripper Track Bar Update
        {
            servo4 = TrackBar4.Value;
            
        }

        private void auxTrackBar_Scroll(object sender, EventArgs e)//On Aux Track Bar Update
        {
            servo5 = TrackBar5.Value;
            
        }

        private void connectButton_Click(object sender, EventArgs e)//On serial button connect/discconect
        {
            if (serialTimer.Enabled) //If Timer is already enable disable it
            {
                serialTimer.Enabled = false;
                disconnect(); //Set labels and status to off
                isPlaying(false); //Stop playback
                
            }
            else //Otherwise turn timer on it on
            {
                serialTimer.Enabled = true;
            }
        }
            public void sendSerial() //Send Serial Values to Arduino and update labels
        {
            String s1 = String.Format("{0:D3}", servo1);
            String s2 = String.Format("{0:D3}", servo2);
            String s3 = String.Format("{0:D3}", servo3);
            String s4 = String.Format("{0:D3}", servo4);
            String s5 = String.Format("{0:D3}", servo5);
            if (arduinoSerialPort.IsOpen)
            {
                arduinoSerialPort.WriteLine(s1 + s2 + s3 + s4 + s5);
                rotationLabel.Text = "Rotation: " + s1;
                extensionLabel.Text = "Extension: " + s2;
                elevationLabel.Text = "Elevation: " + s3;
                gripperLabel.Text = "Gripper: " + s4;
                auxLabel.Text = "Aux: " + s5;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Get Baud Rate from text box
            int baud = int.Parse(baudRateTextBox.Text.ToString());
            //Set Baud Rate in Serial Connector
            arduinoSerialPort.BaudRate = baud;
           
            try
            {
                //Get all serial ports
                String[] list = SerialPort.GetPortNames();
                String portName;
                if (serialPortNumericUpDown.Value<= list.Length)
                {
                    //Set port to number selected
                    arduinoSerialPort.PortName = list[0];
                    portName = list[(int) serialPortNumericUpDown.Value - 1];
                }
                else
                {
                    //Set port to first port available
                    serialPortNumericUpDown.Value = 1;
                    arduinoSerialPort.PortName = list[0];
                    portName = list[0];
                }
                //Attempt to open port, send message, and close port
                //Open port
                arduinoSerialPort.Open();
                //Set status text at bottom of form
                connectionStatusLabel.Text = "Connected on Port: " + portName;
                
                //Set button text
                connectButton.Text = "Disconnect";
                //Send the message to the arduino
                sendSerial();
                //Close the port
                arduinoSerialPort.Close();
            }
            catch //Catch connection error
            {
                //Report connection error to user via message box
                
                //MessageBox.Show("Connection Unsuccesful:   [" + e.ToString() + "]", "Serial Connection Error");
                
                //Set labels on form to match status
                disconnect();
                //Disable timer
                serialTimer.Enabled = false;
                MessageBox.Show("Unable to connect to arduino.","Connection Errror");
                //Ensure playback is stopped.
                isPlaying(false);

            }
            
        }
        

        private void disconnect()
        {
            
            connectButton.Text = "Connect";
            connectionStatusLabel.Text = "Dissconnected";
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                serialTimer.Interval = (int)refreshRateNumericUpDown.Value;
            }
            catch
            {
                MessageBox.Show("Interval must be larger than 0ms and smaller than 10,000ms");
                refreshRateNumericUpDown.Value = 500;
            }
        }

        private void armControlLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }
        List<int[]> framesList = new List<int[]>();
        private void newFrameButton_Click(object sender, EventArgs e) //Add frame
        {
            if (framesListBox.SelectedIndex > 0)
            {
                int duration = (int)durrationNumericUpDown.Value;
                framesList.Insert(framesListBox.SelectedIndex, new int[] { servo1, servo2, servo3, servo4, servo5, duration });
                String add = listToString();
                framesListBox.Items.Insert(framesListBox.SelectedIndex, add);
                framesListBox.SelectedIndex = framesList.Count() - 1;
            }
            else
            {
                int duration = (int)durrationNumericUpDown.Value;
                framesList.Add(new int[] { servo1, servo2, servo3, servo4, servo5, duration });
                String add = listToString();
                framesListBox.Items.Add( add);
                framesListBox.SelectedIndex = framesList.Count() - 1;
            }
        }

        private void framesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
            int selected = framesListBox.SelectedIndex;
            Console.WriteLine(selected);
            int[] selectedArray = framesList[selected];
            servo1 = TrackBar1.Value = selectedArray[0];
            servo2 = TrackBar2.Value = selectedArray[1];
            servo3 = TrackBar3.Value = selectedArray[2];
            servo4 = TrackBar4.Value = selectedArray[3];
            servo5 = TrackBar5.Value = selectedArray[4];
            durrationNumericUpDown.Value = selectedArray[5];
            sendSerial();
                }
            catch { }
        }

        private void overwriteButton_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = framesListBox.SelectedIndex;
                framesList[selectedIndex] = new int[] { TrackBar1.Value, TrackBar2.Value, TrackBar3.Value, TrackBar4.Value, TrackBar5.Value, (int)durrationNumericUpDown.Value };
                framesListBox.Items.Insert(selectedIndex, listToString());
                framesListBox.Items.RemoveAt(selectedIndex + 1);
                framesListBox.SelectedIndex = selectedIndex;
                framesListBox.Select();
                framesListBox.Focus();

            }
            catch { }
        }
        private String listToString()
        {
            int duration = (int)durrationNumericUpDown.Value;
            String add = "[";
            add += servo1.ToString("000");
            add += ",";
            add += servo2.ToString("000");
            add += ",";
            add += servo3.ToString("000");
            add += ",";
            add += servo4.ToString("000");
            add += ",";
            add += servo5.ToString("000");
            add += "]";
            add += " Durration:";
            add += duration.ToString("0000");
            return add;

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            int selected = framesListBox.SelectedIndex;
            framesList.RemoveAt(selected);
            framesListBox.Items.RemoveAt(selected);
            framesListBox.ClearSelected();
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveFileDialog.ShowDialog();
            String fileAddress = saveFileDialog.FileName;
            TextWriter file = new StreamWriter(fileAddress);
            file.WriteLine("Testing..");
            file.Close();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void serialPortNumericUpDown_ValueChanged(object sender, EventArgs e)
        {

        }
        private void playFromTopButton_Click(object sender, EventArgs e)
        {
            currentFrame = 0;
            isPlaying(true);//Start playback

        }
        bool playing = false;
        private void isPlaying(bool onOff) //method to start or stop playback
        {
            playing = onOff;
            playbackTimer.Enabled = onOff;
            if (playbackTimer.Enabled)
            {
                playbackTimer.Interval = 1;
            }
            //Disable all controls while playing back
            onOff = !onOff;
            TrackBar1.Enabled = onOff;
            TrackBar2.Enabled = onOff;
            TrackBar3.Enabled = onOff;
            TrackBar4.Enabled = onOff;
            TrackBar5.Enabled = onOff;
            newFrameButton.Enabled = onOff;
            overwriteButton.Enabled = onOff;
            deleteButton.Enabled = onOff;
            durrationNumericUpDown.Enabled = onOff;

        }

        private void playFromSelectedButton_Click(object sender, EventArgs e)
        {
            currentFrame = framesListBox.SelectedIndex; //Set initial frame
            isPlaying(true);//Start playback
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            isPlaying(false);
        }
        int currentFrame = 0; //Keep track of the current frame
        private void playbackTimer_Tick(object sender, EventArgs e)
        {
            //Select listBox for current frame
            framesListBox.Select();
            framesListBox.SelectedIndex = currentFrame;
            //Load frame
            int[] currentFrameArray = framesList[currentFrame];//Get the current frame
            //Load frame into servo variables
            servo1 = currentFrameArray[0];
            servo2 = currentFrameArray[1];
            servo3 = currentFrameArray[2];
            servo4 = currentFrameArray[3];
            servo5 = currentFrameArray[4];
            //Load frame into trackbar values
            TrackBar1.Value = currentFrameArray[0];
            TrackBar2.Value = currentFrameArray[1];
            TrackBar3.Value = currentFrameArray[2];
            TrackBar4.Value = currentFrameArray[3];
            TrackBar5.Value = currentFrameArray[4];
            //Send via serial
            sendSerial();
            //Change interval
            playbackTimer.Interval = currentFrameArray[5];
            //Check for last frame
            if (framesList.Count > currentFrame + 1)
            {
                //If not, incrmeent frame
                currentFrame++;
            }
            else
            {
                //Otherwise
                if (loopCheckBox.Checked) //If loop then go to top
                {
                    currentFrame = 0;
                }
                else
                { //Otherwise stop playback
                    isPlaying(false);
                }
                
            }
            

        }
    }
    }

