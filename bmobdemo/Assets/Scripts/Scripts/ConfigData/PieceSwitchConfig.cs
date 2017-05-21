using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyBinaryXml;


public class PieceSwitchConfig : ConfigBase
{

    public static Dictionary<string, RewardInfo> configList = new Dictionary<string, RewardInfo>();

    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        LoadData("SwitchPieces", loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
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
            info.itemID = node.GetStringValue("ID");
            info.itemType= node.GetIntValue("Type");
            info.amount = node.GetIntValue("Contract");
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
        base.onloaded(data);
    }

    public static RewardInfo GetRewardInfo(string ItemID)
    {
        if (configList.ContainsKey(ItemID))
            return configList[ItemID];
        return null;
    }
}
