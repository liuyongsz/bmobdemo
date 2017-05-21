using UnityEngine;
using System.Collections.Generic;
using TinyBinaryXml;

public class SuitInfo
{
    public int id;
    public string nameID;
    public string equipId;
    public string suitAdd;
}

public class SuitConfig
{

    public static Dictionary<int, SuitInfo> configList = new Dictionary<int, SuitInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("EquipSuit", AssetBundles.EResType.E_BYTE, LoadSuitConfig, UtilTools.errorload);
    }
    public static void LoadSuitConfig(AssetBundles.NormalRes data)
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
        SuitInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new SuitInfo();
            info.id = node.GetIntValue("id");
            info.nameID = node.GetStringValue("name");
            info.equipId = node.GetStringValue("equiq");
            info.suitAdd = node.GetStringValue("suitadd");
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

    public static SuitInfo GetSuitInfoByID(int id)
    {
        if (configList.ContainsKey(id))
        {
            return configList[id];
        }
        return null;
    }
}
