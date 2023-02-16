using System;

namespace KarmaOnKill
{
    public partial class KarmaOnKill
    {
        private FAtlas atlas;

        private void PlayerGraphicsDrawSpritesHook(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, UnityEngine.Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);

            if (atlas == null)
            {
                return;
            }

            string name = sLeaser.sprites[3]?.element?.name;
            if (enableKarmaOnKill && Options.UseHat.Value && name != null && name.StartsWith("HeadA") && atlas._elementsByName.TryGetValue("EvilSun" + name, out var element))
            {
                sLeaser.sprites[3].element = element;
            }
        }

        private void LoadHat()
        {
            try
            {
                atlas = atlas ?? Futile.atlasManager.LoadAtlas("sprites/evilsunhat");
            }
            catch (Exception)
            {
                Logger.LogWarning("Failed to load hat.");
            }

            if (atlas == null)
            {
                Logger.LogWarning("Evil Sun Hat atlas not found! Reinstall the mod.");
            }
        }
    }
}
