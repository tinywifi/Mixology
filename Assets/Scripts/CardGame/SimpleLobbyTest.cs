using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class SimpleLobbyTest : MonoBehaviour
{
    [Header("Target Canvas")]
    public Canvas targetCanvas;
    
    [ContextMenu("Create Simple Test Lobby")]
    public void CreateSimpleTestLobby()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("Target Canvas not assigned!");
            return;
        }
        
        Debug.Log("Creating simple test lobby...");
        
        // Clear existing UI
        ClearCanvas();
        
        // Ensure EventSystem exists
        SetupEventSystem();
        
        // Setup canvas
        SetupCanvas();
        
        // Create simple test buttons
        CreateSimpleButtons();
        
        Debug.Log("✓ Simple test lobby created!");
    }
    
    private void ClearCanvas()
    {
        for (int i = targetCanvas.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = targetCanvas.transform.GetChild(i).gameObject;
            #if UNITY_EDITOR
            DestroyImmediate(child);
            #else
            Destroy(child);
            #endif
        }
    }
    
    private void SetupEventSystem()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("✓ EventSystem created");
        }
        else
        {
            Debug.Log("✓ EventSystem already exists");
        }
    }
    
    private void SetupCanvas()
    {
        CanvasScaler scaler = targetCanvas.GetComponent<CanvasScaler>();
        if (scaler == null)
            scaler = targetCanvas.gameObject.AddComponent<CanvasScaler>();
            
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        GraphicRaycaster raycaster = targetCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
            raycaster = targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
            
        Debug.Log("✓ Canvas configured");
    }
    
    private void CreateSimpleButtons()
    {
        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(targetCanvas.transform);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.localScale = Vector3.one;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.2f, 1f);
        
        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(targetCanvas.transform);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.2f, 0.8f);
        titleRect.anchorMax = new Vector2(0.8f, 0.9f);
        titleRect.sizeDelta = Vector2.zero;
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.localScale = Vector3.one;
        
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "CHEMISTRY PARTY - TEST LOBBY";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 42;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        
        // Buttons container
        GameObject buttonsContainer = new GameObject("ButtonsContainer");
        buttonsContainer.transform.SetParent(targetCanvas.transform);
        RectTransform buttonsRect = buttonsContainer.AddComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(0.3f, 0.3f);
        buttonsRect.anchorMax = new Vector2(0.7f, 0.7f);
        buttonsRect.sizeDelta = Vector2.zero;
        buttonsRect.anchoredPosition = Vector2.zero;
        buttonsRect.localScale = Vector3.one;
        
        VerticalLayoutGroup layout = buttonsContainer.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 20f;
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        
        // Create test buttons
        CreateTestButton("START GAME", buttonsContainer.transform, () => {
            Debug.Log("Starting game...");
            SceneManager.LoadScene("CardGame-Board");
        }, Color.green);
        
        CreateTestButton("TEST LOG", buttonsContainer.transform, () => {
            Debug.Log("Test button clicked successfully!");
        }, Color.blue);
        
        CreateTestButton("MULTIPLAYER", buttonsContainer.transform, () => {
            Debug.Log("Multiplayer clicked - not implemented yet");
        }, Color.yellow);
        
        CreateTestButton("EXIT", buttonsContainer.transform, () => {
            Debug.Log("Exiting...");
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }, Color.red);
        
        Debug.Log("✓ Test buttons created");
        
        // Add runtime button manager
        LobbyButtonManager buttonManager = targetCanvas.GetComponent<LobbyButtonManager>();
        if (buttonManager == null)
        {
            buttonManager = targetCanvas.gameObject.AddComponent<LobbyButtonManager>();
            Debug.Log("✓ LobbyButtonManager added");
        }
        
        Debug.Log("✓ Simple test lobby created!");
        Debug.Log("IMPORTANT: Enter PLAY MODE to test the buttons!");
    }
    
    private void CreateTestButton(string text, Transform parent, System.Action onClick, Color color)
    {
        GameObject buttonObj = new GameObject($"Button_{text.Replace(" ", "")}");
        buttonObj.transform.SetParent(parent);
        buttonObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(0, 80);
        buttonRect.localScale = Vector3.one;
        
        // Button background
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = color;
        buttonImage.raycastTarget = true;
        
        // Button component
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.interactable = true;
        
        // Add click listener
        button.onClick.AddListener(() => {
            Debug.Log($"=== BUTTON CLICK TEST ===");
            Debug.Log($"Button '{text}' was clicked!");
            
            if (onClick != null)
            {
                try
                {
                    onClick.Invoke();
                    Debug.Log($"Button '{text}' action executed successfully!");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error in button '{text}': {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"No action assigned to button '{text}'");
            }
            
            Debug.Log($"=== END BUTTON TEST ===");
        });
        
        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        textRect.localScale = Vector3.one;
        
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 28;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.raycastTarget = false;
        
        Debug.Log($"✓ Created test button: {text}");
    }
}
#endif