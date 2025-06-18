using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CompoundCreationHelper : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject compoundPreviewPanel;
    [SerializeField] private Text previewFormulaText;
    [SerializeField] private Text previewNameText;
    [SerializeField] private Text previewEffectText;
    [SerializeField] private Button confirmCreationButton;
    [SerializeField] private Button cancelCreationButton;
    [SerializeField] private Transform selectedElementsParent;
    [SerializeField] private GameObject selectedElementUIPrefab;
    
    [Header("Chemistry Database")]
    [SerializeField] private ChemistryDatabase chemistryDatabase;
    
    private List<ElementData> selectedElements = new List<ElementData>();
    private CompoundData previewedCompound = null;
    
    public System.Action<CompoundData, List<ElementData>> OnCompoundConfirmed;
    
    void Start()
    {
        
        if (compoundPreviewPanel)
            compoundPreviewPanel.SetActive(false);
            
        if (confirmCreationButton)
            confirmCreationButton.onClick.AddListener(ConfirmCreation);
            
        if (cancelCreationButton)
            cancelCreationButton.onClick.AddListener(CancelCreation);
    }
    
    public void ShowCompoundCreation(List<ElementData> elements)
    {
        selectedElements = new List<ElementData>(elements);
        
        
        foreach (Transform child in selectedElementsParent)
            Destroy(child.gameObject);
        
        
        foreach (var element in selectedElements)
        {
            GameObject elementUI = Instantiate(selectedElementUIPrefab, selectedElementsParent);
            
            Text elementText = elementUI.GetComponentInChildren<Text>();
            if (elementText)
                elementText.text = element.symbol;
        }
        
        
        CompoundData validCompound = FindValidCompound(selectedElements);
        
        if (validCompound != null)
        {
            previewedCompound = validCompound;
            UpdatePreviewUI(validCompound);
            confirmCreationButton.interactable = true;
        }
        else
        {
            ShowInvalidCombination();
            confirmCreationButton.interactable = false;
        }
        
        compoundPreviewPanel.SetActive(true);
    }
    
    private CompoundData FindValidCompound(List<ElementData> elements)
    {
        if (chemistryDatabase == null) return null;
        
        foreach (var compound in chemistryDatabase.allCompounds)
        {
            if (IsExactMatch(compound, elements))
            {
                return compound;
            }
        }
        
        return null;
    }
    
    private bool IsExactMatch(CompoundData compound, List<ElementData> elements)
    {
        
        Dictionary<ElementData, int> selectedCounts = new Dictionary<ElementData, int>();
        foreach (var element in elements)
        {
            if (selectedCounts.ContainsKey(element))
                selectedCounts[element]++;
            else
                selectedCounts[element] = 1;
        }
        
        
        Dictionary<ElementData, int> requiredCounts = new Dictionary<ElementData, int>();
        foreach (var requirement in compound.requiredElements)
        {
            requiredCounts[requirement.element] = requirement.quantity;
        }
        
        
        if (selectedCounts.Count != requiredCounts.Count)
            return false;
            
        foreach (var kvp in requiredCounts)
        {
            if (!selectedCounts.ContainsKey(kvp.Key) || selectedCounts[kvp.Key] != kvp.Value)
                return false;
        }
        
        return true;
    }
    
    private void UpdatePreviewUI(CompoundData compound)
    {
        if (previewFormulaText)
            previewFormulaText.text = compound.formula;
            
        if (previewNameText)
            previewNameText.text = compound.compoundName;
            
        if (previewEffectText)
        {
            string effectDescription = GetEffectDescription(compound);
            previewEffectText.text = effectDescription;
        }
    }
    
    private void ShowInvalidCombination()
    {
        if (previewFormulaText)
            previewFormulaText.text = "Invalid Combination";
            
        if (previewNameText)
            previewNameText.text = "Cannot form compound";
            
        if (previewEffectText)
        {
            string reason = GetInvalidReason();
            previewEffectText.text = reason;
        }
    }
    
    private string GetInvalidReason()
    {
        
        int totalOxidation = 0;
        foreach (var element in selectedElements)
        {
            totalOxidation += element.oxidationNumber;
        }
        
        if (totalOxidation != 0)
        {
            return $"Oxidation numbers don't balance (sum = {totalOxidation}, should be 0)";
        }
        
        return "No known compound matches this combination";
    }
    
    private string GetEffectDescription(CompoundData compound)
    {
        switch (compound.effect)
        {
            case CompoundEffect.DrawElements:
                return $"Once per turn: Draw {compound.effectValue} elements";
            case CompoundEffect.SkipPlayerTurn:
                return "Once per turn: Skip next player's turn";
            case CompoundEffect.DiscardElements:
                return $"Once per turn: Target player discards {compound.effectValue} elements";
            case CompoundEffect.ReceiveElements:
                return $"Once per turn: Receive {compound.effectValue} elements";
            case CompoundEffect.NegateDissolusion:
                return "If targeted by Dissolution, negate it and receive 2 elements";
            case CompoundEffect.SwapHands:
                return "Once per turn: Swap all hand elements with another player";
            case CompoundEffect.DiscardDraw:
                return "Once per turn: Discard any elements, draw same amount";
            case CompoundEffect.ExchangeHands:
                return "Once per turn: Exchange hands with player to your right";
            case CompoundEffect.ProtectFromReactions:
                return "Nullify all H2O-related reactions targeting you";
            default:
                return "No special effect";
        }
    }
    
    private void ConfirmCreation()
    {
        if (previewedCompound != null && selectedElements.Count > 0)
        {
            OnCompoundConfirmed?.Invoke(previewedCompound, selectedElements);
        }
        
        HidePanel();
    }
    
    private void CancelCreation()
    {
        HidePanel();
    }
    
    private void HidePanel()
    {
        if (compoundPreviewPanel)
            compoundPreviewPanel.SetActive(false);
            
        selectedElements.Clear();
        previewedCompound = null;
    }
    
    
    public bool CanFormAnyCompound(List<ElementData> elements)
    {
        if (chemistryDatabase == null) return false;
        
        foreach (var compound in chemistryDatabase.allCompounds)
        {
            if (compound.CanCreateFrom(elements))
                return true;
        }
        
        return false;
    }
    
    
    public List<CompoundData> GetPossibleCompounds(List<ElementData> elements)
    {
        List<CompoundData> possible = new List<CompoundData>();
        
        if (chemistryDatabase == null) return possible;
        
        foreach (var compound in chemistryDatabase.allCompounds)
        {
            if (compound.CanCreateFrom(elements))
                possible.Add(compound);
        }
        
        return possible;
    }
}