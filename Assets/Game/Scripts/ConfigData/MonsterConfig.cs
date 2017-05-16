using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class TD_Monster : PlayerItem
{
    public int id;
    public string name;
    public string icon;
    public int needLv;
}

class MonsterConfig : ConfigBase
{
    private Dictionary<int, TD_Monster> m_teamData;

    private UnityAction m_callBack;
    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        m_callBack = loadedFun;
        LoadData("Monster", loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
 
        if (asset == null)
            return;

        m_teamData = new Dictionary<int, TD_Monster>();

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

        TD_Monster item;
        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;

            item = new TD_Monster();
            item.id = node.GetIntValue("ID");
            item.name = node.GetStringValue("name");
            item.icon = node.GetStringValue("icon");
            item.needLv = node.GetIntValue("needLv");
            item.shoot = node.GetIntValue("shoot");
            item.pass = node.GetIntValue("pass");
            item.reel = node.GetIntValue("reel");
            item.tech = node.GetIntValue("tech");
            item.control = node.GetIntValue("control");
            item.def = node.GetIntValue("def");
            item.trick = node.GetIntValue("trick");
            item.steal = node.GetIntValue("steal");
            item.health = node.GetIntValue("health");
            item.keep = node.GetIntValue("keep");

            m_teamData[item.id] = item;
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
    /// <param name="langId"></param>
    /// <returns></returns>
    public TD_Monster GetItem(int id)
    {
        TD_Monster itm;

        if (m_teamData.TryGetValue(id, out itm))
        {
            return itm;
        }

        return itm;
    }
}

