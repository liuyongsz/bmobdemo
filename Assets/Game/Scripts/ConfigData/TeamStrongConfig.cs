using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TeamStrong
{
 
    public int level;


    public string prop;
    
}


public class TeamStrongConfig : ConfigLoaderBase
{
    
    private static Dictionary<int, TeamStrong> m_data = new Dictionary<int, TeamStrong>();

    protected override void OnLoad()
    {
        if (!ReadConfig<TeamStrong>("TeamStrong.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(TeamStrong row)
    {
        m_data[row.level] = row;
    }

    public static TeamStrong GetTeamStrong(int level)
    {
        if (m_data.ContainsKey(level))
        {
            return m_data[level];
        }
        return null;

    }
}
