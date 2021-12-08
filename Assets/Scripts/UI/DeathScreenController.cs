using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenController : GameOverScreenController
{
    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddListener("PlayerDeath", ShowScreenAndUnlockCursor);
        EventManager.Instance.AddListener("GameReloaded", imageController.HideScreen);
    }
}
