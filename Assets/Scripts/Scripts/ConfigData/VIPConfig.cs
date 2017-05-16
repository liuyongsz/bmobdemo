using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class VipAllowInfo
{
    public string iD;
    public string allow;
}

public class VipUpInfo
{
    public int ID;
    public int upgradeExp;
    public int dropIndex;
    public string vipBag;
}
public class VIPUPConfig : ConfigLoaderBase
{
    public static Dictionary<int, VipUpInfo> vipUpInfoList = new Dictionary<int, VipUpInfo>();

    protected override void OnLoad()
    {
        if (!ReadConfig<VipUpInfo>("VipUpExp.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        vipUpInfoList.Clear();
    }

    private void OnReadRow(VipUpInfo row)
    {
        vipUpInfoList[row.ID] = row;
    }


    public static VipUpInfo GetVipUpInfo(int level)
    {
        if (vipUpInfoList.ContainsKey(level + 1))
        {
            return vipUpInfoList[level + 1];
        }
        return null;
    }

}
public class VIPAllowConfig : ConfigLoaderBase
{
    public static Dictionary<string, VipAllowInfo> vipAllowList = new Dictionary<string, VipAllowInfo>();

    protected override void OnLoad()
    {
        if (!ReadConfig<VipAllowInfo>("VipAllow.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        vipAllowList.Clear();
    }

    private void OnReadRow(VipAllowInfo row)
    {
        vipAllowList[row.iD] = row;
    }   

    public static VipAllowInfo GetVipAllow(string id)
    {
        if (vipAllowList.ContainsKey(id))
        {
            return vipAllowList[id];
        }
        return null;
    }

}
