Shader "Change/Food" {
	Properties {
		[NoScaleOffset] _MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _BumpMap ("Normal Map", 2D) = "bump" {}
		[NoScaleOffset] _SmoothnessMap ("Smoothness Map", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		
		[Header(SSS)]
		_Distortion ("Distortion", Range(0,1)) = 0.5
		_Power ("Power", Range(0,5)) = 1.0
		_Scale ("Scale", Range(0,5)) = 1.0
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf SSSTranslucent fullforwardshadows
		#pragma target 3.0

		#include "UnityPBSLighting.cginc"

		sampler2D _MainTex, _SmoothnessMap, _BumpMap;
		half _Glossiness, _Distortion, _Power, _Scale;

		struct Input {
			float2 uv_MainTex;
		};

		
		inline fixed4 LightingSSSTranslucent(SurfaceOutputStandard s, float3 viewDir, UnityGI gi)
		{
			fixed4 c = LightingStandard(s, viewDir, gi); // original color 

			float3 lightDir = gi.light.dir;
			float3 normal = s.Normal;

			float3 H = lightDir + _Distortion * normal;
			float VdotH = pow(saturate(dot(viewDir, -H)), _Power) * _Scale;
 
			c.rgb += gi.light.color * VdotH * s.Albedo;
			return c;
		}
 
		void LightingSSSTranslucent_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
		{
			LightingStandard_GI(s, data, gi); 
		}
 

		void surf (Input IN, inout SurfaceOutputStandard o) {

			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;			

			fixed4 smoothnessMap = tex2D (_SmoothnessMap, IN.uv_MainTex);
			o.Smoothness = smoothnessMap.a * _Glossiness;
			o.Alpha = c.a;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
