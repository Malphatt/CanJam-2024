float Unity_RandomRange_float(float2 Seed)
{
    float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
    return  randomno - 0.5;
}

void volumeFog_float(
    Texture3D NoiseTexture,
    SamplerState  TextureSampler,
    float Samples,
    float MeshDistance,
    float OverallHeight,
    float randomness,
    float Size,
    float threshold,
    float multiplier,
    float3 Position,
    float3 View,
    out float Fog
    ) {
    Fog = 0;

    float ratio = OverallHeight / View.y;
    float3 scaledView = ratio * View;
    float OverallDistance = length(scaledView) * -1;

    Samples = min(Samples, 50); // 50 will be the maximum samples, you can change this value if needed
    float Distance = max(OverallDistance,MeshDistance* - 1) / Samples;
    
    float3 rand = Unity_RandomRange_float(Position.xz) * randomness * View * Distance;

    float3 p;
    float yDistance;
    float noise;

    for (int i = 0; i <= Samples ; i++) {
        p = (Position + View * Distance * (i) );
        yDistance =  p.y - Position.y;

        p += rand;
        p *= Size;
        p.y = yDistance / OverallHeight;
        
        noise = SAMPLE_TEXTURE3D(NoiseTexture, TextureSampler, p).x; 
        Fog += saturate((noise - threshold) * multiplier* Distance ) ;
    }

    Fog = 1 - saturate (exp(-Fog));
}