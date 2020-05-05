﻿Shader "Blend Textures Hard" { 
 
Properties {
        _MainTex ("Input Render Texture", 2D) = "black" {}
        _BTex ("Output Render Texture", 2D) = "black" {}
        _Intensity ("Intensity", Range (0,1)) = 0.5
    }
    SubShader {
 
    // No culling or depth
        Cull Off ZWrite Off ZTest Always
 
        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            uniform sampler2D _MainTex;
            uniform sampler2D _BTex;
            uniform float _Intensity;
 
            float4 frag(v2f_img i) : COLOR{
                float4 c1 = tex2D(_MainTex, i.uv);
                i.uv.y = i.uv.y;
                float4 c2 = tex2D(_BTex, i.uv);
                float4 c3 = lerp(c2, c1, _Intensity);
 
                return c3;
            }
            ENDCG
        }
    }
 
}