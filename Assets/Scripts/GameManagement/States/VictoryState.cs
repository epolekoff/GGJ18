using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryState : AbsState
{
    public override void Enter(IStateMachineEntity entity)
    {
        GameManager.Instance.GameCanvas.ShowVictoryUI(true);
    }

    public override void Update(IStateMachineEntity entity)
    {

    }

    public override void Exit(IStateMachineEntity entity)
    {
        GameManager.Instance.GameCanvas.gameObject.SetActive(false);
        GameManager.Instance.GameCanvas.ShowVictoryUI(false);
    }
}
