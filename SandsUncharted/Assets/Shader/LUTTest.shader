Shader "TriplanarTutorial/LUTTest" {
	Properties {
		_SandDiffuse ("Sand Diffuse ", 2D)  = "white" {}
		_RockDiffuse ("Rock Diffuse ", 2D)  = "white" {}
		_LUT ("Lookup Table ", 2D)  = "white" {}
		_TextureScale ("Texture Scale",float) = 1
		_LUTmiplevel ("LUT Mip Level",float) = 1
		_TriplanarBlendSharpness ("Blend Sharpness",float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0

		sampler2D _SandDiffuse;
		sampler2D _RockDiffuse;
		sampler2D _LUT;
		float _TextureScale;
		float _LUTmiplevel;
		float _TriplanarBlendSharpness;

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			// Lookup what Texture to use
			float lookupU =  (dot( IN.worldNormal , half3(0.0, 1.0, 0.0) ) + 1.0 ) / 2.0;
			lookupU = 1 - lookupU;
			float lookupV = IN.worldPos.y / 96.0;
			half4 lookupVector = half4(lookupU, lookupV, 0, _LUTmiplevel);
			half3 lookup = tex2Dlod (_LUT, lookupVector);

			// Find our UVs for each axis based on world position of the fragment.
			half2 yUV = IN.worldPos.xz / _TextureScale;
			half2 xUV = IN.worldPos.zy / _TextureScale;
			half2 zUV = IN.worldPos.xy / _TextureScale;

			// Now do texture samples from our diffuse map with each of the 3 UV set's we've just made.
			half3 yDiff = tex2D (_SandDiffuse, yUV) * lookup.x + tex2D (_RockDiffuse, yUV) * lookup.y;
			half3 xDiff = tex2D (_SandDiffuse, xUV) * lookup.x + tex2D (_RockDiffuse, xUV) * lookup.y;
			half3 zDiff = tex2D (_SandDiffuse, zUV) * lookup.x + tex2D (_RockDiffuse, zUV) * lookup.y;

			// Get the absolute value of the world normal.
			// Put the blend weights to the power of BlendSharpness, the higher the value, 
            // the sharper the transition between the planar maps will be.
			half3 blendWeights = pow (abs(IN.worldNormal), _TriplanarBlendSharpness);

			// Divide our blend mask by the sum of it's components, this will make x+y+z=1
			blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

			// Finally, blend together all three samples based on the blend mask.
			o.Albedo = xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;
		}

		ENDCG
	} 
	FallBack "Diffuse"
}
