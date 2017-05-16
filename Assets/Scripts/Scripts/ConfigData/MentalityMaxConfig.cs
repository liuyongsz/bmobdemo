 using System.Collections.Generic;
using TinyBinaryXml;

public class MentalityMaxConfig : ConfigBase
{
    static Dictionary<int, MentalityMaxInfo> m_confidata;


    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        LoadData("PlayerMentality", loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
    {
        if (null != m_confidata) return;
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
        if (asset == null)
            return;

        m_confidata = new Dictionary<int, MentalityMaxInfo>();

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
            MentalityMaxInfo childItem;
            childNode = xmlNodeList[i] as TbXmlNode;
            childItem = new MentalityMaxInfo();
            childItem.type = childNode.GetIntValue("Type");
            childItem.shoot = childNode.GetIntValue("shoot");
            childItem.pass = childNode.GetIntValue("pass");
            childItem.reel = childNode.GetIntValue("reel");
            childItem.control = childNode.GetIntValue("control");
            childItem.def = childNode.GetIntValue("def");
            childItem.trick = childNode.GetIntValue("trick");
            childItem.steal = childNode.GetIntValue("steal");
            childItem.keep = childNode.GetIntValue("keep");
            childItem.material = childNode.GetStringValue("material");
            if (m_confidata.ContainsKey(childItem.type))
                m_confidata[childItem.type] = childItem;
            else
                m_confidata.Add(childItem.type, childItem);
        }
        asset = null;
        base.onloaded(data);
    }

    public static MentalityMaxInfo GetMentalityMaxInfo(int type)
    {
        if (m_confidata.ContainsKey(type))
        {
            return m_confidata[type];
        }
        return null;
    }
}

public class MentalityMaxInfo
{
    public int type;          // 卡牌类型
    public int shoot;       // 射门
    public int pass;        // 传球
    public int reel;        // 带盘
    public int control;     // 控球
    public int def;         // 防守
    public int trick;       // 拦截
    public int steal;       // 抢断
    public int keep;        // 守门
    public string material;   // 材料
}