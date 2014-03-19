// Combines two images into one warped Side-by-Side image


sampler2D TexMap;

float4 HmdWarpParam;
float2 TexCoordOffset;
float4 TCScaleMat;
float AspectRatio;
float2 Scale;
float PupilOffset;

float4 SBSRift(float2 tc : TEXCOORD0) : COLOR
{
    float2 newtc = tc;
    
    newtc.x += PupilOffset;
    float d = newtc.x * newtc.x + newtc.y * newtc.y;
    newtc *= (HmdWarpParam.x + d*(HmdWarpParam.y + d*(HmdWarpParam.z + HmdWarpParam.w*d)));

    float2 newtc2;
    newtc2.x = newtc.x*TCScaleMat.x + newtc.y*TCScaleMat.y;
    newtc2.y = newtc.x*TCScaleMat.z + newtc.y*TCScaleMat.w;

    newtc2.x *= Scale.x;
    newtc2.x *= AspectRatio;
    newtc2.y *= Scale.y;
    newtc2.x += 0.25;
    newtc2.y += 0.5;
    newtc2.x = (newtc2.x > 0.5) ? 10.0 : newtc2.x;
    newtc2.x = (newtc2.x < 0.0) ? 10.0 : newtc2.x;
    newtc2.x += TexCoordOffset.x;
    
    return tex2D(TexMap, newtc2);
    //return float4(newtc2.x, newtc2.x, newtc2.x, 1.0);
}

technique ViewShader
{
    pass P0
    {
        VertexShader = null;
        PixelShader = compile ps_2_0 SBSRift();
    }
}