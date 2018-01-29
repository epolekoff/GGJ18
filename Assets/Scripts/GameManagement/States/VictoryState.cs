using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryState : AbsState
{
    public override void Enter(IStateMachineEntity entity)
    {
        if(GameManager.Instance.CurrentLevel < GameManager.Instance.MaxLevel)
        {
            GameManager.Instance.GameCanvas.ShowVictoryUI(true);
        }
        else
        {
            GameManager.Instance.GameCanvas.ShowTotalVictoryUI(true);
        }
    }

    public override void Update(IStateMachineEntity entity)
    {

    }

    public override void Exit(IStateMachineEntity entity)
    {
        GameManager.Instance.GameCanvas.gameObject.SetActive(false);
        GameManager.Instance.GameCanvas.ShowVictoryUI(false);
        GameManager.Instance.PhoneCanvas.GameplayBackground.SetActive(false);
    }
}
