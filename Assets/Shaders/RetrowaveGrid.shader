Shader "UI/RetroGrid"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            float4 _ClipRect;
            sampler2D _MainTex;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            // --- ПЕРЕВЕДЕННАЯ ФУНКЦИЯ GRID ---
            float grid(float2 uv)
            {
                float2 size = float2(uv.y, uv.y * uv.y * 0.2) * 0.01;
                uv += float2(0.0, _Time.y * 2.0); // iTime -> _Time.y
                uv = abs(frac(uv) - 0.5);         // fract -> frac
                
                float2 lines = smoothstep(size, float2(0.0, 0.0), uv);
                lines += smoothstep(size * 5.0, float2(0.0, 0.0), uv) * 0.4;
                
                return clamp(lines.x + lines.y, 0.0, 3.0);
            }

            // --- ПЕРЕВЕДЕННЫЙ MAINIMAGE ---
            fixed4 frag(v2f IN) : SV_Target
            {
                // В Shadertoy координаты центрируются от -1 до 1.
                // В Unity UI IN.uv идёт от 0 до 1, поэтому делаем математическое смещение.
                float2 uv = IN.uv * 2.0 - 1.0;
                
                // В оригинале была коррекция соотношения сторон (aspect ratio).
                // Для UI чаще всего нужно, чтобы эффект просто растягивался по размерам Image.
                // Если сетка выглядит слишком вытянутой по горизонтали, можно домножить uv.x:
                // uv.x *= (_ScreenParams.x / _ScreenParams.y);

                float horizon = -0.9;
                float fog = smoothstep(0.2, -0.05, abs(uv.y + horizon));
                float3 col = float3(0.0, 0.1, 0.2); // vec3 -> float3
                
                if (uv.y < -horizon)
                {
                    uv.y = 3.0 / (abs(uv.y + horizon) + 0.05);
                    uv.x *= uv.y * 1.0;
                    
                    float gridVal = grid(uv);
                    // mix -> lerp
                    col = lerp(col, float3(1.0, 0.25, 0.5), gridVal); 
                }

                col += fog * fog * fog;
                col = lerp(float3(0.75, 0.1, 0.45) * 0.2, col, 0.7);

                float4 fragColor = float4(col, 1.0);

                // Корректная обрезка для RectMask2D, если Image будет лежать в скролле или маске
                #ifdef UNITY_UI_CLIP_RECT
                fragColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                return fragColor * IN.color;
            }
            ENDCG
        }
    }
}