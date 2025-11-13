using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class gamemanager : MonoBehaviour
{
   
   // Public references to your other scripts
    public Player playerScript;
    public SaveData saveDataScript; // Re-enabled this reference
    public GameObject Canvas;
    public GameObject lastcanvas;
    
    // Reference to SphereTeleporter
    public sphereandcube sphereTeleporterScript;
    
    // Input action for continue button
    public InputAction continueAction;

    [Header("Experiment Settings")]
    public int[] conditionOrder = { 2, 3, 1 };
    public int totalTrialsPerCondition = 1; 

    // Public flags to signal when the player is ready to continue
    public bool isReadyToContinue = false;
    
    // --- FLAG MODIFICATION ---
    // 'isRecordingData' is ONLY for saving data
    public bool isRecordingData = false;
    // 'isExperimentRunning' is for keeping scripts like Player.cs active
    public bool isExperimentRunning = false;
    // --- END MODIFICATION ---

    private bool isTrialComplete = false; 
    private int currentCondition = 1;
    private int currentTrial = 1;

    void Start()
    {
        if (continueAction != null)
        {
            continueAction.Enable();
            continueAction.performed += OnContinuePressed;
        }
        
        // --- NEW ---
        // Set the main experiment flag to true
        isExperimentRunning = true; 
        // --- END NEW ---
        
        StartCoroutine(RunExperiment());
    }

    void OnDestroy()
    {
        if (continueAction != null)
        {
            continueAction.performed -= OnContinuePressed;
        }
    }

    // Called when the continue button ('A') is pressed
    private void OnContinuePressed(InputAction.CallbackContext context)
    {
        // This flag is used by the StartBreak coroutine
        if (!isReadyToContinue) 
        {
            isReadyToContinue = true;
            Debug.Log("A button pressed - ready for next trial/end");
        }
        
        // --- ADDED ---
        // Also allow 'A' to hide the final canvas
        if (lastcanvas != null && lastcanvas.activeInHierarchy)
        {
            lastcanvas.SetActive(false);
            Debug.Log("A button pressed - hiding final canvas");
        }
        // --- END ADDED ---
    }

    private IEnumerator RunExperiment()
    {
        // Iterate through the custom defined condition order (e.g., 2, 3, 1)
        for (int i = 0; i < conditionOrder.Length; i++)
        {
            currentCondition = conditionOrder[i];
            
            // Apply haptics settings based on the current condition
            if (playerScript != null)
            {
                if (currentCondition == 1 || currentCondition == 2)
                {
                    playerScript.SetProximityHapticsEnabled(false);
                }
                else // This will be for currentCondition == 3
                {
                    playerScript.SetProximityHapticsEnabled(true);
                }
            }


            for (currentTrial = 1; currentTrial <= totalTrialsPerCondition; currentTrial++)
            {
                Debug.Log($"Starting Condition {currentCondition} (Order #{i + 1}), Trial {currentTrial}");

                isReadyToContinue = false;
                isTrialComplete = false; 

                // Stop recording data during the break
                isRecordingData = false;

                // Only show break screen for trials after the very first overall trial.
                if (i > 0 || currentTrial > 1)
                {
                    // This coroutine shows 'Canvas' (Round Complete)
                    yield return StartCoroutine(StartBreak());
                }

                // Start the new trial in sphereandcube
                if (sphereTeleporterScript != null)
                {
                    sphereTeleporterScript.StartNewTrial();
                }

                // Start recording data *after* the break is over
                StartRecording();

                // Wait until sphereandcube script signals completion
                yield return new WaitUntil(() => isTrialComplete);

                // Stop recording and save data
                isRecordingData = false;
                
                if (saveDataScript != null)
                {
                    // Pass 'i + 1' as the sequence order
                    saveDataScript.SaveAndClearData(currentCondition, currentTrial); 
                }

                Debug.Log($"Completed Condition {currentCondition} (Order #{i + 1}), Trial {currentTrial}");
            }
        }

        Debug.Log("Experiment complete!");
        // This will appear after the final (3rd) condition is done
        if (lastcanvas != null)
        {
            lastcanvas.SetActive(true);
        }
        
        // --- NEW ---
        // Set the main experiment flag to false
        isExperimentRunning = false; 
        // --- END NEW ---
    }
    
    
    public void StartRecording()
    {
        isRecordingData = true;
        // You may also want to reset your start time in SaveData here
        // saveDataScript.startTime = Time.time;
        Debug.Log("Data recording has started.");
    }

    // Public method for sphereandcube to call when the trial is finished
    public void SetTrialCompleted()
    {
        isTrialComplete = true;
    }
    

public void TriggerContinue()
{
    if (!isReadyToContinue)
    {
        isReadyToContinue = true;
        Debug.Log("Continue triggered via physical button.");
    }

    if (lastcanvas != null && lastcanvas.activeInHierarchy)
    {
        lastcanvas.SetActive(false);
    }
}


    // This coroutine handles the break
    private IEnumerator StartBreak()
    {
        Debug.Log("Condition complete. Take a break. Press the continue button to proceed.");

        // Activate the canvas
        if (Canvas != null)
        {
            Canvas.SetActive(true);
        }

        // Wait until the player is ready (presses 'A')
        yield return new WaitUntil(() => isReadyToContinue);

        // Deactivate the canvas
        if (Canvas != null)
        {
            Canvas.SetActive(false);
        }

        Debug.Log("Break over. Proceeding with the next trial.");
    }

    // --- ADDED ---
    // Public getter for other scripts to read the current condition
    public int GetCurrentCondition()
    {
        return currentCondition;
    }
    // --- END ADDED ---
} 


