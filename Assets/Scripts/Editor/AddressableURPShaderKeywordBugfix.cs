using System;
using UnityEditor;
using UnityEditor.Build;

public class AddressableURPShaderKeywordBugfix : BuildPlayerProcessor
{
    public override int callbackOrder => -1;


    // Test code
    public static bool UseBugfix = false;
    // - Test code

    public static void GatherShaderFeatures(bool isDevelopmentBuild)
    {
        var assembly = typeof(UnityEditor.Rendering.Universal.UniversalRenderPipelineAssetEditor).Assembly;
        var ShaderBuildPreprocessorType = assembly.GetType("UnityEditor.Rendering.Universal.ShaderBuildPreprocessor");
        var GatherShaderFeaturesMethod = ShaderBuildPreprocessorType.GetMethod("GatherShaderFeatures", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(bool) }, null);
        GatherShaderFeaturesMethod.Invoke(null, new object[] { isDevelopmentBuild });
    }

    public override void PrepareForBuild(BuildPlayerContext buildPlayerContext)
    {
        // Test code
        if (!UseBugfix)
            return;
        // - Test code

        bool isDevelopmentBuild = (buildPlayerContext.BuildPlayerOptions.options & BuildOptions.Development) != 0;
        GatherShaderFeatures(isDevelopmentBuild);
    }
}