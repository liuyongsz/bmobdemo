using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;

public struct TD_MatchArray
{
    public int id;          //编号
    public string name;     //名字
    public string[] array;    //身高
}


class MatchArrayConfig : ConfigBase
{
    Dictionary<int, TD_MatchArray> m_confidata;

    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        LoadData("MatchArray", loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
    {
        if (null != m_confidata) return;
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
        if (asset == null)
            return;

        m_confidata = new Dictionary<int, TD_MatchArray>();
        TD_MatchArray item;

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return;
        }
        List<TbXmlNode> xmlNodeList = docNode.GetNodes("root/item");
        int xmlNodeListLength = xmlNodeList.Count;

        if (xmlNodeListLength < 1)
        {
            return;
        }

        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;

            item = new TD_MatchArray();
            item.id = node.GetIntValue("id");
            item.name = node.GetStringValue("name");
            item.array = node.GetStringValue("array0").Split(',');

            m_confidata[item.id] = item;
        }

        asset = null;
        base.onloaded(data);
    }

    /// <summary>
    /// 获取项
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TD_MatchArray GetItem(int id)
    {
        TD_MatchArray itm;

        if (m_confidata.TryGetValue(id, out itm))
        {
            return itm;
        }

        return itm;
    }
}

