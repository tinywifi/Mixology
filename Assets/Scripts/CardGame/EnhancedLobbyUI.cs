using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using Photon.Pun;

#if UNITY_EDITOR
using UnityEditor;

public class EnhancedLobbyUI : MonoBehaviour
{
    [Header("Target Canvas")]
    public Canvas targetCanvas;
    
    [Header("Theme Settings")]
    public ModernUITheme uiTheme;
    
    [ContextMenu("Create Enhanced Lobby UI")]
    public void CreateEnhancedLobbyUI()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("Target Canvas not assigned!");
            return;
        }
        
        // Ensure we have the theme component
        if (uiTheme == null)
        {
            uiTheme = FindObjectOfType<ModernUITheme>();
            if (uiTheme == null)
            {
                GameObject themeObj = new GameObject("ModernUITheme");
                uiTheme = themeObj.AddComponent<ModernUITheme>();
            }
        }
        
        // Clear existing UI
        ClearExistingUI();
        
        // Setup canvas
        SetupCanvas();
        
        // Create main structure
        CreateBackgroundLayer();
        CreateMainMenuPanel();
        CreateElementsShowcase();
        CreateFooter();
        
        // Add the runtime button manager
        LobbyButtonManager buttonManager = targetCanvas.GetComponent<LobbyButtonManager>();
        if (buttonManager == null)
        {
            buttonManager = targetCanvas.gameObject.AddComponent<LobbyButtonManager>();
            Debug.Log("✓ LobbyButtonManager added for runtime button functionality");
        }
        
        Debug.Log("✓ Enhanced Lobby UI Created Successfully!");
        Debug.Log("IMPORTANT: Buttons will work when you PLAY the scene, not in edit mode!");
        EditorUtility.SetDirty(targetCanvas.gameObject);
    }
    
    private void ClearExistingUI()
    {
        // Remove old UI elements
        for (int i = targetCanvas.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = targetCanvas.transform.GetChild(i).gameObject;
            #if UNITY_EDITOR
            DestroyImmediate(child);
            #else
            Destroy(child);
            #endif
        }
    }
    
    private void SetupCanvas()
    {
        // Configure canvas scaler
        CanvasScaler scaler = targetCanvas.GetComponent<CanvasScaler>();
        if (scaler == null)
            scaler = targetCanvas.gameObject.AddComponent<CanvasScaler>();
            
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        // Configure GraphicRaycaster
        GraphicRaycaster raycaster = targetCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
            raycaster = targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
            
        // Ensure EventSystem exists
        UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("✓ EventSystem created for UI interaction");
        }
    }
    
    private void CreateBackgroundLayer()
    {
        // Main background
        GameObject backgroundObj = CreateUIElement("Background", targetCanvas.transform);
        RectTransform bgRect = backgroundObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        // Add gradient background
        Image bgImage = backgroundObj.AddComponent<Image>();
        Texture2D gradientTexture = uiTheme.CreateGradientTexture(
            uiTheme.colorPalette.backgroundColor,
            new Color(0.1f, 0.15f, 0.25f, 1f),
            256, 256
        );
        bgImage.sprite = Sprite.Create(gradientTexture, new Rect(0, 0, 256, 256), Vector2.one * 0.5f);
        
        // Add background particles effect
        CreateBackgroundParticles(backgroundObj.transform);
    }
    
    private void CreateBackgroundParticles(Transform parent)
    {
        GameObject particlesObj = CreateUIElement("BackgroundParticles", parent);
        RectTransform particlesRect = particlesObj.GetComponent<RectTransform>();
        particlesRect.anchorMin = Vector2.zero;
        particlesRect.anchorMax = Vector2.one;
        particlesRect.sizeDelta = Vector2.zero;
        particlesRect.anchoredPosition = Vector2.zero;
        
        // Create floating chemistry symbols
        string[] symbols = { "H", "O", "C", "N", "Na", "Cl", "Ca", "K" };
        for (int i = 0; i < 15; i++)
        {
            GameObject symbolObj = CreateUIElement($"Symbol_{i}", particlesObj.transform);
            Text symbolText = symbolObj.AddComponent<Text>();
            symbolText.text = symbols[Random.Range(0, symbols.Length)];
            symbolText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            symbolText.fontSize = Random.Range(20, 40);
            symbolText.color = new Color(
                uiTheme.colorPalette.primaryColor.r,
                uiTheme.colorPalette.primaryColor.g,
                uiTheme.colorPalette.primaryColor.b,
                Random.Range(0.1f, 0.3f)
            );
            symbolText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform symbolRect = symbolObj.GetComponent<RectTransform>();
            symbolRect.sizeDelta = new Vector2(50, 50);
            symbolRect.anchoredPosition = new Vector2(
                Random.Range(-960f, 960f),
                Random.Range(-540f, 540f)
            );
            
            // Animate floating
            symbolRect.DOAnchorPosY(symbolRect.anchoredPosition.y + Random.Range(50f, 150f), Random.Range(3f, 8f))
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetDelay(Random.Range(0f, 2f));
                
            symbolText.DOFade(symbolText.color.a * 0.5f, Random.Range(2f, 4f))
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetDelay(Random.Range(0f, 1f));
        }
    }
    
    private void CreateMainMenuPanel()
    {
        // Main menu container
        GameObject menuPanel = CreateUIElement("MainMenuPanel", targetCanvas.transform);
        RectTransform menuRect = menuPanel.GetComponent<RectTransform>();
        menuRect.anchorMin = new Vector2(0.3f, 0.2f);
        menuRect.anchorMax = new Vector2(0.7f, 0.8f);
        menuRect.sizeDelta = Vector2.zero;
        menuRect.anchoredPosition = Vector2.zero;
        
        // Add modern panel component
        ModernPanel panelComponent = menuPanel.AddComponent<ModernPanel>();
        panelComponent.panelStyle = ModernPanel.PanelStyle.Surface;
        panelComponent.enableAnimatedEntrance = true;
        panelComponent.addGlowEffect = true;
        
        // Title
        GameObject titleObj = CreateUIElement("Title", menuPanel.transform);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "Mixology";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 56;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = uiTheme.colorPalette.textPrimary;
        titleText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.1f, 0.8f);
        titleRect.anchorMax = new Vector2(0.9f, 0.95f);
        titleRect.sizeDelta = Vector2.zero;
        titleRect.anchoredPosition = Vector2.zero;
        
        ModernText titleModern = titleObj.AddComponent<ModernText>();
        titleModern.textStyle = ModernText.TextStyle.Title;
        titleModern.enableTypewriterEffect = true;
        
        // Subtitle
        GameObject subtitleObj = CreateUIElement("Subtitle", menuPanel.transform);
        Text subtitleText = subtitleObj.AddComponent<Text>();
        subtitleText.text = "Master the Elements • Create Compounds • Win the Game";
        subtitleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        subtitleText.fontSize = 24;
        subtitleText.color = uiTheme.colorPalette.textSecondary;
        subtitleText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform subtitleRect = subtitleObj.GetComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0.1f, 0.7f);
        subtitleRect.anchorMax = new Vector2(0.9f, 0.8f);
        subtitleRect.sizeDelta = Vector2.zero;
        subtitleRect.anchoredPosition = Vector2.zero;
        
        ModernText subtitleModern = subtitleObj.AddComponent<ModernText>();
        subtitleModern.textStyle = ModernText.TextStyle.Caption;
        
        // Buttons container
        GameObject buttonsContainer = CreateUIElement("ButtonsContainer", menuPanel.transform);
        RectTransform buttonsRect = buttonsContainer.GetComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(0.2f, 0.3f);
        buttonsRect.anchorMax = new Vector2(0.8f, 0.7f);
        buttonsRect.sizeDelta = Vector2.zero;
        buttonsRect.anchoredPosition = Vector2.zero;
        
        VerticalLayoutGroup buttonsLayout = buttonsContainer.AddComponent<VerticalLayoutGroup>();
        buttonsLayout.spacing = 20f;
        buttonsLayout.childControlHeight = false;
        buttonsLayout.childControlWidth = true;
        buttonsLayout.childForceExpandHeight = false;
        buttonsLayout.childForceExpandWidth = true;
        
        // Create buttons
        CreateMenuButton("Start Game", buttonsContainer.transform, () => StartSinglePlayerGame(), ModernButton.ButtonStyle.Primary);
        CreateMenuButton("Multiplayer", buttonsContainer.transform, () => StartMultiplayerGame(), ModernButton.ButtonStyle.Secondary);
        CreateMenuButton("How to Play", buttonsContainer.transform, () => ShowRulesPanel(), ModernButton.ButtonStyle.Ghost);
        CreateMenuButton("Settings", buttonsContainer.transform, () => Debug.Log("Settings clicked"), ModernButton.ButtonStyle.Ghost);
        CreateMenuButton("Exit", buttonsContainer.transform, () => ExitGame(), ModernButton.ButtonStyle.Error);
    }
    
    private GameObject CreateMenuButton(string text, Transform parent, System.Action onClick, ModernButton.ButtonStyle style)
    {
        GameObject buttonObj = CreateUIElement($"Button_{text.Replace(" ", "")}", parent);
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(0, 60);
        
        // Add button image first
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.4f, 0.8f, 1f); // Make sure it's visible
        buttonImage.raycastTarget = true; // Ensure it can receive clicks
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.interactable = true; // Ensure button is interactable
        
        // Test button functionality first
        button.onClick.AddListener(() => {
            Debug.Log($"BUTTON TEST: '{text}' button clicked successfully!");
        });
        
        // Ensure onClick is properly assigned
        if (onClick != null)
        {
            button.onClick.AddListener(() => {
                Debug.Log($"Button '{text}' action executing...");
                try 
                {
                    onClick.Invoke();
                    Debug.Log($"Button '{text}' action completed successfully!");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error executing button action for '{text}': {e.Message}");
                    Debug.LogError($"Stack trace: {e.StackTrace}");
                }
            });
        }
        else
        {
            Debug.LogWarning($"No onClick action assigned to button '{text}'");
        }
        
        // Add modern button component (but don't let it interfere with basic functionality)
        if (Application.isPlaying)
        {
            ModernButton modernButton = buttonObj.AddComponent<ModernButton>();
            modernButton.buttonStyle = style;
        }
        
        // Button text
        GameObject textObj = CreateUIElement("Text", buttonObj.transform);
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 28;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        return buttonObj;
    }
    
    private void CreateElementsShowcase()
    {
        // Elements preview panel
        GameObject showcasePanel = CreateUIElement("ElementsShowcase", targetCanvas.transform);
        RectTransform showcaseRect = showcasePanel.GetComponent<RectTransform>();
        showcaseRect.anchorMin = new Vector2(0.02f, 0.02f);
        showcaseRect.anchorMax = new Vector2(0.25f, 0.98f);
        showcaseRect.sizeDelta = Vector2.zero;
        showcaseRect.anchoredPosition = Vector2.zero;
        
        ModernPanel showcasePanelComp = showcasePanel.AddComponent<ModernPanel>();
        showcasePanelComp.panelStyle = ModernPanel.PanelStyle.Card;
        showcasePanelComp.enableAnimatedEntrance = true;
        
        // Showcase title
        GameObject showcaseTitleObj = CreateUIElement("ShowcaseTitle", showcasePanel.transform);
        Text showcaseTitle = showcaseTitleObj.AddComponent<Text>();
        showcaseTitle.text = "Mixology";
        showcaseTitle.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        showcaseTitle.fontSize = 28;
        showcaseTitle.fontStyle = FontStyle.Bold;
        showcaseTitle.color = uiTheme.colorPalette.textAccent;
        showcaseTitle.alignment = TextAnchor.MiddleCenter;
        
        RectTransform showcaseTitleRect = showcaseTitleObj.GetComponent<RectTransform>();
        showcaseTitleRect.anchorMin = new Vector2(0.05f, 0.9f);
        showcaseTitleRect.anchorMax = new Vector2(0.95f, 0.98f);
        showcaseTitleRect.sizeDelta = Vector2.zero;
        showcaseTitleRect.anchoredPosition = Vector2.zero;
        
        ModernText showcaseTitleModern = showcaseTitleObj.AddComponent<ModernText>();
        showcaseTitleModern.textStyle = ModernText.TextStyle.Header;
        
        // Elements grid
        GameObject elementsGrid = CreateUIElement("ElementsGrid", showcasePanel.transform);
        RectTransform gridRect = elementsGrid.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.05f, 0.05f);
        gridRect.anchorMax = new Vector2(0.95f, 0.85f);
        gridRect.sizeDelta = Vector2.zero;
        gridRect.anchoredPosition = Vector2.zero;
        
        GridLayoutGroup gridLayout = elementsGrid.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(80, 80);
        gridLayout.spacing = new Vector2(10, 10);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 2;
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        
        // Create sample element cards
        string[] elements = { "H", "He", "Li", "Be", "B", "C", "N", "O", "F", "Ne", "Na", "Mg" };
        Color[] elementColors = {
            Color.white, Color.cyan, Color.red, Color.green,
            Color.yellow, Color.black, Color.blue, Color.red,
            Color.yellow, Color.cyan, Color.magenta, Color.green
        };
        
        for (int i = 0; i < elements.Length && i < 8; i++)
        {
            CreateElementShowcaseCard(elements[i], elementColors[i % elementColors.Length], elementsGrid.transform, i * 0.1f);
        }
    }
    
    private void CreateElementShowcaseCard(string symbol, Color elementColor, Transform parent, float animationDelay)
    {
        GameObject cardObj = CreateUIElement($"Element_{symbol}", parent);
        
        Image cardImage = cardObj.AddComponent<Image>();
        cardImage.sprite = uiTheme.CreateRoundedRectSprite(80, 80, 8, uiTheme.colorPalette.cardColor);
        
        // Element symbol
        GameObject symbolObj = CreateUIElement("Symbol", cardObj.transform);
        Text symbolText = symbolObj.AddComponent<Text>();
        symbolText.text = symbol;
        symbolText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        symbolText.fontSize = 32;
        symbolText.fontStyle = FontStyle.Bold;
        symbolText.color = elementColor;
        symbolText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform symbolRect = symbolObj.GetComponent<RectTransform>();
        symbolRect.anchorMin = Vector2.zero;
        symbolRect.anchorMax = Vector2.one;
        symbolRect.sizeDelta = Vector2.zero;
        symbolRect.anchoredPosition = Vector2.zero;
        
        // Add hover effect
        ModernButton cardButton = cardObj.AddComponent<ModernButton>();
        cardButton.buttonStyle = ModernButton.ButtonStyle.Ghost;
        
        // Animate entrance
        cardObj.transform.localScale = Vector3.zero;
        cardObj.transform.DOScale(Vector3.one, 0.5f)
            .SetDelay(animationDelay)
            .SetEase(Ease.OutBack);
    }
    
    private void CreateFooter()
    {
        GameObject footerObj = CreateUIElement("Footer", targetCanvas.transform);
        RectTransform footerRect = footerObj.GetComponent<RectTransform>();
        footerRect.anchorMin = new Vector2(0f, 0f);
        footerRect.anchorMax = new Vector2(1f, 0.08f);
        footerRect.sizeDelta = Vector2.zero;
        footerRect.anchoredPosition = Vector2.zero;
        
        ModernPanel footerPanel = footerObj.AddComponent<ModernPanel>();
        footerPanel.panelStyle = ModernPanel.PanelStyle.Background;
        
        // Footer text
        GameObject footerTextObj = CreateUIElement("FooterText", footerObj.transform);
        Text footerText = footerTextObj.AddComponent<Text>();
        footerText.text = "• Mixology v1.0 •";
        footerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        footerText.fontSize = 18;
        footerText.color = uiTheme.colorPalette.textSecondary;
        footerText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform footerTextRect = footerTextObj.GetComponent<RectTransform>();
        footerTextRect.anchorMin = Vector2.zero;
        footerTextRect.anchorMax = Vector2.one;
        footerTextRect.sizeDelta = Vector2.zero;
        footerTextRect.anchoredPosition = Vector2.zero;
        
        ModernText footerTextModern = footerTextObj.AddComponent<ModernText>();
        footerTextModern.textStyle = ModernText.TextStyle.Caption;
    }
    
    private GameObject CreateUIElement(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        return obj;
    }
    
    private void ShowRulesPanel()
    {
        // Create rules overlay
        GameObject rulesOverlay = CreateUIElement("RulesOverlay", targetCanvas.transform);
        RectTransform overlayRect = rulesOverlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        overlayRect.anchoredPosition = Vector2.zero;
        
        ModernPanel overlayPanel = rulesOverlay.AddComponent<ModernPanel>();
        overlayPanel.panelStyle = ModernPanel.PanelStyle.Overlay;
        
        // Rules content panel
        GameObject rulesPanel = CreateUIElement("RulesPanel", rulesOverlay.transform);
        RectTransform rulesPanelRect = rulesPanel.GetComponent<RectTransform>();
        rulesPanelRect.anchorMin = new Vector2(0.15f, 0.15f);
        rulesPanelRect.anchorMax = new Vector2(0.85f, 0.85f);
        rulesPanelRect.sizeDelta = Vector2.zero;
        rulesPanelRect.anchoredPosition = Vector2.zero;
        
        ModernPanel rulesPanelComp = rulesPanel.AddComponent<ModernPanel>();
        rulesPanelComp.panelStyle = ModernPanel.PanelStyle.Surface;
        rulesPanelComp.addGlowEffect = true;
        
        // Rules title
        GameObject rulesTitleObj = CreateUIElement("RulesTitle", rulesPanel.transform);
        Text rulesTitle = rulesTitleObj.AddComponent<Text>();
        rulesTitle.text = "HOW TO PLAY";
        rulesTitle.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        rulesTitle.fontSize = 42;
        rulesTitle.fontStyle = FontStyle.Bold;
        rulesTitle.color = uiTheme.colorPalette.textPrimary;
        rulesTitle.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rulesTitleRect = rulesTitleObj.GetComponent<RectTransform>();
        rulesTitleRect.anchorMin = new Vector2(0.1f, 0.85f);
        rulesTitleRect.anchorMax = new Vector2(0.9f, 0.95f);
        rulesTitleRect.sizeDelta = Vector2.zero;
        rulesTitleRect.anchoredPosition = Vector2.zero;
        
        // Rules content
        GameObject rulesContentObj = CreateUIElement("RulesContent", rulesPanel.transform);
        Text rulesContent = rulesContentObj.AddComponent<Text>();
        rulesContent.text = "🧪 MIXOLOGY RULES\n\n" +
                           "1. SELECT ELEMENTS from your hand by clicking on them\n\n" +
                           "2. CREATE COMPOUNDS by combining compatible elements\n\n" +
                           "3. DISCARD unwanted cards to draw new ones (ends your turn)\n\n" +
                           "4. FIRST PLAYER to collect 8 unique compounds WINS!\n\n" +
                           "💡 TIP: Pay attention to oxidation numbers for valid compounds!";
        rulesContent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        rulesContent.fontSize = 24;
        rulesContent.color = uiTheme.colorPalette.textPrimary;
        rulesContent.alignment = TextAnchor.UpperLeft;
        
        RectTransform rulesContentRect = rulesContentObj.GetComponent<RectTransform>();
        rulesContentRect.anchorMin = new Vector2(0.1f, 0.2f);
        rulesContentRect.anchorMax = new Vector2(0.9f, 0.8f);
        rulesContentRect.sizeDelta = Vector2.zero;
        rulesContentRect.anchoredPosition = Vector2.zero;
        
        // Close button
        GameObject closeButtonObj = CreateMenuButton("Got It!", rulesPanel.transform, () => {
            #if UNITY_EDITOR
            DestroyImmediate(rulesOverlay);
            #else
            Destroy(rulesOverlay);
            #endif
        }, ModernButton.ButtonStyle.Primary);
        
        RectTransform closeButtonRect = closeButtonObj.GetComponent<RectTransform>();
        closeButtonRect.anchorMin = new Vector2(0.35f, 0.05f);
        closeButtonRect.anchorMax = new Vector2(0.65f, 0.15f);
        closeButtonRect.sizeDelta = Vector2.zero;
        closeButtonRect.anchoredPosition = Vector2.zero;
        
        // Animate rules panel
        rulesPanel.transform.localScale = Vector3.zero;
        rulesPanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    
    private void StartSinglePlayerGame()
    {
        Debug.Log("Starting Single Player Game...");
        
        // Try to find existing lobby manager first
        ChemistryLobbyManager lobbyManager = FindObjectOfType<ChemistryLobbyManager>();
        if (lobbyManager != null)
        {
            // Try to trigger the start game button if it exists
            Button startButton = lobbyManager.GetComponentInChildren<Button>();
            if (startButton != null && startButton.name.ToLower().Contains("start"))
            {
                startButton.onClick.Invoke();
                return;
            }
        }
        
        // Direct scene transition as fallback
        LoadGameScene();
    }
    
    private void LoadGameScene()
    {
        try
        {
            // Validate chemistry database first (similar to ChemistryLobbyManager logic)
            ChemistryDatabase database = FindObjectOfType<ChemistryDatabase>();
            if (database == null)
            {
                // Try to load from resources or scriptable objects
                database = Resources.Load<ChemistryDatabase>("ChemistryDatabase");
            }
            
            if (database == null || database.allElements.Count == 0)
            {
                Debug.LogWarning("Chemistry database not set up properly! Loading scene anyway...");
            }
            
            // Load the game board scene
            SceneManager.LoadScene("CardGame-Board");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load CardGame-Board scene: " + e.Message);
            // Try alternative scene name
            try
            {
                SceneManager.LoadScene("Board");
            }
            catch
            {
                Debug.LogError("Could not find game board scene. Please check scene names in Build Settings.");
                
                // Show error message to user
                ShowErrorMessage("Game scene not found! Please check Build Settings.");
            }
        }
    }
    
    private void StartMultiplayerGame()
    {
        Debug.Log("Starting Multiplayer Game...");
        
        // Try to find network lobby manager
        NetCardLobby netLobby = FindObjectOfType<NetCardLobby>();
        if (netLobby != null)
        {
            // Connect to multiplayer if not already connected
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                // Already connected, join or create room
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.JoinRandomRoom();
                }
                else
                {
                    PhotonNetwork.JoinLobby();
                }
            }
        }
        else
        {
            Debug.LogWarning("Network lobby not found. Multiplayer functionality requires NetCardLobby component.");
            StartSinglePlayerGame(); // Fallback to single player
        }
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
    
    private void ShowErrorMessage(string message)
    {
        Debug.LogError(message);
        
        // Create a simple error popup
        GameObject errorOverlay = CreateUIElement("ErrorOverlay", targetCanvas.transform);
        RectTransform overlayRect = errorOverlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        overlayRect.anchoredPosition = Vector2.zero;
        
        Image overlayImage = errorOverlay.AddComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0.8f);
        
        // Error message panel
        GameObject errorPanel = CreateUIElement("ErrorPanel", errorOverlay.transform);
        RectTransform errorRect = errorPanel.GetComponent<RectTransform>();
        errorRect.anchorMin = new Vector2(0.3f, 0.4f);
        errorRect.anchorMax = new Vector2(0.7f, 0.6f);
        errorRect.sizeDelta = Vector2.zero;
        errorRect.anchoredPosition = Vector2.zero;
        
        Image panelImage = errorPanel.AddComponent<Image>();
        panelImage.color = uiTheme.colorPalette.errorColor;
        
        // Error text
        GameObject errorTextObj = CreateUIElement("ErrorText", errorPanel.transform);
        Text errorText = errorTextObj.AddComponent<Text>();
        errorText.text = message;
        errorText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        errorText.fontSize = 24;
        errorText.color = Color.white;
        errorText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform errorTextRect = errorTextObj.GetComponent<RectTransform>();
        errorTextRect.anchorMin = new Vector2(0.1f, 0.4f);
        errorTextRect.anchorMax = new Vector2(0.9f, 0.8f);
        errorTextRect.sizeDelta = Vector2.zero;
        errorTextRect.anchoredPosition = Vector2.zero;
        
        // OK button
        GameObject okButton = CreateMenuButton("OK", errorPanel.transform, () => {
            #if UNITY_EDITOR
            DestroyImmediate(errorOverlay);
            #else
            Destroy(errorOverlay);
            #endif
        }, ModernButton.ButtonStyle.Primary);
        
        RectTransform okRect = okButton.GetComponent<RectTransform>();
        okRect.anchorMin = new Vector2(0.3f, 0.1f);
        okRect.anchorMax = new Vector2(0.7f, 0.3f);
        okRect.sizeDelta = Vector2.zero;
        okRect.anchoredPosition = Vector2.zero;
        
        // Animate popup
        errorPanel.transform.localScale = Vector3.zero;
        errorPanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
}
#endif