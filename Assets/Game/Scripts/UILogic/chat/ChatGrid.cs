using UnityEngine;
using System.Collections.Generic;
using System;

public class ChatGrid : UIWidgetContainer
{
    public delegate void OnReposition();

    public enum Arrangement
    {
        Horizontal,
        Vertical,
    }

    public Arrangement arrangement = Arrangement.Horizontal;

    public int maxPerLine = 0;

    public float cellWidth = 200f;

    public int cellHeight = 50;

    public bool animateSmoothly = false;

    public bool sorted = false;

    public bool hideInactive = true;

    public OnReposition onReposition = null;

    public bool isMainHeight = false;// 主界面特殊高度

    public bool repositionNow { set { if (value) { mReposition = true; enabled = true; } } }

    bool mStarted = false;

    bool mReposition = false;

    UIPanel mPanel;

    UIScrollView mDrag;

    bool mInitDone = false;

    public bool InitDone = false;

    void Init()
    {
        mInitDone = true;
        InitDone = true;
        mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
        mDrag = NGUITools.FindInParents<UIScrollView>(gameObject);
    }

    void Start()
    {
        if (!mInitDone) Init();
        mStarted = true;
        bool smooth = animateSmoothly;
        animateSmoothly = false;
        Reposition();
        animateSmoothly = smooth;
        enabled = false;
    }

    void Update()
    {
        if (mReposition) Reposition();
        enabled = false;
    }

    static public int SortByName(Transform a, Transform b) { return string.Compare(a.name, b.name); }

    [ContextMenu("Execute")]
    public void Reposition()
    {
        if (Application.isPlaying && !mStarted)
        {
            mReposition = true;
            return;
        }

        if (!mInitDone) Init();

        mReposition = false;
        Transform myTrans = transform;

        int x = 0;
        int y = 0;
        int height = 0;

        List<ChatItemBase> list = OnSortTime();

        for (int i = 0; i < list.Count; ++i)
        {
            ChatItemBase item = list[i];

            if (item == null) continue;

            Transform t = item.transform;

            if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;

            float depth = t.localPosition.z;
            Vector3 vTemp = Vector3.zero;
            vTemp.x = 0;
            vTemp.y =  height;
            vTemp.z =  depth;
            Vector3 pos = vTemp;
            height += cellHeight;      
            if (animateSmoothly && Application.isPlaying)
            {
                SpringPosition.Begin(t.gameObject, pos, 15f);
               
            }
            else t.localPosition = pos;
            if (++x >= maxPerLine && maxPerLine > 0)
            {
                x = 0;
                ++y;
            }
        }

        if (mDrag != null)
        {
            mDrag.UpdateScrollbars(true);

            if (isMainHeight)
                mDrag.RestrictWithinBounds(true);
            else
                mDrag.RestrictWithinBounds(true, false, true);
        }
        else if (mPanel != null)
        {
            //if (isMainHeight)
                mPanel.ConstrainTargetToBounds(myTrans, true);
        }

        if (onReposition != null)
            onReposition();
    }

    private List<ChatItemBase> OnSortTime()
    {
        List<ChatItemBase> list = new List<ChatItemBase>();
        ChatItemBase item;
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform t = transform.GetChild(i);
            item = NGUITools.FindInParents<ChatItemBase>(t);
            if (item != null && t.gameObject.activeSelf)
            {
                list.Add(item);
            }
        }

        list.Sort(OnCompareTime);

        return list;
    }

    /// <summary>
    /// 根据时间排序
    /// </summary>
    /// <param name = "x" ></ param >
    /// < param name="y"></param>
    /// <returns></returns>
    private int OnCompareTime(ChatItemBase x, ChatItemBase y)
    {
        if (x == null || y == null || x.Data == null || y.Data == null) return 0;

        ChatInfo dataX = x.Data as ChatInfo;
        ChatInfo dataY = y.Data as ChatInfo;

        if (dataX != null && dataY != null)
        {
            if (dataX.sendTime > dataY.sendTime)
                return -1;
            else
                return 1;
        }
        return 0;
    }
}
