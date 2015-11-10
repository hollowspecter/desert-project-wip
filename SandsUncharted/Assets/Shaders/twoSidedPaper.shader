Shader "Custom/twoSidedPaper" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FrontDrawTex("Front Drawing Texture", 2D) = "leave empty" {}
		_BackDrawTex("Back Drawing Texture", 2D) = "leave empty" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader 
		{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		
		sampler2D _MainTex;
		sampler2D _FrontDrawTex;
		sampler2D _BackDrawTex;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_FrontDrawTex;
			float2 uv_BackDrawTex;
			float3 worldNormal;
			float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			if (dot(IN.worldNormal, IN.viewDir) > 0)
			{
				// Albedo comes from a texture tinted by color
				float4 mainC = tex2D(_MainTex, IN.uv_MainTex);
				float4 drawC = tex2D(_FrontDrawTex, IN.uv_FrontDrawTex);
				//fixed4 c =  lerp(mainC, drawC, drawC.a) * _Color;
				fixed4 c = (mainC * (1 - drawC.a) + ((drawC + 0.3f * mainC) * drawC.a)) * _Color;
				o.Albedo = c.rgb * 0.5f;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic * drawC.a * 0.5f;
				o.Smoothness = _Glossiness * drawC.a * 0.5f;
				o.Alpha = c.a;
			}
			else
			{
				// Albedo comes from a texture tinted by color
				float4 mainC = tex2D(_MainTex, IN.uv_MainTex);
				float4 drawC = tex2D(_BackDrawTex, IN.uv_BackDrawTex);
				//fixed4 c =  lerp(mainC, drawC, drawC.a) * _Color;
				fixed4 c = (mainC * (1 - drawC.a) + ((drawC + 0.3f * mainC) * drawC.a)) * _Color;
				o.Albedo = c.rgb * 0.5f;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic * drawC.a * 0.5f;
				o.Smoothness = _Glossiness * drawC.a * 0.5f;
				o.Alpha = c.a;
			}
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
