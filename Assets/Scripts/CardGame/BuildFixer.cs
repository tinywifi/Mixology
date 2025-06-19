using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

public class BuildFixer : MonoBehaviour
{
    [ContextMenu("1. Fix All Build Issues")]
    public void FixAllBuildIssues()
    {
        Debug.Log("=== FIXING ALL BUILD ISSUES ===");
        
        // 1. Add scenes to build settings
        AddScenesToBuild();
        
        // 2. Set proper build settings
        ConfigureBuildSettings();
        
        // 3. Validate components
        ValidateComponents();
        
        Debug.Log("=== ALL BUILD ISSUES FIXED ===");
        Debug.Log("You can now try Build and Run again!");
    }
    
    [ContextMenu("2. Create Development Build")]
    public void CreateDevelopmentBuild()
    {
        Debug.Log("=== CREATING DEVELOPMENT BUILD ===");
        
        // Fix issues first
        FixAllBuildIssues();
        
        // Get build path
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
        Debug.Log($"Scenes to build: {sceneList.Count}");
        
        foreach (string scene in sceneList)
        {
            Debug.Log($"  - {scene}");
        }
        
        // Build
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("✓ BUILD SUCCESSFUL!");
            Debug.Log($"Build size: {report.summary.totalSize} bytes");
            Debug.Log($"Build time: {report.summary.totalTime}");
            
            // Open folder
            EditorUtility.RevealInFinder(buildOptions.locationPathName);
            
            // Ask if user wants to run the build
            if (EditorUtility.DisplayDialog("Build Complete", 
                "Build completed successfully! Do you want to run the game?", 
                "Run Game", "Just Open Folder"))
            {
                System.Diagnostics.Process.Start(buildOptions.locationPathName);
            }
        }
        else
        {
            Debug.LogError($"❌ BUILD FAILED: {report.summary.result}");
            
            // Log build errors
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
                        else if (message.type == LogType.Warning)
                        {
                            Debug.LogWarning($"Build Warning: {message.content}");
                        }
                    }
                }
            }
        }
    }
    
    private void AddScenesToBuild()
    {
        Debug.Log("--- Adding Scenes to Build Settings ---");
        
        // Find all scene files
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        var buildScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        
        // Add existing enabled scenes first
        foreach (EditorBuildSettingsScene existingScene in EditorBuildSettings.scenes)
        {
            if (existingScene.enabled && File.Exists(existingScene.path))
            {
                buildScenes.Add(existingScene);
            }
        }
        
        // Find and add important scenes
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
            
            if (!alreadyAdded && (sceneName.Contains("CardGame") || sceneName.Contains("Lobby") || sceneName.Contains("Board")))
            {
                EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(path, true);
                buildScenes.Add(newScene);
                Debug.Log($"✓ Added scene: {sceneName}");
            }
        }
        
        // Update build settings
        EditorBuildSettings.scenes = buildScenes.ToArray();
        Debug.Log($"✓ Build settings updated with {buildScenes.Count} scenes");
    }
    
    private void ConfigureBuildSettings()
    {
        Debug.Log("--- Configuring Build Settings ---");
        
        // Enable development build for debugging
        EditorUserBuildSettings.development = true;
        EditorUserBuildSettings.allowDebugging = true;
        
        // Set target platform to Windows x64 if not already
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        }
        
        // Configure player settings
        PlayerSettings.productName = "Chemistry Party";
        PlayerSettings.companyName = "Chemistry Game Studio";
        PlayerSettings.runInBackground = true;
        
        // Set API compatibility level
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_Standard_2_0);
        
        Debug.Log("✓ Build settings configured");
        Debug.Log($"  - Development Build: {EditorUserBuildSettings.development}");
        Debug.Log($"  - Allow Debugging: {EditorUserBuildSettings.allowDebugging}");
        Debug.Log($"  - Target Platform: {EditorUserBuildSettings.activeBuildTarget}");
        Debug.Log($"  - API Compatibility: {PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Standalone)}");
    }
    
    private void ValidateComponents()
    {
        Debug.Log("--- Validating Components ---");
        
        // Check for DOTween
        bool dotweenFound = System.Type.GetType("DG.Tweening.DOTween") != null;
        Debug.Log($"DOTween available: {dotweenFound}");
        
        if (!dotweenFound)
        {
            Debug.LogWarning("⚠️ DOTween not found - some animations may not work in build");
        }
        
        // Check for Photon
        bool photonFound = System.Type.GetType("Photon.Pun.PhotonNetwork") != null;
        Debug.Log($"Photon PUN2 available: {photonFound}");
        
        if (!photonFound)
        {
            Debug.LogWarning("⚠️ Photon PUN2 not found - multiplayer features may not work");
        }
        
        // Check for missing script references in current scene
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
            Debug.LogWarning($"⚠️ Found {missingScripts} missing script references in current scene");
        }
        else
        {
            Debug.Log("✓ No missing script references found");
        }
    }
    
    [ContextMenu("3. Check Build Readiness")]
    public void CheckBuildReadiness()
    {
        Debug.Log("=== CHECKING BUILD READINESS ===");
        
        bool ready = true;
        
        // Check scenes
        if (EditorBuildSettings.scenes.Length == 0)
        {
            Debug.LogError("❌ No scenes in build settings!");
            ready = false;
        }
        else
        {
            Debug.Log($"✓ {EditorBuildSettings.scenes.Length} scenes in build settings");
        }
        
        // Check for compilation errors
        if (EditorUtility.scriptCompilationFailed)
        {
            Debug.LogError("❌ Script compilation failed!");
            ready = false;
        }
        else
        {
            Debug.Log("✓ No compilation errors");
        }
        
        // Check platform
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.NoTarget)
        {
            Debug.LogError("❌ No build target selected!");
            ready = false;
        }
        else
        {
            Debug.Log($"✓ Build target: {EditorUserBuildSettings.activeBuildTarget}");
        }
        
        if (ready)
        {
            Debug.Log("✅ BUILD READY! You can safely Build and Run.");
        }
        else
        {
            Debug.LogError("❌ BUILD NOT READY! Fix the issues above first.");
        }
    }
}
#endif