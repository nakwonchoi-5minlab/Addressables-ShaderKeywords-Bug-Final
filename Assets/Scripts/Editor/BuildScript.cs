using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class BuildScript
{
    [MenuItem("Bug Test/Make Not Work")]
    public static void MakeNotWork()
    {
        using (ListPool<UniversalRenderPipelineAsset>.Get(out List<UniversalRenderPipelineAsset> urpAssets))
        {
            bool success = EditorUserBuildSettings.activeBuildTarget.TryGetRenderPipelineAssets(urpAssets);
            if (!success)
            {
                Debug.LogError("Unable to get UniversalRenderPipelineAssets from EditorUserBuildSettings.activeBuildTarget.");
                return;
            }

            var m_PrefilteringModeForwardPlusField = typeof(UniversalRenderPipelineAsset).GetField("m_PrefilteringModeMainLightShadows", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            for (int urpAssetIndex = 0; urpAssetIndex < urpAssets.Count; urpAssetIndex++)
            {
                UniversalRenderPipelineAsset urpAsset = urpAssets[urpAssetIndex];
                if (urpAsset == null)
                    continue;

                m_PrefilteringModeForwardPlusField.SetValue(urpAsset, 1);

                EditorUtility.SetDirty(urpAsset);
            }

            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("Bug Test/Make Not Work Build")]
    public static void MakeNotWorkBuild()
    {
        MakeNotWork();

        AndroidIL2CPP();
    }

    [MenuItem("Bug Test/Make Work")]
    public static void MakeWork()
    {
        AddressableURPShaderKeywordBugfix.GatherShaderFeatures(false);

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Bug Test/Make Work Build")]
    public static void MakeWorkBuild()
    {
        MakeWork();

        AndroidIL2CPP();
    }



    [MenuItem("Bug Test/AndroidIL2CPP")]
    public static void AndroidIL2CPP()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)33;
        EditorUserBuildSettings.buildAppBundle = false;
        PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.Android, Il2CppCodeGeneration.OptimizeSize);

        OnCleanAll();

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions()
        {
            scenes = EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path).ToArray(),
            locationPathName = "Build/android_il2cpp/build.apk",
            target = BuildTarget.Android,
            options = BuildOptions.CleanBuildCache,
        });
        if (Application.isBatchMode)
        {
            EditorApplication.Exit(0);
        }
    }

    [MenuItem("Bug Test/Addressable Clean All")]
    static void OnCleanAll()
    {
        void OnCleanAddressables(object builder)
        {
            UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.CleanPlayerContent(builder as UnityEditor.AddressableAssets.Build.IDataBuilder);
        }

        void OnCleanSBP(object prompt)
        {
            UnityEditor.Build.Pipeline.Utilities.BuildCache.PurgeCache((bool)prompt);
        }

        OnCleanAddressables(null);
        OnCleanSBP(false);
    }
}