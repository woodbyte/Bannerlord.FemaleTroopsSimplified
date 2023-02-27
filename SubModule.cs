using Bannerlord.FemaleTroopsSimplified.Behaviors;
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

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (gameStarterObject is CampaignGameStarter cgs)
            {
                cgs.AddBehavior(new FemaleTroopsSimplifiedBehavior());
            }
        }
    }
}