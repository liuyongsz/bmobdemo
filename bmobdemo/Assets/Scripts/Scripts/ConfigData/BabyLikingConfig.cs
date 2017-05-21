using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class BabyStar
{
    public int id;
    public string extent;
    public int state;
    public string addvalue;  
}

public class BabyLikingConfig : ConfigBase
{
    private static Dictionary<int, BabyStar> configList = new Dictionary<int, BabyStar>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("BabyLikingConfig", AssetBundles.EResType.E_BYTE, LoadClothesConfig, UtilTools.errorload);
    }

    static void LoadClothesConfig(AssetBundles.NormalRes data)
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
        BabyStar info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new BabyStar();
            info.id = node.GetIntValue("id");
            info.extent = node.GetStringValue("extent");
            info.state = node.GetIntValue("state");
            info.addvalue = node.GetStringValue("addvalue");
            if (configList.ContainsKey(info.id))
            {
                configList[info.id] = info;
            }
            else
            {
                configList.Add(info.id, info);
            }
        }
        asset = null;
    }

    public static BabyStar GetBabyStarByStar(int id)
    {
        if (configList.ContainsKey(id))
            return configList[id];
        return null;
    }
    public static BabyStar GetBabyStarByLiking(int liking)
    {
        int min = 0;
        int max = 0;
        foreach (BabyStar item in configList.Values)
        {
            min = UtilTools.IntParse(item.extent.Split('-')[0]);
            max = UtilTools.IntParse(item.extent.Split('-')[1]);
            if (liking >= min && liking <= max)
            {
                return item;
            }
        }
        return null;
    }
    
}
