using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class UIColorPalette
{
    [Header("Primary Colors")]
    public Color primaryColor = new Color(0.2f, 0.4f, 0.8f, 1f);        // Modern blue
    public Color primaryDark = new Color(0.1f, 0.2f, 0.6f, 1f);         // Darker blue
    public Color primaryLight = new Color(0.4f, 0.6f, 0.9f, 1f);        // Light blue
    
    [Header("Secondary Colors")]
    public Color secondaryColor = new Color(0.9f, 0.5f, 0.2f, 1f);      // Orange accent
    public Color secondaryDark = new Color(0.7f, 0.3f, 0.1f, 1f);       // Darker orange
    public Color secondaryLight = new Color(1f, 0.7f, 0.4f, 1f);        // Light orange
    
    [Header("Neutral Colors")]
    public Color backgroundColor = new Color(0.05f, 0.08f, 0.15f, 1f);  // Dark navy background
    public Color surfaceColor = new Color(0.12f, 0.16f, 0.25f, 1f);     // Card/panel surface
    public Color cardColor = new Color(0.18f, 0.22f, 0.32f, 1f);        // Individual cards
    
    [Header("Text Colors")]
    public Color textPrimary = new Color(0.95f, 0.95f, 0.95f, 1f);      // White text
    public Color textSecondary = new Color(0.7f, 0.7f, 0.7f, 1f);       // Gray text
    public Color textAccent = new Color(0.4f, 0.8f, 1f, 1f);            // Accent text
    
    [Header("Status Colors")]
    public Color successColor = new Color(0.2f, 0.8f, 0.3f, 1f);        // Green
    public Color warningColor = new Color(0.9f, 0.7f, 0.1f, 1f);        // Yellow
    public Color errorColor = new Color(0.9f, 0.3f, 0.2f, 1f);          // Red
    public Color infoColor = new Color(0.3f, 0.7f, 0.9f, 1f);           // Light blue
}

[System.Serializable]
public class UIAnimationSettings
{
    [Header("Button Animations")]
    public float buttonHoverScale = 1.05f;
    public float buttonPressScale = 0.95f;
    public float animationDuration = 0.2f;
    
    [Header("Panel Animations")]
    public float panelFadeInDuration = 0.3f;
    public float panelSlideDistance = 50f;
    
    [Header("Card Animations")]
    public float cardHoverElevation = 10f;
    public float cardSelectScale = 1.1f;
}

public class ModernUITheme : MonoBehaviour
{
    [Header("Theme Settings")]
    public UIColorPalette colorPalette = new UIColorPalette();
    public UIAnimationSettings animationSettings = new UIAnimationSettings();
    
    [Header("Typography")]
    public Font primaryFont;
    public Font secondaryFont;
    public int titleFontSize = 48;
    public int headerFontSize = 32;
    public int bodyFontSize = 24;
    public int captionFontSize = 18;
    
    [Header("Shadows & Effects")]
    public bool enableShadows = true;
    public Color shadowColor = new Color(0f, 0f, 0f, 0.5f);
    public Vector2 shadowOffset = new Vector2(2f, -2f);
    
    private static ModernUITheme instance;
    public static ModernUITheme Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ModernUITheme>();
                if (instance == null)
                {
                    GameObject themeObject = new GameObject("ModernUITheme");
                    instance = themeObject.AddComponent<ModernUITheme>();
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
            // Only call DontDestroyOnLoad in play mode
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (instance != this)
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
    
    // Gradient creation helper
    public Texture2D CreateGradientTexture(Color startColor, Color endColor, int width = 256, int height = 1)
    {
        Texture2D texture = new Texture2D(width, height);
        
        for (int x = 0; x < width; x++)
        {
            float t = (float)x / (width - 1);
            Color color = Color.Lerp(startColor, endColor, t);
            
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    // Create rounded corner sprite
    public Sprite CreateRoundedRectSprite(int width, int height, int cornerRadius, Color color)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color transparent = new Color(0, 0, 0, 0);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isInside = IsInsideRoundedRect(x, y, width, height, cornerRadius);
                texture.SetPixel(x, y, isInside ? color : transparent);
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
    
    private bool IsInsideRoundedRect(int x, int y, int width, int height, int radius)
    {
        // Check if point is in main rectangle
        if (x >= radius && x < width - radius && y >= radius && y < height - radius)
            return true;
            
        // Check corners
        Vector2[] corners = {
            new Vector2(radius, radius),                    // Bottom-left
            new Vector2(width - radius, radius),            // Bottom-right
            new Vector2(radius, height - radius),           // Top-left
            new Vector2(width - radius, height - radius)    // Top-right
        };
        
        foreach (Vector2 corner in corners)
        {
            float distance = Vector2.Distance(new Vector2(x, y), corner);
            if (distance <= radius)
            {
                if ((x < radius && y < radius && corner.x == radius && corner.y == radius) ||
                    (x >= width - radius && y < radius && corner.x == width - radius && corner.y == radius) ||
                    (x < radius && y >= height - radius && corner.x == radius && corner.y == height - radius) ||
                    (x >= width - radius && y >= height - radius && corner.x == width - radius && corner.y == height - radius))
                {
                    return true;
                }
            }
        }
        
        // Check edge areas
        return (x >= radius && x < width - radius) || (y >= radius && y < height - radius);
    }
}