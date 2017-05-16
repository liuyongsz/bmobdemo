using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//公会权限
public class GuildPower
{
    /// <summary>
    /// id
    ///-------------------------------------------------------
    /// </summary>
    public int id;
    public string name;

}
public class GuildPowerConfig : ConfigLoaderBase
{


    private static Dictionary<int, GuildPower> m_data = new Dictionary<int, GuildPower>();

    //同阵营公会列表
    public static List<GuildInfo> mGuildList = new List<GuildInfo>();
    //已经申请过的公会ID
    public static List<int> mApplyGuildIdList = new List<int>();

    protected override void OnLoad()
    {
        if (!ReadConfig<GuildPower>("GuildPower.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GuildPower row)
    {
        m_data.Add(row.id, row) ;
        
    }
    public static GuildPower GetGuildPower(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

}
