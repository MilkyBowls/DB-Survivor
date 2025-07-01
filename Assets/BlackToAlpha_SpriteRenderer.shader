Shader "Custom/BlackToAlpha_BrightAura_WithBrightness"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Brightness ("Brightness", Float) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _Brightness;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, IN.texcoord);

                // Compute luminance for black-to-alpha
                float luminance = dot(texColor.rgb, float3(0.299, 0.587, 0.114));
                float baseAlpha = saturate(luminance); // black = 0 alpha

                // Apply tint and brightness to color
                fixed3 finalRGB = texColor.rgb * IN.color.rgb * _Brightness;

                // Final alpha uses both luminance and sprite alpha
                float finalAlpha = baseAlpha * IN.color.a;

                return fixed4(finalRGB, finalAlpha);
            }
            ENDCG
        }
    }
}
