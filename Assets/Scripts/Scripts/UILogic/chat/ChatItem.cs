using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class LaBaChatItem : ChatItem
{
    /// <summary>
    /// 更新vip king label的坐标
    /// </summary>
    protected override Vector3 ResetPosition()
    {
        Vector3 vTemp = Vector3.zero;
        vTemp.x = 160f;
        vTemp.y = 20.4f;
        vTemp.z = 0f;
        Vector3 pos = vTemp;

        LabelName.transform.localPosition = pos;
        pos.x += LabelName.width;

        if (m_vip.gameObject.activeSelf)
        {
            vTemp.x = pos.x;
            vTemp.y = 27f;
            vTemp.z = 0;
            m_vip.transform.localPosition = vTemp;
            pos.x += 45;
        }

        float offset = 30f;
        vTemp.x = 66 + (offset / 2);
        vTemp.y = -18;
        vTemp.z = 0;
        LabelText.transform.localPosition = vTemp;
        Vector3 localPos = m_labelName.transform.localPosition;
        localPos.x = 80;
        m_labelName.transform.localPosition = localPos;
        return pos;
    }
}

public class LeftChatItem : ChatItem
{
    /// <summary>
    /// 更新vip king label的坐标
    /// </summary>
    protected override Vector3 ResetPosition()
    {

        Vector3 vTemp = Vector3.zero;
        Vector3 pos = vTemp;
        return pos;
    }
}


public class RightChatItem : ChatItem
{
    /// <summary>
    /// 更新vip king label的坐标
    /// </summary>
    override protected Vector3 ResetPosition()
    {     
        Vector3 vTemp = Vector3.zero;
        Vector3 pos = vTemp;
        return pos;
    }
}

/// <summary>
/// 因为需要统一处理 ChatItem  与 MainChatItem 所以定义一个父类
/// </summary>
public class ChatItemBase : MonoBehaviour
{
    public virtual ChatInfo Data { get; set; }
    public virtual SystemChatInfo data { get; set; }
}

abstract public class ChatItem : ChatItemBase
{

    /// <summary>
    /// VIP 与 Time 的间隔距离
    /// </summary>
    protected const int vip_time_spacing = 15;

    public enum InfoType
    {
        Text,
        Name,
        Face,
    }

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

    protected UILabel m_vip;


    protected UISprite m_head;

    public UISprite head { get { return m_head; } }

    public ChatInfo chatInfo;

   

    protected bool isInit = false;

  
    //语音宽度/
    //private int defaultWidth;

    void Awake()
    {
        isInit = true;
        m_label = UtilTools.GetChild<UILabel>(transform, "Label");
        m_vip = UtilTools.GetChild<UILabel>(transform, "Vip"); 
        m_labelName = UtilTools.GetChild<UILabel>(transform, "Name");
        m_labelTime = UtilTools.GetChild<UILabel>(transform, "time");
        m_head = UtilTools.GetChild<UISprite>(transform, "head"); 
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

    override public ChatInfo Data
    {
        get
        {
            return chatInfo;
        }

        set
        {
            if (!isInit)
            {
                Awake();
            }

            chatInfo = value;
            LabelName.text = chatInfo.name;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = startTime.AddSeconds(chatInfo.sendTime);
            LabelTime.text = dt.ToString("yyyy MM dd HH:mm:ss");
            m_vip.text = "VIP" + chatInfo.vipLevel;
            m_label.text = chatInfo.message;      
           
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

