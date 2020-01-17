using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Data
{
    public static List<HoleCardFlag> holeCards = new List<HoleCardFlag>();
}

public class HoleCardFlag
{
    public List<int[]> id=new List<int[]>();

    public HoleCardFlag(Toggle[] left_num, Toggle[] left_suit, Toggle[] right_num, Toggle[] right_suit)
    {
        for (int a = 0; a < 52; a++)
        {
            for (int b = 0; b < 52; b++)
            {
                if (a < b)
                {
                    if ((left_num[a % 13].isOn && left_suit[a / 13].isOn &&
                        right_num[b % 13].isOn && right_suit[b / 13].isOn)
                        ||
                        (left_num[b % 13].isOn && left_suit[b / 13].isOn &&
                        right_num[a % 13].isOn && right_suit[a / 13].isOn))
                    {
                        int[] array = { a, b };
                        id.Add(array);
                    }
                }
            }
        }
    }
}