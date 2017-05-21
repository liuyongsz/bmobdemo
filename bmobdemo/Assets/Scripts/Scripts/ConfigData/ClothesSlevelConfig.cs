using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class ClothesSlevelInfo
{
    public int slevelID;
    public int addValue;
    public int lockValue;
    public string cost;
}
public class ClothesSlevelConfig : ConfigBase
{
    private static Dictionary<int, ClothesSlevelInfo> configList = new Dictionary<int, ClothesSlevelInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("ClothesSlevelConfig", AssetBundles.EResType.E_BYTE, LoadClothesSlevelConfig, UtilTools.errorload);
    }

    static void LoadClothesSlevelConfig(AssetBundles.NormalRes data)
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
        ClothesSlevelInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new ClothesSlevelInfo();
            info.slevelID = node.GetIntValue("id");
            info.addValue = node.GetIntValue("addvalue");
            info.lockValue = node.GetIntValue("lockvalue");
            info.cost = node.GetStringValue("cost");

            if (configList.ContainsKey(info.slevelID))
            {
                configList[info.slevelID] = info;
            }
            else
            {
                configList.Add(info.slevelID, info);
            }
        }
        asset = null;
    }

    public static ClothesSlevelInfo GetClothesSlevelInfoByID(int id)
    {
        if (configList.ContainsKey(id))
        {
            return configList[id];
        }
        return null;
    }
}
