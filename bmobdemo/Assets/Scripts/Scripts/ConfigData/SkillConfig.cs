using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class SkillInfo
{
    public int id;
    public int index;
    public string name;
    public int type;
}

public struct TD_Skill
{
    public int ID;
    public string name;
    public Skill_Type type;
    public string icon;
    public string des;

    public TD_Skill Clone()
    {
        TD_Skill cItm = new TD_Skill();
        cItm.ID = ID;
        cItm.type = type;

        return cItm;
    }
}

class SkillConfig : ConfigBase
{
    private Dictionary<int, TD_Skill> m_data;

    private UnityAction m_callBack;
    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        m_callBack = loadedFun;
        LoadData("Skill", loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
 
        if (asset == null)
            return;

        m_data = new Dictionary<int, TD_Skill>();

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return;
        }

        List<TbXmlNode> xmlNodeList = docNode.GetNodes("Object/Property");
        int xmlNodeListLength = xmlNodeList.Count;

        if (xmlNodeListLength < 1)
        {
            return;
        }

        TD_Skill item;
        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;

            item = new TD_Skill();
            item.ID = node.GetIntValue("ID");
            item.name = node.GetStringValue("name");
            item.type = (Skill_Type)node.GetIntValue("type");
            item.icon = node.GetStringValue("icon");
            item.des = node.GetStringValue("des");

            m_data[item.ID] = item;
        }

        asset = null;

        if (null != m_callBack)
        {
            m_callBack();
            m_callBack = null;
        }  

        base.onloaded(data);
    }

    /// <summary>
    /// 获取项
    /// </summary>
    /// <returns></returns>
    public TD_Skill GetItem(string skillId)
    {
        int sid = int.Parse(skillId);
        return GetItem(sid);
    }

    /// <summary>
    /// 获取项
    /// </summary>
    /// <returns></returns>
    public TD_Skill GetItem(int skillId)
    {
        TD_Skill itm;

        if (m_data.TryGetValue(skillId, out itm))
        {
            return itm;
        }

        return itm;
    }
}

