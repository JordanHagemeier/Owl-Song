using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine;



public enum Daytime
{
    
    Noon,
    Evening,
    Night,
    Morning,
    Count
}
public class DaytimeManager : MonoBehaviour
{
    //day sun gameobject (light and volume)
    //evening sun gameobject (light and volume)

    //in connection with tick manager somehow (gets current tick number or only gets updated when the tick length/count etc changes, i don't know yet)
    //should have internal tick amounts/day times when it initializes a transition from one light setting to the next one (needs: how long is transition, when does it start, what transitions into what) 

    [SerializeField]    private GameObject              m_Sunlight;
                        private HDAdditionalLightData   m_LightData;
    [SerializeField]    private DaytimeInformation[]    m_DaytimeInfos;
    [SerializeField]    private Daytime                 m_CurrentDayTime = Daytime.Morning;
    [SerializeField]    private bool                    m_IsCurrentlyTransitioning; public bool IsCurrentlyTransitioning { get { return m_IsCurrentlyTransitioning; } set { m_IsCurrentlyTransitioning = value; } }
    [SerializeField]    private float                   m_OverallTransitionDuration; public float OverallTransitionDuration { get { return m_OverallTransitionDuration; } set { m_OverallTransitionDuration = value; } }
                        private float                   m_CurrentTransitionDuration;
    [SerializeField]    private float                   m_TransitioningMargin;

    [SerializeField]    private float[]                 m_VolumeWeightTransitionVelocities;
    [SerializeField]    private float                   m_LightIntensityTransitionVelocity;
    [SerializeField]    private float                   m_LightTemperatureTransitionVelocity;
    [SerializeField]    private Volume                  m_CameraVolume;
                        float                           m_TransitioningTemperature;



    [SerializeField] private Material[] m_ChangingMaterials;
    [SerializeField] private GameObject[] m_EmptyInteractionGOs;
    [SerializeField] private List<Light> m_EmptyInteractionLights;
    [SerializeField] private AnimationCurve[] m_ExposureCurve;

    [SerializeField] private List<GameObject> m_FireFlies; 
    [SerializeField] private bool m_StopTransitioning = false;

    //Achievements
    private bool m_FirstNight = false;
    private bool m_FirstMorning = false;
    private bool m_SecondEvening = false;
    private bool m_ThirdEvening = false;
    private bool m_FourthEvening = false;

    public void AddLightToAffectedLights(Light light)
    {
        m_EmptyInteractionLights.Add(light);
    }

    // Start is called before the first frame update
    void Start()
    {

        if (m_Sunlight == null)
        {
            Debug.LogError("Sunlight was not assigned! Daytime Manager will not commence!");
            return;
        }
        m_LightData = m_Sunlight.GetComponent<HDAdditionalLightData>();
        if (m_DaytimeInfos == null)
        {
            Debug.LogError("Daytime Infos are not assigned!");
            return;
        }
        m_VolumeWeightTransitionVelocities = new float[m_DaytimeInfos.Length];

        m_LightData.SetColor(Color.white, m_DaytimeInfos[(int)m_CurrentDayTime].m_SunlightTemperature);
        m_LightData.intensity = m_DaytimeInfos[(int)m_CurrentDayTime].m_SunlightIntensity;
        m_LightData.EnableColorTemperature(true);


        m_DaytimeInfos[(int)m_CurrentDayTime].m_LightSettingsVolume.SetActive(true);
        m_DaytimeInfos[(int)m_CurrentDayTime].m_LightSettingsVolume.GetComponent<Volume>().weight = 1.0f;
        for (int i = 1; i < m_DaytimeInfos.Length; i++)
        {
            m_DaytimeInfos[i].m_LightSettingsVolume.GetComponent<Volume>().weight = 0.0f;
            //m_DaytimeInfos[i].m_LightSettingsVolume.SetActive(false);
        }
        VolumeProfile volumeProfile = m_CameraVolume.sharedProfile;
        volumeProfile.TryGet<Exposure>(out Exposure exposure);
        exposure.fixedExposure.value = m_DaytimeInfos[(int)m_CurrentDayTime].m_ExposureValue;

        GetAllEmptyInteractionLights();

        if(m_CurrentDayTime != Daytime.Night)
        {
            for (int i = 0; i < m_FireFlies.Count; i++)
            {
                m_FireFlies[i].GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    private void GetAllEmptyInteractionLights()
    {
        for(int i = 0; i < m_EmptyInteractionGOs.Length; i++)
        {
            var blueprintHierachy = m_EmptyInteractionGOs[i].FindComponentInChildWithName<Transform>("Blueprint_Menu_Hierarchical");
            if(blueprintHierachy == null)
            {
                blueprintHierachy = m_EmptyInteractionGOs[i].FindComponentInChildWithName<Transform>("NeedsWSUI");
            }
            Light light = blueprintHierachy.gameObject.FindComponentInChildWithName<Light>("Spot Light");
            
            if(light != null)
            {
                m_EmptyInteractionLights.Add(light);
            }
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_StopTransitioning)
        {
            return;
        }

        if (m_IsCurrentlyTransitioning)
        {
            m_CurrentTransitionDuration += Time.deltaTime;
            
            
            bool sunlightTransitionFinished = TransitionSunlightSettings();
            bool volumeTransitionFinished = TransitionToNextLightSetting();
            bool volumeExposureValueTransitionFinished = TransitionExposureValue();
            bool skyBoxValueTransitionFinished = TransitionSkyBox();
            TransitionMaterialsAndSpotLights();
            if (m_CurrentDayTime == Daytime.Evening)
            {
                for (int i = 0; i < m_FireFlies.Count; i++)
                {
                    m_FireFlies[i].GetComponent<ParticleSystem>().Play();
                }
            }
            if (sunlightTransitionFinished && volumeTransitionFinished && volumeExposureValueTransitionFinished)
            {
                m_CurrentDayTime = (Daytime)(((int)m_CurrentDayTime + 1) % (int)Daytime.Count);
                m_IsCurrentlyTransitioning = false;
                m_CurrentTransitionDuration = 0.0f;
                

                if(m_CurrentDayTime == Daytime.Morning)
                {
                    for (int i = 0; i < m_FireFlies.Count; i++)
                    {
                        m_FireFlies[i].GetComponent<ParticleSystem>().Stop();
                    }
                }
                if(m_CurrentDayTime == Daytime.Night && !m_FirstNight)
                {
                    m_FirstNight = true;
                    Singletons.achievementManager.AchievementWasAccomplished(AchievementType.FirstNight);

                }

                else if(m_CurrentDayTime == Daytime.Morning && m_FirstNight && !m_FirstMorning)
                {
                    m_FirstMorning = true;
                    Singletons.achievementManager.AchievementWasAccomplished(AchievementType.FirstMorning);
                }

                else if(m_CurrentDayTime == Daytime.Night && m_FirstNight && m_FirstMorning && !m_SecondEvening)
                {
                    m_SecondEvening = true;
                    Singletons.achievementManager.AchievementWasAccomplished(AchievementType.SecondNight);
                }

                else if (m_CurrentDayTime == Daytime.Night && m_FirstNight && m_FirstMorning && m_SecondEvening && !m_ThirdEvening)
                {
                    m_ThirdEvening = true;
                    Singletons.achievementManager.AchievementWasAccomplished(AchievementType.ThirdNight);
                }

                else if (m_CurrentDayTime == Daytime.Night && m_FirstNight && m_FirstMorning && m_SecondEvening && m_ThirdEvening && !m_FourthEvening)
                {
                    m_FourthEvening = true;
                    Singletons.achievementManager.AchievementWasAccomplished(AchievementType.FourthNight);
                }
            }
        }
    }


    public void TryTransitioningToNextLightSetting()
    {
        if(m_Sunlight == null)
        {
            Debug.LogError("Sunlight was not assigned!");
            return;
        }
        if(m_DaytimeInfos == null) 
        {
            Debug.LogError("Daytime Informations were not assigned!");
            return;
        }
        m_IsCurrentlyTransitioning = true;
        m_TransitioningTemperature = m_DaytimeInfos[(int)m_CurrentDayTime % m_DaytimeInfos.Length].m_SunlightTemperature;
        Daytime currentDayTime = (Daytime)((int)m_CurrentDayTime % (int)Daytime.Count);
        if (currentDayTime == Daytime.Evening | currentDayTime == Daytime.Night)
        {
            Singletons.backgroundMusicManager.TryStartingTransition(currentDayTime);
        }
       
    }


    private bool TransitionToNextLightSetting()
    {
        bool finishedTransitioning = false;
        float currentVolumeWeight   = m_DaytimeInfos[(int)m_CurrentDayTime % m_DaytimeInfos.Length].m_LightSettingsVolume.GetComponent<Volume>().weight;
        float nextVolumeWeight      = m_DaytimeInfos[((int)m_CurrentDayTime + 1) % m_DaytimeInfos.Length].m_LightSettingsVolume.GetComponent<Volume>().weight;

        DaytimeInformation currentDaytime   = m_DaytimeInfos[(int)m_CurrentDayTime % m_DaytimeInfos.Length];
        DaytimeInformation nextDaytime      = m_DaytimeInfos[((int)m_CurrentDayTime + 1) % m_DaytimeInfos.Length];



        //m_DaytimeInfos[(int)m_CurrentDayTime % m_DaytimeInfos.Length].m_LightSettingsVolume.GetComponent<Volume>().weight       = Mathf.SmoothDamp(currentVolumeWeight, 0.0f, ref m_VolumeWeightTransitionVelocities[(int)m_CurrentDayTime % m_DaytimeInfos.Length], m_OverallTransitionDuration);
        //m_DaytimeInfos[((int)m_CurrentDayTime + 1) % m_DaytimeInfos.Length].m_LightSettingsVolume.GetComponent<Volume>().weight = Mathf.SmoothDamp(nextVolumeWeight, 1.0f, ref m_VolumeWeightTransitionVelocities[((int)m_CurrentDayTime + 1) % m_DaytimeInfos.Length], m_OverallTransitionDuration);
       

        m_DaytimeInfos[(int)m_CurrentDayTime % m_DaytimeInfos.Length].m_LightSettingsVolume.GetComponent<Volume>().weight = Mathf.Lerp(1.0f, 0.0f, m_CurrentTransitionDuration / m_OverallTransitionDuration);
        m_DaytimeInfos[((int)m_CurrentDayTime + 1) % m_DaytimeInfos.Length].m_LightSettingsVolume.GetComponent<Volume>().weight = Mathf.Lerp(0.0f, 1.0f, m_CurrentTransitionDuration / m_OverallTransitionDuration);

        if (1.0f - m_DaytimeInfos[((int)m_CurrentDayTime + 1) % m_DaytimeInfos.Length].m_LightSettingsVolume.GetComponent<Volume>().weight <= m_TransitioningMargin 
            && m_DaytimeInfos[(int)m_CurrentDayTime % m_DaytimeInfos.Length].m_LightSettingsVolume.GetComponent<Volume>().weight <= m_TransitioningMargin)
        {

            finishedTransitioning = true;
            m_DaytimeInfos[((int)m_CurrentDayTime + 1) % m_DaytimeInfos.Length].m_LightSettingsVolume.GetComponent<Volume>().weight = 1.0f;
            m_DaytimeInfos[(int)m_CurrentDayTime % m_DaytimeInfos.Length].m_LightSettingsVolume.GetComponent<Volume>().weight = 0.0f;


        }

        return finishedTransitioning;
    }

    private bool TransitionSunlightSettings()
    {
        bool finishedTransitioning          = false;
        DaytimeInformation currentDaytime   = m_DaytimeInfos[(int)m_CurrentDayTime % m_DaytimeInfos.Length];
        DaytimeInformation nextDaytime      = m_DaytimeInfos[((int)m_CurrentDayTime + 1) % m_DaytimeInfos.Length];

        //m_TransitioningTemperature          = Mathf.SmoothDamp(m_TransitioningTemperature, nextDaytime.m_SunlightTemperature, ref m_LightTemperatureTransitionVelocity, m_OverallTransitionDuration);
        m_TransitioningTemperature = Mathf.Lerp(currentDaytime.m_SunlightTemperature, nextDaytime.m_SunlightTemperature, m_CurrentTransitionDuration / m_OverallTransitionDuration);
        m_LightData.SetColor(Color.white, m_TransitioningTemperature);

        //m_LightData.intensity = Mathf.SmoothDamp(m_LightData.intensity, nextDaytime.m_SunlightIntensity, ref m_LightIntensityTransitionVelocity, m_OverallTransitionDuration);
        m_LightData.intensity = Mathf.Lerp(currentDaytime.m_SunlightIntensity, nextDaytime.m_SunlightIntensity, m_CurrentTransitionDuration / m_OverallTransitionDuration);
        if(Mathf.Abs(nextDaytime.m_SunlightTemperature - m_TransitioningTemperature) <= m_TransitioningMargin && Mathf.Abs(nextDaytime.m_SunlightIntensity - m_LightData.intensity) <= m_TransitioningMargin)
        {
            finishedTransitioning = true;
            m_LightData.intensity = nextDaytime.m_SunlightIntensity;
            m_LightData.SetColor(Color.white, nextDaytime.m_SunlightTemperature);
        }
        return finishedTransitioning;
    }

    private bool TransitionExposureValue()
    {
        bool finishedTransitioning = false;

        DaytimeInformation currentDaytime = m_DaytimeInfos[(int)m_CurrentDayTime % m_DaytimeInfos.Length];
        DaytimeInformation nextDaytime = m_DaytimeInfos[((int)m_CurrentDayTime + 1) % m_DaytimeInfos.Length];
        VolumeProfile volumeProfile = m_CameraVolume.sharedProfile;
        volumeProfile.TryGet<Exposure>(out Exposure exposure);

        float updatedExposure = Mathf.Lerp(currentDaytime.m_ExposureValue, nextDaytime.m_ExposureValue, m_ExposureCurve[(int)m_CurrentDayTime % m_DaytimeInfos.Length].Evaluate(m_CurrentTransitionDuration / m_OverallTransitionDuration));

        exposure.fixedExposure.value = updatedExposure;

        volumeProfile.TryGet<NewSky>(out NewSky lerpingSky);
        lerpingSky.exposure.value = updatedExposure;
        //if (Mathf.Abs(exposure.fixedExposure.value - nextDaytime.m_ExposureValue) <= m_TransitioningMargin)
        //{
        //    finishedTransitioning = true;
        //    exposure.fixedExposure.value = nextDaytime.m_ExposureValue;

        //}

        if(Mathf.Abs(lerpingSky.exposure.value - nextDaytime.m_ExposureValue) <= m_TransitioningMargin)
        {
            finishedTransitioning = true;
            lerpingSky.exposure.value = nextDaytime.m_ExposureValue;
        }

        return finishedTransitioning;

    }

    private bool TransitionMaterialsAndSpotLights()
    {
        bool finishedTransitioning = false;
        DaytimeInformation currentDaytime = m_DaytimeInfos[(int)m_CurrentDayTime % m_DaytimeInfos.Length];
        DaytimeInformation nextDaytime = m_DaytimeInfos[((int)m_CurrentDayTime + 1) % m_DaytimeInfos.Length];

        for(int i = 0; i < m_EmptyInteractionLights.Count; i++)
        {
            m_EmptyInteractionLights[i].intensity = Mathf.Lerp(currentDaytime.m_SpotlightIntensity, nextDaytime.m_SpotlightIntensity, m_CurrentTransitionDuration / m_OverallTransitionDuration);
        }

        for(int k = 0; k < m_ChangingMaterials.Length; k++)
        {
            m_ChangingMaterials[k].SetFloat(Shader.PropertyToID("_night_amount"), Mathf.Lerp(currentDaytime.m_MaterialStateIntensity, nextDaytime.m_MaterialStateIntensity, m_CurrentTransitionDuration / m_OverallTransitionDuration));
        }

        return finishedTransitioning;
    }

    private bool TransitionSkyBox()
    {
        bool finishedTransitioning = false;
        DaytimeInformation currentDaytime = m_DaytimeInfos[(int)m_CurrentDayTime % m_DaytimeInfos.Length];
        DaytimeInformation nextDaytime = m_DaytimeInfos[((int)m_CurrentDayTime + 1) % m_DaytimeInfos.Length];
        VolumeProfile volumeProfile = m_CameraVolume.sharedProfile;

        CubemapParameter currentSky = currentDaytime.m_SkyBox;
        CubemapParameter nextSky = nextDaytime.m_SkyBox;

        volumeProfile.TryGet<NewSky>(out NewSky lerpingSky);
        lerpingSky.hdriSky.value = currentSky.value;
        lerpingSky.hdriSky2.value = nextSky.value;

        lerpingSky.transitionLerp.value = Mathf.Lerp(0.0f, 1.0f, m_CurrentTransitionDuration / m_OverallTransitionDuration);

        if(m_CurrentTransitionDuration / m_OverallTransitionDuration >= 0.97f)
        {
            finishedTransitioning = true;
            lerpingSky.hdriSky.value = nextSky.value;
            lerpingSky.hdriSky2.value = currentSky.value;
            lerpingSky.transitionLerp.value = 0.0f;
        }

        return finishedTransitioning;
    }

   
    
}

   

