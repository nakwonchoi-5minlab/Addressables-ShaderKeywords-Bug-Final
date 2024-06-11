using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;

public class PreprocessShaders : IPreprocessShaders
{
    public int callbackOrder => -1;

    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
    {
        if (shader.name == "Unlit/NewUnlitShader")
        {
            Debug.Log($"PreprocessShaders.OnProcessShader: {System.Environment.StackTrace}");
            var jsonStr = JsonConvert.SerializeObject(data.Select(x => x.shaderKeywordSet.GetShaderKeywords().Select(y => y.name).ToList()).ToList());
            Debug.Log(jsonStr);
            GetShaderProcessors();
            return;
        }
    }

    static void GetShaderProcessors()
    {
        var assembly = typeof(IOrderedCallback).Assembly;
        var type = assembly.GetType("UnityEditor.Build.BuildPipelineInterfaces");
        var m_ProcessorsField = type.GetField("m_Processors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var processors = m_ProcessorsField.GetValue(null);
        var processorsType = processors.GetType();
        var shaderProcessorsField = processorsType.GetField("shaderProcessors", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        IList<IPreprocessShaders> shaderProcessors = (IList<IPreprocessShaders>)shaderProcessorsField.GetValue(processors);
        Debug.Log($"IList<IPreprocessShaders>: \n{string.Join("\n", shaderProcessors.Select(x => $"[{x.callbackOrder}] {x.GetType().FullName} {x.GetType().AssemblyQualifiedName}"))}");
    }
}