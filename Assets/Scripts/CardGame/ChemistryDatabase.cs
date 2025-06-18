using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChemistryDatabase", menuName = "Chemistry Game/Chemistry Database")]
public class ChemistryDatabase : ScriptableObject
{
    [Header("Elements")]
    public List<ElementData> allElements = new List<ElementData>();
    
    [Header("Compounds")]
    public List<CompoundData> allCompounds = new List<CompoundData>();
    
    [Header("Reactions")]
    public List<ReactionData> allReactions = new List<ReactionData>();
    
    public ElementData GetElementBySymbol(string symbol)
    {
        return allElements.Find(e => e.symbol == symbol);
    }
    
    public CompoundData GetCompoundByFormula(string formula)
    {
        return allCompounds.Find(c => c.formula == formula);
    }
    
    public List<CompoundData> GetValidCompounds(List<ElementData> availableElements)
    {
        List<CompoundData> validCompounds = new List<CompoundData>();
        
        foreach (var compound in allCompounds)
        {
            if (compound.CanCreateFrom(availableElements))
            {
                validCompounds.Add(compound);
            }
        }
        
        return validCompounds;
    }
    
    public List<ReactionData> GetValidReactions(List<CompoundData> availableCompounds)
    {
        List<ReactionData> validReactions = new List<ReactionData>();
        
        foreach (var reaction in allReactions)
        {
            if (reaction.CanPerformReaction(availableCompounds))
            {
                validReactions.Add(reaction);
            }
        }
        
        return validReactions;
    }
    
    
    public bool IsValidCompound(List<ElementData> elements, List<int> quantities)
    {
        if (elements.Count != quantities.Count) return false;
        
        int totalOxidation = 0;
        
        for (int i = 0; i < elements.Count; i++)
        {
            totalOxidation += elements[i].oxidationNumber * quantities[i];
        }
        
        return totalOxidation == 0;
    }
}