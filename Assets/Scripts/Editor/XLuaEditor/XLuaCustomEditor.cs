using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class XLuaCustomEditor
{
    [MenuItem("XLua/HotFix/Enable", false)]
    public static void HotFixEnable()
    {
        SetEnabled("HOTFIX_ENABLE", true, true);
    }
    
    [MenuItem("XLua/HotFix/Disable", false)]
    public static void HotFixDisable()
    {
        SetEnabled("HOTFIX_ENABLE", false, true);
    }
    
    [MenuItem("XLua/HotFix/Enable", true)]
    private static bool EnableValidate()
    {
        var defines = GetDefinesList(hotFixBuildTargetGroups[0]);
        if (defines.Contains("HOTFIX_ENABLE"))
            return false;
        return true;
    }
    
    [MenuItem("XLua/HotFix/Disable", true)]
    private static bool DisableValidate()
    {
        var defines = GetDefinesList(hotFixBuildTargetGroups[0]);
        if (defines.Contains("HOTFIX_ENABLE"))
            return true;
        return false;
    }

    private static BuildTargetGroup[] buildTargetGroups = new BuildTargetGroup[]
        {
                BuildTargetGroup.Standalone,
                BuildTargetGroup.Android,
                BuildTargetGroup.iOS
        };

    private static BuildTargetGroup[] hotFixBuildTargetGroups = new BuildTargetGroup[]
        {
                BuildTargetGroup.Standalone,
                BuildTargetGroup.Android,
                BuildTargetGroup.iOS
        };


    private static void SetEnabled(string defineName, bool enable, bool mobile)
    {
        foreach (var group in mobile ? hotFixBuildTargetGroups : buildTargetGroups)
        {
            var defines = GetDefinesList(group);
            if (enable)
            {
                if (defines.Contains(defineName))
                {
                    return;
                }
                defines.Add(defineName);
            }
            else
            {
                if (!defines.Contains(defineName))
                {
                    return;
                }
                while (defines.Contains(defineName))
                {
                    defines.Remove(defineName);
                }
            }
            string definesString = string.Join(";", defines.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definesString);
        }
    }


    private static List<string> GetDefinesList(BuildTargetGroup group)
    {
        return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
    }
}
