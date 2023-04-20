using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public partial class CameraRender
{
    private static ShaderTagId unlitShaderTagId = new ShaderTagId("LYCTest");

    private ScriptableRenderContext context;
    private Camera camera;
    
    private const string bufferName = "Render Camera";
    private CommandBuffer buffer = new CommandBuffer { name = bufferName };
    
    //Settings
    private CullingResults cullingResults;
    
    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.camera = camera;
        this.context = context;

        PrepareForSceneWindow();
        PrepareBuffer();
        if (Cull())
        {
            return;
        }
        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedGeometry();
        DrawGizmos();
        Submit();
    }
    
    //
    void DrawVisibleGeometry()
    {
        SortingSettings sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
        DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings)
            { enableDynamicBatching = false, enableInstancing = true };
        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        context.DrawSkybox(camera);

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    //
    void Setup()
    {
        context.SetupCameraProperties(camera);
        CameraClearFlags flags = camera.clearFlags;
        buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth,flags==CameraClearFlags.Color, flags==CameraClearFlags.Color ? camera.backgroundColor.linear : Color.clear);
        buffer.BeginSample(sampleName);
        ExecuteBuffer();
    }
    void Submit()
    {
        buffer.EndSample(sampleName);
        ExecuteBuffer();
        context.Submit();
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
    
    //
    bool Cull()
    {
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            cullingResults = context.Cull(ref p);
            return false;
        }

        return true;
    }
}
