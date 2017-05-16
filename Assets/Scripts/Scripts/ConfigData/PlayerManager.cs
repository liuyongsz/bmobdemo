using UnityEngine;
using TinyBinaryXml;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;

[SerializeField]
public class PlayerManager
{

    /// <summary>
    /// 球员位置字典表
    /// </summary>
	public static Dictionary<int, TD_PlayerAtkPosition> m_confidata = new Dictionary<int, TD_PlayerAtkPosition>();


    /// <summary>
    /// 球员字典表
    /// </summary>
    public static Dictionary<int, TD_Player> playerList = new Dictionary<int, TD_Player>();

    /// <summary>
    /// 传球或者射门的数据
    /// </summary>
    public float Gravity;

    public  PhysicMaterialVO GroundPhysis;
    public  PhysicMaterialVO BallPhysis;
    public  float ShootTimeScele;
    public  float PassTimeScale;
    public  float AngleTurnTimer;
    public  float GuoRenRange;

    public Vector3 AtkPointOffset = new Vector3(2,0,0);
    public Vector3 AddPostion;
    public Vector3 AddRotation;

    public  List<TD_PassOrShootItem> m_passData;
    public  List<TD_PassOrShootItem> m_shootData;
    public List<TD_PassDisTimer> m_passDisTimer;
    public List<TD_PushShootData> m_pushShootData;
    public List<TD_PassOrShootItem> m_skillShootData;

    /// <summary>
    /// 球员升级数据
    /// </summary>
    public static Dictionary<int, PlayerItem> playerLevelList = new Dictionary<int, PlayerItem>();


    /// <summary>
    /// 传承球员
    /// </summary>
    public static List<InheritItem> inheritItemList = new List<InheritItem>();


    /// <summary>
    /// 球员进阶字典
    /// </summary>
    public static Dictionary<int, PlayerItem> slevelList = new Dictionary<int, PlayerItem>();


    /// <summary>
    /// 球员进突破字典
    /// </summary>
    public static Dictionary<int, StrikeInfo> strikeInfoList = new Dictionary<int, StrikeInfo>();
    public void Init()
    {
        ResourceManager.Instance.LoadBytes("PlayerAtkPosition", AssetBundles.EResType.E_BYTE, LoadPlayerAtkPositionConfig, UtilTools.errorload);
        ResourceManager.Instance.LoadBytes("Player", AssetBundles.EResType.E_BYTE, LoadPlayerConfig, UtilTools.errorload);
        ResourceManager.Instance.LoadBytes("PlayerDefine", AssetBundles.EResType.E_BYTE, LoadPlayerDefineConfig, UtilTools.errorload);
        ResourceManager.Instance.LoadBytes("PlayerLevel", AssetBundles.EResType.E_BYTE, LoadPlayerLevelConfig, UtilTools.errorload);
        ResourceManager.Instance.LoadBytes("PlayerInherit", AssetBundles.EResType.E_BYTE, LoadPlayerInheritConfig, UtilTools.errorload);
        ResourceManager.Instance.LoadBytes("PlayerSlevel", AssetBundles.EResType.E_BYTE, LoadPlayerSlevelConfig, UtilTools.errorload);
        ResourceManager.Instance.LoadBytes("PlayerStrike", AssetBundles.EResType.E_BYTE, LoadPlayerStrikeConfig, UtilTools.errorload);
    }


    /// <summary>
    /// 加载球员进阶配置
    /// </summary>
    public static void LoadPlayerStrikeConfig(AssetBundles.NormalRes data)
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
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode childNode;
            StrikeInfo childItem;
            childNode = xmlNodeList[i] as TbXmlNode;
            childItem = new StrikeInfo();
            childItem.id = childNode.GetIntValue("id");         
            childItem.add = childNode.GetStringValue("addValue");
            childItem.addSkill = childNode.GetIntValue("skill");
            childItem.needCount = childNode.GetIntValue("needCount");
            childItem.shoot = childNode.GetIntValue("shoot");
            childItem.pass = childNode.GetIntValue("pass");
            childItem.keep = childNode.GetIntValue("keep");
            childItem.reel = childNode.GetIntValue("reel");
            childItem.control = childNode.GetIntValue("control");
            childItem.def = childNode.GetIntValue("def");
            childItem.trick = childNode.GetIntValue("trick");
            childItem.steal = childNode.GetIntValue("steal");
            childItem.tech = childNode.GetIntValue("tech");
            childItem.health = childNode.GetIntValue("health");
            if (strikeInfoList.ContainsKey(childItem.id))
                strikeInfoList[childItem.id] = childItem;
            else
                strikeInfoList.Add(childItem.id, childItem);
        }
        asset = null;
    }

    /// <summary>
    /// 加载球员进阶配置
    /// </summary>
    public static void LoadPlayerSlevelConfig(AssetBundles.NormalRes data)
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

        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode childNode;
            PlayerItem childItem;
            childNode = xmlNodeList[i] as TbXmlNode;
            childItem = new PlayerItem();
            childItem.star = childNode.GetIntValue("id");
            childItem.shoot = childNode.GetIntValue("shoot");
            childItem.pass = childNode.GetIntValue("pass");
            childItem.keep = childNode.GetIntValue("keep");
            childItem.reel = childNode.GetIntValue("reel");
            childItem.control = childNode.GetIntValue("control");
            childItem.def = childNode.GetIntValue("def");
            childItem.trick = childNode.GetIntValue("trick");
            childItem.steal = childNode.GetIntValue("steal");
            childItem.material = childNode.GetStringValue("material");
            slevelList[childItem.star] = childItem;
        }
        asset = null;
    }

    /// <summary>
    /// 加载传承球员配置
    /// </summary>
    public static void LoadPlayerInheritConfig(AssetBundles.NormalRes data)
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

        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode childNode;
            InheritItem childItem;
            childNode = xmlNodeList[i] as TbXmlNode;
            childItem = new InheritItem();
            childItem.itemId = childNode.GetStringValue("id");
            childItem.expRate = childNode.GetFloatValue("exp");
            childItem.skillExpRate = childNode.GetFloatValue("skillExp");
            childItem.powerLevelRate = childNode.GetFloatValue("powerLevel");
            childItem.mentalityLevelRate = childNode.GetFloatValue("MentalityLevel");
            childItem.strikeBackItemRate = childNode.GetFloatValue("StrikeBackItem");
            childItem.money = childNode.GetIntValue("money");
            inheritItemList.Add(childItem);
        }
        asset = null;
    }

    /// <summary>
    /// 加载球员等级字典表
    /// </summary>
    public static void LoadPlayerLevelConfig(AssetBundles.NormalRes data)
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

        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode childNode;
            PlayerItem childItem;
            childNode = xmlNodeList[i] as TbXmlNode;
            childItem = new PlayerItem();
            childItem.level = childNode.GetIntValue("level");
            childItem.shoot = childNode.GetIntValue("shoot");
            childItem.pass = childNode.GetIntValue("pass");
            childItem.reel = childNode.GetIntValue("reel");
            childItem.keep = childNode.GetIntValue("keep");
            childItem.control = childNode.GetIntValue("control");
            childItem.def = childNode.GetIntValue("def");
            childItem.trick = childNode.GetIntValue("trick");
            childItem.steal = childNode.GetIntValue("steal");
            childItem.needExp = childNode.GetIntValue("EXP");
            playerLevelList[childItem.level] = childItem;
        }
        asset = null;       
    }

    /// <summary>
    /// 加载传球或者射门的数据
    /// </summary>
    public void LoadPlayerDefineConfig(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
        if (asset == null)
            return;

        m_passData = new List<TD_PassOrShootItem>();
        m_shootData = new List<TD_PassOrShootItem>();
        m_passDisTimer = new List<TD_PassDisTimer>();
        m_pushShootData = new List<TD_PushShootData>();
        m_skillShootData = new List<TD_PassOrShootItem>();

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return;
        }

        TbXmlNode defNode = docNode.GetNodes("Object/Define")[0];
        Gravity = defNode.GetFloatValue("Gravity");
        PassTimeScale = defNode.GetFloatValue("PassTimeScale");
        ShootTimeScele = defNode.GetFloatValue("ShootTimeScele");
        AngleTurnTimer = defNode.GetFloatValue("AngleTurnTimer");
        GuoRenRange = defNode.GetFloatValue("GuoRenRange");

        BallPhysis = new PhysicMaterialVO();
        TbXmlNode ballNode = docNode.GetNodes("Object/Ball")[0];
        BallPhysis.DynamicFriction = ballNode.GetFloatValue("DynamicFriction");
        BallPhysis.StaticFriction = ballNode.GetFloatValue("StaticFriction");
        BallPhysis.Bounciness = ballNode.GetFloatValue("Bounciness");

        GroundPhysis = new PhysicMaterialVO();
        TbXmlNode groundNode = docNode.GetNodes("Object/Ground")[0];
        GroundPhysis.DynamicFriction = groundNode.GetFloatValue("DynamicFriction");
        GroundPhysis.StaticFriction = groundNode.GetFloatValue("StaticFriction");
        GroundPhysis.Bounciness = groundNode.GetFloatValue("Bounciness");

        TbXmlNode cameraNode = docNode.GetNodes("Object/Camera")[0];
        string AddPostionStr = cameraNode.GetStringValue("AddPostion");
        string AddRotationStr = cameraNode.GetStringValue("AddRotation");

        string[] lst = AddPostionStr.Split(',');
        AddPostion = new Vector3(float.Parse(lst[0]), float.Parse(lst[1]), float.Parse(lst[2]));
        lst = AddRotationStr.Split(',');
        AddRotation = new Vector3(float.Parse(lst[0]), float.Parse(lst[1]), float.Parse(lst[2]));

        TD_PassOrShootItem itm;
        List<TbXmlNode> xmlNodeList = docNode.GetNodes("Object/Pass");
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            itm = new TD_PassOrShootItem();
            itm.distance = node.GetFloatValue("distance");
            itm.force = node.GetFloatValue("force");
            itm.passType = node.GetStringValue("passType");
            itm.addFore = node.GetStringValue("addFore");
            itm.drag = node.GetFloatValue("drag");
            itm.airDrag = node.GetFloatValue("airDrag");

            m_passData.Add(itm);
        }

        xmlNodeList = docNode.GetNodes("Object/Shoot");
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            itm = new TD_PassOrShootItem();
            itm.id = node.GetIntValue("ID");
            itm.point = node.GetStringValue("point");
            itm.speed = node.GetFloatValue("speed");
            itm.passType = node.GetStringValue("passType");
            itm.passPer = node.GetStringValue("passPer");
            itm.radHeight = node.GetStringValue("radHeight");
            itm.P1_RelativeDistacnePers = node.GetStringValue("P1_RelativeDistacnePers");
            itm.P1_OffsetAngles = node.GetStringValue("P1_OffsetAngles");

            m_shootData.Add(itm);
        }

        xmlNodeList = docNode.GetNodes("Object/SkillShoot");
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            itm = new TD_PassOrShootItem();
            itm.id = node.GetIntValue("ID");
            itm.speed = node.GetFloatValue("speed");
            itm.passPer = node.GetStringValue("passPer");
            itm.passType = node.GetStringValue("passType");
            itm.radHeight = node.GetStringValue("radHeight");
            itm.P1_RelativeDistacnePers = node.GetStringValue("P1_RelativeDistacnePers");
            itm.P1_OffsetAngles = node.GetStringValue("P1_OffsetAngles");

            m_skillShootData.Add(itm);
        }

        TD_PushShootData pushItm;
        xmlNodeList = docNode.GetNodes("Object/PushShoot");
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            pushItm = new TD_PushShootData();
            pushItm.distance = node.GetFloatValue("distance");
            pushItm.force = node.GetFloatValue("force");
            pushItm.drag = node.GetFloatValue("drag");
            pushItm.addFore = node.GetStringValue("addFore");

            m_pushShootData.Add(pushItm);
        }

        TD_PassDisTimer tItm;
        xmlNodeList = docNode.GetNodes("Object/Dis");
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            tItm = new TD_PassDisTimer();
            tItm.dis = node.GetFloatValue("dis");
            tItm.timer = node.GetFloatValue("timer");
            tItm.oftimer = node.GetFloatValue("oftimer");

            m_passDisTimer.Add(tItm);
        }

        asset = null;
    }

    public void save()
    {
        string path = Application.dataPath + "/Config/PlayerDefine.xml";

        XmlDocument newDoc = new XmlDocument();
        newDoc.Load(path);
        string sContent = "";
        int cnt = 0;

        Regex _rgx = new Regex("\\<!--(.*?)--\\>", RegexOptions.Singleline);
        var mc = _rgx.Matches(newDoc.InnerXml);
        foreach (Match itemReg in mc)
        {
            sContent += itemReg.Groups[1].Value;//这里是注视
        }

        newDoc.RemoveAll();

        XmlElement ment;
        //List<TD_PassOrShootItem> listItem = new List<TD_PassOrShootItem>();
        TD_PassOrShootItem item;

        XmlDeclaration xmldecl = newDoc.CreateXmlDeclaration("1.0", "", null);
        newDoc.AppendChild(xmldecl);
        XmlElement root = newDoc.CreateElement("Object");

        newDoc.AppendChild(newDoc.CreateComment(sContent));
        newDoc.AppendChild(root);

        ment = newDoc.CreateElement("Define");
        ment.SetAttribute("Gravity", Gravity.ToString());
        ment.SetAttribute("PassTimeScale", PassTimeScale.ToString());
        ment.SetAttribute("ShootTimeScele", ShootTimeScele.ToString());
        ment.SetAttribute("AngleTurnTimer", AngleTurnTimer.ToString());
        ment.SetAttribute("GuoRenRange", GuoRenRange.ToString());
        root.AppendChild(ment);

        //Physics.gravity = new Vector3(Physics.gravity.x, Gravity, Physics.gravity.y);

        ment = newDoc.CreateElement("Ball");
        ment.SetAttribute("DynamicFriction", BallPhysis.DynamicFriction.ToString());
        ment.SetAttribute("StaticFriction", BallPhysis.StaticFriction.ToString());
        ment.SetAttribute("Bounciness", BallPhysis.Bounciness.ToString());
        root.AppendChild(ment);

        ment = newDoc.CreateElement("Ground");
        ment.SetAttribute("DynamicFriction", GroundPhysis.DynamicFriction.ToString());
        ment.SetAttribute("StaticFriction", GroundPhysis.StaticFriction.ToString());
        ment.SetAttribute("Bounciness", GroundPhysis.Bounciness.ToString());
        root.AppendChild(ment);

        ment = newDoc.CreateElement("Camera");
        string str = AddPostion.x + "," + AddPostion.y + "," + AddPostion.z;
        ment.SetAttribute("AddPostion", str);
        str = AddRotation.x + "," + AddRotation.y + "," + AddRotation.z;
        ment.SetAttribute("AddRotation", str);
        root.AppendChild(ment);
        

        cnt = m_passData.Count;
        for (int j = 0; j < cnt; j++)
        {
            item = m_passData[j];
            ment = newDoc.CreateElement("Pass");
            ment.SetAttribute("distance", item.distance.ToString());
            ment.SetAttribute("force", item.force.ToString());
            ment.SetAttribute("drag", item.drag.ToString());
            ment.SetAttribute("airDrag", item.airDrag.ToString());
            ment.SetAttribute("addFore", item.addFore);
            ment.SetAttribute("passType", item.passType);

            root.AppendChild(ment);
        }

        cnt = m_shootData.Count;
        for (int j = 0; j < cnt; j++)
        {
            item = m_shootData[j];
            ment = newDoc.CreateElement("Shoot");
            ment.SetAttribute("ID", item.id.ToString());
            ment.SetAttribute("point", item.point);
            ment.SetAttribute("speed", item.speed.ToString());
            ment.SetAttribute("passType", item.passType);
            ment.SetAttribute("passPer", item.passPer);
            ment.SetAttribute("radHeight", item.radHeight);
            ment.SetAttribute("P1_RelativeDistacnePers", item.P1_RelativeDistacnePers);
            ment.SetAttribute("P1_OffsetAngles", item.P1_OffsetAngles);

            root.AppendChild(ment);
        }

        cnt = m_skillShootData.Count;
        for (int j = 0; j < cnt; j++)
        {
            item = m_skillShootData[j];
            ment = newDoc.CreateElement("SkillShoot");
            ment.SetAttribute("ID", item.id.ToString());
            ment.SetAttribute("speed", item.speed.ToString());
            ment.SetAttribute("passType", item.passType.ToString());
            ment.SetAttribute("passPer", item.passPer);
            ment.SetAttribute("radHeight", item.radHeight);
            ment.SetAttribute("P1_RelativeDistacnePers", item.P1_RelativeDistacnePers);
            ment.SetAttribute("P1_OffsetAngles", item.P1_OffsetAngles);

            root.AppendChild(ment);
        }

        cnt = m_pushShootData.Count;
        TD_PushShootData pushItm;
        for (int j = 0; j < cnt; j++)
        {
            pushItm = m_pushShootData[j];
            ment = newDoc.CreateElement("PushShoot");
            ment.SetAttribute("distance", pushItm.distance.ToString());
            ment.SetAttribute("force", pushItm.force.ToString());
            ment.SetAttribute("drag", pushItm.drag.ToString());
            ment.SetAttribute("addFore", pushItm.addFore.ToString());

            root.AppendChild(ment);
        }


        TD_PassDisTimer ptm;
        cnt = m_passDisTimer.Count;
        for (int j = 0; j < cnt; j++)
        {
            ptm = m_passDisTimer[j];
            ment = newDoc.CreateElement("Dis");
            ment.SetAttribute("dis", ptm.dis.ToString());
            ment.SetAttribute("timer", ptm.timer.ToString());
            ment.SetAttribute("oftimer", ptm.oftimer.ToString());

            root.AppendChild(ment);
        }

        newDoc.Save(path);
    }



    /// <summary>
    /// 加载球员位置配置
    /// </summary>
    public  void LoadPlayerAtkPositionConfig(AssetBundles.NormalRes data)
    {
        if (null != m_confidata) return;
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
        if (asset == null)
            return;

        m_confidata = new Dictionary<int, TD_PlayerAtkPosition>();
        TD_PlayerAtkPosition item;

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
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;

            item = new TD_PlayerAtkPosition();
            item.pos = node.GetIntValue("pos");
            item.first = node.GetStringValue("first").Split(',');
            item.scecond = node.GetStringValue("scecond").Split(',');
            item.three = node.GetStringValue("three").Split(',');

            m_confidata[item.pos] = item;
        }

        asset = null;
    }
    /// <summary>
    /// 加载球员配置
    /// </summary>
    public  void LoadPlayerConfig(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
        if (asset == null)
            return;

        TD_Player item;

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
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;

            item = new TD_Player();
            item.id = node.GetIntValue("ID");
            item.CardID = node.GetIntValue("CardID");
            item.initialstar = node.GetIntValue("initialstar");
            item.maxstar = node.GetIntValue("maxstar");
            item.name = node.GetStringValue("name");
            item.sex = node.GetByteValue("sex");
            item.height = node.GetFloatValue("height");
            item.type = (TypePlayer)node.GetIntValue("type");
            item.adaptPos = node.GetStringValue("adaptPos");
            item.levelItems = new List<PlayerItem>();

            item.attr = new PlayerItem();
            item.attr.level = node.GetIntValue("level");
            item.attr.shoot = node.GetIntValue("shoot");
            item.attr.pass = node.GetIntValue("pass");
            item.attr.reel = node.GetIntValue("reel");

            item.attr.control = node.GetIntValue("control");
            item.attr.def = node.GetIntValue("def");
            item.attr.trick = node.GetIntValue("trick");
            item.attr.steal = node.GetIntValue("steal");
            item.attr.keep = node.GetIntValue("keep");
            item.jobIndex = node.GetStringValue("jobIndex");
            item.Job = node.GetStringValue("JOB");
            item.skill = node.GetStringValue("skill");
            item.selfOwnSkill = node.GetStringValue("skill1");
            item.skill1 = item.skill.Split(',')[0];
           //item.passiveSkill = node.GetStringValue("passiveSkill");
           playerList[item.id] = item;
        }
        asset = null;
    }
    /// <summary>
    /// 获取项
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static PlayerItem GetPlayerSlevelInfo(int ID)
    {
        PlayerItem itm;
        if (slevelList.TryGetValue(ID, out itm))
        {
            return itm;
        }
        return itm;
    }

    /// <summary>
    /// 获取项
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static InheritItem GetInheritItem(string id)
    {
        for (int i = 0; i < inheritItemList.Count; ++i)
        {
            if (inheritItemList[i].itemId == id)
            {
                return inheritItemList[i];
            }
        }
        return null;
    }

    public TD_PassOrShootItem GetShootSpeed(int point)
    {
        //point = 34;
        TD_PassOrShootItem item = new TD_PassOrShootItem();
        int cnt = m_shootData.Count;
        for (int j = 0; j < cnt; j++)
        {
            TD_PassOrShootItem val = m_shootData[j];
            string[] ps = val.point.Split(',');
            int pt;
            cnt = ps.Length;
            for (int i = 0; i < cnt; i++)
            {
                pt = int.Parse(ps[i]);
                if (pt == point)
                    return val;
            }
        }

        return item;
    }

    public TD_PassOrShootItem GetShootSpeedByCfgId(int cfgId)
    {
        int cnt = m_shootData.Count;
        for (int j = 0; j < cnt; j++)
        {
            if (m_shootData[j].id == cfgId)
                return m_shootData[j];
        }

        return null;
    }

    public TD_PassOrShootItem GetSkillShootSpeed(int cfgID)
    {
        int cnt = m_skillShootData.Count;
        for (int j = 0; j < cnt; j++)
        {
            if (m_skillShootData[j].id == cfgID)
                return m_skillShootData[j];
        }

        return null;
    }

    public TD_PassDisTimer GetPassDisInfo(float disVal)
    {
        int cnt = m_passDisTimer.Count;
        TD_PassDisTimer nearItem = null;
        for (int j = 0; j < cnt; j++)
        {
            TD_PassDisTimer val = m_passDisTimer[j];

            if (val.dis >= disVal && null == nearItem)
            {
                nearItem = val;
                break;
            }
        }

        if (null != nearItem)
            return nearItem;

        return null;
    }

    /// <summary>获取最近时间的项</summary>
    /// <param name="timer"></param>
    /// <returns></returns>
    public TD_PassDisTimer GetNearTimerPassDisInfo(float timer)
    {
        int cnt = m_passDisTimer.Count;
        TD_PassDisTimer nearItem = null;
        for (int j = 0; j < cnt; j++)
        {
            TD_PassDisTimer val = m_passDisTimer[j];

            if (null == nearItem)
                nearItem = val;
            else
            {
                float nval = Mathf.Abs(nearItem.timer - timer);
                float cval = Mathf.Abs(val.timer - timer);

                if(nval > cval)
                    nearItem = val;
            }
        }

        return nearItem;
    }

    public TD_PushShootData GetPushShootData(float disVal)
    {
        int cnt = m_pushShootData.Count;
        TD_PushShootData nearItem = null;
        for (int j = 0; j < cnt; j++)
        {
            TD_PushShootData val = m_pushShootData[j];

            if (val.distance >= disVal && null == nearItem)
            {
                nearItem = val;
                break;
            }
        }

        if (null != nearItem)
            return nearItem;

        return null;
    }

    /// <summary>获取传球项</summary>
    /// <param name="disVal"></param>
    /// <param name="offsetIndex">偏移几个索引 没有了就是结果值</param>
    /// <returns></returns>
    public TD_PassOrShootItem GetPassItem(float disVal,int offsetIndex = 0)
    {
        int cnt = m_passData.Count;
        for (int j = 0; j < cnt; j++)
        {
            TD_PassOrShootItem val = m_passData[j];

            if (val.distance >= disVal)
            {
                if(offsetIndex != 0)
                {
                    int ofIndex = j + offsetIndex;
                    if (offsetIndex > 0 && ofIndex <= cnt)
                    {
                        return m_passData[ofIndex];
                    }
                    else if (offsetIndex < 0 && ofIndex >= 0)
                    {
                        return m_passData[ofIndex];
                    }
                }

                return val;
            }
        }

        return new TD_PassOrShootItem();
    }

    /// <summary>
    /// 获取项
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static PlayerItem GetPlayerItem(int level)
    {
        PlayerItem itm;

        if (playerLevelList.TryGetValue(level, out itm))
        {
            return itm;
        }

        return itm;
    }
    /// <summary>
    /// 获取项
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TD_Player GetItem(int id)
    {
        TD_Player itm;

        if (playerList.TryGetValue(id, out itm))
        {
            return itm;
        }

        return itm;
    }

    public Dictionary<int, TD_Player> Data
    {
        get
        {
            return playerList;
        }
    }
    /// <summary>
    /// 获取当前突破等级的属性信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="Level"></param>
    /// <returns></returns>
    public static StrikeInfo GetStrikeInfoByID(int id)
    {
        if (strikeInfoList.ContainsKey(id))
        {
            return strikeInfoList[id];
        }
        return null;
    }
    public PlayerItem GetPlayerItemByLevel(TD_Player item, int lv)
    {
        if (null == item.levelItems) return null;

        PlayerItem childItem = null;
        for (int i = 0; i < item.levelItems.Count; i++)
        {
            if (item.levelItems[i].level == lv)
            {
                childItem = item.levelItems[i];
                return childItem;
            }
        }

        return childItem;
    }



    /// <summary>
    /// 获取项
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TD_PlayerAtkPosition GetPlayerAtkPositionItem(int id)
    {
        TD_PlayerAtkPosition itm;

        if (m_confidata.TryGetValue(id, out itm))
        {
            return itm;
        }

        return itm;
    }
}

public class StrikeInfo : PlayerItem
{
    public string add;        // 额外加成
    public int addSkill;
    public int needCount;     // 需要碎片数量
}

public class InheritItem
{
    public string itemId;
    public float expRate;
    public float skillExpRate;
    public float powerLevelRate;
    public float mentalityLevelRate;
    public float strikeBackItemRate;
    public int money;
}

public class PlayerItem
{
    public int id;
    public int star;        // id
    public int level;       // 等级
    public int shoot;       // 射门
    public int pass;        // 传球
    public int reel;        // 带盘
    public float tech;        // 技术
    public int control;     // 控球
    public int def;         // 防守
    public int trick;       // 拦截
    public int steal;       // 抢断
    public float health;      // 身体
    public int keep;        // 守门
    public int needExp;       // 所需经验
    public string material;   // 消耗材料
}

public class TD_Player
{
    public int id;          //编号
    public int CardID;      //碎片类型
    public int initialstar; //初始星级
    public int maxstar;     //最高星级
    public string name;     //名字
    public float height;    //身高
    public byte sex;
    public TypePlayer type; //类型
    public string adaptPos; //合法位置
    public PlayerItem attr;
    public string Job; //合法位置
    public string jobIndex; //合法位置
    public string skill; //主动技能
    public string skill1; //技能
    public string skill2; //技能
    public string skill3; //技能
    public string skill4; //技能
    public string selfOwnSkill; //主角可选技能
    //public string passiveSkill; //被动技能
    public List<PlayerItem> levelItems; //玩家项

    public PlayerItem GetLevelItem(int lv)
    {
        return attr;
        //return InstanceProxy.Get<PlayerLevelConfig>().GetItem(lv);
    }

    /// <summary>获取当前Step的技能</summary>
    public List<TD_SkillAI> GetCanUseSkill(MatchPlayerItem item,string preStr="")
    {
        string skillStr = preStr + item.player.skill;

        SkillAIConfig skillCfg = ProxyInstance.InstanceProxy.Get<SkillAIConfig>();
        List<TD_SkillAI> skillList = new List<TD_SkillAI>();
        string[] lst = skillStr.Split(',');
        int cnt = lst.Length;
        TD_SkillAI skillItm;

        for (int i = 0; i < cnt; i++)
        {
            skillItm = skillCfg.GetItem(lst[i]);

            if (skillItm.CheckCanUse(MatchManager.m_doStep))
            {
                skillList.Add(skillItm);
            }
        }

        return skillList;
    }
}

public struct TD_PlayerAtkPosition
{
    public int pos;              //编号
    public string[] first;      //第一回
    public string[] scecond;    //第二回
    public string[] three;      //第三回
}

[SerializeField]
public class TD_PassDisTimer
{
    public float dis;
    public float timer;
    public float oftimer;
}

[SerializeField]
public class TD_PushShootData
{
    public float distance;
    public float force;
    public float drag;
    public string addFore;
}

[SerializeField]
public class TD_PassOrShootItem
{
    public int id;
    public string point;
    public float distance;
    public float speed;
    public float force;
    public float drag;
    public float airDrag;
    public string radHeight;
    public string passType;
    public string passPer;
    public string addFore;
    public string P1_RelativeDistacnePers;
    public string P1_OffsetAngles;

    [HideInInspector]
    public float typeRadHeight;
    [HideInInspector]
    public float relativeDistacnePer;
    [HideInInspector]
    public int P1_OffsetAngle;

    public E_PassOrShootType GetOperType()
    {
        string[] type = null;
        if(!string.IsNullOrEmpty(passType))
            type = passType.Split(',');

        if (string.IsNullOrEmpty(passPer))
            return E_PassOrShootType.Ground;

        string[] per = passPer.Split(',');
        string[] heights = radHeight.Split(',');
        string[] relDis = P1_RelativeDistacnePers.Split(',');
        string[] angles = P1_OffsetAngles.Split(',');

        float rndNum = UnityEngine.Random.Range(0f, 1f);
        int cnt = per.Length;
        for (int i = 0; i < cnt; i++)
        {
            float perVal = float.Parse(per[i]);
            int typeVal = 0;

            if(null != type)
                typeVal = int.Parse(type[i]);

            if (perVal >= rndNum)
            {
                typeRadHeight = float.Parse(heights[i]);
                relativeDistacnePer = float.Parse(relDis[i]);
                P1_OffsetAngle = int.Parse(angles[i]);
                return (E_PassOrShootType)typeVal;
            }
        }

        return (E_PassOrShootType)int.Parse(type[0]);
    }
}
