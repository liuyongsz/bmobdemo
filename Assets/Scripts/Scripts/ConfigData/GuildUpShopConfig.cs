using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//公会商城升级
public class GuildUpShop
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
    public int openNum;

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

}
public class GuildUpShopConfig : ConfigLoaderBase
{


    private static Dictionary<int, GuildUpShop> m_data = new Dictionary<int, GuildUpShop>();

    
    protected override void OnLoad()
    {
        if (!ReadConfig<GuildUpShop>("GuildUpShop.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GuildUpShop row)
    {
        m_data.Add(row.id, row) ;
        
    }
    public static GuildUpShop GetGuildUpShop(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

}
