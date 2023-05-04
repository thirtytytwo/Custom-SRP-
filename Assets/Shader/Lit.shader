Shader "Custom/Lit"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white"{}
        _BaseColor("BaseColor", Color) = (1,1,1,1)
    }
    SubShader
    {
        //blend One OneMinusSrcAlpha
        Tags { "RenderType"="Opaque" "LightMode" = "SRPLit" }
        LOD 100
        Pass
        {
            HLSLPROGRAM
            #pragma multi_compile_instancing//GPUInstancing
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Assets/ShaderLibrary/LitPass.hlsl"
            ENDHLSL
        }
		Pass {
			Tags {
				"LightMode" = "ShadowCaster"
			}

			ColorMask 0

			HLSLPROGRAM
			#pragma target 3.5
			#pragma multi_compile_instancing
			#pragma vertex ShadowCasterPassVertex
			#pragma fragment ShadowCasterPassFragment
			#include "Assets/ShaderLibrary/ShadowCasterPass.hlsl"
			ENDHLSL
		}
    }
}
