using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace waveFinderSend
{
    public partial class Form1 : Form
    {
        private TcpListener tcp_Listener; // TCP 통신 Listener 
        private Thread listenThread; // Listener Thread 객체
        private NetworkStream clientStream; // Client로 전송되는 값 받아오는 객체

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.txtIP.Text = "127.0.0.1";
            this.txtPort.Text = "8080";
            try
            {
                Server(); // TCP Thread
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void Server()
        {
            string MyIP = this.txtIP.Text.ToString();
            int port = Convert.ToInt32(this.txtPort.Text.ToString());

            IPAddress ipAddress = IPAddress.Parse(MyIP); // IP 주소프로토콜에 접근

            Console.WriteLine("IP Address :" + ipAddress.ToString());

            this.tcp_Listener = new TcpListener(ipAddress, port); // TCP Listener Thread 정의

            this.listenThread = new Thread(new ThreadStart(ListenerThread)); // Listen Thread 생성
            this.listenThread.Start(); // Thread 시작.
            this.lblCon.Text = "IP Address :" + ipAddress.ToString() + " Connection!";
        }

        private void ListenerThread()// Listner Thread 정의
        {
            try
            {
                tcp_Listener.Start(); // tcpListener 시작.

                while (true)
                {
                    TcpClient client = this.tcp_Listener.AcceptTcpClient(); // 클라이언트 접속

                    Thread startClientThread = new Thread(new ParameterizedThreadStart(handleClientComm)); // Client로 부터 접속
                    startClientThread.Start(client); // 시작.
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

         // 클라이언트 Thread 정의
        private void handleClientComm(object client)
        {
            try
            {
                TcpClient tcpClient = (TcpClient)client; // tcp Client 생성
                clientStream = tcpClient.GetStream(); // Client로 부터 Stream 받아오기/
                if (clientStream != null)
                {
                    byte[] message = new byte[4096]; // Message Byte 생성
                    int byteRead; // byte 받아오기
 
                    while (true)
                    {
                        byteRead = 0;
                        byteRead = clientStream.Read(message, 0, 4096); // 클라이언트로 부터 Stream Read..
                        ASCIIEncoding encoder = new ASCIIEncoding(); // Encoder..
                        //TCPdataFrameParsing(message, byteRead); // 받아온 Message로 부터 Parsing 함수
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encoder = new ASCIIEncoding(); // encoder
            string dataWave = "$GNRMC,001031.00,A,4404.13993,N,12118.86023,W,0.146,,100117,,,A*7B";
            byte[] buffer = encoder.GetBytes(dataWave); // 아스키 인코더로 stringTobyte[]
            clientStream.Write(buffer, 0, buffer.Length); // client로 송신
            clientStream.Flush(); // 버퍼 제거

        }
    }
}
