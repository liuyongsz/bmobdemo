using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/***
 * http://blog.csdn.net/yupu56/article/details/69698253
数字 {01 ~ 09} 表示  {1 ~ 9} 筒
数字 {11 ~ 19} 表示  {1 ~ 9} 条
数字 {21 ~ 29} 表示  {1 ~ 9} 万
数字 {31 33 35 37 } 表示 { 东 南 西 北 }
数字 {41 43 45} 表示 {中 發 白}

数字10 20 30 32 34 36 40 42 44 46 空出来不代表任何麻将牌 
这样设计的好处就是使得能够形成顺子的牌在用数字表示出来的时候刚好也是连着的,
而不能够形成顺子的牌, 在用数字表示的时候并不是顺子 . 便于以后使用代码进行判断
***/
public class MaJiangUtil
{
    public static bool IsCanHU(List<int> mah, int ID)
    {
        List<int> pais = new List<int>(mah);

        pais.Add(ID);
        //只有两张牌
        if (pais.Count == 2)
        {
            return pais[0] == pais[1];
        }

        //先排序
        pais.Sort();

        //依据牌的顺序从左到右依次分出将牌
        for (int i = 0; i < pais.Count; i++)
        {
            List<int> paiT = new List<int>(pais);
            List<int> ds = pais.FindAll(delegate (int d)
            {
                return pais[i] == d;
            });

            //判断是否能做将牌
            if (ds.Count >= 2)
            {
                //移除两张将牌
                paiT.Remove(pais[i]);
                paiT.Remove(pais[i]);

                //避免重复运算 将光标移到其他牌上
                i += ds.Count;

                if (HuPaiPanDin(paiT))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool HuPaiPanDin(List<int> mahs)
    {
        if (mahs.Count == 0)
        {
            return true;
        }

        List<int> fs = mahs.FindAll(delegate (int a)
        {
            return mahs[0] == a;
        });

        //组成克子
        if (fs.Count == 3)
        {
            mahs.Remove(mahs[0]);
            mahs.Remove(mahs[0]);
            mahs.Remove(mahs[0]);

            return HuPaiPanDin(mahs);
        }
        else
        { //组成顺子
            if (mahs.Contains(mahs[0] + 1) && mahs.Contains(mahs[0] + 2))
            {
                mahs.Remove(mahs[0] + 2);
                mahs.Remove(mahs[0] + 1);
                mahs.Remove(mahs[0]);

                return HuPaiPanDin(mahs);
            }
            return false;
        }
    }
}
