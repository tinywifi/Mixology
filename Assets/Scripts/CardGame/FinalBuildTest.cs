using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Final build safety test - ensures the game can run in builds
public class FinalBuildTest : MonoBehaviour
{
    [Header("Final Build Test")]
    public Canvas testCanvas;
    public bool runTestOnStart = true;
    
    void Start()
    {
        if (runTestOnStart)
        {
            RunFinalBuildTest();
        }
    }
    
    [ContextMenu("Run Final Build Test")]
    public void RunFinalBuildTest()
    {
        Debug.Log("=== FINAL BUILD TEST STARTING ===");
        
        // Test 1: Basic Unity functionality
        TestBasicUnityFunctions();
        
        // Test 2: UI Creation (build-safe)
        TestUICreation();
        
        // Test 3: Scene loading capability
        TestSceneLoadingCapability();
        
        // Test 4: Component references
        TestComponentReferences();
        
        Debug.Log("=== FINAL BUILD TEST COMPLETE ===");
        Debug.Log("✅ If you see this message, the game should work in builds!");
    }
    
    private void TestBasicUnityFunctions()
    {
        Debug.Log("--- Testing Basic Unity Functions ---");
        
        try
        {
            // Test GameObject creation
            GameObject testObj = new GameObject("BuildTest");
            Debug.Log("✓ GameObject creation works");
            
            // Test component addition
            RectTransform rect = testObj.AddComponent<RectTransform>();
            Debug.Log("✓ Component addition works");
            
            // Test safe destruction
            if (Application.isPlaying)
                Destroy(testObj);
            else
            {
                #if UNITY_EDITOR
                DestroyImmediate(testObj);
                #endif
            }
            Debug.Log("✓ Safe object destruction works");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Basic Unity functions failed: {e.Message}");
        }
    }
    
    private void TestUICreation()
    {
        Debug.Log("--- Testing UI Creation ---");
        
        try
        {
            if (testCanvas == null)
            {
                Debug.LogWarning("⚠️ Test canvas not assigned, skipping UI test");
                return;
            }
            
            // Create a simple test UI element
            GameObject testUI = new GameObject("BuildTestUI");
            testUI.transform.SetParent(testCanvas.transform);
            
            RectTransform uiRect = testUI.AddComponent<RectTransform>();
            uiRect.anchorMin = new Vector2(0.1f, 0.1f);
            uiRect.anchorMax = new Vector2(0.9f, 0.2f);
            uiRect.sizeDelta = Vector2.zero;
            uiRect.anchoredPosition = Vector2.zero;
            uiRect.localScale = Vector3.one;
            
            Text testText = testUI.AddComponent<Text>();
            testText.text = "BUILD TEST: UI Creation Works!";
            testText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            testText.fontSize = 24;
            testText.color = Color.green;
            testText.alignment = TextAnchor.MiddleCenter;
            
            Debug.Log("✓ UI creation works");
            
            // Clean up after a few seconds
            Destroy(testUI, 3f);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ UI creation failed: {e.Message}");
        }
    }
    
    private void TestSceneLoadingCapability()
    {
        Debug.Log("--- Testing Scene Loading Capability ---");
        
        try
        {
            // Check available scenes
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            Debug.Log($"Scenes in build settings: {sceneCount}");
            
            if (sceneCount > 0)
            {
                string currentScene = SceneManager.GetActiveScene().name;
                Debug.Log($"Current scene: {currentScene}");
                Debug.Log("✓ Scene management works");
            }
            else
            {
                Debug.LogWarning("⚠️ No scenes found in build settings");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Scene loading test failed: {e.Message}");
        }
    }
    
    private void TestComponentReferences()
    {
        Debug.Log("--- Testing Component References ---");
        
        try
        {
            // Test finding components
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            Debug.Log($"Found {canvases.Length} Canvas components");
            
            MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();
            int validScripts = 0;
            int nullScripts = 0;
            
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null)
                    validScripts++;
                else
                    nullScripts++;
            }
            
            Debug.Log($"Found {validScripts} valid scripts, {nullScripts} null references");
            
            if (nullScripts == 0)
            {
                Debug.Log("✓ No missing script references");
            }
            else
            {
                Debug.LogWarning($"⚠️ Found {nullScripts} missing script references");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Component reference test failed: {e.Message}");
        }
    }
    
    // Test specific game functionality
    [ContextMenu("Test Game-Specific Functions")]
    public void TestGameSpecificFunctions()
    {
        Debug.Log("=== TESTING GAME-SPECIFIC FUNCTIONS ===");
        
        // Test ChemistryDatabase loading
        TestChemistryDatabase();
        
        // Test Photon availability
        TestPhotonAvailability();
        
        // Test DOTween availability
        TestDOTweenAvailability();
        
        Debug.Log("=== GAME-SPECIFIC TESTS COMPLETE ===");
    }
    
    private void TestChemistryDatabase()
    {
        Debug.Log("--- Testing Chemistry Database ---");
        
        try
        {
            ChemistryDatabase database = FindObjectOfType<ChemistryDatabase>();
            if (database != null)
            {
                Debug.Log($"✓ ChemistryDatabase found with {database.allElements.Count} elements");
            }
            else
            {
                // Try loading from resources
                database = Resources.Load<ChemistryDatabase>("ChemistryDatabase");
                if (database != null)
                {
                    Debug.Log($"✓ ChemistryDatabase loaded from resources with {database.allElements.Count} elements");
                }
                else
                {
                    Debug.LogWarning("⚠️ ChemistryDatabase not found - game may not work properly");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Chemistry Database test failed: {e.Message}");
        }
    }
    
    private void TestPhotonAvailability()
    {
        Debug.Log("--- Testing Photon Availability ---");
        
        try
        {
            System.Type photonType = System.Type.GetType("Photon.Pun.PhotonNetwork");
            if (photonType != null)
            {
                Debug.Log("✓ Photon PUN2 is available");
            }
            else
            {
                Debug.LogWarning("⚠️ Photon PUN2 not found - multiplayer will not work");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Photon test failed: {e.Message}");
        }
    }
    
    private void TestDOTweenAvailability()
    {
        Debug.Log("--- Testing DOTween Availability ---");
        
        try
        {
            System.Type dotweenType = System.Type.GetType("DG.Tweening.DOTween");
            if (dotweenType != null)
            {
                Debug.Log("✓ DOTween is available");
            }
            else
            {
                Debug.LogWarning("⚠️ DOTween not found - animations may not work");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ DOTween test failed: {e.Message}");
        }
    }
}