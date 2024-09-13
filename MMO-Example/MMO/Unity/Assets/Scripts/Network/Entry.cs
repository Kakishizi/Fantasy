using System;
using System.Collections.Generic;
using Fantasy;
using UnityEngine;

using BestGame;

public class Entry : MonoBehaviour
{
    async void Start()
    {
        // 框架初始化
        await Sender.Ins.GameNetStart();

        // 进入登录场景
        UIFacade.Ins.EnterScene(new AccountScene());
        UIFacade.Ins.lastScene = UIFacade.Ins.currentScene;
    }
}
