using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Ultra-safe lobby that should never crash
public class SafeLobby : MonoBehaviour
{
    [Header("Safe Lobby Configuration")]
    public Canvas targetCanvas;
    public bool createLobbyOnStart = true;
    
    void Start()
    {
        Debug.Log("=== SAFE LOBBY STARTING ===");
        
        try
        {
            if (createLobbyOnStart)
            {
                CreateSafeLobby();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Safe Lobby Error: {e.Message}");
            CreateFallbackUI();
        }
        
        Debug.Log("=== SAFE LOBBY READY ===");
    }
    
    public void CreateSafeLobby()
    {
        Debug.Log("Creating ultra-safe lobby...");
        
        try
        {
            // Ensure we have a canvas
            if (targetCanvas == null)
            {
                targetCanvas = FindObjectOfType<Canvas>();
                if (targetCanvas == null)
                {
                    CreateCanvas();
                }
            }
            
            // Clear existing UI safely
            ClearCanvasSafely();
            
            // Create minimal, safe UI
            CreateSafeUI();
            
            Debug.Log("✓ Safe lobby created successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating safe lobby: {e.Message}");
            CreateFallbackUI();
        }
    }
    
    private void CreateCanvas()
    {
        try
        {
            GameObject canvasObj = new GameObject("SafeCanvas");
            targetCanvas = canvasObj.AddComponent<Canvas>();
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            targetCanvas.sortingOrder = 100; // Ensure it's on top
            
            // Add required components
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Ensure EventSystem exists
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            Debug.Log("✓ Safe canvas created");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create canvas: {e.Message}");
        }
    }
    
    private void ClearCanvasSafely()
    {
        if (targetCanvas == null) return;
        
        try
        {
            // Safe clearing that works in builds
            for (int i = targetCanvas.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = targetCanvas.transform.GetChild(i);
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error clearing canvas: {e.Message}");
        }
    }
    
    private void CreateSafeUI()
    {
        try
        {
            // Background
            CreateBackground();
            
            // Title
            CreateTitle();
            
            // Safe buttons
            CreateSafeButtons();
            
            Debug.Log("✓ Safe UI components created");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating safe UI: {e.Message}");
        }
    }
    
    private void CreateBackground()
    {
        try
        {
            GameObject bg = new GameObject("SafeBackground");
            bg.transform.SetParent(targetCanvas.transform, false);
            
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
            
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.2f, 1f); // Dark blue
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating background: {e.Message}");
        }
    }
    
    private void CreateTitle()
    {
        try
        {
            GameObject titleObj = new GameObject("SafeTitle");
            titleObj.transform.SetParent(targetCanvas.transform, false);
            
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.1f, 0.8f);
            titleRect.anchorMax = new Vector2(0.9f, 0.95f);
            titleRect.sizeDelta = Vector2.zero;
            titleRect.anchoredPosition = Vector2.zero;
            
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "CHEMISTRY PARTY";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 48;
            titleText.fontStyle = FontStyle.Bold;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating title: {e.Message}");
        }
    }
    
    private void CreateSafeButtons()
    {
        try
        {
            // Container
            GameObject container = new GameObject("SafeButtonContainer");
            container.transform.SetParent(targetCanvas.transform, false);
            
            RectTransform containerRect = container.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.3f, 0.3f);
            containerRect.anchorMax = new Vector2(0.7f, 0.7f);
            containerRect.sizeDelta = Vector2.zero;
            containerRect.anchoredPosition = Vector2.zero;
            
            VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 20f;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            
            // Create buttons
            CreateSafeButton("START GAME", container.transform, LoadGameScene, Color.green);
            CreateSafeButton("SHOW LOG", container.transform, ShowDebugLog, Color.blue);
            CreateSafeButton("QUIT", container.transform, QuitGame, Color.red);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating buttons: {e.Message}");
        }
    }
    
    private void CreateSafeButton(string text, Transform parent, System.Action onClick, Color color)
    {
        try
        {
            GameObject buttonObj = new GameObject($"SafeButton_{text.Replace(" ", "")}");
            buttonObj.transform.SetParent(parent, false);
            
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0, 60);
            
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = color;
            
            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(() => {
                Debug.Log($"Safe button clicked: {text}");
                try
                {
                    onClick?.Invoke();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error in button action: {e.Message}");
                }
            });
            
            // Button text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            Text buttonText = textObj.AddComponent<Text>();
            buttonText.text = text;
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            buttonText.fontSize = 24;
            buttonText.fontStyle = FontStyle.Bold;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.raycastTarget = false;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating button {text}: {e.Message}");
        }
    }
    
    private void LoadGameScene()
    {
        Debug.Log("Attempting to load game scene...");
        
        try
        {
            // Try multiple scene names
            string[] possibleScenes = { "CardGame-Board", "Board", "GameBoard" };
            
            foreach (string sceneName in possibleScenes)
            {
                try
                {
                    SceneManager.LoadScene(sceneName);
                    Debug.Log($"Successfully loaded scene: {sceneName}");
                    return;
                }
                catch
                {
                    Debug.LogWarning($"Failed to load scene: {sceneName}");
                }
            }
            
            // Try loading by index
            if (SceneManager.sceneCountInBuildSettings > 1)
            {
                SceneManager.LoadScene(1);
                Debug.Log("Loaded scene by index");
            }
            else
            {
                Debug.LogError("No game scene found!");
                ShowMessage("No game scene found in build!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading game scene: {e.Message}");
            ShowMessage($"Error loading scene: {e.Message}");
        }
    }
    
    private void ShowDebugLog()
    {
        Debug.Log("=== DEBUG LOG ===");
        Debug.Log($"Unity Version: {Application.unityVersion}");
        Debug.Log($"Platform: {Application.platform}");
        Debug.Log($"Current Scene: {SceneManager.GetActiveScene().name}");
        Debug.Log($"Scenes in Build: {SceneManager.sceneCountInBuildSettings}");
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"  Scene {i}: {sceneName}");
        }
        
        Debug.Log("=== END DEBUG LOG ===");
    }
    
    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    private void ShowMessage(string message)
    {
        Debug.Log($"MESSAGE: {message}");
        // In a real implementation, you could show this in the UI
    }
    
    private void CreateFallbackUI()
    {
        Debug.LogWarning("Creating fallback UI due to errors...");
        
        try
        {
            // Create the most basic UI possible
            GameObject fallbackObj = new GameObject("FallbackUI");
            Text fallbackText = fallbackObj.AddComponent<Text>();
            fallbackText.text = "SAFE MODE - Check Console for Errors";
            fallbackText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            fallbackText.fontSize = 32;
            fallbackText.color = Color.red;
            fallbackText.alignment = TextAnchor.MiddleCenter;
            
            if (targetCanvas != null)
            {
                fallbackObj.transform.SetParent(targetCanvas.transform, false);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Even fallback UI failed: {e.Message}");
        }
    }
}