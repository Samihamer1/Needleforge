using Needleforge.Attacks;
using UnityEngine;

namespace Needleforge.Data;

/// <summary>
/// Holds a recreation of each of the base game's attacks.
/// Each can be used in place of a regular attack on a custom crest.
/// </summary>
public static class VanillaAttacks
{
    /// <summary>
    /// Due to the large number of grouped attacks (Wanderer's dash slashes, Architect's down slashes, etc),
    /// this class is used to store related groups of objects together for ease of use.
    /// </summary>
    internal class VanillaAttackObjects
    {
        /// <summary>
        /// The first GameObject usually representing the base variant. The first dash slash, uncharged slash, etc.
        /// </summary>
        public GameObject Attack;
        /// <summary>
        /// The second GameObject, usually representing the alternate variant. The second dash slash, charged slash, etc. Optional.
        /// </summary>
        public GameObject? AttackAlt;
        /// <summary>
        /// The very rare case of a third GameObject, used for Wanderer's recoil slash. Optional.
        /// </summary>
        public GameObject? AttackSpecial;

        /// <summary>
        /// Create an instance of the class with given GameObjects.
        /// </summary>
        /// <param name="attack">Base variant of attack object</param>
        /// <param name="attackAlt">Alternate or charged variant of attack object, optional</param>
        /// <param name="attackSpecial">Special variant of attack object, optional</param>
        public VanillaAttackObjects(GameObject attack, GameObject? attackAlt = null, GameObject? attackSpecial = null)
        {
            Attack = attack;
            AttackAlt = attackAlt;
            AttackSpecial = attackSpecial;
        }
    }

    /// <summary>
    /// Stores the function necessary to obtain the vanilla charged slashes.
    /// </summary>
    internal static class ChargedSlashes
    {
        /// <summary>
        /// Obtains the original charged slash GameObject for a given crest.
        /// </summary>
        /// <param name="crest">Crest type</param>
        internal static GameObject? GetChargedSlashForCrest(VanillaAttackType? crest)
        {
            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.transform.Find("Attacks").gameObject;

            switch (crest)
            {
                case VanillaAttackType.HUNTER:
                    return attacks.transform.Find("Charge Slash Basic").gameObject;
                case VanillaAttackType.BEAST:
                    return attacks.transform.Find("Charge Slash Warrior").gameObject;
                case VanillaAttackType.REAPER:
                    return attacks.transform.Find("Charge Slash Scythe").gameObject;
                case VanillaAttackType.WITCH:
                    return attacks.transform.Find("Charge Slash Witch").gameObject;
                case VanillaAttackType.SHAMAN:
                    return attacks.transform.Find("Charge Slash Shaman").gameObject;
                case VanillaAttackType.ARCHITECT:
                    return attacks.transform.Find("Charge Slash Toolmaster").gameObject;
                case VanillaAttackType.WANDERER:
                    return attacks.transform.Find("Charge Slash Wanderer").gameObject;
            }

            ModHelper.LogError($"Could not find a charged slash for crest type {crest}. Returning null.");
            return null;
        }
    }

    /// <summary>
    /// Stores the function necessary to obtain the vanilla custom down slashes.
    /// <para>Reaper, Beast, Shaman, Architect, Witch.</para>
    /// <para>Hunter, Cloakless, and Wanderer should be accessed using
    /// their own crest class, such as <see cref="VanillaAttacks.Hunter"/>.</para>
    /// </summary>
    internal static class CustomDownSlashes
    {
        /// <summary>
        /// Returns the downslash gameobject(s) for a given crest, if any. 
        /// <para>Beast, Reaper, and Shaman all have one object as Attack.</para>
        /// <para>Architect has two, Attack being the uncharged variant, and AttackAlt being the charged variant.</para>
        /// <para>The rest have none, and should be accessed using the Crest classes.</para>
        /// </summary>
        /// <param name="crest"></param>
        /// <returns>VanillaAttackObjects containing the slashes. Null if no valid downslashes are found.</returns>
        internal static VanillaAttackObjects? GetDownSlashForCrest(VanillaAttackType? crest)
        {
            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.transform.Find("Attacks").gameObject;

            switch (crest)
            {
                case VanillaAttackType.BEAST:
                    return new VanillaAttackObjects(attacks.transform.Find("Warrior").Find("SpinSlash").gameObject);
                case VanillaAttackType.REAPER:
                    return new VanillaAttackObjects(attacks.transform.Find("Scythe").Find("DownSlash New").gameObject);
                case VanillaAttackType.WITCH:
                    return new VanillaAttackObjects(attacks.transform.Find("Witch").Find("DownSlash New").gameObject);
                case VanillaAttackType.SHAMAN:
                    return new VanillaAttackObjects(attacks.transform.Find("Shaman").Find("DownSlash").gameObject);
                case VanillaAttackType.ARCHITECT:
                    return new VanillaAttackObjects(attacks.transform.Find("Toolmaster").Find("DownSlash New").gameObject,
                        attacks.transform.Find("Toolmaster").Find("DownSlash New Charged").gameObject);
                case VanillaAttackType.HUNTER:
                    return new VanillaAttackObjects(attacks.transform.Find("Default").Find("DownSlash").gameObject);
                case VanillaAttackType.CLOAKLESS:
                    return new VanillaAttackObjects(attacks.transform.Find("Cloakless").Find("DownKick").gameObject);
            }

            ModHelper.LogError($"Could not find a down slash for crest type {crest}. Returning null.");
            return null;
        }
    }

    /// <summary>
    /// Stores the function necessary to obtain the vanilla dash slashes.
    /// </summary>
    internal static class DashSlashes
    {
        /// <summary>
        /// Returns the dash slash gameobject(s) for a given crest, if any. 
        /// </summary>
        /// <param name="crest"></param>
        /// <returns>VanillaAttackObjects containing the slashes. Null if no valid dash slashes are found.</returns>
        internal static VanillaAttackObjects? GetDashSlashForCrest(VanillaAttackType? crest)
        {
            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.transform.Find("Attacks").gameObject;

            switch (crest)
            {
                case VanillaAttackType.BEAST:
                    return new VanillaAttackObjects(attacks.transform.Find("Warrior").Find("DashSlash").gameObject);
                case VanillaAttackType.REAPER:
                    return new VanillaAttackObjects(attacks.transform.Find("Scythe").Find("DashUpper Slash").gameObject);
                case VanillaAttackType.WITCH:
                    return new VanillaAttackObjects(
                        attacks.transform.Find("Witch").Find("Dash Stab Parent").Find("Dash Stab 1").gameObject,
                        attacks.transform.Find("Witch").Find("Dash Stab Parent").Find("Dash Stab 2").gameObject);
                case VanillaAttackType.SHAMAN:
                    return new VanillaAttackObjects(attacks.transform.Find("Shaman").Find("DashSlash").gameObject);
                case VanillaAttackType.ARCHITECT:
                    return new VanillaAttackObjects(
                        attacks.transform.Find("Toolmaster").Find("Dash Stab").gameObject,
                        attacks.transform.Find("Toolmaster").Find("Dash Stab Charged").gameObject);
                case VanillaAttackType.WANDERER:
                    return new VanillaAttackObjects(
                        attacks.transform.Find("Wanderer").Find("DashStab").gameObject,
                        attacks.transform.Find("Wanderer").Find("DashStabAlt").gameObject,
                        attacks.transform.Find("Wanderer").Find("RecoilStab").gameObject);
                case VanillaAttackType.CLOAKLESS:
                    return new VanillaAttackObjects(attacks.transform.Find("Cloakless").Find("Dash Kick").gameObject);
                case VanillaAttackType.HUNTER:
                    return new VanillaAttackObjects(attacks.transform.Find("HUNTER").Find("Dash Stab").gameObject);
            }

            ModHelper.LogError($"Could not find a dash slash for crest type {crest}. Returning null.");
            return null;
        }
    }

    #pragma warning disable CS1591 // Missing XML comment
    //All of the following classs are generated using the LogAttackInfo method in the AmalgamCrest
    public static class Hunter
    {
        //Guideline to help with pasting
        public static Attack Slash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-1.378911f, 1.109176f), new(-2.639706f, 0.5901985f), new(-3.444635f, -0.163908f), new(-3.405565f, -0.8376141f), new(-2.656851f, -1.285259f), new(-0.8339268f, -1.429016f), new(0.5491926f, -1.069193f), new(0.2596796f, -0.6293708f), new(0.1276605f, 0.7318581f), new(0.2152422f, 1.231026f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.9f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "Slash Clone",
                Position = new Vector3(-0.07f, 0f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.HUNTER,
                UseVanillaSound = VanillaAttackType.HUNTER,
                TinkerHitbox = [new(-2.130932f, 0.522934f), new(-3.046188f, -0.1623912f), new(-2.95216f, -0.6413436f), new(-2.307578f, -1.007505f), new(-1.031897f, -1.13673f), new(0.213729f, -0.8879466f), new(-0.3836526f, -0.001206398f), new(0.1127285f, 1.005616f)],
            };
        }

        public static Attack AltSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-3.371631f, 0.01020908f), new(-3.411086f, -0.6537523f), new(-2.7706f, -1.158113f), new(-1.647945f, -1.356064f), new(0.1401426f, -1.011089f), new(0.2826195f, 0.8337255f), new(-0.7151222f, 0.9295011f), new(-2.153702f, 0.6737847f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.9f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "AltSlash Clone",
                Position = new Vector3(-0.0969982f, 0.027f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.HUNTER,
                UseVanillaSound = VanillaAttackType.HUNTER,
                TinkerHitbox = [new(-2.888845f, 0.00539732f), new(-3.014641f, -0.4923835f), new(-2.652601f, -0.9180737f), new(-1.627664f, -1.087656f), new(-0.3672324f, -1.197071f), new(0.2313614f, 0.6533995f), new(-0.7279375f, 0.6470208f), new(-1.832339f, 0.4197383f)],
            };
        }

        public static Attack UpSlash()
        {
            return new Attack()
            {
                AnimName = "UpSlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(0.09296799f, 3.331319f), new(0.9792442f, 2.844984f), new(1.516476f, 1.229815f), new(1.489143f, -0.8975886f), new(1.070105f, -1.253966f), new(-0.4414425f, -1.230724f), new(-0.8173714f, -0.8982668f), new(-1.023376f, 1.283778f), new(-0.7154503f, 2.797052f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 0.9f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "UpSlash Clone",
                Position = new Vector3(-0.04f, 0.19f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.HUNTER,
                UseVanillaSound = VanillaAttackType.HUNTER,
                TinkerHitbox = [new(0.06896973f, 2.922494f), new(0.6928024f, 2.501007f), new(1.153149f, 1.138981f), new(1.266117f, -0.7053773f), new(0.2977448f, -0.3816398f), new(-0.7943077f, -0.8213822f), new(-0.8088722f, 1.23836f), new(-0.5050583f, 2.476242f)],
            };
        }

        public static Attack WallSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-1.412506f, 1.143569f), new(-2.568985f, 0.6524787f), new(-3.358936f, -0.1393261f), new(-3.342262f, -0.8358898f), new(-2.55138f, -1.320719f), new(-1.153069f, -1.423538f), new(0.5654984f, -1.069193f), new(0.2596796f, -0.6293708f), new(0.1276605f, 0.7318581f), new(0.2152422f, 1.201671f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.9f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "WallSlash Clone",
                Position = new Vector3(0.641f, 0.008f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.HUNTER,
                UseVanillaSound = VanillaAttackType.HUNTER,
                TinkerHitbox = [new(-0.6438503f, 1.02602f), new(-1.964935f, 0.6437778f), new(-3.054035f, -0.1911302f), new(-2.985264f, -0.7822967f), new(-2.0403f, -1.148502f), new(-0.1128731f, -1.190293f), new(0.5824738f, 1.167442f)],
            };
        }

        //End class
    }
    public static class Reaper
    {
        //Guideline to help with pasting
        public static Attack Slash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(0.6541473f, 2.173036f), new(-0.6820984f, 1.961986f), new(-2.132395f, 1.303935f), new(-3.125545f, 0.07393456f), new(-3.195837f, -0.9318354f), new(0.4337189f, -0.7477584f)],
                KnockbackMult = 1.5f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "Slash Clone",
                Position = new Vector3(-0.4f, 0f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.REAPER,
                UseVanillaSound = VanillaAttackType.REAPER,
                TinkerHitbox = [new(0.9851572f, 2.186862f), new(-0.6780505f, 1.614961f), new(-1.97003f, 1.232684f), new(-2.985113f, 0.1482587f), new(-3.40934f, -0.723033f), new(-3.34239f, -1.496542f), new(0.8762078f, -0.7304724f)],
            };
        }

        public static Attack AltSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(0.397731f, 0.297164f), new(-0.5144043f, 0.4312067f), new(-2.869443f, 0.3877187f), new(-2.770142f, -0.2692027f), new(-1.832306f, -1.13419f), new(-0.4850693f, -1.573602f), new(0.6941521f, -1.58861f)],
                KnockbackMult = 1.5f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "AltSlash Clone",
                Position = new Vector3(-0.8f, 0.08f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.REAPER,
                UseVanillaSound = VanillaAttackType.REAPER,
                TinkerHitbox = [new(-0.903614f, 0.9950113f), new(-3.356669f, 1.410387f), new(-3.191143f, 0.4145628f), new(-1.960935f, -0.904336f), new(-1.29941f, -1.268595f), new(-0.4741147f, -1.437723f), new(0.9485426f, -1.399847f)],
            };
        }

        public static Attack UpSlash()
        {
            return new Attack()
            {
                AnimName = "UpSlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(1.438645f, 2.139698f), new(-0.623355f, 1.932651f), new(-1.899132f, 1.317874f), new(-2.988118f, 0.2540811f), new(-3.341559f, -0.7908249f), new(-2.163501f, -0.4711302f), new(-0.3439602f, -0.2433928f)],
                KnockbackMult = 1.25f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.92f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "UpSlash Clone",
                Position = new Vector3(-0.02f, 0.18f, -0.001f),
                Rotation = new Quaternion(0f, 0f, -0.7071068f, 0.7071068f),
                UseVanillaAnimLibrary = VanillaAttackType.REAPER,
                UseVanillaSound = VanillaAttackType.REAPER,
                TinkerHitbox = [new(1.346201f, 1.971633f), new(-0.4912918f, 1.764583f), new(-1.727451f, 1.149806f), new(-2.781957f, 0.2623505f), new(-3.502288f, -1.114474f), new(-3.191685f, -1.392598f), new(-2.409755f, -1.108642f), new(-0.2836727f, -0.6804808f)],
            };
        }

        public static Attack WallSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(0.4563503f, 2.218599f), new(-1.336772f, 1.70155f), new(-2.6597f, 0.6955719f), new(-3.321252f, -0.4743013f), new(-3.332825f, -0.8306394f), new(-0.1660881f, -0.4113045f)],
                KnockbackMult = 1.5f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "WallSlash Clone",
                Position = new Vector3(0.61f, 0f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.REAPER,
                UseVanillaSound = VanillaAttackType.REAPER,
                TinkerHitbox = [new(-0.2201261f, 1.678195f), new(-2.687212f, 1.560575f), new(-3.741085f, 0.5408022f), new(-4.274223f, -1.316184f), new(-0.2473028f, -1.242212f)],
            };
        }

        //End class
    }
    public static class Wanderer
    {
        //Guideline to help with pasting
        public static Attack Slash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-2.275631f, 0.3745367f), new(-3.6f, 0.08807433f), new(-3.859055f, -0.1423087f), new(-3.510472f, -0.401807f), new(-2.074084f, -0.6115804f), new(-0.1987635f, -0.7573026f), new(-0.2670065f, 0.504642f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.8f, 1.4f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 0.8f,
                Name = "Slash Clone",
                Position = new Vector3(-0.03399625f, -0.01600372f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.WANDERER,
                UseVanillaSound = VanillaAttackType.WANDERER,
                TinkerHitbox = [new(-3.342288f, 0.1710601f), new(-3.795103f, -0.1181604f), new(-3.037928f, -0.5379933f), new(-0.2388175f, -0.749114f), new(-0.2007659f, 0.5236073f)],
            };
        }

        public static Attack AltSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-3.266424f, -0.09223898f), new(-3.768245f, -0.3786748f), new(-3.312578f, -0.6775489f), new(-0.08767902f, -1.120088f), new(-0.1519974f, 0.1954108f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.8f, 1.35f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 0.8f,
                Name = "AltSlash Clone",
                Position = new Vector3(-0.12f, 0.364f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.WANDERER,
                UseVanillaSound = VanillaAttackType.WANDERER,
                TinkerHitbox = [new(-2.872244f, -0.09700747f), new(-3.641178f, -0.3853887f), new(-2.842875f, -0.7812645f), new(-0.1057623f, -1.061968f), new(-0.2940146f, 0.1868369f)],
            };
        }

        public static DownAttack DownSlash()
        {
            return new DownAttack()
            {
                AnimName = "DownSlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(2.187407f, -0.2369099f), new(2.100544f, -1.490677f), new(1.269346f, -2.209919f), new(0.1460766f, -2.532595f), new(-1.180697f, -2.260349f), new(-1.816813f, -1.591433f), new(-1.840732f, -0.2457314f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.8f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 0.8f,
                Name = "DownSlash Clone",
                Position = new Vector3(-0.008f, -0.143f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.WANDERER,
                UseVanillaSound = VanillaAttackType.WANDERER,
                TinkerHitbox = [new(2.303401f, 0.08898327f), new(1.921036f, -1.552602f), new(1.249119f, -2.034353f), new(0.1552461f, -2.444582f), new(-1.210093f, -2.071619f), new(-1.749258f, -1.430419f), new(-1.824284f, 0.06242577f)],
            };
        }

        public static Attack DownSlashAlt()
        {
            return new Attack()
            {
                AnimName = "DownSlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(2.204742f, -0.2404288f), new(2.022374f, -1.559938f), new(1.199787f, -2.209163f), new(0.1460813f, -2.473921f), new(-1.065808f, -2.351448f), new(-1.749258f, -1.445091f), new(-1.907567f, -0.2669863f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(-0.8f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 0.8f,
                Name = "DownSlashAlt Clone",
                Position = new Vector3(0.05f, -0.128f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.WANDERER,
                UseVanillaSound = VanillaAttackType.WANDERER,
                TinkerHitbox = [new(2.303401f, 0.08898327f), new(1.921036f, -1.552602f), new(1.249119f, -2.034353f), new(0.1552461f, -2.444582f), new(-1.210093f, -2.071619f), new(-1.749258f, -1.430419f), new(-1.824284f, 0.06242577f)],
            };
        }

        public static Attack UpSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(0.9777958f, -0.5775061f), new(-1.688496f, -0.6323656f), new(-3.857548f, -0.3156492f), new(-3.831499f, -0.002670288f), new(-1.873296f, 0.3729548f), new(0.9478798f, 0.349494f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.7f, -1.3f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 0.8f,
                Name = "UpSlash Clone",
                Position = new Vector3(-0.03f, 0.181f, -0.001f),
                Rotation = new Quaternion(0f, 0f, -0.7071065f, 0.7071071f),
                UseVanillaAnimLibrary = VanillaAttackType.WANDERER,
                UseVanillaSound = VanillaAttackType.WANDERER,
                TinkerHitbox = [new(1.103719f, -0.577506f), new(-1.727242f, -0.6323656f), new(-3.84014f, -0.2497779f), new(-3.831499f, -0.002670288f), new(-1.863612f, 0.3670569f), new(1.037825f, 0.3385442f)],
            };
        }

        public static Attack WallSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-3.102448f, -0.07606211f), new(-3.815657f, -0.2935088f), new(-3.847082f, -0.4414392f), new(-3.116354f, -0.6829863f), new(-0.315182f, -1.026812f), new(-0.3026201f, 0.1419886f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.8f, 1.35f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 0.8f,
                Name = "WallSlash Clone",
                Position = new Vector3(0.24f, 0.43f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.WANDERER,
                UseVanillaSound = VanillaAttackType.WANDERER,
                TinkerHitbox = [new(-3.161566f, -0.135939f), new(-3.950251f, -0.4037436f), new(-3.086119f, -0.616527f), new(-0.245945f, -0.9549789f), new(-0.2118336f, 0.1002394f)],
            };
        }


        //End class
    }
    public static class Shaman
    {
        //Guideline to help with pasting
        public static Attack Slash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-0.1314468f, 1.386868f), new(-1.619068f, 0.5653152f), new(-1.850586f, -0.4958534f), new(-1.456055f, -1.259823f), new(-0.4487152f, -1.592335f), new(1.218796f, -0.2404938f), new(0.7051315f, 1.373138f)],
                KnockbackMult = 0.5f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, -1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "Slash Clone",
                Position = new Vector3(0f, 0f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.SHAMAN,
                UseVanillaSound = VanillaAttackType.SHAMAN,
                TinkerHitbox = [new(-0.1774882f, 1.106045f), new(-1.266612f, 0.520668f), new(-1.740932f, -0.4801851f), new(-1.40906f, -1.259823f), new(-0.4487152f, -1.592335f), new(0.7134395f, 1.073849f)],
                TravelCurve = new AnimationCurve(
             new Keyframe(0f, 0f, 1.820642f, 1.820642f),
             new Keyframe(1f, 1f, 0.4600298f, 0.4600298f)
           ),
                TravelDistance = new Vector2(-4.4f, 0f),
                TravelDuration = 0.25f,
                TravelRecoilTime = 0.25f,
                TravelGroundedYOffset = 0f
            };
        }

        public static Attack AltSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-0.1314468f, 1.433105f), new(-1.619068f, 0.5504303f), new(-1.910112f, -0.4958534f), new(-1.456055f, -1.259823f), new(-0.4178886f, -1.391968f), new(0.9914742f, -0.4078484f), new(0.7051315f, 1.373138f)],
                KnockbackMult = 0.5f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "AltSlash Clone",
                Position = new Vector3(0f, 0f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.SHAMAN,
                UseVanillaSound = VanillaAttackType.SHAMAN,
                TinkerHitbox = [new(-0.08445378f, 1.370444f), new(-1.344936f, 0.473675f), new(-1.678274f, -0.4645207f), new(-1.456055f, -1.103175f), new(-0.3860578f, -1.091063f), new(0.7051315f, 1.373138f)],
                TravelCurve = new AnimationCurve(
             new Keyframe(0f, 0f, 1.820642f, 1.820642f),
             new Keyframe(1f, 1f, 0.4600298f, 0.4600298f)
           ),
                TravelDistance = new Vector2(-4.4f, 0f),
                TravelDuration = 0.25f,
                TravelRecoilTime = 0.25f,
                TravelGroundedYOffset = 0f
            };
        }

        public static Attack DownSlash()
        {
            return new Attack()
            {
                AnimName = "DownSlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(0.7614839f, 1.739986f), new(-0.5321012f, 1.945559f), new(-2.334569f, 0.6866558f), new(-2.72373f, -1.001996f), new(-1.611478f, -2.230062f), new(-0.4727593f, -2.209587f)],
                KnockbackMult = 0.5f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "DownSlash Clone",
                Position = new Vector3(-0.02999878f, -0.1500015f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0.5f, 0.8660254f),
                UseVanillaAnimLibrary = VanillaAttackType.SHAMAN,
                UseVanillaSound = VanillaAttackType.SHAMAN,
                TinkerHitbox = [new(-0.1314468f, 1.433105f), new(-1.470253f, 0.520668f), new(-1.850586f, -0.4958534f), new(-1.456055f, -1.259823f), new(-0.4487152f, -1.592335f), new(0.7051315f, 1.373138f)],
                TravelCurve = new AnimationCurve(
             new Keyframe(0f, 0f, 1.820642f, 1.820642f),
             new Keyframe(1f, 1f, 0.4600298f, 0.4600298f)
           ),
                TravelDistance = new Vector2(-1.18f, -3.45f),
                TravelDuration = 0.2f,
                TravelRecoilTime = 1f,
                TravelGroundedYOffset = 0f
            };
        }

        public static Attack UpSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-0.1314468f, 1.433105f), new(-1.470253f, 0.520668f), new(-1.850586f, -0.4958534f), new(-1.456055f, -1.259823f), new(-0.4487152f, -1.592335f), new(0.7051315f, 1.373138f)],
                KnockbackMult = 0.5f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "UpSlash Clone",
                Position = new Vector3(0f, 0f, -0.001f),
                Rotation = new Quaternion(0f, 0f, -0.7071068f, 0.7071068f),
                UseVanillaAnimLibrary = VanillaAttackType.SHAMAN,
                UseVanillaSound = VanillaAttackType.SHAMAN,
                TinkerHitbox = [new(-0.1774882f, 1.106045f), new(-1.266612f, 0.520668f), new(-1.740932f, -0.4801851f), new(-1.40906f, -1.259823f), new(-0.4487152f, -1.592335f), new(0.7134395f, 1.073849f)],
                TravelCurve = new AnimationCurve(
             new Keyframe(0f, 0f, 1.820642f, 1.820642f),
             new Keyframe(1f, 1f, 0.4600298f, 0.4600298f)
           ),
                TravelDistance = new Vector2(0f, 3.6f),
                TravelDuration = 0.25f,
                TravelRecoilTime = 0.25f,
                TravelGroundedYOffset = 0f
            };
        }

        public static Attack WallSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-0.1314468f, 1.433105f), new(-1.470253f, 0.520668f), new(-1.850586f, -0.4958534f), new(-1.456055f, -1.259823f), new(-0.4487152f, -1.592335f), new(0.2395477f, 1.318363f)],
                KnockbackMult = 0.5f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "WallSlash Clone",
                Position = new Vector3(0.6100006f, 0f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.SHAMAN,
                UseVanillaSound = VanillaAttackType.SHAMAN,
                TinkerHitbox = [new(-0.1314468f, 1.433105f), new(-1.470253f, 0.520668f), new(-1.850586f, -0.4958534f), new(-1.456055f, -1.259823f), new(-0.4487152f, -1.592335f), new(0.1862162f, 1.373138f)],
                TravelCurve = new AnimationCurve(
             new Keyframe(0f, 0f, 1.820642f, 1.820642f),
             new Keyframe(1f, 1f, 0.4600298f, 0.4600298f)
           ),
                TravelDistance = new Vector2(-4.4f, 0f),
                TravelDuration = 0.25f,
                TravelRecoilTime = 0.25f,
                TravelGroundedYOffset = 0f
            };
        }

        //End class
    }

    public static class Witch
    {
        //Guideline to help with pasting
        public static Attack Slash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-1.864603f, 0.9803432f), new(-3.540159f, 0.5209525f), new(-4.533172f, -0.2059211f), new(-4.457632f, -0.7312268f), new(-3.243822f, -1.051077f), new(-1.841377f, -1.098087f), new(-0.1662136f, -0.8690275f), new(-0.1836531f, 1.018156f)],
                KnockbackMult = 0.4f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.94f, 1.15f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "Slash Clone",
                Position = new Vector3(0.038f, 0.014f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.WITCH,
                UseVanillaSound = VanillaAttackType.WITCH,
                TinkerHitbox = [new(-1.943907f, 0.8451843f), new(-3.112503f, 0.5391998f), new(-4.199873f, -0.07701111f), new(-4.437544f, -0.5174522f), new(-3.784388f, -0.8840179f), new(-1.922428f, -1.027859f), new(-0.0982262f, -0.8083254f), new(-0.1937148f, 0.91648f)],
            };
        }

        public static Attack AltSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-2.693781f, 0.7995302f), new(-4.07471f, 0.5141128f), new(-4.499601f, 0.09934463f), new(-4.437096f, -0.2528922f), new(-3.810297f, -0.5969809f), new(-2.67039f, -0.8525136f), new(0.001282372f, -0.9788069f), new(-0.139402f, 0.6138527f)],
                KnockbackMult = 0.4f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.94f, 1.27f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "AltSlash Clone",
                Position = new Vector3(0.05324827f, 0.071f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.WITCH,
                UseVanillaSound = VanillaAttackType.WITCH,
                TinkerHitbox = [new(-0.8519674f, 0.6130633f), new(-2.834664f, 0.7618857f), new(-3.936055f, 0.5098518f), new(-4.570406f, 0.06457567f), new(-4.467158f, -0.2682983f), new(-3.751373f, -0.5291938f), new(-2.77213f, -0.8540588f), new(-0.1229159f, -0.9834825f), new(-0.2010562f, 0.5333951f)],
            };
        }

        public static Attack UpSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-2.219434f, 0.8184038f), new(-3.584109f, 0.683791f), new(-4.43316f, 0.302762f), new(-4.543458f, -0.1424165f), new(-3.917886f, -0.5499598f), new(-2.246635f, -0.9294458f), new(-0.3240262f, -0.9100322f), new(0.4268149f, 0.4168756f)],
                KnockbackMult = 0.4f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.9f, 1.3f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "UpSlash Clone",
                Position = new Vector3(0.182f, -0.102f, -0.001f),
                Rotation = new Quaternion(0f, 0f, -0.548058f, 0.8364404f),
                UseVanillaAnimLibrary = VanillaAttackType.WITCH,
                UseVanillaSound = VanillaAttackType.WITCH,
                TinkerHitbox = [new(-2.354095f, 0.7393622f), new(-3.383793f, 0.591568f), new(-4.238644f, 0.3628682f), new(-4.642245f, -0.01429151f), new(-4.25607f, -0.3214205f), new(-2.690129f, -0.7518311f), new(-0.3845556f, -0.8158565f), new(0.2268636f, 0.4280311f)],
            };
        }

        public static Attack WallSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-1.659663f, 0.7776427f), new(-3.118829f, 0.7466406f), new(-4.221398f, 0.4185991f), new(-4.504023f, 0.1028627f), new(-4.362025f, -0.3477472f), new(-3.059276f, -0.8278606f), new(0.1454569f, -1.09337f), new(0.251136f, 0.5163339f)],
                KnockbackMult = 0.4f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.95f, 1.15f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "WallSlash Clone",
                Position = new Vector3(0.29f, 0f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.WITCH,
                UseVanillaSound = VanillaAttackType.WITCH,
                TinkerHitbox = [new(-2.064669f, 0.6985435f), new(-3.383793f, 0.591568f), new(-4.122205f, 0.3076134f), new(-4.313757f, -0.06670697f), new(-4.003167f, -0.3993797f), new(-2.690129f, -0.7518311f), new(0.06024361f, -0.9820099f), new(0.1009693f, 0.5346031f)],
            };
        }

        //End class
    }

    public static class Architect
    {
        //Guideline to help with pasting
        public static Attack Slash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 2,
                Hitbox = [new(-4.37726f, -0.1674941f), new(-4.371671f, -0.6333559f), new(-0.03895153f, -1.05918f), new(0.0071894f, 0.7997019f)],
                KnockbackMult = 0.5f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.LagHit,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.87f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.FirstHit,
                StunDamage = 0.3333333f,
                Name = "Slash Clone",
                Position = new Vector3(-0.109f, 0.078f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.ARCHITECT,
                UseVanillaSound = VanillaAttackType.ARCHITECT,
                TinkerHitbox = [new(-4.433606f, -0.2878133f), new(-4.397344f, -0.6247689f), new(-0.03895153f, -0.9994648f), new(-0.02990121f, 0.6934371f)],
            };
        }

        public static Attack AltSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 2,
                Hitbox = [new(-4.305623f, -0.39423f), new(-4.288668f, -0.8480631f), new(-0.01810957f, -1.0473f), new(-0.008597109f, 0.5456799f)],
                KnockbackMult = 0.5f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.LagHit,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.87f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.FirstHit,
                StunDamage = 0.3333333f,
                Name = "AltSlash Clone",
                Position = new Vector3(-0.1589926f, -0.08699942f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.ARCHITECT,
                UseVanillaSound = VanillaAttackType.ARCHITECT,
                TinkerHitbox = [new(-4.281367f, -0.4791889f), new(-4.263355f, -0.7847196f), new(-0.1054408f, -1.100453f), new(-0.1085441f, 0.2801936f)],
            };
        }

        public static Attack UpSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 2,
                Hitbox = [new(-4.399632f, -0.1151975f), new(-4.38897f, -0.6121892f), new(-0.04238968f, -1.001415f), new(-0.03035978f, 0.5113368f)],
                KnockbackMult = 0.3333333f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.LagHit,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.87f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.FirstHit,
                StunDamage = 0.3333333f,
                Name = "UpSlash Clone",
                Position = new Vector3(0.11f, -0.5f, -0.001f),
                Rotation = new Quaternion(0f, 0f, -0.7071068f, 0.7071068f),
                UseVanillaAnimLibrary = VanillaAttackType.ARCHITECT,
                UseVanillaSound = VanillaAttackType.ARCHITECT,
                TinkerHitbox = [new(-4.460157f, -0.1797506f), new(-4.468345f, -0.6047509f), new(-0.01386277f, -1.009685f), new(-0.03035977f, 0.4120669f)],
            };
        }

        public static Attack WallSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 2,
                Hitbox = [new(-4.29803f, -0.356111f), new(-4.320097f, -0.7821319f), new(0.06141316f, -1.039114f), new(-0.02612721f, 0.4410953f)],
                KnockbackMult = 0.3333333f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.LagHit,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.87f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.FirstHit,
                StunDamage = 0.3333333f,
                Name = "WallSlash Clone",
                Position = new Vector3(0.21f, 0.08f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.ARCHITECT,
                UseVanillaSound = VanillaAttackType.ARCHITECT,
                TinkerHitbox = [new(-4.286123f, -0.4276953f), new(-4.258687f, -0.7735714f), new(0.06141316f, -1.039114f), new(0.01467366f, 0.3693868f)],
            };
        }


        //End class
    }

    public static class Beast
    {
        //Guideline to help with pasting
        public static Attack Slash()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(0.1583415f, 1.927816f), new(-1.799766f, 1.080612f), new(-2.913895f, -0.3173315f), new(-3.300937f, -1.405101f), new(-2.834026f, -1.960922f), new(-2.310222f, -1.789971f), new(-0.2232692f, -0.4771613f), new(0.7789632f, 1.417203f)],
                KnockbackMult = 0.75f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.95f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "Slash Clone",
                Position = new Vector3(0.1239973f, -0.14f, -0.001f),
                Rotation = new Quaternion(0f, 0f, -0.1293295f, 0.9916017f),
                UseVanillaAnimLibrary = VanillaAttackType.BEAST,
                UseVanillaSound = VanillaAttackType.BEAST,
                TinkerHitbox = [new(0.1037903f, 2.019298f), new(-2.127556f, 0.6837673f), new(-2.590401f, -0.1103945f), new(-3.244377f, -1.390092f), new(0.09509563f, -1.391689f), new(1.176918f, 1.628759f)],
            };
        }

        public static Attack AltSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-1.58898f, 0.8416033f), new(-3.185126f, -0.3010923f), new(-3.536198f, -1.381486f), new(-3.123369f, -1.694607f), new(0.4398748f, 0.01703927f), new(0.5788091f, 1.594657f)],
                KnockbackMult = 0.75f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.95f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "AltSlash Clone",
                Position = new Vector3(0.184f, -0.04f, -0.001f),
                Rotation = new Quaternion(0f, 0f, -0.06400983f, 0.9979493f),
                UseVanillaAnimLibrary = VanillaAttackType.BEAST,
                UseVanillaSound = VanillaAttackType.BEAST,
                TinkerHitbox = [new(-0.4817123f, 1.212391f), new(-2.550278f, 0.2232218f), new(-3.375183f, -1.474832f), new(0.06767035f, -1.468483f), new(0.7665176f, 1.46198f)],
            };
        }

        public static Attack UpSlash()
        {
            return new Attack()
            {
                AnimName = "UpSlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-0.7232119f, 1.086384f), new(-2.321859f, 0.2827509f), new(-3.298136f, -0.6360654f), new(-3.485858f, -1.404755f), new(-3.191488f, -1.749396f), new(-0.9445412f, -0.7733818f), new(0.9461786f, 1.259395f), new(0.09494008f, 1.275437f)],
                KnockbackMult = 0.75f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "UpSlash Clone",
                Position = new Vector3(0.157f, -0.298f, -0.001f),
                Rotation = new Quaternion(0f, 0f, -0.7590445f, 0.6510389f),
                UseVanillaAnimLibrary = VanillaAttackType.BEAST,
                UseVanillaSound = VanillaAttackType.BEAST,
                TinkerHitbox = [new(-0.7156488f, 1.066757f), new(-2.309881f, 0.2050245f), new(-3.499548f, -0.9773641f), new(-3.077475f, -1.614571f), new(0.1795989f, -1.170006f), new(1.098475f, 1.338113f), new(0.09494008f, 1.275437f)],
            };
        }

        public static Attack WallSlash()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-1.530293f, 1.23086f), new(-2.728676f, -0.08019733f), new(-3.201427f, -1.233396f), new(-2.479874f, -1.784595f), new(0.1716719f, 0.2369041f), new(0.3929558f, 1.993113f)],
                KnockbackMult = 0.75f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(0.95f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "WallSlash Clone",
                Position = new Vector3(0.655f, -0.018f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.BEAST,
                UseVanillaSound = VanillaAttackType.BEAST,
                TinkerHitbox = [new(0.2832125f, 1.843135f), new(-2.127556f, 0.6837673f), new(-2.590401f, -0.1103945f), new(-3.244377f, -1.390092f), new(0.09509563f, -1.391689f), new(0.2797987f, 1.24708f)],
            };
        }

        public static Attack SlashRage()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt Rage",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-1.09227f, 1.036819f), new(-2.465698f, 0.004898071f), new(-3.748055f, -1.628109f), new(-0.006744385f, -1.643265f), new(1.235313f, 2.095432f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "Slash Rage Clone",
                Position = new Vector3(-0.6643372f, 0.2169266f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.BEAST,
                UseVanillaSound = VanillaAttackType.BEAST,
                TinkerHitbox = [new(-1.09227f, 1.036819f), new(-2.465698f, 0.004898071f), new(-3.748055f, -1.628109f), new(-0.006744385f, -1.643265f), new(1.235313f, 2.095432f)],
            };
        }

        public static Attack AltSlashRage()
        {
            return new Attack()
            {
                AnimName = "SlashEffect Rage",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-0.09846497f, 2.234371f), new(-2.16703f, 0.619236f), new(-2.915291f, -1.283207f), new(0.08044434f, -1.251312f), new(1.124222f, 2.458408f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "AltSlash Rage Clone",
                Position = new Vector3(-0.6643372f, 0.2169266f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.BEAST,
                UseVanillaSound = VanillaAttackType.BEAST,
                TinkerHitbox = [new(-0.09846497f, 2.234371f), new(-2.16703f, 0.619236f), new(-2.915291f, -1.283207f), new(0.08044434f, -1.251312f), new(1.124222f, 2.458408f)],
            };
        }

        public static Attack UpSlashRage()
        {
            return new Attack()
            {
                AnimName = "UpSlashEffect Rage",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-0.09846497f, 2.234371f), new(-2.16703f, 0.619236f), new(-3.196335f, -1.551479f), new(-2.065712f, -1.609005f), new(2.018448f, 2.611706f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "UpSlash Rage Clone",
                Position = new Vector3(0f, 0f, -0.001f),
                Rotation = new Quaternion(0f, 0f, -0.7071068f, 0.7071068f),
                UseVanillaAnimLibrary = VanillaAttackType.BEAST,
                UseVanillaSound = VanillaAttackType.BEAST,
                TinkerHitbox = [new(-0.09846497f, 2.234371f), new(-2.16703f, 0.619236f), new(-3.196335f, -1.551479f), new(-2.065712f, -1.609005f), new(2.018448f, 2.611706f)],
            };
        }

        public static Attack WallSlashRage()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt Rage",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(0.6733704f, 2.66708f), new(-0.9906006f, 1.629101f), new(-2.258263f, 0.4389191f), new(-3.154953f, -1.453964f), new(-2.255463f, -1.481113f), new(0.5812454f, 1.246117f)],
                KnockbackMult = 1f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(-1f, 1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 1f,
                Name = "WallSlash Rage Clone",
                Position = new Vector3(0.6100006f, 0f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.BEAST,
                UseVanillaSound = VanillaAttackType.BEAST,
                TinkerHitbox = [new(0.6969875f, 2.540031f), new(-0.9906006f, 1.629101f), new(-2.258263f, 0.4389191f), new(-3.154953f, -1.453964f), new(-2.255463f, -1.481113f), new(0.6693183f, 1.206848f)],
            };
        }

        public static Attack DashSlashRage()
        {
            return new Attack()
            {
                AnimName = "Dash Attack Effect Rage",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(0.008283202f, 2.20976f), new(-1.539118f, 1.79911f), new(-2.182922f, 0.7831573f), new(-2.645378f, -0.9358742f), new(-2.411702f, -2.186971f), new(-0.7393968f, -0.916446f), new(1.085493f, 0.6885362f), new(1.799164f, 1.961136f)],
                KnockbackMult = 0.25f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1.15f, 1.1f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 0.5f,
                Name = "DashSlash Rage Clone",
                Position = new Vector3(-0.87f, -0.08f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0.06253184f, 0.9980431f),
                UseVanillaAnimLibrary = VanillaAttackType.BEAST,
                UseVanillaSound = VanillaAttackType.BEAST,
                TinkerHitbox = [new(-1.493291f, 1.728071f), new(-2.512829f, -0.9044462f), new(-2.371932f, -2.062449f), new(-1.82451f, -1.579752f), new(-0.9168167f, -0.8422537f), new(-0.350563f, -0.2209239f), new(1.026352f, 0.8492875f), new(1.536615f, 1.945836f)],
            };
        }

        //End class
    }

    public static class Cloakless
    {
        //Guideline to help with pasting
        public static Attack Kick()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-0.3591563f, -0.09447479f), new(-2.68206f, -0.5980517f), new(-2.687006f, -0.819957f), new(-0.361463f, -0.9400838f)],
                KnockbackMult = 0.75f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1.22f, 1.5f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 0.5f,
                Name = "Kick Clone",
                Position = new Vector3(0.5482807f, 0.3315201f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.CLOAKLESS,
                UseVanillaSound = VanillaAttackType.CLOAKLESS,
                TinkerHitbox = [new(-0.3591563f, -0.09447479f), new(-2.688129f, -0.750927f), new(-0.3554001f, -0.9203568f)],
            };
        }

        public static Attack AltKick()
        {
            return new Attack()
            {
                AnimName = "SlashEffectAlt",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-2.499177f, -0.5149459f), new(-2.499187f, -0.733762f), new(-0.3894946f, -1.023616f), new(-0.4124554f, -0.388766f)],
                KnockbackMult = 0.75f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1.22f, 1.5f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 0.5f,
                Name = "AltKick Clone",
                Position = new Vector3(0.3404846f, 0.3858833f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.CLOAKLESS,
                UseVanillaSound = VanillaAttackType.CLOAKLESS,
                TinkerHitbox = [new(-2.533838f, -0.6262894f), new(-0.3894946f, -1.023616f), new(-0.3942668f, -0.4084905f)],
            };
        }

        public static Attack UpKick()
        {
            return new Attack()
            {
                AnimName = "UpSlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-0.9496937f, -0.1273422f), new(-2.522201f, -0.4889261f), new(-2.549889f, -0.748804f), new(-1.006004f, -1.035868f), new(-0.2835655f, -0.9479523f), new(-0.06134796f, 0.04549026f)],
                KnockbackMult = 0.75f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(1.4f, 1.5f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 0.5f,
                Name = "UpKick Clone",
                Position = new Vector3(0.8129597f, -0.5052757f, -0.001f),
                Rotation = new Quaternion(0f, 0f, -0.6773897f, 0.7356244f),
                UseVanillaAnimLibrary = VanillaAttackType.CLOAKLESS,
                UseVanillaSound = VanillaAttackType.CLOAKLESS,
                TinkerHitbox = [new(-0.9496937f, -0.1273422f), new(-2.511139f, -0.6035881f), new(-1.0047f, -1.021122f), new(-0.2835655f, -0.9479523f), new(-0.06134796f, 0.04549026f)],
            };
        }

        public static Attack WallKick()
        {
            return new Attack()
            {
                AnimName = "SlashEffect",
                DamageMult = 1f,
                FramesBetweenMultiHits = 0,
                Hitbox = [new(-0.4189987f, -0.2021255f), new(-2.628231f, -0.5785439f), new(-2.648257f, -0.8557994f), new(-0.4009311f, -0.9333166f)],
                KnockbackMult = 0.75f,
                MultiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Full,
                MultiHitMultipliers = [],
                Scale = new Vector3(-1.22f, 1.5f, 1f),
                SilkGeneration = HitSilkGeneration.Full,
                StunDamage = 0.5f,
                Name = "WallKick Clone",
                Position = new Vector3(-0.6214561f, 0.3745346f, -0.001f),
                Rotation = new Quaternion(0f, 0f, 0f, 1f),
                UseVanillaAnimLibrary = VanillaAttackType.CLOAKLESS,
                UseVanillaSound = VanillaAttackType.CLOAKLESS,
                TinkerHitbox = [new(-0.4189987f, -0.2021255f), new(-2.642732f, -0.7433599f), new(-0.3732809f, -0.8928401f)],
            };
        }

        //End class
    }
}
