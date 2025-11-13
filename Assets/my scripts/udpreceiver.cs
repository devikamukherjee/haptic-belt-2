using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

public class udpreceiver : MonoBehaviour
{
    [Header("UDP Settings")]
    public int port = 5065;

    [Header("Visualization")]
    public GameObject jointPrefab;  // Assign a small Sphere prefab
    public float scale = 0.01f;     // Scale to adjust units

    private UdpClient client;
    private Thread receiveThread;

    private Vector3[] jointPositions = new Vector3[33];
    private GameObject[] jointObjects = new GameObject[33];

    private bool newDataAvailable = false;

    void Start()
    {
        // Instantiate spheres
        for (int i = 0; i < 33; i++)
        {
            jointObjects[i] = Instantiate(jointPrefab, Vector3.zero, Quaternion.identity);
            jointObjects[i].name = "Joint_" + i;
        }

        // Start UDP listener
        client = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("UDP listener started on port " + port);
    }

    void ReceiveData()
    {
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);
        while (true)
        {
            try
            {
                byte[] data = client.Receive(ref anyIP);
                string json = Encoding.UTF8.GetString(data);
                PoseMessage msg = JsonConvert.DeserializeObject<PoseMessage>(json);
                if (msg.points.Length == 33)
                {
                    for (int i = 0; i < 33; i++)
                    {
                        jointPositions[i] = new Vector3(
                            (float)msg.points[i][0],
                            (float)msg.points[i][1],
                            (float)msg.points[i][2]
                        ) * scale;
                    }
                    newDataAvailable = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("UDP receive error: " + e.Message);
            }
        }
    }

    void Update()
    {
        if (newDataAvailable)
        {
            for (int i = 0; i < 33; i++)
            {
                jointObjects[i].transform.localPosition = jointPositions[i];
            }
            newDataAvailable = false;
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null)
            receiveThread.Abort();
        if (client != null)
            client.Close();
    }

    // Class to match JSON structure
    private class PoseMessage
    {
        public double[][] points;
    }
}