Shader "Custom/Unlit"
{
    Properties
    {
        _BaseColor("BaseColor", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode" = "LYCTest" }
        LOD 100
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite off
            HLSLPROGRAM
            #pragma multi_compile_instancing//GPUInstancing
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            #include "Assets/ShaderLibrary/UnlitPass.hlsl"
            ENDHLSL
        }
    }
}
