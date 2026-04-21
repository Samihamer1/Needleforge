using BepInEx;
using HutongGames.PlayMaker;
using Needleforge;
using Needleforge.Attacks;
using Needleforge.Data;
using System;
using System.Linq;
using System.Resources;
using System.Text;
using UnityEngine;

namespace AmalgamCrest;

[BepInAutoPlugin(id: "io.github.amalgamcrest")]
public partial class AmalgamCrestPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
        Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.Full);


        var amalgamCrest = NeedleforgePlugin.AddCrest("AmalgamCrest");

        amalgamCrest.HudFrame.Preset = VanillaCrest.SHAMAN;

        //Only moveset is implemented in this example.

        #region Moveset

        // The HeroConfig controls Hornet's behaviour when attacking with this crest;
        // this includes things like her animations, attack speeds, and several properties
        // of how down, dash, and charged attacks behave.
        var cfg = ScriptableObject.CreateInstance<HeroConfigNeedleforge>();
        amalgamCrest.Moveset.HeroConfig = cfg;

        cfg.canBind = true;
        cfg.SetCanUseAbilities(true);
        cfg.SetAttackFields(
            time: 0.35f, recovery: 0.15f, cooldown: 0.41f,
            quickSpeedMult: 1.5f, quickCooldown: 0.205f
        );
        cfg.wallSlashSlowdown = true;

        cfg.SetDashStabFields(time: 0.2f, speed: -20, bounceJumpSpeed: 10);
        cfg.SetChargedSlashFields(doesKickoff: true, chain: 3);

        cfg.chargeSlashLungeDeceleration = 1;
        cfg.chargeSlashLungeSpeed = 0;



        //We'll pull straight from the VanillaAttacks class for these.
        //If you'd like, you can modify your own copy of them - I'll make
        //my Reaper slash deal more damage like the lord intended.
        Attack ReaperSlash = VanillaAttacks.Reaper.Slash();
        ReaperSlash.DamageMult = 1.5f;

        amalgamCrest.Moveset.Slash = ReaperSlash;

        //But you don't have to modify them.
        amalgamCrest.Moveset.AltSlash = VanillaAttacks.Architect.AltSlash();


        //Ensure you have the correct animation added to your library for these next two.
        //Hunter doesn't have a default for UpSlashAlt.
        //Also it acts kind of weird, so it's best to just manually define both animations
        //if you're using AltUpSlash.
        amalgamCrest.Moveset.UpSlash = VanillaAttacks.Witch.UpSlash();
        amalgamCrest.Moveset.AltUpSlash = VanillaAttacks.Beast.UpSlashRage();


        amalgamCrest.Moveset.WallSlash = VanillaAttacks.Shaman.WallSlash();

        //If using a Slash type downslash, you will likely need a Slash type animation.
        //Wanderer, for example.
        //The following 2 lines show an example of that.
        //cfg.downSlashType = HeroControllerConfig.DownSlashTypes.Slash;
        //amalgamCrest.Moveset.DownSlash = VanillaAttacks.Wanderer.DownSlash();


        //There's no need to set the amalgamCrest.Moveset.DownSlash property since we're using a vanilla downslash.
        //Same for amalgamCrest.Moveset.ChargedSlash, and amalgamCrest.Moveset.DashSlash.


        //Keep this one in mind. If you're going to use Wanderer's dash slash, there's an extra object.
        //I'll elaborate shortly.
        amalgamCrest.Moveset.UseVanillaDashSlash = VanillaAttackType.WITCH;

        amalgamCrest.Moveset.UseVanillaChargedSlash = VanillaAttackType.WANDERER;

        amalgamCrest.Moveset.UseVanillaDownSlash = VanillaAttackType.SHAMAN;

        //By the way, I'm not doing it here, but if you wanted to clone hunter's or cloakless' downslash,
        //You can just use UseVanillaDownSlash. Set the downspike config like in ExampleCrest, 
        //and it should be good.

        // For this, we're skipping the usual process of creating animations since we're 
        // only using vanilla slashes. 
        // Instead, I'll be modifying certain properties of the attacks to
        // demonstrate how to customise them.
        // Of course, you don't have to customise them if you don't want to.
        amalgamCrest.Moveset.OnInitialized += () =>
        {
            //Even if not creating new animations, we still provide an empty library.
            //This is just good practise. Do it.
            if (GameObject.Find("Amalgam LibraryObject") is GameObject libobj)
                return;

            libobj = new GameObject("Amalgam LibraryObject");
            GameObject.DontDestroyOnLoad(libobj);

            tk2dSpriteAnimation library = libobj.AddComponent<tk2dSpriteAnimation>();
            library.clips = Array.Empty<tk2dSpriteAnimationClip>();
            amalgamCrest.Moveset.HeroConfig.heroAnimOverrideLib = library;

            //We are adding an animation, though. For the alt upslash.
            AddTestAnimationsToLibrary(library, HeroController.instance);

            //Making Shaman's downslash move a little faster, and it a little smaller.
            GameObject? downSlash = amalgamCrest.Moveset.ConfigGroup?.DownSlashObject;
            if (downSlash != null)
            {
                NailSlash slash = downSlash.GetComponent<NailSlash>();
                slash.scale = new Vector2(0.75f, 0.75f);

                NailSlashTravel slashTravel = downSlash.GetComponent<NailSlashTravel>();
                slashTravel.travelDuration *= 0.75f;
                slashTravel.travelDistance *= 1.2f;
            }


            //Modifying dash slash to heal you on hit, for some reason.
            //I'll make it smaller to balance it a little.
            GameObject dashSlashParent = amalgamCrest.Moveset.ConfigGroup!.DashStab;

            //Since this is Witch, we find the child called "Dash Slash 2". In other cases,
            //you can just use the object directly.
            GameObject? dashSlash2 = dashSlashParent.transform.Find("Dash Slash 2").gameObject;
            if (dashSlash2 != null)
            {
                dashSlash2.transform.localScale -= new Vector3(0.4f, 0.2f, 0);
                dashSlash2.GetComponent<DamageEnemies>().DamagedEnemy += HealForOne;
            }

            void HealForOne()
            {
                HeroController.instance.AddHealth(1);
            }

            //As promised, if you're using Wanderer, ensure you remember the format is different.
            //Wanderer's dash attack and dash attack alt are stored as you'd expect,
            //but you can access the third "Recoil Slash", the diving attack after hitting an enemy.
            ConfigGroupNeedleforge convertedConfig = (ConfigGroupNeedleforge) amalgamCrest.Moveset.ConfigGroup!;
            GameObject? RecoilSlash = convertedConfig.SpecialSlash;
            //I'm not using it, but this is how you'd do it.


            //Modifying charged slash scale and hit time.
            GameObject? chargeSlash = amalgamCrest.Moveset.ConfigGroup?.ChargeSlash;
            if (chargeSlash != null)
            {
                chargeSlash.transform.localScale = new Vector3(1.2f, 1.8f, 1);
                chargeSlash.transform.localPosition += new Vector3(-0.5f, 0.5f, 0);
                chargeSlash.GetComponent<DamageEnemies>().stepsPerHit /= 2;
                chargeSlash.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.25f;
            }

            //For dev purposes, basically autocreates the VanillaAttacks classes.
            //You really thought I'd manually type all that out. Fool.
            //LogAllAttackInfo();
        };

        #endregion
    }

    private void LogAllAttackInfo()
    {
        GameObject hornet = HeroController.instance.gameObject;
        GameObject attacks = hornet.transform.Find("Attacks").gameObject;
        Logger.LogInfo("##########################################################");
        Logger.LogInfo("##########################################################");

        string[] RootNames = ["Default", "Scythe", "Wanderer", "Shaman", "Witch", "Toolmaster", "Warrior", "Cloakless"];
        string[] BannedAttacks = ["DashUpper Slash", "DownSlash New", "DashStab", "DashStabAlt", "RecoilStab",
        "DashSlash", "Dash Stab 1", "Dash Stab 2", "RevengeSlash", "Slash Prev", "DashStab", "DownSlashNew Charged",
        "DashStab Charged", "SpinSlash", "SpinSlashRage", "DashSlashRage", "Dash Stab"];

        foreach (string name in RootNames)
        {
            GameObject rootObject = attacks.transform.Find(name).gameObject;

            Logger.LogInfo($"{name}");

            string fullmessage = "";

            string usevanillaanimlibrary = "";

            switch (name)
            {
                case "Default":
                    usevanillaanimlibrary = "VanillaAttackType.HUNTER"; break;
                case "Scythe":
                    usevanillaanimlibrary = "VanillaAttackType.REAPER"; break;
                case "Wanderer":
                    usevanillaanimlibrary = "VanillaAttackType.WANDERER"; break;
                case "Shaman":
                    usevanillaanimlibrary = "VanillaAttackType.SHAMAN"; break;
                case "Toolmaster":
                    usevanillaanimlibrary = "VanillaAttackType.ARCHITECT"; break;
                case "Warrior":
                    usevanillaanimlibrary = "VanillaAttackType.BEAST"; break;
                case "Cloakless":
                    usevanillaanimlibrary = "VanillaAttackType.CLOAKLESS"; break;
                case "Witch":
                    usevanillaanimlibrary = "VanillaAttackType.WITCH"; break;
            }

            foreach (NailSlash slash in rootObject.GetComponentsInChildren<NailSlash>(true))
            {
                if (BannedAttacks.Contains(slash.gameObject.name))
                {
                    continue;
                }

                DamageEnemies dmg = slash.gameObject.GetComponent<DamageEnemies>();
                PolygonCollider2D hitbox = slash.gameObject.GetComponent<PolygonCollider2D>();
                NailSlashTravel travel = slash.gameObject.GetComponent<NailSlashTravel>();
                GameObject tinker = slash.gameObject.transform.Find("Clash Tink").gameObject;
                PolygonCollider2D tinkerhitbox = tinker.GetComponent<PolygonCollider2D>();

                string multihitmult = string.Join(", ", dmg.damageMultPerHit + "f");
                if (multihitmult == "System.Single[]f")
                {
                    multihitmult = "[]";
                }

                string position = "new Vector3 (" + $"{slash.transform.localPosition.x}f," +
                    $"{slash.transform.localPosition.y}f, {slash.transform.localPosition.z}f" +
                    ")";

                string rotation = "new Quaternion (" + $"{slash.transform.localRotation.x}f," +
                    $"{slash.transform.localRotation.y}f, {slash.transform.localRotation.z}f," +
                    $"{slash.transform.localRotation.w}f" +
                    ")";

                string scale = "new Vector3 (" + $"{slash.scale.x}f, {slash.scale.y}f, {slash.scale.z}f)";
                if (slash.gameObject.name == "WallSlash" || slash.gameObject.name == "WallSlashRage")
                {
                    //Shoot me and I couldn't tell you why Wall Slashes need their X reversed.
                    scale = "new Vector3 (" + $"{-slash.scale.x}f, {slash.scale.y}f, {slash.scale.z}f)";
                }

                string obj = $"" +
                    $"public static Attack {slash.gameObject.name.Replace(" ", "")}()\n" +
                    "{\n" +
                    "   return new Attack()\n" +
                    "   {\n" +
                    $"  AnimName = \"{slash.animName}\",\n" +
                    $"  DamageMult = {dmg.damageMultiplier}f,\n" +
                    $"  FramesBetweenMultiHits = {dmg.stepsPerHit},\n" +
                    $"  Hitbox = [{string.Join(", ", hitbox.points.Select(p => $"new({p.x}f, {p.y}f)"))}],\n" +
                    $"  KnockbackMult = {dmg.magnitudeMult}f,\n" +
                    $"  MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.{dmg.multiHitEffects},\n" +
                    $"  MultiHitMultipliers = {multihitmult},\n" +
                    $"  Scale = {scale},\n" +
                    $"  SilkGeneration = HitSilkGeneration.{dmg.silkGeneration},\n" +
                    $"  StunDamage = {dmg.stunDamage}f,\n" +
                    $"  Name = \"{slash.gameObject.name} Clone\",\n" +
                    $"  Position = {position},\n" +
                    $"  Rotation = {rotation},\n" +
                    $"  UseVanillaAnimLibrary = {usevanillaanimlibrary},\n" +
                    $"  UseVanillaSound = {usevanillaanimlibrary},\n" +
                    $"  TinkerHitbox = [{string.Join(", ", tinkerhitbox.points.Select(p => $"new({p.x}f, {p.y}f)"))}],\n" +
                    $"";

                if (travel != null)
                {
                    string travelDistance = "new Vector2(" +
                        $"{travel.travelDistance.x}f, {travel.travelDistance.y}f" +
                        ")"; 

                    obj += $"" +
                        $"  TravelCurve = {RebuildAnimationCurve(travel.travelCurve)},\n" +
                        $"  TravelDistance = {travelDistance},\n" +
                        $"  TravelDuration = {travel.travelDuration}f,\n" +
                        $"  TravelRecoilTime = {travel.recoilDistance}f,\n" +
                        $"  TravelGroundedYOffset = {travel.groundedYOffset}f\n";
                }

                obj += "};\n}\n\n";


                fullmessage += obj;            }

            Logger.LogInfo(fullmessage);

        }

        Logger.LogInfo("##########################################################");
        Logger.LogInfo("##########################################################");
    }

    public static string RebuildAnimationCurve(AnimationCurve curve)
    {
        var str = new StringBuilder();

        str.Append("new AnimationCurve(\n");

        for (int i = 0; i < curve.keys.Length; i++)
        {
            var k = curve.keys[i];

            str.Append($"    new Keyframe({k.time}f, {k.value}f, {k.inTangent}f, {k.outTangent}f)");

            if (i < curve.keys.Length - 1)
                str.Append(",\n");
        }

        str.Append("\n  )");

        return str.ToString();
    }

    private static void AddTestAnimationsToLibrary(tk2dSpriteAnimation animLibrary, HeroController hc)
    {
        //Stealing Wanderer's upslash anim for the alt.
        var wanderer = hc.configs.First(c => c.Config.name == "Wanderer").Config.heroAnimOverrideLib;

        tk2dSpriteAnimationClip upslashalt = new tk2dSpriteAnimationClip();
        upslashalt.CopyFrom(wanderer.GetClipByName("UpSlash"));
        upslashalt.name = "UpSlashAlt";

        animLibrary.clips =
        [
            wanderer.GetClipByName("UpSlash"),
            upslashalt,
            //wanderer.GetClipByName("DownSlash"),
        ];
        animLibrary.isValid = false;
        animLibrary.ValidateLookup();
    }

    /// <summary>
    /// Creates an array of new frame objects with the same sprites as the original frames.
    /// </summary>
    private static tk2dSpriteAnimationFrame[] CloneFrames(tk2dSpriteAnimationFrame[] frames, int? count = null)
    {
        count ??= frames.Length;
        return
        [
            ..
            frames
                .Select(f =>
                    new tk2dSpriteAnimationFrame()
                    {
                        spriteCollection = f.spriteCollection,
                        spriteId = f.spriteId,
                        triggerEvent = false
                    }
                )
                .Take((int)count)
        ];
    }
}