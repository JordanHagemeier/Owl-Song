using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[VolumeComponentMenu("Sky/New Sky")]
// SkyUniqueID does not need to be part of built-in HDRP SkyType enumeration.
// This is only provided to track IDs used by HDRP natively.
// You can use any integer value.
[SkyUniqueID(NEW_SKY_UNIQUE_ID)]
public class NewSky : SkySettings
{
    const int NEW_SKY_UNIQUE_ID = 1023542352;

    [Tooltip("Specify the cubemap HDRP uses to render the sky.")]
    public CubemapParameter hdriSky = new CubemapParameter(null);
    public CubemapParameter hdriSky2 = new CubemapParameter(null);
    public ClampedFloatParameter transitionLerp = new ClampedFloatParameter(0.5f, 0.0f, 1.0f);

    public override Type GetSkyRendererType()
    {
        return typeof(NewSkyRenderer);
    }

    public override int GetHashCode()
    {
        int hash = base.GetHashCode();
        unchecked
        {
            hash = hdriSky.value != null ? hash * 23 + hdriSky.GetHashCode() + hdriSky2.GetHashCode() : hash;
        }
        return hash;
    }
}

