Shader "Change/Balloon"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_ColorSecond ("Color (Secondary)", Color) = (1,1,1,1)
        //_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_RoughnessMap ("Roughness Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex, _RoughnessMap;

        struct Input
        {
            float2 uv_MainTex;
			float4 color : COLOR;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color, _ColorSecond;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = lerp(_ColorSecond, _Color, IN.color.b);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness =  _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
