using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class CameraRender
{
    private ScriptableRenderContext context;
    private Camera camera;
    
    private const string bufferName = "Render Camera";
    private CommandBuffer buffer = new CommandBuffer { name = bufferName };
    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.camera = camera;
        this.context = context;
        
        Setup();
        DrawVisibleGeometry();
        Submit();
    }
    
    //
    void DrawVisibleGeometry()
    {
        context.DrawSkybox(camera);
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
}
