using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;


public class SkillLevelInfo : SkillInfo
{
    public int limit;
}
public class SkillLevelConfig : ConfigBase
{

    public static Dictionary<int, SkillLevelInfo> skillInfoList = new Dictionary<int, SkillLevelInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("SkillLevel", AssetBundles.EResType.E_BYTE, LoadSkillInfo, UtilTools.errorload);
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
        SkillLevelInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new SkillLevelInfo();
            info.id = node.GetIntValue("ID");           
            info.limit = node.GetIntValue("limit");
            if (skillInfoList.ContainsKey(info.id))
            {
                skillInfoList[info.id] = info;
            }
            else
            {
                skillInfoList.Add(info.id, info);
            }
        }
        asset = null;
    }

    public static SkillLevelInfo GetSkillLevelInfo(int id)
    {
        if (skillInfoList.ContainsKey(id))
            return skillInfoList[id];
        return null;
    }
}
