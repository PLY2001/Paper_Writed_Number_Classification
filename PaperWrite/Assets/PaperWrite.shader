Shader "Unlit/PaperWrite"
{
	Properties{
		_Color("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex("Main Tex", 2D) = "white" {}
		


	}
		SubShader{
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry""DisableBatching"="True"}

			Pass {
				Tags { "LightMode" = "ForwardBase" }

				Cull Off

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				#pragma multi_compile_fwdbase

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"
				#include "UnityShaderVariables.cginc"

				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				

				struct a2v {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 worldNormal : TEXCOORD1;
					float3 worldPos : TEXCOORD2;
					fixed4 toWirte:COLOR;
					SHADOW_COORDS(3)
					
				};

				v2f vert(a2v v) {
					v2f o;
					
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.toWirte=v.color;
					
					TRANSFER_SHADOW(o);

					return o;
				}

				float4 frag(v2f i) : SV_Target {
					fixed3 worldNormal = normalize(i.worldNormal);
					fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
					fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
					fixed3 worldHalfDir = normalize(worldLightDir + worldViewDir);

					fixed3 diffuse=i.toWirte.xyz*_Color;
					

					

					return fixed4(diffuse, 1.0);

				}

				ENDCG
			}
		}
			FallBack "Diffuse"
}

