// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using MessagePack;

public class AimDataReceiver : MonoBehaviour
{
    private GameControl gameControl;
    
    private float x = 0f, y = 0f;
    private List<(float X, float Y)> coordinatesWindow = new List<(float X, float Y)>();
    private int maxLength = 50;

    private Thread listenerThread;
    private System.Object objLock = new System.Object();
    private bool keepAlive = false;

    private string pupilRemoteAddress, publisherAddress = "localhost";
    private int pupilRemotePort = 50020;
    private int publisherPort;

    void Start() {
        gameControl = GameObject.Find("GameManager").GetComponent<GameControl>();
    }

    //Periodically check if communication with glasses should be terminated. Otherwise update coordinates on screen.
    void Update() {
        if(keepAlive) {
            gameControl.UpdateCoordinatesText(x, y);
            
            if(gameControl.GetIsMouseAimingOn()) {
                StopListening();
            }

        } else if(!gameControl.GetIsMouseAimingOn()) {
            StartListening();
        }
    }

    //Sets coordinates after averaging them via sliding window.
    void setCoordinates(dynamic coordinates) {
        lock (objLock) {

            float newX = Mathf.Clamp(float.Parse(coordinates[0].ToString()), 0f, 1f);
            float newY = Mathf.Clamp(float.Parse(coordinates[1].ToString()), 0f, 1f);

            if(Vector2.Distance(new Vector2(newX, newY), new Vector2(x, y)) > 0.05f) {
                coordinatesWindow.RemoveRange(0, (int)coordinatesWindow.Count/2);
            }

            if(coordinatesWindow.Count > maxLength) {
                coordinatesWindow.RemoveAt(0);
            }

            coordinatesWindow.Add((newX, newY));
            
            float xSum = 0, ySum = 0;
            foreach(var coords in coordinatesWindow) {
                xSum += coords.Item1;
                ySum += coords.Item2;
            }

            x = xSum / coordinatesWindow.Count;
            y = ySum / coordinatesWindow.Count;
        }
    }

    //Gets port of Subsriber-Publisher communication.
    void GetPublisherInfo() {
        AsyncIO.ForceDotNet.Force();

        string fullAddress = "tcp://" + pupilRemoteAddress + ":" + pupilRemotePort;

        using (var requestSocket = new RequestSocket(fullAddress))
        {   
            requestSocket.SendFrame("SUB_PORT");
            Debug.Log("Sent port request");
            if(requestSocket.TryReceiveFrameString(TimeSpan.FromSeconds(2), out var message)) {
                Debug.Log("Port: " + message);

                Int32.TryParse(message, out int port);
                publisherPort = port;
            }   
            requestSocket.Close();
        }
        NetMQConfig.Cleanup();

        Debug.Log("IP and port received");
    }

    //Connects to the Subscriber-Publisher communication and updates coordinates based on data received.
    void Listen() {
        GetPublisherInfo();
        AsyncIO.ForceDotNet.Force();

        using (var socket = new SubscriberSocket())
        {
            socket.Connect("tcp://" + publisherAddress + ":" + publisherPort);
            socket.Subscribe("surfaces.Game");

            Debug.Log("Subscribed");

            NetMQMessage message = new NetMQMessage();
            
            for (int i = 0; keepAlive; i++)
            { 
                //Debug.Log(i + " : " + "start");

                if(socket.TryReceiveMultipartMessage(TimeSpan.FromSeconds(2), ref message)) {
                    string msgType = message[0].ConvertToString();
                    
                    var messageDictionary = MessagePackSerializer.Deserialize<dynamic>(message[1].ToByteArray());
                        //Debug.Log("No.: " + i + ", data: " + messageDictionary["gaze_normals_3d"]["0"][0]);
                        try {
                            dynamic coordinates;
                            if(messageDictionary.TryGetValue("gaze_on_surfaces", out coordinates)) {
                                setCoordinates(coordinates[0]["norm_pos"]);

                            }
                        } catch (Exception e) {
                            Debug.Log(e);
                        }
                } else {
                    Debug.Log("No.: " + i + " : " + "Timed out");
                }     
                
            }
            
            socket.Close();
        }
        NetMQConfig.Cleanup();

        Debug.Log("Work finished");
    }

    //Starts communication.
    void StartListening() {
        listenerThread = new Thread(Listen);

        if(!listenerThread.IsAlive) {
            listenerThread.Start();
        }

        keepAlive = true;
        Debug.Log("Thread started");
    }

    void OnDestroy()
    {
        StopListening();
    }

    void OnApplicationQuit()
    {
        StopListening();
    }

    void StopListening() {
        if(listenerThread != null && listenerThread.IsAlive) {
            lock (objLock) keepAlive = false;
            listenerThread.Join();
            listenerThread = null;
            Debug.Log("Thread stopped");
        }
    }

    public (float, float) GetCoordinates() {
        return (x, y);
    }
}
