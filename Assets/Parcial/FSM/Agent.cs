using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behaviours
{
    Chase, Patrol, Expode
}

public enum Flags
{
    OnTargetReach, OnTargetNear, OnTargetLost
}
public class Agent : MonoBehaviour
{
    private FSM<Behaviours, Flags> fsm;

    public Transform target;
    public float speed;
    public float explodeDistance;
    public float lostDistance;

    public Transform wayPoint1;
    public Transform wayPoint2;
    public float chaseDistance;

    void Start()
    {
        fsm = new FSM<Behaviours, Flags>();

        fsm.AddBehaviour<ChaseState>(Behaviours.Chase,
            onTickParameters: () => { return new object[] { transform, target, speed, explodeDistance, lostDistance }; });
        fsm.AddBehaviour<PatrolState>(Behaviours.Patrol,
            onTickParameters: () => { return new object[] { transform, wayPoint1, wayPoint2, target, speed, chaseDistance }; });
        fsm.AddBehaviour<ExplodeState>(Behaviours.Expode);

        fsm.SetTransition(Behaviours.Patrol, Flags.OnTargetNear, Behaviours.Chase, () => { Debug.Log("Te vi!"); });
        fsm.SetTransition(Behaviours.Chase, Flags.OnTargetReach, Behaviours.Expode);
        fsm.SetTransition(Behaviours.Chase, Flags.OnTargetLost, Behaviours.Patrol);

        fsm.ForceState(Behaviours.Patrol);
    }

    void Update()
    {
        fsm.Tick();
    }

}


