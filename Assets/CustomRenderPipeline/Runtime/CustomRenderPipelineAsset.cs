using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
public class CustomRenderPipelineAsset : RenderPipelineAsset
{
    public bool isEnableSRPBatch, isEnableGPUInstancing;
    [SerializeField] private ShadowSetting shadows = default;
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline(isEnableSRPBatch, isEnableGPUInstancing, shadows);
    }
}
