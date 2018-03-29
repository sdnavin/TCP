using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;

public class TcpServerConnection : MonoBehaviour {

    public static TcpServerConnection Instance;

    public int PortNo;
    public string IpAddress;

    TcpListener MySocket;

    NetworkStream MyStream;
    StreamReader MyReader;

    Thread MyThread;
    List<NetworkStream> ClientStreams;
    List<Thread> ClientThreads;


    public delegate void OnMessageRecieved(string DataGot);
    public static OnMessageRecieved MessageRecieved;

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        ThreadStart TS = new ThreadStart(MakeConnection);
        ClientThreads = new List<Thread>();
        ClientStreams = new List<NetworkStream>();
        MyThread = new Thread(TS);
        MyThread.Start();
        
    }

    public void OnDestroy()
    {
        if (MyThread!=null&&MyThread.IsAlive)
        {
            MyThread.Abort();
            print("Main Thread Aborted"+ ClientThreads.Count);

        }
        for (int t = 0; t < ClientThreads.Count; t++)
        {
            if (ClientThreads[t] != null&&ClientThreads[t].IsAlive)
            {
                ClientThreads[t].Abort();
                if(ClientStreams[t]!=null)
                    ClientStreams[t].Close();
                print("Client"+ t +" Thread Aborted");

            }
        }
    }
    void MakeConnection()
    {
        try
        {
            MySocket = new TcpListener(System.Net.IPAddress.Parse(IpAddress), PortNo);
            MySocket.Start();
            while (true)
            {
                TcpClient client = MySocket.AcceptTcpClient();
                Debug.LogError("Connected to "+client.ToString());

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                ClientThreads.Add(clientThread);
                clientThread.Start(client);
            }

        }
        catch (SocketException e)
        {
            Debug.Log("SocketException:" + e);
            MyThread.Start();
        }

    }

    private void HandleClientComm(object client)
    {
        string MessageBufffer="";
        TcpClient tcpClient = (TcpClient)client;
        Debug.LogError("Handling"+ tcpClient.Client.AddressFamily.ToString());

        NetworkStream clientStream = tcpClient.GetStream();
        ClientStreams.Add(clientStream);
        byte[] message = new byte[4096];
        int bytesRead;
        while (true)
        {
            bytesRead = 0;
            try
            {
                bytesRead = clientStream.Read(message, 0, 4096);
                if (bytesRead == 0)
                {
                    clientStream.Close();
                    print("Client Disconnected");
                    break;
                }
            }
            catch
            {
                break;
            }
          
            ASCIIEncoding encoder = new ASCIIEncoding();
            MessageBufffer = encoder.GetString(message, 0, bytesRead);
            if (MessageRecieved != null)
            {
                MessageRecieved(MessageBufffer);
            }
            clientStream.Flush();
        }
    }

    public void logdata(string MessageBufffer)
    {
        string[] Messagecontent = MessageBufffer.Split(':');
        UiController.Instance.DebugUIText(Messagecontent[0] + ":" + Messagecontent[1]);
    }
}

