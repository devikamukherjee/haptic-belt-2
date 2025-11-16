using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCubeManager : MonoBehaviour
{
    [Header("Manual Cube Assignment")]
    [Tooltip("Drag all 23 trigger cubes here in the inspector")]
    public GameObject[] triggerCubes = new GameObject[23];

    [Header("Current Layout")]
    [Tooltip("Current layout being displayed (1-15)")]
    public int currentLayout = 1;

    // All 15 layouts hardcoded from your CSV files
    private Vector3[][] allLayouts;

    void Awake()
    {
        InitializeLayouts();
    }

    void Start()
    {
        // Apply the initial layout
        SetLayout(currentLayout);
    }

    void InitializeLayouts()
    {
        allLayouts = new Vector3[15][];

        // Layout 1 - from TriggerPositions_1.csv
        allLayouts[0] = new Vector3[]
        {
            new Vector3(0.408f, 0.330f, 0.090f),    // new Vector3 (20)
            new Vector3(-1.060f, 0.330f, -1.557f),  // new Vector3 (21)
            new Vector3(0.558f, 0.330f, 0.822f),    // new Vector3 (22)
            new Vector3(-0.882f, 0.330f, 1.353f),   // new Vector3 (1)
            new Vector3(-0.104f, 0.330f, -1.056f),  // new Vector3 (19)
            new Vector3(-0.410f, 0.330f, -1.944f),  // new Vector3 (10)
            new Vector3(-1.790f, 0.330f, -1.920f),  // new Vector3 (13)
            new Vector3(-0.332f, 0.330f, -0.267f),  // new Vector3 (4)
            new Vector3(-1.690f, 0.330f, 1.710f),   // new Vector3 (0)
            new Vector3(1.210f, 0.330f, 1.152f),    // new Vector3 (15)
            new Vector3(-0.948f, 0.330f, -1.040f),  // new Vector3 (7)
            new Vector3(1.950f, 0.330f, 0.282f),    // new Vector3 (14)
            new Vector3(0.444f, 0.330f, -1.545f),   // new Vector3 (3)
            new Vector3(-0.126f, 0.330f, 0.972f),   // new Vector3 (6)
            new Vector3(1.770f, 0.330f, -1.020f),   // new Vector3 (16)
            new Vector3(1.250f, 0.330f, -1.662f),   // new Vector3 (11)
            new Vector3(-1.792f, 0.330f, -0.350f),  // new Vector3 (17)
            new Vector3(-0.736f, 0.330f, 0.681f),   // new Vector3 (8)
            new Vector3(0.686f, 0.330f, -0.828f),   // new Vector3 (2)
            new Vector3(0.008f, 0.330f, 2.043f),    // new Vector3 (12)
            new Vector3(-1.058f, 0.330f, -0.021f),  // new Vector3 (5)
            new Vector3(0.740f, 0.330f, 1.503f),    // new Vector3 (9)
            new Vector3(-2.204f, 0.330f, 0.318f)    // new Vector3 (18)
        };

        // Layout 2 - Add your TriggerPositions_2.csv data here
        allLayouts[1] = new Vector3[]
        {
            // TODO: Replace with actual positions from TriggerPositions_2.csv
        new Vector3(-0.738f, 0.330f, 1.809f), // Cube (1)
        new Vector3(1.180f, 0.330f, -1.431f), // Cube (19)
        new Vector3(0.880f, 0.330f, 0.393f),  // Cube (22)
        new Vector3(-0.720f, 0.330f, 0.903f),  // Cube (10)
        new Vector3(0.468f, 0.330f, -2.130f), // Cube (13)
        new Vector3(-0.410f, 0.330f, -1.550f), // Cube (4)
        new Vector3(-1.790f, 0.330f, -1.920f), // Cube (0)
        new Vector3(-0.100f, 0.330f, -0.609f), // Cube (15)
        new Vector3(-1.690f, 0.330f, 1.710f),  // Cube (7)
        new Vector3(1.210f, 0.330f, -0.210f), // Cube (14)
        new Vector3(-1.360f, 0.330f, -1.040f), // Cube (21)
        new Vector3(1.950f, 0.330f, 1.790f),  // Cube (3)
        new Vector3(0.552f, 0.330f, -0.705f), // Cube (6)
        new Vector3(-1.810f, 0.330f, 0.560f),  // Cube (16)
        new Vector3(1.770f, 0.330f, -1.020f), // Cube (11)
        new Vector3(1.590f, 0.330f, 0.810f),  // Cube (17)
        new Vector3(-0.480f, 0.330f, 2.050f),  // Cube (8)
        new Vector3(0.180f, 0.330f, 0.650f),  // Cube (20)
        new Vector3(-0.350f, 0.330f, -0.060f), // Cube (2)
        new Vector3(0.810f, 0.330f, 1.250f),  // Cube (12)
        new Vector3(-1.490f, 0.330f, 0.030f),  // Cube (5)
        new Vector3(0.330f, 0.330f, 1.770f),  // Cube (9)
        new Vector3(1.990f, 0.330f, -0.087f)  // Cube (18)
        };

        // Layout 3 
        allLayouts[2] = new Vector3[]
        {
            // TODO: Replace with actual positions from TriggerPositions_2.csv
        new Vector3(-0.738f, 0.330f, 1.809f), // Cube (1)
        new Vector3(1.180f, 0.330f, -1.431f), // Cube (19)
        new Vector3(0.880f, 0.330f, 0.393f),  // Cube (22)
        new Vector3(-0.720f, 0.330f, 0.903f),  // Cube (10)
        new Vector3(0.468f, 0.330f, -2.130f), // Cube (13)
        new Vector3(-0.410f, 0.330f, -1.550f), // Cube (4)
        new Vector3(-1.790f, 0.330f, -1.920f), // Cube (0)
        new Vector3(-0.100f, 0.330f, -0.609f), // Cube (15)
        new Vector3(-1.690f, 0.330f, 1.710f),  // Cube (7)
        new Vector3(1.210f, 0.330f, -0.210f), // Cube (14)
        new Vector3(-1.360f, 0.330f, -1.040f), // Cube (21)
        new Vector3(1.950f, 0.330f, 1.790f),  // Cube (3)
        new Vector3(0.552f, 0.330f, -0.705f), // Cube (6)
        new Vector3(-1.810f, 0.330f, 0.560f),  // Cube (16)
        new Vector3(1.770f, 0.330f, -1.020f), // Cube (11)
        new Vector3(1.590f, 0.330f, 0.810f),  // Cube (17)
        new Vector3(-0.480f, 0.330f, 2.050f),  // Cube (8)
        new Vector3(0.180f, 0.330f, 0.650f),  // Cube (20)
        new Vector3(-0.350f, 0.330f, -0.060f), // Cube (2)
        new Vector3(0.810f, 0.330f, 1.250f),  // Cube (12)
        new Vector3(-1.490f, 0.330f, 0.030f),  // Cube (5)
        new Vector3(0.330f, 0.330f, 1.770f),  // Cube (9)
        new Vector3(1.990f, 0.330f, -0.087f)  // Cube (18)
        };

        // Layout 4 
        allLayouts[3] = new Vector3[]
        {
        new Vector3(0.448f, 0.330f, -0.459f), // Cube (20)
        new Vector3(-1.060f, 0.330f, -1.557f), // Cube (21)
        new Vector3(0.734f, 0.330f, 0.549f),  // Cube (22)
        new Vector3(-1.254f, 0.330f, 0.180f),  // Cube (1)
        new Vector3(-0.104f, 0.330f, -1.056f), // Cube (19)
        new Vector3(-0.410f, 0.330f, -1.944f), // Cube (10)
        new Vector3(1.282f, 0.330f, -1.431f), // Cube (13)
        new Vector3(-0.332f, 0.330f, -0.267f), // Cube (4)
        new Vector3(-1.690f, 0.330f, 1.710f),  // Cube (0)
        new Vector3(1.444f, 0.330f, 0.135f),  // Cube (15)
        new Vector3(1.576f, 0.330f, -0.573f), // Cube (7)
        new Vector3(2.184f, 0.330f, -1.269f), // Cube (14)
        new Vector3(0.444f, 0.330f, -1.545f), // Cube (3)
        new Vector3(-0.126f, 0.330f, 1.167f),  // Cube (6)
        new Vector3(1.946f, 0.330f, 1.365f),  // Cube (16)
        new Vector3(-2.116f, 0.330f, -1.662f), // Cube (11)
        new Vector3(-1.792f, 0.330f, -0.350f), // Cube (17)
        new Vector3(-0.736f, 0.330f, 0.681f),  // Cube (8)
        new Vector3(-1.566f, 0.330f, 1.008f),  // Cube (2)
        new Vector3(0.322f, 0.330f, 1.710f),  // Cube (12)
        new Vector3(-1.136f, 0.330f, -0.783f), // Cube (5)
        new Vector3(0.956f, 0.330f, 1.248f),  // Cube (9)
        new Vector3(-0.052f, 0.330f, 0.474f)  // Cube (18)
        };

        // Layout 5 
        allLayouts[4] = new Vector3[]
        {
            // Cube (1)
        new Vector3(-0.738f, 0.330f, 1.809f),
        // Cube (19)
        new Vector3(1.180f, 0.330f, -1.431f),
        // Cube (22)
        new Vector3(0.880f, 0.330f, 0.393f),
        // Cube (10)
        new Vector3(-0.720f, 0.330f, 0.903f),
        // Cube (13)
        new Vector3(0.468f, 0.330f, -2.130f),
        // Cube (4)
        new Vector3(-0.410f, 0.330f, -1.550f),
        // Cube (0)
        new Vector3(-1.790f, 0.330f, -1.920f),
        // Cube (15)
        new Vector3(-0.100f, 0.330f, -0.609f),
        // Cube (7)
        new Vector3(-1.690f, 0.330f, 1.710f),
        // Cube (14)
        new Vector3(1.210f, 0.330f, -0.210f),
        // Cube (21)
        new Vector3(-1.360f, 0.330f, -1.040f),
        // Cube (3)
        new Vector3(1.950f, 0.330f, 1.790f),
        // Cube (6)
        new Vector3(0.552f, 0.330f, -0.705f),
        // Cube (16)
        new Vector3(-1.810f, 0.330f, 0.560f),
        // Cube (11)
        new Vector3(1.770f, 0.330f, -1.020f),
        // Cube (17)
        new Vector3(1.590f, 0.330f, 0.810f),
        // Cube (8)
        new Vector3(-0.480f, 0.330f, 2.050f),
        // Cube (20)
        new Vector3(0.180f, 0.330f, 0.650f),
        // Cube (2)
        new Vector3(-0.350f, 0.330f, -0.060f),
        // Cube (12)
        new Vector3(0.810f, 0.330f, 1.250f),
        // Cube (5)
        new Vector3(-1.490f, 0.330f, 0.030f),
        // Cube (9)
        new Vector3(0.330f, 0.330f, 1.770f),
        // Cube (18)
        new Vector3(1.990f, 0.330f, -0.087f)
        };

        // Layout 6 
        allLayouts[5] = new Vector3[]
        {
        // Cube (1)
        new Vector3(-0.738f, 0.330f, 1.809f),
        // Cube (19)
        new Vector3(1.180f, 0.330f, -1.431f),
        // Cube (22)
        new Vector3(0.880f, 0.330f, 0.393f),
        // Cube (10)
        new Vector3(-0.720f, 0.330f, 0.903f),
        // Cube (13)
        new Vector3(0.468f, 0.330f, -2.130f),
        // Cube (4)
        new Vector3(-0.410f, 0.330f, -1.550f),
        // Cube (0)
        new Vector3(-1.790f, 0.330f, -1.920f),
        // Cube (15)
        new Vector3(-0.100f, 0.330f, -0.609f),
        // Cube (7)
        new Vector3(-1.690f, 0.330f, 1.710f),
        // Cube (14)
        new Vector3(1.210f, 0.330f, -0.210f),
        // Cube (21)
        new Vector3(-1.360f, 0.330f, -1.040f),
        // Cube (3)
        new Vector3(1.950f, 0.330f, 1.790f),
        // Cube (6)
        new Vector3(0.552f, 0.330f, -0.705f),
        // Cube (16)
        new Vector3(-1.810f, 0.330f, 0.560f),
        // Cube (11)
        new Vector3(1.770f, 0.330f, -1.020f),
        // Cube (17)
        new Vector3(1.590f, 0.330f, 0.810f),
        // Cube (8)
        new Vector3(-0.480f, 0.330f, 2.050f),
        // Cube (20)
        new Vector3(0.180f, 0.330f, 0.650f),
        // Cube (2)
        new Vector3(-0.350f, 0.330f, -0.060f),
        // Cube (12)
        new Vector3(0.810f, 0.330f, 1.250f),
        // Cube (5)
        new Vector3(-1.490f, 0.330f, 0.030f),
        // Cube (9)
        new Vector3(0.330f, 0.330f, 1.770f),
        // Cube (18)
        new Vector3(1.990f, 0.330f, -0.087f)
        };

        // Layout 7 
        allLayouts[6] = new Vector3[]
        {
        // Cube (20)
        new Vector3(0.448f, 0.330f, -0.663f),
        // Cube (21)
        new Vector3(-1.060f, 0.330f, -1.557f),
        // Cube (22)
        new Vector3(1.788f, 0.330f, 1.344f),
        // Cube (1)
        new Vector3(1.148f, 0.330f, -1.116f),
        // Cube (19)
        new Vector3(0.766f, 0.330f, -1.629f),
        // Cube (10)
        new Vector3(-0.410f, 0.330f, -2.112f),
        // Cube (13)
        new Vector3(-0.806f, 0.330f, 0.399f),
        // Cube (4)
        new Vector3(-0.332f, 0.330f, -0.285f),
        // Cube (0)
        new Vector3(-1.690f, 0.330f, 1.710f),
        // Cube (15)
        new Vector3(1.444f, 0.330f, 0.135f),
        // Cube (7)
        new Vector3(1.576f, 0.330f, -0.573f),
        // Cube (14)
        new Vector3(0.206f, 0.330f, 1.098f),
        // Cube (3)
        new Vector3(0.130f, 0.330f, -1.248f),
        // Cube (6)
        new Vector3(1.132f, 0.330f, -2.088f),
        // Cube (16)
        new Vector3(1.114f, 0.330f, 1.788f),
        // Cube (11)
        new Vector3(0.046f, 0.330f, 2.100f),
        // Cube (17)
        new Vector3(-1.330f, 0.330f, -0.795f),
        // Cube (8)
        new Vector3(-1.920f, 0.330f, -0.003f),
        // Cube (2)
        new Vector3(1.280f, 0.330f, 0.618f),
        // Cube (12)
        new Vector3(-0.768f, 0.330f, 1.710f),
        // Cube (5)
        new Vector3(-0.710f, 0.330f, -0.597f),
        // Cube (9)
        new Vector3(-1.742f, 0.330f, 1.173f),
        // Cube (18)
        new Vector3(-0.422f, 0.330f, 1.104f)
        };
        
        // Layout 8 
        allLayouts[7] = new Vector3[]
        {
        // Cube (1)
        new Vector3(-0.738f, 0.330f, 1.809f),
        // Cube (19)
        new Vector3(1.180f, 0.330f, -1.431f),
        // Cube (22)
        new Vector3(0.880f, 0.330f, 0.393f),
        // Cube (10)
        new Vector3(-0.720f, 0.330f, 0.903f),
        // Cube (13)
        new Vector3(0.468f, 0.330f, -2.130f),
        // Cube (4)
        new Vector3(-0.410f, 0.330f, -1.550f),
        // Cube (0)
        new Vector3(-1.790f, 0.330f, -1.920f),
        // Cube (15)
        new Vector3(-0.100f, 0.330f, -0.609f),
        // Cube (7)
        new Vector3(-1.690f, 0.330f, 1.710f),
        // Cube (14)
        new Vector3(1.210f, 0.330f, -0.210f),
        // Cube (21)
        new Vector3(-1.360f, 0.330f, -1.040f),
        // Cube (3)
        new Vector3(1.950f, 0.330f, 1.790f),
        // Cube (6)
        new Vector3(0.552f, 0.330f, -0.705f),
        // Cube (16)
        new Vector3(-1.810f, 0.330f, 0.560f),
        // Cube (11)
        new Vector3(1.770f, 0.330f, -1.020f),
        // Cube (17)
        new Vector3(1.590f, 0.330f, 0.810f),
        // Cube (8)
        new Vector3(-0.480f, 0.330f, 2.050f),
        // Cube (20)
        new Vector3(0.180f, 0.330f, 0.650f),
        // Cube (2)
        new Vector3(-0.350f, 0.330f, -0.060f),
        // Cube (12)
        new Vector3(0.810f, 0.330f, 1.250f),
        // Cube (5)
        new Vector3(-1.490f, 0.330f, 0.030f),
        // Cube (9)
        new Vector3(0.330f, 0.330f, 1.770f),
        // Cube (18)
        new Vector3(1.990f, 0.330f, -0.087f)
        };

        // Layout 9 
        allLayouts[8] = new Vector3[]
        {
        // Cube (1)
        new Vector3(-0.738f, 0.330f, 1.809f),
        // Cube (19)
        new Vector3(1.180f, 0.330f, -1.431f),
        // Cube (22)
        new Vector3(0.880f, 0.330f, 0.393f),
        // Cube (10)
        new Vector3(-0.720f, 0.330f, 0.903f),
        // Cube (13)
        new Vector3(0.468f, 0.330f, -2.130f),
        // Cube (4)
        new Vector3(-0.410f, 0.330f, -1.550f),
        // Cube (0)
        new Vector3(-1.790f, 0.330f, -1.920f),
        // Cube (15)
        new Vector3(-0.100f, 0.330f, -0.609f),
        // Cube (7)
        new Vector3(-1.690f, 0.330f, 1.710f),
        // Cube (14)
        new Vector3(1.210f, 0.330f, -0.210f),
        // Cube (21)
        new Vector3(-1.360f, 0.330f, -1.040f),
        // Cube (3)
        new Vector3(1.950f, 0.330f, 1.790f),
        // Cube (6)
        new Vector3(0.552f, 0.330f, -0.705f),
        // Cube (16)
        new Vector3(-1.810f, 0.330f, 0.560f),
        // Cube (11)
        new Vector3(1.770f, 0.330f, -1.020f),
        // Cube (17)
        new Vector3(1.590f, 0.330f, 0.810f),
        // Cube (8)
        new Vector3(-0.480f, 0.330f, 2.050f),
        // Cube (20)
        new Vector3(0.180f, 0.330f, 0.650f),
        // Cube (2)
        new Vector3(-0.350f, 0.330f, -0.060f),
        // Cube (12)
        new Vector3(0.810f, 0.330f, 1.250f),
        // Cube (5)
        new Vector3(-1.490f, 0.330f, 0.030f),
        // Cube (9)
        new Vector3(0.330f, 0.330f, 1.770f),
        // Cube (18)
        new Vector3(1.990f, 0.330f, -0.087f)
        };

        // Layout 10
        allLayouts[9] = new Vector3[]
        {
           // Cube (20)
        new Vector3(-0.402f, 0.330f, -0.663f),
        // Cube (21)
        new Vector3(-0.746f, 0.330f, -1.557f),
        // Cube (22)
        new Vector3(1.510f, 0.330f, -0.096f),
        // Cube (1)
        new Vector3(1.814f, 0.330f, 0.729f),
        // Cube (19)
        new Vector3(-1.542f, 0.330f, -1.617f),
        // Cube (10)
        new Vector3(-0.188f, 0.330f, -1.947f),
        // Cube (13)
        new Vector3(-0.806f, 0.330f, 0.897f),
        // Cube (4)
        new Vector3(1.350f, 0.330f, -1.209f),
        // Cube (0)
        new Vector3(-1.690f, 0.330f, 1.710f),
        // Cube (15)
        new Vector3(1.130f, 0.330f, 0.984f),
        // Cube (7)
        new Vector3(-1.880f, 0.330f, -1.146f),
        // Cube (14)
        new Vector3(0.206f, 0.330f, 1.098f),
        // Cube (3)
        new Vector3(0.352f, 0.330f, -1.230f),
        // Cube (6)
        new Vector3(0.854f, 0.330f, -0.498f),
        // Cube (16)
        new Vector3(1.114f, 0.330f, 1.788f),
        // Cube (11)
        new Vector3(0.046f, 0.330f, 2.100f),
        // Cube (17)
        new Vector3(-1.330f, 0.330f, -0.795f),
        // Cube (8)
        new Vector3(-1.920f, 0.330f, -0.003f),
        // Cube (2)
        new Vector3(-1.104f, 0.330f, 0.009f),
        // Cube (12)
        new Vector3(-0.768f, 0.330f, 1.710f),
        // Cube (5)
        new Vector3(0.380f, 0.330f, 0.255f),
        // Cube (9)
        new Vector3(-1.742f, 0.330f, 1.173f),
        // Cube (18)
        new Vector3(-0.348f, 0.330f, 0.069f)
        };


        // Layout 11
        allLayouts[10] = new Vector3[]
        {
        // Cube (1)
        new Vector3(-0.738f, 0.330f, 1.809f),
        // Cube (19)
        new Vector3(1.180f, 0.330f, -1.431f),
        // Cube (22)
        new Vector3(0.880f, 0.330f, 0.393f),
        // Cube (10)
        new Vector3(-0.720f, 0.330f, 0.903f),
        // Cube (13)
        new Vector3(0.468f, 0.330f, -2.130f),
        // Cube (4)
        new Vector3(-0.410f, 0.330f, -1.550f),
        // Cube (0)
        new Vector3(-1.790f, 0.330f, -1.920f),
        // Cube (15)
        new Vector3(-0.100f, 0.330f, -0.609f),
        // Cube (7)
        new Vector3(-1.690f, 0.330f, 1.710f),
        // Cube (14)
        new Vector3(1.210f, 0.330f, -0.210f),
        // Cube (21)
        new Vector3(-1.360f, 0.330f, -1.040f),
        // Cube (3)
        new Vector3(1.950f, 0.330f, 1.790f),
        // Cube (6)
        new Vector3(0.552f, 0.330f, -0.705f),
        // Cube (16)
        new Vector3(-1.810f, 0.330f, 0.560f),
        // Cube (11)
        new Vector3(1.770f, 0.330f, -1.020f),
        // Cube (17)
        new Vector3(1.590f, 0.330f, 0.810f),
        // Cube (8)
        new Vector3(-0.480f, 0.330f, 2.050f),
        // Cube (20)
        new Vector3(0.180f, 0.330f, 0.650f),
        // Cube (2)
        new Vector3(-0.350f, 0.330f, -0.060f),
        // Cube (12)
        new Vector3(0.810f, 0.330f, 1.250f),
        // Cube (5)
        new Vector3(-1.490f, 0.330f, 0.030f),
        // Cube (9)
        new Vector3(0.330f, 0.330f, 1.770f),
        // Cube (18)
        new Vector3(1.990f, 0.330f, -0.087f)
        };

         // Layout 12
        allLayouts[11] = new Vector3[]
        {
        // Cube (1)
        new Vector3(-0.738f, 0.330f, 1.809f),
        // Cube (19)
        new Vector3(1.180f, 0.330f, -1.431f),
        // Cube (22)
        new Vector3(0.880f, 0.330f, 0.393f),
        // Cube (10)
        new Vector3(-0.720f, 0.330f, 0.903f),
        // Cube (13)
        new Vector3(0.468f, 0.330f, -2.130f),
        // Cube (4)
        new Vector3(-0.410f, 0.330f, -1.550f),
        // Cube (0)
        new Vector3(-1.790f, 0.330f, -1.920f),
        // Cube (15)
        new Vector3(-0.100f, 0.330f, -0.609f),
        // Cube (7)
        new Vector3(-1.690f, 0.330f, 1.710f),
        // Cube (14)
        new Vector3(1.210f, 0.330f, -0.210f),
        // Cube (21)
        new Vector3(-1.360f, 0.330f, -1.040f),
        // Cube (3)
        new Vector3(1.950f, 0.330f, 1.790f),
        // Cube (6)
        new Vector3(0.552f, 0.330f, -0.705f),
        // Cube (16)
        new Vector3(-1.810f, 0.330f, 0.560f),
        // Cube (11)
        new Vector3(1.770f, 0.330f, -1.020f),
        // Cube (17)
        new Vector3(1.590f, 0.330f, 0.810f),
        // Cube (8)
        new Vector3(-0.480f, 0.330f, 2.050f),
        // Cube (20)
        new Vector3(0.180f, 0.330f, 0.650f),
        // Cube (2)
        new Vector3(-0.350f, 0.330f, -0.060f),
        // Cube (12)
        new Vector3(0.810f, 0.330f, 1.250f),
        // Cube (5)
        new Vector3(-1.490f, 0.330f, 0.030f),
        // Cube (9)
        new Vector3(0.330f, 0.330f, 1.770f),
        // Cube (18)
        new Vector3(1.990f, 0.330f, -0.087f)
        };

         // Layout 13
        allLayouts[12] = new Vector3[]
        {
        // Cube (1)
        new Vector3(-0.738f, 0.330f, 1.809f),
        // Cube (19)
        new Vector3(1.180f, 0.330f, -1.431f),
        // Cube (22)
        new Vector3(0.880f, 0.330f, 0.393f),
        // Cube (10)
        new Vector3(-0.720f, 0.330f, 0.903f),
        // Cube (13)
        new Vector3(0.468f, 0.330f, -2.130f),
        // Cube (4)
        new Vector3(-0.410f, 0.330f, -1.550f),
        // Cube (0)
        new Vector3(-1.790f, 0.330f, -1.920f),
        // Cube (15)
        new Vector3(-0.100f, 0.330f, -0.609f),
        // Cube (7)
        new Vector3(-1.690f, 0.330f, 1.710f),
        // Cube (14)
        new Vector3(1.210f, 0.330f, -0.210f),
        // Cube (21)
        new Vector3(-1.360f, 0.330f, -1.040f),
        // Cube (3)
        new Vector3(1.950f, 0.330f, 1.790f),
        // Cube (6)
        new Vector3(0.552f, 0.330f, -0.705f),
        // Cube (16)
        new Vector3(-1.810f, 0.330f, 0.560f),
        // Cube (11)
        new Vector3(1.770f, 0.330f, -1.020f),
        // Cube (17)
        new Vector3(1.590f, 0.330f, 0.810f),
        // Cube (8)
        new Vector3(-0.480f, 0.330f, 2.050f),
        // Cube (20)
        new Vector3(0.180f, 0.330f, 0.650f),
        // Cube (2)
        new Vector3(-0.350f, 0.330f, -0.060f),
        // Cube (12)
        new Vector3(0.810f, 0.330f, 1.250f),
        // Cube (5)
        new Vector3(-1.490f, 0.330f, 0.030f),
        // Cube (9)
        new Vector3(0.330f, 0.330f, 1.770f),
        // Cube (18)
        new Vector3(1.990f, 0.330f, -0.087f),
        // Avatar
        new Vector3(-1.310f, 0.330f, 2.220f)
        };

                 // Layout 14
        allLayouts[13] = new Vector3[]
        {
        // Cube (1)
        new Vector3(-0.738f, 0.330f, 1.809f),
        // Cube (19)
        new Vector3(1.180f, 0.330f, -1.431f),
        // Cube (22)
        new Vector3(0.880f, 0.330f, 0.393f),
        // Cube (10)
        new Vector3(-0.720f, 0.330f, 0.903f),
        // Cube (13)
        new Vector3(0.468f, 0.330f, -2.130f),
        // Cube (4)
        new Vector3(-0.410f, 0.330f, -1.550f),
        // Cube (0)
        new Vector3(-1.790f, 0.330f, -1.920f),
        // Cube (15)
        new Vector3(-0.100f, 0.330f, -0.609f),
        // Cube (7)
        new Vector3(-1.690f, 0.330f, 1.710f),
        // Cube (14)
        new Vector3(1.210f, 0.330f, -0.210f),
        // Cube (21)
        new Vector3(-1.360f, 0.330f, -1.040f),
        // Cube (3)
        new Vector3(1.950f, 0.330f, 1.790f),
        // Cube (6)
        new Vector3(0.552f, 0.330f, -0.705f),
        // Cube (16)
        new Vector3(-1.810f, 0.330f, 0.560f),
        // Cube (11)
        new Vector3(1.770f, 0.330f, -1.020f),
        // Cube (17)
        new Vector3(1.590f, 0.330f, 0.810f),
        // Cube (8)
        new Vector3(-0.480f, 0.330f, 2.050f),
        // Cube (20)
        new Vector3(0.180f, 0.330f, 0.650f),
        // Cube (2)
        new Vector3(-0.350f, 0.330f, -0.060f),
        // Cube (12)
        new Vector3(0.810f, 0.330f, 1.250f),
        // Cube (5)
        new Vector3(-1.490f, 0.330f, 0.030f),
        // Cube (9)
        new Vector3(0.330f, 0.330f, 1.770f),
        // Cube (18)
        new Vector3(1.990f, 0.330f, -0.087f),
        // Avatar
        new Vector3(-1.310f, 0.330f, 2.220f)
        };

             // Layout 15
        allLayouts[14] = new Vector3[]
        {
        // Cube (1)
        new Vector3(-0.738f, 0.330f, 1.809f),
        // Cube (19)
        new Vector3(1.180f, 0.330f, -1.431f),
        // Cube (22)
        new Vector3(0.880f, 0.330f, 0.393f),
        // Cube (10)
        new Vector3(-0.720f, 0.330f, 0.903f),
        // Cube (13)
        new Vector3(0.468f, 0.330f, -2.130f),
        // Cube (4)
        new Vector3(-0.410f, 0.330f, -1.550f),
        // Cube (0)
        new Vector3(-1.790f, 0.330f, -1.920f),
        // Cube (15)
        new Vector3(-0.100f, 0.330f, -0.609f),
        // Cube (7)
        new Vector3(-1.690f, 0.330f, 1.710f),
        // Cube (14)
        new Vector3(1.210f, 0.330f, -0.210f),
        // Cube (21)
        new Vector3(-1.360f, 0.330f, -1.040f),
        // Cube (3)
        new Vector3(1.950f, 0.330f, 1.790f),
        // Cube (6)
        new Vector3(0.552f, 0.330f, -0.705f),
        // Cube (16)
        new Vector3(-1.810f, 0.330f, 0.560f),
        // Cube (11)
        new Vector3(1.770f, 0.330f, -1.020f),
        // Cube (17)
        new Vector3(1.590f, 0.330f, 0.810f),
        // Cube (8)
        new Vector3(-0.480f, 0.330f, 2.050f),
        // Cube (20)
        new Vector3(0.180f, 0.330f, 0.650f),
        // Cube (2)
        new Vector3(-0.350f, 0.330f, -0.060f),
        // Cube (12)
        new Vector3(0.810f, 0.330f, 1.250f),
        // Cube (5)
        new Vector3(-1.490f, 0.330f, 0.030f),
        // Cube (9)
        new Vector3(0.330f, 0.330f, 1.770f),
        // Cube (18)
        new Vector3(1.990f, 0.330f, -0.087f),
        // Avatar
        new Vector3(-1.310f, 0.330f, 2.220f)
        };



        
        for (int i = 2; i < 15; i++)
        {
            allLayouts[i] = (Vector3[])allLayouts[0].Clone();
        }
    }

 /// <summary>
    /// Set cubes to a specific layout (1-15)
    /// </summary>
    public void SetLayout(int layoutNumber)
    {
        if (layoutNumber < 1 || layoutNumber > 15)
        {
            Debug.LogError($"Invalid layout number: {layoutNumber}. Must be 1-15.");
            return;
        }

        int arrayIndex = layoutNumber - 1;
        Vector3[] positions = allLayouts[arrayIndex];

        int cubesAssigned = 0;
        for (int i = 0; i < triggerCubes.Length; i++)
        {
            if (triggerCubes[i] != null)
            {
                if (i < positions.Length)
                {
                    triggerCubes[i].SetActive(true);
                    triggerCubes[i].transform.position = positions[i];
                    cubesAssigned++;
                }
                else
                {
                    triggerCubes[i].SetActive(false);
                    Debug.LogWarning($"Layout {layoutNumber} has fewer positions than cubes. Hiding cube {i}.");
                }
            }
            else
            {
                Debug.LogWarning($"Trigger cube slot {i} is null! Please assign in inspector.");
            }
        }

        currentLayout = layoutNumber;
        Debug.Log($"Set {cubesAssigned} cubes to layout {layoutNumber}");
    }

    /// <summary>
    /// Cycle to the next layout
    /// </summary>
    public void NextLayout()
    {
        int nextLayout = (currentLayout % 15) + 1;
        SetLayout(nextLayout);
    }

    /// <summary>
    /// Cycle to previous layout
    /// </summary>
    public void PreviousLayout()
    {
        int prevLayout = currentLayout - 1;
        if (prevLayout < 1) prevLayout = 15;
        SetLayout(prevLayout);
    }

    /// <summary>
    /// Hide all cubes
    /// </summary>
    public void HideAllCubes()
    {
        foreach (GameObject cube in triggerCubes)
        {
            if (cube != null)
            {
                cube.SetActive(false);
            }
        }
        Debug.Log("All trigger cubes hidden");
    }

    /// <summary>
    /// Show all cubes at current layout
    /// </summary>
    public void ShowAllCubes()
    {
        SetLayout(currentLayout);
    }

    // Get current layout number (1-15)
    public int GetCurrentLayout()
    {
        return currentLayout;
    }
}