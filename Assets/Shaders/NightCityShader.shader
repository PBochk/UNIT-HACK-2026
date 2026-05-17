Shader "UI/SynthCity"
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
            // Требуется Target 5.0 из-за тяжелых циклов и побитовых операций uint
            #pragma target 5.0 

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

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            // --- НАСТРОЙКИ ШЕЙДЕРА ---
            // ВАЖНО: Я уменьшил FSAA до 1 (в оригинале 2). Значение 2 делает 4 прохода 
            // Raymarching на каждый пиксель, что убьет FPS в UI. Если запас GPU позволяет - верните 2.
            #define FSAA 1 
            #define EPS 0.01
            #define FOVH 70.0
            #define D 0.1
            #define L 200.0
            #define MAX_ITER 100
            #define ROAD_RADIUS 4.0
            #define SPEED 2.0

            static const float3 CYAN = float3(0.4, 0.95, 1.0);
            static const float3 PINK = float3(1.0, 0.2, 0.9);
            static const float3 BLACK = float3(0.0, 0.0, 0.0);

            // --- ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ ---
            // Аналог GLSL mod (HLSL fmod работает иначе с отрицательными числами)
            float mod(float x, float y) { return x - y * floor(x / y); }
            float2 mod(float2 x, float2 y) { return x - y * floor(x / y); }
            float3 mod(float3 x, float3 y) { return x - y * floor(x / y); }

            float2 hashv(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
            }

            float noise(in float2 p)
            {
                const float K1 = 0.366025404;
                const float K2 = 0.211324865;

                float2 i = floor(p + (p.x + p.y) * K1);
                float2 a = p - i + (i.x + i.y) * K2;
                float m = step(a.y, a.x); 
                float2 o = float2(m, 1.0 - m);
                float2 b = a - o + K2;
                float2 c = a - 1.0 + 2.0 * K2;
                float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0);
                float3 n = h * h * h * h * float3(dot(a, hashv(i + 0.0)), dot(b, hashv(i + o)), dot(c, hashv(i + 1.0)));
                return dot(n, float3(70.0, 70.0, 70.0));
            }

            float4 min_sdf(float4 a, float4 b) {
                return a.w < b.w ? a : b;
            }

            uint hash(uint x)
            {
                x ^= x >> 16;
                x *= 0x7feb352dU;
                x ^= x >> 15;
                x *= 0x846ca68bU;
                x ^= x >> 16;
                return x;
            }

            float4 sdf_building(float3 p, float3 col, float3 b, bool get_color)
            {
                float3 q = abs(p) - b;
                float sdf_ = length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
                if (get_color) {
                    float3 bdv = smoothstep(float3(-0.6, -0.6, -0.6), float3(-0.4, -0.4, -0.4), q);
                    float bd = max(bdv.x * bdv.y, max(bdv.x * bdv.z, bdv.y * bdv.z));
                    col = lerp(col, CYAN, bd);
                }
                return float4(col, sdf_);
            }

            float sdf_box(float3 p, float3 b)
            {
                float3 q = abs(p) - b;
                return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
            }

            float4 sdf(in float3 pos, bool get_color) {
                float4 sdf_ = float4(0, 0, 0, 1000);

                const float R_0_X = 32.0;
                const float R_0_Z = 32.0;
                const float R_0_D = 100.0;
                const int GM = 2;
                const int GN = 2;
                const float w = R_0_X / float(GN);
                const float we = 0.325 * w;
                
                float3 col;
                if (get_color) {
                    uint col_hash = hash((uint)(0.6 * abs(pos.x)) + ((uint)(1.5 * abs(pos.y)) << 12) + ((uint)(0.6 * abs(pos.z)) << 16) + (pos.x > 0.0 ? 13u : 0u));
                    int col_idx = (int)(col_hash % 16u);
                    col = col_idx == 0 ? CYAN : (col_idx == 1 ? PINK : BLACK);
                } else {
                    col = BLACK;
                }

                // В HLSL массивы инициализируются через фигурные скобки
                const float heights[4] = {42.0, 34.0, 34.0, 32.0};
                
                for (int i = 0; i < GM; i++) {
                    for (int j = 0; j < GN; j++) {
                        int idx = GN * i + j;
                        float height = heights[idx];
                        float3 c = float3(-R_0_X / 2.0 + w * (0.5 + float(j)), 0.0, -R_0_Z / 2.0 + w * (0.5 + float(i)));
                        float3 pos0 = pos - c;
                        pos0.x += sign(pos.x) * 15.0;
                        pos0.x = pos0.x - R_0_X * (clamp(round(pos0.x / R_0_X), pos.x > 0.0 ? 1.0 : -8.0, pos.x > 0.0 ? 8.0 : -1.0));
                        pos0.z += R_0_D;
                        pos0.z = pos0.z - R_0_Z * clamp(round(pos0.z / R_0_Z), -16.0, 0.0);
                        
                        sdf_ = min_sdf(sdf_, sdf_building(pos0, col, float3(we, height, we), get_color));
                    }
                }
                
                {
                    float3 pos1 = pos;
                    pos1.x -= ROAD_RADIUS;
                    pos1.x = mod(pos1.x + ROAD_RADIUS, 2.0 * ROAD_RADIUS) - ROAD_RADIUS;
                    sdf_ = min_sdf(sdf_, float4(CYAN, sdf_box(pos1, float3(0.05, 0.05, 400.0))));
                }
                {
                    float3 pos2 = pos;
                    pos2.x = abs(pos2.x);
                    pos2.z -= SPEED * _Time.y; // iTime -> _Time.y
                    pos2.z = mod(pos2.z + ROAD_RADIUS, 2.0 * ROAD_RADIUS) - ROAD_RADIUS;
                    sdf_ = min_sdf(sdf_, float4(CYAN, sdf_box(pos2 - float3(ROAD_RADIUS + 200.0, 0, 0), float3(200.0, 0.05, 0.05))));
                }
                
                return sdf_;
            }

            float non_zero(float x) {
                return x + (x >= 0.0 ? 0.0001 : -0.0001);
            }

            void rayMarcher(in float3 cameraPos, in float3 lookDir, in float2 screenDim, in float2 uv, inout float3 col, in bool mirror) {
                float3 up = float3(0, 1, 0);
                float3 lookPerH = normalize(cross(lookDir, up));
                float3 lookPerV = normalize(cross(-lookDir, lookPerH));
                float3 screenCenter = cameraPos + lookDir;
                float3 screenPos = screenCenter + 0.5 * screenDim.x * uv.x * lookPerH
                                 + 0.5 * screenDim.y * uv.y * lookPerV;
                
                float3 rayDir = normalize(screenPos - cameraPos);
                float diffuse = 0.0;
                
                if (mirror) {
                    float s = -cameraPos.y / non_zero(rayDir.y);
                    float3 surf_inter = cameraPos + s * rayDir;
                    if (s < 0.0) return;
                    float2 surf_uv = (surf_inter.xz - float2(0, SPEED * _Time.y));
                    float noise_ = noise(5.0 * surf_uv);
                    diffuse = abs(surf_uv.x) < ROAD_RADIUS ? 0.1 : 0.2;
                
                    float noise_coef = abs(surf_uv.x) < ROAD_RADIUS ? 0.05 : 0.2;
                    cameraPos = surf_inter;
                    rayDir.y = -rayDir.y + noise_coef * noise_;
                }
                
                float t = 0.0;
                float4 dist;
                float3 pos;
                int iter = 0;
                do {
                    pos = cameraPos + t * rayDir;
                    dist = sdf(pos, false);
                    t += dist.w;
                    iter++;
                } while (t < L && iter < MAX_ITER && dist.w > EPS);
                
                if (pos.y < 0.0) return;
                
                if (dist.w <= EPS && (!mirror || pos.z > -200.0)) {
                    dist = sdf(pos, true);
                    float v = 1.0 - t / L;
                    col = dist.xyz;
                } else {
                    float s = (40.0 - cameraPos.z) / rayDir.z;
                    float2 sky_uv = (cameraPos + s * rayDir).xy;
                    float stars_noise = noise(sky_uv);
                    float white_intensity = smoothstep(0.6, 1.0, stars_noise);
                    col = lerp(col, float3(1, 1, 1), white_intensity);
                }
                
                if (mirror) {
                    col = lerp(col, float3(0.5, 0, 0.7), diffuse);
                }
            }

            float4 sampleColor(in float2 sampleCoord)
            {
                // Для UI мы берем соотношение сторон самого объекта или экрана
                float aspectRatio = _ScreenParams.x / _ScreenParams.y;
                float screenWidth = 2.0 * D * tan(0.5 * FOVH * (3.14159265 / 180.0)); // atan -> tan и перевод в радианы для FOV

                float3 cameraPos = float3(0, 2.5, 0);
                float3 lookDir = float3(0, 0.0, -D);
                float2 screenDim = float2(screenWidth, screenWidth / aspectRatio);

                // Нормализация под экран Unity
                float2 uv = 2.0 * sampleCoord / _ScreenParams.xy - 1.0;
                
                float3 col = BLACK;
                rayMarcher(cameraPos, lookDir, screenDim, uv, col, true);
                rayMarcher(cameraPos, lookDir, screenDim, uv, col, false);
                
                return float4(col, 1.0);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Переводим нормализованные UV обратно в пиксельные координаты для алгоритма
                float2 fragCoord = IN.uv * _ScreenParams.xy;
                
                float4 colSum = float4(0, 0, 0, 0);
                
                for(int i = 0; i < FSAA; i++) {
                    for(int j = 0; j < FSAA; j++) {
                        colSum += sampleColor(fragCoord + float2(float(i) / float(FSAA), float(j) / float(FSAA)));
                    }
                }
                
                float4 fragColor = colSum / colSum.w;

                // Поддержка UI масок (например, RectMask2D)
                #ifdef UNITY_UI_CLIP_RECT
                fragColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                return fragColor * IN.color;
            }
            ENDCG
        }
    }
}