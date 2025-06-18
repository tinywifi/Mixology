using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class SimpleChemistrySetup : MonoBehaviour
{
    [ContextMenu("Create Chemistry Database Only")]
    public void CreateDatabaseOnly()
    {
        
        ChemistryDatabase database = ScriptableObject.CreateInstance<ChemistryDatabase>();
        database.name = "ChemistryDatabase";
        
        
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        }
        
        string assetPath = "Assets/ScriptableObjects/ChemistryDatabase.asset";
        AssetDatabase.CreateAsset(database, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"✓ Created Chemistry Database at: {assetPath}");
        Debug.Log("Next: Add ChemistryDataInitializer to this GameObject and run 'Initialize Chemistry Database'");
    }
    
    [ContextMenu("Initialize Chemistry Database")]
    public void InitializeDatabase()
    {
        
        ChemistryDatabase database = AssetDatabase.LoadAssetAtPath<ChemistryDatabase>("Assets/ScriptableObjects/ChemistryDatabase.asset");
        
        if (database == null)
        {
            Debug.LogError("Chemistry Database not found! Run 'Create Chemistry Database Only' first.");
            return;
        }
        
        
        ChemistryDataInitializer initializer = GetComponent<ChemistryDataInitializer>();
        if (initializer == null)
        {
            initializer = gameObject.AddComponent<ChemistryDataInitializer>();
        }
        
        initializer.database = database;
        initializer.InitializeDatabase();
        
        Debug.Log($"✓ Initialized database with {database.allElements.Count} elements, {database.allCompounds.Count} compounds, {database.allReactions.Count} reactions");
    }
    
    [ContextMenu("Create Simple Element Card Prefab")]
    public void CreateElementCardPrefab()
    {
        
        GameObject cardObj = new GameObject("ElementCard");
        
        
        RectTransform rect = cardObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 180);
        
        
        UnityEngine.UI.Image bg = cardObj.AddComponent<UnityEngine.UI.Image>();
        bg.color = Color.white;
        
        
        ElementCard elementCard = cardObj.AddComponent<ElementCard>();
        
        
        GameObject symbolObj = new GameObject("Symbol");
        symbolObj.transform.SetParent(cardObj.transform);
        RectTransform symbolRect = symbolObj.AddComponent<RectTransform>();
        symbolRect.anchorMin = new Vector2(0.2f, 0.6f);
        symbolRect.anchorMax = new Vector2(0.8f, 0.9f);
        symbolRect.anchoredPosition = Vector2.zero;
        symbolRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Text symbolText = symbolObj.AddComponent<UnityEngine.UI.Text>();
        symbolText.text = "H";
        symbolText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        symbolText.fontSize = 24;
        symbolText.fontStyle = FontStyle.Bold;
        symbolText.alignment = TextAnchor.MiddleCenter;
        symbolText.color = Color.black;
        
        
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(cardObj.transform);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.1f, 0.4f);
        nameRect.anchorMax = new Vector2(0.9f, 0.6f);
        nameRect.anchoredPosition = Vector2.zero;
        nameRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Text nameText = nameObj.AddComponent<UnityEngine.UI.Text>();
        nameText.text = "Hydrogen";
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 12;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.black;
        
        
        GameObject oxidationObj = new GameObject("Oxidation");
        oxidationObj.transform.SetParent(cardObj.transform);
        RectTransform oxidationRect = oxidationObj.AddComponent<RectTransform>();
        oxidationRect.anchorMin = new Vector2(0.7f, 0.8f);
        oxidationRect.anchorMax = new Vector2(1f, 1f);
        oxidationRect.anchoredPosition = Vector2.zero;
        oxidationRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Text oxidationText = oxidationObj.AddComponent<UnityEngine.UI.Text>();
        oxidationText.text = "+1";
        oxidationText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        oxidationText.fontSize = 10;
        oxidationText.alignment = TextAnchor.MiddleCenter;
        oxidationText.color = Color.red;
        
        
        GameObject highlightObj = new GameObject("SelectionHighlight");
        highlightObj.transform.SetParent(cardObj.transform);
        RectTransform highlightRect = highlightObj.AddComponent<RectTransform>();
        highlightRect.anchorMin = Vector2.zero;
        highlightRect.anchorMax = Vector2.one;
        highlightRect.anchoredPosition = Vector2.zero;
        highlightRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Image highlightImage = highlightObj.AddComponent<UnityEngine.UI.Image>();
        highlightImage.color = new Color(1f, 1f, 0f, 0.5f);
        highlightObj.SetActive(false);
        
        
        System.Reflection.FieldInfo[] fields = typeof(ElementCard).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        foreach (var field in fields)
        {
            if (field.Name == "cardImage") field.SetValue(elementCard, bg);
            else if (field.Name == "symbolText") field.SetValue(elementCard, symbolText);
            else if (field.Name == "nameText") field.SetValue(elementCard, nameText);
            else if (field.Name == "oxidationText") field.SetValue(elementCard, oxidationText);
            else if (field.Name == "selectionHighlight") field.SetValue(elementCard, highlightObj);
        }
        
        
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Chemistry"))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Chemistry");
        }
        
        
        string prefabPath = "Assets/Prefabs/Chemistry/ElementCard.prefab";
        PrefabUtility.SaveAsPrefabAsset(cardObj, prefabPath);
        
        
        DestroyImmediate(cardObj);
        
        Debug.Log($"✓ Created Element Card prefab at: {prefabPath}");
    }
    
    [ContextMenu("Create Simple Compound Card Prefab")]
    public void CreateCompoundCardPrefab()
    {
        
        GameObject cardObj = new GameObject("CompoundCard");
        
        
        RectTransform rect = cardObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(140, 200);
        
        
        UnityEngine.UI.Image bg = cardObj.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.9f, 0.9f, 1f, 1f);
        
        
        CompoundCard compoundCard = cardObj.AddComponent<CompoundCard>();
        
        
        GameObject formulaObj = new GameObject("Formula");
        formulaObj.transform.SetParent(cardObj.transform);
        RectTransform formulaRect = formulaObj.AddComponent<RectTransform>();
        formulaRect.anchorMin = new Vector2(0.1f, 0.8f);
        formulaRect.anchorMax = new Vector2(0.9f, 0.95f);
        formulaRect.anchoredPosition = Vector2.zero;
        formulaRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Text formulaText = formulaObj.AddComponent<UnityEngine.UI.Text>();
        formulaText.text = "H2O";
        formulaText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        formulaText.fontSize = 18;
        formulaText.fontStyle = FontStyle.Bold;
        formulaText.alignment = TextAnchor.MiddleCenter;
        formulaText.color = Color.black;
        
        
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(cardObj.transform);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.1f, 0.65f);
        nameRect.anchorMax = new Vector2(0.9f, 0.8f);
        nameRect.anchoredPosition = Vector2.zero;
        nameRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Text nameText = nameObj.AddComponent<UnityEngine.UI.Text>();
        nameText.text = "Water";
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 12;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.black;
        
        
        GameObject effectObj = new GameObject("Effect");
        effectObj.transform.SetParent(cardObj.transform);
        RectTransform effectRect = effectObj.AddComponent<RectTransform>();
        effectRect.anchorMin = new Vector2(0.05f, 0.2f);
        effectRect.anchorMax = new Vector2(0.95f, 0.65f);
        effectRect.anchoredPosition = Vector2.zero;
        effectRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Text effectText = effectObj.AddComponent<UnityEngine.UI.Text>();
        effectText.text = "No special effect";
        effectText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        effectText.fontSize = 10;
        effectText.alignment = TextAnchor.UpperCenter;
        effectText.color = Color.blue;
        
        
        GameObject highlightObj = new GameObject("SelectionHighlight");
        highlightObj.transform.SetParent(cardObj.transform);
        RectTransform highlightRect = highlightObj.AddComponent<RectTransform>();
        highlightRect.anchorMin = Vector2.zero;
        highlightRect.anchorMax = Vector2.one;
        highlightRect.anchoredPosition = Vector2.zero;
        highlightRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Image highlightImage = highlightObj.AddComponent<UnityEngine.UI.Image>();
        highlightImage.color = new Color(0f, 1f, 0f, 0.5f);
        highlightObj.SetActive(false);
        
        
        GameObject usedObj = new GameObject("UsedIndicator");
        usedObj.transform.SetParent(cardObj.transform);
        RectTransform usedRect = usedObj.AddComponent<RectTransform>();
        usedRect.anchorMin = new Vector2(0.8f, 0.8f);
        usedRect.anchorMax = new Vector2(1f, 1f);
        usedRect.anchoredPosition = Vector2.zero;
        usedRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Image usedImage = usedObj.AddComponent<UnityEngine.UI.Image>();
        usedImage.color = Color.red;
        usedObj.SetActive(false);
        
        
        System.Reflection.FieldInfo[] fields = typeof(CompoundCard).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        foreach (var field in fields)
        {
            if (field.Name == "cardImage") field.SetValue(compoundCard, bg);
            else if (field.Name == "formulaText") field.SetValue(compoundCard, formulaText);
            else if (field.Name == "nameText") field.SetValue(compoundCard, nameText);
            else if (field.Name == "effectText") field.SetValue(compoundCard, effectText);
            else if (field.Name == "selectionHighlight") field.SetValue(compoundCard, highlightObj);
            else if (field.Name == "usedIndicator") field.SetValue(compoundCard, usedObj);
        }
        
        
        string prefabPath = "Assets/Prefabs/Chemistry/CompoundCard.prefab";
        PrefabUtility.SaveAsPrefabAsset(cardObj, prefabPath);
        
        
        DestroyImmediate(cardObj);
        
        Debug.Log($"✓ Created Compound Card prefab at: {prefabPath}");
    }
    
    [ContextMenu("Complete Simple Setup")]
    public void CompleteSimpleSetup()
    {
        Debug.Log("=== STARTING SIMPLE CHEMISTRY SETUP ===");
        
        CreateDatabaseOnly();
        InitializeDatabase();
        CreateElementCardPrefab();
        CreateCompoundCardPrefab();
        
        Debug.Log("=== SETUP COMPLETE ===");
        Debug.Log("✓ Chemistry Database created and populated");
        Debug.Log("✓ Element and Compound card prefabs created");
        Debug.Log("");
        Debug.Log("NEXT STEPS:");
        Debug.Log("1. Open CardGame-Board scene");
        Debug.Log("2. Replace CardManager script with ChemistryCardManager");
        Debug.Log("3. Link the ChemistryDatabase asset and prefabs in the inspector");
        Debug.Log("4. Test the game!");
    }
}
#endif