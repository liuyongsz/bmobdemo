using UnityEngine;
using UnityEditor;


[UnityEditor.CustomEditor(typeof(MyCamera))]
public class CameraEdit : Editor {


	public void EditorTest()
	{

	}

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Save Camera Data"))
        {
            MyCamera.Instance.SaveData();
            BinaryXmlConvertor.SaveSngXML("Camera");
        }
    }
}
