using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Golfer : Actor
{
    public GolferAIState State;
    public Animator AnimationController;
    public ChunkFamily family;
    public StandardGamemode gamemode;

    public int GroupSize = 1;
    public List<Golfer> Group = new List<Golfer>();
    public bool GroupHonors = false;

    public int animationState = 0;

    public void StartAI()
    {
        StartCoroutine(cycle());
    }

    private IEnumerator cycle()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.25f);
            AITick();
        }
    }

    public void GroupMessage_GiveHonors()
    {
        GroupHonors = true;
    }

    public void GroupMessage_WaitForOther(Golfer g)
    {

    }

    private void AITick()
    {
        AnimationController.SetFloat("Velocity", Velocity.magnitude);

        switch(State)
        {
            case GolferAIState.Arriving: AITickArriving(); break;
            case GolferAIState.Wandering: AITickWandering(); break;
            case GolferAIState.WaitingForRound: AITickWaitingForRound(); break;
            case GolferAIState.TeeBall: AITickTeeBall(); break;
        }
    }

    private void AITickTeeBall()
    {
        if (animationState == 0)
        {
            animationState = 1;
            if (family.HoleList.Count >= round.CurrentHole - 1)
            {
                Hole goToHole = family.HoleList[round.CurrentHole - 1];

                StartToPathToPoint(new Vector2(goToHole.TeesList[0].Position.x, goToHole.TeesList[0].Position.z), family);
            }
        }
    }

    private void AITickWaitingForRound()
    {
        if (gamemode.TimeOfDay >= round.TeeTime - 100)
        {
            if (GroupSize < 2 || GroupHonors == true)
            {
                round.CurrentHole = 1;
                round.Start();
                State = GolferAIState.TeeBall;
            }
        }

        //wander, sit, eat, talk, drink, buy
        if (Random.Range(0f, 1f) < 0.5f)
        {
            if (!Moving)
            {
                float newX = anchorPosition.x + Random.Range(-4f, 4f);
                float newY = anchorPosition.x + Random.Range(-4f, 4f);

                StartToMovePoint(new Vector2(newX, newY));
            }
        }
        else
        {
            //look for a place to sit
        }
    }

    private Vector2 anchorPosition;
    private void AITickWandering()
    {
        if (!Moving)
        {
            if (Random.Range(0f, 1f) < 0.6f)
            {
                float newX = anchorPosition.x + Random.Range(-4f, 4f);
                float newY = anchorPosition.x + Random.Range(-4f, 4f);

                StartToMovePoint(new Vector2(newX, newY));
            }
        }
    }

    private void AITickArriving()
    {
        anchorPosition = FlatPosition;
        State = GolferAIState.WaitingForRound;
    }
}

public enum GolferAIState
{
    Undefined,
    Arriving,
    Idling,
    WaitingForRound,
    Wandering,   
    WaitingAtGroupForGroup,
    WaitingAtBallForGroup,
    WaitingAtBallForClear,
    WaitingThinkingAtBall,
    WaitingReadyAtBall,
    Swinging,
    WaitingLookingAtBall,
    ReactingToBall,
    ReactingToGroup,
    ReactingToEvent,
    PickupBall,
    TeeBall,
    TakingFlag,
    ReplacingFlag,
    BuyingItem,
    UsingItem,
    InteractingWithObject,
}
