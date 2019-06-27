Shader "Change/Dissolve_Transparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NoiseTex ("Noise", 2D) = "white" {}
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
		[Header(Glow)]
		[HDR] _GlowColor ("Color", Color) = (1,1,1,1)
		[Header(Outline)]
		_MinSize ("Min", Range(0,1)) = 0.9
		_MaxSize ("Max", Range(0,1)) = 1
		_lineSettings ("Tiling(1,2), Speed(1,2)", Vector) = (1,1,1,-1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        sampler2D _MainTex, _NoiseTex;
		half _Glossiness, _MinSize, _MaxSize;
        fixed4 _Color, _GlowColor, _lineSettings;
		float4 _PlaneLeft, _PlaneRight, _PlaneFront, _PlaneBack;


        struct Input
        {
            float2 uv_MainTex;
			float3 worldPos;
			float facing : VFACE;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			// Clipping
			half distanceRight = dot(IN.worldPos, _PlaneRight.xyz) + _PlaneRight.w;	//calculate signed distance to plane
			half distanceLeft = dot(IN.worldPos, _PlaneLeft.xyz) + _PlaneLeft.w;
			half distanceFront = dot(IN.worldPos, _PlaneFront.xyz) + _PlaneFront.w;
			half distanceBack = dot(IN.worldPos, _PlaneBack.xyz) + _PlaneBack.w;

			half lineRight = smoothstep(_MinSize, _MaxSize, 1 - distanceRight);
			half lineLeft = smoothstep(_MinSize, _MaxSize, 1 - distanceLeft);
			half lineFront = smoothstep(_MinSize, _MaxSize, 1 - distanceFront);
			half lineBack = smoothstep(_MinSize, _MaxSize, 1 - distanceBack);
			
			// Fill
			float facing = IN.facing * 0.5 + 0.5;

			// Noise
			half noise1 = tex2D (_NoiseTex, IN.uv_MainTex * _lineSettings.x + _Time.x * _lineSettings.z);
			half noise2 = tex2D (_NoiseTex, IN.uv_MainTex * _lineSettings.y + _Time.x * _lineSettings.w);

			half noise = noise1 * noise2;

			clip(distanceRight);
			clip(distanceLeft);
			clip(distanceFront);
			clip(distanceBack);

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Smoothness = _Glossiness;
			o.Alpha = c.r * _Color;
			o.Emission = c.r;

			//half lines = lineRight + lineLeft + lineFront + lineBack;
			//o.Emission = lines *_GlowColor + lerp(0, 1, 1 - facing);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
