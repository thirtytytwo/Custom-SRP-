using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

partial class CameraRender
{
    partial void DrawGizmos();
    partial void DrawUnsupportedGeometry();
    partial void PrepareForSceneWindow();
    partial void PrepareBuffer();
#if UNITY_EDITOR
    private string sampleName { get; set; }
    private static ShaderTagId[] legacyShaderTagIds = {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };

    private static Material errorMaterial;

    partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
    }
    partial void DrawUnsupportedGeometry()
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
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    partial void PrepareBuffer()
    {
        buffer.name = sampleName = camera.name;
    }

    partial void PrepareForSceneWindow()
    {
        if (camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }
#else
    const string sampleName = bufferName;
#endif
}
