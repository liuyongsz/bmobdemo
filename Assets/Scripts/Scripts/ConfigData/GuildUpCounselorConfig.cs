using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//公会商城升级
public class GuildUpCounselor
{
    /// <summary>
    /// id
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// 拉拢成功率
    ///-------------------------------------------------------
    /// </summary>
    public int preSucc;

    /// <summary>
    /// 升级时间
    ///-------------------------------------------------------
    /// </summary>
    public int needTime;

    /// <summary>
    /// 需要资金
    ///-------------------------------------------------------
    /// </summary>
    public int needFunds;

    /// <summary>
    /// 保护时间
    ///-------------------------------------------------------
    /// </summary>
    public int protectTime;

    /// <summary>
    /// 顾问数量
    ///-------------------------------------------------------
    /// </summary>
    public int counselorNum;

}
public class GuildUpCounselorConfig : ConfigLoaderBase
{


    private static Dictionary<int, GuildUpCounselor> m_data = new Dictionary<int, GuildUpCounselor>();

    
    protected override void OnLoad()
    {
        if (!ReadConfig<GuildUpCounselor>("GuildUpCounselor.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GuildUpCounselor row)
    {
        m_data.Add(row.id, row) ;
        
    }
    public static GuildUpCounselor GetGuildUpCounselor(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

}
