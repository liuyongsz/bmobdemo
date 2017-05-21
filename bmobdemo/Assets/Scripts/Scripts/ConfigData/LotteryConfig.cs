using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LotteryInfo
{
    /// <summary>
    /// 编号
    ///-------------------------------------------------------
    /// </summary>
    public int type;

    /// <summary>
    /// 开启等级
    ///-------------------------------------------------------
    /// </summary>
    public int level;

    /// <summary>
    /// 免费次数CD
    ///-------------------------------------------------------
    /// </summary>
    public int freeCD;

    /// <summary>
    /// 每日免费次数
    ///-------------------------------------------------------
    /// </summary>
    public int freeCount;

    /// <summary>
    /// 消耗货币类型
    ///-------------------------------------------------------
    /// </summary>
    public int capitalType;

    /// <summary>
    /// 单抽价格
    ///-------------------------------------------------------
    /// </summary>
    public int capitalValue;

    /// <summary>
    /// 十连抽价格
    ///-------------------------------------------------------
    /// </summary>
    public float discount;

}
public class LotteryConfig : ConfigLoaderBase
{
    private static Dictionary<int, LotteryInfo> lotteryInfoList = new Dictionary<int, LotteryInfo>();
    protected override void OnLoad()
    {
        if (!ReadConfig<LotteryInfo>("Lottery.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        lotteryInfoList.Clear();
    }

    private void OnReadRow(LotteryInfo row)
    {
        lotteryInfoList[row.type] = row;
    }

    public static LotteryInfo GetLotteryInfoByType(int type)
    {
        if (lotteryInfoList.ContainsKey(type))
        {
            return lotteryInfoList[type];
        }
        return null;
    }
}
