using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreenController : GameOverScreenController
{
    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddListener("Victory", ShowScreenAndUnlockCursor);
        EventManager.Instance.AddListener("GameReloaded", imageController.HideScreen);
    }
}
