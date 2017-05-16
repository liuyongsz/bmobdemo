using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using UnityEditor.ProjectWindowCallback;
using System.Text.RegularExpressions;
using System.Diagnostics;

public class CustomLuaEditor
{
    [MenuItem("Assets/Create/LJ Script/Lua Script", false,80)]
    public static void CreatNewLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewLua.lua",
        null,
       "Assets/Editor/XLuaEditor/LuaTemplate.txt");
    }

    [MenuItem("Assets/Create/LJ Script/Lua HotFix Script", false,80)]
    public static void CreateNewHotFixLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewLua.lua",
        null,
       "Assets/Editor/XLuaEditor/LuaHotFixTemplate.txt");
    }

    [MenuItem("Assets/Create/LJ Script/UI Script", false,80)]
    public static void CreateNewUIScript()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewPanel.cs",
        null,
       "Assets/Editor/XLuaEditor/UIScript.txt");
    }

    [MenuItem("Assets/Create/LJ Script/MonoBehaviour Script", false,80)]
    public static void CreateNewMonoBehaviourScript()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewBehaviourScript.cs",
        null,
       "Assets/Editor/XLuaEditor/MonoBehaviourScript.txt");
    }

    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    [MenuItem("Assets/Open Lua Project")]
    public static void OpenLuaProject()
    {
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = LuaConst.zbsExe;
        info.Arguments = "";
        info.WindowStyle = ProcessWindowStyle.Minimized;
        Process pro = Process.Start(info);
        pro.Close();
    }
}


class MyDoCreateScriptAsset : EndNameEditAction
{


    public override void Action(int instanceId, string pathName, string resourceFile)
    {

        UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
        ProjectWindowUtil.ShowCreatedAsset(o);
    }

    internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
    {
        string fullPath = Path.GetFullPath(pathName);
        StreamReader streamReader = new StreamReader(resourceFile);
        string text = streamReader.ReadToEnd();
        streamReader.Close();
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
        text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
        //bool encoderShouldEmitUTF8Identifier = true;
        //bool throwOnInvalidBytes = false;
        //UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
        
        bool append = false;
        StreamWriter streamWriter = new StreamWriter(fullPath, append, Encoding.ASCII);
        streamWriter.Write(text);
        streamWriter.Close();
        AssetDatabase.ImportAsset(pathName);
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
    }

}