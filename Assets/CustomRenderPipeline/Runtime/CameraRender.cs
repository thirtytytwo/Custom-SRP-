using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class CameraRender
{
    private static ShaderTagId unlitShaderTagId = new ShaderTagId("LYCTest");
    private static ShaderTagId[] legacyShaderTagIds = {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };

    private static Material errorMaterial;
    
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

        if (Cull())
        {
            return;
        }
        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedGeometry();
        Submit();
    }
    
    //
    void DrawVisibleGeometry()
    {
        SortingSettings sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
        DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        context.DrawSkybox(camera);

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    void DrawUnsupportedGeometry()
    {
        if (errorMaterial == null) errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        DrawingSettings drawingSettings = new DrawingSettings(legacyShaderTagIds[0], new SortingSettings(camera))
            { overrideMaterial = errorMaterial };
        for (int i = 0; i < legacyShaderTagIds.Length; i++)
        {
            drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
        }
        FilteringSettings filteringSettings = FilteringSettings.defaultValue;
        RenderStateBlock block = new RenderStateBlock(RenderStateMask.Depth);
        block.depthState = new DepthState(false, CompareFunction.GreaterEqual);
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings, ref block);
    }
    
    //
    void Setup()
    {
        context.SetupCameraProperties(camera);
        buffer.ClearRenderTarget(true,true, Color.clear);
        buffer.BeginSample(bufferName);
        ExecuteBuffer();
    }
    void Submit()
    {
        buffer.EndSample(bufferName);
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
