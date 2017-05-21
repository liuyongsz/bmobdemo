using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;
using System.Diagnostics;
using System.Xml;
using System.Text;
using System.Security.AccessControl;

public static class LuaBytesBuilder
{

    [MenuItem("Assets/LuaBytesBuilder/Build LuaBytes")]
    static void BuildLuaBytes()
    {
        string tempDir = LuaConst.luaBytesDir;
        ClearAllLuaBytes(tempDir);
        CopyLuaBytesFiles(LuaConst.luaDir, tempDir);
        
        AssetDatabase.Refresh();
        PostprocessAssets();
        AssetDatabase.Refresh();
    }
    
    static void ClearAllLuaBytes(string dir)
    {
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
        }

        Directory.CreateDirectory(dir);
        AssetDatabase.Refresh();
    }
    
    static void CopyLuaBytesFiles(string sourceDir, string destDir, bool appendext = true, string searchPattern = "*.lua", SearchOption option = SearchOption.AllDirectories)
    {
        if (!Directory.Exists(sourceDir))
        {
            return;
        }

        string[] files = Directory.GetFiles(sourceDir, searchPattern, option);
        int len = sourceDir.Length;

        if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
        {
            --len;
        }

        for (int i = 0; i < files.Length; i++)
        {
            string str = files[i].Remove(0, len);
            string dest = destDir + "/" + str;
            if (appendext) dest += ".bytes";
            string dir = Path.GetDirectoryName(dest);
            Directory.CreateDirectory(dir);
            File.Copy(files[i], dest, true);
        }
    }

    static void PostprocessAssets()
    {
        AssetImporter importer = AssetImporter.GetAtPath("Assets/LuaBytes");
        if (importer)
        {
            importer.assetBundleName = "lua.bundle";
            importer.assetBundleVariant = null;
        }
    }
    
    /// <summary>
    /// 将某lua table内容保存到文件
    /// </summary>
    static bool SaveLuaFile(string tableName, string content)
    {
        try
        {
            string savePath = Application.dataPath + "/ConfigLua/Exported";
            string luaName = string.Format("{0}.lua",tableName);
            string filePath = string.Format("{0}/{1}", savePath, luaName);
            StreamWriter writer = new StreamWriter(filePath, false, new UTF8Encoding(false));
            writer.Write(content);
            writer.Flush();
            writer.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    static string GetLuaTableIndentation(int level)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < level; ++i)
            stringBuilder.Append("\t");

        return stringBuilder.ToString();
    }
}
