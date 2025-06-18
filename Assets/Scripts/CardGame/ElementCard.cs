using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ElementCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Text symbolText;
    [SerializeField] private Text nameText;
    [SerializeField] private Text oxidationText;
    [SerializeField] private GameObject selectionHighlight;
    
    private ElementData elementData;
    private bool isSelected = false;
    private bool isInteractable = true;
    
    public ElementData Data => elementData;
    public bool IsSelected => isSelected;
    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
    
    public System.Action<ElementCard> OnCardClicked;
    public System.Action<ElementCard> OnCardHovered;
    public System.Action<ElementCard> OnCardUnhovered;
    
    public void Initialize(ElementData data)
    {
        elementData = data;
        UpdateCardDisplay();
    }
    
    private void UpdateCardDisplay()
    {
        if (elementData == null) return;
        
        if (cardImage && elementData.cardSprite)
            cardImage.sprite = elementData.cardSprite;
            
        
        if (symbolText)
            symbolText.gameObject.SetActive(false);
            
        if (nameText)
            nameText.gameObject.SetActive(false);
            
        if (oxidationText)
            oxidationText.gameObject.SetActive(false);
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selectionHighlight)
            selectionHighlight.SetActive(selected);
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