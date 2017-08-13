using GA.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Golfer : Actor
{
    public Clubhouse clubhouse;
    [SerializeField]
    public Round round;

    public void Init()
    {
        round = new Round();
    }
}