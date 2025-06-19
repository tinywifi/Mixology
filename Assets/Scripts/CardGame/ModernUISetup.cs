using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

public class ModernUISetup : MonoBehaviour
{
    [Header("Target Canvases")]
    public Canvas lobbyCanvas;
    public Canvas gameCanvas;
    
    [Header("Chemistry Database")]
    public ChemistryDatabase chemistryDatabase;
    
    [Header("Card Prefabs")]
    public GameObject elementCardPrefab;
    public GameObject compoundCardPrefab;
    
    [ContextMenu("1. Setup Modern UI Theme")]
    public void SetupModernUITheme()
    {
        Debug.Log("=== Setting up Modern UI Theme ===");
        
        // Create or find the theme component
        ModernUITheme theme = FindObjectOfType<ModernUITheme>();
        if (theme == null)
        {
            GameObject themeObj = new GameObject("ModernUITheme");
            theme = themeObj.AddComponent<ModernUITheme>();
            
            // Only call DontDestroyOnLoad in play mode
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(themeObj);
            }
        }
        
        Debug.Log("✓ Modern UI Theme created and configured");
    }
    
    [ContextMenu("2. Create Enhanced Lobby UI")]
    public void CreateEnhancedLobbyUI()
    {
        if (lobbyCanvas == null)
        {
            Debug.LogError("Lobby Canvas not assigned!");
            return;
        }
        
        Debug.Log("=== Creating Enhanced Lobby UI ===");
        
        // Get or create the lobby UI setup component
        EnhancedLobbyUI lobbySetup = FindObjectOfType<EnhancedLobbyUI>();
        if (lobbySetup == null)
        {
            GameObject setupObj = new GameObject("EnhancedLobbyUISetup");
            lobbySetup = setupObj.AddComponent<EnhancedLobbyUI>();
        }
        
        // Configure and run
        lobbySetup.targetCanvas = lobbyCanvas;
        lobbySetup.CreateEnhancedLobbyUI();
        
        Debug.Log("✓ Enhanced Lobby UI created successfully");
    }
    
    [ContextMenu("3. Create Enhanced Game Board UI")]
    public void CreateEnhancedGameBoardUI()
    {
        if (gameCanvas == null)
        {
            Debug.LogError("Game Canvas not assigned!");
            return;
        }
        
        Debug.Log("=== Creating Enhanced Game Board UI ===");
        
        // Get or create the game board UI setup component
        EnhancedGameBoardUI gameBoardSetup = FindObjectOfType<EnhancedGameBoardUI>();
        if (gameBoardSetup == null)
        {
            GameObject setupObj = new GameObject("EnhancedGameBoardUISetup");
            gameBoardSetup = setupObj.AddComponent<EnhancedGameBoardUI>();
        }
        
        // Configure and run
        gameBoardSetup.targetCanvas = gameCanvas;
        gameBoardSetup.CreateEnhancedGameBoardUI();
        
        Debug.Log("✓ Enhanced Game Board UI created successfully");
    }
    
    [ContextMenu("4. Setup Enhanced Card Prefabs")]
    public void SetupEnhancedCardPrefabs()
    {
        Debug.Log("=== Setting up Enhanced Card Prefabs ===");
        
        // Create enhanced element card prefab
        CreateEnhancedElementCardPrefab();
        
        // Create enhanced compound card prefab
        CreateEnhancedCompoundCardPrefab();
        
        Debug.Log("✓ Enhanced card prefabs created successfully");
    }
    
    [ContextMenu("5. Complete Modern UI Setup")]
    public void CompleteModernUISetup()
    {
        Debug.Log("=== Starting Complete Modern UI Setup ===");
        
        // Run all setup steps in order
        SetupModernUITheme();
        SetupEnhancedCardPrefabs();
        
        if (lobbyCanvas != null)
            CreateEnhancedLobbyUI();
        else
            Debug.LogWarning("Lobby Canvas not assigned - skipping lobby UI creation");
            
        if (gameCanvas != null)
            CreateEnhancedGameBoardUI();
        else
            Debug.LogWarning("Game Canvas not assigned - skipping game board UI creation");
        
        // Integrate with existing chemistry game setup
        IntegrateWithChemistrySetup();
        
        Debug.Log("=== Complete Modern UI Setup Finished ===");
        Debug.Log("Your Chemistry Party game now has a modern, professional UI!");
        Debug.Log("Next steps:");
        Debug.Log("1. Test the lobby scene");
        Debug.Log("2. Test the game board scene");
        Debug.Log("3. Adjust colors/fonts in ModernUITheme if desired");
    }
    
    private void CreateEnhancedElementCardPrefab()
    {
        // Create the prefab structure
        GameObject cardObj = new GameObject("EnhancedElementCard");
        
        // Add RectTransform
        RectTransform cardRect = cardObj.AddComponent<RectTransform>();
        cardRect.sizeDelta = new Vector2(150, 200);
        
        // Add main card background
        Image cardBg = cardObj.AddComponent<Image>();
        cardBg.raycastTarget = true;
        
        // Add enhanced element card component
        EnhancedElementCard enhancedCard = cardObj.AddComponent<EnhancedElementCard>();
        enhancedCard.cardBackground = cardBg;
        
        // Create element icon
        GameObject iconObj = new GameObject("ElementIcon");
        iconObj.transform.SetParent(cardObj.transform);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.6f);
        iconRect.anchorMax = new Vector2(0.9f, 0.9f);
        iconRect.sizeDelta = Vector2.zero;
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.localScale = Vector3.one;
        
        Image iconImage = iconObj.AddComponent<Image>();
        iconImage.raycastTarget = false;
        enhancedCard.elementIcon = iconImage;
        
        // Create element symbol
        GameObject symbolObj = new GameObject("ElementSymbol");
        symbolObj.transform.SetParent(cardObj.transform);
        RectTransform symbolRect = symbolObj.AddComponent<RectTransform>();
        symbolRect.anchorMin = new Vector2(0.1f, 0.4f);
        symbolRect.anchorMax = new Vector2(0.9f, 0.6f);
        symbolRect.sizeDelta = Vector2.zero;
        symbolRect.anchoredPosition = Vector2.zero;
        symbolRect.localScale = Vector3.one;
        
        Text symbolText = symbolObj.AddComponent<Text>();
        symbolText.text = "H";
        symbolText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        symbolText.fontSize = 48;
        symbolText.fontStyle = FontStyle.Bold;
        symbolText.alignment = TextAnchor.MiddleCenter;
        symbolText.color = Color.white;
        symbolText.raycastTarget = false;
        enhancedCard.elementSymbol = symbolText;
        
        // Create element name
        GameObject nameObj = new GameObject("ElementName");
        nameObj.transform.SetParent(cardObj.transform);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.05f, 0.25f);
        nameRect.anchorMax = new Vector2(0.95f, 0.4f);
        nameRect.sizeDelta = Vector2.zero;
        nameRect.anchoredPosition = Vector2.zero;
        nameRect.localScale = Vector3.one;
        
        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = "Hydrogen";
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 16;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.white;
        nameText.raycastTarget = false;
        enhancedCard.elementName = nameText;
        
        // Create oxidation number
        GameObject oxidationObj = new GameObject("OxidationNumber");
        oxidationObj.transform.SetParent(cardObj.transform);
        RectTransform oxidationRect = oxidationObj.AddComponent<RectTransform>();
        oxidationRect.anchorMin = new Vector2(0.1f, 0.05f);
        oxidationRect.anchorMax = new Vector2(0.9f, 0.25f);
        oxidationRect.sizeDelta = Vector2.zero;
        oxidationRect.anchoredPosition = Vector2.zero;
        oxidationRect.localScale = Vector3.one;
        
        Text oxidationText = oxidationObj.AddComponent<Text>();
        oxidationText.text = "+1";
        oxidationText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        oxidationText.fontSize = 20;
        oxidationText.fontStyle = FontStyle.Bold;
        oxidationText.alignment = TextAnchor.MiddleCenter;
        oxidationText.color = Color.green;
        oxidationText.raycastTarget = false;
        enhancedCard.oxidationNumber = oxidationText;
        
        // Create selection glow
        GameObject glowObj = new GameObject("SelectionGlow");
        glowObj.transform.SetParent(cardObj.transform);
        glowObj.transform.SetAsFirstSibling();
        RectTransform glowRect = glowObj.AddComponent<RectTransform>();
        glowRect.anchorMin = Vector2.zero;
        glowRect.anchorMax = Vector2.one;
        glowRect.sizeDelta = new Vector2(10, 10);
        glowRect.anchoredPosition = Vector2.zero;
        glowRect.localScale = Vector3.one;
        
        Image glowImage = glowObj.AddComponent<Image>();
        glowImage.color = new Color(0.3f, 0.7f, 1f, 0f);
        glowImage.raycastTarget = false;
        enhancedCard.selectionGlow = glowImage;
        
        // Create card border
        GameObject borderObj = new GameObject("CardBorder");
        borderObj.transform.SetParent(cardObj.transform);
        borderObj.transform.SetSiblingIndex(1);
        RectTransform borderRect = borderObj.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = new Vector2(5, 5);
        borderRect.anchoredPosition = Vector2.zero;
        borderRect.localScale = Vector3.one;
        
        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = Color.white;
        borderImage.raycastTarget = false;
        enhancedCard.cardBorder = borderImage;
        
        // Save as prefab
        System.IO.Directory.CreateDirectory("Assets/Prefabs/Chemistry");
        string prefabPath = "Assets/Prefabs/Chemistry/EnhancedElementCard.prefab";
        
        #if UNITY_EDITOR
        PrefabUtility.SaveAsPrefabAsset(cardObj, prefabPath);
        #endif
        
        DestroyImmediate(cardObj);
        Debug.Log("✓ Enhanced Element Card prefab created");
    }
    
    private void CreateEnhancedCompoundCardPrefab()
    {
        // Create the prefab structure
        GameObject cardObj = new GameObject("EnhancedCompoundCard");
        
        // Add RectTransform
        RectTransform cardRect = cardObj.AddComponent<RectTransform>();
        cardRect.sizeDelta = new Vector2(200, 140);
        
        // Add main card background
        Image cardBg = cardObj.AddComponent<Image>();
        cardBg.raycastTarget = true;
        
        // Add enhanced compound card component
        EnhancedCompoundCard enhancedCard = cardObj.AddComponent<EnhancedCompoundCard>();
        enhancedCard.cardBackground = cardBg;
        
        // Create formula text
        GameObject formulaObj = new GameObject("FormulaText");
        formulaObj.transform.SetParent(cardObj.transform);
        RectTransform formulaRect = formulaObj.AddComponent<RectTransform>();
        formulaRect.anchorMin = new Vector2(0.1f, 0.6f);
        formulaRect.anchorMax = new Vector2(0.9f, 0.9f);
        formulaRect.sizeDelta = Vector2.zero;
        formulaRect.anchoredPosition = Vector2.zero;
        formulaRect.localScale = Vector3.one;
        
        Text formulaText = formulaObj.AddComponent<Text>();
        formulaText.text = "H2O";
        formulaText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        formulaText.fontSize = 32;
        formulaText.fontStyle = FontStyle.Bold;
        formulaText.alignment = TextAnchor.MiddleCenter;
        formulaText.color = Color.white;
        formulaText.raycastTarget = false;
        enhancedCard.formulaText = formulaText;
        
        // Create compound name
        GameObject nameObj = new GameObject("CompoundName");
        nameObj.transform.SetParent(cardObj.transform);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.05f, 0.4f);
        nameRect.anchorMax = new Vector2(0.95f, 0.6f);
        nameRect.sizeDelta = Vector2.zero;
        nameRect.anchoredPosition = Vector2.zero;
        nameRect.localScale = Vector3.one;
        
        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = "Water";
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 14;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.white;
        nameText.raycastTarget = false;
        enhancedCard.compoundNameText = nameText;
        
        // Create effect text
        GameObject effectObj = new GameObject("EffectText");
        effectObj.transform.SetParent(cardObj.transform);
        RectTransform effectRect = effectObj.AddComponent<RectTransform>();
        effectRect.anchorMin = new Vector2(0.05f, 0.2f);
        effectRect.anchorMax = new Vector2(0.95f, 0.4f);
        effectRect.sizeDelta = Vector2.zero;
        effectRect.anchoredPosition = Vector2.zero;
        effectRect.localScale = Vector3.one;
        
        Text effectText = effectObj.AddComponent<Text>();
        effectText.text = "No effect";
        effectText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        effectText.fontSize = 12;
        effectText.alignment = TextAnchor.MiddleCenter;
        effectText.color = Color.gray;
        effectText.raycastTarget = false;
        enhancedCard.effectText = effectText;
        
        // Create elements container
        GameObject elementsObj = new GameObject("ElementsContainer");
        elementsObj.transform.SetParent(cardObj.transform);
        RectTransform elementsRect = elementsObj.AddComponent<RectTransform>();
        elementsRect.anchorMin = new Vector2(0.05f, 0.02f);
        elementsRect.anchorMax = new Vector2(0.95f, 0.18f);
        elementsRect.sizeDelta = Vector2.zero;
        elementsRect.anchoredPosition = Vector2.zero;
        elementsRect.localScale = Vector3.one;
        
        HorizontalLayoutGroup elementsLayout = elementsObj.AddComponent<HorizontalLayoutGroup>();
        elementsLayout.spacing = 5f;
        elementsLayout.childControlWidth = false;
        elementsLayout.childControlHeight = false;
        elementsLayout.childForceExpandWidth = false;
        elementsLayout.childForceExpandHeight = false;
        elementsLayout.childAlignment = TextAnchor.MiddleCenter;
        
        enhancedCard.elementsContainer = elementsObj.transform;
        
        // Create selection glow
        GameObject glowObj = new GameObject("SelectionGlow");
        glowObj.transform.SetParent(cardObj.transform);
        glowObj.transform.SetAsFirstSibling();
        RectTransform glowRect = glowObj.AddComponent<RectTransform>();
        glowRect.anchorMin = Vector2.zero;
        glowRect.anchorMax = Vector2.one;
        glowRect.sizeDelta = new Vector2(10, 10);
        glowRect.anchoredPosition = Vector2.zero;
        glowRect.localScale = Vector3.one;
        
        Image glowImage = glowObj.AddComponent<Image>();
        glowImage.color = new Color(0.9f, 0.5f, 0.2f, 0f);
        glowImage.raycastTarget = false;
        enhancedCard.selectionGlow = glowImage;
        
        // Save as prefab
        string prefabPath = "Assets/Prefabs/Chemistry/EnhancedCompoundCard.prefab";
        
        #if UNITY_EDITOR
        PrefabUtility.SaveAsPrefabAsset(cardObj, prefabPath);
        #endif
        
        DestroyImmediate(cardObj);
        Debug.Log("✓ Enhanced Compound Card prefab created");
    }
    
    private void IntegrateWithChemistrySetup()
    {
        Debug.Log("=== Integrating with existing Chemistry Setup ===");
        
        // Find existing chemistry setup components and update them to use new UI
        ChemistryGameSetup existingSetup = FindObjectOfType<ChemistryGameSetup>();
        if (existingSetup != null)
        {
            // Load the enhanced prefabs
            GameObject enhancedElementPrefab = Resources.Load<GameObject>("Assets/Prefabs/Chemistry/EnhancedElementCard");
            GameObject enhancedCompoundPrefab = Resources.Load<GameObject>("Assets/Prefabs/Chemistry/EnhancedCompoundCard");
            
            if (enhancedElementPrefab != null)
            {
                var field = typeof(ChemistryGameSetup).GetField("elementCardPrefab");
                if (field != null)
                    field.SetValue(existingSetup, enhancedElementPrefab);
            }
            
            if (enhancedCompoundPrefab != null)
            {
                var field = typeof(ChemistryGameSetup).GetField("compoundCardPrefab");
                if (field != null)
                    field.SetValue(existingSetup, enhancedCompoundPrefab);
            }
            
            EditorUtility.SetDirty(existingSetup);
        }
        
        // Update existing managers to use enhanced UI
        SimpleChemistryManager[] managers = FindObjectsOfType<SimpleChemistryManager>();
        foreach (var manager in managers)
        {
            EditorUtility.SetDirty(manager);
        }
        
        Debug.Log("✓ Integration with existing chemistry setup complete");
    }
    
    [ContextMenu("6. Apply Theme to Existing UI")]
    public void ApplyThemeToExistingUI()
    {
        Debug.Log("=== Applying Modern Theme to Existing UI Elements ===");
        
        ModernUITheme theme = ModernUITheme.Instance;
        if (theme == null)
        {
            Debug.LogError("Modern UI Theme not found! Run 'Setup Modern UI Theme' first.");
            return;
        }
        
        // Find and update all buttons in the scene
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            // Add modern button component if it doesn't exist
            ModernButton modernButton = button.GetComponent<ModernButton>();
            if (modernButton == null)
            {
                modernButton = button.gameObject.AddComponent<ModernButton>();
                modernButton.buttonStyle = ModernButton.ButtonStyle.Primary;
            }
        }
        
        // Find and update all text components
        Text[] texts = FindObjectsOfType<Text>();
        foreach (Text text in texts)
        {
            ModernText modernText = text.GetComponent<ModernText>();
            if (modernText == null)
            {
                modernText = text.gameObject.AddComponent<ModernText>();
                modernText.textStyle = ModernText.TextStyle.Body;
            }
        }
        
        // Find and update all image panels
        Image[] images = FindObjectsOfType<Image>();
        foreach (Image image in images)
        {
            if (image.gameObject.name.Contains("Panel") || image.gameObject.name.Contains("Background"))
            {
                ModernPanel modernPanel = image.GetComponent<ModernPanel>();
                if (modernPanel == null)
                {
                    modernPanel = image.gameObject.AddComponent<ModernPanel>();
                    modernPanel.panelStyle = ModernPanel.PanelStyle.Surface;
                }
            }
        }
        
        Debug.Log("✓ Modern theme applied to existing UI elements");
    }
}
#endif