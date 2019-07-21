Shader "Change/Karton"
{
    Properties
    {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _Bump ("Normal Map", 2D) = "bump" {}
		[NoScaleOffset] _RoughMap ("Roughness", 2D) = "black" {}
		[NoScaleOffset] _OcclusionMap ("Occlusion Map", 2D) = "white" {}
		[NoScaleOffset] _Mask1 ("Mask", 2D) = "black" {}
        _Color ("Color (Main)", Color) = (1,1,1,1)
        _ColorSecond ("Color (Secondary)", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex, _Bump, _RoughMap, _Mask1, _OcclusionMap;
		fixed4 _Color, _ColorSecond;
		half _Glossiness;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_OcclusionMap;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float3 bump = UnpackScaleNormal(tex2D(_Bump, IN.uv_MainTex), 1);
			o.Normal = bump;
			
			fixed4 occ = tex2D (_OcclusionMap, IN.uv_OcclusionMap);
			o.Occlusion = occ;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed mask = tex2D (_Mask1, IN.uv_MainTex);
			o.Albedo = c.rgb * lerp(_Color, _ColorSecond, mask) * occ;
            
			
			

			o.Smoothness = tex2D (_RoughMap, IN.uv_MainTex).a * _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
