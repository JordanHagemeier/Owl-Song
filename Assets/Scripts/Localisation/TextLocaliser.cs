using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextLocaliser : MonoBehaviour
{
    TextMeshProUGUI textField;

    public string key;

    
    private void Start()
    {
        Translate();
        LocalisationManager.OnLanguageChanged += Translate;
    }

    private void Translate()
    {
        textField = GetComponent<TextMeshProUGUI>();
        string value = LocalisationManager.GetLocalisedValue(key);

        textField.text = value;
    }

    private void OnDestroy()
    {
        LocalisationManager.OnLanguageChanged -= Translate;
    }

}
