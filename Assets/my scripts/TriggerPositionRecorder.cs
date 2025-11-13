using UnityEngine;
using System.IO;         // Required for file writing
using System.Text;       // Required for StringBuilder
using System;           // Required for DateTime
using System.Globalization; // Required for InvariantCulture (to force '.' as decimal)

#if UNITY_EDITOR
using UnityEditor;     // Required for opening the folder
#endif

/// <summary>
/// A utility script that runs once on Start().
/// It finds all GameObjects in the scene tagged with "trigger".
/// It then saves their names and positions to a timestamped CSV file.
/// It ALSO logs the C# array format to the console for easy copy-pasting
/// into a ScriptableObject or other script.
/// </summary>
public class TriggerPositionRecorder : MonoBehaviour
{
    void Start()
    {
        // 1. Find all GameObjects with the "trigger" tag
        GameObject[] triggerObjects = GameObject.FindGameObjectsWithTag("trigger");

        if (triggerObjects == null || triggerObjects.Length == 0)
        {
            Debug.LogWarning("TriggerPositionRecorder: No GameObjects found with tag 'trigger'. No file saved.");
            return;
        }

        Debug.Log($"TriggerPositionRecorder: Found {triggerObjects.Length} trigger objects.");

        // We will build two strings: one for the CSV and one for C# code
        StringBuilder csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("ObjectName,PosX,PosY,PosZ"); // CSV Header

        StringBuilder csharpBuilder = new StringBuilder();
        csharpBuilder.AppendLine("// --- C# Array for ScriptableObject (Copy from here) ---");
        csharpBuilder.AppendLine("new Vector3[]");
        csharpBuilder.AppendLine("{");

        // 2. Loop through all found objects and format their data
        foreach (GameObject obj in triggerObjects)
        {
            Vector3 pos = obj.transform.position;
            string objectName = obj.name;

            // Add to CSV string
            // Using InvariantCulture ensures decimals are '.' not ','
            string csvLine = string.Format(CultureInfo.InvariantCulture,
                                        "{0},{1:F3},{2:F3},{3:F3}",
                                        objectName, pos.x, pos.y, pos.z);
            csvBuilder.AppendLine(csvLine);

            // Add to C# string
            // (e.g., "    new Vector3(-0.950f, 0.100f, -0.930f), // trigger_cube_1")
            string csharpLine = string.Format(CultureInfo.InvariantCulture,
                                            "    new Vector3({0:F3}f, {1:F3}f, {2:F3}f), // {3}",
                                            pos.x, pos.y, pos.z, objectName);
            csharpBuilder.AppendLine(csharpLine);
        }

        csharpBuilder.AppendLine("};");
        csharpBuilder.AppendLine("// --- C# Array for ScriptableObject (Copy to here) ---");


        // 3. Define file path and save the CSV
        try
        {
            // Application.persistentDataPath is a safe folder on all platforms
            string folderPath = Path.Combine(Application.persistentDataPath, "TriggerRecordings");
            // Ensure the directory exists
            Directory.CreateDirectory(folderPath);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = $"TriggerPositions_{timestamp}.csv";
            string filePath = Path.Combine(folderPath, fileName);

            File.WriteAllText(filePath, csvBuilder.ToString());

            // 4. Log success and show the user where the file is
            Debug.Log($"TriggerPositionRecorder: Successfully saved CSV to: {filePath}");

            #if UNITY_EDITOR
            // This will open the folder in Windows Explorer / Mac Finder
            EditorUtility.RevealInFinder(filePath);
            #endif
        }
        catch (Exception e)
        {
            Debug.LogError($"TriggerPositionRecorder: Failed to write CSV file. Error: {e.Message}");
        }


        // 5. Log the C# array to the console for easy copy-pasting
        Debug.Log(csharpBuilder.ToString());
        Debug.LogWarning("TriggerPositionRecorder: C# array format for ScriptableObjects was logged above. Copy it from the console!");
    }
}