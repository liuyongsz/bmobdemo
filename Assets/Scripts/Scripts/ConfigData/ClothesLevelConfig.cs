using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class ClothesLevelInfo
{
    public int levelId;
    public int addValue;
    public int lockValue;
    public int needExp;
    public int needMoney;
}

public class ClothesLevelConfig
{
    private static Dictionary<int, ClothesLevelInfo> configList = new Dictionary<int, ClothesLevelInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("ClothesLevelConfig", AssetBundles.EResType.E_BYTE, LoadClothesLevelInfo, UtilTools.errorload);
    }

   static void LoadClothesLevelInfo(AssetBundles.NormalRes data)
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
        ClothesLevelInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new ClothesLevelInfo();
            info.levelId = node.GetIntValue("id");
            info.addValue = node.GetIntValue("addvalue");
            info.lockValue = node.GetIntValue("lockvalue"); 
            info.needExp = node.GetIntValue("needexp");
            info.needMoney = node.GetIntValue("needmoney");
            if (configList.ContainsKey(info.levelId))
            {
                configList[info.levelId] = info;
            }
            else
            {
                configList.Add(info.levelId, info);
            }
        }
        asset = null;
    }

    public static ClothesLevelInfo GetClothesLevelInfoByID(int id)
    {
        if (configList.ContainsKey(id))
        {
            return configList[id];
        }
        return null;
    }
}
