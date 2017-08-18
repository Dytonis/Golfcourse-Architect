using GA.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Golfer : Actor
{
    public Clubhouse clubhouse;
    [SerializeField]
    public Round round;

    public Ball BallPrefab;
    public GameObject TeePrefab;

    public Ball PlayerBall;

    public void Init()
    {
        round = new Round();
    }

    #region ANIMATION_EVENTS
    public void AE_PlaceTee()
    {
        Debug.Log("AnimationEvent: AE_PlaceTee");

        Tees teebox = family.Gamemode.getTeebox(round.TeeType, round.CurrentHole);

        GameObject tee = Instantiate(TeePrefab, teebox.transform.position, teebox.transform.rotation);
        PlayerBall = Instantiate(BallPrefab, new Vector3(teebox.transform.position.x, teebox.transform.position.y + 0.06f, teebox.transform.position.z), teebox.transform.rotation) as Ball;
    }
    #endregion
}