using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CompoundCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Text formulaText;
    [SerializeField] private Text nameText;
    [SerializeField] private Text effectText;
    [SerializeField] private GameObject selectionHighlight;
    [SerializeField] private GameObject usedIndicator;
    
    private CompoundData compoundData;
    private bool isSelected = false;
    private bool isInteractable = true;
    private bool hasBeenUsedThisTurn = false;
    
    public CompoundData Data => compoundData;
    public bool IsSelected => isSelected;
    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
    public bool HasBeenUsedThisTurn { get => hasBeenUsedThisTurn; set => hasBeenUsedThisTurn = value; }
    
    public System.Action<CompoundCard> OnCardClicked;
    public System.Action<CompoundCard> OnCardHovered;
    public System.Action<CompoundCard> OnCardUnhovered;
    
    public void Initialize(CompoundData data)
    {
        compoundData = data;
        UpdateCardDisplay();
    }
    
    private void UpdateCardDisplay()
    {
        if (compoundData == null) return;
        
        if (cardImage && compoundData.cardSprite)
            cardImage.sprite = compoundData.cardSprite;
            
        if (formulaText)
            formulaText.text = compoundData.formula;
            
        if (nameText)
            nameText.text = compoundData.compoundName;
            
        if (effectText)
        {
            string effectDescription = GetEffectDescription();
            effectText.text = effectDescription;
        }
        
        UpdateUsedIndicator();
    }
    
    private string GetEffectDescription()
    {
        switch (compoundData.effect)
        {
            case CompoundEffect.DrawElements:
                return $"Draw {compoundData.effectValue} elements";
            case CompoundEffect.SkipPlayerTurn:
                return "Skip next player's turn";
            case CompoundEffect.DiscardElements:
                return $"Target player discards {compoundData.effectValue} elements";
            case CompoundEffect.ReceiveElements:
                return $"Receive {compoundData.effectValue} elements";
            case CompoundEffect.NegateDissolusion:
                return "Negates Dissolution + receive 2 elements";
            case CompoundEffect.SwapHands:
                return "Swap all hand elements";
            case CompoundEffect.DiscardDraw:
                return "Discard any elements, draw same amount";
            case CompoundEffect.ExchangeHands:
                return "Exchange hands with player to right";
            case CompoundEffect.ProtectFromReactions:
                return "Nullify H2O reactions targeting you";
            default:
                return "";
        }
    }
    
    private void UpdateUsedIndicator()
    {
        if (usedIndicator)
        {
            bool shouldShow = hasBeenUsedThisTurn && compoundData.isOncePerTurn;
            usedIndicator.SetActive(shouldShow);
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selectionHighlight)
            selectionHighlight.SetActive(selected);
    }
    
    public void ResetTurnUsage()
    {
        hasBeenUsedThisTurn = false;
        UpdateUsedIndicator();
    }
    
    public bool CanUseThisTurn()
    {
        if (!compoundData.isOncePerTurn) return true;
        return !hasBeenUsedThisTurn;
    }
    
    public void MarkAsUsed()
    {
        if (compoundData.isOncePerTurn)
        {
            hasBeenUsedThisTurn = true;
            UpdateUsedIndicator();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        SetSelected(!isSelected);
        OnCardClicked?.Invoke(this);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        OnCardHovered?.Invoke(this);
        
        transform.localScale = Vector3.one * 1.1f;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        OnCardUnhovered?.Invoke(this);
        
        transform.localScale = Vector3.one;
    }
    
    public void PlayMoveAnimation(Vector3 targetPosition, System.Action onComplete = null)
    {
        StartCoroutine(MoveToPosition(targetPosition, onComplete));
    }
    
    private IEnumerator MoveToPosition(Vector3 targetPosition, System.Action onComplete)
    {
        Vector3 startPosition = transform.position;
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        transform.position = targetPosition;
        onComplete?.Invoke();
    }
}