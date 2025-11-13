using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class udp_receiver_test : MonoBehaviour
{
    // UDP configuration
    public int listenPort = 5056;
    private UdpClient udpClient;
    private Thread receiveThread;
    private bool isRunning;

    private Vector3[] receivedVector = new Vector3[33];

    public Transform[] joint_cubes = new Transform[33];

    private Vector3[] jointPositions = new Vector3[33];

    void Start()
    {
        StartUDPListener();
    }

    void Update()
    {
        for (int i = 0; i < joint_cubes.Length; i++)
        {
            joint_cubes[i].position = jointPositions[i];
        }
    }

    void StartUDPListener()
    {
        try
        {
            udpClient = new UdpClient(listenPort);
            isRunning = true;
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            Debug.Log($"[UDPReceiver] Listening for UDP messages on port {listenPort}...");
        }
        catch (Exception e)
        {
            Debug.LogError($"[UDPReceiver] Error starting listener: {e.Message}");
        }
    }

    private void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                // Debug.Log(message);

                string[] parts = message.Split(',');

                // Process each joint's coordinates
                for (int joint = 0; joint < 33; joint++)
                {
                    int baseIndex = joint * 3;
                    if (baseIndex + 2 < parts.Length)
                    {
                        float x = float.Parse(parts[baseIndex]);
                        float y = float.Parse(parts[baseIndex + 1]);
                        float z = float.Parse(parts[baseIndex + 2]);
                        jointPositions[joint] = new Vector3(x, y, z);
                    }
                }

                // Debug.Log($"[UDPReceiver] Received from {remoteEndPoint.Address}: {message}");
            }
            catch (SocketException se)
            {
                if (isRunning) // ignore exceptions when stopping
                    Debug.LogError($"[UDPReceiver] Socket error: {se.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[UDPReceiver] Exception: {e.Message}");
            }
        }
    }

    void OnApplicationQuit()
    {
        StopUDPListener();
    }

    void OnDisable()
    {
        StopUDPListener();
    }

    void StopUDPListener()
    {
        if (isRunning)
        {
            isRunning = false;
            udpClient?.Close();
            receiveThread?.Join();
            Debug.Log("UDP Receiver Stopped listening.");
        }
    }
}
