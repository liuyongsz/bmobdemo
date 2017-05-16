using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class ClothesMapInfo
{
    public int mapID;
    public int haveNum;
    public string addValue;
    public string reward;
    public string suitName;
    public string spriteName;
}
public class ClothesMapConfig : ConfigBase
{
    public static Dictionary<int, ClothesMapInfo> configList = new Dictionary<int, ClothesMapInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("ClothesMap", AssetBundles.EResType.E_BYTE, LoadClothesMapConfig, UtilTools.errorload);
    }

    static void LoadClothesMapConfig(AssetBundles.NormalRes data)
    {
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
        ClothesMapInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new ClothesMapInfo();
            info.mapID = node.GetIntValue("id");
            info.addValue = node.GetStringValue("addvalue");
            info.reward = node.GetStringValue("reward"); 
            info.suitName = node.GetStringValue("suiName");
            info.spriteName = node.GetStringValue("spriteName");
            if (configList.ContainsKey(info.mapID))
            {
                configList[info.mapID] = info;
            }
            else
            {
                configList.Add(info.mapID, info);
            }
        }
        asset = null;
    }


    public static ClothesMapInfo GetClothesSlevelInfoByID(int id)
    {
        if (configList.ContainsKey(id))
        {
            return configList[id];
        }
        return null;
    }
}
