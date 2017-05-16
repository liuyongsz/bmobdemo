using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

/// <summary>
/// 支持动态字体都Label组件
/// </summary>
public class SymbolLabel : MonoBehaviour
{
    /// <summary>
    /// 表情转移字符定义
    /// </summary>
    private List<string> m_Symbols = new List<string> { "{00}", "{01}", "{02}", "{03}", "{04}", "{05}"
                                            , "{06}", "{07}", "{08}", "{09}", "{10}","{11}", "{12}", "{13}", "{14}", "{15}"
                                            , "{16}", "{17}", "{18}", "{19}", "{20}","{21}", "{22}", "{23}", "{24}", "{25}"
                                            , "{26}", "{27}", "{28}", "{29}", "{30}","{31}", "{32}", "{33}", "{34}", "{35}"
                                            , "{36}", "{37}", "{38}", "{39}", "{40}","{41}", "{42}", "{43}", "{44}", "{45}"
                                            , "{46}", "{47}", "{48}", "{49}", "{50}","{51}", "{52}", "{53}", "{54}", "{55}"
                                            , "{56}", "{57}", "{58}", "{59}", "{60}","{61}", "{62}", "{63}", "{64}", "{65}"
                                            , "{66}", "{67}", "{68}", "{69}", "{70}","{71}", "{72}", "{73}", "{74}", "{75}"
                                            , "{76}", "{77}", "{78}", "{79}", "{80}","{81}", "{82}", "{83}", "{84}", "{85}"
                                            , "{86}", "{87}", "{88}", "{89}", "{90}","{91}", "{92}", "{93}", "{94}", "{95}"
                                            , "{96}", "{97}", };

    private string m_Text;
    private string m_realText;
    public string realText { get { return m_realText; } }

    private Vector2 m_textSize;
    public Vector2 textSize { get { return m_textSize; } }

    public UIFont uifont;
    //public Font font;
    public int fontSize = 26;
    public int symbolSize = 40;
    public int spacingY = 0;
    public int width = 100;
    public int depth = 0;
    public int maxLine = 0;
    public int textHeight = 24;
    public UILabel.Overflow overflowMethod = UILabel.Overflow.ResizeHeight;
    public NGUIText.Alignment alignment = NGUIText.Alignment.Left;
    public UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft;
    public UILabel.Effect effectType;

    private UILabel m_TextLabel;
    private UILabel m_SymbolLabel;

    private MatchCollection m_matchs;
    private MatchCollection m_spaceMatchs;
    private List<Match> m_realMatchs;

    void Awake()
    {
        m_realMatchs = new List<Match>();

        m_TextLabel = NGUITools.AddChild<UILabel>(gameObject);
        m_TextLabel.name = "textLabel";
        if (Config.SnailFont != null)
        {
            m_TextLabel.trueTypeFont = Config.SnailFont.dynamicFont;
        }
        
        m_TextLabel.spacingY = spacingY;
        m_TextLabel.fontSize = fontSize;
        m_TextLabel.overflowMethod = overflowMethod;
        m_TextLabel.alignment = alignment;
        m_TextLabel.pivot = pivot;
        m_TextLabel.width = width;
        m_TextLabel.depth = depth;
        m_TextLabel.transform.localPosition = Vector3.zero;
        m_TextLabel.SetSymbolOffset(SymbolOffset);
        if (overflowMethod == UILabel.Overflow.ClampContent)
        {
            m_TextLabel.height = textHeight;
            m_TextLabel.maxLineCount = maxLine;
        }

        m_SymbolLabel = NGUITools.AddChild<UILabel>(gameObject);
        m_SymbolLabel.name = "symbolLabel";
        m_SymbolLabel.bitmapFont = uifont;
        m_SymbolLabel.fontSize = symbolSize;
        m_SymbolLabel.overflowMethod = overflowMethod;
        m_SymbolLabel.alignment = alignment;
        m_SymbolLabel.pivot = pivot;
        m_SymbolLabel.depth = depth + 1;
        Vector3 vTemp = Vector3.zero;
        vTemp.x = 0;
        vTemp.y =  -3;
        vTemp.z =  0;
        m_SymbolLabel.transform.localPosition = vTemp;
        m_SymbolLabel.SetSymbolOffset(SymbolOffset);
        m_SymbolLabel.width = m_SymbolLabel.height = 10;

        m_TextLabel.effectStyle = effectType;
        if (effectType != UILabel.Effect.None)
        {
            m_TextLabel.effectColor = new Color(22f / 255, 1f / 255, 2f / 255);
            m_TextLabel.effectDistance = new Vector2(1, 1);
        }
    }

    /// <summary>
    /// 设置Label宽度
    /// </summary>
    /// <param name="width"></param>
    public void SetWidth(int width)
    {
        this.width = width;
        m_TextLabel.width = width;
        m_SymbolLabel.width = width;
    }

    public int height
    {
        get
        {
            return m_TextLabel.height;
        }
    }

    public UILabel labelText
    {
        get
        {
            return m_TextLabel;
        }
    }

    public UILabel labelSymbol
    {
        get
        {
            return m_SymbolLabel;
        }
    }
    StringBuilder sString = new StringBuilder();
    public string text
    {
        get { return m_Text; }

        set
        {
            if (string.IsNullOrEmpty(value))
            {
                m_Text = string.Empty;
                m_TextLabel.text = null;
                m_SymbolLabel.text = null;
                m_realMatchs.Clear();
                return;
            }

            m_realMatchs.Clear();

            m_Text = value;

            string mProcessedText = m_TextLabel.processedText;
            m_TextLabel.UpdateNGUIText();

            NGUIText.fontSize = fontSize;
            NGUIText.maxLines = maxLine;
            NGUIText.rectWidth = width;
            NGUIText.rectHeight = 100000;

            if (overflowMethod == UILabel.Overflow.ResizeHeight) mProcessedText = m_Text;
            else NGUIText.WrapSymbolText(m_Text, out mProcessedText, symbolSize);

            m_textSize = NGUIText.CalculatePrintedSize(value);
           

            string t = value;
            //const string pattern = "\\{\\d\\d*\\}";
            const string pattern = "\\{\\w\\w\\}";

            m_realText = NGUIText.StripSymbols(mProcessedText);
            m_matchs = Regex.Matches(m_realText, pattern);

            const string pat = " ";
            m_spaceMatchs = Regex.Matches(m_realText, pat);

            if (sString.Length > 0)
            {
                sString.Remove(0, sString.Length);
            }
         
            if (m_matchs.Count > 0)
            {
                Match item;
                for (int i = 0; i < m_matchs.Count; i++)
                {
                    item = m_matchs[i];

                    if (m_Symbols.IndexOf(item.Value) > -1)
                    {
                        m_realMatchs.Add(item);
                        sString.Append(item.Value);
                    }
                }
            }
            //int index = 0;
            //NGUIText.ParseSymbol(t, ref index);

            m_TextLabel.text = t;
            m_SymbolLabel.text = sString.ToString();

            m_SymbolLabel.width = m_TextLabel.width;
            m_SymbolLabel.height = m_TextLabel.height;

            m_SymbolLabel.MarkAsChanged();
        }
    }

    /// <summary>
    /// 修改顶点坐标 适配表情位置
    /// 1 — 2
    /// |  / |
    /// 0 — 3
    /// </summary>
    private void SymbolOffset()
    {
        BetterList<Vector3> textVerts = m_TextLabel.geometry.verts;
        BetterList<Vector3> symbolVerts = m_SymbolLabel.geometry.verts;
        Vector3 vTemp = Vector3.zero;
        vTemp.x = 0;
        vTemp.y =  0;
        Vector3 spacing = vTemp;
        if (textVerts.size > 0 && symbolVerts.size > 0)
        {
            Match item;
            float tw, sw, x = 0;
            int end, start;

            for (int i = 0; i < m_realMatchs.Count; i++)
            {
                item = m_realMatchs[i];

                //获取表情转移字符顶点开始、结束索引
                start = GetIndex(item.Index) * 4;
                end = start + (item.Length - 1) * 4 + 3;

                //表情的顶点索引
                int p = i * 4;

                if ((p + 3) >= symbolVerts.buffer.Length) break;

                //表情宽度
                sw = Mathf.Abs(symbolVerts.buffer[p].x - symbolVerts.buffer[p + 3].x);

                //如果不换行，计算文本表情转移符都宽带 否则换行不需要计算 添加1个单位距离 跟在后面
                if (textVerts.buffer[start].y == textVerts.buffer[end].y)
                {
                    //文本表情转义符宽度
                    tw = Mathf.Abs(textVerts.buffer[start].x - textVerts.buffer[end].x);

                    //计算居中坐标
                    x = (tw - sw) / 2;
                }
                else x = 1;

                //居中显示表情
                spacing.x = x;

                //计算偏移
                Vector2 po = m_TextLabel.pivotOffset;
                float fx = Mathf.Lerp(0f, -NGUIText.rectWidth, po.x);
                float fy = Mathf.Lerp(NGUIText.rectHeight, 0f, po.y) + Mathf.Lerp((m_TextLabel.printedSize.y - NGUIText.rectHeight), 0f, po.y);
                fx = Mathf.Round(fx);
                fy = Mathf.Round(fy);

                //计算出位移向量   
                Vector3 v = textVerts.buffer[start] - symbolVerts.buffer[p];

                //第一个顶点
                symbolVerts.buffer[p] = textVerts.buffer[start] + spacing;
                symbolVerts.buffer[p].x -= fx;
                symbolVerts.buffer[p++].y -= fy;

                //第二个顶点
                symbolVerts.buffer[p] = symbolVerts.buffer[p] + v + spacing;
                symbolVerts.buffer[p].x -= fx;
                symbolVerts.buffer[p++].y -= fy;

                //第三个顶点
                symbolVerts.buffer[p] = symbolVerts.buffer[p] + v + spacing;
                symbolVerts.buffer[p].x -= fx;
                symbolVerts.buffer[p++].y -= fy;

                //第四个顶点
                symbolVerts.buffer[p] = symbolVerts.buffer[p] + v + spacing;
                symbolVerts.buffer[p].x -= fx;
                symbolVerts.buffer[p++].y -= fy;

                for (int j = 0; j < item.Length; j++)
                {
                    //本来是希望将顶点坐标抹除、但是由于会出现坐标不对都情况、所以放弃了该方法，将顶点都颜色清除掉。
                    //textVerts.buffer[start++] = Vector3.zero;
                    //textVerts.buffer[start++] = Vector3.zero;
                    //textVerts.buffer[start++] = Vector3.zero;
                    //textVerts.buffer[start++] = Vector3.zero;

                    if (m_TextLabel.geometry.cols.size >= (start + 4))
                    {
                        m_TextLabel.geometry.cols[start++] = Color.clear;
                        m_TextLabel.geometry.cols[start++] = Color.clear;
                        m_TextLabel.geometry.cols[start++] = Color.clear;
                        m_TextLabel.geometry.cols[start++] = Color.clear;
                    }
                }
            }

        }
    }

    /// <summary>
    /// 获取表情转移字符'{'顶点索引，并且需要排除空格符的部分，因为空格符UILabel是不会生成顶点的 所以需要减去空格符都数量，才能正确获得表情索引
    /// </summary>
    /// <returns></returns>
    private int GetIndex(int itemIndex)
    {
        Match item;

        int count = 0;
        for (int i = 0; i < m_spaceMatchs.Count; i++)
        {
            item = m_spaceMatchs[i];
            if (item.Index < itemIndex)
            {
                count++;
            }
        }

        return itemIndex - count;
    }

}