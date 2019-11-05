using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keepScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //OnGUI();
    }

    // Update is called once per frame
    void Update()
    {
      //  Blackjack.bjPlay.money++;
    }
    void OnGUI()
    {
        GUI.Box(new Rect(0, 600, 200, 200),"$"+ Blackjack.bjPlay.money.ToString());
    }
}
