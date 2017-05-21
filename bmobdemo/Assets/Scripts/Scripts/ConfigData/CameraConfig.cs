using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;
using UnityEngine.Events;
using System.Xml;
using System.Text.RegularExpressions;

class CameraConfig: ConfigBase
{
    public float offX;
    public float offY;
    public float offZ;
    public float pX;
    public float pY;
    public float pZ;
    public float rX;
    public float rY;
    public float rZ;
    public string bound;
    public float fieldView;

    public void LoadXml(UnityEngine.Events.UnityAction loadedFun = null)
    {
        LoadData("Camera", loadedFun);
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

        TbXmlNode node = xmlNodeList[0] as TbXmlNode;
        offX = node.GetFloatValue("offX");
        offY = node.GetFloatValue("offY");
        offZ = node.GetFloatValue("offZ");
        pX = node.GetFloatValue("pX");
        pY = node.GetFloatValue("pY");
        pZ = node.GetFloatValue("pZ");
        rX = node.GetFloatValue("rX");
        rY = node.GetFloatValue("rY");
        rZ = node.GetFloatValue("rZ");
        bound = node.GetStringValue("bound");
        fieldView = node.GetFloatValue("fieldView");

        asset = null;
        base.onloaded(data);
    }

    public Rect GetBound()
    {
        string[] lst = bound.Split(',');
        return new Rect(float.Parse(lst[0]), float.Parse(lst[1]), float.Parse(lst[2]), float.Parse(lst[3]));
    }

    public void save()
    {
        string path = Application.dataPath + "/Config/Camera.xml";

        XmlDocument newDoc = new XmlDocument();
        newDoc.Load(path);
        string sContent = "";

        Regex _rgx = new Regex("\\<!--(.*?)--\\>", RegexOptions.Singleline);
        var mc = _rgx.Matches(newDoc.InnerXml);
        foreach (Match itemReg in mc)
        {
            sContent += itemReg.Groups[1].Value;//这里是注视
        }

        newDoc.RemoveAll();

        XmlElement ment;

        XmlDeclaration xmldecl = newDoc.CreateXmlDeclaration("1.0", "", null);
        newDoc.AppendChild(xmldecl);
        XmlElement root = newDoc.CreateElement("Object");

        newDoc.AppendChild(newDoc.CreateComment(sContent));
        newDoc.AppendChild(root);

        ment = newDoc.CreateElement("Property");
        ment.SetAttribute("offX", offX.ToString());
        ment.SetAttribute("offY", offY.ToString());
        ment.SetAttribute("offZ", offZ.ToString());
        ment.SetAttribute("pX", pX.ToString());
        ment.SetAttribute("pY", pY.ToString());
        ment.SetAttribute("pZ", pZ.ToString());
        ment.SetAttribute("rX", rX.ToString());
        ment.SetAttribute("rY", rY.ToString());
        ment.SetAttribute("rZ", rZ.ToString());
        ment.SetAttribute("bound", bound);
        ment.SetAttribute("fieldView", fieldView.ToString());
        root.AppendChild(ment);

        newDoc.Save(path);
    }
}

