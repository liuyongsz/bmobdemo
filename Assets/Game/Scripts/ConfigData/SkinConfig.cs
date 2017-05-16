using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class TD_TeamSkin
{
    public int team;
    public string teamName;
    public string cloth;
    public int mainClothColor;
    public string cloth2;
    public int mainClothColor2;
    public string skin;
    public string hair;
}

public class TD_GKSkin
{
    public string cloth;
    public string cloth2;
    public string cloth3;
    public string skin;
    public string skin2;
    public string skin3;
    public string hair;
    public string hair2;
    public string hair3;
}

class SkinConfig : ConfigBase
{
    private TD_GKSkin m_gkData;
    private Dictionary<int, TD_TeamSkin> m_teamData;

    private UnityAction m_callBack;
    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        m_callBack = loadedFun;
        LoadData("Skin", loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
 
        if (asset == null)
            return;

        m_teamData = new Dictionary<int, TD_TeamSkin>();

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

        TD_TeamSkin item;
        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;

            item = new TD_TeamSkin();
            item.team = node.GetIntValue("team");
            item.teamName = node.GetStringValue("teamName");
            item.cloth = node.GetStringValue("cloth");
            item.mainClothColor = node.GetIntValue("mainClothColor");
            item.cloth2 = node.GetStringValue("cloth2");
            item.mainClothColor2 = node.GetIntValue("mainClothColor2");
            item.skin = node.GetStringValue("skin");
            item.hair = node.GetStringValue("hair");

            m_teamData[item.team] = item;
        }

        xmlNodeList = docNode.GetNodes("Object/GKProperty");
        xmlNodeListLength = xmlNodeList.Count;
        TD_GKSkin gkItem = null;
        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;

            gkItem = new TD_GKSkin();
            gkItem.cloth = node.GetStringValue("cloth");
            gkItem.cloth2 = node.GetStringValue("cloth2");
            gkItem.cloth3 = node.GetStringValue("cloth3");
            gkItem.skin = node.GetStringValue("skin");
            gkItem.skin2 = node.GetStringValue("skin2");
            gkItem.skin3 = node.GetStringValue("skin3");
            gkItem.hair = node.GetStringValue("hair");
            gkItem.hair2 = node.GetStringValue("hair2");
            gkItem.hair3 = node.GetStringValue("hair3");
        }

        m_gkData = gkItem;

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
    public TD_TeamSkin GetItem(int teamId)
    {
        TD_TeamSkin itm;

        if (m_teamData.TryGetValue(teamId, out itm))
        {
            return itm;
        }

        return itm;
    }

    /// <summary>守门员皮肤</summary>
    public TD_GKSkin GKSkin
    {
        get
        {
            return m_gkData;
        }
    }
}

