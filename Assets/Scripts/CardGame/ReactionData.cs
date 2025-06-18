using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompoundRequirement
{
    public CompoundData compound;
    public int quantity;
    
    public CompoundRequirement(CompoundData comp, int qty)
    {
        compound = comp;
        quantity = qty;
    }
}

[System.Serializable]
public enum ReactionType
{
    Dissociation,
    AcidForming,
    Blast,
    Neutralisation,
    MetalRedox,
    Corrosion,
    CombustionComplete,
    CombustionIncomplete,
    WaterGasShift
}

[CreateAssetMenu(fileName = "ReactionData", menuName = "Chemistry Game/Reaction Data")]
public class ReactionData : ScriptableObject
{
    [Header("Basic Info")]
    public string reactionName;
    public ReactionType reactionType;
    
    [Header("Reactants")]
    public List<CompoundRequirement> requiredCompounds = new List<CompoundRequirement>();
    
    [Header("Products")]
    public List<CompoundData> producedCompounds = new List<CompoundData>();
    public List<ElementData> producedElements = new List<ElementData>();
    
    [Header("Special Effects")]
    public bool banishReactants = true;
    public bool targetOtherPlayers = false;
    public bool forciblyEndTurn = false;
    public bool limitReactions = false;
    public int reactionLimit = 5;
    
    [Header("Effect Description")]
    [TextArea(3, 5)]
    public string effectDescription;
    
    public bool CanPerformReaction(List<CompoundData> availableCompounds)
    {
        Dictionary<CompoundData, int> compoundCounts = new Dictionary<CompoundData, int>();
        
        
        foreach (var compound in availableCompounds)
        {
            if (compoundCounts.ContainsKey(compound))
                compoundCounts[compound]++;
            else
                compoundCounts[compound] = 1;
        }
        
        
        foreach (var requirement in requiredCompounds)
        {
            if (!compoundCounts.ContainsKey(requirement.compound) || 
                compoundCounts[requirement.compound] < requirement.quantity)
            {
                return false;
            }
        }
        
        return true;
    }
}