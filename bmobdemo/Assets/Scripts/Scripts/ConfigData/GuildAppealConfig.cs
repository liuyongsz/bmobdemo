using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//公会职位
public class GuildAppeal
{
    /// <summary>
    /// id(攻击类型ID)
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// 消耗道具
    /// </summary>
    public string consume;

    /// <summary>
    /// 减少公会资金
    /// </summary>
    public int reduceFund;

    /// <summary>
    /// 减少公会声望
    /// </summary>
    public int reducereputation;

    /// <summary>
    /// 获得个人贡献
    /// </summary>
    public int Addcontribution;

    /// <summary>
    /// 成功率
    /// </summary>
    public float successrate;

    /// <summary>
    /// 是否免疫
    /// </summary>
    public int isimmune;
}
public class GuildAppealConfig : ConfigLoaderBase
{


    private static Dictionary<int, GuildAppeal> m_data = new Dictionary<int, GuildAppeal>();


    protected override void OnLoad()
    {
        if (!ReadConfig<GuildAppeal>("GuildAppeal.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GuildAppeal row)
    {
        m_data.Add(row.id, row) ;
    }
    public static GuildAppeal GetGuildAppeal(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

}
