using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class EquipUseInfo
{
    public int star;
    public int exp;
    public int money;
}
public class UseEquipConfig : ConfigBase
{
    private static Dictionary<int, EquipUseInfo> configList = new Dictionary<int, EquipUseInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("UseEquip", AssetBundles.EResType.E_BYTE, LoadUseEquipConfig, UtilTools.errorload);
    }

   static void LoadUseEquipConfig(AssetBundles.NormalRes data)
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
        EquipUseInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new EquipUseInfo();
            info.star = node.GetIntValue("id");
            info.exp = node.GetIntValue("exp");
            info.money = node.GetIntValue("money");
            if (configList.ContainsKey(info.star))
            {
                configList[info.star] = info;
            }
            else
            {
                configList.Add(info.star, info);
            }
        }
        asset = null;
    }

    public static EquipUseInfo GetClothesByStar(int id)
    {
        if (configList.ContainsKey(id))
        {
            return configList[id];
        }
        return null;
    }
}
