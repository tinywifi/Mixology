using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class BuildDiagnostics : MonoBehaviour
{
    [ContextMenu("1. Check Build Issues")]
    public void CheckBuildIssues()
    {
        Debug.Log("=== BUILD DIAGNOSTICS ===");
        
        // Check scenes in build settings
        CheckScenesInBuild();
        
        // Check for missing references
        CheckMissingReferences();
        
        // Check for compilation errors
        CheckCompilationErrors();
        
        // Check required components
        CheckRequiredComponents();
        
        // Check build settings
        CheckBuildSettings();
        
        Debug.Log("=== BUILD DIAGNOSTICS COMPLETE ===");
    }
    
    [ContextMenu("2. Fix Common Build Issues")]
    public void FixCommonBuildIssues()
    {
        Debug.Log("=== FIXING COMMON BUILD ISSUES ===");
        
        // Add scenes to build settings if missing
        FixScenesInBuild();
        
        // Remove problematic editor-only code
        FixEditorOnlyIssues();
        
        // Set proper build settings
        FixBuildSettings();
        
        Debug.Log("=== BUILD FIXES COMPLETE ===");
    }
    
    private void CheckScenesInBuild()
    {
        Debug.Log("--- Checking Scenes in Build ---");
        
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        Debug.Log($"Total scenes in build: {scenes.Length}");
        
        for (int i = 0; i < scenes.Length; i++)
        {
            EditorBuildSettingsScene scene = scenes[i];
            Debug.Log($"Scene {i}: {scene.path} (Enabled: {scene.enabled})");
            
            if (!File.Exists(scene.path))
            {
                Debug.LogError($"❌ Scene file missing: {scene.path}");
            }
        }
        
        // Check for common scene names
        string[] requiredScenes = { "CardGame-Lobby", "CardGame-Board" };
        foreach (string sceneName in requiredScenes)
        {
            bool found = false;
            foreach (var scene in scenes)
            {
                if (scene.path.Contains(sceneName))
                {
                    found = true;
                    break;
                }
            }
            
            if (!found)
            {
                Debug.LogWarning($"⚠️ Required scene not found in build: {sceneName}");
            }
        }
    }
    
    private void CheckMissingReferences()
    {
        Debug.Log("--- Checking Missing References ---");
        
        // Check for missing script references
        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
        int missingCount = 0;
        
        foreach (MonoBehaviour mb in allMonoBehaviours)
        {
            if (mb == null)
            {
                missingCount++;
            }
        }
        
        if (missingCount > 0)
        {
            Debug.LogError($"❌ Found {missingCount} missing script references!");
        }
        else
        {
            Debug.Log("✓ No missing script references found");
        }
    }
    
    private void CheckCompilationErrors()
    {
        Debug.Log("--- Checking Compilation ---");
        
        // Force recompilation
        AssetDatabase.Refresh();
        
        // Check if there are compilation errors
        if (EditorUtility.scriptCompilationFailed)
        {
            Debug.LogError("❌ Script compilation failed! Fix compilation errors before building.");
        }
        else
        {
            Debug.Log("✓ No compilation errors detected");
        }
    }
    
    private void CheckRequiredComponents()
    {
        Debug.Log("--- Checking Required Components ---");
        
        // Check for DOTween
        bool dotweenFound = System.Type.GetType("DG.Tweening.DOTween") != null;
        Debug.Log($"DOTween available: {dotweenFound}");
        
        // Check for Photon
        bool photonFound = System.Type.GetType("Photon.Pun.PhotonNetwork") != null;
        Debug.Log($"Photon PUN2 available: {photonFound}");
        
        if (!dotweenFound)
        {
            Debug.LogWarning("⚠️ DOTween not found - animations may not work");
        }
        
        if (!photonFound)
        {
            Debug.LogWarning("⚠️ Photon PUN2 not found - multiplayer may not work");
        }
    }
    
    private void CheckBuildSettings()
    {
        Debug.Log("--- Checking Build Settings ---");
        
        Debug.Log($"Target Platform: {EditorUserBuildSettings.activeBuildTarget}");
        Debug.Log($"Development Build: {EditorUserBuildSettings.development}");
        Debug.Log($"Script Debugging: {EditorUserBuildSettings.allowDebugging}");
        Debug.Log($"Auto-connect Profiler: {EditorUserBuildSettings.connectProfiler}");
        
        // Check player settings
        Debug.Log($"Company Name: {PlayerSettings.companyName}");
        Debug.Log($"Product Name: {PlayerSettings.productName}");
        Debug.Log($"API Compatibility: {PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup)}");
    }
    
    private void FixScenesInBuild()
    {
        Debug.Log("--- Adding Scenes to Build ---");
        
        // Find scene files
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        System.Collections.Generic.List<EditorBuildSettingsScene> buildScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        
        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = Path.GetFileNameWithoutExtension(path);
            
            // Add important scenes
            if (sceneName.Contains("CardGame") || sceneName.Contains("Lobby") || sceneName.Contains("Board"))
            {
                EditorBuildSettingsScene buildScene = new EditorBuildSettingsScene(path, true);
                buildScenes.Add(buildScene);
                Debug.Log($"✓ Added scene to build: {sceneName}");
            }
        }
        
        if (buildScenes.Count > 0)
        {
            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log($"✓ Updated build settings with {buildScenes.Count} scenes");
        }
    }
    
    private void FixEditorOnlyIssues()
    {
        Debug.Log("--- Checking Editor-Only Code ---");
        
        // Check for common editor-only issues
        string[] problematicPaths = {
            "Assets/Scripts/CardGame/ModernUISetup.cs",
            "Assets/Scripts/CardGame/EnhancedLobbyUI.cs",
            "Assets/Scripts/CardGame/EnhancedGameBoardUI.cs",
            "Assets/Scripts/CardGame/SimpleLobbyTest.cs"
        };
        
        foreach (string path in problematicPaths)
        {
            if (File.Exists(path))
            {
                string content = File.ReadAllText(path);
                
                // Check if properly wrapped in UNITY_EDITOR
                if (content.Contains("DestroyImmediate") && !content.Contains("#if UNITY_EDITOR"))
                {
                    Debug.LogWarning($"⚠️ {path} may contain editor-only code not properly wrapped");
                }
                else
                {
                    Debug.Log($"✓ {path} appears to be properly wrapped for builds");
                }
            }
        }
    }
    
    private void FixBuildSettings()
    {
        Debug.Log("--- Optimizing Build Settings ---");
        
        // Set safer build settings
        EditorUserBuildSettings.development = true; // Enable for debugging
        EditorUserBuildSettings.allowDebugging = true;
        
        // Set API compatibility to .NET Standard 2.0 if available
        PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_Standard_2_0);
        
        // Ensure proper settings
        PlayerSettings.runInBackground = true;
        
        Debug.Log("✓ Build settings optimized for debugging");
    }
    
    [ContextMenu("3. Create Safe Build")]
    public void CreateSafeBuild()
    {
        Debug.Log("=== CREATING SAFE BUILD ===");
        
        // Fix issues first
        FixCommonBuildIssues();
        
        // Set build location
        string buildPath = EditorUtility.SaveFolderPanel("Choose Build Location", "", "ChemistryPartyBuild");
        
        if (!string.IsNullOrEmpty(buildPath))
        {
            // Create build options
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            
            // Collect all enabled scenes
            var sceneList = new System.Collections.Generic.List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    sceneList.Add(scene.path);
                }
            }
            
            buildOptions.scenes = sceneList.ToArray();
            
            buildOptions.locationPathName = Path.Combine(buildPath, PlayerSettings.productName + ".exe");
            buildOptions.target = BuildTarget.StandaloneWindows64;
            buildOptions.options = BuildOptions.Development | BuildOptions.AllowDebugging;
            
            Debug.Log($"Building to: {buildOptions.locationPathName}");
            Debug.Log($"Scenes: {string.Join(", ", buildOptions.scenes)}");
            
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("✓ Build completed successfully!");
                
                // Open build folder
                EditorUtility.RevealInFinder(buildOptions.locationPathName);
            }
            else
            {
                Debug.LogError($"❌ Build failed: {report.summary.result}");
                
                foreach (BuildStep step in report.steps)
                {
                    foreach (BuildStepMessage message in step.messages)
                    {
                        if (message.type == LogType.Error || message.type == LogType.Exception)
                        {
                            Debug.LogError($"Build Error: {message.content}");
                        }
                    }
                }
            }
        }
    }
}
#endif