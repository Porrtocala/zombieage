Shader "Custom/TextureShader" {
	Properties {
		tintColor ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Texture (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Pass {
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform float4 tintColor;

		struct appdata {
		    float4 vertex : POSITION;
            float4 uv : TEXCOORD0;
            float4 col: COLOR;
		};

        struct v2f {
        	float4 pos : SV_POSITION;
            fixed4 color : COLOR0;
            float4 uv : TEXCOORD0;
        };

        v2f vert (appdata v) {
        	v2f o;
            o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
            o.color = tintColor;
            o.uv = v.uv;
            return o;
        }

        fixed4 frag (v2f i) : SV_Target {
            return tex2D(_MainTex, i.uv.xy) * i.color;
        }
        ENDCG
        }
	}
	
	FallBack "Diffuse"
}
