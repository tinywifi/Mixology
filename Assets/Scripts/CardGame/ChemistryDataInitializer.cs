using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[System.Serializable]
public class ElementTemplate
{
    public string name;
    public string symbol;
    public int oxidationNumber;
    public ElementGroup group;
    public string texturePath;
}

public class ChemistryDataInitializer : MonoBehaviour
{
    [Header("Database Reference")]
    public ChemistryDatabase database;
    
    [Header("Asset Paths")]
    public string elementDataPath = "Assets/ScriptableObjects/Elements/";
    public string compoundDataPath = "Assets/ScriptableObjects/Compounds/";
    public string reactionDataPath = "Assets/ScriptableObjects/Reactions/";
    
    
    private ElementTemplate[] elementTemplates = new ElementTemplate[]
    {
        new ElementTemplate { name = "Hydrogen", symbol = "H", oxidationNumber = 1, group = ElementGroup.None, texturePath = "Hydrogen.jpg" },
        new ElementTemplate { name = "Sodium", symbol = "Na", oxidationNumber = 1, group = ElementGroup.AlkaliMetal, texturePath = "Sodium.jpg" },
        new ElementTemplate { name = "Potassium", symbol = "K", oxidationNumber = 1, group = ElementGroup.AlkaliMetal, texturePath = "Potassium.jpg" },
        new ElementTemplate { name = "Rubidium", symbol = "Rb", oxidationNumber = 1, group = ElementGroup.AlkaliMetal, texturePath = "Rubidium.jpg" },
        new ElementTemplate { name = "Magnesium", symbol = "Mg", oxidationNumber = 2, group = ElementGroup.AlkaliEarth, texturePath = "Magnesium.jpg" },
        new ElementTemplate { name = "Calcium", symbol = "Ca", oxidationNumber = 2, group = ElementGroup.AlkaliEarth, texturePath = "Calcium.jpg" },
        new ElementTemplate { name = "Strontium", symbol = "Sr", oxidationNumber = 2, group = ElementGroup.AlkaliEarth, texturePath = "Strontium.jpg" },
        new ElementTemplate { name = "Carbon", symbol = "C", oxidationNumber = 4, group = ElementGroup.Carbon, texturePath = "Carbon.jpg" },
        new ElementTemplate { name = "Oxygen", symbol = "O", oxidationNumber = -2, group = ElementGroup.Oxygen, texturePath = "Oxygen.jpg" },
        new ElementTemplate { name = "Fluorine", symbol = "F", oxidationNumber = -1, group = ElementGroup.Halogen, texturePath = "Fluorine.jpg" },
        new ElementTemplate { name = "Chlorine", symbol = "Cl", oxidationNumber = -1, group = ElementGroup.Halogen, texturePath = "Chlorine.jpg" },
        new ElementTemplate { name = "Bromine", symbol = "Br", oxidationNumber = -1, group = ElementGroup.Halogen, texturePath = "Bromine.jpg" },
        new ElementTemplate { name = "Copper", symbol = "Cu", oxidationNumber = 2, group = ElementGroup.TransitionMetal, texturePath = "Coppa.jpg" },
        new ElementTemplate { name = "Zinc", symbol = "Zn", oxidationNumber = 2, group = ElementGroup.TransitionMetal, texturePath = "Zinc.jpg" }
    };
    
    [ContextMenu("Initialize Chemistry Database")]
    public void InitializeDatabase()
    {
        if (database == null)
        {
            Debug.LogError("Database reference not set!");
            return;
        }
        
        CreateElementDataAssets();
        CreateCompoundDataAssets();
        CreateReactionDataAssets();
        
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("Chemistry database initialized successfully!");
    }
    
    private void CreateElementDataAssets()
    {
        database.allElements.Clear();
        
        
        if (!AssetDatabase.IsValidFolder(elementDataPath.TrimEnd('/')))
        {
            string[] pathParts = elementDataPath.TrimEnd('/').Split('/');
            string currentPath = pathParts[0];
            
            for (int i = 1; i < pathParts.Length; i++)
            {
                string newPath = currentPath + "/" + pathParts[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                }
                currentPath = newPath;
            }
        }
        
        foreach (var template in elementTemplates)
        {
            ElementData elementData = ScriptableObject.CreateInstance<ElementData>();
            
            elementData.elementName = template.name;
            elementData.symbol = template.symbol;
            elementData.oxidationNumber = template.oxidationNumber;
            elementData.group = template.group;
            
            
            string spritePath = $"Assets/Cards/{template.texturePath}";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite == null)
            {
                
                spritePath = $"Assets/Cards/{template.texturePath.Replace(".jpg", "")}";
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            }
            elementData.cardSprite = sprite;
            
            
            elementData.canFormPositiveIons = template.oxidationNumber > 0;
            elementData.canFormNegativeIons = template.oxidationNumber < 0;
            
            string assetPath = elementDataPath + template.symbol + ".asset";
            AssetDatabase.CreateAsset(elementData, assetPath);
            
            database.allElements.Add(elementData);
        }
    }
    
    private void CreateCompoundDataAssets()
    {
        database.allCompounds.Clear();
        
        
        if (!AssetDatabase.IsValidFolder(compoundDataPath.TrimEnd('/')))
        {
            string[] pathParts = compoundDataPath.TrimEnd('/').Split('/');
            string currentPath = pathParts[0];
            
            for (int i = 1; i < pathParts.Length; i++)
            {
                string newPath = currentPath + "/" + pathParts[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                }
                currentPath = newPath;
            }
        }
        
        
        CreateCompound("Water", "H2O", CompoundEffect.None, 0, 
            new[] { ("H", 2), ("O", 1) });
            
        CreateCompound("Carbon Dioxide", "CO2", CompoundEffect.DrawElements, 4,
            new[] { ("C", 1), ("O", 2) });
            
        CreateCompound("Acid", "Acid", CompoundEffect.SkipPlayerTurn, 0,
            new[] { ("H", 1), ("F", 1) }); 
            
        CreateCompound("Base", "Base", CompoundEffect.DiscardElements, 3,
            new[] { ("Na", 1), ("O", 1), ("H", 1) }); 
            
        CreateCompound("Salt", "Salt", CompoundEffect.ReceiveElements, 4,
            new[] { ("Na", 1), ("Cl", 1) }); 
            
        
        CreateUniversalMetallicCompound("Metallic", "Metal3", CompoundEffect.NegateDissolusion, 2);
            
        CreateCompound("Metal Hydride", "Metal Hydride", CompoundEffect.SwapHands, 0,
            new[] { ("Na", 2), ("H", 2) }); 
            
        CreateCompound("Hydrocarbon", "CH4", CompoundEffect.DiscardDraw, 0,
            new[] { ("C", 1), ("H", 4) }); 
            
        CreateCompound("Network Solid", "Network Solid", CompoundEffect.ExchangeHands, 0,
            new[] { ("C", 5) }); 
    }
    
    private void CreateCompound(string name, string formula, CompoundEffect effect, int effectValue, (string symbol, int quantity)[] requirements)
    {
        CompoundData compound = ScriptableObject.CreateInstance<CompoundData>();
        
        compound.compoundName = name;
        compound.formula = formula;
        compound.effect = effect;
        compound.effectValue = effectValue;
        compound.isOncePerTurn = true;
        
        
        foreach (var req in requirements)
        {
            ElementData element = database.allElements.Find(e => e.symbol == req.symbol);
            if (element != null)
            {
                compound.requiredElements.Add(new ElementRequirement(element, req.quantity));
            }
        }
        
        string assetPath = compoundDataPath + formula.Replace("/", "_").Replace("-", "_") + ".asset";
        AssetDatabase.CreateAsset(compound, assetPath);
        
        database.allCompounds.Add(compound);
    }
    
    private void CreateMetallicCompound(string name, string formula, CompoundEffect effect, int effectValue, (string symbol, int quantity)[] requirements)
    {
        CompoundData compound = ScriptableObject.CreateInstance<CompoundData>();
        
        compound.compoundName = name;
        compound.formula = formula;
        compound.effect = effect;
        compound.effectValue = effectValue;
        compound.isOncePerTurn = true;
        
        
        compound.isTemporary = false;
        compound.protectsFromWaterReactions = true; 
        
        
        foreach (var req in requirements)
        {
            ElementData element = database.allElements.Find(e => e.symbol == req.symbol);
            if (element != null)
            {
                compound.requiredElements.Add(new ElementRequirement(element, req.quantity));
            }
        }
        
        string assetPath = compoundDataPath + "Metallic_" + formula.Replace("-", "_") + ".asset";
        AssetDatabase.CreateAsset(compound, assetPath);
        
        database.allCompounds.Add(compound);
    }
    
    private void CreateUniversalMetallicCompound(string name, string formula, CompoundEffect effect, int effectValue)
    {
        CompoundData compound = ScriptableObject.CreateInstance<CompoundData>();
        
        compound.compoundName = name;
        compound.formula = formula;
        compound.effect = effect;
        compound.effectValue = effectValue;
        compound.isOncePerTurn = true;
        compound.isMetallicCompound = true; 
        compound.protectsFromWaterReactions = true;
        
        
        
        
        string assetPath = compoundDataPath + "Universal_" + formula + ".asset";
        AssetDatabase.CreateAsset(compound, assetPath);
        
        database.allCompounds.Add(compound);
    }
    
    private void CreateReactionDataAssets()
    {
        database.allReactions.Clear();
        
        
        if (!AssetDatabase.IsValidFolder(reactionDataPath.TrimEnd('/')))
        {
            string[] pathParts = reactionDataPath.TrimEnd('/').Split('/');
            string currentPath = pathParts[0];
            
            for (int i = 1; i < pathParts.Length; i++)
            {
                string newPath = currentPath + "/" + pathParts[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                }
                currentPath = newPath;
            }
        }
        
        
        CreateReaction("Dissociation", ReactionType.Dissociation, 
            new[] { ("H2O", 1), ("H+X-", 1) }, 
            new string[] { }, 
            new string[] { },
            "Banish all related reactants");
            
        CreateReaction("Acid Forming", ReactionType.AcidForming,
            new[] { ("CO2", 1), ("H2O", 1) },
            new[] { "H+X-" }, 
            new string[] { },
            "Yield Acid, target player skips next turn");
            
        CreateReaction("Neutralisation", ReactionType.Neutralisation,
            new[] { ("H+X-", 1), ("Na+OH-", 1) }, 
            new[] { "NaCl" }, 
            new string[] { },
            "Yield Salt, target up to 2 opponent compounds for dissolution");
            
        CreateReaction("Combustion Complete", ReactionType.CombustionComplete,
            new[] { ("CH4", 1) }, 
            new[] { "CO2", "H2O" },
            new string[] { },
            "Yield CO2 and H2O, forcibly end turn and start next turn immediately");
    }
    
    private void CreateReaction(string name, ReactionType type, (string formula, int quantity)[] reactants, 
                              string[] products, string[] elementProducts, string description)
    {
        ReactionData reaction = ScriptableObject.CreateInstance<ReactionData>();
        
        reaction.reactionName = name;
        reaction.reactionType = type;
        reaction.effectDescription = description;
        reaction.banishReactants = true;
        
        
        foreach (var reactant in reactants)
        {
            CompoundData compound = database.allCompounds.Find(c => c.formula == reactant.formula);
            if (compound != null)
            {
                reaction.requiredCompounds.Add(new CompoundRequirement(compound, reactant.quantity));
            }
        }
        
        
        foreach (var product in products)
        {
            CompoundData compound = database.allCompounds.Find(c => c.formula == product);
            if (compound != null)
            {
                reaction.producedCompounds.Add(compound);
            }
        }
        
        
        foreach (var elementSymbol in elementProducts)
        {
            ElementData element = database.allElements.Find(e => e.symbol == elementSymbol);
            if (element != null)
            {
                reaction.producedElements.Add(element);
            }
        }
        
        
        switch (type)
        {
            case ReactionType.CombustionComplete:
                reaction.forciblyEndTurn = true;
                break;
            case ReactionType.Corrosion:
                reaction.limitReactions = true;
                reaction.reactionLimit = 5;
                break;
        }
        
        string assetPath = reactionDataPath + name.Replace(" ", "_") + ".asset";
        AssetDatabase.CreateAsset(reaction, assetPath);
        
        database.allReactions.Add(reaction);
    }
}
#endif