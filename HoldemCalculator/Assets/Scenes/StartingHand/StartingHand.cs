using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartingHand : MonoBehaviour
{
    public Toggle[] left_num,left_suit,right_num,right_suit;

    public void OnClick()
    {
        HoleCardFlag holeCardFlag =
            new HoleCardFlag(left_num, left_suit, right_num, right_suit);
        Data.holeCards.Add(holeCardFlag);

        SceneManager.LoadScene("FlopResult");
    }
}
