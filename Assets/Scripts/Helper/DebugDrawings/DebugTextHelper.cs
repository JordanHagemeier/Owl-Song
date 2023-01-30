using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

///////////////////////////////////////////////////////////////////////////

public enum DebugTextFeature
{
    Test,
    CheatKeys,
    Network,
    AIBehaviours,
}

///////////////////////////////////////////////////////////////////////////

[RequireComponent(typeof(TextMeshProUGUI))]
public class DebugTextHelper : MonoBehaviour
{
    private TextMeshProUGUI m_TextField;

    struct DebugTextInfo
    {
        public string Text;
        public float? ExpiresAt;
    }

    private Dictionary<DebugTextFeature, List<DebugTextInfo>> m_StringPerFeature = new Dictionary<DebugTextFeature, List<DebugTextInfo>>();

    ///////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        m_TextField = GetComponent<TextMeshProUGUI>();
    }

    ///////////////////////////////////////////////////////////////////////////

    public void Update()
    {
        if (IsActive())
        {
            UpdateTextField();
        }
        else
        {
            if (m_StringPerFeature.Count > 0)
            {
                ClearAllTexts();
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    private bool IsActive()
    {
        //return Singletons.cheatManager.CheckForActiveDebugText();
        return Singletons.debugDrawingManager.enableDebugDrawings;
    }

    ///////////////////////////////////////////////////////////////////////////

    private void UpdateTextField()
    {
        string finalText = "";
        foreach (var featureToTextInfos in m_StringPerFeature)
        {
            List<DebugTextInfo> textInfos = featureToTextInfos.Value;

            // 1) Delete expired texts
            for (int i = textInfos.Count - 1; i >= 0; --i)
            {
                if (textInfos[i].ExpiresAt.HasValue && Time.realtimeSinceStartup >= textInfos[i].ExpiresAt.Value)
                {
                    textInfos.RemoveAt(i);
                }
            }

            // 2) Sum up current Feature
            string curFeatureText = "";

            foreach (DebugTextInfo info in textInfos)
            {
                if (info.Text == "")
                {
                    continue;
                }

                if (curFeatureText != "")
                {
                    curFeatureText += "\n";
                }

                curFeatureText += info.Text;
            }

            // 3) Sum up to all features
            if (curFeatureText == "")
            {
                continue;
            }

            if (finalText != "")
            {
                finalText += "\n";
            }

            //Color featureColor = MathHelper.GetRandomColor_NotTooDark((int)featureToTextInfos.Key);
            //featureColor = Color.Lerp(featureColor, Color.white, 0.85f);

            //finalText += "<color=#" + ColorUtility.ToHtmlStringRGBA(featureColor) + ">";
            finalText += curFeatureText;
            finalText += "</color>";
        }

        this.CallGetComponent_InEditor_IfNecessary<TextMeshProUGUI>(ref m_TextField);

        m_TextField.text = finalText;
    }

    ///////////////////////////////////////////////////////////////////////////

    public void AddText(DebugTextFeature feature, string str, float? displayDuration = null, bool clearFeatureFirst = false)
    {
        float? expiresAt = displayDuration.HasValue ? (Time.realtimeSinceStartup + displayDuration.Value) : (float?)null;

        if (!IsActive())
        {
            return;
        }

        List < DebugTextInfo > featureList = GetOrCreateFeatureList(feature, clearFeatureFirst);
        featureList.Add(new DebugTextInfo() { Text = str, ExpiresAt = expiresAt });

        UpdateTextField();
    }

    ///////////////////////////////////////////////////////////////////////////

    public void SetText(DebugTextFeature feature, string str, float? displayDuration = null)
    {
        AddText(feature, str, displayDuration, true);
    }

    ///////////////////////////////////////////////////////////////////////////

    List<DebugTextInfo> GetOrCreateFeatureList(DebugTextFeature feature, bool clearList)
    {
        List<DebugTextInfo> list;
        if (m_StringPerFeature.TryGetValue(feature, out list))
        {
            if (clearList)
            {
                list.Clear();
            }

            return list;
        }

        list = new List<DebugTextInfo>();
        m_StringPerFeature.Add(feature, list);

        return list;
    }

    ///////////////////////////////////////////////////////////////////////////

    public void ClearText(DebugTextFeature feature)
    {
        if (!IsActive())
        {
            return;
        }

        m_StringPerFeature.Remove(feature);

        UpdateTextField();
    }

    ///////////////////////////////////////////////////////////////////////////

    public void ClearAllTexts()
    {
        Debug.Log("Clearing all Debug Infos");

        m_StringPerFeature.Clear();
        UpdateTextField();
    }
}
