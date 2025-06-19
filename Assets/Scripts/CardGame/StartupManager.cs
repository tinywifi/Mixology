#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Startup manager to ensure the game loads correctly and prevent crashes
public class StartupManager : MonoBehaviour
{
    [Header("Startup Configuration")]
    public string targetSceneName = "CardGame-Lobby";
    public bool enableRuntimeDiagnostics = true;
    public bool autoRedirectToGameScene = true;
    
    void Awake()
    {
        // Ensure this persists across scene loads
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("=== STARTUP MANAGER INITIALIZING ===");
        
        // Set up crash prevention immediately
        SetupCrashPrevention();
    }
    
    void Start()
    {
        Debug.Log($"Current scene: {SceneManager.GetActiveScene().name}");
        
        if (enableRuntimeDiagnostics)
        {
            RunStartupDiagnostics();
        }
        
        if (autoRedirectToGameScene)
        {
            CheckAndRedirectScene();
        }
    }
    
    private void SetupCrashPrevention()
    {
        // Prevent crashes from exceptions
        Application.logMessageReceived += HandleLogMessage;
        
        // Set safe Unity settings
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        
        Debug.Log("✓ Crash prevention enabled");
    }
    
    private void HandleLogMessage(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            Debug.LogError($"EXCEPTION CAUGHT: {logString}");
            Debug.LogError($"Stack: {stackTrace}");
            
            // Try to recover from critical exceptions
            if (logString.Contains("NullReferenceException") && logString.Contains("NetCardLobby"))
            {
                Debug.LogWarning("NetCardLobby crash detected - attempting recovery...");
                DisableNetworkingComponents();
            }
            
            if (logString.Contains("ModernUI") || logString.Contains("DOTween"))
            {
                Debug.LogWarning("UI system crash detected - attempting recovery...");
                DisableEnhancedUI();
            }
        }
    }
    
    private void RunStartupDiagnostics()
    {
        Debug.Log("=== STARTUP DIAGNOSTICS ===");
        
        try
        {
            // Basic environment check
            Debug.Log($"Unity Version: {Application.unityVersion}");
            Debug.Log($"Platform: {Application.platform}");
            Debug.Log($"Is Playing: {Application.isPlaying}");
            Debug.Log($"Current Scene: {SceneManager.GetActiveScene().name}");
            Debug.Log($"Scene Build Index: {SceneManager.GetActiveScene().buildIndex}");
            Debug.Log($"Total Scenes in Build: {SceneManager.sceneCountInBuildSettings}");
            
            // List all scenes in build
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                Debug.Log($"  Build Scene {i}: {sceneName}");
            }
            
            // Check components in current scene
            CheckSceneComponents();
            
            // Check dependencies
            CheckDependencies();
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Startup diagnostics failed: {e.Message}");
        }
        
        Debug.Log("=== STARTUP DIAGNOSTICS COMPLETE ===");
    }
    
    private void CheckSceneComponents()
    {
        try
        {
            // Count components
            MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
            
            Debug.Log($"Scene Components:");
            Debug.Log($"  Scripts: {scripts.Length}");
            Debug.Log($"  Canvases: {canvases.Length}");
            Debug.Log($"  EventSystems: {eventSystems.Length}");
            
            // Check for null scripts (missing references)
            int nullScripts = 0;
            foreach (MonoBehaviour script in scripts)
            {
                if (script == null) nullScripts++;
            }
            
            if (nullScripts > 0)
            {
                Debug.LogError($"❌ CRITICAL: {nullScripts} missing script references found!");
                Debug.LogError("This will cause crashes - need to fix missing scripts");
            }
            else
            {
                Debug.Log("✓ No missing script references");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Component check failed: {e.Message}");
        }
    }
    
    private void CheckDependencies()
    {
        Debug.Log("Checking Dependencies:");
        
        // DOTween
        bool dotweenAvailable = System.Type.GetType("DG.Tweening.DOTween") != null;
        Debug.Log($"  DOTween: {(dotweenAvailable ? "✓" : "❌")}");
        
        // Photon
        bool photonAvailable = System.Type.GetType("Photon.Pun.PhotonNetwork") != null;
        Debug.Log($"  Photon PUN2: {(photonAvailable ? "✓" : "❌")}");
        
        if (!dotweenAvailable)
        {
            Debug.LogWarning("DOTween missing - disabling enhanced UI components");
            DisableEnhancedUI();
        }
        
        if (!photonAvailable)
        {
            Debug.LogWarning("Photon missing - disabling networking components");
            DisableNetworkingComponents();
        }
    }
    
    private void CheckAndRedirectScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        Debug.Log($"Checking scene redirect from '{currentScene}' to '{targetSceneName}'");
        
        // If we're in a demo scene, redirect to the actual game
        if (currentScene.Contains("Demo") || currentScene.Contains("Asteroids"))
        {
            Debug.LogWarning($"Detected demo scene '{currentScene}' - redirecting to game scene");
            LoadTargetScene();
        }
        else if (currentScene != targetSceneName)
        {
            Debug.Log($"Not in target scene '{targetSceneName}' - checking if we should redirect");
            
            // Check if target scene exists in build
            bool targetExists = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (sceneName == targetSceneName)
                {
                    targetExists = true;
                    break;
                }
            }
            
            if (targetExists)
            {
                LoadTargetScene();
            }
            else
            {
                Debug.LogError($"Target scene '{targetSceneName}' not found in build!");
                CreateFallbackUI();
            }
        }
        else
        {
            Debug.Log($"Already in target scene '{targetSceneName}'");
            EnsureSceneHasBasicUI();
        }
    }
    
    private void LoadTargetScene()
    {
        try
        {
            Debug.Log($"Loading target scene: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load target scene: {e.Message}");
            
            // Try alternative scene names
            string[] alternatives = { "CardGame-Lobby", "Lobby", "MainMenu" };
            foreach (string alt in alternatives)
            {
                if (alt != targetSceneName)
                {
                    try
                    {
                        Debug.Log($"Trying alternative scene: {alt}");
                        SceneManager.LoadScene(alt);
                        return;
                    }
                    catch
                    {
                        Debug.LogWarning($"Alternative scene '{alt}' also failed");
                    }
                }
            }
            
            Debug.LogError("All scene loading attempts failed - creating fallback UI");
            CreateFallbackUI();
        }
    }
    
    private void EnsureSceneHasBasicUI()
    {
        // Make sure the scene has basic UI components
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        if (canvases.Length == 0)
        {
            Debug.LogWarning("No Canvas found - creating basic UI");
            CreateFallbackUI();
        }
        
        UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystems.Length == 0)
        {
            Debug.LogWarning("No EventSystem found - creating one");
            CreateEventSystem();
        }
    }
    
    private void DisableEnhancedUI()
    {
        try
        {
            // Find and disable enhanced UI components
            MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
            
            foreach (MonoBehaviour script in allScripts)
            {
                if (script == null) continue;
                
                string scriptType = script.GetType().Name;
                if (scriptType.Contains("Enhanced") || scriptType.Contains("Modern") || scriptType.Contains("DOTween"))
                {
                    script.enabled = false;
                    Debug.Log($"Disabled {scriptType} due to missing dependencies");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error disabling enhanced UI: {e.Message}");
        }
    }
    
    private void DisableNetworkingComponents()
    {
        try
        {
            // Find and disable networking components
            MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
            
            foreach (MonoBehaviour script in allScripts)
            {
                if (script == null) continue;
                
                string scriptType = script.GetType().Name;
                if (scriptType.Contains("Net") || scriptType.Contains("Photon") || scriptType.Contains("PUN"))
                {
                    script.enabled = false;
                    Debug.Log($"Disabled {scriptType} due to missing Photon dependency");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error disabling networking: {e.Message}");
        }
    }
    
    private void CreateFallbackUI()
    {
        try
        {
            Debug.Log("Creating fallback UI...");
            
            // Create canvas
            GameObject canvasObj = new GameObject("FallbackCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // Ensure it's on top
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Background
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(canvas.transform, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
            
            UnityEngine.UI.Image bgImage = bg.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.2f, 1f);
            
            // Title
            GameObject title = new GameObject("Title");
            title.transform.SetParent(canvas.transform, false);
            RectTransform titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.1f, 0.6f);
            titleRect.anchorMax = new Vector2(0.9f, 0.8f);
            titleRect.sizeDelta = Vector2.zero;
            titleRect.anchoredPosition = Vector2.zero;
            
            UnityEngine.UI.Text titleText = title.AddComponent<UnityEngine.UI.Text>();
            titleText.text = "CHEMISTRY PARTY\n(Safe Mode)";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 36;
            titleText.fontStyle = FontStyle.Bold;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            // Message
            GameObject message = new GameObject("Message");
            message.transform.SetParent(canvas.transform, false);
            RectTransform messageRect = message.AddComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0.1f, 0.4f);
            messageRect.anchorMax = new Vector2(0.9f, 0.6f);
            messageRect.sizeDelta = Vector2.zero;
            messageRect.anchoredPosition = Vector2.zero;
            
            UnityEngine.UI.Text messageText = message.AddComponent<UnityEngine.UI.Text>();
            messageText.text = "Some dependencies are missing, but the game is running.\nCheck the console for details.";
            messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            messageText.fontSize = 20;
            messageText.color = Color.yellow;
            messageText.alignment = TextAnchor.MiddleCenter;
            
            CreateEventSystem();
            
            Debug.Log("✓ Fallback UI created");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create fallback UI: {e.Message}");
        }
    }
    
    private void CreateEventSystem()
    {
        try
        {
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("✓ EventSystem created");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create EventSystem: {e.Message}");
        }
    }
    
    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLogMessage;
    }
}
#endif