sampler uImage0 : register(s0); // Contents of the screen.
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float4 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float power = 1.5f;
float cullEdge = 2;
float2 texSize;
float pixelSize;
float rotation;
float2 center = float2(0.5, 0.5);


float4 Combined(float2 uv : TEXCOORD0, float4 vColor : COLOR0) : COLOR0
{
    // PIXELLATION
    float2 rel = uv - center;
    
    float s = sin(-rotation);
    float c = cos(-rotation);
    float2x2 rot = float2x2(c, -s, s, c);
    rel = mul(rel, rot);
    
    rel = floor(rel * texSize / pixelSize) * pixelSize / texSize;
    
    uv = rel + center;

    // CULLING
    float dist = max(0.0, 0.5 - length(uv - 0.5) * cullEdge);

    float4 color = tex2D(uImage0, uv);
    color *= pow(dist, power);

    return color * vColor;
}

technique Technique1
{
    pass Combined
    {
        PixelShader = compile ps_3_0 Combined();
    }
}