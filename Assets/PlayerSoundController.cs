using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    public bool m_PlayerSoundsAllowed = false;
    [SerializeField] private AudioSource        m_PlayerStepSource;
    [SerializeField] private AudioSource        m_PlayerAudioSource;
    [SerializeField] private List<AudioClip>    m_GrasSteppingSounds;
    [SerializeField] private AudioClip          m_BridgeSteppingSound;
    [SerializeField] private List<AudioClip>    m_HummingSounds;
    [SerializeField] private List<AudioClip>    m_PickupSounds;
    [SerializeField] private List<AudioClip>    m_DropSounds;
    [SerializeField] private List<AudioClip>    m_BuildSounds;
    // Start is called before the first frame update

    private void Awake()
    {
        PlayerInteractionController.OnDrop += DropGrunt;
        ConsumerLogic.OnDrop += DropGrunt;
       
    }

    private void OnDestroy()
    {
        PlayerInteractionController.OnDrop -= DropGrunt;
        ConsumerLogic.OnDrop -= DropGrunt;
    }

    public void Step()
    {
        int random = (int)Mathf.Abs(Random.Range(0.0f, (float)(m_GrasSteppingSounds.Count - 1)));
        m_PlayerStepSource.clip = m_GrasSteppingSounds[random];
        RaycastHit hit;
        Vector3 direction = gameObject.transform.up * -1.0f;

        if(Physics.Raycast(gameObject.transform.position, direction, out hit, 2.0f))
        {
            if(hit.collider.gameObject.tag == "Bridge")
            {
                m_PlayerStepSource.clip = m_BridgeSteppingSound; ;
            }
        }
        m_PlayerStepSource.Play();
    }

    public void Humming()
    {
        if (m_PlayerSoundsAllowed)
        {
            int random = (int)Mathf.Abs(Random.Range(0.0f, (float)(m_HummingSounds.Count - 1)));
            m_PlayerAudioSource.clip = m_HummingSounds[random];
            m_PlayerAudioSource.Play();
        }
        
    }

    public void PickUpGrunt()
    {
        int random = (int)Mathf.Abs(Random.Range(0.0f, (float)(m_PickupSounds.Count - 1)));
        m_PlayerAudioSource.clip = m_PickupSounds[random];
        m_PlayerAudioSource.Play();
    }

    public void DropGrunt(bool ontogras)
    {
        if (ontogras)
        {
            int random = (int)Mathf.Abs(Random.Range(0.0f, (float)(m_DropSounds.Count - 1)));
            m_PlayerAudioSource.clip = m_DropSounds[random];
        }

        else
        {
            int random = (int)Mathf.Abs(Random.Range(0.0f, (float)(m_BuildSounds.Count - 1)));
            m_PlayerAudioSource.clip = m_BuildSounds[random];
        }


        m_PlayerAudioSource.Play();
    }
}
