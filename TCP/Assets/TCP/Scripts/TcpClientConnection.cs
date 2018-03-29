using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;

public class TcpClientConnection : MonoBehaviour {

    public static TcpClientConnection Instance;

    public int PortNo;
    public string IpAddress;

    TcpClient MySocket;

    NetworkStream MyStream;

    StreamReader MyReader;

    Thread MyThread;
    Thread ClientThread;
    Thread SendThread;

    string Msgtosend;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartConnection();
    }

    private void StartConnection()
    {
        ThreadStart TS = new ThreadStart(MakeConnection);
        MyThread = new Thread(TS);
        MyThread.Start();
    }

    public void MessagetoSend(InputField Datatosend)
    {
        Msgtosend = Datatosend.text;
        SendMessage();
    }

    public void OnDestroy()
    {
        if (MyThread!=null&&MyThread.IsAlive)
        {
            MyThread.Abort();
            print("Main Thread Aborted");
        }
        if (ClientThread != null&&ClientThread.IsAlive)
        {
            print("ClientThread Aborted");
            ClientThread.Abort();
        }
        if (SendThread != null && SendThread.IsAlive)
        {
            print("SendThread Aborted");
            SendThread.Abort();
        }
    }
    void MakeConnection()
    {
        try
        {
            MySocket = new TcpClient();
            while (!MySocket.Connected)
            {
                MySocket.Connect(System.Net.IPAddress.Parse(IpAddress), PortNo);
               
            }
            if (MySocket.Connected)
            {
                ClientThread = new Thread(new ThreadStart(HandleCommunication));
                ClientThread.Start();
            }
        }catch(SocketException e)
        {
            Debug.LogWarning("SocketException:" + e);
            StartConnection();
        }
    }

    void SendMessage()
    {
        if (MySocket.Connected)
        {
            SendThread = new Thread(new ThreadStart(SendMessageToServer));
            SendThread.Start();
        }
    }

    void HandleCommunication()
    {
        Debug.LogError("Handling");

        try
        {
            byte[] message = new byte[4096];
            int bytesRead;
            while (true)
            {
                print("loo");
                MyStream = MySocket.GetStream();
                bytesRead = MyStream.Read(message, 0, 4096);
                if (bytesRead == 0)
                {
                    Debug.LogError("Disconnected");
                    StartConnection();
                    break;
                }
            }
        }
        catch (SocketException e)
        {
            Debug.LogWarning("SocketException:" + e);
        }
    }

    void SendMessageToServer()
    {
        Debug.LogError("Sending");
        try
        {
            string MessageTo = Msgtosend;
            print("M :" + MessageTo.Length);
            MyStream = MySocket.GetStream();
            Debug.Log("sending: " + MessageTo);
            ASCIIEncoding EncodedString = new ASCIIEncoding();
            byte[] bytestosend = EncodedString.GetBytes(MessageTo);
            MyStream.Write(bytestosend, 0, bytestosend.Length);
        }
        catch (SocketException e)
        {
            Debug.LogWarning("SocketException:" + e);
        }
    }

    bool isConnected;
    private void FixedUpdate()
    {
        if (MySocket != null)
        {
            if (!isConnected && MySocket.Connected)
            {
                isConnected = true;
                Debug.LogError("Connected");
            }
            else if (isConnected  && !MySocket.Connected)
            {
                isConnected = false;
                StartConnection();
                Debug.LogError("Disconnected");
            }
        }
    }

}

