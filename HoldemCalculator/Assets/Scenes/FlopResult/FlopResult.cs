using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlopResult : MonoBehaviour
{
    public Text text;

    private int straight_draw_1 = 0;
    private int straight_draw_2_belly = 0;
    private int straight_draw_2_open = 0;
    private int flush_draw = 0;
    private int three_card = 0;
    private int straight = 0;
    private int flush = 0;
    private int full_house = 0;
    private int quads = 0;
    private int straight_flush = 0;
    private int count = 0;

	void Start ()
    {
        Calculate();
	}

    private void Calculate()
    {
        straight_draw_1 = 0;
        straight_draw_2_belly = 0;
        straight_draw_2_open = 0;
        flush_draw = 0;
        straight = 0;
        flush = 0;
        full_house = 0;
        quads = 0;
        straight_flush = 0;
        count = 0;

        Flop();

        int all = 
            straight_draw_1 + straight_draw_2_belly+ straight_draw_2_open+flush_draw+
            three_card+ straight + flush + full_house + quads + straight_flush;
        string chances =
            (100.0f * straight_flush / count).ToString("F5") + "%" + Environment.NewLine +
            (100.0f * quads / count).ToString("F5") + "%" + Environment.NewLine +
            (100.0f * full_house / count).ToString("F5") + "%" + Environment.NewLine +
            (100.0f * flush / count).ToString("F5") + "%" + Environment.NewLine +
            (100.0f * straight / count).ToString("F5") + "%" + Environment.NewLine +
            (100.0f * three_card / count).ToString("F5") + "%" + Environment.NewLine +
            (100.0f * flush_draw / count).ToString("F5") + "%" + Environment.NewLine +
            (100.0f * straight_draw_2_open / count).ToString("F5") + "%" + Environment.NewLine +
            (100.0f * straight_draw_2_belly / count).ToString("F5") + "%" + Environment.NewLine +
            (100.0f * straight_draw_1 / count).ToString("F5") + "%" + Environment.NewLine +
            (100.0f * all / count).ToString("F5") + "%";

        text.text = chances;
    }

    private void Flop()
    {
        ulong lost = 0;
        ulong bit = 0;

        for (int a = 0; a < 52; a++)
        {
            for (int b = 0; b < 52; b++)
            {
                for (int c = 0; c < 52; c++)
                {
                    if (a < b && b < c)
                    {
                        PlayerLoop(0,new List<int[]>(), lost | (bit << a) | (bit << b) | (bit << c),a,b,c);
                    }
                }
            }
        }
    }

    private void PlayerLoop(int player,List<int[]> holecard, ulong lost,int a,int b,int c)
    {
        ulong bit = 0x0001;

        if (player < Data.holeCards.Count)
        {
            foreach (int[] id in Data.holeCards[player].id)
            {
                if (id[0] != a && id[0] != b && id[0] != c &&
                    id[1] != a && id[1] != b && id[1] != c)
                {
                    List<int[]> list = new List<int[]>();
                    list.AddRange(holecard);
                    list.Add(id);
                    PlayerLoop(player + 1, list, lost | (bit << id[0]) | (bit << id[1]), a, b, c);
                }
            }
        }
        else
        {
            RankCheck(lost,holecard,a,b,c);
        }
    }

    private void RankCheck(ulong lost,List<int[]> holecard,int a,int b,int c)
    {
        List<int> cards = new List<int>();
        cards.Add(holecard[0][0]);
        cards.Add(holecard[0][1]);
        cards.Add(a);
        cards.Add(b);
        cards.Add(c);


        count++;
        switch (Ranks.Score(cards))
        {
            case Ranks.straight_flush:
                straight_flush++;
                return;
            case Ranks.quads:
                quads++;
                return;
            case Ranks.full_house:
                full_house++;
                return;
            case Ranks.flush:
                flush++;
                return;
            case Ranks.straight:
                straight++;
                return;
            case Ranks.three_card:
                three_card++;
                return;
            default:
                break;
        }

        if (Ranks.FlushDraw(cards))
        {
            flush_draw++;
            return;
        }
        int straight_draw_rank = Ranks.StraightDraw(cards);
        switch (straight_draw_rank)
        {
            case 3:
                straight_draw_2_open++;
                return;
            case 2:
                straight_draw_2_belly++;
                return;
            case 1:
                straight_draw_1++;
                return;
            default:
                break;
        }
    }
}
