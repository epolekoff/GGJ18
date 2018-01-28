using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : AbsState
{
    public override void Enter(IStateMachineEntity entity)
    {
        GameManager.Instance.GameCanvas.gameObject.SetActive(true);
        GameManager.Instance.GameCanvas.ShowGameUI(true);
    }

    public override void Update(IStateMachineEntity entity)
    {

    }

    public override void Exit(IStateMachineEntity entity)
    {
        
    }
}
