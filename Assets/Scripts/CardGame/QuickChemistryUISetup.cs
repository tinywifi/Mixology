using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

public class QuickChemistryUISetup : MonoBehaviour
{
    [Header("Target Canvas")]
    public Canvas targetCanvas;
    
    [ContextMenu("Create Simple Chemistry UI")]
    public void CreateSimpleUI()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("Target Canvas not assigned!");
            return;
        }
        
        
        HideOldCrapetteUI();
        
        
        SetupCanvasScaling();
        
        
        GameObject mainPanel = CreatePanel("ChemistryGamePanel", targetCanvas.transform);
        RectTransform mainRect = mainPanel.GetComponent<RectTransform>();
        mainRect.anchorMin = Vector2.zero;
        mainRect.anchorMax = Vector2.one;
        mainRect.sizeDelta = Vector2.zero;
        mainRect.anchoredPosition = Vector2.zero;
        
        
        GameObject statusObj = CreateText("StatusText", mainPanel.transform, "Chemistry Game Ready!");
        RectTransform statusRect = statusObj.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.1f, 0.9f);
        statusRect.anchorMax = new Vector2(0.9f, 0.95f);
        statusRect.sizeDelta = Vector2.zero;
        statusRect.anchoredPosition = Vector2.zero;
        Text statusText = statusObj.GetComponent<Text>();
        statusText.fontSize = 48; 
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.color = Color.blue;
        statusText.fontStyle = FontStyle.Bold;
        
        
        GameObject elementCountObj = CreateText("ElementCountText", mainPanel.transform, "Elements: 0/10");
        RectTransform elementCountRect = elementCountObj.GetComponent<RectTransform>();
        elementCountRect.anchorMin = new Vector2(0.1f, 0.85f);
        elementCountRect.anchorMax = new Vector2(0.5f, 0.9f);
        elementCountRect.sizeDelta = Vector2.zero;
        elementCountRect.anchoredPosition = Vector2.zero;
        Text elementCountText = elementCountObj.GetComponent<Text>();
        elementCountText.fontSize = 28;
        elementCountText.fontStyle = FontStyle.Bold;
        
        
        GameObject compoundCountObj = CreateText("CompoundCountText", mainPanel.transform, "Compounds: 0/8");
        RectTransform compoundCountRect = compoundCountObj.GetComponent<RectTransform>();
        compoundCountRect.anchorMin = new Vector2(0.5f, 0.85f);
        compoundCountRect.anchorMax = new Vector2(0.9f, 0.9f);
        compoundCountRect.sizeDelta = Vector2.zero;
        compoundCountRect.anchoredPosition = Vector2.zero;
        Text compoundCountText = compoundCountObj.GetComponent<Text>();
        compoundCountText.fontSize = 28;
        compoundCountText.fontStyle = FontStyle.Bold;
        
        
        GameObject handScrollArea = CreateScrollAreaContainer("PlayerHandScrollArea", mainPanel.transform);
        RectTransform handScrollRect = handScrollArea.GetComponent<RectTransform>();
        handScrollRect.anchorMin = new Vector2(0.05f, 0.5f);
        handScrollRect.anchorMax = new Vector2(0.95f, 0.8f);
        handScrollRect.sizeDelta = Vector2.zero;
        handScrollRect.anchoredPosition = Vector2.zero;
        
        
        GameObject handPanel = handScrollArea.transform.Find("Content").gameObject;
        
        
        GameObject handLabel = CreateText("HandLabel", mainPanel.transform, "Your Elements (Click to Select):");
        RectTransform handLabelRect = handLabel.GetComponent<RectTransform>();
        handLabelRect.anchorMin = new Vector2(0.05f, 0.8f);
        handLabelRect.anchorMax = new Vector2(0.5f, 0.85f);
        handLabelRect.sizeDelta = Vector2.zero;
        handLabelRect.anchoredPosition = Vector2.zero;
        Text handLabelText = handLabel.GetComponent<Text>();
        handLabelText.fontSize = 26;
        handLabelText.fontStyle = FontStyle.Bold;
        
        
        GameObject compoundsScrollArea = CreateScrollAreaContainer("PlayerCompoundsScrollArea", mainPanel.transform);
        RectTransform compoundsScrollRect = compoundsScrollArea.GetComponent<RectTransform>();
        compoundsScrollRect.anchorMin = new Vector2(0.05f, 0.2f);
        compoundsScrollRect.anchorMax = new Vector2(0.95f, 0.45f);
        compoundsScrollRect.sizeDelta = Vector2.zero;
        compoundsScrollRect.anchoredPosition = Vector2.zero;
        
        
        GameObject compoundsPanel = compoundsScrollArea.transform.Find("Content").gameObject;
        
        
        CreateOpponentUI(mainPanel.transform);
        
        
        GameObject compoundsLabel = CreateText("CompoundsLabel", mainPanel.transform, "Your Compounds:");
        RectTransform compoundsLabelRect = compoundsLabel.GetComponent<RectTransform>();
        compoundsLabelRect.anchorMin = new Vector2(0.05f, 0.45f);
        compoundsLabelRect.anchorMax = new Vector2(0.5f, 0.5f);
        compoundsLabelRect.sizeDelta = Vector2.zero;
        compoundsLabelRect.anchoredPosition = Vector2.zero;
        Text compoundsLabelText = compoundsLabel.GetComponent<Text>();
        compoundsLabelText.fontSize = 26;
        compoundsLabelText.fontStyle = FontStyle.Bold;
        
        
        GameObject createButton = CreateButton("CreateCompoundButton", mainPanel.transform, "Create Compound");
        RectTransform createRect = createButton.GetComponent<RectTransform>();
        createRect.anchorMin = new Vector2(0.1f, 0.05f);
        createRect.anchorMax = new Vector2(0.4f, 0.15f);
        createRect.sizeDelta = Vector2.zero;
        createRect.anchoredPosition = Vector2.zero;
        
        GameObject endTurnButton = CreateButton("EndTurnButton", mainPanel.transform, "End Turn");
        RectTransform endTurnRect = endTurnButton.GetComponent<RectTransform>();
        endTurnRect.anchorMin = new Vector2(0.6f, 0.05f);
        endTurnRect.anchorMax = new Vector2(0.9f, 0.15f);
        endTurnRect.sizeDelta = Vector2.zero;
        endTurnRect.anchoredPosition = Vector2.zero;
        
        
        GameObject cheatButton = CreateButton("CheatModeButton", mainPanel.transform, "Cheat Mode");
        RectTransform cheatRect = cheatButton.GetComponent<RectTransform>();
        cheatRect.anchorMin = new Vector2(0.05f, 0.85f);
        cheatRect.anchorMax = new Vector2(0.25f, 0.9f);
        cheatRect.sizeDelta = Vector2.zero;
        cheatRect.anchoredPosition = Vector2.zero;
        
        
        GameObject cheatDisplay = CreateScrollAreaContainer("CheatModeDisplay", mainPanel.transform);
        RectTransform cheatDisplayRect = cheatDisplay.GetComponent<RectTransform>();
        cheatDisplayRect.anchorMin = new Vector2(0.05f, 0.15f);
        cheatDisplayRect.anchorMax = new Vector2(0.35f, 0.45f);
        cheatDisplayRect.sizeDelta = Vector2.zero;
        cheatDisplayRect.anchoredPosition = Vector2.zero;
        cheatDisplay.SetActive(false); 
        
        
        SimpleChemistryManager manager = FindObjectOfType<SimpleChemistryManager>();
        if (manager != null)
        {
            SetupManagerReferences(manager, handPanel, compoundsPanel, createButton, endTurnButton, cheatButton,
                                 statusObj, elementCountObj, compoundCountObj, cheatDisplay);
        }
        
        Debug.Log("✓ Simple Chemistry UI created!");
        Debug.Log("Now add SimpleChemistryManager to a GameObject and link the references.");
        
        EditorUtility.SetDirty(targetCanvas.gameObject);
    }
    
    private GameObject CreatePanel(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        Image image = obj.AddComponent<Image>();
        image.color = new Color(0.9f, 0.9f, 0.9f, 0.1f);
        
        return obj;
    }
    
    private GameObject CreateText(string name, Transform parent, string text)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        Text textComponent = obj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 24; 
        textComponent.alignment = TextAnchor.MiddleLeft;
        textComponent.color = Color.black;
        
        return obj;
    }
    
    private GameObject CreateButton(string name, Transform parent, string text)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        Image image = obj.AddComponent<Image>();
        image.color = Color.white;
        
        Button button = obj.AddComponent<Button>();
        
        
        GameObject textObj = CreateText("Text", obj.transform, text);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        Text buttonText = textObj.GetComponent<Text>();
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.fontSize = 22; 
        buttonText.fontStyle = FontStyle.Bold;
        
        return obj;
    }
    
    private GameObject CreateScrollArea(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        
        Image bg = obj.AddComponent<Image>();
        bg.color = new Color(0.8f, 0.8f, 0.8f, 0.3f);
        
        
        GameObject content = new GameObject("Content");
        content.transform.SetParent(obj.transform);
        content.layer = LayerMask.NameToLayer("UI");
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.localScale = Vector3.one;
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.sizeDelta = Vector2.zero;
        contentRect.anchoredPosition = Vector2.zero;
        
        
        HorizontalLayoutGroup layoutGroup = content.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 20; 
        layoutGroup.padding = new RectOffset(10, 10, 10, 10); 
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;
        
        
        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        
        ScrollRect scrollRect = obj.AddComponent<ScrollRect>();
        scrollRect.content = contentRect;
        scrollRect.horizontal = true;
        scrollRect.vertical = false;
        
        return content; 
    }
    
    private GameObject CreateScrollAreaContainer(string name, Transform parent)
    {
        GameObject scrollArea = new GameObject(name);
        scrollArea.transform.SetParent(parent);
        scrollArea.layer = LayerMask.NameToLayer("UI");
        
        RectTransform scrollRect = scrollArea.AddComponent<RectTransform>();
        scrollRect.localScale = Vector3.one;
        
        
        Image bg = scrollArea.AddComponent<Image>();
        bg.color = new Color(0.8f, 0.8f, 0.8f, 0.3f);
        
        
        GameObject content = new GameObject("Content");
        content.transform.SetParent(scrollArea.transform);
        content.layer = LayerMask.NameToLayer("UI");
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.localScale = Vector3.one;
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.sizeDelta = Vector2.zero;
        contentRect.anchoredPosition = Vector2.zero;
        
        
        HorizontalLayoutGroup layoutGroup = content.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 20; 
        layoutGroup.padding = new RectOffset(10, 10, 10, 10); 
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
        
        
        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        
        ScrollRect scroll = scrollArea.AddComponent<ScrollRect>();
        scroll.content = contentRect;
        scroll.horizontal = true;
        scroll.vertical = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        
        return scrollArea; 
    }
    
    private void CreateOpponentUI(Transform parent)
    {
        
        GameObject opponentPanel = CreatePanel("OpponentArea", parent);
        RectTransform opponentRect = opponentPanel.GetComponent<RectTransform>();
        opponentRect.anchorMin = new Vector2(0.05f, 0.85f);
        opponentRect.anchorMax = new Vector2(0.95f, 0.95f);
        opponentRect.sizeDelta = Vector2.zero;
        opponentRect.anchoredPosition = Vector2.zero;
        
        
        GameObject opponentInfo = CreateText("OpponentInfo", opponentPanel.transform, "Opponent: 0 Elements | 0 Compounds");
        RectTransform opponentInfoRect = opponentInfo.GetComponent<RectTransform>();
        opponentInfoRect.anchorMin = new Vector2(0.1f, 0.2f);
        opponentInfoRect.anchorMax = new Vector2(0.9f, 0.8f);
        opponentInfoRect.sizeDelta = Vector2.zero;
        opponentInfoRect.anchoredPosition = Vector2.zero;
        Text opponentInfoText = opponentInfo.GetComponent<Text>();
        opponentInfoText.fontSize = 24;
        opponentInfoText.fontStyle = FontStyle.Bold;
        opponentInfoText.color = Color.red;
        opponentInfoText.alignment = TextAnchor.MiddleCenter;
        
        
        GameObject opponentCompoundsArea = CreateScrollAreaContainer("OpponentCompoundsArea", parent);
        RectTransform opponentCompoundsRect = opponentCompoundsArea.GetComponent<RectTransform>();
        opponentCompoundsRect.anchorMin = new Vector2(0.7f, 0.05f);
        opponentCompoundsRect.anchorMax = new Vector2(0.95f, 0.35f);
        opponentCompoundsRect.sizeDelta = Vector2.zero;
        opponentCompoundsRect.anchoredPosition = Vector2.zero;
        
        
        GameObject opponentCompoundsLabel = CreateText("OpponentCompoundsLabel", parent, "Opponent Compounds:");
        RectTransform opponentCompoundsLabelRect = opponentCompoundsLabel.GetComponent<RectTransform>();
        opponentCompoundsLabelRect.anchorMin = new Vector2(0.7f, 0.35f);
        opponentCompoundsLabelRect.anchorMax = new Vector2(0.95f, 0.4f);
        opponentCompoundsLabelRect.sizeDelta = Vector2.zero;
        opponentCompoundsLabelRect.anchoredPosition = Vector2.zero;
        Text opponentCompoundsLabelText = opponentCompoundsLabel.GetComponent<Text>();
        opponentCompoundsLabelText.fontSize = 20;
        opponentCompoundsLabelText.fontStyle = FontStyle.Bold;
        opponentCompoundsLabelText.color = Color.red;
        
        Debug.Log("✓ Opponent UI area created for multiplayer support");
    }
    
    private void SetupManagerReferences(SimpleChemistryManager manager, GameObject handPanel, 
                                      GameObject compoundsPanel, GameObject createButton, 
                                      GameObject endTurnButton, GameObject cheatButton, GameObject statusText,
                                      GameObject elementCountText, GameObject compoundCountText, GameObject cheatDisplay)
    {
        
        var fields = typeof(SimpleChemistryManager).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        foreach (var field in fields)
        {
            if (field.Name == "playerHandParent") field.SetValue(manager, handPanel.transform);
            else if (field.Name == "playerCompoundsParent") field.SetValue(manager, compoundsPanel.transform);
            else if (field.Name == "createCompoundButton") field.SetValue(manager, createButton.GetComponent<Button>());
            else if (field.Name == "endTurnButton") field.SetValue(manager, endTurnButton.GetComponent<Button>());
            else if (field.Name == "cheatModeButton") field.SetValue(manager, cheatButton.GetComponent<Button>());
            else if (field.Name == "cheatModeDisplay") field.SetValue(manager, cheatDisplay);
            else if (field.Name == "statusText") field.SetValue(manager, statusText.GetComponent<Text>());
            else if (field.Name == "elementCountText") field.SetValue(manager, elementCountText.GetComponent<Text>());
            else if (field.Name == "compoundCountText") field.SetValue(manager, compoundCountText.GetComponent<Text>());
        }
        
        EditorUtility.SetDirty(manager);
        Debug.Log("✓ Manager references automatically set up!");
    }
    
    private void HideOldCrapetteUI()
    {
        
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        
        foreach (GameObject rootObj in rootObjects)
        {
            
            if (rootObj.name.Contains("CardGame") && !rootObj.name.Contains("Chemistry"))
            {
                Canvas canvas = rootObj.GetComponentInChildren<Canvas>();
                if (canvas != null && canvas != targetCanvas)
                {
                    canvas.gameObject.SetActive(false);
                    Debug.Log($"Hidden old UI: {canvas.gameObject.name}");
                }
            }
            
            
            CardStack[] cardStacks = rootObj.GetComponentsInChildren<CardStack>();
            foreach (var stack in cardStacks)
            {
                stack.gameObject.SetActive(false);
                Debug.Log($"Hidden CardStack: {stack.gameObject.name}");
            }
            
            
            Transform[] transforms = rootObj.GetComponentsInChildren<Transform>();
            foreach (var t in transforms)
            {
                if (t.name.Contains("PlayerBoard") || t.name.Contains("OpponentBoard") || 
                    t.name.Contains("MainStack") || t.name.Contains("LeftStack") || t.name.Contains("RightStack"))
                {
                    t.gameObject.SetActive(false);
                    Debug.Log($"Hidden old UI element: {t.name}");
                }
            }
        }
    }
    
    private void SetupCanvasScaling()
    {
        CanvasScaler scaler = targetCanvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = targetCanvas.gameObject.AddComponent<CanvasScaler>();
        }
        
        
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); 
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f; 
        
        Debug.Log("✓ Canvas scaling configured for larger UI elements");
    }
}
#endif