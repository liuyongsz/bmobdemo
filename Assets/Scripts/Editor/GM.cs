using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

public class GM : EditorWindow
{

    [MenuItem("UGame/GM")]
    static void OpenWindow()
    {
        EditorWindow.GetWindow<GM>(false, "GM命令", true).Show();
    }
    // 等级
    private string level;
    // 欧元
    private string euro;
    // 钻石
    private string diamond;
    //个人贡献点
    private string guildDonate;
    //公会资金
    private string guildFunds;
    //公会s声望
    private string guildReputation;
    // 体力
    private string bodyPower;

    private string blackMoney;
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        //输入框控件
        level = EditorGUILayout.TextField("设置主角等级", level, GUILayout.Width(300), GUILayout.Height(20));
        if (GUILayout.Button("确定", GUILayout.Width(50), GUILayout.Height(20)))
        {
            ServerCustom.instance.SendClientMethods("onClientGM", "level " + level);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        //输入框控件
        blackMoney = EditorGUILayout.TextField("添加黑市币", blackMoney, GUILayout.Width(300), GUILayout.Height(20));
        if (GUILayout.Button("确定", GUILayout.Width(50), GUILayout.Height(20)))
        {
            ServerCustom.instance.SendClientMethods("onClientGM", "blackMoney " + blackMoney);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        diamond = EditorGUILayout.TextField("添加钻石", diamond, GUILayout.Width(300), GUILayout.Height(20));
        if (GUILayout.Button("确定", GUILayout.Width(50), GUILayout.Height(20)))
        {
            ServerCustom.instance.SendClientMethods("onClientGM", "diamond " + diamond);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        guildDonate = EditorGUILayout.TextField("添加个人公会贡献点", guildDonate, GUILayout.Width(300), GUILayout.Height(20));
        if (GUILayout.Button("确定", GUILayout.Width(50), GUILayout.Height(20)))
        {
            ServerCustom.instance.SendClientMethods("onClientGM", "guildDonate " + guildDonate);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        euro = EditorGUILayout.TextField("添加欧元", euro, GUILayout.Width(300), GUILayout.Height(20));
        if (GUILayout.Button("确定", GUILayout.Width(50), GUILayout.Height(20)))
        {
            ServerCustom.instance.SendClientMethods("onClientGM", "euro " + euro);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        bodyPower = EditorGUILayout.TextField("添加体力", bodyPower, GUILayout.Width(300), GUILayout.Height(20));
        if (GUILayout.Button("确定", GUILayout.Width(50), GUILayout.Height(20)))
        {
            ServerCustom.instance.SendClientMethods("onClientGM", "bodyPower " + bodyPower);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        guildFunds = EditorGUILayout.TextField("添加公会资金", guildFunds, GUILayout.Width(300), GUILayout.Height(20));
        if (GUILayout.Button("确定", GUILayout.Width(50), GUILayout.Height(20)))
        {
            ServerCustom.instance.SendClientMethods("onClientGMAddGuildFunds",int.Parse(guildFunds));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        guildReputation = EditorGUILayout.TextField("添加公会声望", guildReputation, GUILayout.Width(300), GUILayout.Height(20));
        if (GUILayout.Button("确定", GUILayout.Width(50), GUILayout.Height(20)))
        {
            ServerCustom.instance.SendClientMethods("onClientGMAddGuildReputation", int.Parse(guildReputation));
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("批量添加道具", GUILayout.Width(120), GUILayout.Height(20)))
        {
            ServerCustom.instance.SendClientMethods("onClientGmAddAll");
        }
        GUILayout.EndHorizontal();
        
    }
 
}
