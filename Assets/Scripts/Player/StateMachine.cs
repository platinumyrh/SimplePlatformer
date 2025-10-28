using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
  public PlayerState CurrentState {  get; private set; }

    public void Initialize(PlayerState staringState)
    {
        CurrentState = staringState;
        CurrentState.Enter();
    }
    public void ChangeState(PlayerState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }


}
