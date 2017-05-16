using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public class PropertySkillLevelInfo : SkillInfo
{
    public int id;   
    public int addValue;  
}
public class PropertySkillLevelConfig : ConfigBase
{

    public static Dictionary<int, PropertySkillLevelInfo> propertySkillList = new Dictionary<int, PropertySkillLevelInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("propertySkillLevel", AssetBundles.EResType.E_BYTE, LoadSkillInfo, UtilTools.errorload);
    }

    static void LoadSkillInfo(AssetBundles.NormalRes data)
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
        PropertySkillLevelInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new PropertySkillLevelInfo();
            info.id = node.GetIntValue("ID");           
            info.addValue = node.GetIntValue("addValue");
            if (propertySkillList.ContainsKey(info.id))
            {
                propertySkillList[info.id] = info;
            }
            else
            {
                propertySkillList.Add(info.id, info);
            }
        }
        asset = null;
    }

    public static PropertySkillLevelInfo GetSkillLevelInfo(int id)
    {
        if (propertySkillList.ContainsKey(id))
            return propertySkillList[id];
        return null;
    }
}
