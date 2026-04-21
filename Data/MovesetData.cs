using Needleforge.Attacks;
using System;
using System.Reflection;
using UnityEngine;
using ConfigGroup = HeroController.ConfigGroup;

namespace Needleforge.Data;

/// <summary>
/// Represents a moveset for a custom crest.
/// </summary>
public class MovesetData
{

    internal readonly CrestData Crest;

    internal MovesetData(CrestData owner)
    {
        Crest = owner;
    }

    /// <summary>
    /// <para>
    /// Defines how Hornet behaves when this crest is equipped. Properties of this object
    /// control speed and recovery time for attacks, which of Hornet's animations are
    /// overridden for this crest, which movement abilities she can use, whether or not
    /// she can access her inventory or tools, some of the behaviour of charged and down
    /// slashes, and more.
    /// </para><para>
    /// If left null, then after a save file is loaded this will be set to a copy of
    /// Hunter crest's configuration.
    /// You can modify this default config in <see cref="OnInitialized"/> or
    /// any time during gameplay.
    /// </para>
    /// </summary>
    public HeroConfigNeedleforge? HeroConfig
    {
        get => _heroConf;
        set
        {
            if (value) value.name = Crest.name;

            _heroConf = value;
            if (Crest.ToolCrest)
                Crest.ToolCrest.heroConfig = value;
            if (ConfigGroup != null)
                ConfigGroup.Config = value;

            if (DownSlash != null)
                DownSlash.HeroConfig = value;
            if (AltDownSlash != null)
                AltDownSlash.HeroConfig = value;
        }
    }
    private HeroConfigNeedleforge? _heroConf;

    /// <summary>
    /// Defines the visual, auditory, and damage properties of the side attack.
    /// </summary>
    /// <remarks>
    /// The corresponding <see cref="HeroControllerConfig.heroAnimOverrideLib"/> animation
    /// is "Slash"
    /// </remarks>
    public Attack? Slash
    {
        get => _slash;
        set
        {
            _slash = value;
            UpdateConfigGroup(nameof(ConfigGroup.NormalSlashObject), value);
        }
    }
    private Attack? _slash;

    /// <summary>
    /// Defines the visual, auditory, and damage properties of the up attack.
    /// </summary>
    /// <remarks>
    /// The corresponding <see cref="HeroControllerConfig.heroAnimOverrideLib"/> animation
    /// is "UpSlash"
    /// </remarks>
    public Attack? UpSlash
    {
        get => _upslash;
        set
        {
            _upslash = value;
            UpdateConfigGroup(nameof(ConfigGroup.UpSlashObject), value);
        }
    }
    private Attack? _upslash;

    /// <summary>
    /// Defines the visual, auditory, and damage properties of the wall-sliding attack.
    /// </summary>
    /// <remarks>
    /// The corresponding <see cref="HeroControllerConfig.heroAnimOverrideLib"/> animation
    /// is "Wall Slash"
    /// </remarks>
    public Attack? WallSlash
    {
        get => _wallSlash;
        set
        {
            if (value != null)
                value.IsWallSlash = true;
            _wallSlash = value;
            UpdateConfigGroup(nameof(ConfigGroup.WallSlashObject), value);
        }
    }
    private Attack? _wallSlash;

    /// <summary>
    /// Defines the visual, auditory, and damage properties of the down attack.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The type and behaviour of down attacks are determined by properties of
    /// <see cref="HeroConfig"/>, particularly
    /// <see cref="HeroControllerConfig.downSlashType">downSlashType</see>, which must
    /// be set <i>before</i> the moveset is initialized. If the type is set to Custom, 
    /// </para><para>
    /// Unless an FSM edit is specified that plays different animations, the animations
    /// in <see cref="HeroControllerConfig.heroAnimOverrideLib"/> which are used for this
    /// attack are "DownSpike Antic", "DownSpike", and "Downspike Recovery" for
    /// Downspike-type; "DownSlash" and "DownSlashAlt" for Slash and Custom types.
    /// </para>
    /// </remarks>
    public DownAttack? DownSlash
    {
        get => _downSlash;
        set
        {
            if (value != null)
                value.HeroConfig = HeroConfig;
            _downSlash = value;
            UpdateConfigGroup(nameof(ConfigGroup.DownSlashObject), value);
        }
    }
    private DownAttack? _downSlash;

    /// <summary>
    /// When a certain vanilla down slash is desired, this property acts as an alternative
    /// to <see cref="DownSlash"/>.
    /// This will overwrite the behaviour defined in <see cref="DownSlash"/>.
    /// Optional.
    /// <para>Note that this only refers to downslashes with custom behaviour.
    /// Valid options are WITCH, REAPER, SHAMAN, ARCHITECT, BEAST.
    /// Required animations will be automatically cloned, but if desired, 
    /// custom (non-looping) animations can be set under the following names.
    /// Animations that require a trigger will be marked with a T:</para>
    /// <para>When using BEAST, required animations are "SpinBall Antic", "SpinBall Launch",
    /// "SpinBall", "SpinBall Grind", "SpinBall Rebound".</para>
    /// <para>When using REAPER, required animations are "v3 Down Slash Antic", T:"v3 Down Slash".</para>
    /// <para>When using SHAMAN, the required animation is T:"DownSlash".</para>
    /// <para>When using WITCH, the required animations are "DownSpike", "DownSpike Antic", "Downspike Followup".</para>
    /// <para>When using ARCHITECT, the required animations are T:"DownSpike Charge", "DownSpike Antic", 
    /// "DownSpike", "DownSpike Charged", "Drill Grind", "Drill Grind Charged".</para>
    /// </summary>
    public VanillaAttackType? UseVanillaDownSlash { get; set; } = null;

    /// <summary>
    /// Defines the visual, auditory and damage properties of the dash attack.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, each attack in the Steps array plays in sequence as a series of
    /// lunging attacks, similar to Hunter or Witch crest. The speed and duration of the
    /// attacks is controlled by properties of <see cref="HeroConfig"/>.
    /// This default behaviour can be changed by specifying a
    /// <see cref="HeroConfigNeedleforge.DashSlashFsmEdit"/> function.
    /// </para><para>
    /// Unless an FSM edit is specified that plays different animations, the animations
    /// in <see cref="HeroControllerConfig.heroAnimOverrideLib"/> which are used for this
    /// attack are "Dash Attack Antic 1" and "Dash Attack 1", "Dash Attack Antic 2" and
    /// "Dash Attack 2", and so on, for as many Steps as the attack has.
    /// </para>
    /// </remarks>
    public DashAttack? DashSlash
    {
        get => _dashSlash;
        set
        {
            _dashSlash = value;
            UpdateConfigGroup(nameof(ConfigGroup.DashStab), value);
        }
    }
    private DashAttack? _dashSlash;
    /// <summary>
    /// When a certain vanilla dash slash is desired, this property acts as an alternative
    /// to <see cref="DashSlash"/>.
    /// This will overwrite the behaviour defined in <see cref="DashSlash"/>.
    /// Optional.
    /// <para>Hunter will have its Attack Steps forced to 1, and Witch will
    /// have its forced to 2. To get around this, consider using <see cref="DashSlash"/> instead.</para>
    /// <para>Note that when using Witch, the object stored in <see cref="ConfigGroup.DashStab"/>
    /// is the parent object containing "Dash Slash 1" and "Dash Slash 2". In all other cases,
    /// <see cref="ConfigGroup.DashStab"/> is the standard slash, and <see cref="ConfigGroup.DashStabAlt"/>
    /// is a variant slash, such as Wanderer's alternate or Architect's charged.</para>
    /// <para>Required animations will be automatically cloned, but if desired, 
    /// custom (non-looping) animations can be set under the following names.
    /// Animations that require a trigger will be marked with a T:</para>
    /// <para>When using REAPER, required animations are "Dash Upper Antic", "Dash Upper",
    /// "Dash Upper Recovery"</para>
    /// <para>When using WANDERER, required animations are T:"Wanderer Dash Attack",
    /// T:"Wanderer Dash Attack Alt", T:"Wanderer DashRecoil", "Wanderer RecoilStab"</para>
    /// <para>When using BEAST or SHAMAN, required animations are "Dash Attack Antic", 
    /// "Dash Attack Leap", "Dash Attack Slash"</para>
    /// <para>When using ARCHITECT, required animations are T:"Dash Attack Charge", 
    /// "Dash Attack"</para>
    /// <para>When using WITCH, required animations are "Dash Attack Antic 1", "Dash Attack 1",
    /// "Dash Attack Recover", "Dash Attack Antic 2", "Dash Attack 2"</para>
    /// </summary>
    public VanillaAttackType? UseVanillaDashSlash { get; set; } = null;

    /// <summary>
    /// Defines the visual, auditory and damage properties of the charged attack,
    /// as well as some of Hornet's behaviour when she performs the attack.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, each attack in the Steps array plays in sequence when the attack's
    /// GameObject is activated, and the GameObject deactivates afterwards. Properties
    /// in <see cref="HeroConfig"/> affect how Hornet moves during the attack and whether
    /// or not it can be repeated by mashing the attack button.
    /// This default behaviour can be changed by specifying a
    /// <see cref="HeroConfigNeedleforge.ChargedSlashFsmEdit"/> and adjusting the
    /// properties of this object.
    /// </para><para>
    /// Unless an FSM edit is specified that plays different animations, the animations
    /// in <see cref="HeroControllerConfig.heroAnimOverrideLib"/> which are used for this
    /// attack are "Slash_Charged" and "Slash_Charged_Loop".
    /// Note that "Slash_Charged" needs one trigger frame marking the end of its
    /// anticipation stage.
    /// </para>
    /// </remarks>
    public ChargedAttack? ChargedSlash
    {
        get => _chargedSlash;
        set
        {
            _chargedSlash = value;
            UpdateConfigGroup(nameof(ConfigGroup.ChargeSlash), value);
        }
    }
    private ChargedAttack? _chargedSlash;

    /// <summary>
    /// When a vanilla charged slash is desired, this property acts as an alternative 
    /// to setting up a custom one with <see cref="ChargedSlash"/>.
    /// This will overwrite the behaviour defined in <see cref="ChargedSlash"/>.
    /// Optional.
    /// <para>Cloakless does not have a charged slash, so it has no effect when set as this field.</para>
    /// </summary>
    public VanillaAttackType? UseVanillaChargedSlash { get; set; } = null;

    /// <summary>
    /// Defines the visual, auditory, and damage properties of the alternate side attack,
    /// which is used when the player attacks multiple times in quick succession.
    /// Optional.
    /// </summary>
    /// <remarks>
    /// The corresponding <see cref="HeroControllerConfig.heroAnimOverrideLib"/> animation
    /// is "SlashAlt"
    /// </remarks>

    public Attack? AltSlash
    {
        get => _slashAlt;
        set
        {
            _slashAlt = value;
            UpdateConfigGroup(nameof(ConfigGroup.AlternateSlashObject), value);
        }
    }
    private Attack? _slashAlt;

    /// <summary>
    /// Defines the visual, auditory, and damage properties of the alternate up attack,
    /// which is used when the player attacks multiple times in quick succession.
    /// Optional.
    /// </summary>
    /// <remarks>
    /// The corresponding <see cref="HeroControllerConfig.heroAnimOverrideLib"/> animation
    /// is "UpSlashAlt"
    /// </remarks>
    public Attack? AltUpSlash
    {
        get => _upslashAlt;
        set
        {
            _upslashAlt = value;
            UpdateConfigGroup(nameof(ConfigGroup.AltUpSlashObject), value);
        }
    }
    private Attack? _upslashAlt;

    /// <summary>
    /// Defines the visual, auditory, and damage properties of the alternate down attack,
    /// which is used when the player attacks multiple times in quick succession.
    /// Optional.
    /// </summary>
    /// <inheritdoc cref="DownSlash" path="/remarks"/>
    public DownAttack? AltDownSlash
    {
        get => _altDownSlash;
        set
        {
            if (value != null)
                value.HeroConfig = HeroConfig;
            _altDownSlash = value;
            UpdateConfigGroup(nameof(ConfigGroup.AltDownSlashObject), value);
        }
    }
    private DownAttack? _altDownSlash;

    /// <summary>
    /// <para>
    /// Container for this crest's <see cref="HeroConfig"/> and attacks which is
    /// referenced by the <see cref="HeroController"/>.
    /// This will only contain a value after a save has been loaded;
    /// it can be accessed in <see cref="OnInitialized"/>.
    /// </para><para>
    /// Modifying properties of this object directly will <i>not</i> update MovesetData.
    /// It's recommended to use the Attack properties to make changes whenever possible.
    /// </para>
    /// </summary>
    public ConfigGroup? ConfigGroup { get; internal set; }

    /// <summary>
    /// <para>
    /// Runs immediately after the <see cref="GameObject"/>s for
    /// this moveset's attacks are created and set up. Any additional modifications or
    /// set up for attacks or <see cref="HeroConfig"/> which couldn't be done without
    /// GameObject access can be done in this event.
    /// </para><para>
    /// It's recommended to use the properties of each of this moveset's
    /// <see cref="Attack"/>s to make modifications to them. For finer control which may
    /// require more knowledge of the underlying structure of an attack, each attack's
    /// GameObject property can be modified directly.
    /// </para>
    /// </summary>
    public event Action? OnInitialized;

    /// <inheritdoc cref="OnInitialized"/>
    internal void ExtraInitialization() => OnInitialized?.DynamicInvoke();

    private void UpdateConfigGroup(string field, GameObjectProxy? value) {
        if (ConfigGroup == null)
            return;

        FieldInfo fi = typeof(ConfigGroup).GetField(field);
        if (fi == null)
        {
            ModHelper.LogError($"{Crest.name}: Failed to update config group field \"{field}\".");
            return;
        }

        if (value != null)
        {
            fi.SetValue(
                ConfigGroup,
                value.GameObject
                    ? value.GameObject
                    : value.CreateGameObject(ConfigGroup.ActiveRoot, HeroController.instance)
            );
        }
        else
            fi.SetValue(ConfigGroup, null);

        ConfigGroup.Setup();
    }

}
