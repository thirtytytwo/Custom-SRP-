Shader "Custom/Unlit"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white"{}
        _BaseColor("BaseColor", Color) = (1,1,1,1)
        _Cutoff("cut off", Range(0.0,1.0)) = 0.5
        [Toggle(_CLIPPING)] _Clipping ("Clip Switch", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "LightMode" = "SRPUnlit" }
        LOD 100
        Pass
        {
            //Blend SrcAlpha OneMinusSrcAlpha
            ZWrite off
            HLSLPROGRAM
            #pragma shader_feature _CLIPPING
            #pragma multi_compile_instancing//GPUInstancing
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            #include "Assets/ShaderLibrary/UnlitPass.hlsl"
            ENDHLSL
        }
    }
}
