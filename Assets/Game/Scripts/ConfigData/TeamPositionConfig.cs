using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TeamPosition
{
    public int id;
    //位置
    public string name;
  
}

//阵型球员位置
public class TeamPositionConfig : ConfigLoaderBase
{
    
    private static Dictionary<int, TeamPosition> m_data = new Dictionary<int, TeamPosition>();

    protected override void OnLoad()
    {
        if (!ReadConfig<TeamPosition>("TeamPosition.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(TeamPosition row)
    {
        m_data[row.id] = row;
    }

    public static TeamPosition GetTeamPosition(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;

    }
}
