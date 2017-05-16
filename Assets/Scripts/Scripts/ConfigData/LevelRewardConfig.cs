using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Text;
using TinyBinaryXml;
using System.Collections.Generic;


public class LevelRewardInfo
{
    public int id;
    public int exp;
    public int Euro;
    public string preview1;
    public int star;
}

public class LevelRewardConfig  {

    public static Dictionary<int, LevelRewardInfo> levelconfigDic = new Dictionary<int, LevelRewardInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("LevelReward", AssetBundles.EResType.E_BYTE, LoadLevelRewardInfo, UtilTools.errorload);
    }

    static void LoadLevelRewardInfo(AssetBundles.NormalRes data)
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
        LevelRewardInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new LevelRewardInfo();
            info.id = node.GetIntValue("id");
            info.exp = node.GetIntValue("exp");
            info.Euro = node.GetIntValue("Euro");
            info.preview1 = node.GetStringValue("preview1");
            if (levelconfigDic.ContainsKey(info.id))
            {
                levelconfigDic[info.id] = info;
            }
            else
            {
                levelconfigDic.Add(info.id, info);
            }
        }
        asset = null;
    }

    public static LevelRewardInfo GetLevelRewardInfobyID(int id)
    {
        if (levelconfigDic.ContainsKey(id))
        {
            return levelconfigDic[id];
        }
        return null;
    }
}
