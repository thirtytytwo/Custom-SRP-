using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public partial class CameraRender
{
    private static ShaderTagId[] ShaderTagIds =
    {
        new ShaderTagId("SRPUnlit"),
        new ShaderTagId("SRPLit")
    };

    private ScriptableRenderContext context;
    private Camera camera;
    private Lighting Light = new Lighting();

    private bool IsEnableGPUInstancing;
    
    private const string bufferName = "Render Camera";
    private CommandBuffer buffer = new CommandBuffer { name = bufferName };
    
    //Settings
    private CullingResults cullingResults;
    
    public void Render(ScriptableRenderContext context, Camera camera, bool isEnableGPUInstancing)
    {
        this.camera = camera;
        this.context = context;
        this.IsEnableGPUInstancing = isEnableGPUInstancing;

        PrepareForSceneWindow();
        PrepareBuffer();
        if (Cull())
        {
            return;
        }
        Setup();
        Light.Setup(this.context, this.cullingResults);
        DrawVisibleGeometry();
        DrawUnsupportedGeometry();
        DrawGizmos();
        Submit();
    }
    
    //
    void DrawVisibleGeometry()
    {
        SortingSettings sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
        DrawingSettings drawingSettings = new DrawingSettings(ShaderTagIds[0], sortingSettings)
            { enableDynamicBatching = false, enableInstancing = this.IsEnableGPUInstancing };
        drawingSettings.SetShaderPassName(1, ShaderTagIds[1]);
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
