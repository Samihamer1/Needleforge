using Needleforge.Attacks;
using Needleforge.Data;
using SharpDX.DirectInput;
using System.Linq;
using UnityEngine;
using static Needleforge.Data.VanillaAttacks;
using ConfigGroup = HeroController.ConfigGroup;


namespace Needleforge.Makers;

internal class MovesetMaker
{
    private static ConfigGroup? hunter;

    internal static void InitializeMoveset(MovesetData moveset)
    {
        if (!TryFindDefaultMovesets())
            return;

        if (!moveset.HeroConfig)
            moveset.HeroConfig = HeroConfigNeedleforge.Copy(hunter!.Config);

        HeroController hc = HeroController.instance;

        GameObject root = new(moveset.Crest.name);
        root.transform.SetParent(hunter!.ActiveRoot.transform.parent);
        root.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);


        // In case of vanilla options
        #region Charged slash
        GameObject? Charged_Slash = null;

        if (moveset.UseVanillaChargedSlash != null)
        {
            GameObject? chargedPrefab = VanillaAttacks.ChargedSlashes.GetChargedSlashForCrest(moveset.UseVanillaChargedSlash);
            if (chargedPrefab != null)
            {
                ClonedAttack clonedChargedAttack = new()
                {
                    OriginalObject = chargedPrefab,
                    Name = $"{moveset.Crest.name} {moveset.UseVanillaChargedSlash} Charged Slash clone"
                };
                Charged_Slash = clonedChargedAttack.CreateGameObject(root, hc);
            }
        }

        if (Charged_Slash == null)
            Charged_Slash = AttackOrDefault(moveset.ChargedSlash, hunter.ChargeSlash);
        #endregion

        #region Downslash and Alt
        GameObject? DownSlash = null;
        GameObject? AltDownSlash = null;
        if (moveset.UseVanillaDownSlash != null)
        {
            VanillaAttackObjects? objects = CustomDownSlashes.GetDownSlashForCrest(moveset.UseVanillaDownSlash);

            if (objects != null) {
                GameObject downslashPrefab = objects.Attack;
                ClonedAttack clonedAttack = new() { OriginalObject = downslashPrefab, 
                    Name = $"{moveset.Crest.name} {moveset.UseVanillaDownSlash} Downslash clone" };
                DownSlash = clonedAttack.CreateGameObject(root, hc);

                //Only in the case of architect do we have a second prefab to clone.
                //The charged variant will be stored in the alt downslash slot.
                //This should be fine since with a custom downslash, you never access alt downslash anyway.
                if (moveset.UseVanillaDownSlash == VanillaAttackType.ARCHITECT)
                {
                    GameObject chargedPrefab = objects.AttackAlt!;
                    ClonedAttack clonedAttackCharged = new()
                    {
                        OriginalObject = chargedPrefab,
                        Name = $"{moveset.Crest.name} {moveset.UseVanillaDownSlash} Downslash clone Charged"
                    };
                    AltDownSlash = clonedAttackCharged.CreateGameObject(root, hc);
                }
            }
        }

        if (DownSlash == null)
        {
            DownSlash = AttackOrDefault(moveset.DownSlash, hunter.DownSlashObject);
        }

        if (AltDownSlash == null)
        {
            AltDownSlash = moveset.AltDownSlash?.CreateGameObject(root, hc);
        }
        #endregion

        #region Ensuring correct event is sent when using vanilla down slashes

        switch (moveset.UseVanillaDownSlash)
        {
            case VanillaAttackType.BEAST:
                moveset.HeroConfig.downSlashType = HeroControllerConfig.DownSlashTypes.Custom;
                moveset.HeroConfig.downSlashEvent = "WARRIOR DOWNSLASH";
                break;
            case VanillaAttackType.REAPER:
                moveset.HeroConfig.downSlashType = HeroControllerConfig.DownSlashTypes.Custom;
                moveset.HeroConfig.downSlashEvent = "RPR DOWNSLASH";
                break;
            case VanillaAttackType.WITCH:
                moveset.HeroConfig.downSlashType = HeroControllerConfig.DownSlashTypes.Custom;
                moveset.HeroConfig.downSlashEvent = "WITCH DOWNSLASH";
                break;
            case VanillaAttackType.SHAMAN:
                moveset.HeroConfig.downSlashType = HeroControllerConfig.DownSlashTypes.Custom;
                moveset.HeroConfig.downSlashEvent = "SHAMAN DOWNSLASH";
                break;
            case VanillaAttackType.ARCHITECT:
                moveset.HeroConfig.downSlashType = HeroControllerConfig.DownSlashTypes.Custom;
                moveset.HeroConfig.downSlashEvent = "TOOLMASTER DOWNSLASH";
                break;
            case VanillaAttackType.HUNTER: case VanillaAttackType.WANDERER: case VanillaAttackType.CLOAKLESS:
                ModHelper.LogWarning($"Crest {moveset.Crest.name} is requesting use of a DownSlash type" +
                    $" {moveset.UseVanillaDownSlash}, which has no unique downslash." + 
                    " Check the VanillaAttacks class for the right DownAttack prefab.");
                break;

        }

        #endregion

        #region Dash slash and Alt, also Special
        GameObject? DashSlash = null;
        GameObject? DashSlashAlt = null;
        GameObject? SpecialSlash = null; //Currently only wanderer's recoil slash.

        if (moveset.UseVanillaDashSlash != null)
        {
            VanillaAttackObjects? objects = DashSlashes.GetDashSlashForCrest(moveset.UseVanillaDashSlash);

            if (objects != null)
            {
                ClonedAttack clonedDashSlash = new()
                {
                    OriginalObject = objects.Attack,
                    Name = $"{moveset.Crest.name} {moveset.UseVanillaDashSlash} Dash Slash clone"
                };
                DashSlash = clonedDashSlash.CreateGameObject(root, hc);

                ClonedAttack clonedDashSlashAlt = new()
                {
                    OriginalObject = objects.AttackAlt,
                    Name = $"{moveset.Crest.name} {moveset.UseVanillaDashSlash} Dash Slash clone Alt"
                };

                ClonedAttack clonedSpecialSlash = new()
                {
                    OriginalObject = objects.AttackSpecial,
                    Name = $"{moveset.Crest.name} {moveset.UseVanillaDashSlash} Special Slash clone"
                };

                if (objects.AttackAlt != null)
                    DashSlashAlt = clonedDashSlashAlt.CreateGameObject(root, hc);

                if (objects.AttackSpecial != null)
                    SpecialSlash = clonedSpecialSlash.CreateGameObject(root, hc);

                //super special case for witch
                if (moveset.UseVanillaDashSlash == VanillaAttackType.WITCH)
                {
                    GameObject dashroot = new GameObject($"{moveset.Crest.name} Witch Dash Slash clone");
                    dashroot.transform.parent = root.transform;

                    //copying dimensions of dash slash parent
                    Transform originalParent = objects.Attack.transform.parent;
                    dashroot.transform.localPosition = originalParent.localPosition;
                    dashroot.transform.localScale = originalParent.localScale;

                    //renaming slashes so that they can be found easier
                    DashSlash.name = "Dash Slash 1";
                    DashSlashAlt!.name = "Dash Slash 2";

                    //reparenting
                    DashSlash.transform.parent = dashroot.transform;
                    DashSlashAlt!.transform.parent = dashroot.transform;

                    //cleaning stored objects, only dash slash parent is stored
                    DashSlash = dashroot;
                    DashSlashAlt = null;
                }
            }
        } 

        if (DashSlash == null)
        {
            DashSlash = AttackOrDefault(moveset.DashSlash, hunter.DashStab);
        }
        #endregion

        moveset.ConfigGroup = new ConfigGroupNeedleforge()
        {
            ActiveRoot = root,
            Config = moveset.HeroConfig,

            // If the moveset doesn't define one of the minimum required attacks
            // for crests to function, copy it from Hunter
            NormalSlashObject = AttackOrDefault(moveset.Slash,     hunter.NormalSlashObject),
            UpSlashObject =     AttackOrDefault(moveset.UpSlash,   hunter.UpSlashObject),
            WallSlashObject =   AttackOrDefault(moveset.WallSlash, hunter.WallSlashObject),
            DownSlashObject =   DownSlash,
            DashStab =          DashSlash,
            DashStabAlt =       DashSlashAlt,
            SpecialSlash =      SpecialSlash,
            ChargeSlash =       Charged_Slash,
            TauntSlash =        AttackOrDefault(null, hunter.TauntSlash),

            AlternateSlashObject = moveset.AltSlash?.CreateGameObject(root, hc),
            AltUpSlashObject =     moveset.AltUpSlash?.CreateGameObject(root, hc),
            AltDownSlashObject =   AltDownSlash,
        };

        hc.configs = [.. hc.configs, moveset.ConfigGroup];

        moveset.ExtraInitialization();
        HeroConfigErrorChecking(moveset);
        moveset.ConfigGroup.Setup();

        GameObject? AttackOrDefault(GameObjectProxy? attack, GameObject? _default)
        {
            if (attack == null)
            {
                if (!_default)
                    return null;
                else
                {
                    GameObject clone = Object.Instantiate(_default, root.transform);
                    clone.name = clone.name.Replace("(Clone)", "");
                    return clone;
                }
            }
            return attack.CreateGameObject(root, hc);
        }

        GameObject? DashAttackOrDefault(GameObjectProxy? attack, GameObject? _default)
        {
            if (attack == null)
            {
                if (!_default)
                    return null;
                else
                {
                    GameObject cloneParent = new("Dash Stab Parent");
                    cloneParent.transform.parent = root.transform;
                    cloneParent.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    cloneParent.transform.localScale = Vector3.one;

                    GameObject clone = Object.Instantiate(_default, cloneParent.transform);
                    clone.name = clone.name.Replace("(Clone)", "");

                    return cloneParent;
                }
            }
            return attack.CreateGameObject(root, hc);
        }
    }

    private static bool TryFindDefaultMovesets() {
        HeroController hc = HeroController.instance;

        if (!hc)
            return false;

        if (hunter == null || !hunter.Config || !hunter.NormalSlashObject)
            hunter = hc.configs.First(c => c.Config.name == "Default");

        return true;
    }

    private static void HeroConfigErrorChecking(MovesetData moveset) {
		HeroController hc = HeroController.instance;
		string
            name = moveset.Crest.name,
			m = nameof(CrestData.Moveset),
            mcfg = $"{m}.{nameof(MovesetData.HeroConfig)}",
            tcfg = $"{nameof(ToolCrest)}.{nameof(ToolCrest.HeroConfig)}",
            gcfg = $"{m}.{nameof(MovesetData.ConfigGroup)}.{nameof(ConfigGroup.Config)}",
            correctSetter = $"The only place you should set the moveset config is {mcfg}";

        // Config in MovesetData, ToolCrest, and ConfigGroup should be the exact same object.
		if (
			!ReferenceEquals(moveset.HeroConfig, moveset.Crest.ToolCrest!.HeroConfig)
			|| !ReferenceEquals(moveset.HeroConfig, moveset.ConfigGroup!.Config)
		) {
			ModHelper.LogWarning(
				$"{name}: {mcfg} object is not the same object as its {gcfg} and/or " +
                $"{tcfg}; this can cause issues with its attacks and save data. " +
                $"{correctSetter}");
		}

        // Config objects CANNOT be shared by reference between any two ToolCrests or ConfigGroups
		string sharedCfg = "is a direct reference to another crest's config. This can " +
			$"cause issues with both crests' attacks and save data. {correctSetter}";
		if (
			ToolItemManager.GetAllCrests().Except([moveset.Crest.ToolCrest])
			.Any(x => ReferenceEquals(x.HeroConfig, moveset.Crest.ToolCrest!.HeroConfig))
		) {
			ModHelper.LogError($"{name}: {tcfg} {sharedCfg}");
		}
		if (
			hc.configs.Except([moveset.ConfigGroup!])
			.Any(x => ReferenceEquals(x.Config, moveset.ConfigGroup!.Config))
		) {
			ModHelper.LogError($"{name}: {gcfg} {sharedCfg}");
		}

        // The crest's name and the name in its config MUST be identical
        if (name != moveset.HeroConfig!.name) {
            ModHelper.LogError(
                $"{name}: The crest's .{nameof(CrestData.name)} does not match the " +
                $"name in its {mcfg}. Custom attacks may not work. {correctSetter}");
        }
	}

}
