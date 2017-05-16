using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;

public struct TD_PositionAttribute
{
    public int pos;                         //编号
    public float powerPer;                  //威力系数
    public float distance;                  //距离
    public bool isBound;                    //是否是边界点
}

class PositionAttributeConfig : ConfigBase
{
    Dictionary<int, TD_PositionAttribute> m_confidata;

    public void LoadData(System.Action<bool, string> LoadComplete)
    {

    }

    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        LoadData("PositionAttribute", loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
    {
        if (null != m_confidata) return;
        m_confidata = new Dictionary<int, TD_PositionAttribute>();

        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
        if (asset == null)
            return;
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

        TD_PositionAttribute item;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;

            item = new TD_PositionAttribute();
            item.pos = node.GetIntValue("pos");
            item.powerPer = node.GetFloatValue("powerPer");
            item.distance = node.GetFloatValue("distance");
            item.isBound = node.GetBooleanValue("bound");

            m_confidata[item.pos] = item;
        }
        asset = null;
        base.onloaded(data);
    }

    /// <summary>
    /// 获取项
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TD_PositionAttribute GetItem(int id)
    {
        TD_PositionAttribute itm;

        if (m_confidata.TryGetValue(id, out itm))
        {
            return itm;
        }

        return itm;
    }
}

