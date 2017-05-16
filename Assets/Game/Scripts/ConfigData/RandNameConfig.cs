using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;

public class RandNameConfig : ConfigBase
{

    /// <summary>
    ///    姓
    /// </summary>
    public static Dictionary<int, string> surnameList = new Dictionary<int, string>();

    /// <summary>
    ///    名
    /// </summary>
    public static Dictionary<int, string> nameList = new Dictionary<int, string>();

    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        LoadData("RandomName", loadedFun);
    }
    public override void onloaded(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;

        if (asset == null)
            return;

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return;
        }
        List<TbXmlNode> xmlNodeList = docNode.GetNodes("Object/Property");
        int xmlNodeListLength = xmlNodeList.Count;

        if (xmlNodeListLength < 1)
        {
            return;
        }

        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            int id = node.GetIntValue("ID");
            string surname = node.GetStringValue("TheSurname");
            string name = node.GetStringValue("TheName");
            if (surname != string.Empty)
            {
                if (surnameList.ContainsKey(id))
                    surnameList[id] = surname;
                else
                    surnameList.Add(id, surname);
            }
            if (nameList.ContainsKey(id))
                nameList[id] = name;
            else
                nameList.Add(id, name);
        }
        asset = null;

        base.onloaded(data);
    }
}
