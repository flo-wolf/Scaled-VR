Shader "Custom/KCalDoll"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FillMaskTex ("FillMask", 2D) = "black" {}
		_LightSquareTex ("LightSquare", 2D) = "black" {}
		_HighlightTex("Highlight", 2D) = "black" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
		[Header(Emission)]
		[HDR] _Color("Main", Color) = (0,0,0,0)
		[HDR] _ColorHighlight("Highlight", Color) = (0,0,0,0)
        _Height ("Height", Range(0,1)) = 0.0
		[HideInInspector] _startOffset ("Start Offset", Range(0,50)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

        sampler2D _MainTex, _FillMaskTex, _LightSquareTex, _HighlightTex;
		half _Glossiness, _Height;
        fixed4 _Color, _ColorHighlight;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_HighlightTex;
        };

		UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(half, _startOffset)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
				// Mask
			//half3 lightSquare = tex2D(_LightSquareTex, IN.uv_MainTex + fixed2(0, (1 - frac(_Height)) + UNITY_ACCESS_INSTANCED_PROP(Props, _startOffset)));
			half3 lightSquare = tex2D(_LightSquareTex, IN.uv_MainTex + fixed2(0, (1 - _Height)));
			half highlightMask = tex2D(_HighlightTex, IN.uv_HighlightTex + fixed2(0, (1 - _Height)));
			half fillMask = tex2D(_FillMaskTex, IN.uv_MainTex);
			
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;

			o.Emission = lightSquare * _Color + highlightMask * _ColorHighlight;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
