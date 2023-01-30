using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum Language
{
    English,
    German,
    Spanish,
    Russian,
    Greek
}

public class LocalisationManager :MonoBehaviour
{
    public static event Action OnLanguageChanged;

    public static bool isInitialized;
    public static Language language = Language.English;

    private static Dictionary<string, string> localisedEN;
    private static Dictionary<string, string> localisedDE;
    private static Dictionary<string, string> localisedES;
    private static Dictionary<string, string> localisedRU;
    private static Dictionary<string, string> localisedGR;

    private static Queue<Language> possibleLanguages = new Queue<Language>();

    public static event Action<CornerTextLocaliser.TranslatedInteractions[]> OnCornerTextChanged;

    private void Awake()
    {
        //checks system language that is supported
        
        /*switch (Application.systemLanguage)
        {
            case SystemLanguage.German:
                language = Language.German;
                break;
            case SystemLanguage.Greek:
                language = Language.Greek;
                break;
            case SystemLanguage.Russian:
                language = Language.Russian;
                break;
            case SystemLanguage.Spanish:
                language = Language.Spanish;
                break;
        }*/

        Init();
    }

    public static void Init()
    {
        Singletons.csvReader.CreateLocalisationDictionary();

        localisedEN = Singletons.csvReader.GetDictionaryValues("en");
        localisedDE = Singletons.csvReader.GetDictionaryValues("de");
        localisedES = Singletons.csvReader.GetDictionaryValues("es");
        localisedRU = Singletons.csvReader.GetDictionaryValues("ru");
        localisedGR = Singletons.csvReader.GetDictionaryValues("gr");

        possibleLanguages.Clear();

        if(language != Language.English) { possibleLanguages.Enqueue(Language.English); }
        if(language != Language.German)  { possibleLanguages.Enqueue(Language.German); }
        if(language != Language.Spanish) { possibleLanguages.Enqueue(Language.Spanish); }
        //if(language != Language.Russian) { possibleLanguages.Enqueue(Language.Russian); }
        //if(language != Language.Greek)   { possibleLanguages.Enqueue(Language.Greek); }

        isInitialized = true;
    }

    public static string GetLocalisedValue (string key)
    {
        if (!isInitialized)
        {
            Init();
        }

        string value = key;

        switch(language)
        {
            case Language.English:
                if (localisedEN.ContainsKey(key)) 
                {
                    localisedEN.TryGetValue(key, out value);
                }
                break;
            case Language.German:
                if (localisedDE.ContainsKey(key))
                {
                    localisedDE.TryGetValue(key, out value);
                }
                break;
            case Language.Spanish:
                if (localisedES.ContainsKey(key))
                {
                    localisedES.TryGetValue(key, out value);
                }
                break;
            case Language.Russian:
                if (localisedES.ContainsKey(key))
                {
                    localisedRU.TryGetValue(key, out value);
                }
                break;
            case Language.Greek:
                if (localisedES.ContainsKey(key))
                {
                    localisedGR.TryGetValue(key, out value);
                }
                break;
        }

        return value;
    }

    public void ChangeLanguage()
    {
        possibleLanguages.Enqueue(language);
        language = possibleLanguages.Dequeue();
        OnLanguageChanged();
    }


    //Debug cheat
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.K))
            {
                CornerTextLocaliser.TranslatedInteractions[] translateds = {CornerTextLocaliser.TranslatedInteractions.None};
                ChangeLanguage();
                OnCornerTextChanged(translateds);
            }
    }
}
