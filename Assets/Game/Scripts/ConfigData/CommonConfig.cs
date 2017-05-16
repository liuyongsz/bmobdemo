using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class CommonInfo
{
    public int id;
    public int value;
}
public class CommonConfig : ConfigBase
{
    private static Dictionary<int, CommonInfo> List = new Dictionary<int, CommonInfo>();
    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("Common", AssetBundles.EResType.E_BYTE, LoadCommonConfig, UtilTools.errorload);
    }
    public static void LoadCommonConfig(AssetBundles.NormalRes data)
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
        CommonInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new CommonInfo();
            info.id = node.GetIntValue("ID");
            info.value = node.GetIntValue("value");
            if (List.ContainsKey(info.id))
            {
                List[info.id] = info;
            }
            else
            {
                List.Add(info.id, info);
            }
        }
        asset = null;
    }
    public static CommonInfo GetCommonInfo(int id)
    {
        if (List.ContainsKey(id))
        {
            return List[id];
        }
        return null;
    }
}
