using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// ------ for Socket -------
using System.Net;
using System.Net.Sockets;

// ------ for Thread -------
using System.Threading;

namespace VoiceChatTcp
{
    public partial class Form1 : Form
    {
        Socket socServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> socClients = new List<Socket>();
        Thread trReceive;
        Thread trSendVoice;
        public Form1()
        {
            Application.ApplicationExit += new EventHandler(OnExitApp);
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(txtIP.Text), int.Parse(txtPort.Text));
            socServer.Bind(ipEndPoint);
            socServer.Listen(1);
            Thread trAccept = new Thread(new ThreadStart(Accept));
            trAccept.Start();
        }

        private void Accept()
        {
            while (true)
            {
                Socket client = socServer.Accept();
                socClients.Add(client);
                showListAllClient();
                trReceive = new Thread(Receive);
                trReceive.Start(client);
            }
        }

        private void Receive(Object ObjSoc)
        {
            Socket client = (Socket)ObjSoc;
            
            while (true)
            {
                byte[] data = new byte[client.ReceiveBufferSize];
                try
                {
                    client.Receive(data);
                    IPEndPoint ir = client.RemoteEndPoint as IPEndPoint;
                    foreach (Socket cl in socClients)
                    {
                        IPEndPoint ip = cl.RemoteEndPoint as IPEndPoint;
                        if (ip.Address.ToString() != ir.Address.ToString() || ip.Port.ToString() != ir.Port.ToString())
                        {
                            cl.Send(data);
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        private void SendVoice(Object b)
        {
            byte[] c = (byte[])b;

            foreach (Socket client in socClients)
            {
                client.Send(c);
            }
            return;
        }

        public void showListAllClient()
        {
            
            this.Invoke((MethodInvoker)delegate
            {
                listClients.Items.Clear();
                foreach (Socket client in socClients)
                {
                    IPEndPoint Ip = client.RemoteEndPoint as IPEndPoint;
                    listClients.Items.Add(Ip.Address + ":" + Ip.Port);
                }
            });
        }

        public void OnExitApp(object sender, EventArgs e)
        {
            try
            {
                foreach (Socket client in socClients)
                {
                    if (client != null)
                    {
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                    }
                }

                if (socServer != null)
                {
                    socServer.Shutdown(SocketShutdown.Both);
                }
                Environment.Exit(Environment.ExitCode);
            }
            catch
            {
                Environment.Exit(Environment.ExitCode);
            }
            finally
            {
                Environment.Exit(Environment.ExitCode);
            }
        }
    }
}
