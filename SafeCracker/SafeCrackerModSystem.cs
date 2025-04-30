using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using HarmonyLib;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using System;

namespace SafeCracker
{
    [HarmonyPatch] // Place on any class with harmony patches
    public class SafeCrackerModSystem : ModSystem
    {
        private static Random rand = new Random();
        public static ICoreAPI api;
        static string configName = "safecracker_config.json";
        public Harmony harmony;
        public static SafeCrackerConfig SCConfig
        {
            get
            {
                return (SafeCrackerConfig)api.ObjectCache[configName];
            }
            set
            {
                api.ObjectCache.Add(configName, value);
            }
        }
        public override void Start(ICoreAPI api)
        {
            SafeCrackerModSystem.api = api;

            // The mod is started once for the server and once for the client.
            // Prevent the patches from being applied by both in the same process.
            if (!Harmony.HasAnyPatches(Mod.Info.ModID))
            {
                harmony = new Harmony(Mod.Info.ModID);
                harmony.PatchAll(); // Applies all harmony patches
            }
            SafeCrackerConfig safeCrackerConfig = null;
            try
            {
                safeCrackerConfig = api.LoadModConfig<SafeCrackerConfig>(configName);
            }
            catch (Exception e)
            {
                base.Mod.Logger.Warning($"Safecracker: Error loading config, regenerating. {e.Message}");
                api.StoreModConfig<SafeCrackerConfig>(new SafeCrackerConfig(), configName);
                safeCrackerConfig = api.LoadModConfig<SafeCrackerConfig>(configName);
            }
            if (safeCrackerConfig == null)
            {
                base.Mod.Logger.Warning("Safecracker: Regenerating default config as it was missing or broken...");
                api.StoreModConfig<SafeCrackerConfig>(new SafeCrackerConfig(), configName);
                safeCrackerConfig = api.LoadModConfig<SafeCrackerConfig>(configName);
            }
            SCConfig = safeCrackerConfig;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockBehaviorReinforcable), nameof(BlockBehaviorReinforcable.OnBlockExploded))]
        // Note that the name of the function does not matter
        public static bool OnBlockExploded(IWorldAccessor world, BlockPos pos, BlockPos explosionCenter, EnumBlastType blastType, ref EnumHandling handling)
        {
            ModSystemBlockReinforcement modBre;
            modBre = world.Api.ModLoader.GetModSystem<ModSystemBlockReinforcement>();
            BlockReinforcement bre = modBre.GetReinforcment(pos);

            if (bre != null && bre.Strength > 0)
            {
                var expstr = rand.Next(SCConfig.MinDamage, SCConfig.MaxDamage + 1);
                modBre.ConsumeStrength(pos, expstr);
                world.BlockAccessor.MarkBlockDirty(pos);
                if(expstr >=bre.Strength)
                {
                    handling = EnumHandling.PassThrough;
                }
                else
                {
                    handling = EnumHandling.PreventDefault;
                }  
                return false;
            }

            handling = EnumHandling.PassThrough;
            return false;
        }

    }

}
