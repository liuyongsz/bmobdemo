using System;
using System.Collections.Generic;

public class ServerInfo
{
    public int ID;
    public string ip;
    public string Name;
}
public class ServerConfig : ConfigLoaderBase
{
    public static Dictionary<int, ServerInfo> serverList = new Dictionary<int, ServerInfo>();
    protected override void OnLoad()
    {
        if (!ReadConfig<ServerInfo>("Server.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        serverList.Clear();
    }

    private void OnReadRow(ServerInfo row)
    {
        serverList[row.ID] = row;
    }
  
    public static string GetServerNameByIP(string ip)
    {
        foreach (ServerInfo item in serverList.Values)
        {
            if (item.ip == ip)
            {
                return item.Name;
            }
        }
        return null;
    }
}
