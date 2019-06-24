Shader "Custom/SSS (modified)" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Distortion ("Distortion", Range(0,1)) = 0.5
		_Power ("Power", Range(0,5)) = 1.0
		_Scale ("Scale", Range(0,5)) = 1.0
		_BumpMap ("Albedo (RGB)", 2D) = "bump" {}
		[NoScaleOffset] _Ramp ("Toon Ramp", 2D) = "gray" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf SSSTranslucent fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _BumpMap, _Ramp;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _Distortion;
		half _Power;
		half _Scale;

		#include "UnityPBSLighting.cginc"
		inline fixed4 LightingSSSTranslucent(SurfaceOutputStandard s, float3 viewDir, UnityGI gi)
		{
			float3 lightDir = gi.light.dir;
			float3 normal = s.Normal;
			
			half NdotL = dot (normal, lightDir);
			half d = dot (normal, lightDir) * 0.5 + 0.5;

			half3 ramp = tex2D(_Ramp, float2(d,d)).rgb;

			// Rim
			float rim = (1 - dot(viewDir, normal)) * (1 - ramp);

			fixed4 c = LightingStandard(s, viewDir, gi); // original color 
			c.rgb = s.Albedo * ramp * gi.light.color + s.Albedo * gi.indirect.diffuse * (1 - ramp);// + smoothstep(0.8, 0.9,rim) * 0.2;



			float3 H = lightDir + _Distortion * normal;
			float VdotH = pow(saturate(dot(viewDir, -H)), _Power) * _Scale;
 
			c.rgb += gi.light.color * (s.Albedo * 0.5) * VdotH * (1-NdotL);
			return c;
		}
 
		void LightingSSSTranslucent_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
		{
			LightingStandard_GI(s, data, gi); 
		}
 
		
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
