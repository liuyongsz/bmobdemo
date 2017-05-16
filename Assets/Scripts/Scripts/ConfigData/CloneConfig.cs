using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class TD_Clone
{
    public int id;
    public string name;
    public int openLv;
    public string targetDes;
}

class CloneConfig : ConfigBase
{
    private Dictionary<int, TD_Clone> m_teamData;

    private UnityAction m_callBack;
    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        m_callBack = loadedFun;
        LoadData("Clone", loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
 
        if (asset == null)
            return;

        m_teamData = new Dictionary<int, TD_Clone>();

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

        TD_Clone item;
        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;

            item = new TD_Clone();
            item.id = node.GetIntValue("ID");
            item.name = node.GetStringValue("name");
            item.openLv = node.GetIntValue("openLv");
            item.targetDes = node.GetStringValue("targetDes");

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
    public TD_Clone GetItem(int id)
    {
        TD_Clone itm;

        if (m_teamData.TryGetValue(id, out itm))
        {
            return itm;
        }

        return itm;
    }
}

