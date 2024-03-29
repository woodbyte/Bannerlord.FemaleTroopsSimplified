﻿using Bannerlord.FemaleTroopsSimplified.Behaviors;
using Bannerlord.FemaleTroopsSimplified.Configuration;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.FemaleTroopsSimplified
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            var harmony = new Harmony("bannerlord.femaletroopssimplified");
            harmony.PatchAll();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            FemaleTroopsSimplifiedBehavior.Clean();
            GlobalSettings.Initialize();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (gameStarterObject is CampaignGameStarter cgs)
            {
                if (GlobalSettings.Instance != null)
                    GlobalSettings.Instance.Unregister();

                cgs.AddBehavior(new FemaleTroopsSimplifiedBehavior());
            }
        }
    }
}