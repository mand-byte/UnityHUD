Shader "Unlit/HUDFont"
{
	Properties
	{
		_MainTex ("Alpha (A)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		_UnitPerPixel ("Unit Per Pixel", Float) = 0.01
	}
	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}

		Cull Front
		Lighting Off
		ZWrite Off
		// ZTest Off
		ZTest LEqual
		Fog { Mode Off }
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{	
			CGPROGRAM
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				//float4 ScreenPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			//sampler2D_float _CameraDepthTexture;
			float _UnitPerPixel;
			v2f vert (appdata_t v)
			{
				v2f o;
				float2  uvOffset = v.uv2*_UnitPerPixel;
				float3  right = UNITY_MATRIX_IT_MV[0].xyz;
				float3  up = UNITY_MATRIX_IT_MV[1].xyz;
				float3  vPos = v.vertex.xyz + uvOffset.x * right + uvOffset.y * up;
				float4  vFinal = float4(vPos.xyz, 1.0);
				o.vertex = UnityObjectToClipPos(vFinal);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = i.color;

				col.a = tex2D(_MainTex, i.texcoord).a;
				return col;
			}
			#pragma vertex vert
			#pragma fragment frag
			ENDCG 
		}
	}	
}
