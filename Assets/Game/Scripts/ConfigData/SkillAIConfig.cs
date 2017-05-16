using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;

public struct TD_SkillAI
{
    public int ID;
    public Skill_ShowFlag showFlag;
    public Skill_RangeType rangType;
    public string name;
    public int shootSkillID;
    public string trail;
    public string passTrail;
    public string eff;
    public string effIcon;
    public string skillIcon;
    public string useStep;
    public Skill_ActionType actionType;
    public string effFun;
    public int keepRound;

    /// <summary>检测是否使用这个位置</summary>
    /// <param name="step"></param>
    /// <returns></returns>
    public bool CheckCanUse(int step)
    {
        if (useStep.IsNullOrEmpty())
            return false;

        string[] lst = useStep.Split(',');
        int cnt = lst.Length;
        for (int i = 0; i < cnt;i++)
        {
            if(int.Parse(lst[i]) == step)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>0,1传球和射门</summary>
    public bool IsNormalSkill
    {
        get
        {
            return ID == 0 || ID == 1;
        }
    }

    public TD_SkillAI Clone()
    {
        TD_SkillAI cItm = new TD_SkillAI();
        cItm.ID = ID;
        cItm.rangType = rangType;
        cItm.name = name;
        cItm.eff = eff;
        cItm.effIcon = effIcon;
        cItm.useStep = useStep;
        cItm.actionType = actionType;
        cItm.effFun = effFun;

        return cItm;
    }
}

public class SkillAIConfig: ConfigBase
{
    private Dictionary<int, TD_SkillAI> m_data;

    private UnityAction m_callBack;
    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        m_callBack = loadedFun;
        LoadData("SkillAI", loadedFun);
    }

    public override void onloaded(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;
 
        if (asset == null)
            return;

        m_data = new Dictionary<int, TD_SkillAI>();

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

        TD_SkillAI item;
        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;

            item = new TD_SkillAI();
            item.ID = node.GetIntValue("ID");
            item.showFlag = (Skill_ShowFlag)node.GetIntValue("showFlag");
            item.rangType = (Skill_RangeType)node.GetIntValue("rangType");
            item.name = node.GetStringValue("name");
            item.shootSkillID = node.GetIntValue("shootSkillID");
            item.trail = node.GetStringValue("trail");
            item.passTrail = node.GetStringValue("passTrail");
            item.eff = node.GetStringValue("eff");
            item.effIcon = node.GetStringValue("effIcon");
            item.useStep = node.GetStringValue("useStep");
            item.actionType = (Skill_ActionType)node.GetIntValue("actionType");
            item.keepRound = node.GetIntValue("keepRound");
            item.effFun = node.GetStringValue("effFun");

            m_data[item.ID] = item;
        }

        asset = null;

        if (null != m_callBack)
        {
            m_callBack();
            m_callBack = null;
        }  

        base.onloaded(data);
    }

    /// <summary>
    /// 获取项
    /// </summary>
    /// <returns></returns>
    public TD_SkillAI GetItem(string skillId)
    {
        int sid = int.Parse(skillId);
        return GetItem(sid);
    }

    /// <summary>
    /// 获取项
    /// </summary>
    /// <returns></returns>
    public TD_SkillAI GetItem(int skillId)
    {
        TD_SkillAI itm;

        if (m_data.TryGetValue(skillId, out itm))
        {
            return itm;
        }

        return itm;
    }

    public Dictionary<int, TD_SkillAI> Data
    {
       get
        {
            return m_data;
        }
    }
}

