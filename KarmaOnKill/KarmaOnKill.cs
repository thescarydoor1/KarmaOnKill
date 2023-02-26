using BepInEx;
using System.Security.Permissions;

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace KarmaOnKill
{
    [BepInPlugin("thescarydoor.karmaonkill", "KarmaOnKill", "1.0.1")]
    public partial class KarmaOnKill : BaseUnityPlugin
    {
        private bool enableKarmaOnKill = false;

        public void OnEnable()
        {
            // Startup and Cleanup
            On.GameSession.ctor += GameSessionCtorHook;
            On.RainWorld.OnModsInit += RainWorldOnModsInitHook;
            On.RainWorldGame.ShutDownProcess += RainWorldGameShutDownProcessHook;

            // Bonus Karma Logic
            On.Creature.Die += CreatureDieHook;
            On.Player.Update += PlayerUpdateHook;
            On.RegionGate.Update += RegionGateUpdateHook;
            On.StoryGameSession.ctor += StoryGameSessionCtorHook;
            On.TempleGuardAI.ThrowOutScore += TempleGuardAIThrowOutScoreHook;

            // Hat
            On.PlayerGraphics.DrawSprites += PlayerGraphicsDrawSpritesHook;
        }

        private void RainWorldGameShutDownProcessHook(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
        {
            orig(self);
            ClearMemory();
        }

        private void GameSessionCtorHook(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
        {
            orig(self, game);
            ClearMemory();
        }

        private void ClearMemory()
        {
            scavengerKills = 0;
            enableKarmaOnKill = false;
            creaturesSeen.Clear();
        }

        private void RainWorldOnModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            MachineConnector.SetRegisteredOI("karmaonkill", new Options());
            LoadHat();
        }
    }
}
