using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ElementGroup
{
    None,
    AlkaliMetal,    
    AlkaliEarth,    
    Carbon,         
    Oxygen,         
    Halogen,        
    TransitionMetal 
}

[CreateAssetMenu(fileName = "ElementData", menuName = "Chemistry Game/Element Data")]
public class ElementData : ScriptableObject
{
    [Header("Basic Info")]
    public string elementName;
    public string symbol;
    public Sprite cardSprite;
    
    [Header("Chemical Properties")]
    public int oxidationNumber;
    public ElementGroup group;
    
    [Header("Bonding Rules")]
    public bool canFormPositiveIons = false;
    public bool canFormNegativeIons = false;
    
    public override string ToString()
    {
        return $"[{symbol}]";
    }
    
    
    public bool IsMetal()
    {
        return group == ElementGroup.AlkaliMetal || 
               group == ElementGroup.AlkaliEarth || 
               group == ElementGroup.TransitionMetal;
    }
}