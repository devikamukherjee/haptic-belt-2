using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphereandcube : MonoBehaviour
{
    [Header("Game Objects")]
    public Transform player;
    public Transform centerEyeAnchor;
    public Transform sphere;
    public Transform[] points;
    public GameObject gameOverCanvas;
    public GameObject roundOverCanvas;

    [Header("Trigger Cube Manager")]
    public TriggerCubeManager cubeManager; // Assign in inspector

    [Header("Audio")]
    public AudioClip catchSound;
    private AudioSource audioSource;

    [Header("Script References")]
    public SaveData saveDataScript; 
    public gamemanager gamemanager;

    [Header("Detection Settings")]
    public float detectionRadius = 0.5f;
    public bool useHeadPosition = true;

    public int currentPointIndex = 0;
    private int roundCount = 0;
    private int currentLayoutNumber = 1; // 1-15 to match TriggerCubeManager

    private bool sphereActive = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (saveDataScript == null)
        {
           saveDataScript = FindFirstObjectByType<SaveData>();
        }

        if (gamemanager == null)
        {
            gamemanager = FindFirstObjectByType<gamemanager>();
        }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        if (roundOverCanvas != null)
        {
            roundOverCanvas.SetActive(false);
        }

        // Auto-find CenterEyeAnchor if not assigned
        if (centerEyeAnchor == null)
        {
            OVRCameraRig cameraRig = FindFirstObjectByType<OVRCameraRig>();
            if (cameraRig != null)
            {
                centerEyeAnchor = cameraRig.centerEyeAnchor;
                Debug.Log("Auto-assigned CenterEyeAnchor from OVRCameraRig");
            }
            else
            {
                Debug.LogError("CenterEyeAnchor not assigned and OVRCameraRig not found!");
            }
        }

        if (player == null && !useHeadPosition)
        {
            Debug.LogError("Player Transform is not assigned in sphereandcube script!");
        }

        // Auto-find TriggerCubeManager if not assigned
        if (cubeManager == null)
        {
            cubeManager = FindFirstObjectByType<TriggerCubeManager>();
            if (cubeManager != null)
            {
                Debug.Log("Auto-found TriggerCubeManager");
            }
            else
            {
                Debug.LogWarning("TriggerCubeManager not found! Cube positioning will not work.");
            }
        }

        // Set initial cube layout
        if (cubeManager != null)
        {
            cubeManager.SetLayout(currentLayoutNumber);
            Debug.Log($"Set initial cube layout to {currentLayoutNumber}");
        }

        // Show the sphere at the first point
        if (points.Length > 0 && sphere != null)
        {
            sphere.position = points[currentPointIndex].position;
            sphereActive = true;
            Debug.Log($"Starting at point {currentPointIndex}: {points[currentPointIndex].position}");
        }
        else
        {
            Debug.LogError("Points array is empty or sphere is not assigned!");
        }
    }

    void Update()
    {
        // Only check for sphere collision if sphere is active
        if (sphereActive && sphere != null)
        {
            Vector3 detectionPos;
            
            if (useHeadPosition && centerEyeAnchor != null)
            {
                detectionPos = centerEyeAnchor.position;
            }
            else if (player != null)
            {
                detectionPos = player.position;
            }
            else
            {
                Debug.LogWarning("No valid position reference for sphere detection!");
                return;
            }

            Vector3 spherePos = sphere.position;

            // Flatten Y coordinate for 2D distance check
            detectionPos.y = 0f;
            spherePos.y = 0f;

            float distance = Vector3.Distance(detectionPos, spherePos);
            
            // Visual debugging in Scene view
            Debug.DrawLine(detectionPos, spherePos, distance <= detectionRadius ? Color.green : Color.yellow);

            if (distance <= detectionRadius)
            {
                OnSphereReached();
            }
        }
    }

    void OnSphereReached()
    {
        // Prevent multiple triggers
        if (!sphereActive) return;
        
        // Play sound
        if (catchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(catchSound);
            Debug.Log("Playing catch sound!");
        }
        else
        {
            Debug.LogWarning("Catch sound or AudioSource not set!");
        }
        
        // Increment counters
        currentPointIndex++;
        roundCount++;
        
        // Log to SaveData
        if (saveDataScript != null)
        {
            saveDataScript.LogSphereReachedEvent(
                roundCount - 1,
                currentPointIndex - 1,
                sphere.position,
                currentLayoutNumber
            );
        }

        Debug.Log($"Sphere reached! Moving to point {currentPointIndex}. Round: {roundCount}");
        
        // Change cube layout every round using TriggerCubeManager
        if (roundCount > 0 && cubeManager != null)
        {
            currentLayoutNumber = (currentLayoutNumber % 15) + 1; // Cycle through 1-15
            cubeManager.SetLayout(currentLayoutNumber);
            Debug.Log($"Updated trigger positions to layout {currentLayoutNumber}");
        }

        // Check if all points reached
        if (currentPointIndex >= points.Length)
        {
            Debug.Log("All spheres reached for this trial.");
            HideSphere();
            
            if (gamemanager != null)
            {
                gamemanager.SetTrialCompleted();
            }
            return;
        }

        // Move sphere to next point
        sphere.position = points[currentPointIndex].position;
        Debug.Log($"Moved sphere to point {currentPointIndex}: {points[currentPointIndex].position}");
    }

    private void HideSphere()
    {
        sphereActive = false;
        if (sphere != null)
        {
            sphere.gameObject.SetActive(false);
        }
        Debug.Log("Sphere hidden - trial complete");
    }

    public void StartNewTrial()
    {
        currentPointIndex = 0;
        roundCount = 0;
        currentLayoutNumber = 1; // Reset to first layout
        
        // Reset cube layout to first layout
        if (cubeManager != null)
        {
            cubeManager.SetLayout(currentLayoutNumber);
            Debug.Log("Reset cubes to layout 1");
        }
        
        ShowSphere();
        
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        if (roundOverCanvas != null)
        {
            roundOverCanvas.SetActive(false);
        }

        Debug.Log("New trial started - fresh sphere appeared at point 1");
    }

    private void ShowSphere()
    {
        sphereActive = true;
        if (sphere != null)
        {
            sphere.gameObject.SetActive(true);
            if (points.Length > 0)
            {
                sphere.position = points[currentPointIndex].position;
                Debug.Log($"New sphere appeared at point {currentPointIndex}: {points[currentPointIndex].position}");
            }
        }
    }

    public int GetCurrentRound()
    {
        return roundCount;
    }

    public int GetCurrentArrayIndex()
    {
        return currentLayoutNumber;
    }
}

