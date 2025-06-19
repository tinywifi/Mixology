using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

public class EnhancedGameBoardUI : MonoBehaviour
{
    [Header("Target Canvas")]
    public Canvas targetCanvas;
    
    [Header("Theme Settings")]
    public ModernUITheme uiTheme;
    
    [ContextMenu("Create Enhanced Game Board UI")]
    public void CreateEnhancedGameBoardUI()
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
        
        // Create game board layout
        CreateGameBackground();
        CreateHUD();
        CreatePlayerArea();
        CreateOpponentArea();
        CreateCenterGameArea();
        CreateActionBar();
        CreateGameStatusPanel();
        
        Debug.Log("✓ Enhanced Game Board UI Created Successfully!");
        EditorUtility.SetDirty(targetCanvas.gameObject);
    }
    
    private void ClearExistingUI()
    {
        for (int i = targetCanvas.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(targetCanvas.transform.GetChild(i).gameObject);
        }
    }
    
    private void SetupCanvas()
    {
        CanvasScaler scaler = targetCanvas.GetComponent<CanvasScaler>();
        if (scaler == null)
            scaler = targetCanvas.gameObject.AddComponent<CanvasScaler>();
            
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        GraphicRaycaster raycaster = targetCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
            raycaster = targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
    }
    
    private void CreateGameBackground()
    {
        GameObject backgroundObj = CreateUIElement("GameBackground", targetCanvas.transform);
        RectTransform bgRect = backgroundObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        // Animated chemistry background
        Image bgImage = backgroundObj.AddComponent<Image>();
        Texture2D gradientTexture = uiTheme.CreateGradientTexture(
            new Color(0.05f, 0.1f, 0.2f, 1f),
            new Color(0.1f, 0.2f, 0.3f, 1f),
            512, 512
        );
        bgImage.sprite = Sprite.Create(gradientTexture, new Rect(0, 0, 512, 512), Vector2.one * 0.5f);
        
        // Add subtle pattern overlay
        CreateBackgroundPattern(backgroundObj.transform);
    }
    
    private void CreateBackgroundPattern(Transform parent)
    {
        GameObject patternObj = CreateUIElement("BackgroundPattern", parent);
        RectTransform patternRect = patternObj.GetComponent<RectTransform>();
        patternRect.anchorMin = Vector2.zero;
        patternRect.anchorMax = Vector2.one;
        patternRect.sizeDelta = Vector2.zero;
        patternRect.anchoredPosition = Vector2.zero;
        
        // Create molecular structure pattern
        for (int i = 0; i < 20; i++)
        {
            CreateMolecule(patternObj.transform, i);
        }
    }
    
    private void CreateMolecule(Transform parent, int index)
    {
        GameObject moleculeObj = CreateUIElement($"Molecule_{index}", parent);
        RectTransform moleculeRect = moleculeObj.GetComponent<RectTransform>();
        moleculeRect.sizeDelta = new Vector2(100, 100);
        moleculeRect.anchoredPosition = new Vector2(
            Random.Range(-960f, 960f),
            Random.Range(-540f, 540f)
        );
        
        // Central atom
        GameObject centralAtom = CreateUIElement("CentralAtom", moleculeObj.transform);
        Image centralImage = centralAtom.AddComponent<Image>();
        centralImage.sprite = uiTheme.CreateRoundedRectSprite(20, 20, 10, 
            new Color(uiTheme.colorPalette.primaryColor.r, uiTheme.colorPalette.primaryColor.g, uiTheme.colorPalette.primaryColor.b, 0.2f));
        
        RectTransform centralRect = centralAtom.GetComponent<RectTransform>();
        centralRect.sizeDelta = new Vector2(20, 20);
        centralRect.anchoredPosition = Vector2.zero;
        
        // Surrounding atoms
        for (int j = 0; j < Random.Range(2, 5); j++)
        {
            GameObject atomObj = CreateUIElement($"Atom_{j}", moleculeObj.transform);
            Image atomImage = atomObj.AddComponent<Image>();
            atomImage.sprite = uiTheme.CreateRoundedRectSprite(12, 12, 6,
                new Color(uiTheme.colorPalette.secondaryColor.r, uiTheme.colorPalette.secondaryColor.g, uiTheme.colorPalette.secondaryColor.b, 0.15f));
            
            RectTransform atomRect = atomObj.GetComponent<RectTransform>();
            atomRect.sizeDelta = new Vector2(12, 12);
            
            float angle = (j * 360f / 4f) * Mathf.Deg2Rad;
            atomRect.anchoredPosition = new Vector2(
                Mathf.Cos(angle) * 30f,
                Mathf.Sin(angle) * 30f
            );
            
            // Create bond line
            GameObject bondObj = CreateUIElement($"Bond_{j}", moleculeObj.transform);
            Image bondImage = bondObj.AddComponent<Image>();
            bondImage.color = new Color(uiTheme.colorPalette.textSecondary.r, uiTheme.colorPalette.textSecondary.g, uiTheme.colorPalette.textSecondary.b, 0.1f);
            
            RectTransform bondRect = bondObj.GetComponent<RectTransform>();
            bondRect.sizeDelta = new Vector2(30f, 2f);
            bondRect.anchoredPosition = new Vector2(Mathf.Cos(angle) * 15f, Mathf.Sin(angle) * 15f);
            bondRect.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
        
        // Animate rotation
        moleculeObj.transform.DORotate(new Vector3(0, 0, 360f), Random.Range(20f, 40f), RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
    
    private void CreateHUD()
    {
        // Top HUD panel
        GameObject hudPanel = CreateUIElement("HUDPanel", targetCanvas.transform);
        RectTransform hudRect = hudPanel.GetComponent<RectTransform>();
        hudRect.anchorMin = new Vector2(0f, 0.9f);
        hudRect.anchorMax = new Vector2(1f, 1f);
        hudRect.sizeDelta = Vector2.zero;
        hudRect.anchoredPosition = Vector2.zero;
        
        ModernPanel hudPanelComp = hudPanel.AddComponent<ModernPanel>();
        hudPanelComp.panelStyle = ModernPanel.PanelStyle.Surface;
        hudPanelComp.enableAnimatedEntrance = true;
        
        // Game title
        GameObject titleObj = CreateUIElement("GameTitle", hudPanel.transform);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "CHEMISTRY PARTY";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 32;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = uiTheme.colorPalette.textAccent;
        titleText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.4f, 0.1f);
        titleRect.anchorMax = new Vector2(0.6f, 0.9f);
        titleRect.sizeDelta = Vector2.zero;
        titleRect.anchoredPosition = Vector2.zero;
        
        ModernText titleModern = titleObj.AddComponent<ModernText>();
        titleModern.textStyle = ModernText.TextStyle.Header;
        
        // Turn indicator
        GameObject turnIndicatorObj = CreateUIElement("TurnIndicator", hudPanel.transform);
        Text turnText = turnIndicatorObj.AddComponent<Text>();
        turnText.text = "YOUR TURN";
        turnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        turnText.fontSize = 24;
        turnText.fontStyle = FontStyle.Bold;
        turnText.color = uiTheme.colorPalette.successColor;
        turnText.alignment = TextAnchor.MiddleLeft;
        
        RectTransform turnRect = turnIndicatorObj.GetComponent<RectTransform>();
        turnRect.anchorMin = new Vector2(0.05f, 0.1f);
        turnRect.anchorMax = new Vector2(0.35f, 0.9f);
        turnRect.sizeDelta = Vector2.zero;
        turnRect.anchoredPosition = Vector2.zero;
        
        ModernText turnModern = turnIndicatorObj.AddComponent<ModernText>();
        turnModern.textStyle = ModernText.TextStyle.Accent;
        
        // Score display
        GameObject scoreObj = CreateUIElement("ScoreDisplay", hudPanel.transform);
        Text scoreText = scoreObj.AddComponent<Text>();
        scoreText.text = "COMPOUNDS: 0/8";
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 20;
        scoreText.color = uiTheme.colorPalette.textPrimary;
        scoreText.alignment = TextAnchor.MiddleRight;
        
        RectTransform scoreRect = scoreObj.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0.65f, 0.1f);
        scoreRect.anchorMax = new Vector2(0.95f, 0.9f);
        scoreRect.sizeDelta = Vector2.zero;
        scoreRect.anchoredPosition = Vector2.zero;
        
        ModernText scoreModern = scoreObj.AddComponent<ModernText>();
        scoreModern.textStyle = ModernText.TextStyle.Body;
    }
    
    private void CreatePlayerArea()
    {
        // Player hand area
        GameObject playerArea = CreateUIElement("PlayerArea", targetCanvas.transform);
        RectTransform playerRect = playerArea.GetComponent<RectTransform>();
        playerRect.anchorMin = new Vector2(0.05f, 0.02f);
        playerRect.anchorMax = new Vector2(0.95f, 0.35f);
        playerRect.sizeDelta = Vector2.zero;
        playerRect.anchoredPosition = Vector2.zero;
        
        ModernPanel playerPanel = playerArea.AddComponent<ModernPanel>();
        playerPanel.panelStyle = ModernPanel.PanelStyle.Card;
        playerPanel.enableAnimatedEntrance = true;
        playerPanel.addGlowEffect = true;
        
        // Player label
        GameObject playerLabelObj = CreateUIElement("PlayerLabel", playerArea.transform);
        Text playerLabel = playerLabelObj.AddComponent<Text>();
        playerLabel.text = "YOUR ELEMENTS";
        playerLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        playerLabel.fontSize = 24;
        playerLabel.fontStyle = FontStyle.Bold;
        playerLabel.color = uiTheme.colorPalette.textAccent;
        playerLabel.alignment = TextAnchor.MiddleLeft;
        
        RectTransform playerLabelRect = playerLabelObj.GetComponent<RectTransform>();
        playerLabelRect.anchorMin = new Vector2(0.02f, 0.85f);
        playerLabelRect.anchorMax = new Vector2(0.5f, 0.98f);
        playerLabelRect.sizeDelta = Vector2.zero;
        playerLabelRect.anchoredPosition = Vector2.zero;
        
        ModernText playerLabelModern = playerLabelObj.AddComponent<ModernText>();
        playerLabelModern.textStyle = ModernText.TextStyle.Header;
        
        // Player hand scroll area
        GameObject handScrollArea = CreateScrollArea("PlayerHandScrollArea", playerArea.transform, true);
        RectTransform handScrollRect = handScrollArea.GetComponent<RectTransform>();
        handScrollRect.anchorMin = new Vector2(0.02f, 0.15f);
        handScrollRect.anchorMax = new Vector2(0.98f, 0.8f);
        handScrollRect.sizeDelta = Vector2.zero;
        handScrollRect.anchoredPosition = Vector2.zero;
        
        // Player compounds area
        GameObject compoundsArea = CreateUIElement("PlayerCompoundsArea", playerArea.transform);
        RectTransform compoundsRect = compoundsArea.GetComponent<RectTransform>();
        compoundsRect.anchorMin = new Vector2(0.02f, 0.02f);
        compoundsRect.anchorMax = new Vector2(0.98f, 0.12f);
        compoundsRect.sizeDelta = Vector2.zero;
        compoundsRect.anchoredPosition = Vector2.zero;
        
        GameObject compoundsScrollArea = CreateScrollArea("PlayerCompoundsScrollArea", compoundsArea.transform, true);
        RectTransform compoundsScrollRect = compoundsScrollArea.GetComponent<RectTransform>();
        compoundsScrollRect.anchorMin = Vector2.zero;
        compoundsScrollRect.anchorMax = Vector2.one;
        compoundsScrollRect.sizeDelta = Vector2.zero;
        compoundsScrollRect.anchoredPosition = Vector2.zero;
    }
    
    private void CreateOpponentArea()
    {
        // Opponent area
        GameObject opponentArea = CreateUIElement("OpponentArea", targetCanvas.transform);
        RectTransform opponentRect = opponentArea.GetComponent<RectTransform>();
        opponentRect.anchorMin = new Vector2(0.05f, 0.75f);
        opponentRect.anchorMax = new Vector2(0.95f, 0.88f);
        opponentRect.sizeDelta = Vector2.zero;
        opponentRect.anchoredPosition = Vector2.zero;
        
        ModernPanel opponentPanel = opponentArea.AddComponent<ModernPanel>();
        opponentPanel.panelStyle = ModernPanel.PanelStyle.Card;
        opponentPanel.enableAnimatedEntrance = true;
        
        // Opponent label
        GameObject opponentLabelObj = CreateUIElement("OpponentLabel", opponentArea.transform);
        Text opponentLabel = opponentLabelObj.AddComponent<Text>();
        opponentLabel.text = "OPPONENT'S COMPOUNDS";
        opponentLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        opponentLabel.fontSize = 20;
        opponentLabel.fontStyle = FontStyle.Bold;
        opponentLabel.color = uiTheme.colorPalette.errorColor;
        opponentLabel.alignment = TextAnchor.MiddleCenter;
        
        RectTransform opponentLabelRect = opponentLabelObj.GetComponent<RectTransform>();
        opponentLabelRect.anchorMin = new Vector2(0.02f, 0.7f);
        opponentLabelRect.anchorMax = new Vector2(0.98f, 0.95f);
        opponentLabelRect.sizeDelta = Vector2.zero;
        opponentLabelRect.anchoredPosition = Vector2.zero;
        
        ModernText opponentLabelModern = opponentLabelObj.AddComponent<ModernText>();
        opponentLabelModern.textStyle = ModernText.TextStyle.Body;
        
        // Opponent compounds scroll area
        GameObject opponentScrollArea = CreateScrollArea("OpponentCompoundsArea", opponentArea.transform, true);
        RectTransform opponentScrollRect = opponentScrollArea.GetComponent<RectTransform>();
        opponentScrollRect.anchorMin = new Vector2(0.02f, 0.05f);
        opponentScrollRect.anchorMax = new Vector2(0.98f, 0.65f);
        opponentScrollRect.sizeDelta = Vector2.zero;
        opponentScrollRect.anchoredPosition = Vector2.zero;
    }
    
    private void CreateCenterGameArea()
    {
        // Center area for game status and effects
        GameObject centerArea = CreateUIElement("CenterGameArea", targetCanvas.transform);
        RectTransform centerRect = centerArea.GetComponent<RectTransform>();
        centerRect.anchorMin = new Vector2(0.25f, 0.4f);
        centerRect.anchorMax = new Vector2(0.75f, 0.7f);
        centerRect.sizeDelta = Vector2.zero;
        centerRect.anchoredPosition = Vector2.zero;
        
        // This area will be used for reaction animations and status messages
        GameObject statusMessageObj = CreateUIElement("StatusMessage", centerArea.transform);
        Text statusMessage = statusMessageObj.AddComponent<Text>();
        statusMessage.text = "Select elements to create compounds!";
        statusMessage.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusMessage.fontSize = 32;
        statusMessage.fontStyle = FontStyle.Bold;
        statusMessage.color = uiTheme.colorPalette.textAccent;
        statusMessage.alignment = TextAnchor.MiddleCenter;
        
        RectTransform statusRect = statusMessageObj.GetComponent<RectTransform>();
        statusRect.anchorMin = Vector2.zero;
        statusRect.anchorMax = Vector2.one;
        statusRect.sizeDelta = Vector2.zero;
        statusRect.anchoredPosition = Vector2.zero;
        
        ModernText statusModern = statusMessageObj.AddComponent<ModernText>();
        statusModern.textStyle = ModernText.TextStyle.Header;
        statusModern.enableTypewriterEffect = true;
    }
    
    private void CreateActionBar()
    {
        // Action buttons bar
        GameObject actionBar = CreateUIElement("ActionBar", targetCanvas.transform);
        RectTransform actionRect = actionBar.GetComponent<RectTransform>();
        actionRect.anchorMin = new Vector2(0.1f, 0.36f);
        actionRect.anchorMax = new Vector2(0.9f, 0.46f);
        actionRect.sizeDelta = Vector2.zero;
        actionRect.anchoredPosition = Vector2.zero;
        
        ModernPanel actionPanel = actionBar.AddComponent<ModernPanel>();
        actionPanel.panelStyle = ModernPanel.PanelStyle.Surface;
        actionPanel.enableAnimatedEntrance = true;
        actionPanel.addGlowEffect = true;
        
        // Buttons container
        GameObject buttonsContainer = CreateUIElement("ButtonsContainer", actionBar.transform);
        RectTransform buttonsRect = buttonsContainer.GetComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(0.05f, 0.1f);
        buttonsRect.anchorMax = new Vector2(0.95f, 0.9f);
        buttonsRect.sizeDelta = Vector2.zero;
        buttonsRect.anchoredPosition = Vector2.zero;
        
        HorizontalLayoutGroup buttonsLayout = buttonsContainer.AddComponent<HorizontalLayoutGroup>();
        buttonsLayout.spacing = 20f;
        buttonsLayout.childControlHeight = true;
        buttonsLayout.childControlWidth = true;
        buttonsLayout.childForceExpandHeight = true;
        buttonsLayout.childForceExpandWidth = true;
        
        // Create action buttons
        CreateActionButton("Create Compound", buttonsContainer.transform, ModernButton.ButtonStyle.Primary);
        CreateActionButton("Discard", buttonsContainer.transform, ModernButton.ButtonStyle.Warning);
        CreateActionButton("End Turn", buttonsContainer.transform, ModernButton.ButtonStyle.Secondary);
        CreateActionButton("Menu", buttonsContainer.transform, ModernButton.ButtonStyle.Ghost);
    }
    
    private GameObject CreateActionButton(string text, Transform parent, ModernButton.ButtonStyle style)
    {
        GameObject buttonObj = CreateUIElement($"Button_{text.Replace(" ", "")}", parent);
        
        Button button = buttonObj.AddComponent<Button>();
        ModernButton modernButton = buttonObj.AddComponent<ModernButton>();
        modernButton.buttonStyle = style;
        modernButton.enableHoverEffects = true;
        modernButton.enableClickAnimation = true;
        
        // Button text
        GameObject textObj = CreateUIElement("Text", buttonObj.transform);
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 24;
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
    
    private void CreateGameStatusPanel()
    {
        // Side status panel
        GameObject statusPanel = CreateUIElement("GameStatusPanel", targetCanvas.transform);
        RectTransform statusRect = statusPanel.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.02f, 0.4f);
        statusRect.anchorMax = new Vector2(0.23f, 0.85f);
        statusRect.sizeDelta = Vector2.zero;
        statusRect.anchoredPosition = Vector2.zero;
        
        ModernPanel statusPanelComp = statusPanel.AddComponent<ModernPanel>();
        statusPanelComp.panelStyle = ModernPanel.PanelStyle.Card;
        statusPanelComp.enableAnimatedEntrance = true;
        
        // Status title
        GameObject statusTitleObj = CreateUIElement("StatusTitle", statusPanel.transform);
        Text statusTitle = statusTitleObj.AddComponent<Text>();
        statusTitle.text = "GAME STATUS";
        statusTitle.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusTitle.fontSize = 22;
        statusTitle.fontStyle = FontStyle.Bold;
        statusTitle.color = uiTheme.colorPalette.textAccent;
        statusTitle.alignment = TextAnchor.MiddleCenter;
        
        RectTransform statusTitleRect = statusTitleObj.GetComponent<RectTransform>();
        statusTitleRect.anchorMin = new Vector2(0.05f, 0.9f);
        statusTitleRect.anchorMax = new Vector2(0.95f, 0.98f);
        statusTitleRect.sizeDelta = Vector2.zero;
        statusTitleRect.anchoredPosition = Vector2.zero;
        
        ModernText statusTitleModern = statusTitleObj.AddComponent<ModernText>();
        statusTitleModern.textStyle = ModernText.TextStyle.Header;
        
        // Elements count
        GameObject elementsCountObj = CreateUIElement("ElementsCount", statusPanel.transform);
        Text elementsCount = elementsCountObj.AddComponent<Text>();
        elementsCount.text = "Elements: 8/10";
        elementsCount.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        elementsCount.fontSize = 18;
        elementsCount.color = uiTheme.colorPalette.textPrimary;
        elementsCount.alignment = TextAnchor.MiddleLeft;
        
        RectTransform elementsCountRect = elementsCountObj.GetComponent<RectTransform>();
        elementsCountRect.anchorMin = new Vector2(0.1f, 0.75f);
        elementsCountRect.anchorMax = new Vector2(0.9f, 0.85f);
        elementsCountRect.sizeDelta = Vector2.zero;
        elementsCountRect.anchoredPosition = Vector2.zero;
        
        ModernText elementsCountModern = elementsCountObj.AddComponent<ModernText>();
        elementsCountModern.textStyle = ModernText.TextStyle.Body;
        
        // Compounds count
        GameObject compoundsCountObj = CreateUIElement("CompoundsCount", statusPanel.transform);
        Text compoundsCount = compoundsCountObj.AddComponent<Text>();
        compoundsCount.text = "Compounds: 3/8";
        compoundsCount.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        compoundsCount.fontSize = 18;
        compoundsCount.color = uiTheme.colorPalette.textPrimary;
        compoundsCount.alignment = TextAnchor.MiddleLeft;
        
        RectTransform compoundsCountRect = compoundsCountObj.GetComponent<RectTransform>();
        compoundsCountRect.anchorMin = new Vector2(0.1f, 0.6f);
        compoundsCountRect.anchorMax = new Vector2(0.9f, 0.7f);
        compoundsCountRect.sizeDelta = Vector2.zero;
        compoundsCountRect.anchoredPosition = Vector2.zero;
        
        ModernText compoundsCountModern = compoundsCountObj.AddComponent<ModernText>();
        compoundsCountModern.textStyle = ModernText.TextStyle.Body;
        
        // Opponent info
        GameObject opponentInfoObj = CreateUIElement("OpponentInfo", statusPanel.transform);
        Text opponentInfo = opponentInfoObj.AddComponent<Text>();
        opponentInfo.text = "Opponent:\n2 Compounds";
        opponentInfo.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        opponentInfo.fontSize = 16;
        opponentInfo.color = uiTheme.colorPalette.errorColor;
        opponentInfo.alignment = TextAnchor.MiddleLeft;
        
        RectTransform opponentInfoRect = opponentInfoObj.GetComponent<RectTransform>();
        opponentInfoRect.anchorMin = new Vector2(0.1f, 0.4f);
        opponentInfoRect.anchorMax = new Vector2(0.9f, 0.55f);
        opponentInfoRect.sizeDelta = Vector2.zero;
        opponentInfoRect.anchoredPosition = Vector2.zero;
        
        ModernText opponentInfoModern = opponentInfoObj.AddComponent<ModernText>();
        opponentInfoModern.textStyle = ModernText.TextStyle.Caption;
    }
    
    private GameObject CreateScrollArea(string name, Transform parent, bool horizontal)
    {
        GameObject scrollAreaObj = CreateUIElement(name, parent);
        
        // Scroll view background
        Image scrollBg = scrollAreaObj.AddComponent<Image>();
        scrollBg.color = new Color(uiTheme.colorPalette.backgroundColor.r, uiTheme.colorPalette.backgroundColor.g, uiTheme.colorPalette.backgroundColor.b, 0.5f);
        scrollBg.sprite = uiTheme.CreateRoundedRectSprite(200, 100, 8, scrollBg.color);
        
        // Content area
        GameObject contentObj = CreateUIElement("Content", scrollAreaObj.transform);
        RectTransform contentRect = contentObj.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.sizeDelta = Vector2.zero;
        contentRect.anchoredPosition = Vector2.zero;
        
        // Layout group
        if (horizontal)
        {
            HorizontalLayoutGroup layoutGroup = contentObj.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.spacing = 15f;
            layoutGroup.padding = new RectOffset(15, 15, 10, 10);
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            
            ContentSizeFitter sizeFitter = contentObj.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        else
        {
            VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 10f;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
        }
        
        // Scroll rect
        ScrollRect scrollRect = scrollAreaObj.AddComponent<ScrollRect>();
        scrollRect.content = contentRect;
        scrollRect.horizontal = horizontal;
        scrollRect.vertical = !horizontal;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 20f;
        
        return scrollAreaObj;
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
}
#endif