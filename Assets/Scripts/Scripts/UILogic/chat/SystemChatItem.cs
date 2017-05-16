using UnityEngine;
using System.Collections;
using System;

public class SystemChatItem : ChatItemBase
{


    //聊天内容显示label
    protected UILabel m_label;

    public UILabel LabelText
    {
        get { return m_label; }
    }

    //角色名称
    protected UILabel m_labelName;

    public UILabel LabelName
    {
        get { return m_labelName; }
    }


    protected UILabel m_labelTime;
    public UILabel LabelTime
    {
        get { return m_labelTime; }
    }


   

    public SystemChatInfo systemChatInfo;

    protected bool isInit = false;


    //语音宽度/
    //private int defaultWidth;

    void Awake()
    {
        isInit = true;
        m_label = UtilTools.GetChild<UILabel>(transform, "content");
        m_labelName = UtilTools.GetChild<UILabel>(transform, "Sprite/Label");
        m_labelTime = UtilTools.GetChild<UILabel>(transform, "time");
    }


    virtual protected void Init()
    {
        if (!isInit)
        {
            Awake();
        }
    }

    /// <summary>
    /// 获得Info对应链接参数
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public string GetInfoUrlAtPostion(Vector3 pos)
    {
        string value = string.Empty;

        if (m_label != null)
        {
            //value = m_label.labelText.GetUrlAtPosition(pos);
            if (!string.IsNullOrEmpty(value)) return value;
        }

        return value;
    }

    /// <summary>
    /// 获得Name对应链接参数
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public string GetNameUrlAtPostion(Vector3 pos)
    {
        string value = string.Empty;

        if (m_labelName != null)
        {
            value = m_labelName.GetUrlAtPosition(pos);
            if (!string.IsNullOrEmpty(value)) return value;
        }

        return value;
    }

    /// <summary>
    /// 获得对应链接值
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    virtual public string GetNameAtPosition(Vector3 pos)
    {
        if (m_label != null)
        {
            int characterIndex = m_labelName.GetCharacterIndexAtPosition(pos);
            //string name = m_labelName.GetUrlAtCharacterIndex(characterIndex);
            string text = m_labelName.text;

            if (characterIndex != -1 && characterIndex < text.Length)
            {
                int linkStart = text.LastIndexOf("]", characterIndex);

                if (linkStart != -1)
                {
                    linkStart += 1;
                    int linkEnd = text.IndexOf("[", linkStart);

                    if (linkEnd != -1)
                    {
                        int closingStatement = text.IndexOf("[/url]", linkEnd);
                        if (closingStatement == -1 || closingStatement >= characterIndex)
                            return text.Substring(linkStart, linkEnd - linkStart);
                    }
                }
            }
            return string.Empty;
        }
        else return string.Empty;
    }

    override public SystemChatInfo data
    {
        get
        {
            return systemChatInfo;
        }
        set
        {
            if (!isInit)
            {
                Awake();
            }

            systemChatInfo = value;
            LabelName.text = "系统";
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = startTime.AddSeconds(systemChatInfo.sendTime);
            LabelTime.text = dt.ToString("yyyy MM dd HH:mm:ss");
            m_label.text = systemChatInfo.message;

            ResetPosition();
        }
    }

    /// <summary>
    /// 更新vip king label的坐标
    /// </summary>
    virtual protected Vector3 ResetPosition()
    {
        Vector3 vTemp = Vector3.zero;
        vTemp.x = 0f;
        vTemp.y = 0f;
        vTemp.z = 0f;
        Vector3 pos = vTemp;
        return pos;
    }

    /// <summary>
    /// 获得声音图标位置
    /// </summary>
    /// <returns></returns>

    virtual protected Vector3 GetAudioPostion()
    {
        Vector3 v = Vector3.zero;
        return v;
    }
}
