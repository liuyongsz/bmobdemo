using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class ArenaReward
{
    public int id;
    public string arenaReward;
}

public class ArenaConfig : ConfigBase
{
    private static Dictionary<int, ArenaReward> configList = new Dictionary<int, ArenaReward>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("ArenaConfig", AssetBundles.EResType.E_BYTE, LoadArenaConfig, UtilTools.errorload);
    }

    static void LoadArenaConfig(AssetBundles.NormalRes data)
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
        ArenaReward info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new ArenaReward();
            info.id = node.GetIntValue("ID");
            info.arenaReward = node.GetStringValue("reward");
            if (configList.ContainsKey(info.id))
                configList[info.id] = info;
            else
                configList.Add(info.id, info);
        }
        asset = null;
    }

    public static ArenaReward GetArenaRewardByRank(int rank)
    {       
        if (configList.ContainsKey(rank))
            return configList[rank];
        ArenaReward info = new ArenaReward();
        foreach (ArenaReward item in configList.Values)
        {
            if (rank < item.id)
                return info;
            info = item;
        }
        return null;
    }
}
