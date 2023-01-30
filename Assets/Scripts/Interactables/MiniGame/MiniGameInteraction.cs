using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameInteraction : Interactable
{
    [SerializeField] private SkippingMinigame.SkippingSystem m_SkippingSystem;
    [SerializeField] private Camera m_MainCamera;
    [SerializeField] private Camera m_MiniGameCamera;
    [SerializeField] private GameObject m_soundsource;

    public override bool OnInteract(InteractionTypes type, PlayerInteractionController controller)
    {
        bool success = false;
        if (m_SkippingSystem.m_SkippingGameActive)
        {
            return success;
        }
        m_SkippingSystem.SetMiniGameActive();
        m_MainCamera.gameObject.SetActive(false);
        m_MiniGameCamera.gameObject.SetActive(true);
        m_soundsource.GetComponent<AudioSource>().playOnAwake = false;
        Singletons.audioBookManager.audioClipsAreAllowedToPlay = false;
        Singletons.gameStateManager.m_InMenu = true;

        Singletons.gameStateManager.ChangePlayerMovementState(false);

        return success;
    }
}
