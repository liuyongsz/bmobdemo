using UnityEngine;
using System.Collections.Generic;
using TinyBinaryXml;

public class RankRewardInfo
{
    public int id;
    public string getName;
    public string robRewardID;
    public string rewardID;
}
public class BossRewardConfig
{
    public static Dictionary<int, RankRewardInfo> bossRewardList = new Dictionary<int, RankRewardInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("BossRankReward", AssetBundles.EResType.E_BYTE, LoadConfigInfo, UtilTools.errorload);
    }

    static void LoadConfigInfo(AssetBundles.NormalRes data)
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
        RankRewardInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new RankRewardInfo();
            info.id = node.GetIntValue("ID");
            info.robRewardID = node.GetStringValue("robRewardID");
            info.rewardID = node.GetStringValue("rewardID");
            if (bossRewardList.ContainsKey(info.id))
            {
                bossRewardList[info.id] = info;
            }
            else
            {
                bossRewardList.Add(info.id, info);
            }
        }
        asset = null;
    }

}
