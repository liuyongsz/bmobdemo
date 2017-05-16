using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;

public class TD_PlayerPosition
{
    public int pos;                       // 编号
    public float atkPer;                  // 攻击系数
    public float defPer;                  // 防御系数
    public float controlPer;              // 控球系数
    public float passPer;                 // 传球系数
    public bool atkEnable;                // 是否参与进攻 0不能 1可以
    public string adaptDef;               // 可参与防守球员的范围 对应MatchArray表中在位置上的球员
}

class PlayerPositionConfig : ConfigBase
{
    Dictionary<int, TD_PlayerPosition> m_confidata;

    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        LoadData("PlayerPosition",loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
    {
        if (null != m_confidata) return;
        m_confidata = new Dictionary<int, TD_PlayerPosition>();

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

        TD_PlayerPosition item;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;

            item = new TD_PlayerPosition();
            item.pos = node.GetIntValue("pos");
            item.atkPer = node.GetFloatValue("atkPer");
            item.defPer = node.GetFloatValue("defPer");
            item.controlPer = node.GetFloatValue("controlPer");
            item.passPer = node.GetFloatValue("passPer");
            item.atkEnable = node.GetIntValue("atkEnable") == 1 ? true : false;
            item.adaptDef = node.GetStringValue("adaptDef");

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
    public TD_PlayerPosition GetItem(int id)
    {
        TD_PlayerPosition itm;

        if (m_confidata.TryGetValue(id, out itm))
        {
            return itm;
        }

        return itm;
    }
}

