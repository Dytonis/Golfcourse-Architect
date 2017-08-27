using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GA.Game.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class Golfer : Actor
{
    private AIState _state;
    public AIState State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            _state.golfer = this;
            _state.OnBecameActiveState();
            Debug.Log("AIState Switched to " + _state.GetType().ToString());
        }
    }
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

    public void CompleteInSeconds(float seconds)
    {
        StartCoroutine(_CompleteInSeconds(seconds));
    }

    private IEnumerator _CompleteInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        State.Complete = true;
    }

    private void AITick()
    {
        AnimationController.SetFloat("Velocity", Velocity.magnitude);

        if(State.Complete)
        {
            State.TimeComplete += Time.deltaTime;
            State.OnTickDuringActionComplete();
        }
        else
        {
            State.OnTickDuringActionIncomplete();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Golfer))]
    public class GolferEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Golfer myScript = (Golfer)target;
            if (GUILayout.Button("Start AI"))
            {
                myScript.State = new AIStateWaitingForRound(new Vector2(myScript.clubhouse.InitialMoveSpot.transform.position.x, myScript.clubhouse.InitialMoveSpot.transform.position.z));
                myScript.StartAI();
            }

            GUILayout.Label("MSC: " + myScript.Personality.MaxShotCost.ToString());
        }
    }
#endif
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
