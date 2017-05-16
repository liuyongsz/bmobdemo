using UnityEngine;
using System.Collections.Generic;
using TinyBinaryXml;

public class BossDailyInfo
{
    public int day;
    public int bossID;
    public string openTime;
}
public class BossDailyConfig
{
    private static Dictionary<int, BossDailyInfo> bossDailyList = new Dictionary<int, BossDailyInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("BossDaily", AssetBundles.EResType.E_BYTE, LoadConfigInfo, UtilTools.errorload);
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
        BossDailyInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new BossDailyInfo();
            info.day = node.GetIntValue("ID");
            info.bossID = node.GetIntValue("boosID");
            info.openTime = node.GetStringValue("openTime");
            if (bossDailyList.ContainsKey(info.day))
            {
                bossDailyList[info.day] = info;
            }
            else
            {
                bossDailyList.Add(info.day, info);
            }
        }
        asset = null;
    }

    public static BossDailyInfo GetBossDailyInfoByDay(int id)
    {
        if (bossDailyList.ContainsKey(id))
        {
            return bossDailyList[id];
        }
        return null;
    }
}
