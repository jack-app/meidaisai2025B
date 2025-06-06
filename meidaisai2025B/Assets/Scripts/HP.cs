using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviour
{
    private Text hpText;
    private int chp;
    private int Mhp;
    PlayerHealth HP;

    void Start()
    {
        hpText = GetComponent<Text>();
        GameObject obj = GameObject.Find("GameObject");
        HP = obj.GetComponent<PlayerHealth>();
        Mhp = HP.maxHp;
    }

    void Update()
    {
        chp = HP.GetcurrentHp();
        hpText.text = chp + "/" + Mhp;
    }
}