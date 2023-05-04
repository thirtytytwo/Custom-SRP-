using UnityEngine;
using UnityEngine.Rendering;

struct ShadowedDirectionalLights
{
    public int visibleLightIndex;
}
public class Shadows
{
    private const string bufferName = "Shadows";
    private CommandBuffer buffer = new CommandBuffer { name = bufferName };
    private ScriptableRenderContext context;
    private CullingResults cullingResults;
    private ShadowSetting shadowSetting;

    private const int maxShadowedDirectionalLightCount = 1;
    private int shadowedDirectionalLightCount;

    private static int dirShadowAtlasID = Shader.PropertyToID("_DirectionalShaderAtlas");

    private ShadowedDirectionalLights[] shadowedDirectionalLights =
        new ShadowedDirectionalLights[maxShadowedDirectionalLightCount];
    public void Setup(ScriptableRenderContext context, CullingResults cullingResults, ShadowSetting shadowSetting)
    {
        this.context = context;
        this.cullingResults = cullingResults;
        this.shadowSetting = shadowSetting;

        shadowedDirectionalLightCount = 0;
    }

    public void ReserveDirectionalShadows(Light light, int visibleLightIndex)
    {
        if (shadowedDirectionalLightCount < maxShadowedDirectionalLightCount && light.shadows != LightShadows.None && light.shadowStrength > 0f && cullingResults.GetShadowCasterBounds(visibleLightIndex, out Bounds b))
        {
            shadowedDirectionalLights[shadowedDirectionalLightCount++] = new ShadowedDirectionalLights
                { visibleLightIndex = visibleLightIndex };
        }
        else
        {
            buffer.GetTemporaryRT(dirShadowAtlasID, 1, 1, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
        }
    }

    public void Render()
    {
        if (shadowedDirectionalLightCount > 0)
        {
            RenderDirectionalShadows();
        }
    }

    void RenderDirectionalShadows()
    {
        int atlasSize = (int)shadowSetting.directional.atlasSize;
        buffer.GetTemporaryRT(dirShadowAtlasID, atlasSize, atlasSize, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
        buffer.SetRenderTarget(dirShadowAtlasID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        buffer.ClearRenderTarget(true, false, Color.clear);
        for (int i = 0; i < shadowedDirectionalLightCount; i++)
        {
            RenderDirectionalShadows(i, atlasSize);
        }
        ExecuteBuffer();
    }

    void RenderDirectionalShadows(int index, int tileSize)
    {
        ShadowedDirectionalLights lighs = shadowedDirectionalLights[index];
        var shadowSetting = new ShadowDrawingSettings(cullingResults, lighs.visibleLightIndex);
        cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(lighs.visibleLightIndex, 0, 1, Vector3.zero,
            tileSize, 0f, out Matrix4x4 viewMatrix, out Matrix4x4 projectionMatrix, out ShadowSplitData splitData);
        shadowSetting.splitData = splitData;
        buffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
        ExecuteBuffer();
        context.DrawShadows(ref shadowSetting);
    }
    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    public void Cleanup()
    {
        buffer.ReleaseTemporaryRT(dirShadowAtlasID);
        ExecuteBuffer();
    }
}
