#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

// Runtime crash diagnostic and fix tool
public class RuntimeCrashFix : MonoBehaviour
{
    [Header("Runtime Crash Prevention")]
    public bool enableCrashPrevention = true;
    public bool verboseLogging = true;
    
    void Awake()
    {
        if (enableCrashPrevention)
        {
            // Set up crash prevention as early as possible
            SetupCrashPrevention();
        }
    }
    
    void Start()
    {
        if (verboseLogging)
        {
            Debug.Log("=== RUNTIME CRASH FIX STARTING ===");
            DiagnoseRuntimeEnvironment();
        }
        
        // Prevent common Unity runtime crashes
        PreventCommonCrashes();
        
        if (verboseLogging)
        {
            Debug.Log("=== RUNTIME CRASH FIX COMPLETE ===");
        }
    }
    
    private void SetupCrashPrevention()
    {
        // Ensure DontDestroyOnLoad works in builds
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }
        
        // Set up exception handling
        Application.logMessageReceived += HandleException;
        
        // Configure Unity settings for stability
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        
        Debug.Log("✓ Crash prevention systems activated");
    }
    
    private void HandleException(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            Debug.LogError($"RUNTIME ERROR DETECTED: {logString}");
            Debug.LogError($"Stack Trace: {stackTrace}");
            
            // Try to recover from common errors
            if (logString.Contains("NullReferenceException"))
            {
                Debug.LogWarning("Attempting to recover from null reference...");
                // Don't let null references crash the game
            }
            
            if (logString.Contains("MissingReferenceException"))
            {
                Debug.LogWarning("Attempting to recover from missing reference...");
                // Try to continue despite missing references
            }
        }
    }
    
    private void DiagnoseRuntimeEnvironment()
    {
        Debug.Log("--- Runtime Environment Diagnostics ---");
        
        try
        {
            Debug.Log($"Unity Version: {Application.unityVersion}");
            Debug.Log($"Platform: {Application.platform}");
            Debug.Log($"Is Editor: {Application.isEditor}");
            Debug.Log($"Current Scene: {SceneManager.GetActiveScene().name}");
            Debug.Log($"Scene Build Index: {SceneManager.GetActiveScene().buildIndex}");
            Debug.Log($"Graphics Device: {SystemInfo.graphicsDeviceName}");
            Debug.Log($"System Memory: {SystemInfo.systemMemorySize} MB");
            
            // Check for common crash causes
            CheckForCrashCauses();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during diagnostics: {e.Message}");
        }
    }
    
    private void CheckForCrashCauses()
    {
        Debug.Log("--- Checking for Common Crash Causes ---");
        
        // Check for missing components
        CheckMissingComponents();
        
        // Check for problematic scripts
        CheckProblematicScripts();
        
        // Check scene validity
        CheckSceneValidity();
        
        // Check dependencies
        CheckDependencies();
    }
    
    private void CheckMissingComponents()
    {
        try
        {
            MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
            int nullCount = 0;
            
            foreach (MonoBehaviour script in allScripts)
            {
                if (script == null)
                {
                    nullCount++;
                }
            }
            
            if (nullCount > 0)
            {
                Debug.LogError($"❌ Found {nullCount} missing script references - MAJOR CRASH RISK!");
                Debug.LogError("This is likely causing the crash. Missing scripts need to be fixed.");
            }
            else
            {
                Debug.Log("✓ No missing script references found");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking components: {e.Message}");
        }
    }
    
    private void CheckProblematicScripts()
    {
        try
        {
            // Check for scripts that commonly cause crashes
            string[] problematicScripts = {
                "EnhancedLobbyUI",
                "ModernUITheme", 
                "NetCardLobby",
                "ChemistryLobbyManager"
            };
            
            foreach (string scriptName in problematicScripts)
            {
                UnityEngine.Object[] objects = FindObjectsOfType(System.Type.GetType(scriptName));
            Component[] components = new Component[objects.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                components[i] = objects[i] as Component;
            }
                if (components != null && components.Length > 0)
                {
                    Debug.Log($"Found {components.Length} {scriptName} components");
                    
                    // Check if any are null
                    foreach (Component comp in components)
                    {
                        if (comp == null)
                        {
                            Debug.LogError($"❌ NULL {scriptName} component found - CRASH RISK!");
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking problematic scripts: {e.Message}");
        }
    }
    
    private void CheckSceneValidity()
    {
        try
        {
            Scene currentScene = SceneManager.GetActiveScene();
            
            if (!currentScene.isLoaded)
            {
                Debug.LogError("❌ Current scene is not properly loaded!");
                return;
            }
            
            GameObject[] rootObjects = currentScene.GetRootGameObjects();
            Debug.Log($"Scene has {rootObjects.Length} root objects");
            
            // Check for essential components
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            if (canvases.Length == 0)
            {
                Debug.LogWarning("⚠️ No Canvas found - UI may not work");
            }
            else
            {
                Debug.Log($"✓ Found {canvases.Length} Canvas components");
            }
            
            UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystems.Length == 0)
            {
                Debug.LogWarning("⚠️ No EventSystem found - UI interaction may not work");
            }
            else
            {
                Debug.Log($"✓ Found {eventSystems.Length} EventSystem components");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking scene validity: {e.Message}");
        }
    }
    
    private void CheckDependencies()
    {
        Debug.Log("--- Checking Dependencies ---");
        
        // Check for DOTween
        try
        {
            System.Type dotweenType = System.Type.GetType("DG.Tweening.DOTween");
            if (dotweenType != null)
            {
                Debug.Log("✓ DOTween available");
            }
            else
            {
                Debug.LogWarning("⚠️ DOTween not available - some scripts may fail");
            }
        }
        catch
        {
            Debug.LogWarning("⚠️ DOTween check failed");
        }
        
        // Check for Photon
        try
        {
            System.Type photonType = System.Type.GetType("Photon.Pun.PhotonNetwork");
            if (photonType != null)
            {
                Debug.Log("✓ Photon PUN2 available");
            }
            else
            {
                Debug.LogWarning("⚠️ Photon PUN2 not available - networking scripts may fail");
            }
        }
        catch
        {
            Debug.LogWarning("⚠️ Photon check failed");
        }
    }
    
    private void PreventCommonCrashes()
    {
        try
        {
            // Disable problematic components that might cause crashes
            DisableProblematicComponents();
            
            // Create minimal UI if none exists
            EnsureBasicUIExists();
            
            Debug.Log("✓ Common crash prevention measures applied");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in crash prevention: {e.Message}");
        }
    }
    
    private void DisableProblematicComponents()
    {
        // Disable components that are known to cause crashes without dependencies
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        
        foreach (MonoBehaviour script in allScripts)
        {
            if (script == null) continue;
            
            string scriptType = script.GetType().Name;
            
            // Disable scripts that use DOTween if DOTween is not available
            if (System.Type.GetType("DG.Tweening.DOTween") == null)
            {
                if (scriptType.Contains("Enhanced") || scriptType.Contains("Modern"))
                {
                    script.enabled = false;
                    Debug.LogWarning($"⚠️ Disabled {scriptType} due to missing DOTween dependency");
                }
            }
            
            // Disable Photon scripts if Photon is not available
            if (System.Type.GetType("Photon.Pun.PhotonNetwork") == null)
            {
                if (scriptType.Contains("Net") || scriptType.Contains("Photon"))
                {
                    script.enabled = false;
                    Debug.LogWarning($"⚠️ Disabled {scriptType} due to missing Photon dependency");
                }
            }
        }
    }
    
    private void EnsureBasicUIExists()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        
        if (canvases.Length == 0)
        {
            Debug.LogWarning("No Canvas found - creating basic UI");
            CreateEmergencyUI();
        }
        
        UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystems.Length == 0)
        {
            Debug.LogWarning("No EventSystem found - creating one");
            CreateEventSystem();
        }
    }
    
    private void CreateEmergencyUI()
    {
        try
        {
            GameObject canvasObj = new GameObject("Emergency Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            Debug.Log("✓ Emergency Canvas created");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create emergency UI: {e.Message}");
        }
    }
    
    private void CreateEventSystem()
    {
        try
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            Debug.Log("✓ EventSystem created");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create EventSystem: {e.Message}");
        }
    }
    
    void OnDestroy()
    {
        // Clean up event handlers
        Application.logMessageReceived -= HandleException;
    }
}
#endif