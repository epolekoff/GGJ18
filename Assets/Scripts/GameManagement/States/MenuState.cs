using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuState : AbsState
{
    public override void Enter(IStateMachineEntity entity)
    {
        GameManager.Instance.HomeScreen.gameObject.SetActive(true);
    }

    public override void Update(IStateMachineEntity entity)
    {
        
    }

    public override void Exit(IStateMachineEntity entity)
    {
        GameManager.Instance.HomeScreen.gameObject.SetActive(false);
    }
}
