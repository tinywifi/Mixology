using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

public class ChemistryGameSetup : MonoBehaviour
{
    [Header("Required References")]
    public Canvas targetCanvas;
    public ChemistryDatabase chemistryDatabase;
    public GameObject elementCardPrefab;
    public GameObject compoundCardPrefab;
    
    [ContextMenu("1. Setup Chemistry Data Only")]
    public void SetupChemistryDataOnly()
    {
        SetupChemistryData();
    }
    
    [ContextMenu("2. Complete Chemistry Game Setup")]
    public void CompleteSetup()
    {
        Debug.Log("=== Starting Complete Chemistry Game Setup ===");
        
        
        SetupChemistryData();
        
        
        AssignElementTextures();
        
        
        CreateChemistryUI();
        
        
        SetupGameManager();
        
        Debug.Log("=== Chemistry Game Setup Complete ===");
    }
    
    [ContextMenu("3. Create ALL Chemistry Compounds")]
    public void CreateAllChemistryCompounds()
    {
        if (chemistryDatabase == null)
        {
            Debug.LogError("Chemistry Database not assigned!");
            return;
        }
        
        
        chemistryDatabase.allCompounds.Clear();
        
        
        CreateAllCompounds();
        
        EditorUtility.SetDirty(chemistryDatabase);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"✓ Created {chemistryDatabase.allCompounds.Count} compounds according to specifications");
    }
    
    private void CreateBasicCompounds()
    {
        
        CreateCompoundAsset("Water", "H2O", new System.Collections.Generic.Dictionary<string, int> 
            { {"H", 2}, {"O", 1} }, CompoundEffect.None, 0);
        
        
        CreateCompoundAsset("Hydrochloric Acid", "HCl", new System.Collections.Generic.Dictionary<string, int> 
            { {"H", 1}, {"Cl", 1} }, CompoundEffect.SkipPlayerTurn, 0);
        
        
        CreateCompoundAsset("Sodium Chloride", "NaCl", new System.Collections.Generic.Dictionary<string, int> 
            { {"Na", 1}, {"Cl", 1} }, CompoundEffect.ReceiveElements, 4);
        
        
        CreateCompoundAsset("Carbon Dioxide", "CO2", new System.Collections.Generic.Dictionary<string, int> 
            { {"C", 1}, {"O", 2} }, CompoundEffect.DrawElements, 4);
        
        Debug.Log("Created basic test compounds: H2O, HCl, NaCl, CO2");
    }
    
    [ContextMenu("Setup Chemistry Data Only")]
    public void SetupChemistryData()
    {
        if (chemistryDatabase == null)
        {
            Debug.LogError("Chemistry Database not assigned!");
            return;
        }
        
        Debug.Log("Setting up chemistry compounds and reactions...");
        
        
        chemistryDatabase.allCompounds.Clear();
        
        
        CreateAllCompounds();
        
        EditorUtility.SetDirty(chemistryDatabase);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"✓ Created {chemistryDatabase.allCompounds.Count} compounds");
    }
    
    private void CreateAllCompounds()
    {
        
        CreateCompoundAsset("Water", "H2O", new System.Collections.Generic.Dictionary<string, int> 
            { {"H", 2}, {"O", 1} }, CompoundEffect.None, 0);
        
        
        CreateCompoundAsset("Carbon Dioxide", "CO2", new System.Collections.Generic.Dictionary<string, int> 
            { {"C", 1}, {"O", 2} }, CompoundEffect.DrawElements, 4);
        
        
        CreateCompoundAsset("Hydrochloric Acid", "HCl", new System.Collections.Generic.Dictionary<string, int> 
            { {"H", 1}, {"Cl", 1} }, CompoundEffect.SkipPlayerTurn, 0);
        
        CreateCompoundAsset("Hydrofluoric Acid", "HF", new System.Collections.Generic.Dictionary<string, int> 
            { {"H", 1}, {"F", 1} }, CompoundEffect.SkipPlayerTurn, 0);
        
        CreateCompoundAsset("Hydrobromic Acid", "HBr", new System.Collections.Generic.Dictionary<string, int> 
            { {"H", 1}, {"Br", 1} }, CompoundEffect.SkipPlayerTurn, 0);
        
        
        CreateCompoundAsset("Sodium Hydroxide", "NaOH", new System.Collections.Generic.Dictionary<string, int> 
            { {"Na", 1}, {"O", 1}, {"H", 1} }, CompoundEffect.DiscardElements, 3);
        
        CreateCompoundAsset("Potassium Hydroxide", "KOH", new System.Collections.Generic.Dictionary<string, int> 
            { {"K", 1}, {"O", 1}, {"H", 1} }, CompoundEffect.DiscardElements, 3);
        
        
        CreateCompoundAsset("Calcium Hydroxide", "Ca(OH)2", new System.Collections.Generic.Dictionary<string, int> 
            { {"Ca", 1}, {"O", 2}, {"H", 2} }, CompoundEffect.DiscardElements, 3);
        
        
        CreateCompoundAsset("Sodium Chloride", "NaCl", new System.Collections.Generic.Dictionary<string, int> 
            { {"Na", 1}, {"Cl", 1} }, CompoundEffect.ReceiveElements, 4);
        
        CreateCompoundAsset("Potassium Fluoride", "KF", new System.Collections.Generic.Dictionary<string, int> 
            { {"K", 1}, {"F", 1} }, CompoundEffect.ReceiveElements, 4);
        
        CreateCompoundAsset("Calcium Chloride", "CaCl2", new System.Collections.Generic.Dictionary<string, int> 
            { {"Ca", 1}, {"Cl", 2} }, CompoundEffect.ReceiveElements, 4);
        
        
        CreateCompoundAsset("Rubidium Hydroxide", "RbOH", new System.Collections.Generic.Dictionary<string, int> 
            { {"Rb", 1}, {"O", 1}, {"H", 1} }, CompoundEffect.DiscardElements, 3);
        
        
        CreateCompoundAsset("Magnesium Hydroxide", "Mg(OH)2", new System.Collections.Generic.Dictionary<string, int> 
            { {"Mg", 1}, {"O", 2}, {"H", 2} }, CompoundEffect.DiscardElements, 3);
        
        CreateCompoundAsset("Strontium Hydroxide", "Sr(OH)2", new System.Collections.Generic.Dictionary<string, int> 
            { {"Sr", 1}, {"O", 2}, {"H", 2} }, CompoundEffect.DiscardElements, 3);
        
        
        CreateCompoundAsset("Sodium Fluoride", "NaF", new System.Collections.Generic.Dictionary<string, int> 
            { {"Na", 1}, {"F", 1} }, CompoundEffect.ReceiveElements, 4);
        
        CreateCompoundAsset("Sodium Bromide", "NaBr", new System.Collections.Generic.Dictionary<string, int> 
            { {"Na", 1}, {"Br", 1} }, CompoundEffect.ReceiveElements, 4);
        
        CreateCompoundAsset("Potassium Chloride", "KCl", new System.Collections.Generic.Dictionary<string, int> 
            { {"K", 1}, {"Cl", 1} }, CompoundEffect.ReceiveElements, 4);
        
        CreateCompoundAsset("Potassium Bromide", "KBr", new System.Collections.Generic.Dictionary<string, int> 
            { {"K", 1}, {"Br", 1} }, CompoundEffect.ReceiveElements, 4);
        
        CreateCompoundAsset("Rubidium Chloride", "RbCl", new System.Collections.Generic.Dictionary<string, int> 
            { {"Rb", 1}, {"Cl", 1} }, CompoundEffect.ReceiveElements, 4);
        
        
        CreateCompoundAsset("Magnesium Chloride", "MgCl2", new System.Collections.Generic.Dictionary<string, int> 
            { {"Mg", 1}, {"Cl", 2} }, CompoundEffect.ReceiveElements, 4);
        
        CreateCompoundAsset("Strontium Chloride", "SrCl2", new System.Collections.Generic.Dictionary<string, int> 
            { {"Sr", 1}, {"Cl", 2} }, CompoundEffect.ReceiveElements, 4);
        
        CreateCompoundAsset("Copper Chloride", "CuCl2", new System.Collections.Generic.Dictionary<string, int> 
            { {"Cu", 1}, {"Cl", 2} }, CompoundEffect.ReceiveElements, 4);
        
        CreateCompoundAsset("Zinc Chloride", "ZnCl2", new System.Collections.Generic.Dictionary<string, int> 
            { {"Zn", 1}, {"Cl", 2} }, CompoundEffect.ReceiveElements, 4);
        
        
        CreateCompoundAsset("Copper Metal", "Cu3", new System.Collections.Generic.Dictionary<string, int> 
            { {"Cu", 3} }, CompoundEffect.NegateDissolusion, 0);
        
        CreateCompoundAsset("Zinc Metal", "Zn3", new System.Collections.Generic.Dictionary<string, int> 
            { {"Zn", 3} }, CompoundEffect.NegateDissolusion, 0);
        
        
        CreateCompoundAsset("Sodium Hydride", "Na2H2", new System.Collections.Generic.Dictionary<string, int> 
            { {"Na", 2}, {"H", 2} }, CompoundEffect.SwapHands, 0);
        
        CreateCompoundAsset("Potassium Hydride", "K2H2", new System.Collections.Generic.Dictionary<string, int> 
            { {"K", 2}, {"H", 2} }, CompoundEffect.SwapHands, 0);
        
        CreateCompoundAsset("Rubidium Hydride", "Rb2H2", new System.Collections.Generic.Dictionary<string, int> 
            { {"Rb", 2}, {"H", 2} }, CompoundEffect.SwapHands, 0);
        
        
        CreateCompoundAsset("Methane", "CH4", new System.Collections.Generic.Dictionary<string, int> 
            { {"C", 1}, {"H", 4} }, CompoundEffect.DiscardDraw, 0);
        
        
        CreateCompoundAsset("Diamond", "C5", new System.Collections.Generic.Dictionary<string, int> 
            { {"C", 5} }, CompoundEffect.ExchangeHands, 0);
    }
    
    private void CreateCompoundAsset(string name, string formula, System.Collections.Generic.Dictionary<string, int> requirements, 
                                   CompoundEffect effect, int effectValue)
    {
        CompoundData compound = ScriptableObject.CreateInstance<CompoundData>();
        compound.compoundName = name;
        compound.formula = formula;
        compound.effect = effect;
        compound.effectValue = effectValue;
        compound.isOncePerTurn = true;
        
        
        foreach (var req in requirements)
        {
            ElementData element = FindElementBySymbol(req.Key);
            if (element != null)
            {
                compound.requiredElements.Add(new ElementRequirement(element, req.Value));
            }
            else
            {
                Debug.LogWarning($"Element {req.Key} not found for compound {formula}");
            }
        }
        
        
        System.IO.Directory.CreateDirectory("Assets/ScriptableObjects/Compounds");
        string path = $"Assets/ScriptableObjects/Compounds/{formula}.asset";
        AssetDatabase.CreateAsset(compound, path);
        
        
        chemistryDatabase.allCompounds.Add(compound);
        
        Debug.Log($"✓ Created compound: {formula} ({name})");
    }
    
    private ElementData FindElementBySymbol(string symbol)
    {
        foreach (var element in chemistryDatabase.allElements)
        {
            if (element.symbol == symbol)
                return element;
        }
        return null;
    }
    
    private void AssignElementTextures()
    {
        if (chemistryDatabase == null)
        {
            Debug.LogError("Chemistry Database not assigned!");
            return;
        }
        
        var textureMap = new System.Collections.Generic.Dictionary<string, string>()
        {
            {"H", "Hydrogen"}, {"Na", "Sodium"}, {"K", "Potassium"}, {"Rb", "Rubidium"},
            {"Mg", "Magnesium"}, {"Ca", "Calcium"}, {"Sr", "Strontium"}, {"C", "Carbon"},
            {"O", "Oxygen"}, {"F", "Fluorine"}, {"Cl", "Chlorine"}, {"Br", "Bromine"},
            {"Cu", "Coppa"}, {"Zn", "Zinc"}
        };
        
        foreach (var element in chemistryDatabase.allElements)
        {
            if (textureMap.ContainsKey(element.symbol))
            {
                string texturePath = $"Assets/Cards/{textureMap[element.symbol]}.jpg";
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                
                if (texture != null)
                {
                    
                    TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                    if (importer != null && importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;
                        AssetDatabase.ImportAsset(texturePath);
                    }
                    
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
                    if (sprite != null)
                    {
                        element.cardSprite = sprite;
                        EditorUtility.SetDirty(element);
                        Debug.Log($"✓ Assigned texture to {element.elementName}");
                    }
                }
            }
        }
        AssetDatabase.SaveAssets();
    }
    
    private void CreateChemistryUI()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("Target Canvas not assigned!");
            return;
        }
        
        
        QuickChemistryUISetup uiSetup = FindObjectOfType<QuickChemistryUISetup>();
        if (uiSetup == null)
        {
            GameObject setupObj = new GameObject("ChemistryUISetup");
            uiSetup = setupObj.AddComponent<QuickChemistryUISetup>();
        }
        
        
        var field = typeof(QuickChemistryUISetup).GetField("targetCanvas", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        field.SetValue(uiSetup, targetCanvas);
        
        
        uiSetup.CreateSimpleUI();
        Debug.Log("✓ Chemistry UI created");
    }
    
    private void SetupGameManager()
    {
        
        SimpleChemistryManager manager = FindObjectOfType<SimpleChemistryManager>();
        if (manager == null)
        {
            GameObject managerObj = new GameObject("SimpleChemistryManager");
            manager = managerObj.AddComponent<SimpleChemistryManager>();
        }
        
        
        var managerType = typeof(SimpleChemistryManager);
        var fields = managerType.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        foreach (var field in fields)
        {
            if (field.Name == "chemistryDatabase") 
                field.SetValue(manager, chemistryDatabase);
            else if (field.Name == "elementCardPrefab") 
                field.SetValue(manager, elementCardPrefab);
            else if (field.Name == "compoundCardPrefab") 
                field.SetValue(manager, compoundCardPrefab);
        }
        
        
        GameObject handScrollArea = GameObject.Find("PlayerHandScrollArea");
        GameObject compoundsScrollArea = GameObject.Find("PlayerCompoundsScrollArea");
        GameObject opponentCompoundsScrollArea = GameObject.Find("OpponentCompoundsArea");
        GameObject handPanel = handScrollArea?.transform.Find("Content")?.gameObject;
        GameObject compoundsPanel = compoundsScrollArea?.transform.Find("Content")?.gameObject;
        GameObject opponentCompoundsPanel = opponentCompoundsScrollArea?.transform.Find("Content")?.gameObject;
        GameObject createButton = GameObject.Find("CreateCompoundButton");
        GameObject discardButton = GameObject.Find("DiscardButton");
        Debug.Log($"DiscardButton found: {discardButton != null}");
        GameObject endTurnButton = GameObject.Find("EndTurnButton");
        GameObject cheatButton = GameObject.Find("CheatModeButton");
        GameObject cheatDisplay = GameObject.Find("CheatModeDisplay");
        GameObject statusText = GameObject.Find("StatusText");
        GameObject elementCountText = GameObject.Find("ElementCountText");
        GameObject compoundCountText = GameObject.Find("CompoundCountText");
        GameObject opponentInfo = GameObject.Find("OpponentInfo");
        
        
        foreach (var field in fields)
        {
            if (field.Name == "playerHandParent" && handPanel != null) 
                field.SetValue(manager, handPanel.transform);
            else if (field.Name == "playerCompoundsParent" && compoundsPanel != null) 
                field.SetValue(manager, compoundsPanel.transform);
            else if (field.Name == "opponentCompoundsParent" && opponentCompoundsPanel != null) 
                field.SetValue(manager, opponentCompoundsPanel.transform);
            else if (field.Name == "createCompoundButton" && createButton != null) 
                field.SetValue(manager, createButton.GetComponent<Button>());
            else if (field.Name == "discardButton" && discardButton != null) 
            {
                field.SetValue(manager, discardButton.GetComponent<Button>());
                Debug.Log("✓ Discard button assigned to SimpleChemistryManager");
            }
            else if (field.Name == "endTurnButton" && endTurnButton != null) 
                field.SetValue(manager, endTurnButton.GetComponent<Button>());
            else if (field.Name == "cheatModeButton" && cheatButton != null) 
                field.SetValue(manager, cheatButton.GetComponent<Button>());
            else if (field.Name == "cheatModeDisplay" && cheatDisplay != null) 
                field.SetValue(manager, cheatDisplay);
            else if (field.Name == "statusText" && statusText != null) 
                field.SetValue(manager, statusText.GetComponent<Text>());
            else if (field.Name == "elementCountText" && elementCountText != null) 
                field.SetValue(manager, elementCountText.GetComponent<Text>());
            else if (field.Name == "compoundCountText" && compoundCountText != null) 
                field.SetValue(manager, compoundCountText.GetComponent<Text>());
            else if (field.Name == "opponentInfoText" && opponentInfo != null) 
                field.SetValue(manager, opponentInfo.GetComponent<Text>());
        }
        
        EditorUtility.SetDirty(manager);
        Debug.Log("✓ Game manager configured with all references");
        
        
        if (Application.isPlaying)
        {
            manager.StartChemistryGame();
        }
    }
    
    [ContextMenu("Debug: Test Card Creation")]
    public void TestCardCreation()
    {
        if (elementCardPrefab == null)
        {
            Debug.LogError("Element card prefab not assigned!");
            return;
        }
        
        if (chemistryDatabase == null || chemistryDatabase.allElements.Count == 0)
        {
            Debug.LogError("No elements in database!");
            return;
        }
        
        
        GameObject testCard = Instantiate(elementCardPrefab);
        ElementCard card = testCard.GetComponent<ElementCard>();
        
        if (card != null)
        {
            ElementData testElement = chemistryDatabase.allElements[0];
            card.Initialize(testElement);
            Debug.Log($"✓ Test card created for {testElement.elementName}");
            
            
            testCard.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            Debug.LogError("ElementCard component not found on prefab!");
        }
    }
}
#endif