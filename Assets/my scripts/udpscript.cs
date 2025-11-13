using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// HiFiBerry UDP-JSON client for the MixerServer running on the Raspberry Pi.
/// Singleton pattern allows easy access from other scripts.
/// </summary>
public class udpscript : MonoBehaviour
{
    [Header("Server")]
    [Tooltip("IP address of the Raspberry Pi running hifi_server.py")]
    public string serverIP = "192.168.1.12";
    public int serverPort = 5005;
    [Tooltip("Receive timeout in milliseconds (0 = no wait)")]
    public int timeoutMs = 2000;

    // Singleton instance
    public static udpscript Instance { get; private set; }

    private UdpClient _udp;
    private IPEndPoint _remoteEndPoint;

    // Events for other scripts to subscribe to
    public event System.Action<string> OnServerResponse;
    public event System.Action<string> OnConnectionError;

    // ---------- Unity lifecycle ----------
    void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeConnection();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeConnection()
    {
        _udp = new UdpClient();
        _udp.Client.ReceiveTimeout = timeoutMs;
        _remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        Debug.Log($"UDP_Comms initialized - Target: {serverIP}:{serverPort}");
    }

    void OnDestroy()
    {
        _udp?.Close();
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // ---------- Example key controls ----------
    void Update()
    {
        // Commented out - let other scripts handle input
        /*
        if (Input.GetKeyDown(KeyCode.A))
            Play("HAPTICS_BassPulse_441.wav", channel: 1, gain: 0.9f, loop: false);
        if (Input.GetKeyDown(KeyCode.Space))
            StopAll();
        if (Input.GetKeyDown(KeyCode.L))
            ListTracks();
        */
    }

    // ---------- Public API ----------
    public void Play(string file, int channel, float gain = 1f, bool loop = false)
    {
        var msg = new Dictionary<string, object>
        {
            { "cmd", "play" },
            { "file", file },
            { "channel", channel },
            { "gain", gain },
            { "loop", loop ? 1 : 0 }
        };
        Send(msg);
    }

    public void Stop(int trackId)
    {
        var msg = new Dictionary<string, object>
        {
            { "cmd", "stop" },
            { "track_id", trackId }
        };
        Send(msg);
    }

    public void StopFile(string file)
    {
        var msg = new Dictionary<string, object>
        {
            { "cmd", "stop" },
            { "file", file }
        };
        Send(msg);
    }

    public void StopAll()
    {
        Send(new Dictionary<string, object> { { "cmd", "stop_all" } });
    }

    public void SetVolume(int trackId, float gain)
    {
        var msg = new Dictionary<string, object>
        {
            { "cmd", "set_vol" },
            { "track_id", trackId },
            { "gain", gain }
        };
        Send(msg);
    }

    public void ListTracks()
    {
        Send(new Dictionary<string, object> { { "cmd", "list" } });
    }

    public void ServerStatus()
    {
        Send(new Dictionary<string, object> { { "cmd", "status" } });
    }

    // ---------- Convenience methods for common use cases ----------
    
    /// <summary>
    /// Play a haptic feedback file on a specific channel
    /// </summary>
    public void PlayHaptic(string hapticFile, int channel, float intensity = 1f)
    {
        Play(hapticFile, channel, intensity, false);
    }

    /// <summary>
    /// Play background audio that loops
    /// </summary>
    public void PlayLooping(string audioFile, int channel, float volume = 1f)
    {
        Play(audioFile, channel, volume, true);
    }

    /// <summary>
    /// Quick method to play a sound effect
    /// </summary>
    public void PlaySoundEffect(string soundFile, int channel = 1, float volume = 1f)
    {
        Play(soundFile, channel, volume, false);
    }

    // ---------- Internals ----------
    private void Send(Dictionary<string, object> message)
    {
        if (_udp == null)
        {
            Debug.LogError("UDP_Comms: UDP client not initialized!");
            OnConnectionError?.Invoke("UDP client not initialized");
            return;
        }

        string json = BuildJson(message) + "\n";
        byte[] data = Encoding.UTF8.GetBytes(json);
        
        try
        {
            _udp.Send(data, data.Length, _remoteEndPoint);
            Debug.Log($"Sent to Pi: {json.Trim()}");

            // Try to receive a reply (optional)
            IPEndPoint any = new IPEndPoint(IPAddress.Any, 0);
            byte[] reply = _udp.Receive(ref any);
            string response = Encoding.UTF8.GetString(reply).Trim();
            Debug.Log($"Server reply: {response}");
            
            // Notify subscribers
            OnServerResponse?.Invoke(response);
        }
        catch (SocketException ex)
        {
            Debug.LogWarning($"UDP_Comms network error: {ex.Message}");
            OnConnectionError?.Invoke($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"UDP_Comms unexpected error: {ex}");
            OnConnectionError?.Invoke($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Check if the UDP connection is available
    /// </summary>
    public bool IsConnected()
    {
        return _udp != null && _udp.Client != null && _udp.Client.Connected;
    }

    /// <summary>
    /// Minimal JSON builder (no external libs needed). Works for our simple message shapes.
    /// </summary>
    private static string BuildJson(Dictionary<string, object> dict)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append('{');
        bool first = true;
        foreach (var kv in dict)
        {
            if (!first) sb.Append(',');
            first = false;
            sb.Append('"').Append(Escape(kv.Key)).Append("\":");
            switch (kv.Value)
            {
                case string s:
                    sb.Append('"').Append(Escape(s)).Append('"');
                    break;
                case bool b:
                    sb.Append(b ? "true" : "false");
                    break;
                case int i:
                    sb.Append(i);
                    break;
                case float f:
                    sb.Append(f.ToString("0.###"));
                    break;
                default:
                    sb.Append(kv.Value);
                    break;
            }
        }
        sb.Append('}');
        return sb.ToString();
    }

    private static string Escape(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}