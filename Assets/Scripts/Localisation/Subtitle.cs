using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct Line
{
    //add identifier to connect to event
    ///////////////////////////////////

    public Speaker speaker;

    [TextArea(1, 3)]
    public string text;

    [Range(.1f, 10.0f)]
    public float displayTime;
}

[CreateAssetMenu (fileName = "New Subtitle", menuName = "Subtitle")]
public class Subtitle : ScriptableObject
{
    public Line[] lines;
}

public enum Speaker
{
    NONE,
    MC,
    VICTIM,
    CUSTOMER,
    GUIDE,
    MALE_RADIO_SPEAKER,
    FEMALE_RADIO_SPEAKER,
    MOM,
    GRANDPA,
    YOUNG_MC,
    POLICE,
    INTERVIEWER
}