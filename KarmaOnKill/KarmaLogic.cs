using System;
using System.Collections.Generic;
using MoreSlugcats;
using RWCustom;

namespace KarmaOnKill
{
    public partial class KarmaOnKill
    {
        private int scavengerKills = 0;
        private readonly HashSet<int> creaturesSeen = new HashSet<int>();

        private int KarmaBonus()
        {
            return Math.Min(scavengerKills / Options.ScavengerKillsPerKarma.Value, Options.MaxBonusKarma.Value);
        }

        private void PlayerUpdateHook(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            int karmaBonus = KarmaBonus();
            if (karmaBonus > 0)
            {
                HUD.KarmaMeter karmaMeter = self.room.game.cameras[0].hud.karmaMeter;
                IntVector2 displayKarma = new IntVector2(karmaMeter.displayKarma.x + karmaBonus, karmaMeter.displayKarma.y + karmaBonus);
                karmaMeter.karmaSprite.element = Futile.atlasManager.GetElementWithName(HUD.KarmaMeter.KarmaSymbolSprite(true, displayKarma));
            }
        }

        private void RegionGateUpdateHook(On.RegionGate.orig_Update orig, RegionGate self, bool eu)
        {
            if (!enableKarmaOnKill)
            {
                orig(self, eu);
                return;
            }

            int karmaBonus = KarmaBonus();
            DeathPersistentSaveData saveData = (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData;
            int oldKarma = saveData.karma;
            saveData.karma += karmaBonus;

            if (Options.EnableDebugLogs.Value)
            {
                Logger.LogMessage($"Old karma {oldKarma}, New karma {saveData.karma}, Karma Bonus {karmaBonus}");
            }
            orig(self, eu);
            saveData.karma = oldKarma;

            if (Options.EnableDebugLogs.Value)
            {
                Logger.LogMessage($"New karma {saveData.karma}");
            }
        }

        private float TempleGuardAIThrowOutScoreHook(On.TempleGuardAI.orig_ThrowOutScore orig, TempleGuardAI self, Tracker.CreatureRepresentation crit)
        {
            if (!enableKarmaOnKill)
            {
                return orig(self, crit);
            }

            int karmaBonus = KarmaBonus();
            DeathPersistentSaveData saveData = (crit.representedCreature.realizedCreature.room.game.session as StoryGameSession).saveState.deathPersistentSaveData;
            int oldKarma = saveData.karma;
            int oldKarmaCap = saveData.karmaCap;
            saveData.karma += karmaBonus;
            saveData.karmaCap += karmaBonus;

            float adjustedThrowOutScore = orig(self, crit);
            saveData.karma = oldKarma;
            saveData.karmaCap = oldKarmaCap;
            return adjustedThrowOutScore;
        }

        private void StoryGameSessionCtorHook(On.StoryGameSession.orig_ctor orig, StoryGameSession self, SlugcatStats.Name saveStateNumber, RainWorldGame game)
        {
            orig(self, saveStateNumber, game);

            if (ModManager.MSC && saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
            {
                if (Options.EnableDebugLogs.Value)
                {
                    Logger.LogMessage("Artificer save, enabling KarmaOnKill.");
                }
                scavengerKills = 0;
                enableKarmaOnKill = true;
                creaturesSeen.Clear();
            }
        }

        private void CreatureDieHook(On.Creature.orig_Die orig, Creature self)
        {
            if (enableKarmaOnKill && self is Scavenger
                && self?.killTag.creatureTemplate.type == CreatureTemplate.Type.Slugcat
                && !creaturesSeen.Contains(self.abstractCreature.ID.number))
            {
                creaturesSeen.Add(self.abstractCreature.ID.number);  // Explosions cause multiple deaths of the same creature for some reason.
                scavengerKills++;
                if (Options.EnableDebugLogs.Value)
                {
                    Logger.LogMessage($"Scavenger kills: {scavengerKills}");
                }

                HUD.KarmaMeter karmaMeter = self.room.game.cameras[0].hud.karmaMeter;
                karmaMeter.forceVisibleCounter = 40;
                karmaMeter.ClearScavengerFlash();
            }
            orig(self);
        }
    }
}
