using UnityEngine;

namespace Needleforge.Data;

/// <summary>
/// Stores references to vanilla sounds and animation libraries for use in cloning.
/// </summary>
public class VanillaReferences
{
#pragma warning disable CS1591 // Missing XML comment
    /// <summary>
    /// Contains the library names for each of the crests.
    /// </summary>
    public static class AnimationLibraryNames
    {

        public static readonly string SHAMAN = "Hornet CrestWeapon Shaman Anim";
        public static readonly string WANDERER = "Hornet CrestWeapon Dagger Anim";
        public static readonly string WITCH = "Hornet CrestWeapon Whip Anim";
        public static readonly string REAPER = "Hornet CrestWeapon Scythe Anim";
        public static readonly string BEAST = "Hornet CrestWeapon Warrior Anim";
        public static readonly string CLOAKLESS = "Hornet Cloakless Anim";
        public static readonly string ARCHITECT = "Hornet CrestWeapon Drill Lance Anim";
        public static readonly string DEFAULT = "Knight";
    }

    private static tk2dSpriteAnimation? HunterLibrary;
    private static tk2dSpriteAnimation? BeastLibrary;
    private static tk2dSpriteAnimation? ReaperLibrary;
    private static tk2dSpriteAnimation? WitchLibrary;
    private static tk2dSpriteAnimation? ShamanLibrary;
    private static tk2dSpriteAnimation? ArchitectLibrary;
    private static tk2dSpriteAnimation? CloaklessLibrary;
    private static tk2dSpriteAnimation? WandererLibrary;

    private static AudioClip? HunterAudio;
    private static AudioClip? BeastAudio;
    private static AudioClip? ReaperAudio;
    private static AudioClip? WitchAudio;
    private static AudioClip? ShamanAudio;
    private static AudioClip? ArchitectAudio;
    private static AudioClip? CloaklessAudio;
    private static AudioClip? WandererAudio;

    internal static void InitialiseReferences()
    {
        HunterLibrary = InitLibraryForCrestType(AnimationLibraryNames.DEFAULT);
        BeastLibrary = InitLibraryForCrestType(AnimationLibraryNames.BEAST);
        ReaperLibrary = InitLibraryForCrestType(AnimationLibraryNames.REAPER);
        WitchLibrary = InitLibraryForCrestType(AnimationLibraryNames.WITCH);
        ShamanLibrary = InitLibraryForCrestType(AnimationLibraryNames.SHAMAN);
        ArchitectLibrary = InitLibraryForCrestType(AnimationLibraryNames.ARCHITECT);
        CloaklessLibrary = InitLibraryForCrestType(AnimationLibraryNames.CLOAKLESS);
        WandererLibrary = InitLibraryForCrestType(AnimationLibraryNames.WANDERER);
    }

    /// <summary>
    /// Fetches the animation library reference for a given crest type.
    /// </summary>
    /// <param name="crestType"></param>
    /// <returns></returns>
    public static tk2dSpriteAnimation? GetLibraryForCrestType(VanillaAttackType? crestType)
    {
        return crestType switch
        {
            VanillaAttackType.HUNTER => HunterLibrary,
            VanillaAttackType.BEAST => BeastLibrary,
            VanillaAttackType.REAPER => ReaperLibrary,
            VanillaAttackType.WITCH => WitchLibrary,
            VanillaAttackType.SHAMAN => ShamanLibrary,
            VanillaAttackType.ARCHITECT => ArchitectLibrary,
            VanillaAttackType.CLOAKLESS => CloaklessLibrary,
            VanillaAttackType.WANDERER => WandererLibrary,
            _ => null,
        };
    }

    private static tk2dSpriteAnimation? InitLibraryForCrestType(string name)
    {
        tk2dSpriteAnimation? library = null;

        //Checking through crest configs to find libraries
        foreach (HeroController.ConfigGroup configGroup in HeroController.instance.configs)
        {
            HeroControllerConfig config = configGroup.Config;
            if (config == null) { continue; }

            if (config.heroAnimOverrideLib == null)
            {
                if (name == "Knight")
                { //in case of hunter
                    library = HeroController.instance.GetComponent<tk2dSpriteAnimator>().library; //setting library to default manually
                    return library;
                }
                continue;
            }

            if (config.heroAnimOverrideLib.name == name)
            {
                library = config.heroAnimOverrideLib;
            }
        }

        if (library == null)
        {
            ModHelper.LogWarning($"Failed to find animation library for {name} crest. Attacks cloned from this crest will not function.");
        }

        return library;
    }

    public static AudioClip? GetAudioClipForCrestType(VanillaAttackType? crestType)
    {
        GameObject hornet = HeroController.instance.gameObject;
        Transform attacks = hornet.transform.Find("Attacks");

        AudioClip? clip = null;

        switch (crestType)
        {
            case VanillaAttackType.HUNTER:
                clip = GetClipFromRoot(attacks.Find("Default")); break;
            case VanillaAttackType.BEAST:
                clip = GetClipFromRoot(attacks.Find("Warrior")); break;
            case VanillaAttackType.ARCHITECT:
                clip = GetClipFromRoot(attacks.Find("Toolmaster")); break;
            case VanillaAttackType.WITCH:
                clip = GetClipFromRoot(attacks.Find("Witch")); break;
            case VanillaAttackType.REAPER:
                clip = GetClipFromRoot(attacks.Find("Scythe")); break;
            case VanillaAttackType.CLOAKLESS:
                clip = GetClipFromRoot(attacks.Find("Cloakless")); break;
            case VanillaAttackType.WANDERER:
                clip = GetClipFromRoot(attacks.Find("Wanderer")); break;
            case VanillaAttackType.SHAMAN:
                clip = GetClipFromRoot(attacks.Find("Shaman")); break;
        }
        
        if (clip == null)
        {
            ModHelper.LogWarning($"AudioClip for vanilla crest type {crestType} not found, returning null.");
        }

        return clip;
    }

    private static AudioClip? GetClipFromRoot(Transform? root)
    {
        if (root == null) { return null; }

        Transform slash = root.Find("Slash");
        if (slash == null)
        {
            //Maybe cloakless?
            slash = root.Find("Kick");
        }
        
        if (slash == null)
        {
            //Still nothing, so no audio to find.
            return null;
        }

        AudioSource source = slash.gameObject.GetComponent<AudioSource>();
        if (source == null) { return null; }

        return source.clip;
    }
}

