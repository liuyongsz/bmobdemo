using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//公会商城升级
public class GuildUpTask
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
    /// 解锁任务类型
    ///-------------------------------------------------------
    /// </summary>
    public string openType;

}
public class GuildUpTaskConfig : ConfigLoaderBase
{


    private static Dictionary<int, GuildUpTask> m_data = new Dictionary<int, GuildUpTask>();

    
    protected override void OnLoad()
    {
        if (!ReadConfig<GuildUpTask>("GuildUpTask.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GuildUpTask row)
    {
        m_data.Add(row.id, row) ;
        
    }
    public static GuildUpTask GetGuildUpTask(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

}
