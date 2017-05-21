using UnityEngine;
using System.Collections;
using XLua;
using UnityEngine.UI;
using System.Collections.Generic;

public enum DirectionType
{
    Null,
    Left,
    Right,
    UP,
    Down,
}
public class TestPanel3 : UIBase
{

    private Text m_text;
    private GameObject btn;
    private GameObject backBtn;
    private GameObject sure;
    private GameObject leftBtn;
    private GameObject rightBtn;
    private GameObject upBtn;
    private GameObject downBtn;

    private UIGrid ballerGrid;
    public UIInput iptUser;
    private DirectionType mCurDir;
    public List<Transform> listTrans = new List<Transform>();

    private float time = 0;
    public float duration = 1;

    private Transform mCurTrans;

    private bool IsPress;

    private Vector3 mVector ;
    protected override void Init()
    {
    
    }

    protected override void OnStart()
    {
        ballerGrid = gameObject.transform.FindChild("formineList").GetComponent<UIGrid>();
        iptUser = gameObject.transform.FindChild("iptUser").GetComponent<UIInput>();
        btn = gameObject.transform.FindChild("btn").gameObject;
        backBtn = gameObject.transform.FindChild("backBtn").gameObject;
        sure = gameObject.transform.FindChild("sure").gameObject;

        leftBtn = gameObject.transform.FindChild("GameObject/leftBtn").gameObject;
        rightBtn = gameObject.transform.FindChild("GameObject/rightBtn").gameObject;
        upBtn = gameObject.transform.FindChild("GameObject/upBtn").gameObject;
        downBtn = gameObject.transform.FindChild("GameObject/downBtn").gameObject;


        ballerGrid.enabled = true;
        ballerGrid.BindCustomCallBack(OnUpdateDataRow);
        ballerGrid.StartCustom();

    }
    protected override void OnUpdate()
    {
        base.OnUpdate();


    }
    protected override void Register()
    {
        UIEventListener.Get(btn).onClick = OnClick;
        UIEventListener.Get(backBtn).onClick = OnClick;
        UIEventListener.Get(sure).onClick = OnClick;

        LongClickEvent.Get(leftBtn).onPress = OnClickPress;
        LongClickEvent.Get(rightBtn).onPress = OnClickPress;
        LongClickEvent.Get(upBtn).onPress = OnClickPress;
        LongClickEvent.Get(downBtn).onPress = OnClickPress;

    }

    private void SetBaller()
    {
        ballerGrid.ClearCustomGrid();
        string str = iptUser.value.Trim(' ');
        string[] arr = str.Split(',');
        List<object> listObj = new List<object>();
        
        for (int i=0; i< arr.Length; i++)
        {
            listObj.Add(arr[i]);
            
        }
        ballerGrid.AddCustomDataList(listObj);

    }



    private void OnClickPress(GameObject go,bool isPress)
    {
        if (mCurTrans == null||!isPress)
            return;
        IsPress = isPress;
        switch (go.name)
            {
                case "leftBtn":
                    mCurTrans.transform.Translate(Vector3.left * Time.deltaTime/7);
                    break;
                case "rightBtn":
                mCurTrans.transform.Translate(Vector3.right * Time.deltaTime/7);
                    break;
                case "upBtn":
                mCurTrans.transform.Translate(Vector3.up * Time.deltaTime/7);
                    break;
                case "downBtn":
                mCurTrans.transform.Translate(Vector3.down * Time.deltaTime/7);
                    break;
            }
    }

    private void OnClick(GameObject go)
    {
        if (go.transform.name == "backBtn")
        {
            PanelManager.ClosePanel("TestPanel3");
            return;
        }
        if (go.transform.name == "sure")
        {
            SetBaller();
            return;
        }
        OutputInfo();
    }

    private void OutputInfo()
    {
        string content = "";
        listTrans.Clear();
        int i = 0;
        for ( i = 0; i < ballerGrid.transform.childCount; i++)
        {
            Transform child = ballerGrid.transform.GetChild(i);
            listTrans.Add(child);
         
        }
        listTrans.Sort(CompareItem);
        for ( i = 0; i < listTrans.Count; i++)
        {
            Transform child = listTrans[i];
         
            string vector = "";
            float pox = child.transform.localPosition.x;
            float poy = child.transform.localPosition.y;
            vector = GameConvert.IntConvert(pox) + "," + GameConvert.IntConvert(poy);
            content += vector + ";";
        }
        Debug.Log("-------POS-----:   " + content);

    }

    private int CompareItem(Transform trans1, Transform trans2)
    {
        int value1 = int.Parse(trans1.name.ToString());
        int value2 = int.Parse(trans2.name.ToString());

        if (value1 < value2)
            return -1;
        else if (value1 > value2)
            return 1;

        return 0;
    }
    private void OnUpdateDataRow(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        //UIEventListener.Get(item.gameObject).onPress = OnPressed;
        item.onDrag = Ondrag;
        UILabel name = item.mScripts[0] as UILabel;
        UILabel pos = item.mScripts[1] as UILabel;
        item.onClick = ClickItem;

        name.text = item.oData.ToString();
        item.transform.name = item.oData.ToString();
    }
    private void OnPressed(GameObject item, bool isPressed)
    {
        Vector3 mousePosition = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
        item.transform.position = mousePosition;
    }
    private void Ondrag(UIGridItem item,Vector2 pos)
    {
        //item.transform.localPosition = pos;
        mCurTrans = item.transform;
        Vector3 mousePosition = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
        item.transform.position = mousePosition;
    }

    private void ClickItem(UIGridItem item)
    {
        if (mCurTrans != null)
            mCurTrans.GetComponent<UIGridItem>().Selected = false;
        item.Selected = true;
        mCurTrans = item.transform;

    }
    protected override void UnRegister()
    {
        
    }

    protected override void Destroy()
    {
        
    }
}
