using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//公会权限
public class GuildUpdate
{
    /// <summary>
    /// id(等级)
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// 需要声望
    ///-------------------------------------------------------
    /// </summary>
    public int needReputation;

}
public class GuildUpdateConfig : ConfigLoaderBase
{


    private static Dictionary<int, GuildUpdate> m_data = new Dictionary<int, GuildUpdate>();

    //同阵营公会列表
    public static List<GuildInfo> mGuildList = new List<GuildInfo>();
    //已经申请过的公会ID
    public static List<int> mApplyGuildIdList = new List<int>();

    protected override void OnLoad()
    {
        if (!ReadConfig<GuildUpdate>("GuildUpdate.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GuildUpdate row)
    {
        m_data.Add(row.id, row) ;
        
    }
    public static GuildUpdate GetGuildUpdate(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

}
