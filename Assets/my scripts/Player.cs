using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public struct SpeakerOutput
{
    public float SpeakerId;
    public float Volume;

    public SpeakerOutput(float id, float vol)
    {
        SpeakerId = id;
        Volume = vol;
    }
}

public class Player : MonoBehaviour
{
    // ---------------- UDP to Raspberry Pi ----------------
    [Header("Raspberry Pi Server")]
    public string serverIP = "192.168.1.3";
    public int serverPort = 5005;
    public int timeoutMs = 200; // small so misuses don't hang; we don't await replies anyway

    [Header("Haptics Control")]
    public bool enableProximityHaptics = true;

    private UdpClient _udp;
    private IPEndPoint _remoteEndPoint;
    public gamemanager gameManager; // Reference to GameManager for experiment control

    // ---------------- Save / Motion ----------------
    public SaveData saveDataScript; // keep: you use this to log events
    public float velVal = 0f;

    //public float speed = 20f; // only used if you re-enable Rigidbody movement
    // not needed for VR 
    // private Rigidbody rb;
    // private string playerfile;

    // for VR locomotion velocity (CharacterController)
    //private CharacterController characterController;

    // ---------------- Audio / Haptics ----------------
    [Header("Audio Files for Raspberry Pi")]
    public string triggerSoundFile = "collisionglass.wav"; //"3D PHYSX WOOD CUBE impact carpet heavy os 12.wav";
    public string proximitySoundFile = "hum.wav";
    public string hapticsignal = "TONE 200Hz 441.wav"; //"3D PHYSX WOOD CUBE impact carpet heavy os 12.wav";

    // Speaker layout (center angles in degrees)
    private readonly float[] speakerCenterAngles = new float[]
    {
        

     45f,  // Speaker 1 (Front-Right)
     90f,  // Speaker 2 (Right)
     135f, // Speaker 3 (Back-Right)
     225f, // Speaker 4 (Back-Left)
     270f, // Speaker 5 (Left)
     315f  // Speaker 6 (Front-Left)
    };

    // ---------------- Proximity ----------------
    [Header("Proximity Haptics")]
    public float proximityRange = 3.00f;      // meters
    public float maxProximityVolume = 100f;    // 0-100
    private GameObject[] proximityObstacles;
    private GameObject closestObstacle;
    private GameObject secondClosestObstacle;
    private float closestDistance = float.MaxValue;
    private float secondClosestDistance = float.MaxValue;

    private Vector3 previousPos = new Vector3(0f,0f,0f); 

    void Awake()
    {
       if (!IPAddress.TryParse(serverIP, out var ip))
        {
            Debug.LogError($"Invalid server IP: {serverIP}");
            enabled = false;
            return;
        }

        _udp = new UdpClient();
        _udp.Client.ReceiveTimeout = timeoutMs;
        _remoteEndPoint = new IPEndPoint(ip, serverPort);

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<gamemanager>();
        }
        if (saveDataScript == null)
        {
            saveDataScript = FindObjectOfType<SaveData>();
        }

        try
        {
            _udp = new UdpClient();
            _udp.Client.SendTimeout = timeoutMs;
            _udp.Client.ReceiveTimeout = timeoutMs;
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            Debug.Log($"UDP Client connected to {serverIP}:{serverPort}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize UDP client: {e.Message}");
            _udp = null;
        }
    }
    void OnDestroy()
    {
        try { _udp?.Close(); } catch { /* ignore */ }
    }

    // --- SEND UDP ---
    // private void SendUDPMessage(string message)
    // {
    //     try
    //     {
    //         byte[] data = Encoding.UTF8.GetBytes(message);
    //         _udp.Send(data, data.Length, _remoteEndPoint);
    //         Debug.Log($"[UDP] Sent: {message}");
    //     }
    //     catch (System.Exception e)
    //     {
    //         Debug.LogError($"[UDP ERROR] {e}");
    //     }
    // }




    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        //   Debug.Log($"Proximity range set to: {proximityRange}"); // Debug proximity range

       //characterController = GetComponent<CharacterController>();
        // if (characterController == null)
        // {
        //     Debug.LogError("CharacterController component not found on the player GameObject. VR locomotion velocity cannot be calculated.");
        //     enabled = false;
        //     return;
        // }

        //Find all GameObjects with the "trigger" tag
        proximityObstacles = GameObject.FindGameObjectsWithTag("trigger");
        Debug.Log($"Found {proximityObstacles.Length} proximity obstacles in Start()");
   
        
    }

//     private IEnumerator SpeakerTest()
// {
//     Debug.Log("Starting speaker test sequence...");
//     for (int speaker = 1; speaker <= 6; speaker++)
//     {
//         Play(hapticsignal, channel: speaker, gain: 1.0f, loop: false);
//         yield return new WaitForSeconds(0.5f);
//     }
//     yield return new WaitForSeconds(1.0f);
//     for (int speaker = 1; speaker <= 6; speaker++)
//     {
//         Play(hapticsignal, channel: speaker, gain: 1.0f, loop: false);
//     }
//     Debug.Log("Speaker test sequence completed.");
// }

     void Update()
    {
        // velVal = rb != null ? rb.velocity.magnitude : 0f;
        // velVal = characterController.velocity.magnitude;
        velVal = ((transform.position - previousPos) / Time.deltaTime).magnitude;
        previousPos = transform.position;

        if (enableProximityHaptics)
        {
            CheckProximityHaptics();
        }
        // (Kept for reference — keyboard locomotion)
        // if (Input.GetKey(KeyCode.W)) rb.AddForce(0, 0, 1f * speed);
        // if (Input.GetKey(KeyCode.S)) rb.AddForce(0, 0, -1f * speed);
        // if (Input.GetKey(KeyCode.A)) rb.AddForce(-1f * speed, 0, 0);
        // if (Input.GetKey(KeyCode.D)) rb.AddForce(1f * speed, 0, 0);
    }

    // ---------------- Public JSON API (compatible with your Pi server) ----------------
    // NOTE: We do NOT block waiting for replies to avoid frame hitches.
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

    // ---------------- Internals: JSON + UDP ----------------
    private static string Escape(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "\\\"");

    /// Minimal JSON builder for simple shapes (string/int/float/bool only).
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

    /// Fire-and-forget UDP send. Adds trailing newline (server expects it).
    private void Send(Dictionary<string, object> message)
    {
        string json = BuildJson(message) + "\n";
        byte[] data = Encoding.UTF8.GetBytes(json);

        try
        {
            _udp.Send(data, data.Length, _remoteEndPoint);
            // We do NOT call Receive() here — avoids blocking the main thread.
            // If you ever need replies, add a separate async listener.
            // Debug.Log($"[UDP → Pi] {json.Trim()}");
        }
        catch (SocketException ex)
        {
            Debug.LogWarning($"HiFiClient UDP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"HiFiClient unexpected error: {ex}");
        }
    }

    // ---------------- Collision (impact) ----------------
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "trigger")
        {
            Play(triggerSoundFile, channel: 1, gain: 0.9f, loop: false);
            // Debug.LogError("Hit happened"); 

        }
        Vector3 playerPos = transform.position;
        Vector3 playerForward = transform.forward;
        Vector3 triggerPos = other.ClosestPoint(playerPos);

        Vector3 toTrigger = (triggerPos - playerPos);
        toTrigger.y = 0f;
        playerForward.y = 0f;

        float angle = Vector3.SignedAngle(playerForward, toTrigger, Vector3.up);
        if (angle < 0) angle += 360f; // convert to 0–360
        // Scale the player's velocity ('velVal') to a 0-100 range for impact intensity.
      
        float impactVolume = Mathf.Clamp(velVal * 1f, 0, 100);

        // Get the weighted output for the two most relevant speakers based on the collision angle and volume.
        (SpeakerOutput s1, SpeakerOutput s2) = GetWeightedSpeakerOutputs(angle, impactVolume);


        // // The Arduino will interpret SpeakerId with a fractional part (e.g., 1.001) as an impact.
        // SendHapticOutput(new SpeakerOutput(s1.SpeakerId + 0.001f, s1.Volume), new SpeakerOutput(s2.SpeakerId + 0.001f, s2.Volume));

        // Create a UDP payload for Raspberry Pi
        
        SendHapticOutputToRaspberryPi(s1, s2, triggerSoundFile, isCollision: true);


        if (saveDataScript != null)
        {
            // Assuming 'other' has a unique index or tag
            int obstacleIndex = other.gameObject.GetInstanceID(); // Example: use a unique ID
            Vector3 obstaclePosition = other.transform.position;
            float width = other.bounds.size.x; 
            float depth = other.bounds.size.z; 

            saveDataScript.LogCollisionEvent(obstacleIndex, obstaclePosition, angle, width, depth, velVal, s1, s2);
        }

        Debug.Log($"Player collided with {other.gameObject.name}) → Angle: {angle:F1}° -> S{s1.SpeakerId}: {s1.Volume:F0}%, S{s2.SpeakerId}: {s2.Volume:F0}% Velocity: {velVal}");
        // SphereManager.Instance.TryCatchSphere(other.gameObject);

    }


    void OnTriggerExit(Collider other)
    {

        Debug.Log($"Player exited {other.gameObject.name} ");
        //SendHapticOutput(new SpeakerOutput(10, 0), new SpeakerOutput(0, 0));

        
        // // Send stop signal to Pi
        // SendUDPMessage("STOP_ALL");
    }


    private (SpeakerOutput s1, SpeakerOutput s2) GetWeightedSpeakerOutputs(float angle, float baseVolume)
    {
        // Normalize angle to 0-360 range.
        angle = (angle % 360 + 360) % 360;


        int speaker1Index = -1; // Index of the closest speaker 
        int speaker2Index = -1; // Index of the second closest speaker 
        float minAngleDiff1 = 360f; // Smallest angular difference
        float minAngleDiff2 = 360f; // Second smallest angular difference 


        for (int i = 0; i < speakerCenterAngles.Length; i++)
        {
            float centerAngle = speakerCenterAngles[i];

            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(angle, centerAngle));

            if (angleDiff < minAngleDiff1)
            {

                minAngleDiff2 = minAngleDiff1;
                speaker2Index = speaker1Index;
                minAngleDiff1 = angleDiff;
                speaker1Index = i;
            }
            else if (angleDiff < minAngleDiff2)
            {
                // If this speaker is closer than the current second closest, update it.
                minAngleDiff2 = angleDiff;
                speaker2Index = i;
            }
        }


        float blendFactor = 0f;
        if (speaker1Index != -1 && speaker2Index != -1) // Ensure two speakers were found
        {

            if ((minAngleDiff1 + minAngleDiff2) > 0.001f)
            {

                blendFactor = Mathf.Clamp01(minAngleDiff1 / (minAngleDiff1 + minAngleDiff2));
            }
        }


        float vol1 = baseVolume * (1f - blendFactor); // Volume for the closest speaker
        float vol2 = baseVolume * blendFactor;        // Volume for the second closest speaker


        return (new SpeakerOutput(speaker1Index + 1, vol1), new SpeakerOutput(speaker2Index + 1, vol2));
    }
    


    public void SetProximityHapticsEnabled(bool enabled)
    {
        enableProximityHaptics = enabled;
        Debug.Log($"Proximity haptics are now: {(enabled ? "ENABLED" : "DISABLED")}");
        
        // Stop all haptics if they are being disabled to prevent them from lingering
        if (!enabled)
        {
            StopAll();
        }
    }

    

    private void CheckProximityHaptics()
    {
        GameObject[] currentObstacles = GameObject.FindGameObjectsWithTag("trigger");

        if (currentObstacles == null || currentObstacles.Length == 0)
        {
            return;
        }


        float[] combinedSpeakerVolumes = new float[speakerCenterAngles.Length];
        bool proximityEventActive = false;

        closestObstacle = null;
        secondClosestObstacle = null;
        closestDistance = float.MaxValue;
        secondClosestDistance = float.MaxValue;

        foreach (GameObject obstacle in currentObstacles)
        {
            if (obstacle == null) continue;

            float distance = Vector3.Distance(transform.position, obstacle.transform.position);


            // Debug.Log($"DISTANCE CHECK: {obstacle.name} is {distance:F2}m away (threshold: {proximityRange:F2}m)");
            // Debug.Log($"COMPARISON: {distance} <= {proximityRange} = {distance <= proximityRange}");

            if (distance < closestDistance)
            {

                secondClosestDistance = closestDistance;
                secondClosestObstacle = closestObstacle;


                closestDistance = distance;
                closestObstacle = obstacle;
            }
            else if (distance < secondClosestDistance)
            {
                secondClosestDistance = distance;
                secondClosestObstacle = obstacle;
            }

            if (distance <= proximityRange)
            {
                // Debug.Log($"Proximity Alert! Player is {distance:F2}m from {obstacle.name}");
                proximityEventActive = true;

                Vector3 playerPos = transform.position;
                Vector3 playerForward = transform.forward;
                Vector3 obstaclePos = obstacle.transform.position;

                Vector3 toObstacle = (obstaclePos - playerPos);
                toObstacle.y = 0f;
                playerForward.y = 0f;

                float angle = Vector3.SignedAngle(playerForward, toObstacle, Vector3.up);
                if (angle < 0) angle += 360f;

                // Scale the volume based on distance. Closer means higher volume.
                float scaledVolume = Mathf.Lerp(maxProximityVolume, 0, distance / proximityRange);

                // Get the weighted output for this specific obstacle
                (SpeakerOutput s1, SpeakerOutput s2) = GetWeightedSpeakerOutputs(angle, scaledVolume);

                // Add the volumes to our combined array.
                // We use s1.SpeakerId - 1 to get the correct array index.
                if ((int)s1.SpeakerId - 1 >= 0 && (int)s1.SpeakerId - 1 < combinedSpeakerVolumes.Length)
                {
                    combinedSpeakerVolumes[(int)s1.SpeakerId - 1] += s1.Volume;
                }
                if ((int)s2.SpeakerId - 1 >= 0 && (int)s2.SpeakerId - 1 < combinedSpeakerVolumes.Length)
                {
                    combinedSpeakerVolumes[(int)s2.SpeakerId - 1] += s2.Volume;
                }
            }

        }

        // After checking all obstacles, find the two speakers with the highest volume and send a single command.
        if (proximityEventActive)
        {
            float maxVolume1 = 0;
            float maxVolume2 = 0;
            int speakerId1 = 0;
            int speakerId2 = 0;

            for (int i = 0; i < combinedSpeakerVolumes.Length; i++)
            {
                if (combinedSpeakerVolumes[i] > maxVolume1)
                {
                    maxVolume2 = maxVolume1;
                    speakerId2 = speakerId1;
                    maxVolume1 = combinedSpeakerVolumes[i];
                    speakerId1 = i + 1;
                }
                else if (combinedSpeakerVolumes[i] > maxVolume2)
                {
                    maxVolume2 = combinedSpeakerVolumes[i];
                    speakerId2 = i + 1;
                }
            }

            float combinedVol1 = Mathf.Min(maxVolume1, maxProximityVolume);
            float combinedVol2 = Mathf.Min(maxVolume2, maxProximityVolume);

            // SendHapticOutput(new SpeakerOutput(10, 0), new SpeakerOutput(0, 0));

            // Send proximity haptic output to Raspberry Pi
            SendHapticOutputToRaspberryPi(
                new SpeakerOutput(speakerId1, combinedVol1),
                new SpeakerOutput(speakerId2, combinedVol2),
                proximitySoundFile,
                isCollision: false
            );

            if (saveDataScript != null && closestObstacle != null && secondClosestObstacle != null)
            {
                float closestAngle = GetAngleToObstacle(closestObstacle);
                float closestScaledVolume = Mathf.Lerp(maxProximityVolume, 0, closestDistance / proximityRange);
                (SpeakerOutput closestS1, SpeakerOutput closestS2) = GetWeightedSpeakerOutputs(closestAngle, closestScaledVolume);

                float secondClosestAngle = GetAngleToObstacle(secondClosestObstacle);
                float secondClosestScaledVolume = Mathf.Lerp(maxProximityVolume, 0, secondClosestDistance / proximityRange);
                (SpeakerOutput secondClosestS1, SpeakerOutput secondClosestS2) = GetWeightedSpeakerOutputs(secondClosestAngle, secondClosestScaledVolume);

                saveDataScript.LogProximityEvent(
                    closestObstacle.GetInstanceID(),
                    closestObstacle.transform.position,
                    closestAngle,
                    closestObstacle.GetComponent<Collider>().bounds.size.x,
                    closestObstacle.GetComponent<Collider>().bounds.size.z,
                    closestDistance,
                    closestS1,
                    closestS2,
                    secondClosestObstacle.GetInstanceID(),
                    secondClosestObstacle.transform.position,
                    secondClosestAngle,
                    secondClosestObstacle.GetComponent<Collider>().bounds.size.x,
                    secondClosestObstacle.GetComponent<Collider>().bounds.size.z,
                    secondClosestDistance,
                    secondClosestS1,
                    secondClosestS2
                );
            }
        }
    }
    // This is a helper method to calculate the angle to an obstacle
   private float GetAngleToObstacle(GameObject obstacle)
    {
        Vector3 playerPos = transform.position;
        Vector3 playerForward = transform.forward;
        Vector3 obstaclePos = obstacle.transform.position;

        Vector3 toObstacle = (obstaclePos - playerPos);
        toObstacle.y = 0f;
        playerForward.y = 0f;

        float angle = Vector3.SignedAngle(playerForward, toObstacle, Vector3.up);
        if (angle < 0) angle += 360f;

        return angle;
    }
     // Send haptic output using the SAME JSON protocol as your working UDP client.
    private void SendHapticOutputToRaspberryPi(SpeakerOutput s1, SpeakerOutput s2, string audioFile, bool isCollision)
    {
       

        // Convert 0–100 to 0–1 and enforce a small floor
        const float MIN_IMPACT_GAIN = 0.5f; // tweaked
        float normalizedVol1 = Mathf.Max(MIN_IMPACT_GAIN, Mathf.Clamp01(s1.Volume / 100f));
        float normalizedVol2 = Mathf.Max(MIN_IMPACT_GAIN, Mathf.Clamp01(s2.Volume / 100f));


        // Primary channel
        if (normalizedVol1 > 0.01f && s1.SpeakerId >= 1 && s1.SpeakerId <= 6)
        {
            int ch1 = Mathf.RoundToInt(s1.SpeakerId);
            Play(audioFile, ch1, normalizedVol1, loop: false);
            Debug.Log($"Pi Audio: {audioFile} on channel {ch1} at volume {normalizedVol1:F2} ({s1.Volume:F0}%)");
        }

        // Secondary channel (avoid duplicate if same speaker)
        if (normalizedVol2 > 0.01f && s2.SpeakerId >= 1 && s2.SpeakerId <= 6 && s2.SpeakerId != s1.SpeakerId)
        {
            int ch2 = Mathf.RoundToInt(s2.SpeakerId);
            Play(audioFile, ch2, normalizedVol2, loop: false);
            Debug.Log($"Pi Audio: {audioFile} on channel {ch2} at volume {normalizedVol2:F2} ({s2.Volume:F0}%)");
        }

        if (isCollision)
            Debug.Log($"Collision haptic sent - Primary: Ch{s1.SpeakerId}@{s1.Volume:F0}%, Secondary: Ch{s2.SpeakerId}@{s2.Volume:F0}%");
        else
            Debug.Log($"Proximity haptic sent - Primary: Ch{s1.SpeakerId}@{s1.Volume:F0}%, Secondary: Ch{s2.SpeakerId}@{s2.Volume:F0}%");
    }


}
    

   