using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 
using System.Globalization; 

public class sphereandcube : MonoBehaviour
{
    [Header("Game Objects")]
    public Transform player;
    public Transform sphere;
    public Transform[] points;
    public GameObject gameOverCanvas;
    public GameObject roundOverCanvas;

    [Header("Audio")]
    public AudioClip catchSound;
    //public AudioClip gameOverSound;
    private AudioSource audioSource;

    [Header("Script References")]
    public SaveData saveDataScript; 
    public gamemanager gamemanager;

    // --- ACTIVATED ---
    [Header("Trigger Cubes")]
    private List<GameObject> triggerObjects = new List<GameObject>();

    public int currentPointIndex = 0;
    private int roundCount = 0;
    private int currentArrayIndex = 0;

    // Track if sphere should be visible
    private bool sphereActive = true;
    
    // This will hold all the layout arrays loaded from CSV
    private Vector3[][] allTriggerPositions;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (saveDataScript == null)
        {
           saveDataScript = FindObjectOfType<SaveData>();
        }


        if (gamemanager == null)
        {
            gamemanager = FindObjectOfType<gamemanager>();
        }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        if (roundOverCanvas != null)
        {
            roundOverCanvas.SetActive(false);
        }
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned in sphereandcube script!");
        }

  
  
        // Load all layouts from CSV files 
        LoadAllLayouts();
        
        if (allTriggerPositions == null || allTriggerPositions.Length == 0)
        {
            Debug.LogError("No trigger layouts were loaded! Make sure CSV files are in the 'Assets/layouts' folder.");
            return; 
        }



  
        // Find all trigger cubes in the scene
        GameObject[] foundTriggers = GameObject.FindGameObjectsWithTag("trigger");
        triggerObjects.AddRange(foundTriggers);
        Debug.Log($"Found {triggerObjects.Count} trigger objects");

        // Assign the first layout
        AssignTriggerPositions(currentArrayIndex);
        // --- END OF ADDED CODE ---


        // Show the sphere at the first point for the initial trial
        if (points.Length > 0)
        {
            sphere.transform.position = points[currentPointIndex].position;
            sphereActive = true;
            Debug.Log($"Starting at point {currentPointIndex}: {points[currentPointIndex].position}");
        }
    }
    
    // --- NormalizeYPositions() and Awake() methods have been REMOVED ---
    // The Y-coordinate is now normalized to 0.33f during the CSV loading process.

    void Update()
    {
        // Only check for sphere collision if sphere is active
        if (sphereActive)
        {
            float distance = Vector3.Distance(player.transform.position, sphere.transform.position);
            if (distance <= 2f)
            {
                OnSphereReached();
            }
        }
    }

    void OnSphereReached()
    {
        if (catchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(catchSound);
            Debug.Log("Playing catch sound!");
        }
        else
        {
            Debug.LogWarning("Catch sound or AudioSource not set!");
        }

    
        
        
        currentPointIndex++;
        roundCount++;
        
        if (saveDataScript != null)
        {
            saveDataScript.LogSphereReachedEvent(
                roundCount - 1,       // Pass the 0-indexed round we just finished
                currentPointIndex - 1, // Pass the 0-indexed sphere we just reached
                sphere.position,
                currentArrayIndex + 1  // Pass the 1-indexed layout number
            );
        }
        

        Debug.Log($"Sphere reached! Moving to point {currentPointIndex}. Round: {roundCount}");
        
        // --- MODIFIED ---
        // Logic to change cube layout every round
        if (roundCount > 0 && allTriggerPositions.Length > 0)
        {
            // This increments the layout index and wraps it around if it reaches the end
            currentArrayIndex = (currentArrayIndex + 1) % allTriggerPositions.Length;
            AssignTriggerPositions(currentArrayIndex);
            Debug.Log($"Updated trigger positions to layout {currentArrayIndex + 1}");
        }
        // --- END OF MODIFIED CODE ---


        if (currentPointIndex >= points.Length)
        {
            Debug.Log("All spheres reached for this trial.");

            // Hide the sphere when trial is complete
            HideSphere();
            
            // Activate the correct canvas based on the condition
            if (gamemanager != null)
            {
                gamemanager.SetTrialCompleted();
            }
            
            // Signal the GameManager that the trial is complete
            if (gamemanager != null)
            {
                gamemanager.SetTrialCompleted();
            }
            return;
        }

        sphere.transform.position = points[currentPointIndex].position;
        Debug.Log($"Moved sphere to point {currentPointIndex}: {points[currentPointIndex].position}");
    }

    // Method to hide the sphere
    private void HideSphere()
    {
        sphereActive = false;
        if (sphere != null)
        {
            sphere.gameObject.SetActive(false);
        }
        Debug.Log("Sphere hidden - trial complete");
    }

    // Public method for GameManager to call when starting a new trial
    public void StartNewTrial()
    {
        // Reset counters for new trial
        currentPointIndex = 0;
        roundCount = 0;

        // --- ADDED ---
        currentArrayIndex = 0; // Reset to the first layout
        if (allTriggerPositions != null && allTriggerPositions.Length > 0)
        {
            AssignTriggerPositions(currentArrayIndex); // Assign the first layout
        }
        // --- END OF ADDED CODE ---
        
        // Show the sphere at the first point (like a new sphere appearing)
        ShowSphere();
        
        // Hide game over canvas
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

    // Method to show the sphere (like spawning a new one)
    private void ShowSphere()
    {
        sphereActive = true;
        if (sphere != null)
        {
            sphere.gameObject.SetActive(true);
            if (points.Length > 0)
            {
                sphere.transform.position = points[currentPointIndex].position;
                Debug.Log($"New sphere appeared at point {currentPointIndex}: {points[currentPointIndex].position}");
            }
        }
    }

    // --- NEW METHOD to load all layouts from CSV files ---
    void LoadAllLayouts()
    {
        List<Vector3[]> layouts = new List<Vector3[]>();
        
       
        string folderPath = Path.Combine(Application.dataPath, "layouts");

        // --- MODIFIED to loop to 15 ---
        // Loop from 1 to 15 to load TriggerPositions_1.csv to TriggerPositions_15.csv
        for (int i = 1; i <= 15; i++) 
        {
            string fileName = $"TriggerPositions_{i}.csv";
            string filePath = Path.Combine(folderPath, fileName);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"Could not find layout file: {filePath}. Skipping. (Make sure it's in 'Assets/layouts')");
                continue;
            }

            string csvText;
            try
            {
                csvText = File.ReadAllText(filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to read file {filePath}. Error: {e.Message}");
                continue;
            }

            List<Vector3> positions = new List<Vector3>();
            // Split by newlines, removing empty entries
            string[] lines = csvText.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            // Start from 1 to skip the header row (ObjectName,PosX,PosY,PosZ)
            for (int j = 1; j < lines.Length; j++)
            {
                // ... (rest of the parsing logic is the same) ...
                string line = lines[j].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] columns = line.Split(',');
                
                // Ensure we have enough columns (at least 4 for ObjectName,x,y,z)
                if (columns.Length >= 4)
                {
                    try
                    {
                        // Parse X from columns[1]
                        float x = float.Parse(columns[1], CultureInfo.InvariantCulture);
                        

                        float y = 0.33f; // Apply fixed Y as per original script's NormalizeYPositions
                        
                        // Parse Z from columns[3]
                        float z = float.Parse(columns[3], CultureInfo.InvariantCulture);
                        
                        positions.Add(new Vector3(x, y, z));
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"Failed to parse line {j} in {fileName}: \"{lines[j]}\". Error: {e.Message}");
                    }
                }
            }
            
            if (positions.Count > 0)
            {
                layouts.Add(positions.ToArray());
                Debug.Log($"Loaded layout {i} from {fileName} with {positions.Count} positions.");
            }
        }


        // Convert the List to the array our script uses
        allTriggerPositions = layouts.ToArray();
        Debug.Log($"Loaded a total of {allTriggerPositions.Length} layouts.");
    }
    
    // --- UN-COMMENTED (This function is unchanged, it now just uses the loaded data) ---
    void AssignTriggerPositions(int arrayIndex)
    {
        if (triggerObjects.Count == 0)
        {
            Debug.LogWarning("No trigger objects found to position!");
            return;
        }

        // Ensure the arrayIndex is valid
        if (arrayIndex < 0 || arrayIndex >= allTriggerPositions.Length)
        {
            Debug.LogError($"Invalid layout index: {arrayIndex}. Cannot assign positions.");
            return;
        }

        Vector3[] selectedPositions = allTriggerPositions[arrayIndex];

        for (int i = 0; i < triggerObjects.Count; i++)
        {
            if (triggerObjects[i] != null)
            {
                // --- MODIFIED to also re-activate objects ---
                if (i < selectedPositions.Length)
                {
                    triggerObjects[i].SetActive(true); // Make sure it's active
                    triggerObjects[i].transform.position = selectedPositions[i];
                }
                else
                {
                    // If you have more trigger objects than positions, hide the extra ones
                    // or assign to last known position. Hiding is safer.
                    triggerObjects[i].SetActive(false);
                    Debug.LogWarning($"Not enough positions in layout {arrayIndex + 1} for trigger object {i}. Hiding object.");
                }
            }
        }

        Debug.Log($"Assigned {triggerObjects.Count} trigger objects to positions from layout {arrayIndex + 1}");
    }
    // --- END OF UN-COMMENTED SECTION ---

    public int GetCurrentRound()
    {
        return roundCount;
    }

    public int GetCurrentArrayIndex()
    {
        return currentArrayIndex + 1;
    }
}