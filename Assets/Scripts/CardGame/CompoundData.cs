using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ElementRequirement
{
    public ElementData element;
    public int quantity;
    
    public ElementRequirement(ElementData elem, int qty)
    {
        element = elem;
        quantity = qty;
    }
}

[System.Serializable]
public enum CompoundEffect
{
    None,
    DrawElements,           
    SkipPlayerTurn,        
    DiscardElements,       
    ReceiveElements,       
    NegateDissolusion,     
    SwapHands,             
    DiscardDraw,           
    ExchangeHands,         
    ProtectFromReactions   
}

[CreateAssetMenu(fileName = "CompoundData", menuName = "Chemistry Game/Compound Data")]
public class CompoundData : ScriptableObject
{
    [Header("Basic Info")]
    public string compoundName;
    public string formula;
    public Sprite cardSprite;
    
    [Header("Creation Requirements")]
    public List<ElementRequirement> requiredElements = new List<ElementRequirement>();
    
    [Header("Effects")]
    public CompoundEffect effect;
    public int effectValue = 0; 
    public bool isOncePerTurn = true;
    
    [Header("Special Properties")]
    public bool isTemporary = false; 
    public bool protectsFromWaterReactions = false;
    public bool isMetallicCompound = false; 
    
    public override string ToString()
    {
        return $"[{formula}]";
    }
    
    public bool CanCreateFrom(List<ElementData> availableElements)
    {
        Dictionary<ElementData, int> elementCounts = new Dictionary<ElementData, int>();
        
        
        foreach (var element in availableElements)
        {
            if (elementCounts.ContainsKey(element))
                elementCounts[element]++;
            else
                elementCounts[element] = 1;
        }
        
        
        foreach (var requirement in requiredElements)
        {
            if (!elementCounts.ContainsKey(requirement.element) || 
                elementCounts[requirement.element] < requirement.quantity)
            {
                return false;
            }
        }
        
        return true;
    }
    
    
    public bool CanCreateMetallicFrom(List<ElementData> availableElements)
    {
        if (!isMetallicCompound) return CanCreateFrom(availableElements);
        
        
        Dictionary<int, List<ElementData>> metalElementsByCharge = new Dictionary<int, List<ElementData>>();
        
        foreach (var element in availableElements)
        {
            
            if (element.oxidationNumber > 0 && element.IsMetal())
            {
                if (!metalElementsByCharge.ContainsKey(element.oxidationNumber))
                    metalElementsByCharge[element.oxidationNumber] = new List<ElementData>();
                metalElementsByCharge[element.oxidationNumber].Add(element);
            }
        }
        
        
        foreach (var chargeGroup in metalElementsByCharge)
        {
            if (chargeGroup.Value.Count >= 3)
            {
                return true; 
            }
        }
        
        return false;
    }
}