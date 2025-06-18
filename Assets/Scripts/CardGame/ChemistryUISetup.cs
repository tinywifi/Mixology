using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

public class ChemistryUISetup : MonoBehaviour
{
    [Header("Canvas Reference")]
    public Canvas gameCanvas;
    
    [ContextMenu("Setup Chemistry UI")]
    public void SetupChemistryUI()
    {
        if (gameCanvas == null)
        {
            Debug.LogError("Game Canvas not assigned!");
            return;
        }
        
        CreateChemistryUI();
        Debug.Log("Chemistry UI setup completed!");
    }
    
    private void CreateChemistryUI()
    {
        
        GameObject gamePanel = CreateUIElement("ChemistryGamePanel", gameCanvas.transform);
        RectTransform gamePanelRect = gamePanel.GetComponent<RectTransform>();
        gamePanelRect.anchorMin = Vector2.zero;
        gamePanelRect.anchorMax = Vector2.one;
        gamePanelRect.sizeDelta = Vector2.zero;
        gamePanelRect.anchoredPosition = Vector2.zero;
        
        
        GameObject topPanel = CreateUIElement("TopPanel", gamePanel.transform);
        RectTransform topRect = topPanel.GetComponent<RectTransform>();
        topRect.anchorMin = new Vector2(0, 0.9f);
        topRect.anchorMax = new Vector2(1, 1);
        topRect.sizeDelta = Vector2.zero;
        topRect.anchoredPosition = Vector2.zero;
        
        
        GameObject playerElementCount = CreateTextElement("PlayerElementCount", topPanel.transform, "Elements: 0/10");
        RectTransform playerElementRect = playerElementCount.GetComponent<RectTransform>();
        playerElementRect.anchorMin = new Vector2(0, 0.5f);
        playerElementRect.anchorMax = new Vector2(0.2f, 1);
        playerElementRect.sizeDelta = Vector2.zero;
        playerElementRect.anchoredPosition = Vector2.zero;
        
        
        GameObject turnIndicator = CreateTextElement("TurnIndicator", topPanel.transform, "Your Turn");
        RectTransform turnRect = turnIndicator.GetComponent<RectTransform>();
        turnRect.anchorMin = new Vector2(0.4f, 0);
        turnRect.anchorMax = new Vector2(0.6f, 1);
        turnRect.sizeDelta = Vector2.zero;
        turnRect.anchoredPosition = Vector2.zero;
        Text turnText = turnIndicator.GetComponent<Text>();
        turnText.fontSize = 18;
        turnText.fontStyle = FontStyle.Bold;
        turnText.color = Color.blue;
        
        
        GameObject opponentElementCount = CreateTextElement("OpponentElementCount", topPanel.transform, "Elements: 0/10");
        RectTransform opponentElementRect = opponentElementCount.GetComponent<RectTransform>();
        opponentElementRect.anchorMin = new Vector2(0.8f, 0.5f);
        opponentElementRect.anchorMax = new Vector2(1, 1);
        opponentElementRect.sizeDelta = Vector2.zero;
        opponentElementRect.anchoredPosition = Vector2.zero;
        
        
        GameObject handsPanel = CreateUIElement("HandsPanel", gamePanel.transform);
        RectTransform handsRect = handsPanel.GetComponent<RectTransform>();
        handsRect.anchorMin = new Vector2(0, 0.6f);
        handsRect.anchorMax = new Vector2(1, 0.9f);
        handsRect.sizeDelta = Vector2.zero;
        handsRect.anchoredPosition = Vector2.zero;
        
        
        GameObject playerHandPanel = CreateScrollableArea("PlayerHandPanel", handsPanel.transform);
        RectTransform playerHandRect = playerHandPanel.GetComponent<RectTransform>();
        playerHandRect.anchorMin = new Vector2(0, 0);
        playerHandRect.anchorMax = new Vector2(1, 0.45f);
        playerHandRect.sizeDelta = Vector2.zero;
        playerHandRect.anchoredPosition = Vector2.zero;
        
        
        GameObject opponentHandPanel = CreateScrollableArea("OpponentHandPanel", handsPanel.transform);
        RectTransform opponentHandRect = opponentHandPanel.GetComponent<RectTransform>();
        opponentHandRect.anchorMin = new Vector2(0, 0.55f);
        opponentHandRect.anchorMax = new Vector2(1, 1);
        opponentHandRect.sizeDelta = Vector2.zero;
        opponentHandRect.anchoredPosition = Vector2.zero;
        
        
        GameObject compoundsPanel = CreateUIElement("CompoundsPanel", gamePanel.transform);
        RectTransform compoundsRect = compoundsPanel.GetComponent<RectTransform>();
        compoundsRect.anchorMin = new Vector2(0, 0.3f);
        compoundsRect.anchorMax = new Vector2(1, 0.6f);
        compoundsRect.sizeDelta = Vector2.zero;
        compoundsRect.anchoredPosition = Vector2.zero;
        
        
        GameObject playerCompoundsPanel = CreateScrollableArea("PlayerCompoundsPanel", compoundsPanel.transform);
        RectTransform playerCompoundsRect = playerCompoundsPanel.GetComponent<RectTransform>();
        playerCompoundsRect.anchorMin = new Vector2(0, 0);
        playerCompoundsRect.anchorMax = new Vector2(1, 0.45f);
        playerCompoundsRect.sizeDelta = Vector2.zero;
        playerCompoundsRect.anchoredPosition = Vector2.zero;
        
        
        GameObject playerCompoundCount = CreateTextElement("PlayerCompoundCount", playerCompoundsPanel.transform, "Compounds: 0/8");
        RectTransform playerCompoundRect = playerCompoundCount.GetComponent<RectTransform>();
        playerCompoundRect.anchorMin = new Vector2(0, 0.8f);
        playerCompoundRect.anchorMax = new Vector2(0.3f, 1);
        playerCompoundRect.sizeDelta = Vector2.zero;
        playerCompoundRect.anchoredPosition = Vector2.zero;
        
        
        GameObject opponentCompoundsPanel = CreateScrollableArea("OpponentCompoundsPanel", compoundsPanel.transform);
        RectTransform opponentCompoundsRect = opponentCompoundsPanel.GetComponent<RectTransform>();
        opponentCompoundsRect.anchorMin = new Vector2(0, 0.55f);
        opponentCompoundsRect.anchorMax = new Vector2(1, 1);
        opponentCompoundsRect.sizeDelta = Vector2.zero;
        opponentCompoundsRect.anchoredPosition = Vector2.zero;
        
        
        GameObject opponentCompoundCount = CreateTextElement("OpponentCompoundCount", opponentCompoundsPanel.transform, "Compounds: 0/8");
        RectTransform opponentCompoundRect = opponentCompoundCount.GetComponent<RectTransform>();
        opponentCompoundRect.anchorMin = new Vector2(0, 0.8f);
        opponentCompoundRect.anchorMax = new Vector2(0.3f, 1);
        opponentCompoundRect.sizeDelta = Vector2.zero;
        opponentCompoundRect.anchoredPosition = Vector2.zero;
        
        
        GameObject buttonsPanel = CreateUIElement("ButtonsPanel", gamePanel.transform);
        RectTransform buttonsRect = buttonsPanel.GetComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(0, 0.1f);
        buttonsRect.anchorMax = new Vector2(1, 0.3f);
        buttonsRect.sizeDelta = Vector2.zero;
        buttonsRect.anchoredPosition = Vector2.zero;
        
        
        GameObject createButton = CreateButtonElement("CreateCompoundButton", buttonsPanel.transform, "Create Compound");
        RectTransform createRect = createButton.GetComponent<RectTransform>();
        createRect.anchorMin = new Vector2(0.1f, 0.6f);
        createRect.anchorMax = new Vector2(0.3f, 0.9f);
        createRect.sizeDelta = Vector2.zero;
        createRect.anchoredPosition = Vector2.zero;
        
        
        GameObject reactionButton = CreateButtonElement("PerformReactionButton", buttonsPanel.transform, "Perform Reaction");
        RectTransform reactionRect = reactionButton.GetComponent<RectTransform>();
        reactionRect.anchorMin = new Vector2(0.4f, 0.6f);
        reactionRect.anchorMax = new Vector2(0.6f, 0.9f);
        reactionRect.sizeDelta = Vector2.zero;
        reactionRect.anchoredPosition = Vector2.zero;
        
        
        GameObject endTurnButton = CreateButtonElement("EndTurnButton", buttonsPanel.transform, "End Turn");
        RectTransform endTurnRect = endTurnButton.GetComponent<RectTransform>();
        endTurnRect.anchorMin = new Vector2(0.7f, 0.6f);
        endTurnRect.anchorMax = new Vector2(0.9f, 0.9f);
        endTurnRect.sizeDelta = Vector2.zero;
        endTurnRect.anchoredPosition = Vector2.zero;
        
        
        GameObject statusPanel = CreateUIElement("StatusPanel", gamePanel.transform);
        RectTransform statusRect = statusPanel.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 0);
        statusRect.anchorMax = new Vector2(1, 0.1f);
        statusRect.sizeDelta = Vector2.zero;
        statusRect.anchoredPosition = Vector2.zero;
        
        EditorUtility.SetDirty(gameCanvas.gameObject);
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
    
    private GameObject CreateTextElement(string name, Transform parent, string text)
    {
        GameObject obj = CreateUIElement(name, parent);
        
        Text textComponent = obj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 14;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = Color.black;
        
        return obj;
    }
    
    private GameObject CreateButtonElement(string name, Transform parent, string text)
    {
        GameObject obj = CreateUIElement(name, parent);
        
        Image image = obj.AddComponent<Image>();
        image.color = Color.white;
        
        Button button = obj.AddComponent<Button>();
        
        GameObject textObj = CreateTextElement("Text", obj.transform, text);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        return obj;
    }
    
    private GameObject CreateScrollableArea(string name, Transform parent)
    {
        GameObject obj = CreateUIElement(name, parent);
        
        
        ScrollRect scrollRect = obj.AddComponent<ScrollRect>();
        scrollRect.horizontal = true;
        scrollRect.vertical = false;
        
        
        GameObject viewport = CreateUIElement("Viewport", obj.transform);
        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        viewportRect.anchoredPosition = Vector2.zero;
        
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(1, 1, 1, 0.1f);
        
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        
        
        GameObject content = CreateUIElement("Content", viewport.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(0, 1);
        contentRect.sizeDelta = new Vector2(0, 0);
        contentRect.anchoredPosition = Vector2.zero;
        
        HorizontalLayoutGroup layoutGroup = content.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 10;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        
        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;
        
        return content; 
    }
}
#endif