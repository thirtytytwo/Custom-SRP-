using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    private CameraRender renderer = new CameraRender();
    private bool isEnableGPUInstancing;
    public CustomRenderPipeline(bool isEnableSRPBatch, bool isEnableGPUInstancing)
    {
        GraphicsSettings.useScriptableRenderPipelineBatching = isEnableSRPBatch;
        this.isEnableGPUInstancing = isEnableGPUInstancing;
        GraphicsSettings.lightsUseLinearIntensity = true;
    }
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var cam in cameras)
        {
            renderer.Render(context, cam, isEnableGPUInstancing);
        }
    }
}
