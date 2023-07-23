using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// ------- for Socket -------
using System.Net;
using System.Net.Sockets;

// ------- for Get mic -------
using NAudio.Wave;

// ------- for Thread --------
using System.Threading;
using System.IO;

namespace VoiceChatTcp_Client
{
    public partial class Form1 : Form
    {
        Socket socClient;
        WaveIn wave;
        byte[] b;
        Thread trSend;
        Thread trReceive;

        public Form1()
        {
            Application.ApplicationExit += new EventHandler(OnExitApp);
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse(txtIP.Text), int.Parse(txtPort.Text));
            socClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;
            try
            {
                socClient.Connect(serverIP);
                wave = new WaveIn();
                wave.WaveFormat = new WaveFormat(8000, 8, 1);
                wave.BufferMilliseconds = 200;
                wave.DataAvailable +=wave_DataAvailable;
                wave.StartRecording();
                trSend = new Thread(new ThreadStart(Send));
                trSend.Start();
                trReceive = new Thread(new ThreadStart(Receive));
                trReceive.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Send()
        {
            while (true)
            {
                if (b != null)
                {
                    socClient.Send(b);
                    Thread.Sleep(200);
                }
            }
        }

        private void Receive()
        {
            while (true)
            {
                byte[] b = new byte[socClient.ReceiveBufferSize];
                socClient.Receive(b);
                Thread trPlay = new Thread(Play);
                trPlay.Start(b);
            }
        }

        private void Play(Object e)
        {
            byte[] b = (byte[])e;
            DirectSoundOut d = new DirectSoundOut();
            d.Init(new RawSourceWaveStream(new MemoryStream(b), new WaveFormat(8000, 8, 1)));
            d.Play();
        }

        private void wave_DataAvailable(object sender, WaveInEventArgs e)
        {
            b = new byte[e.BytesRecorded];
            b = e.Buffer;
        }


        private void Disconnect()
        {
            try
            {
                wave.StopRecording();
                bool disconnected = false;
                if (socClient != null)
                {
                    socClient.Disconnect(disconnected);
                    socClient.Shutdown(SocketShutdown.Both);
                    socClient.Close();
                }
            }
            catch
            {
                ;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
            btnDisconnect.Enabled = false;
            btnConnect.Enabled = true;
        }

        public void OnExitApp(object sender, EventArgs e)
        {
            Disconnect();
        }
    }
}
