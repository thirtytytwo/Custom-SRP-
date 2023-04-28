using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    private const string bufferName = "Lighting";

    private CommandBuffer buffer = new CommandBuffer { name = bufferName };

    private const int maxCount = 4;
    private static int
        dirLightColorID = Shader.PropertyToID("_DirectionalLightColor"),
        dirLightDirectionID = Shader.PropertyToID("_DirectionalLightDirection"),
        dirLightCountID = Shader.PropertyToID("_DirectionalLightCount");

    private static Vector4[]
        dirLightColors = new Vector4[maxCount],
        dirLightDirections = new Vector4[maxCount];

    private CullingResults _cullingResults;
    public void Setup(ScriptableRenderContext context, CullingResults cullingResults)
    {
        _cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        SetupLights();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    void SetupDirectionalLight(int index, ref VisibleLight light)
    {
        dirLightColors[index] = light.finalColor;
        dirLightDirections[index] = -light.localToWorldMatrix.GetColumn(2);
        //Debug.Log(light.localToWorldMatrix);
    }

    void SetupLights()
    {
        NativeArray<VisibleLight> visibleLights = _cullingResults.visibleLights;
        int dirLightCount = 0;
        for (int i = 0; i < visibleLights.Length; i++)
        {
            if(visibleLights[i].lightType != LightType.Directional) continue;
            VisibleLight light = visibleLights[i];
            SetupDirectionalLight(dirLightCount++, ref light);
            if(dirLightCount >= maxCount) break;
        }
        
        buffer.SetGlobalInt(dirLightCountID, visibleLights.Length);
        buffer.SetGlobalVectorArray(dirLightColorID, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionID, dirLightDirections);
    }
}
