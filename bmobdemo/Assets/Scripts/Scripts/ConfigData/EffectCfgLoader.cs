using UnityEngine;
using System.Collections.Generic;
using System;

public class EffectCfgEX
{
    // 配置表数据
    public int ID;
    public string OnBeginPlay;
    public string OnEndPlay;
    public string AssetName;
    public string BindPoint;
    public int Delay;
    public int Lifetime;
    public bool FollowPosition;
    public bool FollowRotation;
    public bool FollowScale;
    public string LocalPosition;
    public string LocalRotation;
    public string LocalScale;
    public int FadeOutTime;
    public int Audio;

    // 运行时数据
    public int[] OnBeginPlayArray;
    public int[] OnEndPlayArray;
    public Vector3 LocalPositionVec3;
    public Vector3 LocalRotationVec3;
    public Quaternion LocalRotationQuaternion;
    public Vector3 LocalScaleVec3;
}


public class EffectCfgLoader : ConfigLoaderBase
{
    private Dictionary<int, EffectCfgEX> m_data = new Dictionary<int, EffectCfgEX>();

    protected override void OnLoad()
    {
        if (!ReadConfig<EffectCfgEX>("EffectCfg.xml", OnReadRow))
            return;
    }

    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(EffectCfgEX row)
    {
        InitRuntimeData(row);
        m_data[row.ID] = row;
    }

    private void InitRuntimeData(EffectCfgEX row)
    {
        row.OnBeginPlayArray = ConfigParseUtil.ParseIntArray(row.OnBeginPlay);
        row.OnEndPlayArray = ConfigParseUtil.ParseIntArray(row.OnEndPlay);
        row.LocalPositionVec3 = ConfigParseUtil.ParseVec3(row.LocalPosition);
        row.LocalRotationVec3 = ConfigParseUtil.ParseVec3(row.LocalRotation);
        row.LocalRotationQuaternion = Quaternion.Euler(row.LocalRotationVec3);

        row.LocalScaleVec3 = ConfigParseUtil.ParseVec3(row.LocalScale);
        if (row.LocalScaleVec3 == Vector3.zero)
            row.LocalScaleVec3 = Vector3.one;
    }

    public EffectCfgEX GetConfig(int id)
    {
        EffectCfgEX config;
        m_data.TryGetValue(id, out config);
        return config;
    }
}
