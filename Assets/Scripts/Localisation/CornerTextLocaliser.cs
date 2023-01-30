using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class CornerTextLocaliser : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI binding;
    [TextArea(1, 3)]
    public string key;

    private void Start()
    {
        LocalisationManager.OnLanguageChanged += Translate;
        SkippingMinigame.SkippingSystem.OnCornerTextChanged += AssignKey;
        LocalisationManager.OnCornerTextChanged += AssignKey;
        PlayerInteractionController.OnCornerTextChanged += AssignKey;

        TranslatedInteractions[] translateds = { TranslatedInteractions.None };
        AssignKey(translateds);
    }

    private void Translate()
    {
        int longeststring = 0;

        string[] data = key.Split(new string[] { "\n" }, StringSplitOptions.None);

        string currentbindingText = "";

        for (int i = 0; i < data.Length; i++)
        {
            currentbindingText += LocalisationManager.GetLocalisedValue(data[i]);
            string bindingkey = data[i].Replace("action", "key");

            currentbindingText += " | " + LocalisationManager.GetLocalisedValue(bindingkey);


            if (LocalisationManager.GetLocalisedValue(bindingkey).ToCharArray().Length > longeststring)
            {
                longeststring = LocalisationManager.GetLocalisedValue(bindingkey).ToCharArray().Length;
            }

            if (i < data.Length - 1)
            {
                currentbindingText += "\n";
            }
        }

        binding.text = currentbindingText;
    }

    public void AssignKey(TranslatedInteractions[] interactions)
    {
        key = "";
        int i = 0;
        bool none = false;

        foreach (TranslatedInteractions interaction in interactions)
        {
            switch (interaction)
            {
                case TranslatedInteractions.Cancel:
                    key += "$action_UI_cancel";
                    break;
                case TranslatedInteractions.Confirm:
                    key += "$action_UI_confirm";
                    break;
                case TranslatedInteractions.Drop:
                    key += "$action_drop";
                    break;
                case TranslatedInteractions.Eat:
                    key += "$action_eat";
                    break;
                case TranslatedInteractions.Left:
                    key += "$action_UI_left";
                    break;
                case TranslatedInteractions.Right:
                    key += "$action_UI_right";
                    break;
                case TranslatedInteractions.Take:
                    key += "$action_pick_up";
                    break;
                case TranslatedInteractions.Quit:
                    key += "$action_minigame_quit";
                    break;
                case TranslatedInteractions.Throw:
                    key += "$action_minigame_throw";
                    break;
                case TranslatedInteractions.None:
                    none = true;
                    break;
            }

            if (i < interactions.Length - 1)
            {
                i++;
                key += "\n";
            }

            if(!none)
            {
                Translate();
            }

            else
            {
                binding.text = "";
            }
        }
    }

    private void OnDestroy()
    {
        LocalisationManager.OnLanguageChanged -= Translate;
        SkippingMinigame.SkippingSystem.OnCornerTextChanged -= AssignKey;
        LocalisationManager.OnCornerTextChanged -= AssignKey;
        PlayerInteractionController.OnCornerTextChanged -= AssignKey;

    }

    public enum TranslatedInteractions
    {
        Eat,
        Drop,
        Take,
        Confirm,
        Cancel,
        Left,
        Right,
        Throw,
        Quit,
        None
    }
}
