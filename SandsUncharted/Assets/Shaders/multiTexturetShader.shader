﻿Shader "Custom/multiTexture" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Background Texture", 2D) = "white" {}
		_DrawTex ("Drawing Texture", 2D) = "drawing" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _DrawTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_DrawTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float4 mainC = tex2D (_MainTex, IN.uv_MainTex);
			float4 drawC = tex2D (_DrawTex, IN.uv_DrawTex);
			//fixed4 c =  lerp(mainC, drawC, drawC.a) * _Color;
			fixed4 c =  (mainC * (1-drawC.a) + ((drawC + 0.3f * mainC) * drawC.a)) * _Color;
			o.Albedo = c.rgb * 0.5f;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic * drawC.a;
			o.Smoothness = _Glossiness * drawC.a;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}