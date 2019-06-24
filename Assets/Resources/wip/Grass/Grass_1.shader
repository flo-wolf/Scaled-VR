Shader "Custom/Grass_1"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
		_SecondColor ("Secondary Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Speed("MoveSpeed", Range(20,50)) = 25 // speed of the swaying
        _Rigidness("Rigidness", Range(0,1)) = 25 // lower makes it look more "liquid" higher makes it look rigid
        _SwayMax("Sway Max", Range(0, 0.1)) = .005 // how far the swaying goes
        _YOffset("Y offset", float) = 0.5// y offset, below this is no animation
		[Header(Rim)]
		_RimColor ("Color", Color) = (1,1,1,1)
		[Header(Outline (Clipping))]
		_MinSize ("Min", Range(0,1)) = 0.9
		_MaxSize ("Max", Range(0,1)) = 1
		_lineSettings ("Tiling(1,2), Speed(1,2)", Vector) = (1,1,1,-1)
		[HDR] _GlowColor ("Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
		Cull Off

        CGPROGRAM
        #pragma surface surf Standard vertex:vert addshadow 
        #pragma target 3.0

        sampler2D _MainTex;
		half _Glossiness, _Speed, _SwayMax, _YOffset, _Rigidness, _MinSize, _MaxSize;
        fixed4 _MainColor, _SecondColor, _RimColor, _lineSettings, _GlowColor;
		float4 _PlaneLeft, _PlaneRight, _PlaneFront, _PlaneBack;

        struct Input
        {
			float4 color : COLOR;
			float3 viewDir;
			float3 worldPos;
        };



		void vert(inout appdata_full v)
        {
            float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz; // world position         
            float wind = lerp(-1, 1, (wpos.x * _Rigidness) + _Time.x); // wind over world position
            float z = sin(_Speed*wind);
            float x = cos(_Speed*wind);
            v.vertex.x += step(0,v.vertex.y - _YOffset) * x * _SwayMax ; // apply the movement if the vertex's y above the YOffset
            v.vertex.z += step(0,v.vertex.y - _YOffset) * z * _SwayMax ;
        }


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
			half lines = lineRight + lineLeft + lineFront + lineBack;

			clip(distanceRight );
			clip(distanceLeft );
			clip(distanceFront );
			clip(distanceBack );


			// Rim
			half rim = 1- saturate(dot (normalize(IN.viewDir), o.Normal));

            fixed3 c = lerp(_MainColor, _SecondColor, IN.color.r);
            o.Albedo = c.rgb + (pow(rim, 2) * _RimColor * _LightColor0.rgb);
            o.Smoothness = _Glossiness;
			o.Emission = lines * _GlowColor;
            //o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
