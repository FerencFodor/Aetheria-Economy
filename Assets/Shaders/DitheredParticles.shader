﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

Shader "Aetheria/Dithered Particles"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Dither Functions.cginc"
    
    struct appdata_particles {
        float4 vertex : POSITION;
        float4 color : COLOR;
        float4 texcoord : TEXCOORD0;
    };

    struct v2f
    {
        float4 position : SV_POSITION;
        float4 color : COLOR;
        float2 texcoord : TEXCOORD0;
        float2 ditherOffset : TEXCOORD1;
        float4 screenPos : TEXCOORD2;
    };

    sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _Color;
    float _Cutoff;

    v2f vert(appdata_particles v)
    {
        v2f o;
        o.position = UnityObjectToClipPos(v.vertex);
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
        o.ditherOffset = v.texcoord.zw;
        o.color = v.color;
        o.screenPos = ComputeScreenPos(o.position);
        return o;
    }

    float4 frag(v2f i) : COLOR
    {
        float4 c = tex2D(_MainTex, i.texcoord) * _Color * i.color;
        // Apply screen space dithering
		float2 screenUV = i.screenPos.xy / i.screenPos.w;
        ditherClip(screenUV + i.ditherOffset, c.a);
        return c;
    }
    
    float4 frag_shadow(v2f i) : COLOR
    {
        float4 c = tex2D(_MainTex, i.texcoord) * _Color.a * i.color.a;
        // Apply screen space dithering
		float2 screenUV = i.screenPos.xy / i.screenPos.w;
        ditherClip(screenUV + i.ditherOffset, c.a);
        SHADOW_CASTER_FRAGMENT(i)
    }

    ENDCG

    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_shadow
            ENDCG
        }
    } 
    FallBack "Diffuse"
}
