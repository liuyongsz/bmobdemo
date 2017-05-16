using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class ClothesInfo
{
    public int configID;
    public int isWear;
    public int level;
    public int star;
    public int exp;   
    public int type;
    public string addValue;
    public string lockAdd;
    public int lockLevel;
    public int maxLevel;
    public int initstar;
    public int maxstar;
    public int worthValue;
    public int percentType;
}
public class ClothesConfig : ConfigBase
{
    private static Dictionary<int, ClothesInfo> configList = new Dictionary<int, ClothesInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("ClothesConfig", AssetBundles.EResType.E_BYTE, LoadClothesConfig, UtilTools.errorload);
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
        ClothesInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new ClothesInfo();
            info.configID = node.GetIntValue("id");
            info.addValue = node.GetStringValue("addvalue");
            info.lockAdd = node.GetStringValue("lockadd"); 
            info.initstar = node.GetIntValue("initstar");
            info.type = node.GetIntValue("type");
            info.lockLevel = node.GetIntValue("locklevel");
            info.maxLevel = node.GetIntValue("maxlevel");
            info.maxstar = node.GetIntValue("maxstar");
            info.worthValue = node.GetIntValue("worthvalue");
            info.percentType = node.GetIntValue("percenttype");
            if (configList.ContainsKey(info.configID))
            {
                configList[info.configID] = info;
            }
            else
            {
                configList.Add(info.configID, info);
            }
        }
        asset = null;
    }

    public static ClothesInfo GetClothesByID(int id)
    {
        if (configList.ContainsKey(id))
        {
            return configList[id];
        }
        return null;
    }
}
