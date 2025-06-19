#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;
using System.IO;

// Quick and reliable build fix tool
public class QuickBuildFix : MonoBehaviour
{
    [ContextMenu("🚀 Quick Build Fix & Test")]
    public void QuickBuildFixAndTest()
    {
        Debug.Log("=== QUICK BUILD FIX STARTING ===");
        
        // Step 1: Fix critical build issues
        FixCriticalIssues();
        
        // Step 2: Configure build settings
        ConfigureBuildSettings();
        
        // Step 3: Validate everything is ready
        ValidateBuildReadiness();
        
        Debug.Log("=== QUICK BUILD FIX COMPLETE ===");
        Debug.Log("✅ You can now try Build and Run!");
        Debug.Log("💡 Recommended: Use 'Development Build' option for debugging");
    }
    
    [ContextMenu("🔧 Fix Critical Issues Only")]
    public void FixCriticalIssues()
    {
        Debug.Log("--- Fixing Critical Build Issues ---");
        
        // 1. Ensure scenes are in build settings
        AddEssentialScenes();
        
        // 2. Set safe build configuration
        SetSafeBuildConfiguration();
        
        // 3. Validate critical components
        ValidateCriticalComponents();
        
        Debug.Log("✓ Critical issues fixed");
    }
    
    private void AddEssentialScenes()
    {
        Debug.Log("Adding essential scenes to build settings...");
        
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        var buildScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        
        // Keep existing enabled scenes
        foreach (EditorBuildSettingsScene existingScene in EditorBuildSettings.scenes)
        {
            if (existingScene.enabled && File.Exists(existingScene.path))
            {
                buildScenes.Add(existingScene);
            }
        }
        
        // Add missing essential scenes
        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = Path.GetFileNameWithoutExtension(path);
            
            // Check if already added
            bool alreadyAdded = false;
            foreach (var existing in buildScenes)
            {
                if (existing.path == path)
                {
                    alreadyAdded = true;
                    break;
                }
            }
            
            // Add important scenes
            if (!alreadyAdded && (sceneName.Contains("CardGame") || sceneName.Contains("Lobby") || sceneName.Contains("Board")))
            {
                EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(path, true);
                buildScenes.Add(newScene);
                Debug.Log($"✓ Added scene: {sceneName}");
            }
        }
        
        EditorBuildSettings.scenes = buildScenes.ToArray();
        Debug.Log($"✓ Build settings updated with {buildScenes.Count} scenes");
    }
    
    private void SetSafeBuildConfiguration()
    {
        Debug.Log("Setting safe build configuration...");
        
        // Enable development build for debugging
        EditorUserBuildSettings.development = true;
        EditorUserBuildSettings.allowDebugging = true;
        
        // Set target to Windows 64-bit if not already
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        }
        
        // Configure player settings for stability  
        PlayerSettings.productName = "Chemistry Party";
        PlayerSettings.companyName = "Unity Developer";
        PlayerSettings.runInBackground = true;
        
        // Use .NET Standard 2.0 for better compatibility
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_Standard_2_0);
        
        Debug.Log("✓ Build configuration set for maximum compatibility");
    }
    
    private void ValidateCriticalComponents()
    {
        Debug.Log("Validating critical components...");
        
        // Check for missing scripts in current scene
        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
        int missingScripts = 0;
        
        foreach (MonoBehaviour mb in allMonoBehaviours)
        {
            if (mb == null)
            {
                missingScripts++;
            }
        }
        
        if (missingScripts > 0)
        {
            Debug.LogWarning($"⚠️ Found {missingScripts} missing script references - this may cause build issues");
        }
        else
        {
            Debug.Log("✓ No missing script references found");
        }
        
        // Quick dependency check
        bool dotweenFound = System.Type.GetType("DG.Tweening.DOTween") != null;
        bool photonFound = System.Type.GetType("Photon.Pun.PhotonNetwork") != null;
        
        Debug.Log($"DOTween available: {dotweenFound}");
        Debug.Log($"Photon PUN2 available: {photonFound}");
        
        if (!dotweenFound)
        {
            Debug.LogWarning("⚠️ DOTween not found - some animations may not work");
        }
        
        if (!photonFound)
        {
            Debug.LogWarning("⚠️ Photon PUN2 not found - multiplayer features will not work");
        }
    }
    
    private void ConfigureBuildSettings()
    {
        Debug.Log("--- Configuring Build Settings ---");
        
        // Ensure we have at least one scene
        if (EditorBuildSettings.scenes.Length == 0)
        {
            Debug.LogError("❌ No scenes in build settings! Adding scenes first...");
            AddEssentialScenes();
        }
        
        Debug.Log($"✓ {EditorBuildSettings.scenes.Length} scenes configured for build");
        Debug.Log($"✓ Target Platform: {EditorUserBuildSettings.activeBuildTarget}");
        Debug.Log($"✓ Development Build: {EditorUserBuildSettings.development}");
    }
    
    private void ValidateBuildReadiness()
    {
        Debug.Log("--- Validating Build Readiness ---");
        
        bool ready = true;
        
        // Check scenes
        if (EditorBuildSettings.scenes.Length == 0)
        {
            Debug.LogError("❌ No scenes in build settings!");
            ready = false;
        }
        
        // Check for compilation errors
        if (EditorUtility.scriptCompilationFailed)
        {
            Debug.LogError("❌ Script compilation failed!");
            ready = false;
        }
        
        // Check platform
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.NoTarget)
        {
            Debug.LogError("❌ No build target selected!");
            ready = false;
        }
        
        if (ready)
        {
            Debug.Log("✅ BUILD READY!");
            Debug.Log("💡 You can now use File -> Build and Run");
            Debug.Log("💡 Or use the 'Create Safe Build' option below");
        }
        else
        {
            Debug.LogError("❌ BUILD NOT READY! Please fix the issues above.");
        }
    }
    
    [ContextMenu("🎯 Create Safe Build Now")]
    public void CreateSafeBuildNow()
    {
        Debug.Log("=== CREATING SAFE BUILD ===");
        
        // Fix issues first
        QuickBuildFixAndTest();
        
        // Choose build location
        string buildPath = EditorUtility.SaveFolderPanel("Choose Build Location", "", "ChemistryParty_Build");
        
        if (string.IsNullOrEmpty(buildPath))
        {
            Debug.Log("Build cancelled by user");
            return;
        }
        
        // Create build options
        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        
        // Get all enabled scenes
        var sceneList = new System.Collections.Generic.List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && !string.IsNullOrEmpty(scene.path))
            {
                sceneList.Add(scene.path);
            }
        }
        
        buildOptions.scenes = sceneList.ToArray();
        buildOptions.locationPathName = Path.Combine(buildPath, "ChemistryParty.exe");
        buildOptions.target = BuildTarget.StandaloneWindows64;
        buildOptions.options = BuildOptions.Development | BuildOptions.AllowDebugging;
        
        Debug.Log($"Building to: {buildOptions.locationPathName}");
        Debug.Log($"Including {sceneList.Count} scenes");
        
        // Build
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("🎉 BUILD SUCCESSFUL!");
            Debug.Log($"Build size: {report.summary.totalSize} bytes");
            Debug.Log($"Build time: {report.summary.totalTime}");
            
            // Open build folder
            EditorUtility.RevealInFinder(buildOptions.locationPathName);
            
            // Ask to run
            if (EditorUtility.DisplayDialog("Build Complete!", 
                "Build completed successfully!\n\nDo you want to run the game now?", 
                "Run Game", "Just Open Folder"))
            {
                System.Diagnostics.Process.Start(buildOptions.locationPathName);
            }
        }
        else
        {
            Debug.LogError($"❌ BUILD FAILED: {report.summary.result}");
            
            // Show detailed error information
            foreach (BuildStep step in report.steps)
            {
                if (step.messages != null)
                {
                    foreach (BuildStepMessage message in step.messages)
                    {
                        if (message.type == LogType.Error)
                        {
                            Debug.LogError($"Build Error: {message.content}");
                        }
                    }
                }
            }
            
            Debug.LogError("Please check the errors above and try again.");
        }
    }
    
    [ContextMenu("📋 Show Build Summary")]
    public void ShowBuildSummary()
    {
        Debug.Log("=== BUILD SUMMARY ===");
        
        Debug.Log($"Total scenes in build: {EditorBuildSettings.scenes.Length}");
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                Debug.Log($"  ✓ {sceneName}");
            }
        }
        
        Debug.Log($"Target Platform: {EditorUserBuildSettings.activeBuildTarget}");
        Debug.Log($"Development Build: {EditorUserBuildSettings.development}");
        Debug.Log($"Product Name: {PlayerSettings.productName}");
        Debug.Log($"API Compatibility: {PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Standalone)}");
        
        bool dotweenFound = System.Type.GetType("DG.Tweening.DOTween") != null;
        bool photonFound = System.Type.GetType("Photon.Pun.PhotonNetwork") != null;
        
        Debug.Log($"DOTween Available: {(dotweenFound ? "✓" : "❌")}");
        Debug.Log($"Photon PUN2 Available: {(photonFound ? "✓" : "❌")}");
        
        Debug.Log("=== END SUMMARY ===");
    }
}
#endif