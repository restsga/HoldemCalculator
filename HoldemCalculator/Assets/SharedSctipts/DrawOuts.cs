using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class DrawOuts
{
    /*
     * n(<7)枚の札(int[])を入力して存在するドローのフラグを数値型(uint)で返す
     * 最下位ビットからガットショット,ダブルベリー,オープンエンド,フラッシュドロー
     * ストレートドロー系はオープンエンド>ダブルベリー>ガットショットの優先度でフラグを立てる
     * フラッシュドロー系はナッツフラッシュドロー>フラッシュドローの優先度
     * 
     * 7枚以上の入力も可能だが予期せぬ動作をする可能性あり
     * 少なくともフラドロとナッツフラドロの正確な判定は不可能
     * (例:8枚でクラブフラドロ＆ハートナッツフラドロ→クラブフラドロ)
     * 
     */

    //各ドローのID
    public const uint NO_DRAW = 0x0000;
    public const uint GUT_SHOT = 0x0001;
    public const uint DOUBLE_BELLY = 0x0002;
    public const uint OPEN_END = 0x0004;
    public const uint FLUSH_DRAW = 0x0008;
    public const uint NUTS_FLUSH_DRAW = 0x0010;

    public static uint Draw(int[] cards,int[] hole)
    {
        return FlushDraw(cards, hole) | StraightDraw(cards);
    }

    private static uint FlushDraw(int[] cards,int[] hole)
    {
        //各スートの枚数
        int[] suitCount = RankScore.CountSuit(cards);

        //フラッシュドロー、ナッツフラドロの検査
        for (int i = 0; i < suitCount.Length; i++)
        {
            //フラドロ以上
            if (suitCount[i] >= 4)
            {
                //手札と与えられたカード全体から指定のスートを抽出してカードランクをフラグに変換
                uint flag_cards = RankScore.HighCard(cards.Where(id => id / 13 == i).ToArray());
                uint flag_hand = RankScore.HighCard(hole.Where(id => id / 13 == i).ToArray());

                //繰り上がりで15ビット目が立つならナッツフラドロ
                if (flag_cards + flag_hand >= 0x4000)
                {
                    return NUTS_FLUSH_DRAW;
                }
                else
                {
                    return FLUSH_DRAW;
                }
            }
        }

        return NO_DRAW;
    }

    private static uint StraightDraw(int[] cards)
    {
        //ランクごとに存在するかフラグに変換
        uint flag= RankScore.HighCard(cards);

        {
            //オープンエンドとダブルベリー検出用のマスクと終了条件
            uint mask, end;

            //オープンエンド
            mask = 0x1E00;
            end = 0x001E;
            while (mask >= end)
            {
                if ((flag & mask) == mask)
                {
                    return OPEN_END;
                }

                //右に1ビットシフト
                mask = mask >> 1;
            }

            //ダブルベリー
            mask = 0x2E80;
            end = 0x005D;
            while (mask >= end)
            {
                if ((flag & mask) == mask)
                {
                    return DOUBLE_BELLY;
                }

                //右に1ビットシフト
                mask = mask >> 1;
            }
        }

        //A-4,J-A
        if((flag&0x3C00)==0x3C00 || (flag & 0x000F) == 0x000F)
        {
            return GUT_SHOT;
        }

        //ガットショット
        //マスクと終了条件が3種類存在 {10111,11011,11101}
        uint[] masks = { 0x2E00 ,0x3600,0x3A00};
        uint[] ends = { 0x0017 ,0x001B,0x001D};
        for (int i = 0; i < masks.Length; i++)
        {
            //マスクを取得
            uint mask = masks[i];

            while (mask >= ends[i])
            {
                if ((flag & mask) == mask)
                {
                    return GUT_SHOT;
                }

                //右に1ビットシフト
                mask = mask >> 1;
            }
        }

        return NO_DRAW;
    }
}
