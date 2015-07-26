Shader "Hidden/ClearForwardGBuffer"
{

    SubShader
    {
        Pass
        {
            Cull Off
            ZTest Always
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _CameraGBufferTexture2;   // normal (rgb), --unused, very low precision-- (a) 
            #define HalfPixelSize ((_ScreenParams.zw-1.0)*0.5)
            float4 _ClearColor;

            struct ia_out {
                float4 vertex : POSITION;
            };

            struct vs_out {
                float4 vertex : SV_POSITION;
                float4 screen_pos : TEXCOORD0;
            };

            struct ps_out {
                half4 color : SV_Target;
                float depth : SV_Depth;
            };

            vs_out vert(ia_out v)
            {
                vs_out o;
                o.vertex = o.screen_pos = v.vertex;
                o.screen_pos.y *= _ProjectionParams.x;
                return o;
            }

            ps_out frag(vs_out IN)
            {
                float2 coord = IN.screen_pos.xy * 0.5 + 0.5;
                // this makes me sad...
                #if SHADER_API_D3D9 
                    coord += HalfPixelSize;
                #endif

                float3 n = tex2D(_CameraGBufferTexture2, coord);
                if (dot(n, 1) > 0) { discard; } // discard if this pixel is deferred object

                ps_out o;
                o.color = _ClearColor;
                o.depth = 1;
                return o;
            }

            ENDCG
        }
    }
}
