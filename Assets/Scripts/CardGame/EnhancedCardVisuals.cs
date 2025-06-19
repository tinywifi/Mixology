using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

// Enhanced Element Card with modern styling
public class EnhancedElementCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Card Data")]
    public ElementData elementData;
    
    [Header("Visual Components")]
    public Image cardBackground;
    public Image elementIcon;
    public Text elementSymbol;
    public Text elementName;
    public Text oxidationNumber;
    public Image selectionGlow;
    public Image cardBorder;
    
    [Header("Animation Settings")]
    public float hoverScale = 1.1f;
    public float selectScale = 1.15f;
    public float animationDuration = 0.2f;
    
    private bool isSelected = false;
    private bool isHovered = false;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private ModernUITheme theme;
    private System.Action<EnhancedElementCard> onCardClicked;
    
    void Awake()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
        theme = ModernUITheme.Instance;
    }
    
    public void Initialize(ElementData data, System.Action<EnhancedElementCard> clickCallback = null)
    {
        elementData = data;
        onCardClicked = clickCallback;
        SetupVisuals();
    }
    
    void SetupVisuals()
    {
        if (theme == null || elementData == null) return;
        
        // Setup card background with element-specific colors
        Color elementColor = GetElementColor(elementData.symbol);
        Color cardColor = new Color(elementColor.r * 0.3f, elementColor.g * 0.3f, elementColor.b * 0.3f, 1f);
        
        if (cardBackground != null)
        {
            cardBackground.color = cardColor;
            cardBackground.sprite = theme.CreateRoundedRectSprite(150, 200, 16, cardColor);
        }
        
        // Setup element icon
        if (elementIcon != null && elementData.cardSprite != null)
        {
            elementIcon.sprite = elementData.cardSprite;
            elementIcon.color = elementColor;
        }
        
        // Setup element symbol
        if (elementSymbol != null)
        {
            elementSymbol.text = elementData.symbol;
            elementSymbol.color = Color.white;
            elementSymbol.fontSize = 48;
            elementSymbol.fontStyle = FontStyle.Bold;
            
            // Add outline for better visibility
            Outline symbolOutline = elementSymbol.gameObject.GetComponent<Outline>();
            if (symbolOutline == null)
                symbolOutline = elementSymbol.gameObject.AddComponent<Outline>();
            symbolOutline.effectColor = Color.black;
            symbolOutline.effectDistance = new Vector2(2f, -2f);
        }
        
        // Setup element name
        if (elementName != null)
        {
            elementName.text = elementData.elementName;
            elementName.color = theme.colorPalette.textPrimary;
            elementName.fontSize = 16;
        }
        
        // Setup oxidation number
        if (oxidationNumber != null)
        {
            string oxidationText = elementData.oxidationNumber >= 0 ? $"+{elementData.oxidationNumber}" : elementData.oxidationNumber.ToString();
            oxidationNumber.text = oxidationText;
            oxidationNumber.color = elementData.oxidationNumber >= 0 ? theme.colorPalette.successColor : theme.colorPalette.errorColor;
            oxidationNumber.fontSize = 20;
            oxidationNumber.fontStyle = FontStyle.Bold;
        }
        
        // Setup selection glow (initially hidden)
        if (selectionGlow != null)
        {
            selectionGlow.color = new Color(theme.colorPalette.primaryColor.r, theme.colorPalette.primaryColor.g, theme.colorPalette.primaryColor.b, 0f);
            selectionGlow.sprite = theme.CreateRoundedRectSprite(160, 210, 20, Color.white);
        }
        
        // Setup card border
        if (cardBorder != null)
        {
            cardBorder.color = elementColor;
            cardBorder.sprite = theme.CreateRoundedRectSprite(155, 205, 18, Color.white);
        }
        
        // Add shadow effect
        AddShadowEffect();
    }
    
    void AddShadowEffect()
    {
        if (!theme.enableShadows) return;
        
        GameObject shadowObj = new GameObject("CardShadow");
        shadowObj.transform.SetParent(transform);
        shadowObj.transform.SetAsFirstSibling();
        
        RectTransform shadowRect = shadowObj.AddComponent<RectTransform>();
        Image shadowImage = shadowObj.AddComponent<Image>();
        
        // Position shadow
        RectTransform cardRect = GetComponent<RectTransform>();
        shadowRect.anchorMin = cardRect.anchorMin;
        shadowRect.anchorMax = cardRect.anchorMax;
        shadowRect.sizeDelta = cardRect.sizeDelta;
        shadowRect.anchoredPosition = cardRect.anchoredPosition + theme.shadowOffset;
        shadowRect.localScale = Vector3.one;
        
        // Style shadow
        shadowImage.sprite = theme.CreateRoundedRectSprite(150, 200, 16, theme.shadowColor);
        shadowImage.raycastTarget = false;
    }
    
    Color GetElementColor(string symbol)
    {
        // Color coding by element type
        switch (symbol)
        {
            // Alkali metals
            case "H": case "Li": case "Na": case "K": case "Rb": case "Cs":
                return new Color(1f, 0.3f, 0.3f, 1f); // Red
            
            // Alkaline earth metals
            case "Be": case "Mg": case "Ca": case "Sr": case "Ba":
                return new Color(1f, 0.6f, 0.2f, 1f); // Orange
            
            // Transition metals
            case "Cu": case "Zn": case "Fe": case "Ni":
                return new Color(0.8f, 0.8f, 0.2f, 1f); // Gold
            
            // Halogens
            case "F": case "Cl": case "Br": case "I":
                return new Color(0.3f, 1f, 0.3f, 1f); // Green
            
            // Noble gases
            case "He": case "Ne": case "Ar": case "Kr":
                return new Color(0.6f, 0.3f, 1f, 1f); // Purple
            
            // Nonmetals
            case "C": case "N": case "O": case "P": case "S":
                return new Color(0.3f, 0.6f, 1f, 1f); // Blue
            
            default:
                return Color.white;
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (selectionGlow != null)
        {
            float targetAlpha = selected ? 0.8f : 0f;
            selectionGlow.DOFade(targetAlpha, animationDuration);
        }
        
        UpdateScale();
    }
    
    void UpdateScale()
    {
        float targetScale = 1f;
        
        if (isSelected)
            targetScale = selectScale;
        else if (isHovered)
            targetScale = hoverScale;
        
        transform.DOScale(originalScale * targetScale, animationDuration)
            .SetEase(Ease.OutQuad);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        
        // Elevate card
        transform.DOLocalMoveZ(-10f, animationDuration);
        
        UpdateScale();
        
        // Enhance glow
        if (selectionGlow != null && !isSelected)
        {
            selectionGlow.DOFade(0.3f, animationDuration);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        
        // Return to original position
        transform.DOLocalMoveZ(0f, animationDuration);
        
        UpdateScale();
        
        // Remove glow if not selected
        if (selectionGlow != null && !isSelected)
        {
            selectionGlow.DOFade(0f, animationDuration);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // Animate click
        transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 10, 1f);
        
        // Toggle selection
        SetSelected(!isSelected);
        
        // Notify callback
        onCardClicked?.Invoke(this);
        
        // Play sound effect (if available)
        PlayClickSound();
    }
    
    void PlayClickSound()
    {
        // Add sound effect here if available
        // AudioSource.PlayClipAtPoint(clickSound, transform.position);
    }
    
    // Animation for when card is created/dealt
    public void PlayDealAnimation(float delay = 0f)
    {
        // Start from deck position
        transform.localScale = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
        
        // Animate to final position
        transform.DOScale(originalScale, 0.5f)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
            
        transform.DORotate(Vector3.zero, 0.5f)
            .SetDelay(delay)
            .SetEase(Ease.OutQuad);
    }
    
    // Animation for when card is used/discarded
    public void PlayDiscardAnimation(System.Action onComplete = null)
    {
        Sequence discardSequence = DOTween.Sequence();
        
        discardSequence.Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
        discardSequence.Join(transform.DORotate(new Vector3(0, 0, 90f), 0.3f).SetEase(Ease.InQuad));
        discardSequence.OnComplete(() => {
            onComplete?.Invoke();
            gameObject.SetActive(false);
        });
    }
}

// Enhanced Compound Card
public class EnhancedCompoundCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Card Data")]
    public CompoundData compoundData;
    
    [Header("Visual Components")]
    public Image cardBackground;
    public Text formulaText;
    public Text compoundNameText;
    public Text effectText;
    public Image effectIcon;
    public Image selectionGlow;
    
    [Header("Required Elements Display")]
    public Transform elementsContainer;
    public GameObject elementSlotPrefab;
    
    private bool isSelected = false;
    private Vector3 originalScale;
    private ModernUITheme theme;
    private System.Action<EnhancedCompoundCard> onCardClicked;
    
    void Awake()
    {
        originalScale = transform.localScale;
        theme = ModernUITheme.Instance;
    }
    
    public void Initialize(CompoundData data, System.Action<EnhancedCompoundCard> clickCallback = null)
    {
        compoundData = data;
        onCardClicked = clickCallback;
        SetupVisuals();
    }
    
    void SetupVisuals()
    {
        if (theme == null || compoundData == null) return;
        
        // Setup card background
        Color compoundColor = GetCompoundColor(compoundData.effect);
        
        if (cardBackground != null)
        {
            cardBackground.color = compoundColor;
            cardBackground.sprite = theme.CreateRoundedRectSprite(200, 140, 12, compoundColor);
        }
        
        // Setup formula text
        if (formulaText != null)
        {
            formulaText.text = compoundData.formula;
            formulaText.color = Color.white;
            formulaText.fontSize = 32;
            formulaText.fontStyle = FontStyle.Bold;
        }
        
        // Setup compound name
        if (compoundNameText != null)
        {
            compoundNameText.text = compoundData.compoundName;
            compoundNameText.color = theme.colorPalette.textPrimary;
            compoundNameText.fontSize = 14;
        }
        
        // Setup effect text
        if (effectText != null)
        {
            effectText.text = GetEffectDescription(compoundData.effect);
            effectText.color = theme.colorPalette.textSecondary;
            effectText.fontSize = 12;
        }
        
        // Setup selection glow
        if (selectionGlow != null)
        {
            selectionGlow.color = new Color(theme.colorPalette.secondaryColor.r, theme.colorPalette.secondaryColor.g, theme.colorPalette.secondaryColor.b, 0f);
        }
        
        // Display required elements
        DisplayRequiredElements();
    }
    
    Color GetCompoundColor(CompoundEffect effect)
    {
        switch (effect)
        {
            case CompoundEffect.DrawElements:
                return new Color(0.3f, 0.7f, 1f, 1f); // Blue
            case CompoundEffect.DiscardElements:
                return new Color(1f, 0.5f, 0.2f, 1f); // Orange
            case CompoundEffect.ReceiveElements:
                return new Color(0.4f, 0.8f, 0.3f, 1f); // Green
            case CompoundEffect.SkipPlayerTurn:
                return new Color(0.9f, 0.3f, 0.3f, 1f); // Red
            case CompoundEffect.SwapHands:
                return new Color(0.7f, 0.3f, 0.9f, 1f); // Purple
            default:
                return new Color(0.5f, 0.5f, 0.5f, 1f); // Gray
        }
    }
    
    string GetEffectDescription(CompoundEffect effect)
    {
        switch (effect)
        {
            case CompoundEffect.DrawElements:
                return "Draw more elements";
            case CompoundEffect.DiscardElements:
                return "Force opponent discard";
            case CompoundEffect.ReceiveElements:
                return "Gain bonus elements";
            case CompoundEffect.SkipPlayerTurn:
                return "Skip opponent's turn";
            case CompoundEffect.SwapHands:
                return "Swap hands with opponent";
            default:
                return "No special effect";
        }
    }
    
    void DisplayRequiredElements()
    {
        if (elementsContainer == null || compoundData.requiredElements == null) return;
        
        // Clear existing elements
        foreach (Transform child in elementsContainer)
        {
            DestroyImmediate(child.gameObject);
        }
        
        // Create element slots
        foreach (var requirement in compoundData.requiredElements)
        {
            for (int i = 0; i < requirement.quantity; i++)
            {
                GameObject slotObj = new GameObject($"Element_{requirement.element.symbol}_{i}");
                slotObj.transform.SetParent(elementsContainer);
                
                RectTransform slotRect = slotObj.AddComponent<RectTransform>();
                slotRect.sizeDelta = new Vector2(30, 30);
                slotRect.localScale = Vector3.one;
                
                Image slotImage = slotObj.AddComponent<Image>();
                slotImage.sprite = theme.CreateRoundedRectSprite(30, 30, 4, Color.white);
                
                Text slotText = slotObj.AddComponent<Text>();
                slotText.text = requirement.element.symbol;
                slotText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                slotText.fontSize = 14;
                slotText.fontStyle = FontStyle.Bold;
                slotText.color = Color.black;
                slotText.alignment = TextAnchor.MiddleCenter;
                
                RectTransform textRect = slotText.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.anchoredPosition = Vector2.zero;
            }
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (selectionGlow != null)
        {
            float targetAlpha = selected ? 0.8f : 0f;
            selectionGlow.DOFade(targetAlpha, 0.2f);
        }
        
        float targetScale = selected ? 1.1f : 1f;
        transform.DOScale(originalScale * targetScale, 0.2f).SetEase(Ease.OutQuad);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            transform.DOScale(originalScale * 1.05f, 0.2f).SetEase(Ease.OutQuad);
            
            if (selectionGlow != null)
                selectionGlow.DOFade(0.3f, 0.2f);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutQuad);
            
            if (selectionGlow != null)
                selectionGlow.DOFade(0f, 0.2f);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        SetSelected(!isSelected);
        onCardClicked?.Invoke(this);
    }
}