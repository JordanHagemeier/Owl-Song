using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public struct colorPerPerson
{
    public Color color;
    public Speaker speakerkey;
}

public class SubtitleManager : MonoBehaviour
{
    public colorPerPerson[] colorPerPeople;
    public Color defaultColor;

    public GameObject bgPanel;
    public GameObject textBox;

    public AudioClip lastSubtitleKey; 

    public Dictionary<AudioClip, Subtitle> subtitlesWithAudioKey = new Dictionary<AudioClip, Subtitle>();
    public Subtitle[] subtitleValues;
    public AudioClip[] audioClipKeys;

    private string currentTextKey;
    private string currentSpeakerKey;
    private float buffer = 15;

    private bool IsLastSub = false;

    [SerializeField] Animator credits;

    private void Awake()
    {
        //reset textbox
        textBox.GetComponent<TextMeshProUGUI>().text = "";
        bgPanel.SetActive(false);


        //create dictionary
        subtitlesWithAudioKey.Clear();

        int count = 0;

        foreach (AudioClip key in audioClipKeys)
        {

            if(!subtitlesWithAudioKey.ContainsKey(key))
            {
                if (subtitleValues[count] != null)
                {
                    subtitlesWithAudioKey.Add(key, subtitleValues[count]);
                }
            }
            count++;
        }


        //debug print
        /*foreach(AudioClip key in subtitlesWithAudioKey.Keys)
        {
            Debug.Log(key.ToString() + " " + subtitlesWithAudioKey[key].ToString());
        }*/

        //StartSubtitles(testSubtitle);
    }

    public void StartSubtitles(AudioClip key)
    {
        if(subtitlesWithAudioKey.ContainsKey(key))
        {
            bgPanel.SetActive(true);
            ChangeBGPanelSize(0, 0);
            Subtitle subtitle = subtitlesWithAudioKey[key];

            if (subtitle == subtitlesWithAudioKey[lastSubtitleKey])
            {
                IsLastSub = true;
            }

            if (subtitle.lines.Length >= 0)
            {
                StartCoroutine(SequenceOfSubtitles(subtitle));
            }
        }

        else
        {
            Debug.Log("subtitle not assigned");
        }
    }

    private IEnumerator SequenceOfSubtitles(Subtitle subtitle)
    {
        foreach (Line line in subtitle.lines)
        {          
            yield return SingleSubtitle(line);
        }

        
        yield return null; ;
        textBox.GetComponent<TextMeshProUGUI>().text = "";
        ChangeBGPanelSize(0, 0);
        bgPanel.SetActive(true);
        if (IsLastSub)
        {
            StopAllCoroutines();

            textBox.SetActive(false);
            bgPanel.SetActive(false);

            credits.SetTrigger("StartCreditRoll");
            gameObject.SetActive(false);
        }

    }

    private IEnumerator SingleSubtitle (Line singleLine)
    {
        currentSpeakerKey = "$SPEAKER_" + singleLine.speaker.ToString();
        currentTextKey = singleLine.text;

        string[] data = singleLine.text.Split(new string[] {"\n"}, StringSplitOptions.None);

        string currentSpeaker = LocalisationManager.GetLocalisedValue(currentSpeakerKey);

        string currentText = "";

        if (data.Length <= 1)
        {
            currentText = LocalisationManager.GetLocalisedValue(currentTextKey);
        }

        else
        {
            for (int i = 0; i < data.Length; i++)
            {
                string textaddition = LocalisationManager.GetLocalisedValue(data[i]);

                if (textaddition.StartsWith("["))
                {
                    Debug.Log("hoi");
                    string [] seperatespeaker = textaddition.Split(new string[] { "]" }, StringSplitOptions.None);

                    string dataspeaker = seperatespeaker[0].Replace("[", "");
                    dataspeaker = dataspeaker.Replace("]", "");
                    textaddition = LocalisationManager.GetLocalisedValue(dataspeaker);
                    textaddition += seperatespeaker[1];

                    textaddition = "<color=#F19C9C>" + textaddition + "</color>";
                }

                currentText += textaddition;


                if (i < data.Length-1)
                {
                    currentText += "\n";
                }
            }
        }

        textBox.GetComponent<TextMeshProUGUI>().text = currentSpeaker + currentText;
        textBox.GetComponent<TextMeshProUGUI>().color = ChangeTextColor(singleLine.speaker);

        yield return null;

        //adjust Panel
        float textWidth = textBox.GetComponent<TextMeshProUGUI>().renderedWidth;
        float textHeigth = textBox.GetComponent<TextMeshProUGUI>().renderedHeight;
        ChangeBGPanelSize(textHeigth, textWidth);
        yield return new WaitForSeconds (singleLine.displayTime);
    }

    //change character color
    private Color ChangeTextColor(Speaker speaker)
    {
        Color _color = defaultColor;

        foreach(colorPerPerson person in colorPerPeople)
        {
            if (person.speakerkey == speaker)
            {
                _color = person.color;
                break;
            }
        }
        //print(_color);
        return _color;
    }


    //BG Panel
    private void ChangeBGPanelSize(float _height, float _width)
    {
        bgPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(_width + buffer, _height);
    }


}
