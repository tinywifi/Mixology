using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

public class ChemistrySetupHelper : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject elementCardPrefab;
    public GameObject compoundCardPrefab;
    
    [Header("Database")]
    public ChemistryDatabase chemistryDatabase;
    public ChemistryDataInitializer dataInitializer;
    
    [ContextMenu("Setup Chemistry Game")]
    public void SetupChemistryGame()
    {
        CreateElementCardPrefab();
        CreateCompoundCardPrefab();
        CreateChemistryDatabase();
        
        if (dataInitializer != null)
        {
            dataInitializer.database = chemistryDatabase;
            dataInitializer.InitializeDatabase();
        }
        
        Debug.Log("Chemistry game setup completed!");
    }
    
    private void CreateElementCardPrefab()
    {
        
        GameObject cardObj = new GameObject("ElementCard");
        cardObj.layer = LayerMask.NameToLayer("UI");
        
        
        RectTransform rectTransform = cardObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(120, 180);
        
        
        Image cardImage = cardObj.AddComponent<Image>();
        cardImage.color = Color.white;
        cardImage.raycastTarget = true;
        
        
        ElementCard elementCard = cardObj.AddComponent<ElementCard>();
        
        
        GameObject symbolObj = new GameObject("Symbol");
        symbolObj.transform.SetParent(cardObj.transform);
        symbolObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform symbolRect = symbolObj.AddComponent<RectTransform>();
        symbolRect.anchorMin = new Vector2(0.5f, 0.7f);
        symbolRect.anchorMax = new Vector2(0.5f, 0.9f);
        symbolRect.anchoredPosition = Vector2.zero;
        symbolRect.sizeDelta = new Vector2(80, 30);
        
        Text symbolText = symbolObj.AddComponent<Text>();
        symbolText.text = "H";
        symbolText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        symbolText.fontSize = 24;
        symbolText.fontStyle = FontStyle.Bold;
        symbolText.alignment = TextAnchor.MiddleCenter;
        symbolText.color = Color.black;
        
        
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(cardObj.transform);
        nameObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.1f, 0.5f);
        nameRect.anchorMax = new Vector2(0.9f, 0.7f);
        nameRect.anchoredPosition = Vector2.zero;
        nameRect.sizeDelta = Vector2.zero;
        
        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = "Hydrogen";
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 12;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.black;
        
        
        GameObject oxidationObj = new GameObject("Oxidation");
        oxidationObj.transform.SetParent(cardObj.transform);
        oxidationObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform oxidationRect = oxidationObj.AddComponent<RectTransform>();
        oxidationRect.anchorMin = new Vector2(0.7f, 0.8f);
        oxidationRect.anchorMax = new Vector2(1f, 1f);
        oxidationRect.anchoredPosition = Vector2.zero;
        oxidationRect.sizeDelta = Vector2.zero;
        
        Text oxidationText = oxidationObj.AddComponent<Text>();
        oxidationText.text = "+1";
        oxidationText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        oxidationText.fontSize = 10;
        oxidationText.alignment = TextAnchor.MiddleCenter;
        oxidationText.color = Color.red;
        
        
        GameObject highlightObj = new GameObject("SelectionHighlight");
        highlightObj.transform.SetParent(cardObj.transform);
        highlightObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform highlightRect = highlightObj.AddComponent<RectTransform>();
        highlightRect.anchorMin = Vector2.zero;
        highlightRect.anchorMax = Vector2.one;
        highlightRect.anchoredPosition = Vector2.zero;
        highlightRect.sizeDelta = Vector2.zero;
        
        Image highlightImage = highlightObj.AddComponent<Image>();
        highlightImage.color = new Color(1f, 1f, 0f, 0.5f); 
        highlightObj.SetActive(false);
        
        
        var cardImageField = typeof(ElementCard).GetField("cardImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var symbolTextField = typeof(ElementCard).GetField("symbolText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var nameTextField = typeof(ElementCard).GetField("nameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var oxidationTextField = typeof(ElementCard).GetField("oxidationText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var highlightField = typeof(ElementCard).GetField("selectionHighlight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        cardImageField?.SetValue(elementCard, cardImage);
        symbolTextField?.SetValue(elementCard, symbolText);
        nameTextField?.SetValue(elementCard, nameText);
        oxidationTextField?.SetValue(elementCard, oxidationText);
        highlightField?.SetValue(elementCard, highlightObj);
        
        
        string prefabPath = "Assets/Prefabs/Chemistry/ElementCard.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cardObj, prefabPath);
        
        elementCardPrefab = prefab;
        
        
        DestroyImmediate(cardObj);
        
        Debug.Log("Element Card prefab created at: " + prefabPath);
    }
    
    private void CreateCompoundCardPrefab()
    {
        
        GameObject cardObj = new GameObject("CompoundCard");
        cardObj.layer = LayerMask.NameToLayer("UI");
        
        
        RectTransform rectTransform = cardObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(140, 200);
        
        
        Image cardImage = cardObj.AddComponent<Image>();
        cardImage.color = new Color(0.9f, 0.9f, 1f, 1f); 
        cardImage.raycastTarget = true;
        
        
        CompoundCard compoundCard = cardObj.AddComponent<CompoundCard>();
        
        
        GameObject formulaObj = new GameObject("Formula");
        formulaObj.transform.SetParent(cardObj.transform);
        formulaObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform formulaRect = formulaObj.AddComponent<RectTransform>();
        formulaRect.anchorMin = new Vector2(0.1f, 0.8f);
        formulaRect.anchorMax = new Vector2(0.9f, 0.95f);
        formulaRect.anchoredPosition = Vector2.zero;
        formulaRect.sizeDelta = Vector2.zero;
        
        Text formulaText = formulaObj.AddComponent<Text>();
        formulaText.text = "H2O";
        formulaText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        formulaText.fontSize = 18;
        formulaText.fontStyle = FontStyle.Bold;
        formulaText.alignment = TextAnchor.MiddleCenter;
        formulaText.color = Color.black;
        
        
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(cardObj.transform);
        nameObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.1f, 0.65f);
        nameRect.anchorMax = new Vector2(0.9f, 0.8f);
        nameRect.anchoredPosition = Vector2.zero;
        nameRect.sizeDelta = Vector2.zero;
        
        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = "Water";
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 12;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.black;
        
        
        GameObject effectObj = new GameObject("Effect");
        effectObj.transform.SetParent(cardObj.transform);
        effectObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform effectRect = effectObj.AddComponent<RectTransform>();
        effectRect.anchorMin = new Vector2(0.05f, 0.2f);
        effectRect.anchorMax = new Vector2(0.95f, 0.65f);
        effectRect.anchoredPosition = Vector2.zero;
        effectRect.sizeDelta = Vector2.zero;
        
        Text effectText = effectObj.AddComponent<Text>();
        effectText.text = "No special effect";
        effectText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        effectText.fontSize = 10;
        effectText.alignment = TextAnchor.UpperCenter;
        effectText.color = Color.blue;
        
        
        GameObject highlightObj = new GameObject("SelectionHighlight");
        highlightObj.transform.SetParent(cardObj.transform);
        highlightObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform highlightRect = highlightObj.AddComponent<RectTransform>();
        highlightRect.anchorMin = Vector2.zero;
        highlightRect.anchorMax = Vector2.one;
        highlightRect.anchoredPosition = Vector2.zero;
        highlightRect.sizeDelta = Vector2.zero;
        
        Image highlightImage = highlightObj.AddComponent<Image>();
        highlightImage.color = new Color(0f, 1f, 0f, 0.5f); 
        highlightObj.SetActive(false);
        
        
        GameObject usedObj = new GameObject("UsedIndicator");
        usedObj.transform.SetParent(cardObj.transform);
        usedObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform usedRect = usedObj.AddComponent<RectTransform>();
        usedRect.anchorMin = new Vector2(0.8f, 0.8f);
        usedRect.anchorMax = new Vector2(1f, 1f);
        usedRect.anchoredPosition = Vector2.zero;
        usedRect.sizeDelta = Vector2.zero;
        
        Image usedImage = usedObj.AddComponent<Image>();
        usedImage.color = Color.red;
        usedObj.SetActive(false);
        
        
        var cardImageField = typeof(CompoundCard).GetField("cardImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var formulaTextField = typeof(CompoundCard).GetField("formulaText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var nameTextField = typeof(CompoundCard).GetField("nameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var effectTextField = typeof(CompoundCard).GetField("effectText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var highlightField = typeof(CompoundCard).GetField("selectionHighlight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var usedField = typeof(CompoundCard).GetField("usedIndicator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        cardImageField?.SetValue(compoundCard, cardImage);
        formulaTextField?.SetValue(compoundCard, formulaText);
        nameTextField?.SetValue(compoundCard, nameText);
        effectTextField?.SetValue(compoundCard, effectText);
        highlightField?.SetValue(compoundCard, highlightObj);
        usedField?.SetValue(compoundCard, usedObj);
        
        
        string prefabPath = "Assets/Prefabs/Chemistry/CompoundCard.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cardObj, prefabPath);
        
        compoundCardPrefab = prefab;
        
        
        DestroyImmediate(cardObj);
        
        Debug.Log("Compound Card prefab created at: " + prefabPath);
    }
    
    private void CreateChemistryDatabase()
    {
        
        ChemistryDatabase database = ScriptableObject.CreateInstance<ChemistryDatabase>();
        
        string assetPath = "Assets/ScriptableObjects/ChemistryDatabase.asset";
        AssetDatabase.CreateAsset(database, assetPath);
        
        chemistryDatabase = database;
        
        Debug.Log("Chemistry Database created at: " + assetPath);
    }
}
#endif