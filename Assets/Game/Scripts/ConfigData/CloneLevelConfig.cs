using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyBinaryXml;

public class TD_CloneLevel
{
    public int id;
    public string name;
    public string levels;
    public string cup1;
    public int star1;
    public string cup2;
    public int star2;
    public string cup3;
    public int star3;
    public string cup4;
    public int star4;

    private List<object> m_levels;

    /// <summary>获取管卡等级列表</summary>
    public List<object> GetLevels()
    {
        if (null != m_levels)
            return m_levels;

        m_levels = new List<object>();
        string[] strs = levels.Split(',');
        int cnt = strs.Length;

        for (int i = 0; i < cnt; i++)
            m_levels.Add(int.Parse(strs[i]));

        return m_levels;
    }
}


public class CloneLevelConfig
{

    public static Dictionary<int, TD_CloneLevel> CupconfigDic = new Dictionary<int, TD_CloneLevel>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("CloneLevel", AssetBundles.EResType.E_BYTE, LoadCupRewardInfo, UtilTools.errorload);
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
        TD_CloneLevel info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new TD_CloneLevel();
            info.id = node.GetIntValue("ID");
            info.name = node.GetStringValue("name");
            info.levels = node.GetStringValue("levels");
            info.cup1 = node.GetStringValue("cup1");
            info.star1 = node.GetIntValue("star1");
            info.cup2 = node.GetStringValue("cup2");
            info.star2 = node.GetIntValue("star2");
            info.cup3 = node.GetStringValue("cup3");
            info.star3 = node.GetIntValue("star3");
            info.cup4 = node.GetStringValue("cup4");
            info.star4 = node.GetIntValue("star4");

            CupconfigDic[info.id] = info;
        }
        asset = null;
    }
    public static TD_CloneLevel GetLevelRewardInfobyID(int id)
    {
        if (CupconfigDic.ContainsKey(id))
        {
            return CupconfigDic[id];
        }
        return null;
    }
}
