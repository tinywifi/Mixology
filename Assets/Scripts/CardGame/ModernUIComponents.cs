using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

// Enhanced Button Component
public class ModernButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button Settings")]
    public ButtonStyle buttonStyle = ButtonStyle.Primary;
    public bool enableHoverEffects = true;
    public bool enableClickAnimation = true;
    
    private Button button;
    private Image buttonImage;
    private Text buttonText;
    private Vector3 originalScale;
    private Color originalColor;
    private ModernUITheme theme;
    
    public enum ButtonStyle
    {
        Primary,
        Secondary,
        Success,
        Warning,
        Error,
        Ghost
    }
    
    void Start()
    {
        theme = ModernUITheme.Instance;
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>();
        originalScale = transform.localScale;
        
        // Don't interfere with existing button functionality
        if (button != null)
        {
            Debug.Log($"ModernButton: Setting up for button '{gameObject.name}', interactable: {button.interactable}");
        }
        
        ApplyTheme();
    }
    
    void ApplyTheme()
    {
        if (theme == null || buttonImage == null) return;
        
        // Apply colors based on style
        switch (buttonStyle)
        {
            case ButtonStyle.Primary:
                originalColor = theme.colorPalette.primaryColor;
                break;
            case ButtonStyle.Secondary:
                originalColor = theme.colorPalette.secondaryColor;
                break;
            case ButtonStyle.Success:
                originalColor = theme.colorPalette.successColor;
                break;
            case ButtonStyle.Warning:
                originalColor = theme.colorPalette.warningColor;
                break;
            case ButtonStyle.Error:
                originalColor = theme.colorPalette.errorColor;
                break;
            case ButtonStyle.Ghost:
                originalColor = new Color(theme.colorPalette.textPrimary.r, theme.colorPalette.textPrimary.g, theme.colorPalette.textPrimary.b, 0.1f);
                break;
        }
        
        buttonImage.color = originalColor;
        
        // Create rounded sprite if needed
        if (buttonImage.sprite == null)
        {
            buttonImage.sprite = theme.CreateRoundedRectSprite(200, 60, 12, originalColor);
        }
        
        // Apply text styling
        if (buttonText != null)
        {
            buttonText.color = buttonStyle == ButtonStyle.Ghost ? theme.colorPalette.textPrimary : Color.white;
            buttonText.fontSize = theme.bodyFontSize;
            buttonText.fontStyle = FontStyle.Bold;
            
            if (theme.primaryFont != null)
                buttonText.font = theme.primaryFont;
        }
        
        // Add shadow effect
        AddShadowEffect();
    }
    
    void AddShadowEffect()
    {
        if (!theme.enableShadows) return;
        
        GameObject shadowObj = new GameObject("ButtonShadow");
        shadowObj.transform.SetParent(transform);
        shadowObj.transform.SetAsFirstSibling();
        
        RectTransform shadowRect = shadowObj.AddComponent<RectTransform>();
        Image shadowImage = shadowObj.AddComponent<Image>();
        
        // Copy button dimensions
        RectTransform buttonRect = GetComponent<RectTransform>();
        shadowRect.anchorMin = buttonRect.anchorMin;
        shadowRect.anchorMax = buttonRect.anchorMax;
        shadowRect.sizeDelta = buttonRect.sizeDelta;
        shadowRect.anchoredPosition = buttonRect.anchoredPosition + theme.shadowOffset;
        shadowRect.localScale = Vector3.one;
        
        // Apply shadow styling
        shadowImage.sprite = buttonImage.sprite;
        shadowImage.color = theme.shadowColor;
        shadowImage.raycastTarget = false;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!enableHoverEffects || !button.interactable) return;
        
        transform.DOScale(originalScale * theme.animationSettings.buttonHoverScale, theme.animationSettings.animationDuration)
            .SetEase(Ease.OutQuad);
            
        Color hoverColor = originalColor * 1.2f;
        hoverColor.a = originalColor.a;
        buttonImage.DOColor(hoverColor, theme.animationSettings.animationDuration);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!enableHoverEffects) return;
        
        transform.DOScale(originalScale, theme.animationSettings.animationDuration)
            .SetEase(Ease.OutQuad);
            
        buttonImage.DOColor(originalColor, theme.animationSettings.animationDuration);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!enableClickAnimation || !button.interactable) return;
        
        transform.DOScale(originalScale * theme.animationSettings.buttonPressScale, theme.animationSettings.animationDuration * 0.5f)
            .SetEase(Ease.OutQuad);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!enableClickAnimation) return;
        
        float targetScale = enableHoverEffects && eventData.pointerCurrentRaycast.gameObject == gameObject ? 
            theme.animationSettings.buttonHoverScale : 1f;
            
        transform.DOScale(originalScale * targetScale, theme.animationSettings.animationDuration * 0.5f)
            .SetEase(Ease.OutQuad);
    }
}

// Enhanced Panel Component
public class ModernPanel : MonoBehaviour
{
    [Header("Panel Settings")]
    public PanelStyle panelStyle = PanelStyle.Surface;
    public bool enableAnimatedEntrance = true;
    public bool addGlowEffect = false;
    
    private Image panelImage;
    private ModernUITheme theme;
    
    public enum PanelStyle
    {
        Background,
        Surface,
        Card,
        Overlay
    }
    
    void Start()
    {
        theme = ModernUITheme.Instance;
        panelImage = GetComponent<Image>();
        
        ApplyTheme();
        
        if (enableAnimatedEntrance)
            PlayEntranceAnimation();
    }
    
    void ApplyTheme()
    {
        if (theme == null || panelImage == null) return;
        
        Color panelColor;
        switch (panelStyle)
        {
            case PanelStyle.Background:
                panelColor = theme.colorPalette.backgroundColor;
                break;
            case PanelStyle.Surface:
                panelColor = theme.colorPalette.surfaceColor;
                break;
            case PanelStyle.Card:
                panelColor = theme.colorPalette.cardColor;
                break;
            case PanelStyle.Overlay:
                panelColor = new Color(0f, 0f, 0f, 0.8f);
                break;
            default:
                panelColor = theme.colorPalette.surfaceColor;
                break;
        }
        
        panelImage.color = panelColor;
        
        // Create rounded sprite for surface and card panels
        if (panelStyle == PanelStyle.Surface || panelStyle == PanelStyle.Card)
        {
            RectTransform rect = GetComponent<RectTransform>();
            int width = Mathf.RoundToInt(rect.sizeDelta.x);
            int height = Mathf.RoundToInt(rect.sizeDelta.y);
            
            if (width > 0 && height > 0)
            {
                panelImage.sprite = theme.CreateRoundedRectSprite(width, height, 16, panelColor);
            }
        }
        
        // Add glow effect if enabled
        if (addGlowEffect)
            AddGlowEffect();
    }
    
    void AddGlowEffect()
    {
        GameObject glowObj = new GameObject("PanelGlow");
        glowObj.transform.SetParent(transform);
        glowObj.transform.SetAsFirstSibling();
        
        RectTransform glowRect = glowObj.AddComponent<RectTransform>();
        Image glowImage = glowObj.AddComponent<Image>();
        
        // Make glow slightly larger
        RectTransform panelRect = GetComponent<RectTransform>();
        glowRect.anchorMin = panelRect.anchorMin;
        glowRect.anchorMax = panelRect.anchorMax;
        glowRect.sizeDelta = panelRect.sizeDelta + Vector2.one * 10f;
        glowRect.anchoredPosition = panelRect.anchoredPosition;
        glowRect.localScale = Vector3.one;
        
        // Create glow sprite
        int width = Mathf.RoundToInt(glowRect.sizeDelta.x);
        int height = Mathf.RoundToInt(glowRect.sizeDelta.y);
        Color glowColor = new Color(theme.colorPalette.primaryColor.r, theme.colorPalette.primaryColor.g, theme.colorPalette.primaryColor.b, 0.3f);
        
        glowImage.sprite = theme.CreateRoundedRectSprite(width, height, 20, glowColor);
        glowImage.raycastTarget = false;
        
        // Animate glow
        glowImage.DOFade(0.1f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
    
    void PlayEntranceAnimation()
    {
        if (panelImage == null) return; // Safety check
        
        Vector3 originalPosition = transform.localPosition;
        Color originalColor = panelImage.color;
        
        // Start from below and transparent
        transform.localPosition = originalPosition + Vector3.down * theme.animationSettings.panelSlideDistance;
        panelImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        
        // Animate to final position and alpha
        transform.DOLocalMove(originalPosition, theme.animationSettings.panelFadeInDuration)
            .SetEase(Ease.OutQuad);
        panelImage.DOColor(originalColor, theme.animationSettings.panelFadeInDuration);
    }
}

// Enhanced Text Component
public class ModernText : MonoBehaviour
{
    [Header("Text Settings")]
    public TextStyle textStyle = TextStyle.Body;
    public bool enableTypewriterEffect = false;
    public float typewriterSpeed = 0.05f;
    
    private Text textComponent;
    private ModernUITheme theme;
    private string fullText;
    
    public enum TextStyle
    {
        Title,
        Header,
        Body,
        Caption,
        Accent
    }
    
    void Start()
    {
        theme = ModernUITheme.Instance;
        textComponent = GetComponent<Text>();
        
        if (textComponent != null)
        {
            fullText = textComponent.text;
            ApplyTheme();
            
            if (enableTypewriterEffect)
                StartCoroutine(TypewriterEffect());
        }
    }
    
    void ApplyTheme()
    {
        if (theme == null || textComponent == null) return;
        
        // Apply font
        if (theme.primaryFont != null)
            textComponent.font = theme.primaryFont;
        
        // Apply styling based on text style
        switch (textStyle)
        {
            case TextStyle.Title:
                textComponent.fontSize = theme.titleFontSize;
                textComponent.color = theme.colorPalette.textPrimary;
                textComponent.fontStyle = FontStyle.Bold;
                break;
            case TextStyle.Header:
                textComponent.fontSize = theme.headerFontSize;
                textComponent.color = theme.colorPalette.textPrimary;
                textComponent.fontStyle = FontStyle.Bold;
                break;
            case TextStyle.Body:
                textComponent.fontSize = theme.bodyFontSize;
                textComponent.color = theme.colorPalette.textPrimary;
                textComponent.fontStyle = FontStyle.Normal;
                break;
            case TextStyle.Caption:
                textComponent.fontSize = theme.captionFontSize;
                textComponent.color = theme.colorPalette.textSecondary;
                textComponent.fontStyle = FontStyle.Normal;
                break;
            case TextStyle.Accent:
                textComponent.fontSize = theme.bodyFontSize;
                textComponent.color = theme.colorPalette.textAccent;
                textComponent.fontStyle = FontStyle.Bold;
                break;
        }
        
        // Add outline for better readability
        Outline outline = textComponent.gameObject.GetComponent<Outline>();
        if (outline == null)
            outline = textComponent.gameObject.AddComponent<Outline>();
            
        outline.effectColor = new Color(0f, 0f, 0f, 0.5f);
        outline.effectDistance = new Vector2(1f, -1f);
    }
    
    IEnumerator TypewriterEffect()
    {
        textComponent.text = "";
        
        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }
}