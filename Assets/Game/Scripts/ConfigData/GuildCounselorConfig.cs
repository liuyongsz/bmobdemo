using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GuildCounselor
{
    /// <summary>
    /// 顾问id
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// 顾问名字
    ///-------------------------------------------------------
    /// </summary>
    public string counselorname;

    /// <summary>
    /// 顾问星级
    ///-------------------------------------------------------
    /// </summary>
    public int starlevel;

    /// <summary>
    /// 加成效果
    ///-------------------------------------------------------
    /// </summary>
    public float buff;

    /// <summary>
    /// 初始所属
    ///-------------------------------------------------------
    /// </summary>
    public int initialType;

    /// <summary>
    /// 初始友好度
    ///-------------------------------------------------------
    /// </summary>
    public int initialamity;

    /// <summary>
    /// 加成类型
    ///-------------------------------------------------------
    /// </summary>
    public string buffType;

}


public class GuildCounselorConfig : ConfigLoaderBase
{
    public static Dictionary<int, GuildCounselor> m_data = new Dictionary<int, GuildCounselor>();


    //服务器的顾问数据
    public static Dictionary<int, AdviserInfo> mAdviserDict = new Dictionary<int, AdviserInfo>();

    //顾问的公会好友度排名
    public static List<AdviserGuildInfo> mGuildFriendsList = new List<AdviserGuildInfo>();

    protected override void OnLoad()
    {
        if (!ReadConfig<GuildCounselor>("GuildCounselor.xml", OnReadRow))
            return;
    }

    protected override void OnUnload()
    {
        m_data.Clear();

    }
    private void OnReadRow(GuildCounselor row)
    {
        m_data.Add(row.id, row);

    }
    public static GuildCounselor GetGuildCounselor(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

    //获取顾问
    public static AdviserInfo GetAdviserInfo(int id)
    {
        if (mAdviserDict.ContainsKey(id))
        {
            return mAdviserDict[id];
        }
        return null;
    }
    //获取已拉拢顾问数量
    public static int GetRopeAdviserNum(int  guildID)
    {
        int num=0;
        var enumor = mAdviserDict.Values.GetEnumerator();
        while(enumor.MoveNext())
        {
            if (enumor.Current.guildDBID == guildID)
                num++;
        }
        return num;
            
    }
  
}
