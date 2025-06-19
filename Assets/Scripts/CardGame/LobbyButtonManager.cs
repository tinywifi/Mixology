using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Pun;

public class LobbyButtonManager : MonoBehaviour
{
    [Header("Button References")]
    public Button startGameButton;
    public Button multiplayerButton;
    public Button howToPlayButton;
    public Button settingsButton;
    public Button exitButton;
    
    [Header("Rules Panel")]
    public GameObject rulesPanel;
    
    void Start()
    {
        Debug.Log("LobbyButtonManager: Setting up button listeners...");
        
        // Auto-find buttons if not assigned
        if (startGameButton == null) startGameButton = GameObject.Find("Button_StartGame")?.GetComponent<Button>();
        if (multiplayerButton == null) multiplayerButton = GameObject.Find("Button_Multiplayer")?.GetComponent<Button>();
        if (howToPlayButton == null) howToPlayButton = GameObject.Find("Button_HowtoPlay")?.GetComponent<Button>();
        if (settingsButton == null) settingsButton = GameObject.Find("Button_Settings")?.GetComponent<Button>();
        if (exitButton == null) exitButton = GameObject.Find("Button_Exit")?.GetComponent<Button>();
        
        // Setup button listeners
        SetupButtonListeners();
        
        Debug.Log("LobbyButtonManager: Button setup complete!");
    }
    
    void SetupButtonListeners()
    {
        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(StartGame);
            Debug.Log("✓ Start Game button listener added");
        }
        else
        {
            Debug.LogWarning("Start Game button not found!");
        }
        
        if (multiplayerButton != null)
        {
            multiplayerButton.onClick.RemoveAllListeners();
            multiplayerButton.onClick.AddListener(StartMultiplayer);
            Debug.Log("✓ Multiplayer button listener added");
        }
        
        if (howToPlayButton != null)
        {
            howToPlayButton.onClick.RemoveAllListeners();
            howToPlayButton.onClick.AddListener(ShowRules);
            Debug.Log("✓ How to Play button listener added");
        }
        
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(ShowSettings);
            Debug.Log("✓ Settings button listener added");
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(ExitGame);
            Debug.Log("✓ Exit button listener added");
        }
    }
    
    public void StartGame()
    {
        Debug.Log("=== START GAME CLICKED ===");
        
        try
        {
            // Try to find existing lobby manager first
            ChemistryLobbyManager lobbyManager = FindObjectOfType<ChemistryLobbyManager>();
            if (lobbyManager != null)
            {
                Debug.Log("Found ChemistryLobbyManager, checking for start button...");
                
                // Find the original start button and invoke it
                Button originalStartButton = lobbyManager.GetComponentInChildren<Button>();
                if (originalStartButton != null && originalStartButton.gameObject.name.ToLower().Contains("start"))
                {
                    Debug.Log("Invoking original start button...");
                    originalStartButton.onClick.Invoke();
                    return;
                }
            }
            
            // Validate chemistry database
            ValidateChemistryDatabase();
            
            // Load game scene
            Debug.Log("Loading CardGame-Board scene...");
            SceneManager.LoadScene("CardGame-Board");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error starting game: {e.Message}");
            
            // Try alternative scene name
            try
            {
                Debug.Log("Trying alternative scene name 'Board'...");
                SceneManager.LoadScene("Board");
            }
            catch
            {
                Debug.LogError("Failed to load game scene. Check Build Settings.");
                ShowErrorMessage("Game scene not found in Build Settings!");
            }
        }
    }
    
    public void StartMultiplayer()
    {
        Debug.Log("=== MULTIPLAYER CLICKED ===");
        
        try
        {
            NetCardLobby netLobby = FindObjectOfType<NetCardLobby>();
            if (netLobby != null)
            {
                Debug.Log("NetCardLobby found, checking Photon connection...");
                
                if (!PhotonNetwork.IsConnected)
                {
                    Debug.Log("Connecting to Photon...");
                    ShowMultiplayerStatus("Connecting to Photon...");
                    PhotonNetwork.ConnectUsingSettings();
                }
                else
                {
                    Debug.Log("Already connected to Photon, joining room...");
                    ShowMultiplayerStatus("Joining room...");
                    PhotonNetwork.JoinRandomRoom();
                }
            }
            else
            {
                Debug.LogWarning("NetCardLobby component not found in scene!");
                ShowErrorMessage("Multiplayer not configured. Starting single player game...");
                
                // Wait a moment then start single player
                Invoke("StartGame", 2f);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error starting multiplayer: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            
            ShowErrorMessage("Multiplayer error occurred. Starting single player game...");
            Invoke("StartGame", 2f);
        }
    }
    
    public void ShowRules()
    {
        Debug.Log("=== HOW TO PLAY CLICKED ===");
        
        if (rulesPanel != null)
        {
            rulesPanel.SetActive(true);
            Debug.Log("Rules panel shown");
        }
        else
        {
            // Create a simple rules display
            ShowSimpleRules();
        }
    }
    
    public void ShowSettings()
    {
        Debug.Log("=== SETTINGS CLICKED ===");
        Debug.Log("Settings functionality not implemented yet");
        
        // For now, just show a debug message
        ShowErrorMessage("Settings menu coming soon!");
    }
    
    public void ExitGame()
    {
        Debug.Log("=== EXIT CLICKED ===");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Stopping play mode in editor");
        #else
        Application.Quit();
        Debug.Log("Quitting application");
        #endif
    }
    
    void ValidateChemistryDatabase()
    {
        ChemistryDatabase database = FindObjectOfType<ChemistryDatabase>();
        if (database == null)
        {
            database = Resources.Load<ChemistryDatabase>("ChemistryDatabase");
        }
        
        if (database == null || database.allElements.Count == 0)
        {
            Debug.LogWarning("Chemistry database not properly set up!");
        }
        else
        {
            Debug.Log($"Chemistry database validated: {database.allElements.Count} elements found");
        }
    }
    
    void ShowSimpleRules()
    {
        // Create a simple rules popup
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        GameObject rulesOverlay = new GameObject("RulesOverlay");
        rulesOverlay.transform.SetParent(canvas.transform);
        
        RectTransform overlayRect = rulesOverlay.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        overlayRect.anchoredPosition = Vector2.zero;
        overlayRect.localScale = Vector3.one;
        
        Image overlayImage = rulesOverlay.AddComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0.8f);
        
        // Rules panel
        GameObject rulesPanel = new GameObject("RulesPanel");
        rulesPanel.transform.SetParent(rulesOverlay.transform);
        
        RectTransform panelRect = rulesPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.2f, 0.2f);
        panelRect.anchorMax = new Vector2(0.8f, 0.8f);
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.localScale = Vector3.one;
        
        Image panelImage = rulesPanel.AddComponent<Image>();
        panelImage.color = new Color(0.2f, 0.2f, 0.3f, 1f);
        
        // Rules text
        GameObject rulesTextObj = new GameObject("RulesText");
        rulesTextObj.transform.SetParent(rulesPanel.transform);
        
        RectTransform textRect = rulesTextObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.2f);
        textRect.anchorMax = new Vector2(0.9f, 0.8f);
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        textRect.localScale = Vector3.one;
        
        Text rulesText = rulesTextObj.AddComponent<Text>();
        rulesText.text = "🧪 CHEMISTRY PARTY RULES\n\n" +
                         "1. SELECT ELEMENTS from your hand\n" +
                         "2. CREATE COMPOUNDS by combining elements\n" +
                         "3. DISCARD unwanted cards to draw new ones\n" +
                         "4. First to collect 8 compounds WINS!";
        rulesText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        rulesText.fontSize = 24;
        rulesText.color = Color.white;
        rulesText.alignment = TextAnchor.UpperLeft;
        
        // Close button
        GameObject closeButton = new GameObject("CloseButton");
        closeButton.transform.SetParent(rulesPanel.transform);
        
        RectTransform closeRect = closeButton.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.4f, 0.05f);
        closeRect.anchorMax = new Vector2(0.6f, 0.15f);
        closeRect.sizeDelta = Vector2.zero;
        closeRect.anchoredPosition = Vector2.zero;
        closeRect.localScale = Vector3.one;
        
        Image closeImage = closeButton.AddComponent<Image>();
        closeImage.color = Color.red;
        
        Button closeBtn = closeButton.AddComponent<Button>();
        closeBtn.targetGraphic = closeImage;
        closeBtn.onClick.AddListener(() => {
            Destroy(rulesOverlay);
        });
        
        // Close button text
        GameObject closeTextObj = new GameObject("Text");
        closeTextObj.transform.SetParent(closeButton.transform);
        
        RectTransform closeTextRect = closeTextObj.AddComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.sizeDelta = Vector2.zero;
        closeTextRect.anchoredPosition = Vector2.zero;
        closeTextRect.localScale = Vector3.one;
        
        Text closeText = closeTextObj.AddComponent<Text>();
        closeText.text = "CLOSE";
        closeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        closeText.fontSize = 20;
        closeText.color = Color.white;
        closeText.alignment = TextAnchor.MiddleCenter;
        closeText.fontStyle = FontStyle.Bold;
        
        Debug.Log("Simple rules panel created");
    }
    
    void ShowErrorMessage(string message)
    {
        Debug.LogError($"ERROR: {message}");
        
        // You could create a UI popup here if needed
        // For now, just log the error
    }
    
    void ShowMultiplayerStatus(string message)
    {
        Debug.Log($"MULTIPLAYER STATUS: {message}");
        
        // Try to find and update any status text in the UI
        Text[] statusTexts = FindObjectsOfType<Text>();
        foreach (Text text in statusTexts)
        {
            if (text.gameObject.name.ToLower().Contains("status") || 
                text.gameObject.name.ToLower().Contains("info"))
            {
                text.text = message;
                break;
            }
        }
    }
    
    [ContextMenu("Test All Buttons")]
    public void TestAllButtons()
    {
        Debug.Log("=== TESTING ALL BUTTONS ===");
        
        if (startGameButton != null)
        {
            Debug.Log("Testing Start Game button...");
            StartGame();
        }
        
        Debug.Log("=== BUTTON TESTS COMPLETE ===");
    }
}