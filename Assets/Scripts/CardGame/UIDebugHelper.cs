using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDebugHelper : MonoBehaviour
{
    [ContextMenu("Debug UI Setup")]
    public void DebugUISetup()
    {
        Debug.Log("=== UI DEBUG ANALYSIS ===");
        
        // Check EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ NO EVENTSYSTEM FOUND! UI interactions will not work.");
        }
        else
        {
            Debug.Log($"✓ EventSystem found: {eventSystem.name}");
            Debug.Log($"  - Current Selected: {eventSystem.currentSelectedGameObject}");
            Debug.Log($"  - Sending Events: {eventSystem.sendNavigationEvents}");
        }
        
        // Check Canvas
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Debug.Log($"Found {canvases.Length} Canvas(es):");
        
        foreach (Canvas canvas in canvases)
        {
            Debug.Log($"  Canvas: {canvas.name}");
            Debug.Log($"    - Render Mode: {canvas.renderMode}");
            Debug.Log($"    - Sort Order: {canvas.sortingOrder}");
            
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                Debug.LogWarning($"    ❌ No GraphicRaycaster on {canvas.name}");
            }
            else
            {
                Debug.Log($"    ✓ GraphicRaycaster present, blocking objects: {raycaster.blockingObjects}");
            }
        }
        
        // Check Buttons
        Button[] buttons = FindObjectsOfType<Button>();
        Debug.Log($"Found {buttons.Length} Button(s):");
        
        foreach (Button button in buttons)
        {
            Debug.Log($"  Button: {button.name}");
            Debug.Log($"    - Interactable: {button.interactable}");
            Debug.Log($"    - Target Graphic: {(button.targetGraphic != null ? button.targetGraphic.name : "NULL")}");
            Debug.Log($"    - Raycast Target: {(button.targetGraphic != null ? button.targetGraphic.raycastTarget : false)}");
            Debug.Log($"    - Listeners: {button.onClick.GetPersistentEventCount()}");
            
            if (button.targetGraphic != null)
            {
                RectTransform rect = button.targetGraphic.GetComponent<RectTransform>();
                if (rect != null)
                {
                    Debug.Log($"    - Size: {rect.sizeDelta}");
                    Debug.Log($"    - Position: {rect.anchoredPosition}");
                }
            }
        }
        
        Debug.Log("=== END UI DEBUG ===");
    }
    
    [ContextMenu("Test Button Clicks")]
    public void TestButtonClicks()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        
        foreach (Button button in buttons)
        {
            if (button.interactable)
            {
                Debug.Log($"Testing button: {button.name}");
                try
                {
                    button.onClick.Invoke();
                    Debug.Log($"  ✓ Button {button.name} clicked successfully");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"  ❌ Error clicking button {button.name}: {e.Message}");
                }
            }
            else
            {
                Debug.Log($"  ⚠️ Button {button.name} is not interactable");
            }
        }
    }
    
    [ContextMenu("Fix Common UI Issues")]
    public void FixCommonUIIssues()
    {
        Debug.Log("=== FIXING COMMON UI ISSUES ===");
        
        // Ensure EventSystem exists
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("✓ Created EventSystem");
        }
        
        // Fix Canvas issues
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = canvas.gameObject.AddComponent<GraphicRaycaster>();
                Debug.Log($"✓ Added GraphicRaycaster to {canvas.name}");
            }
        }
        
        // Fix Button issues
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            if (button.targetGraphic == null)
            {
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    button.targetGraphic = image;
                    image.raycastTarget = true;
                    Debug.Log($"✓ Fixed target graphic for {button.name}");
                }
            }
            
            if (button.targetGraphic != null && !button.targetGraphic.raycastTarget)
            {
                button.targetGraphic.raycastTarget = true;
                Debug.Log($"✓ Enabled raycast target for {button.name}");
            }
        }
        
        Debug.Log("=== UI FIXES COMPLETE ===");
    }
}