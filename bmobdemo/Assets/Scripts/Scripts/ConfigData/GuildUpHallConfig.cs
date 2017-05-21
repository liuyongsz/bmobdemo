using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//公会权限
public class GuildUpHall
{
    /// <summary>
    /// id
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// 增加数量
    ///-------------------------------------------------------
    /// </summary>
    public int addNum;

    /// <summary>
    /// 声誉
    ///-------------------------------------------------------
    /// </summary>
    public int reputation;

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
    /// 检查次数
    ///-------------------------------------------------------
    /// </summary>
    public int inspect;

}
public class GuildUpHallConfig : ConfigLoaderBase
{


    private static Dictionary<int, GuildUpHall> m_data = new Dictionary<int, GuildUpHall>();

    
    protected override void OnLoad()
    {
        if (!ReadConfig<GuildUpHall>("GuildUpHall.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GuildUpHall row)
    {
        m_data.Add(row.id, row) ;
        
    }
    public static GuildUpHall GetGuildUpHall(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

}
