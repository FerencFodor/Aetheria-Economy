#include "UnityCG.cginc"

//
//  Value Noise 3D Deriv
//  Return value range of 0.0->1.0, with format float4( value, xderiv, yderiv, zderiv )
//
float4 Value3D_Deriv( float3 P )
{
    //  https://github.com/BrianSharpe/Wombat/blob/master/Value3D_Deriv.glsl

    // establish our grid cell and unit position
    float3 Pi = floor(P);
    float3 Pf = P - Pi;
    float3 Pf_min1 = Pf - 1.0;

    // clamp the domain
    Pi.xyz = Pi.xyz - floor(Pi.xyz * ( 1.0 / 69.0 )) * 69.0;
    float3 Pi_inc1 = step( Pi, float3( (69.0 - 1.5).xxx ) ) * ( Pi + 1.0 );

    // calculate the hash
    float4 Pt = float4( Pi.xy, Pi_inc1.xy ) + float2( 50.0, 161.0 ).xyxy;
    Pt *= Pt;
    Pt = Pt.xzxz * Pt.yyww;
    float2 hash_mod = float2( 1.0 / ( 635.298681 + float2( Pi.z, Pi_inc1.z ) * 48.500388 ) );
    float4 hash_lowz = frac( Pt * hash_mod.xxxx );
    float4 hash_highz = frac( Pt * hash_mod.yyyy );

    //	blend the results and return
    float3 blend = Pf * Pf * Pf * (Pf * (Pf * 6.0 - 15.0) + 10.0);
    float3 blendDeriv = Pf * Pf * (Pf * (Pf * 30.0 - 60.0) + 30.0);
    float4 res0 = lerp( hash_lowz, hash_highz, blend.z );
    float4 res1 = lerp( res0.xyxz, res0.zwyw, blend.yyxx );
    float4 res3 = lerp( float4( hash_lowz.xy, hash_highz.xy ), float4( hash_lowz.zw, hash_highz.zw ), blend.y );
    float2 res4 = lerp( res3.xz, res3.yw, blend.x );
    return float4( res1.x, 0.0, 0.0, 0.0 ) + ( float4( res1.yyw, res4.y ) - float4( res1.xxz, res4.x ) ) * float4( blend.x, blendDeriv );
    return float4(1,1,1,1);
}

uniform sampler2D _NebulaSurfaceHeight;
float4 _NebulaSurfaceHeight_TexelSize;
uniform sampler2D _NebulaPatch;
uniform sampler2D _NebulaPatchHeight;
uniform sampler2D _NebulaTint;
float4 _NebulaTint_TexelSize;
uniform sampler2D _FluidVelocity;

uniform float3 _GridTransform;
uniform float3 _FluidTransform;

uniform half _NebulaFillDensity,
            _NebulaFillDistance,
            _NebulaFillExponent,
            _NebulaFillOffset,
            _NebulaFloorDensity,
            _NebulaPatchDensity,
            _NebulaFloorOffset,
            _NebulaFloorBlend,
            _NebulaPatchBlend,
            _NebulaLuminance,
            _TintExponent,
            _TintLodExponent,
            _NebulaNoiseScale,
            _NebulaNoiseExponent,
            _NebulaNoiseAmplitude,
            _NebulaNoiseSpeed,
            _FlowScale,
            _FlowAmplitude,
            _FlowScroll,
            _FlowPeriod,
            _SafetyDistance,
            _DynamicLodRange,
            _DynamicLodBias,
            _DynamicIntensity,
            _DynamicSkyBoost;

float tri(in float x){return abs(frac(x)-.5);}
float3 tri3(in float3 p){return float3( tri(p.z+tri(p.y*1.)), tri(p.z+tri(p.x*1.)), tri(p.y+tri(p.x*1.)));}

float triNoise3d(in float3 p)
{
    float z=1.4;
    float rz = 0.001;
    float3 bp = p;
    for (float i=0.; i<=1.; i++ )
    {
        float3 dg = tri3(bp * 2.);
        p += dg + _Time.y * _NebulaNoiseSpeed;

        bp *= 1.8;
        z *= 1.5;
        p *= 1.2;
	        
        rz+= (tri(p.z+tri(p.x+tri(p.y))))/z;
        bp += 0.14;
    }
    return rz;
}

float parabola( float x, float k )
{
    return pow( 4.0*x*(1.0-x), k );
}

float2 getUV(float2 pos)
{
    return -(pos-_GridTransform.xy)/_GridTransform.z + float2(.5,.5);
}

float2 getUVFluid(float2 pos)
{
    return (pos-_FluidTransform.xy)/_FluidTransform.z + float2(.5,.5);
}

float cloudDensity(float3 pos, float surfaceDisp)
{
    float2 uv = getUV(pos.xz);
    const float dist = pos.y + surfaceDisp - _NebulaFloorOffset;
    const float patch = tex2Dlod(_NebulaPatch, half4(uv, 0, 0)).r;
    const float patchDisp = tex2Dlod(_NebulaPatchHeight, half4(uv, 0, 0)).r;

    const float patchDensity = saturate((-abs(pos.y+patchDisp - _NebulaFloorOffset)+patch)/_NebulaPatchBlend)*_NebulaPatchDensity;
    const float floorDist = -dist;
    const float floorDensity = floorDist/_NebulaFloorBlend*_NebulaFloorDensity;
    return patchDensity + max(0,floorDensity);
}

// TODO: Get this working?
float2 flowTex(float3 pos)
{
    const float2 fluidUv = getUVFluid(pos.xz);
    if(any(fluidUv<0)||any(fluidUv>1)) return 0;
    const float2 fluidSample = tex2Dlod(_FluidVelocity, half4(fluidUv, 0, 0)).xy;
    // TODO: remove magic number for fluid texture dimensions
    return float3(fluidSample.x,0,fluidSample.y) * (512 / _FluidTransform.z);
}

float3 flow(float3 pos)
{
    const float4 noiseSample1 = Value3D_Deriv( pos / _FlowScale - float3(0,_FlowScroll,0) );
    const float4 noiseSample2 = Value3D_Deriv( pos / (_FlowScale * 1.61803398875) - float3(0,_FlowScroll,0) ); // make it golden
    return cross(normalize(noiseSample2.yzw), normalize(noiseSample1.yzw)) * _FlowAmplitude;
}

float tri2(in float x){return 1-2*abs(frac(x)-.5);}

float density(float3 pos)
{
    float2 uv = getUV(pos.xz);
    const float surfaceDisp = tex2Dlod(_NebulaSurfaceHeight, half4(uv, 0, 0)).r;
    const float dist = pos.y + surfaceDisp;
    float d = pow(abs(dist+_NebulaFillOffset)/_NebulaFillDistance,-_NebulaFillExponent) * _NebulaFillDensity;
    //d += pow(abs(dist+_NebulaFillOffset)/(_NebulaFillDistance*10),1/_NebulaFillExponent) * _NebulaFillDensity/4;
    if(dist < _SafetyDistance)
    {
        const float heightFade = smoothstep(_SafetyDistance,_SafetyDistance*.75,dist);
        const float3 fl = flow(pos);
        const float lerp1 = frac(_Time.y / _FlowPeriod);
        const float lerp2 = frac(_Time.y / _FlowPeriod + .5);
        const float noise1 = pow(triNoise3d((pos+fl * (lerp1 - .5) * _FlowPeriod) / _NebulaNoiseScale),_NebulaNoiseExponent) * _NebulaNoiseAmplitude * tri2(lerp1);
        const float noise2 = pow(triNoise3d((pos+fl * (lerp2 - .5) * _FlowPeriod) / _NebulaNoiseScale),_NebulaNoiseExponent) * _NebulaNoiseAmplitude * tri2(lerp2);
        pos.y += (noise1 + noise2) * heightFade;
        const float lerp3 = frac(_Time.y / _FlowPeriod * 2 + .25);
        const float lerp4 = frac(_Time.y / _FlowPeriod * 2 + .75);
        const float noise3 = pow(triNoise3d((pos+fl * (lerp3 - .5) * _FlowPeriod / 2) / _NebulaNoiseScale * 8), _NebulaNoiseExponent) * _NebulaNoiseAmplitude * tri2(lerp3) / 2;
        const float noise4 = pow(triNoise3d((pos+fl * (lerp4 - .5) * _FlowPeriod / 2) / _NebulaNoiseScale * 8), _NebulaNoiseExponent) * _NebulaNoiseAmplitude * tri2(lerp4) / 2;
        pos.y -= (noise3 + noise4) * heightFade;
        d += cloudDensity(pos, surfaceDisp);
    }
    return d;
}

float4 VolumeSampleColor(float3 pos)
{
	float d = density(pos);
    float2 uv = getUV(pos.xz);
    float3 tint = tex2Dlod(_NebulaTint, float4(uv.x, uv.y, 0, pow(max(.01,d), _TintLodExponent)));
    const float albedo = smoothstep(0,-250,pos.y) * pow(max(.1,d), _TintExponent) * _NebulaLuminance;
    return float4(albedo*tint, d);
}

float2 tintGradient (float2 uv)
{
    return float2(
        length(tex2Dlod(_NebulaTint, float4(uv.x + _NebulaTint_TexelSize.x, uv.y, 0, 0))) - length(tex2Dlod(_NebulaTint, float4(uv.x - _NebulaTint_TexelSize.x, uv.y, 0, 0))),
        length(tex2Dlod(_NebulaTint, float4(uv.x, uv.y + _NebulaTint_TexelSize.y, 0, 0))) - length(tex2Dlod(_NebulaTint, float4(uv.x, uv.y - _NebulaTint_TexelSize.y, 0, 0)))
    );
}
        
float3 VolumeSampleColorSimple(float3 pos, float3 normal)
{
    float2 uv = getUV(pos.xz);
    float3 low = tex2Dlod(_NebulaTint, float4(uv.x, uv.y, 0, _DynamicLodBias)).rgb;
    float3 high = tex2Dlod(_NebulaTint, float4(uv.x, uv.y, 0, _DynamicLodBias + _DynamicLodRange)).rgb * _DynamicSkyBoost;
    float upness = dot(normal, float3(0,1,0));
    //float lambert = 2 - dot(normalize(normal.xz), normalize(tintGradient(uv)));
    return lerp(low,high,sqrt((upness+1)/2)) * _DynamicIntensity / (density(pos)+1);
}
