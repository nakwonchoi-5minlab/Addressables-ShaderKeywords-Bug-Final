# How to reproduce

## Can build/run android

- Open editor and project
- On menu, click 'Bug Test' - 'Not Work Build'
- After build, install 'Build/android_il2cpp/build.apk' and run

## Cannot build/run android

- Open editor and project
- On menu, click 'Bug Test' - Not Work' and 'Addressable Clean All'
- On menu, click 'File' - 'Build setting'
- Select platform
- click Build option (triangle right of Build button) and 'Clean Build'
- Install and run

## Expect

- The color of right cube should turn from gray to green
  <img width="201" alt="image" src="https://github.com/nakwonchoi-5minlab/SBP-ShaderKeywords-Bug-Final/assets/106501566/34e0a5b8-516d-4093-a0b1-a200da2a625e">
  to
  <img width="200" alt="image" src="https://github.com/nakwonchoi-5minlab/SBP-ShaderKeywords-Bug-Final/assets/106501566/63ddb73e-b060-48d1-a44b-1cce69478d65">

## Actual

- The color of right cube will turn from gray to magenta (fail to load shader)
  <img width="278" alt="image" src="https://github.com/nakwonchoi-5minlab/SBP-ShaderKeywords-Bug-Final/assets/106501566/0056ee0b-369e-4df0-8a3c-a4d357998e86">

# Why it does happen

## When build player

- first, process 'Addressable Content' using [BuildPlayerProcessor.PrepareForBuild](https://docs.unity3d.com/ScriptReference/Build.BuildPlayerProcessor.PrepareForBuild.html) callback
  - process shader in Addressable [log](etc/AddressablesPlayerBuildProcessor.PrepareForBuild.log)
- second, process 'Player'
  - process shaders in resources, sharedAssets (assets in scene) [log](etc/UnityEditor.BuildPipeline.BuildPlayerInternalNoCheck.log)

## When Engine Strip Shaderkeyword

- First, generate keywords set (=ShaderCompilerData list) using [ShaderKeywordFilter.FilterAttribute](https://docs.unity3d.com/ScriptReference/ShaderKeywordFilter.FilterAttribute.html)
  - In URP, in `Library/PackageCache/com.unity.render-pipelines.universal@14.0.11/Runtime/Data/UniversalRenderPipelineAssetPrefiltering.cs`

```csharp
        // User can change cascade count at runtime so we have to include both MainLightShadows and MainLightShadowCascades.
        // ScreenSpaceShadows renderer feature has separate filter attribute for keeping MainLightShadowScreen.
        // NOTE: off variants are atm always removed when shadows are supported
        [ShaderKeywordFilter.RemoveIf(PrefilteringModeMainLightShadows.Remove,                     keywordNames: new [] {ShaderKeywordStrings.MainLightShadows, ShaderKeywordStrings.MainLightShadowCascades})]
        [ShaderKeywordFilter.SelectIf(PrefilteringModeMainLightShadows.SelectMainLight,            keywordNames: ShaderKeywordStrings.MainLightShadows)]
        [ShaderKeywordFilter.SelectIf(PrefilteringModeMainLightShadows.SelectMainLightAndOff,      keywordNames: new [] {"", ShaderKeywordStrings.MainLightShadows})]
        [ShaderKeywordFilter.SelectIf(PrefilteringModeMainLightShadows.SelectMainLightAndCascades, keywordNames: new [] {ShaderKeywordStrings.MainLightShadows, ShaderKeywordStrings.MainLightShadowCascades})]
        [ShaderKeywordFilter.SelectIf(PrefilteringModeMainLightShadows.SelectAll,                  keywordNames: new [] {"", ShaderKeywordStrings.MainLightShadows, ShaderKeywordStrings.MainLightShadowCascades})]
        [SerializeField] private PrefilteringModeMainLightShadows m_PrefilteringModeMainLightShadows = PrefilteringModeMainLightShadows.SelectMainLight;
```

- Second, modify keywords set (=ShaderCompilerData list) using [IPreprocessShaders.OnProcessShader](https://docs.unity3d.com/ScriptReference/Build.IPreprocessShaders.OnProcessShader.html)
  - In URP, in `Library/PackageCache/com.unity.render-pipelines.core@14.0.11/Editor/ShaderStripping/ShaderPreprocessor.cs`
  - And `CanRemoveVariant` callback in `Library/PackageCache/com.unity.render-pipelines.universal@14.0.11/Editor/ShaderScriptableStripper.cs`

# When `UniversalRenderPipelineAssetPrefiltering` updated

`UniversalRenderPipelineAsset.UpdateShaderKeywordPrefiltering` called

```
Void UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset:UpdateShaderKeywordPrefiltering (ShaderPrefilteringData) (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.universal@14.0.11/Runtime/Data/UniversalRenderPipelineAssetPrefiltering.cs:227)
Void UnityEditor.Rendering.Universal.ShaderBuildPreprocessor:GetSupportedShaderFeaturesFromAssets (List`1, List`1, Boolean) (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.universal@14.0.11/Editor/ShaderBuildPreprocessor.cs:358)
Void UnityEditor.Rendering.Universal.ShaderBuildPreprocessor:GatherShaderFeatures (Boolean) (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.universal@14.0.11/Editor/ShaderBuildPreprocessor.cs:238)
```

## First `IpreprocessShaders.OnProcessShader` called

```
Void UnityEditor.Rendering.Universal.ShaderBuildPreprocessor:GatherShaderFeatures (Boolean) (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.universal@14.0.11/Editor/ShaderBuildPreprocessor.cs:238)
List`1 UnityEditor.Rendering.Universal.ShaderBuildPreprocessor:get_supportedFeaturesList () (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.universal@14.0.11/Editor/ShaderBuildPreprocessor.cs:112)
Boolean UnityEditor.Rendering.Universal.ShaderScriptableStripper:CanRemoveVariant (Shader, ShaderSnippetData, ShaderCompilerData) (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.universal@14.0.11/Editor/ShaderScriptableStripper.cs:1080)
Boolean <>c__DisplayClass15_0:<CanRemoveVariant>b__1 (IVariantStripper`2) (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.core@14.0.11/Editor/ShaderStripping/ShaderPreprocessor.cs:85)
Boolean System.Linq.Enumerable:All (IEnumerable`1, Func`2) (알 수 없는 소스:0)
Boolean UnityEditor.Rendering.ShaderPreprocessor`2:CanRemoveVariant (Shader, ShaderSnippetData, ShaderCompilerData) (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.core@14.0.11/Editor/ShaderStripping/ShaderPreprocessor.cs:83)
Boolean UnityEditor.Rendering.ShaderPreprocessor`2:TryStripShaderVariants (Shader, ShaderSnippetData, IList`1, Exception) (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.core@14.0.11/Editor/ShaderStripping/ShaderPreprocessor.cs:137)
Void UnityEditor.Rendering.ShaderVariantStripper:OnProcessShader (Shader, ShaderSnippetData, IList`1) (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.core@14.0.11/Editor/ShaderStripping/ShaderPreprocessor.cs:169)
ShaderCompilerData[] UnityEditor.Build.BuildPipelineInterfaces:OnPreprocessShaders (Shader, ShaderSnippetData, ShaderCompilerData[]) (알 수 없는 소스:0)
```

## `IPreprocessBuildWithReport.OnPreprocessBuild` called

```
Void UnityEditor.Rendering.Universal.ShaderBuildPreprocessor:GatherShaderFeatures (Boolean) (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.universal@14.0.11/Editor/ShaderBuildPreprocessor.cs:223)
Void UnityEditor.Rendering.Universal.ShaderBuildPreprocessor:OnPreprocessBuild (BuildReport) (/Users/5minlab/Project/SBP-ShaderKeywords-Bug-Final/Library/PackageCache/com.unity.render-pipelines.universal@14.0.11/Editor/ShaderBuildPreprocessor.cs:207)
```

## Summary

- Create new URP Asset
- Modifty options
- generate keywords set for Addressable Content build from _Prefiltering Not Updated_ URP Asset
  - Already wrong ShaderCompilerData list
- `ShaderPreprocessor.OnProcessShader` (URP code) called for Addressable contents stripping
- `UniversalRenderPipelineAsset.UpdateShaderKeywordPrefiltering` called
  - _Prefiltering Updated_ URP Asset
- generate keywords set for Player build from _Prefiltering Updated_ URP Asset
- `ShaderPreprocessor.OnProcessShader` called for Player build assets stripping

Shader in Addressable has problem and in Player work correctly.

## Suggestion

[Assets/Scripts/Editor/AddressableURPShaderKeywordBugfix.cs](Assets/Scripts/Editor/AddressableURPShaderKeywordBugfix.cs)
