using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;

[System.Serializable]
public class DaytimeInformation : MonoBehaviour
{
    [SerializeField] public float m_SunlightTemperature;
    [SerializeField] public float m_SunlightIntensity;
    [SerializeField] public GameObject m_LightSettingsVolume;
    [SerializeField] public float m_ExposureValue;
    [SerializeField] public CubemapParameter m_SkyBox;
    [SerializeField] public float m_MaterialStateIntensity;
    [SerializeField] public float m_SpotlightIntensity;
}
