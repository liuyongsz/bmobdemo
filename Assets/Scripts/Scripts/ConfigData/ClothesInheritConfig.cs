using UnityEngine;
using System.Collections.Generic;
using TinyBinaryXml;

public class ClothesInherit
{
    public int id;
    public string cost;
    public int percent;
}
public class ClothesInheritConfig
{

    private static Dictionary<int, ClothesInherit> configList = new Dictionary<int, ClothesInherit>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("ClothesInheritConfig", AssetBundles.EResType.E_BYTE, LoadClothesInheritInfo, UtilTools.errorload);
    }

    static void LoadClothesInheritInfo(AssetBundles.NormalRes data)
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
        ClothesInherit info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new ClothesInherit();
            info.id = node.GetIntValue("id");
            info.cost = node.GetStringValue("cost");
            info.percent = node.GetIntValue("percent");
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
    public static ClothesInherit GetClothesInheritInfoByID(int id)
    {
        if (configList.ContainsKey(id))
        {
            return configList[id];
        }
        return null;
    }
}
