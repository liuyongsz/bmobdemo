using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyBinaryXml;

public class RewardInfo : Item
{
    public int contractType;
    public int needContract;
}
public class LotterRewardManager
{
    public static Dictionary<string, RewardInfo> configList = new Dictionary<string, RewardInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("LotterReward", AssetBundles.EResType.E_BYTE, LoadLotterRewardConfig, UtilTools.errorload);  
    }

    public static void LoadLotterRewardConfig(AssetBundles.NormalRes data)
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
        RewardInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new RewardInfo();
            info.itemID = node.GetStringValue("ItemID");
            info.amount = node.GetIntValue("Switch");
            info.itemType = node.GetIntValue("Type");
            if (configList.ContainsKey(info.itemID))
            {
                configList[info.itemID] = info;
            } 
            else
            {
                configList.Add(info.itemID, info);
            }
        }
        asset = null;
    }

    public static RewardInfo GetRewardInfo(string ItemID)
    {
        if (configList.ContainsKey(ItemID))
            return configList[ItemID];
        return null;
    }
}
