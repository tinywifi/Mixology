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
    ProtectFromReactions,
    H2O_ChangeElements,     
    CO2_DestroyCompound,    
    Acid_SkipTurn,          
    Base_DiscardThree,      
    Salt_TakeElements,      
    Metallic_NegateDisso,   
    MetalOxide_Protect,     
    MetalHydride_Draw4,     
    Hydrocarbon_DiscardDraw,
    NetworkSolid_Protection 
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
    public bool isMetalHydrideCompound = false;
    public bool isHydrocarbonCompound = false;
    public bool isNetworkSolidCompound = false;
    public bool isMetalOxideCompound = false; 
    
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
    
    public bool CanCreateMetalHydrideFrom(List<ElementData> availableElements)
    {
        if (!isMetalHydrideCompound) return CanCreateFrom(availableElements);
        
        int metalCount = 0;
        int hydrogenCount = 0;
        
        foreach (var element in availableElements)
        {
            if (element.IsMetal() && element.oxidationNumber > 0)
                metalCount++;
            else if (element.symbol == "H")
                hydrogenCount++;
        }
        
        return metalCount >= 1 && hydrogenCount >= 1;
    }
    
    public bool CanCreateHydrocarbonFrom(List<ElementData> availableElements)
    {
        if (!isHydrocarbonCompound) return CanCreateFrom(availableElements);
        
        int carbonCount = 0;
        int hydrogenCount = 0;
        
        foreach (var element in availableElements)
        {
            if (element.symbol == "C")
                carbonCount++;
            else if (element.symbol == "H")
                hydrogenCount++;
        }
        
        return carbonCount >= 1 && hydrogenCount >= 4;
    }
    
    public bool CanCreateNetworkSolidFrom(List<ElementData> availableElements)
    {
        if (!isNetworkSolidCompound) return CanCreateFrom(availableElements);
        
        int carbonCount = 0;
        foreach (var element in availableElements)
        {
            if (element.symbol == "C")
                carbonCount++;
        }
        
        return carbonCount >= 4;
    }
    
    public bool CanCreateMetalOxideFrom(List<ElementData> availableElements)
    {
        if (!isMetalOxideCompound) return CanCreateFrom(availableElements);
        
        int metalCount = 0;
        int oxygenCount = 0;
        
        foreach (var element in availableElements)
        {
            if (element.IsMetal() && element.oxidationNumber > 0)
                metalCount++;
            else if (element.symbol == "O")
                oxygenCount++;
        }
        
        return metalCount >= 1 && oxygenCount >= 1;
    }
}