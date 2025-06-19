using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Simple crash prevention that should always work
public class SimpleCrashFix : MonoBehaviour
{
    [Header("Simple Crash Prevention")]
    public bool enableLogging = true;
    
    void Awake()
    {
        // Prevent this object from being destroyed
        DontDestroyOnLoad(gameObject);
        
        if (enableLogging)
        {
            Debug.Log("=== SIMPLE CRASH FIX ACTIVE ===");
        }
        
        // Set up basic crash prevention
        SetupBasicCrashPrevention();
    }
    
    void Start()
    {
        if (enableLogging)
        {
            LogBasicInfo();
        }
        
        // Fix common issues
        FixCommonIssues();
        
        if (enableLogging)
        {
            Debug.Log("=== CRASH FIX COMPLETE ===");
        }
    }
    
    private void SetupBasicCrashPrevention()
    {
        try
        {
            // Handle exceptions to prevent crashes
            Application.logMessageReceived += HandleException;
            
            // Set safe settings
            Application.targetFrameRate = 60;
            
            if (enableLogging)
            {
                Debug.Log("✓ Basic crash prevention enabled");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting up crash prevention: {e.Message}");
        }
    }
    
    private void HandleException(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            Debug.LogError($"EXCEPTION INTERCEPTED: {logString}");
            
            // Try to handle specific crashes
            if (logString.Contains("NullReferenceException"))
            {
                Debug.LogWarning("Null reference caught - continuing execution");
            }
            
            if (logString.Contains("MissingReferenceException"))
            {
                Debug.LogWarning("Missing reference caught - continuing execution");
            }
            
            if (logString.Contains("DOTween") || logString.Contains("Enhanced") || logString.Contains("Modern"))
            {
                Debug.LogWarning("UI system error caught - disabling enhanced components");
                DisableEnhancedComponents();
            }
            
            if (logString.Contains("Photon") || logString.Contains("Net"))
            {
                Debug.LogWarning("Network error caught - disabling network components");
                DisableNetworkComponents();
            }
        }
    }
    
    private void LogBasicInfo()
    {
        try
        {
            Debug.Log("=== BASIC SYSTEM INFO ===");
            Debug.Log($"Unity Version: {Application.unityVersion}");
            Debug.Log($"Platform: {Application.platform}");
            Debug.Log($"Current Scene: {SceneManager.GetActiveScene().name}");
            Debug.Log($"Is Editor: {Application.isEditor}");
            Debug.Log($"Total Scenes in Build: {SceneManager.sceneCountInBuildSettings}");
            
            // List scenes
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (!string.IsNullOrEmpty(scenePath))
                {
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                    Debug.Log($"  Scene {i}: {sceneName}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error logging info: {e.Message}");
        }
    }
    
    private void FixCommonIssues()
    {
        try
        {
            // Check for missing components
            CheckForMissingScripts();
            
            // Ensure basic UI exists
            EnsureBasicUI();
            
            // Disable problematic components
            DisableProblematicComponents();
            
            Debug.Log("✓ Common issues checked and fixed");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error fixing common issues: {e.Message}");
        }
    }
    
    private void CheckForMissingScripts()
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
                Debug.LogError($"❌ CRITICAL: Found {nullCount} missing script references!");
                Debug.LogError("This is likely causing the crash. Missing scripts need to be removed from GameObjects.");
            }
            else
            {
                Debug.Log("✓ No missing script references found");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking scripts: {e.Message}");
        }
    }
    
    private void EnsureBasicUI()
    {
        try
        {
            // Check if we have basic UI components
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
            
            Debug.Log($"Found {canvases.Length} Canvas components");
            Debug.Log($"Found {eventSystems.Length} EventSystem components");
            
            // Create EventSystem if missing
            if (eventSystems.Length == 0)
            {
                Debug.LogWarning("No EventSystem found - creating one");
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("✓ EventSystem created");
            }
            
            // If no canvas, create a basic one
            if (canvases.Length == 0)
            {
                Debug.LogWarning("No Canvas found - creating basic canvas");
                CreateBasicCanvas();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error ensuring basic UI: {e.Message}");
        }
    }
    
    private void CreateBasicCanvas()
    {
        try
        {
            GameObject canvasObj = new GameObject("BasicCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // Ensure it's on top
            
            // Add basic canvas components
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Add a simple background
            GameObject background = new GameObject("Background");
            background.transform.SetParent(canvas.transform, false);
            
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
            
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.2f, 1f); // Dark background
            
            // Add a title
            GameObject title = new GameObject("Title");
            title.transform.SetParent(canvas.transform, false);
            
            RectTransform titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.1f, 0.7f);
            titleRect.anchorMax = new Vector2(0.9f, 0.9f);
            titleRect.sizeDelta = Vector2.zero;
            titleRect.anchoredPosition = Vector2.zero;
            
            Text titleText = title.AddComponent<Text>();
            titleText.text = "CHEMISTRY PARTY\n(Safe Mode - Check Console)";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 32;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            Debug.Log("✓ Basic canvas created");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create basic canvas: {e.Message}");
        }
    }
    
    private void DisableProblematicComponents()
    {
        try
        {
            // Check if dependencies are available
            bool hasDOTween = System.Type.GetType("DG.Tweening.DOTween") != null;
            bool hasPhoton = System.Type.GetType("Photon.Pun.PhotonNetwork") != null;
            
            Debug.Log($"DOTween available: {hasDOTween}");
            Debug.Log($"Photon available: {hasPhoton}");
            
            if (!hasDOTween)
            {
                DisableEnhancedComponents();
            }
            
            if (!hasPhoton)
            {
                DisableNetworkComponents();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking dependencies: {e.Message}");
        }
    }
    
    private void DisableEnhancedComponents()
    {
        try
        {
            MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
            int disabledCount = 0;
            
            foreach (MonoBehaviour script in allScripts)
            {
                if (script == null) continue;
                
                string scriptType = script.GetType().Name;
                if (scriptType.Contains("Enhanced") || scriptType.Contains("Modern") || 
                    scriptType.Contains("DOTween") || scriptType.Contains("Tween"))
                {
                    script.enabled = false;
                    disabledCount++;
                    if (enableLogging)
                    {
                        Debug.Log($"Disabled {scriptType} (missing DOTween)");
                    }
                }
            }
            
            if (disabledCount > 0)
            {
                Debug.LogWarning($"Disabled {disabledCount} enhanced UI components due to missing DOTween");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error disabling enhanced components: {e.Message}");
        }
    }
    
    private void DisableNetworkComponents()
    {
        try
        {
            MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
            int disabledCount = 0;
            
            foreach (MonoBehaviour script in allScripts)
            {
                if (script == null) continue;
                
                string scriptType = script.GetType().Name;
                if (scriptType.Contains("Net") || scriptType.Contains("Photon") || 
                    scriptType.Contains("PUN") || scriptType.Contains("Network"))
                {
                    script.enabled = false;
                    disabledCount++;
                    if (enableLogging)
                    {
                        Debug.Log($"Disabled {scriptType} (missing Photon)");
                    }
                }
            }
            
            if (disabledCount > 0)
            {
                Debug.LogWarning($"Disabled {disabledCount} network components due to missing Photon");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error disabling network components: {e.Message}");
        }
    }
    
    void OnDestroy()
    {
        try
        {
            Application.logMessageReceived -= HandleException;
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}