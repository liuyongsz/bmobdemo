using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyBinaryXml;

public class CupRewardInfo
{
    public int id;
    public string cup1;
    public int star1;
    public string cup2;
    public int star2;
    public string cup3;
    public int star3;
    public string cup4;
    public int star4;
}


public class CupRewardConfig{

    public static Dictionary<int, CupRewardInfo> CupconfigDic = new Dictionary<int, CupRewardInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("CupReward", AssetBundles.EResType.E_BYTE, LoadCupRewardInfo, UtilTools.errorload);
    }
    static void LoadCupRewardInfo(AssetBundles.NormalRes data)
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
        CupRewardInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new CupRewardInfo();
            info.id = node.GetIntValue("id");
            info.cup1 = node.GetStringValue("cup1");
            info.star1 = node.GetIntValue("star1");
            info.cup2 = node.GetStringValue("cup2");
            info.star2 = node.GetIntValue("star2");
            info.cup3 = node.GetStringValue("cup3");
            info.star3 = node.GetIntValue("star3");
            info.cup4 = node.GetStringValue("cup4");
            info.star4 = node.GetIntValue("star4");
            if (CupconfigDic.ContainsKey(info.id))
            {
                CupconfigDic[info.id] = info;
            }
            else
            {
                CupconfigDic.Add(info.id, info);
            }
        }
        asset = null;
    }
    public static CupRewardInfo GetLevelRewardInfobyID(int id)
    {
        if (CupconfigDic.ContainsKey(id))
        {
            return CupconfigDic[id];
        }
        return null;
    }
}
