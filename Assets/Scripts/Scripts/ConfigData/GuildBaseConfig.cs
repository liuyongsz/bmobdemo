using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GuildBase
{
    /// <summary>
    /// id
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// 公会名字最少长度
    ///-------------------------------------------------------
    /// </summary>
    public int nameLenMin;

    /// <summary>
    /// 公会名字最大长度
    ///-------------------------------------------------------
    /// </summary>
    public int nameLenMax;

    /// <summary>
    /// 创建公会需要的钻石
    ///-------------------------------------------------------
    /// </summary>
    public int createNeedDiamond;

    /// <summary>
    /// 公会简介长度
    ///-------------------------------------------------------
    /// </summary>
    public int introductionLen;

    /// <summary>
    /// 公会公告长度
    ///-------------------------------------------------------
    /// </summary>
    public int noticeLen;

    /// <summary>
    /// 公会最大人数
    ///-------------------------------------------------------
    /// </summary>
    public int maxMemberNum;

    /// <summary>
    /// 修改公会名字需要的钻石
    ///-------------------------------------------------------
    /// </summary>
    public int changeNameDiamond;

    /// <summary>
    /// 弹劾时间（天）
    ///-------------------------------------------------------
    /// </summary>
    public int impeachTime;

    /// <summary>
    /// 公会开启等级
    ///-------------------------------------------------------
    /// </summary>
    public int openLevel;

    /// <summary>
    /// 单份捐献欧元
    ///-------------------------------------------------------
    /// </summary>
    public int euroPer;

    /// <summary>
    /// 单份捐献获得贡献
    ///-------------------------------------------------------
    /// </summary>
    public int contributionPer;

    /// <summary>
    /// 最大等级
    ///-------------------------------------------------------
    /// </summary>
    public int maxLevel;

    /// <summary>
    /// 加速1小时需公会资金
    ///-------------------------------------------------------
    /// </summary>
    public int speedTimeFunds;

    /// <summary>
    /// 公会保护每小时消耗
    /// </summary>
    public int protectconsume;

}
public class GuildBaseConfig : ConfigLoaderBase
{


    private static Dictionary<int, GuildBase> m_data = new Dictionary<int, GuildBase>();

    //同阵营公会列表
    public static List<GuildInfo> mGuildList = new List<GuildInfo>();
    //已经申请过的公会ID
    public static List<int> mApplyGuildIdList = new List<int>();

    protected override void OnLoad()
    {
        if (!ReadConfig<GuildBase>("GuildBase.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GuildBase row)
    {
        m_data.Add(row.id, row) ;
        
    }
    public static GuildBase GetGuildBase(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

}
