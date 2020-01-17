using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ranks
{
    public const int straight_flush = 8, quads = 7, full_house = 6, flush = 5, straight = 4,
        three_card=3,two_pair=2,one_pair=1, high_card=0;

    public static int Score(List<int> cards)
    {
        if (StraightFlush(cards))
        {
            return straight_flush;
        }

        int pair_rank = Pair(cards);
        if (pair_rank == quads||pair_rank==full_house)
        {
            return pair_rank;
        }
        if (Flush(cards))
        {
            return flush;
        }
        if (Straight(cards))
        {
            return straight;
        }
        return pair_rank;
    }

    private static bool StraightFlush(List<int> cards)
    {
        int[] suits = new int[4];

        foreach (int id in cards)
        {
            suits[id / 13]++;
        }

        for(int s = 0; s < suits.Length; s++)
        {
            if (suits[s] >= 5)
            {
                List<int> list = new List<int>();
                foreach(int id in cards)
                {
                    if (id / 13 == s)
                    {
                        list.Add(id);
                    }
                }

                if (Straight(list))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static int Pair(List<int> cards)
    {
        int[] count = new int[13];

        foreach (int id in cards)
        {
            count[id % 13]++;
        }

        int rank = high_card;
        for(int i=0;i<count.Length;i++)
        {
            if (count[i] == 4)
            {
                rank = Mathf.Max(rank, quads);
            }
            if (count[i] == 3)
            {
                rank = Mathf.Max(rank, three_card);
                for(int j =0;j<count.Length;j++)
                {
                    if (i != j)
                    {
                        if (count[j] >= 2)
                        {
                            rank = Mathf.Max(rank, full_house);
                        }
                    }
                }
            }
            if (count[i] == 2)
            {
                rank = Mathf.Max(rank, one_pair);
                for(int j = 0; j < count.Length; j++)
                {
                    if (i != j)
                    {
                        if (count[j] >= 2)
                        {
                            rank = Mathf.Max(rank, two_pair);
                        }
                    }
                }
            }
        }

        return rank;
    }

    private static bool Flush(List<int> cards)
    {
        int[] suits = new int[4];

        foreach(int id in cards)
        {
            suits[id / 13]++;
        }

        if (suits.Where(n=>n>=5).Count()>=1)
        {
            return true;
        }

        return false;
    }

    private static bool Straight(List<int> cards)
    {
        int flag = 0;
        foreach(int id in cards)
        {
            flag |= 0x0001 << (id % 13);
        }

        if ((flag & 0x0001) != 0)
        {
            flag |= 0x0001<<13;
        }

        int mask = 0x001F;
        int inside = 0x2FFFF;
        while ((mask & inside) == mask)
        {
            if ((flag & mask) == mask)
            {
                return true;
            }
            mask=mask << 1;
        }

        return false;
    }

    public static bool FlushDraw(List<int> cards)
    {
        int[] suits = new int[4];

        foreach (int id in cards)
        {
            suits[id / 13]++;
        }

        if (suits.Where(n => n >= 4).Count() >= 1)
        {
            return true;
        }

        return false;
    }

    public static int StraightDraw(List<int> cards)
    {
        int flag = 0;
        foreach (int id in cards)
        {
            flag |= 0x0001 << (id % 13);
        }

        if ((flag & 0x0001) != 0)
        {
            flag |= 0x0001 << 13;
        }

        int mask3 = 0x001E;
        int inside3 = 0x1FFFE;
        while ((mask3 & inside3) == mask3)
        {
            if ((flag & mask3) == mask3)
            {
                return 3;
            }
            mask3 = mask3 << 1;
        }

        int mask2 = 0x005D ;
        int inside2 = 0x2FFFF ;
        while ((mask2 & inside2) == mask2)
        {
            if ((flag & mask2) == mask2)
            {
                return 2;
            }
            mask2 = mask2 << 1;
        }
        
        if ((flag & 0x000F) == 0x000F)
        {
            return 1;
        }
        if ((flag & 0x3C000) == 0x3C000)
        {
            return 1;
        }
        int[] mask1 = { 0x001D, 0x001B, 0x0017 };
        int inside1 = 0x2FFFF;
        foreach (int element in mask1)
        {
            int mask = element;
            while ((mask & inside1) == mask)
            {
                if ((flag & mask) == mask)
                {
                    return 1;
                }
                mask = mask << 1;
            }
        }

        return 0;
    }
}
