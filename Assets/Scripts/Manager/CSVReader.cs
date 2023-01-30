using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CSVReader :MonoBehaviour
{
    public TextAsset textAssetData;

    private string[] locales = { "en","de","es","gr","ru"};

    public Dictionary<string, Dictionary<string,string>> AllLocalisedText = new Dictionary<string, Dictionary<string, string>> ();


    public void CreateLocalisationDictionary()
    {
        AllLocalisedText.Clear();
        string[] data = textAssetData.text.Split(new string[] { "#", "\n" }, StringSplitOptions.None);

        string currenttextkey = "";
        int currentlocale = 0;

        foreach (string locale in locales)
        {
            if (!AllLocalisedText.ContainsKey(locale))
            { AllLocalisedText.Add(locale, new Dictionary<string, string>()); }
        }

        foreach (string entry in data)
        {

            if (entry != "" && entry.Substring(0, 1) == "$")
            {
                currenttextkey = entry;
                currentlocale = 0;
            }

            else if (currenttextkey != "" && currentlocale < locales.Length)
            {
                string _entry = entry.Replace("\r", "");

                if (!AllLocalisedText[locales[currentlocale]].ContainsKey(currenttextkey))
                {
                    AllLocalisedText[locales[currentlocale]].Add(currenttextkey, _entry);
                }

                else
                {
                    AllLocalisedText[locales[currentlocale]].Remove(currenttextkey);
                    AllLocalisedText[locales[currentlocale]].Add(currenttextkey, _entry);
                }

                currentlocale++;
            }
        }

        /*
        foreach (string locale in AllLocalisedText.Keys)
        {
            foreach (string key in AllLocalisedText[locale].Keys)
            {
                print(key + " " + AllLocalisedText[locale][key]);
            }
        }*/
    }

    public Dictionary <string,string> GetDictionaryValues (string locale)
    {
        Dictionary <string,string> dictionary= new Dictionary<string, string>();

        if (AllLocalisedText.ContainsKey(locale))
        {
            dictionary = AllLocalisedText[locale];
        }
        return dictionary;
    }
    
}
