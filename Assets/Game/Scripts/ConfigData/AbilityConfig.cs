using System.Collections.Generic;
using TinyBinaryXml;


public class AbilityItem: PlayerItem
{

}

public class AbilityConfig : ConfigBase
{
    static Dictionary<int, AbilityItem> m_confidata;

    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        LoadData("PlayerPower", loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
    {
        if (null != m_confidata) return;
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
        if (asset == null)
            return;

        m_confidata = new Dictionary<int, AbilityItem>();

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

        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode childNode;
            AbilityItem childItem;
            childNode = xmlNodeList[i] as TbXmlNode;
            childItem = new AbilityItem();
            childItem.level = childNode.GetIntValue("level");
            childItem.shoot = childNode.GetIntValue("shoot");
            childItem.pass = childNode.GetIntValue("pass");
            childItem.reel = childNode.GetIntValue("reel");
            childItem.keep= childNode.GetIntValue("keep");
            childItem.control = childNode.GetIntValue("control");
            childItem.def = childNode.GetIntValue("def");
            childItem.trick = childNode.GetIntValue("trick");
            childItem.steal = childNode.GetIntValue("steal");
            childItem.needExp = childNode.GetIntValue("exp");
            m_confidata[childItem.level] = childItem;
        }
        asset = null;
        base.onloaded(data);
    }

    public static AbilityItem GetAbilityItem(int level)
    {
        if (m_confidata.ContainsKey(level))
        {
            return m_confidata[level];
        }
        return null;
    }

    public static Dictionary<int, AbilityItem> GetAbilityList()
    {
        return m_confidata;
    }
}
