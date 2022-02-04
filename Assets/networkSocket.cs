using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using TMPro;
using UnityEngine.UI;
 


public class networkSocket:MonoBehaviour
{
    public TMP_Text statusText;
    Socket serverSocket;
    Socket clientSocket;
    IPEndPoint ipEnd; 
    string recvStr;
    string sendStr; 
    byte[] recvData=new byte[1024];
    byte[] sendData=new byte[1024];
    int recvLen; 
    Thread connectThread;


    void InitSocket()
    {
        
        ipEnd=new IPEndPoint(0,60000);
        
        serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        serverSocket.Bind(ipEnd);
        serverSocket.Listen(10);

        connectThread= new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

   
    void SocketConnet()
    {
        if(clientSocket!=null)
            clientSocket.Close();
        print("Waiting for a client");
        clientSocket=serverSocket.Accept();
        IPEndPoint ipEndClient=(IPEndPoint)clientSocket.RemoteEndPoint;
        print("Connect with "+ipEndClient.Address.ToString()+":"+ipEndClient.Port.ToString());
        sendStr="Welcome to my server";
        SocketSend(sendStr);
    }

    void SocketSend(string sendStr)
    {
        sendData=new byte[1024];
        sendData=Encoding.ASCII.GetBytes(sendStr);
        clientSocket.Send(sendData,sendData.Length,SocketFlags.None);
    }

    void SocketReceive()
    {
        SocketConnet();      
        while(true)
        {
         
            recvData=new byte[1024];
            recvLen=clientSocket.Receive(recvData);
            if(recvLen==0)
            {
                SocketConnet();
                continue;
            }
        
            recvStr=Encoding.ASCII.GetString(recvData,0,recvLen);
            print(recvStr);
            sendStr="From Server: "+recvStr;
            SocketSend(sendStr);
        }
    }

    void SocketQuit()
    {
        if(clientSocket!=null)
            clientSocket.Close();
        if(connectThread!=null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        serverSocket.Close();
        print("diconnect");
    }

    void Start()
    {
        InitSocket();
    }


    void Update()
    {
        statusText.text = Encoding.ASCII.GetString(recvData,0,recvLen);
    }

    void OnApplicationQuit()
    {
        SocketQuit();
    }
}