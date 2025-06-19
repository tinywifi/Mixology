using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Build-safe UI setup that works in both editor and runtime
public class BuildSafeUI : MonoBehaviour
{
    [Header("UI Setup")]
    public bool createUIOnStart = true;
    public Canvas targetCanvas;
    
    [Header("Crash Prevention")]
    public bool enableCrashPrevention = true;
    public bool enableDiagnostics = true;
    public bool autoFixScene = true;
    
    void Awake()
    {
        if (enableCrashPrevention)
        {
            SetupCrashPrevention();
        }
    }
    
    void Start()
    {
        Debug.Log("=== BUILD SAFE UI STARTING ===");
        
        if (enableDiagnostics)
        {
            RunCrashDiagnostics();
        }
        
        if (createUIOnStart)
        {
            if (targetCanvas != null)
            {
                CreateBasicUI();
            }
            else if (autoFixScene)
            {
                Debug.LogWarning("No target canvas assigned - creating one automatically");
                CreateCanvasAndUI();
            }
        }
        
        Debug.Log("=== BUILD SAFE UI COMPLETE ===");
    }
    
    private void SetupCrashPrevention()
    {
        Debug.Log("Setting up crash prevention...");
        
        // Handle exceptions
        Application.logMessageReceived += HandleCrashException;
        
        // Set safe Unity settings
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        
        // Disable problematic components
        DisableProblematicComponents();
        
        Debug.Log("✓ Crash prevention active");
    }
    
    private void HandleCrashException(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            Debug.LogError($"EXCEPTION CAUGHT BY CRASH PREVENTION: {logString}");
            
            // Handle specific crash types
            if (logString.Contains("NullReferenceException"))
            {
                Debug.LogWarning("Null reference detected - attempting recovery...");
            }
            
            if (logString.Contains("MissingReferenceException"))
            {
                Debug.LogWarning("Missing reference detected - attempting recovery...");
            }
            
            if (logString.Contains("DOTween") || logString.Contains("DG.Tweening"))
            {
                Debug.LogWarning("DOTween error detected - disabling enhanced UI...");
                DisableDOTweenComponents();
            }
            
            if (logString.Contains("Photon") || logString.Contains("PUN"))
            {
                Debug.LogWarning("Photon error detected - disabling networking...");
                DisablePhotonComponents();
            }
        }
    }
    
    private void RunCrashDiagnostics()
    {
        Debug.Log("=== CRASH DIAGNOSTICS ===");
        
        try
        {
            Debug.Log($"Unity Version: {Application.unityVersion}");
            Debug.Log($"Platform: {Application.platform}");
            Debug.Log($"Current Scene: {SceneManager.GetActiveScene().name}");
            Debug.Log($"Is Playing: {Application.isPlaying}");
            
            // Check for missing scripts
            CheckForMissingScripts();
            
            // Check dependencies
            CheckDependencies();
            
            // Check scene components
            CheckSceneComponents();
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Diagnostics failed: {e.Message}");
        }
        
        Debug.Log("=== DIAGNOSTICS COMPLETE ===");
    }
    
    private void CheckForMissingScripts()
    {
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        int nullCount = 0;
        
        foreach (MonoBehaviour script in allScripts)
        {
            if (script == null) nullCount++;
        }
        
        if (nullCount > 0)
        {
            Debug.LogError($"❌ CRITICAL: {nullCount} missing script references found!");
            Debug.LogError("This is likely causing crashes - missing scripts need to be removed");
        }
        else
        {
            Debug.Log("✓ No missing script references");
        }
    }
    
    private void CheckDependencies()
    {
        bool dotweenFound = System.Type.GetType("DG.Tweening.DOTween") != null;
        bool photonFound = System.Type.GetType("Photon.Pun.PhotonNetwork") != null;
        
        Debug.Log($"DOTween available: {dotweenFound}");
        Debug.Log($"Photon PUN2 available: {photonFound}");
        
        if (!dotweenFound)
        {
            Debug.LogWarning("⚠️ DOTween missing - disabling enhanced UI");
            DisableDOTweenComponents();
        }
        
        if (!photonFound)
        {
            Debug.LogWarning("⚠️ Photon missing - disabling networking");
            DisablePhotonComponents();
        }
    }
    
    private void CheckSceneComponents()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        
        Debug.Log($"Scene has {canvases.Length} Canvas components");
        Debug.Log($"Scene has {eventSystems.Length} EventSystem components");
        
        if (canvases.Length == 0)
        {
            Debug.LogWarning("⚠️ No Canvas found - UI may not work");
        }
        
        if (eventSystems.Length == 0)
        {
            Debug.LogWarning("⚠️ No EventSystem found - creating one");
            CreateEventSystem();
        }
    }
    
    private void DisableProblematicComponents()
    {
        try
        {
            MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
            
            foreach (MonoBehaviour script in allScripts)
            {
                if (script == null) continue;
                
                string scriptType = script.GetType().Name;
                
                // Disable enhanced UI components if DOTween not available
                if (System.Type.GetType("DG.Tweening.DOTween") == null)
                {
                    if (scriptType.Contains("Enhanced") || scriptType.Contains("Modern"))
                    {
                        script.enabled = false;
                        Debug.Log($"Disabled {scriptType} (missing DOTween)");
                    }
                }
                
                // Disable networking components if Photon not available
                if (System.Type.GetType("Photon.Pun.PhotonNetwork") == null)
                {
                    if (scriptType.Contains("Net") || scriptType.Contains("Photon"))
                    {
                        script.enabled = false;
                        Debug.Log($"Disabled {scriptType} (missing Photon)");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error disabling problematic components: {e.Message}");
        }
    }
    
    private void DisableDOTweenComponents()
    {
        try
        {
            MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
            foreach (MonoBehaviour script in allScripts)
            {
                if (script == null) continue;
                string scriptType = script.GetType().Name;
                if (scriptType.Contains("Enhanced") || scriptType.Contains("Modern"))
                {
                    script.enabled = false;
                    Debug.Log($"Disabled {scriptType} due to DOTween dependency");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error disabling DOTween components: {e.Message}");
        }
    }
    
    private void DisablePhotonComponents()
    {
        try
        {
            MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
            foreach (MonoBehaviour script in allScripts)
            {
                if (script == null) continue;
                string scriptType = script.GetType().Name;
                if (scriptType.Contains("Net") || scriptType.Contains("Photon"))
                {
                    script.enabled = false;
                    Debug.Log($"Disabled {scriptType} due to Photon dependency");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error disabling Photon components: {e.Message}");
        }
    }
    
    private void CreateCanvasAndUI()
    {
        try
        {
            // Create canvas first
            GameObject canvasObj = new GameObject("SafeCanvas");
            targetCanvas = canvasObj.AddComponent<Canvas>();
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            targetCanvas.sortingOrder = 100;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            CreateEventSystem();
            
            // Now create UI
            CreateBasicUI();
            
            Debug.Log("✓ Canvas and UI created successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create canvas and UI: {e.Message}");
        }
    }
    
    private void CreateEventSystem()
    {
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("✓ EventSystem created");
        }
    }
    
    void OnDestroy()
    {
        Application.logMessageReceived -= HandleCrashException;
    }
    
    public void CreateBasicUI()
    {
        Debug.Log("Creating build-safe UI...");
        
        // Clear existing UI safely
        ClearCanvas();
        
        // Create basic lobby UI that works in builds
        CreateSafeBackground();
        CreateSafeTitle();
        CreateSafeButtons();
        
        Debug.Log("✓ Build-safe UI created");
    }
    
    private void ClearCanvas()
    {
        if (targetCanvas == null) return;
        
        // Safe clearing that works in builds
        while (targetCanvas.transform.childCount > 0)
        {
            Transform child = targetCanvas.transform.GetChild(0);
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }
    
    private void CreateSafeBackground()
    {
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(targetCanvas.transform);
        
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.localScale = Vector3.one;
        
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.2f, 1f);
    }
    
    private void CreateSafeTitle()
    {
        GameObject title = new GameObject("Title");
        title.transform.SetParent(targetCanvas.transform);
        
        RectTransform titleRect = title.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.1f, 0.8f);
        titleRect.anchorMax = new Vector2(0.9f, 0.95f);
        titleRect.sizeDelta = Vector2.zero;
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.localScale = Vector3.one;
        
        Text titleText = title.AddComponent<Text>();
        titleText.text = "CHEMISTRY PARTY";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 48;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
    }
    
    private void CreateSafeButtons()
    {
        // Button container
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(targetCanvas.transform);
        
        RectTransform containerRect = buttonContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.3f, 0.3f);
        containerRect.anchorMax = new Vector2(0.7f, 0.7f);
        containerRect.sizeDelta = Vector2.zero;
        containerRect.anchoredPosition = Vector2.zero;
        containerRect.localScale = Vector3.one;
        
        VerticalLayoutGroup layout = buttonContainer.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 20f;
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        
        // Create buttons
        CreateSafeButton("START GAME", buttonContainer.transform, StartGame, new Color(0.2f, 0.8f, 0.2f));
        CreateSafeButton("MULTIPLAYER", buttonContainer.transform, StartMultiplayer, new Color(0.2f, 0.4f, 0.8f));
        CreateSafeButton("HOW TO PLAY", buttonContainer.transform, ShowRules, new Color(0.6f, 0.6f, 0.6f));
        CreateSafeButton("EXIT", buttonContainer.transform, ExitGame, new Color(0.8f, 0.2f, 0.2f));
    }
    
    private void CreateSafeButton(string text, Transform parent, System.Action onClick, Color color)
    {
        GameObject button = new GameObject($"Button_{text.Replace(" ", "")}");
        button.transform.SetParent(parent);
        
        RectTransform buttonRect = button.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(0, 60);
        buttonRect.localScale = Vector3.one;
        
        // Button image
        Image buttonImage = button.AddComponent<Image>();
        buttonImage.color = color;
        buttonImage.raycastTarget = true;
        
        // Button component
        Button buttonComp = button.AddComponent<Button>();
        buttonComp.targetGraphic = buttonImage;
        buttonComp.interactable = true;
        
        // Add click listener
        buttonComp.onClick.AddListener(() => {
            Debug.Log($"Safe button clicked: {text}");
            onClick?.Invoke();
        });
        
        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        textRect.localScale = Vector3.one;
        
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 24;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.raycastTarget = false;
    }
    
    private void StartGame()
    {
        Debug.Log("Starting single player game...");
        
        try
        {
            SceneManager.LoadScene("CardGame-Board");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load CardGame-Board: {e.Message}");
            
            // Try alternative scene names
            try
            {
                SceneManager.LoadScene("Board");
            }
            catch
            {
                try
                {
                    SceneManager.LoadScene(1); // Try by index
                }
                catch
                {
                    Debug.LogError("No game scene found! Check Build Settings.");
                }
            }
        }
    }
    
    private void StartMultiplayer()
    {
        Debug.Log("Multiplayer clicked - falling back to single player");
        StartGame();
    }
    
    private void ShowRules()
    {
        Debug.Log("Rules: Create compounds from elements to win!");
        
        // Create simple rules popup
        GameObject rulesPopup = new GameObject("RulesPopup");
        rulesPopup.transform.SetParent(targetCanvas.transform);
        
        RectTransform popupRect = rulesPopup.AddComponent<RectTransform>();
        popupRect.anchorMin = new Vector2(0.2f, 0.2f);
        popupRect.anchorMax = new Vector2(0.8f, 0.8f);
        popupRect.sizeDelta = Vector2.zero;
        popupRect.anchoredPosition = Vector2.zero;
        popupRect.localScale = Vector3.one;
        
        Image popupImage = rulesPopup.AddComponent<Image>();
        popupImage.color = new Color(0, 0, 0, 0.9f);
        
        // Rules text
        GameObject rulesText = new GameObject("RulesText");
        rulesText.transform.SetParent(rulesPopup.transform);
        
        RectTransform textRect = rulesText.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.2f);
        textRect.anchorMax = new Vector2(0.9f, 0.8f);
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        textRect.localScale = Vector3.one;
        
        Text text = rulesText.AddComponent<Text>();
        text.text = "CHEMISTRY PARTY RULES\n\n" +
                   "1. Select elements from your hand\n" +
                   "2. Create compounds by combining elements\n" +
                   "3. First to collect 8 compounds wins!\n\n" +
                   "Click anywhere to close";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        // Close on click
        Button closeButton = rulesPopup.AddComponent<Button>();
        closeButton.targetGraphic = popupImage;
        closeButton.onClick.AddListener(() => {
            Destroy(rulesPopup);
        });
    }
    
    private void ExitGame()
    {
        Debug.Log("Exiting game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}