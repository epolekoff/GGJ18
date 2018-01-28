﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameState : AbsState
{
    public override void Enter(IStateMachineEntity entity)
    {
        GameManager.Instance.GameCanvas.gameObject.SetActive(true);
        GameManager.Instance.GameCanvas.ShowPreGameUI(true);
        GameManager.Instance.PhoneCanvas.GameplayBackground.SetActive(true);
    }

    public override void Update(IStateMachineEntity entity)
    {

    }

    public override void Exit(IStateMachineEntity entity)
    {
        GameManager.Instance.GameCanvas.ShowPreGameUI(false);
    }
}
