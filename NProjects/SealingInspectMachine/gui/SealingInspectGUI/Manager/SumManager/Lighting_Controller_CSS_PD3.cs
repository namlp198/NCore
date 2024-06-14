using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SealingInspectGUI.Manager.SumManager
{
    public class Lighting_Controller_CSS_PD3
    {
        private const int timeout_period = 10000;
        private string m_strIP;
        private int m_nPort = 40001;
        private string[] m_arrCmdSetLightIntensity_255;
        private string[] m_arrCmdSetLightIntensity_0;
        public Lighting_Controller_CSS_PD3(string strIP, int nPort)
        {
            m_strIP = strIP;
            m_nPort = nPort;

            m_arrCmdSetLightIntensity_255 = new string[4] 
            {
                "@00F25582",
                "@01F25583",
                "@02F00078",
                "@03F25585"
            };
            m_arrCmdSetLightIntensity_0 = new string[4]
            {
                "@00F00076",
                "@01F00077",
                "@02F00078",
                "@03F00079"
            };
        }

        // method to TCP receive
        private bool Tcp_receive(NetworkStream ns, ref byte[] resBytes)
        {
            int resSize;

            do
            {
                // receive part of data
                resSize = ns.Read(resBytes, 0, resBytes.Length);

                // check server disconnect
                if (resSize == 0)
                {
                    return false;
                }

                // continue receiving if there is still data
            } while (resBytes[resSize - 2] != '\r' && resBytes[resSize - 1] != '\n');

            return true;
        }
        private void SendCmd(string[] arrCmd)
        {
            try
            {
                // make socket
                using (TcpClient tcp = new TcpClient())
                {
                    if (tcp.ConnectAsync(m_strIP, m_nPort).Wait(timeout_period) != true)
                    {

                        throw new SocketException((int)SocketError.TimedOut);
                    }


                    tcp.Client.SendTimeout = timeout_period;
                    tcp.Client.ReceiveTimeout = timeout_period;

                    // get network stream used to send and receive data
                    using (NetworkStream ns = tcp.GetStream())
                    {

                        foreach (string send_text in arrCmd)
                        {

                            // when null or whitespace, skip loop
                            if (!string.IsNullOrWhiteSpace(send_text))
                            {

                                // change encoding
                                Encoding enc = Encoding.UTF8;
                                byte[] sendBytes = enc.GetBytes(send_text + "\r\n");

                                // send data
                                ns.Write(sendBytes, 0, sendBytes.Length);

                                // receive data
                                byte[] resBytes = new byte[256];
                                if (!Tcp_receive(ns, ref resBytes))
                                {
                                    throw new Exception("");
                                }

                                // change encoding
                                string resMSG = Encoding.UTF8.GetString(resBytes);

                            }
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public bool Ping_IP()
        {
            if (m_strIP.CompareTo("") == 0)
                return false;

            // check IP address
            if (!IPAddress.TryParse(m_strIP, out _))
            {
                return false;
            }

            using (System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping())
            {
                try
                {
                    System.Net.NetworkInformation.PingReply reply = ping.Send(m_strIP);
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }

        }
        public void Set_4_Light_255()
        {
            SendCmd(m_arrCmdSetLightIntensity_255);
        }
        public void Set_4_Light_0()
        {
            SendCmd(m_arrCmdSetLightIntensity_0);
        }
    }
}
