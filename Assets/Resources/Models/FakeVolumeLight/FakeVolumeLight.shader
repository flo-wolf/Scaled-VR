Shader "Custom/FakeVolumeLight"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Gradient", 2D) = "white" {}
        [Header(Radius)]
		_RadiusTop ("Top", Range(0,5)) = 0
		_RadiusBottom ("Bottom", Range(0,5)) = 0
		_Range ("Range", Range(0,10)) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200
		Blend One One
		Cull Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade vertex:vert
        #pragma target 3.0

        sampler2D _MainTex, _CameraDepthTexture;

        struct Input
        {
            float2 uv_MainTex;
			float4 color : COLOR;
			float4 screenPos;
        };

        half _RadiusTop, _RadiusBottom, _Range;
        fixed4 _Color;

		void vert (inout appdata_full v) {
			v.vertex.xyz += (v.normal * _RadiusTop * v.color.r) + (v.normal * (_RadiusBottom - 0.5) * v.color.g);
			v.vertex.y += _Range * v.color.r;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)));
			half intersect1 = (1 - smoothstep(0, 1, depth - IN.screenPos.w)) * 0.01;
			half FadeOut = (smoothstep(0, 0.75, depth - IN.screenPos.w));

            fixed4 c = (tex2D (_MainTex, IN.uv_MainTex) * _Color + intersect1) * FadeOut;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
			o.Emission = c * _Color;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
