using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class ChemistryGameDataSetup : MonoBehaviour
{
    [Header("References")]
    public ChemistryDatabase chemistryDatabase;
    
    [ContextMenu("Setup Complete Chemistry Game Data")]
    public void SetupCompleteGameData()
    {
        if (chemistryDatabase == null)
        {
            Debug.LogError("Chemistry Database not assigned!");
            return;
        }
        
        Debug.Log("=== Setting up Complete Chemistry Game Data ===");
        
        
        chemistryDatabase.allCompounds.Clear();
        chemistryDatabase.allReactions.Clear();
        
        
        CreateCompounds();
        
        
        CreateReactions();
        
        EditorUtility.SetDirty(chemistryDatabase);
        AssetDatabase.SaveAssets();
        
        Debug.Log("=== Chemistry Game Data Setup Complete ===");
    }
    
    private void CreateCompounds()
    {
        
        CreateCompound("Water", "H2O", new Dictionary<string, int> { {"H", 2}, {"O", 1} }, 
                      CompoundEffect.None, 0);
        
        
        CreateCompound("Carbon Dioxide", "CO2", new Dictionary<string, int> { {"C", 1}, {"O", 2} }, 
                      CompoundEffect.DrawElements, 4);
        
        
        CreateCompound("Acid", "HCl", new Dictionary<string, int> { {"H", 1}, {"Cl", 1} }, 
                      CompoundEffect.SkipPlayerTurn, 0);
        
        
        CreateCompound("Hydrofluoric Acid", "HF", new Dictionary<string, int> { {"H", 1}, {"F", 1} }, 
                      CompoundEffect.SkipPlayerTurn, 0);
        
        CreateCompound("Hydrobromic Acid", "HBr", new Dictionary<string, int> { {"H", 1}, {"Br", 1} }, 
                      CompoundEffect.SkipPlayerTurn, 0);
        
        
        CreateCompound("Sodium Hydroxide", "NaOH", new Dictionary<string, int> { {"Na", 1}, {"O", 1}, {"H", 1} }, 
                      CompoundEffect.DiscardElements, 3);
        
        CreateCompound("Potassium Hydroxide", "KOH", new Dictionary<string, int> { {"K", 1}, {"O", 1}, {"H", 1} }, 
                      CompoundEffect.DiscardElements, 3);
        
        
        CreateCompound("Calcium Hydroxide", "Ca(OH)2", new Dictionary<string, int> { {"Ca", 1}, {"O", 2}, {"H", 2} }, 
                      CompoundEffect.DiscardElements, 3);
        
        
        CreateCompound("Sodium Chloride", "NaCl", new Dictionary<string, int> { {"Na", 1}, {"Cl", 1} }, 
                      CompoundEffect.ReceiveElements, 4);
        
        CreateCompound("Potassium Fluoride", "KF", new Dictionary<string, int> { {"K", 1}, {"F", 1} }, 
                      CompoundEffect.ReceiveElements, 4);
        
        
        CreateCompound("Copper Metal", "Cu3", new Dictionary<string, int> { {"Cu", 3} }, 
                      CompoundEffect.NegateDissolusion, 0);
        
        CreateCompound("Zinc Metal", "Zn3", new Dictionary<string, int> { {"Zn", 3} }, 
                      CompoundEffect.NegateDissolusion, 0);
        
        
        CreateCompound("Sodium Hydride", "NaH2", new Dictionary<string, int> { {"Na", 2}, {"H", 2} }, 
                      CompoundEffect.SwapHands, 0);
        
        
        CreateCompound("Methane", "CH4", new Dictionary<string, int> { {"C", 1}, {"H", 4} }, 
                      CompoundEffect.DiscardDraw, 0);
        
        
        CreateCompound("Diamond", "C5", new Dictionary<string, int> { {"C", 5} }, 
                      CompoundEffect.ExchangeHands, 0);
        
        Debug.Log($"Created {chemistryDatabase.allCompounds.Count} compounds");
    }
    
    private void CreateCompound(string name, string formula, Dictionary<string, int> requirements, 
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
        
        
        string path = $"Assets/ScriptableObjects/Compounds/{formula}.asset";
        AssetDatabase.CreateAsset(compound, path);
        
        
        chemistryDatabase.allCompounds.Add(compound);
        
        Debug.Log($"✓ Created compound: {formula} ({name})");
    }
    
    private void CreateReactions()
    {
        
        
        Debug.Log("Reaction system ready for implementation");
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
    
    [ContextMenu("Debug: List All Data")]
    public void DebugListAllData()
    {
        if (chemistryDatabase == null) return;
        
        Debug.Log("=== ELEMENTS ===");
        foreach (var element in chemistryDatabase.allElements)
        {
            Debug.Log($"{element.symbol} - {element.elementName} (Oxidation: {element.oxidationNumber})");
        }
        
        Debug.Log("=== COMPOUNDS ===");
        foreach (var compound in chemistryDatabase.allCompounds)
        {
            string requirements = "";
            foreach (var req in compound.requiredElements)
            {
                requirements += $"{req.quantity}x{req.element.symbol} ";
            }
            Debug.Log($"{compound.formula} - {compound.compoundName} | Requires: {requirements}| Effect: {compound.effect}");
        }
    }
}
#endif