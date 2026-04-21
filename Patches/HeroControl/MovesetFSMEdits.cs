using HarmonyLib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Needleforge.Attacks;
using Needleforge.Data;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PlayerDataTest;
using DownSlashTypes = HeroControllerConfig.DownSlashTypes;

namespace Needleforge.Patches.HeroControl;

[HarmonyPatch(typeof(HeroController), nameof(HeroController.Start))]
internal static class MovesetFSMEdits
{
    private static void Prefix(HeroController __instance)
    {
        ModHelper.Log("Editing Moveset FSMs...");
        DownSlashFSMEdits(__instance);
        DashSlashFSMEdits(__instance);
        ChargedSlashFSMEdits(__instance);
    }

    private static void DownSlashFSMEdits(HeroController hc)
    {
        IEnumerable<MovesetData>
            movesets = NeedleforgePlugin.newCrestData
                .Select(cd => cd.Moveset)
                .Where(m => m.HeroConfig
                    && (m.HeroConfig.DownSlashType == DownSlashTypes.Custom
                        || m.HeroConfig.DownSlashFsmEdit != null)
                );

        if (!movesets.Any())
            return;

        PlayMakerFSM fsm = hc.crestAttacksFSM;
        if (!fsm.Fsm.preprocessed)
            fsm.Preprocess();

        FsmState
            Idle = fsm.GetState("Idle")!,
            End = fsm.GetState("End")!,
            SpinBallAntic = fsm.GetState("SpinBall Antic")!,
            SetNotRage = fsm.GetState("Set Not Rage")!,
            RprDownslashAntic = fsm.GetState("Rpr Downslash Antic")!,
            RprDownslash = fsm.GetState("Rpr Downslash")!,
            ShamanDownslashAntic = fsm.GetState("Shaman Downslash Antic")!,
            ShamanDownslash = fsm.GetState("Shaman Downslash")!,
            WitchDownslashAntic = fsm.GetState("Witch Downslash Antic")!,
            WitchDownslash = fsm.GetState("Witch Downslash")!,
            GetLashAngle = fsm.GetState("Get Lash Angle")!,
            NewLashGrab = fsm.GetState("NewLash Grab")!,
            ToolMChargeStart = fsm.GetState("ToolM ChargeStart")!,
            DrillUncharged = fsm.GetState("Drill Uncharged")!,
            DrillCharged = fsm.GetState("Drill Charged")!;

        #region Default behaviour for cloned vanilla slashes

        #region Beast
        SpinBallAntic.InsertMethod(0, _ =>
        {
            CrestData? crest = GetEquippedCrestIfOfDownslashType(VanillaAttackType.BEAST);
            if (crest == null) { return; }

            string[] requiredAnimations = new[] { "SpinBall Antic", "SpinBall Launch",
                    "SpinBall", "SpinBall Grind", "SpinBall Rebound" };

            CloneAnimationsToIfNotExists(crest, VanillaReferences.AnimationLibraryNames.BEAST, requiredAnimations);
        });

        SetNotRage.AddMethod(_ =>
        {
            CrestData? crest = GetEquippedCrestIfOfDownslashType(VanillaAttackType.BEAST);
            if (crest == null) { return; }

            GameObject? downSlash = crest.Moveset.ConfigGroup?.DownSlashObject?.gameObject;

            fsm.GetGameObjectVariable("Current SpinSlash").value = downSlash;

            if (downSlash == null)
            {
                ModHelper.LogError($"Crest {crest.name} unable to copy Beast's downslash - " +
                    " the value of the GameObject is null. Has vanilla Beast been modified?");
            }
        });
        #endregion

        #region Reaper
        RprDownslashAntic.InsertMethod(0, _ =>
        {
            //Reset slash target to vanilla
            SendMessage? sendMessage = RprDownslash.GetFirstActionOfType<SendMessage>();
            if (sendMessage != null)
            {
                sendMessage.gameObject.GameObject = VanillaAttacks.CustomDownSlashes.GetDownSlashForCrest(VanillaAttackType.REAPER)!.Attack;
            }

            CrestData? crest = GetEquippedCrestIfOfDownslashType(VanillaAttackType.REAPER);
            if (crest == null) { return; }

            string[] requiredAnimations = new[] { "v3 Down Slash Antic", "v3 Down Slash" };

            CloneAnimationsToIfNotExists(crest, VanillaReferences.AnimationLibraryNames.REAPER, requiredAnimations);

            //Update slash target
            if (sendMessage != null)
            {
                sendMessage.gameObject.GameObject = crest.Moveset.ConfigGroup?.DownSlashObject?.gameObject;
            }
        });
        #endregion

        #region Shaman

        ShamanDownslashAntic.InsertMethod(0, _ =>
        {
            //Reset slash target to vanilla
            SendMessage? sendMessage = ShamanDownslash.GetAction<SendMessage>(4);
            if (sendMessage != null)
            {
                sendMessage.gameObject.GameObject = VanillaAttacks.CustomDownSlashes.GetDownSlashForCrest(VanillaAttackType.SHAMAN)!.Attack;
            }

            CrestData? crest = GetEquippedCrestIfOfDownslashType(VanillaAttackType.SHAMAN);
            if (crest == null) { return; }

            string[] requiredAnimations = new[] { "DownSlash" };

            CloneAnimationsToIfNotExists(crest, VanillaReferences.AnimationLibraryNames.SHAMAN, requiredAnimations);

            //Update slash target
            if (sendMessage != null)
            {
                sendMessage.gameObject.GameObject = crest.Moveset.ConfigGroup?.DownSlashObject?.gameObject;
            }
        });

        #endregion

        #region Witch

        WitchDownslashAntic.InsertMethod(0, _ =>
        {
            //Reset slash target to vanilla
            SendMessage? sendMessage = WitchDownslash.GetAction<SendMessage>(4);
            CallMethodProper? callMethod = GetLashAngle.GetFirstActionOfType<CallMethodProper>();
            CallMethodProper? callMethod2 = NewLashGrab.GetFirstActionOfType<CallMethodProper>();
            //I always wondered if you could skip the != null part, but I never checked.
            //Oh well.
            if (sendMessage != null && callMethod != null && callMethod2 != null)
            {
                sendMessage.gameObject.GameObject = VanillaAttacks.CustomDownSlashes.GetDownSlashForCrest(VanillaAttackType.WITCH)!.Attack;
                callMethod.gameObject.GameObject = VanillaAttacks.CustomDownSlashes.GetDownSlashForCrest(VanillaAttackType.WITCH)!.Attack;
                callMethod2.gameObject.GameObject = VanillaAttacks.CustomDownSlashes.GetDownSlashForCrest(VanillaAttackType.WITCH)!.Attack;
            }

            CrestData? crest = GetEquippedCrestIfOfDownslashType(VanillaAttackType.WITCH);
            if (crest == null) { return; }

            string[] requiredAnimations = new[] { "DownSpike", "DownSpike Antic", "Downspike Followup",  };

            CloneAnimationsToIfNotExists(crest, VanillaReferences.AnimationLibraryNames.WITCH, requiredAnimations);

            //Update slash target
            if (sendMessage != null && callMethod != null && callMethod2 != null)
            {
                sendMessage.gameObject.GameObject = crest.Moveset.ConfigGroup?.DownSlashObject?.gameObject;
                callMethod.gameObject.GameObject = crest.Moveset.ConfigGroup?.DownSlashObject?.gameObject;
                callMethod2.gameObject.GameObject = crest.Moveset.ConfigGroup?.DownSlashObject?.gameObject;
            }

            //The witch downslash has a few extra objects that it uses, namely Followup Slash and Lash Checker.
            //Cloning these would be kind of a hassle, and they don't have any unique behaviour.
            //So, we'll just...
            hc.gameObject.transform.Find("Attacks").Find("Witch").gameObject.SetActive(true);
            //As far as I know, this has no side effects. But keep it in mind, I guess.
        });

        #endregion

        #region Architect

        ToolMChargeStart.InsertMethod(0, _ =>
        {
            //Reset slash target to vanilla
            SendMessage? sendMessage = DrillUncharged.GetFirstActionOfType<SendMessage>();
            SendMessage? sendMessage2 = DrillCharged.GetFirstActionOfType<SendMessage>();
            if (sendMessage != null && sendMessage2 != null)
            {
                sendMessage.gameObject.GameObject = VanillaAttacks.CustomDownSlashes.GetDownSlashForCrest(VanillaAttackType.ARCHITECT)!.Attack;
                sendMessage2.gameObject.GameObject = VanillaAttacks.CustomDownSlashes.GetDownSlashForCrest(VanillaAttackType.ARCHITECT)!.AttackAlt;
            }

            CrestData? crest = GetEquippedCrestIfOfDownslashType(VanillaAttackType.ARCHITECT);
            if (crest == null) { return; }

            string[] requiredAnimations = new[] { "DownSpike Charge", "DownSpike Antic", "DownSpike", "DownSpike Charged",
            "Drill Grind", "Drill Grind Charged"};

            CloneAnimationsToIfNotExists(crest, VanillaReferences.AnimationLibraryNames.ARCHITECT, requiredAnimations);

            //Update slash target
            //Only in the case of architect is AltDownSlashObject used, storing the charged variant.
            if (sendMessage != null && sendMessage2 != null)
            {
                sendMessage.gameObject.GameObject = crest.Moveset.ConfigGroup?.DownSlashObject?.gameObject;
                sendMessage2.gameObject.GameObject = crest.Moveset.ConfigGroup?.AltDownSlashObject?.gameObject;
            }
        });

        #endregion
        #endregion

        foreach (MovesetData m in movesets)
        {
            string name = m.Crest.name;
            ModHelper.Log($"{name} Down Slash");

            var fsmEdit = m.HeroConfig!.DownSlashFsmEdit;

            if (fsmEdit == null)
            {
                ModHelper.LogError(
                    $"Crest {name} has a custom downslash type, but doesn't define " +
                    $"a {nameof(HeroConfigNeedleforge.DownSlashFsmEdit)} function."
                );
                continue;
            }
            if (m.HeroConfig.DownSlashType != DownSlashTypes.Custom)
            {
                ModHelper.LogError(
                    $"Crest {name} has a {nameof(HeroConfigNeedleforge.DownSlashFsmEdit)} " +
                    $"function, but its {nameof(HeroConfigNeedleforge.DownSlashType)} " +
                    $"is not {DownSlashTypes.Custom}."
                );
                continue;
            }
            if (string.IsNullOrWhiteSpace(m.HeroConfig!.downSlashEvent))
            {
                ModHelper.LogError(
                    $"Crest {name} has a custom downslash, but doesn't have a " +
                    $"valid {nameof(HeroControllerConfig.downSlashEvent)}."
                );
                continue;
            }

            FsmState AtkStart = fsm.AddState($"{name} Start");
            Idle.AddTransition(m.HeroConfig!.downSlashEvent, AtkStart.Name);

            fsmEdit.Invoke(fsm, AtkStart, out FsmState[] AtkEnds);

            foreach(var end in AtkEnds)
                end.AddTransition("FINISHED", End.Name);
        }
    }

    private static void DashSlashFSMEdits(HeroController hc)
    {
        PlayMakerFSM fsm = hc.sprintFSM;
        if (!fsm.Fsm.preprocessed)
            fsm.Preprocess();

        FsmState
            StartAttack = fsm.GetState("Start Attack")!,
            RegainControlNormal = fsm.GetState("Regain Control Normal")!,
            ReaperAntic = fsm.GetState("Reaper Antic")!,
            ReaperUpper = fsm.GetState("Reaper Upper")!,
            WandererSet = fsm.GetState("Wanderer Set ")!,
            WandererNml = fsm.GetState("Wanderer Nml")!,
            WandererAlt = fsm.GetState("Wanderer Alt")!,
            RecoilStabAttack = fsm.GetState("RecoilStab Attack")!,
            RecoilStabBounce = fsm.GetState("RecoilStab Bounce")!,
            WarriorAntic = fsm.GetState("Warrior Antic")!,
            RageMode = fsm.GetState("Rage Mode?")!,
            ShamanAntic = fsm.GetState("Shaman Antic")!,
            DrillChargeStart = fsm.GetState("Drill Charge Start")!,
            SetUncharged = fsm.GetState("Set Uncharged")!,
            SetCharged = fsm.GetState("Set Charged")!,
            SetAttackMultiple = fsm.GetState("Set Attack Multiple")!,
            ReactionType = fsm.GetState("Reaction Type")!;

        FsmInt
            crestIdx = fsm.AddIntVariable($"Equipped Crest {NeedleforgePlugin.Id}");


        #region Default behaviour for cloned vanilla slashes
        #region Reaper
        ReaperAntic.InsertMethod(0, _ =>
        {
            GameObject activeSlash = VanillaAttacks.DashSlashes.GetDashSlashForCrest(VanillaAttackType.REAPER)!.Attack;

            CrestData? crest = GetEquippedCrestIfOfDashslashType(VanillaAttackType.REAPER);
            if (crest != null) {
                string[] requiredAnimations = new[] { "Dash Upper Antic", "Dash Upper",
                "Dash Upper Recovery"};

                CloneAnimationsToIfNotExists(crest, VanillaReferences.AnimationLibraryNames.REAPER, requiredAnimations);

                activeSlash = crest.Moveset.ConfigGroup!.DashStab;
            }


            CallMethodProper? CallMethod = ReaperUpper.GetAction<CallMethodProper>(2);
            SendMessage? SendMessage = ReaperUpper.GetAction<SendMessage>(3);
            if (CallMethod != null && SendMessage != null)
            {
                CallMethod.gameObject.GameObject = activeSlash;
                SendMessage.gameObject.GameObject = activeSlash;
            }
        });
        #endregion

        #region Wanderer
        WandererSet.InsertMethod(0, _ =>
        {
            VanillaAttacks.VanillaAttackObjects objects = VanillaAttacks.DashSlashes.GetDashSlashForCrest(VanillaAttackType.WANDERER)!;

            GameObject activeNml = objects.Attack;
            GameObject activeAlt = objects.AttackAlt!;
            GameObject activeRecoil = objects.AttackSpecial!;

            CrestData? crest = GetEquippedCrestIfOfDashslashType(VanillaAttackType.WANDERER);
            if (crest != null)
            {
                string[] requiredAnimations = new[] { "Wanderer Dash Attack", "Wanderer Dash Attack Alt",
                "Wanderer DashRecoil", "Wanderer RecoilStab"};

                CloneAnimationsToIfNotExists(crest, VanillaReferences.AnimationLibraryNames.WANDERER, requiredAnimations);

                ConfigGroupNeedleforge config = (ConfigGroupNeedleforge)crest.Moveset.ConfigGroup!;

                activeNml = config.DashStab;
                activeAlt = config.DashStabAlt;
                activeRecoil = config.SpecialSlash!;
            }

            WandererNml.GetFirstActionOfType<SetGameObject>()!.gameObject = activeNml;
            WandererAlt.GetFirstActionOfType<SetGameObject>()!.gameObject = activeAlt;
            RecoilStabAttack.GetFirstActionOfType<SendMessage>()!.gameObject.GameObject = activeRecoil;
            RecoilStabBounce.GetFirstActionOfType<SendMessage>()!.gameObject.GameObject = activeRecoil;

        });
        #endregion

        #region Beast
        WarriorAntic.InsertMethod(0, _ =>
        {
            CrestData? crest = GetEquippedCrestIfOfDashslashType(VanillaAttackType.BEAST);
            if (crest != null)
            {
                string[] requiredAnimations = new[] { "Dash Attack Antic", "Dash Attack Leap",
                "Dash Attack Slash"};

                CloneAnimationsToIfNotExists(crest, VanillaReferences.AnimationLibraryNames.BEAST, requiredAnimations);                
            }
        });

        RageMode.AddMethod(_ =>
        {
            //It's nice when there's existing logic for this stuff.
            CrestData? crest = GetEquippedCrestIfOfDashslashType(VanillaAttackType.BEAST);
            if (crest != null)
            {
                GameObject activeSlash = crest.Moveset.ConfigGroup!.DashStab;
                fsm.GetGameObjectVariable("Current Attack").value = activeSlash;
            }
        });
        #endregion

        #region Shaman
        //Inserting to 1 past the existing SetGameObject
        ShamanAntic.InsertMethod(1 + Array.FindLastIndex(ShamanAntic.Actions, x => x is SetGameObject), _ =>
        {
            CrestData? crest = GetEquippedCrestIfOfDashslashType(VanillaAttackType.SHAMAN);
            if (crest != null)
            {
                string[] requiredAnimations = new[] { "Dash Attack Antic", "Dash Attack Leap",
                "Dash Attack Slash"};

                CloneAnimationsToIfNotExists(crest, VanillaReferences.AnimationLibraryNames.SHAMAN, requiredAnimations);

                GameObject activeSlash = crest.Moveset.ConfigGroup!.DashStab;
                fsm.GetGameObjectVariable("Current Attack").value = activeSlash;
            }
        });

        #endregion

        #region Architect

        DrillChargeStart.InsertMethod(0, _ =>
        {
            CrestData? crest = GetEquippedCrestIfOfDashslashType(VanillaAttackType.ARCHITECT);
            if (crest != null)
            {
                string[] requiredAnimations = new[] { "Dash Attack Charge", "Dash Attack"};
                CloneAnimationsToIfNotExists(crest, VanillaReferences.AnimationLibraryNames.ARCHITECT, requiredAnimations);
            }
        });

        SetUncharged.AddMethod(_ =>
        {
            CrestData? crest = GetEquippedCrestIfOfDashslashType(VanillaAttackType.ARCHITECT);
            if (crest != null)
            {
                GameObject activeSlash = crest.Moveset.ConfigGroup!.DashStab; //uncharged
                fsm.GetGameObjectVariable("Drill Stab Crt").value = activeSlash;
            }
        });

        SetCharged.AddMethod(_ =>
        {
            CrestData? crest = GetEquippedCrestIfOfDashslashType(VanillaAttackType.ARCHITECT);
            if (crest != null)
            {
                GameObject activeSlash = crest.Moveset.ConfigGroup!.DashStabAlt; //charged
                fsm.GetGameObjectVariable("Drill Stab Crt").value = activeSlash;
            }
        });

        #endregion

        #region witch
        //Should be fine to put here since future edits to Set Attack Multiple use insert, but keep in mind.
        SetAttackMultiple.AddMethod(_ =>
        {
            CrestData? crest = GetEquippedCrestIfOfDashslashType(VanillaAttackType.WITCH);
            if (crest != null)
            {
                string[] requiredAnimations = new[] { "Dash Attack Antic 1", "Dash Attack 1",
                "Dash Attack Recover", "Dash Attack Antic 2", "Dash Attack 2"};
                CloneAnimationsToIfNotExists(crest, VanillaReferences.AnimationLibraryNames.WITCH, requiredAnimations);
            }
        });

        ReactionType.AddMethod(_ =>
        {
            CrestData? crest = GetEquippedCrestIfOfDashslashType(VanillaAttackType.WITCH);
            if (crest != null)
            {
                //Once again, Witch is the problem child. There's an annoying amount of extra GameObjects.
                //I'm still not sure if this has no side effects, but I'll let it slide for now.
                hc.gameObject.transform.Find("Attacks").Find("Witch").gameObject.SetActive(true);

                fsm.SendEvent("WITCH");
            }
        });

        #endregion

        #endregion

        #region Default behaviour

        StartAttack.InsertMethod(
            1 + Array.FindLastIndex(StartAttack.Actions, x => x is SetIntValue),
            () => {
                crestIdx.Value = NeedleforgePlugin.newCrestData.FindIndex(x => x.IsEquipped);
                if (crestIdx.Value >= 0)
                {
                    var crest = NeedleforgePlugin.newCrestData[crestIdx.Value];
                    var attack = crest.Moveset.ConfigGroup!.DashStab.transform;
                    fsm.GetIntVariable("Attack Steps").Value = attack.childCount;

                    #region events switch case
                    //Here, in case of a cloned vanilla dash slash, we send the necessary event.
                    switch (crest.Moveset.UseVanillaDashSlash)
                    {
                        case VanillaAttackType.HUNTER:
                            //Needed to force hunter to only attack once.
                            fsm.GetIntVariable("Attack Steps").Value = 1;
                            fsm.SendEvent("SINGLE"); return;
                        case VanillaAttackType.WITCH:
                            fsm.SendEvent("MULTIPLE"); return;
                        case VanillaAttackType.BEAST:
                            fsm.SendEvent("WARRIOR"); return;
                        case VanillaAttackType.REAPER:
                            fsm.SendEvent("REAPER"); return;
                        case VanillaAttackType.WANDERER:
                            fsm.SendEvent("WANDERER"); return;
                        case VanillaAttackType.SHAMAN:
                            fsm.SendEvent("SHAMAN"); return;
                        case VanillaAttackType.ARCHITECT:
                            fsm.SendEvent("TOOLMASTER"); return;
                    }
                    #endregion

                    if (attack.childCount <= 0)
                        ModHelper.LogWarning($"{crest.name}: {nameof(MovesetData.DashSlash)} has no steps; the attack won't do anything.");
                    else if (crest.Moveset.HeroConfig!.dashStabSteps > attack.childCount)
                        ModHelper.LogWarning(
                            $"{crest.name}: The {nameof(MovesetData.HeroConfig)}.dashStabSteps " +
                            $"field has no effect for Needleforge crests. Modify the " +
                            $"{nameof(MovesetData.DashSlash)}.{nameof(Attacks.DashAttack.Steps)} array instead.");

                    // witch is our default path
                    fsm.Fsm.Event("MULTIPLE");
                }
            }
        );

        SetAttackMultiple.InsertMethod(
            Array.FindLastIndex(SetAttackMultiple.Actions, x => x is SetPolygonCollider),
            () => {
                if (crestIdx.Value >= 0)
                {
                    var crest = NeedleforgePlugin.newCrestData[crestIdx.Value];
                    var attack = crest.Moveset.ConfigGroup!.DashStab.transform;
                    int i = fsm.GetIntVariable("Attack Step").Value - 1;

                    fsm.GetStringVariable("Attack Child Name")
                        .Value = attack.GetChild(i).name;
                }
            }
        );

        FsmState AttackDashStart = fsm.GetState("Attack Dash Start")!;
        AttackDashStart.InsertMethod(
            0,
            () => {
                // Necessary to avoid breaking hunter and witch dash slashes if the custom
                // crest's dash attack is used before either of theirs after a save is loaded
                foreach(var callmethod in AttackDashStart.GetActionsOfType<CallMethodProper>())
                    if (
                        typeof(NailAttackBase).IsAssignableFrom(callmethod.cachedType)
                        && callmethod.cachedType != typeof(DashStabNailAttack)
                    ) {
                        callmethod.cachedType = null;
                        callmethod.cachedMethodInfo = null;
                        callmethod.cachedParameterInfo = [];
                    }
            }
        );
        AttackDashStart.InsertMethod(
            Array.FindIndex(AttackDashStart.Actions, x => x is PlayAudioEvent),
            () => {
                if (crestIdx.Value >= 0)
                {
                    var crest = NeedleforgePlugin.newCrestData[crestIdx.Value];
                    var attack = crest.Moveset.ConfigGroup!.DashStab.transform;
                    int i = fsm.GetIntVariable("Attack Step").Value - 1;
                    var audioSrc = attack.GetChild(i).GetComponent<AudioSource>();
                    if (audioSrc)
                        fsm.FsmVariables.FindFsmObject("Clip").Value = audioSrc.clip;
                }
            }
        );

        #endregion

        IEnumerable<MovesetData>
            movesets = NeedleforgePlugin.newCrestData
                .Select(cd => cd.Moveset)
                .Where(m => m.HeroConfig && m.HeroConfig.DashSlashFsmEdit != null);

        if (!movesets.Any())
            return;

        int equipCheckIndex = 1 + Array.FindLastIndex(StartAttack.Actions, x => x is CheckIfCrestEquipped);

        foreach(MovesetData m in movesets)
        {
            string name = m.Crest.name;
            ModHelper.Log($"{name} Dash Slash");

            FsmState AtkStart = fsm.AddState($"{name} Start");

            var equipCheckAction = CreateCrestEquipCheck(m.Crest);
            StartAttack.InsertAction(equipCheckIndex, equipCheckAction);
            StartAttack.AddTransition(equipCheckAction.trueEvent.Name, AtkStart.name);

            m.HeroConfig!.DashSlashFsmEdit!.Invoke(fsm, AtkStart, out FsmState[] AtkEnds);

            foreach(var end in AtkEnds)
                end.AddTransition("FINISHED", RegainControlNormal.Name);
        }
    }

    private static void ChargedSlashFSMEdits(HeroController hc)
    {
        PlayMakerFSM fsm = hc.gameObject.GetFsmPreprocessed("Nail Arts")!;

        FsmState
            AnticType = fsm.GetState("Antic Type")!,
            SetFinished = fsm.GetState("Set Finished")!,
            QueuedSpin = fsm.GetState("Queued Spin")!,
            BeginSpin = fsm.GetState("Begin Spin")!,
            DoSpin = fsm.GetState("Do Spin")!;

        #region Behaviour for copied vanilla charged slashes

        AnticType.AddMethod(_ =>
        {
            CrestData? equipped = NeedleforgePlugin.newCrestData.FirstOrDefault(x => x.IsEquipped);
            if (equipped != null)
            {
                switch (equipped.Moveset.UseVanillaChargedSlash)
                {
                    case VanillaAttackType.BEAST:
                        fsm.SendEvent("WARRIOR");
                        break;
                    case VanillaAttackType.REAPER:
                        fsm.SendEvent("REAPER");
                        break;
                    case VanillaAttackType.SHAMAN:
                        fsm.SendEvent("SHAMAN");
                        break;
                    case VanillaAttackType.ARCHITECT:
                        fsm.SendEvent("TOOLMASTER");
                        break;
                    case VanillaAttackType.WANDERER:
                        fsm.SendEvent("WANDERER");
                        break;
                    case VanillaAttackType.HUNTER:
                        fsm.SendEvent("FINISHED");
                        break;
                    case VanillaAttackType.WITCH:
                        fsm.SendEvent("FINISHED");
                        break;
                    //Cloakless is skipped because it doesn't have a charged slash.
                    //Default case cannot exist or it'll overwrite future custom events.
                }
            }
        });


        //An addition to the Do Spin state to allow shaman to work.
        ActivateGameObject? activateAction = DoSpin.GetFirstActionOfType<ActivateGameObject>();
        if (activateAction != null)
        {
            DoSpin.AddAction(new SendMessageV2
            {
                delivery = 0,
                options = SendMessageOptions.DontRequireReceiver,
                everyFrame = false,
                gameObject = activateAction.gameObject,
                functionCall = new FunctionCall()
                {
                    FunctionName = "OnSlashStarting"
                }
            });
        } else
        {
            ModHelper.LogError("Couldn't find ActivateGameObject action in DoSpin state. Copies of certain charged slashes may not work properly.");
        }
        #endregion

        #region Minor edits to Queued Spin and Begin Spin to prevent sound effect when not on Witch
        //Identical fix for both states
        void DisableWitchAudio(FsmState state)
        {
            PlayAudioEvent? playAudio = state.GetFirstActionOfType<PlayAudioEvent>();
            if (playAudio == null) { return; }

            playAudio.enabled = false;

            if (HeroController.instance.Config.name == "Witch")
                playAudio.enabled = true;

            CrestData? equipped = NeedleforgePlugin.newCrestData.FirstOrDefault(x => x.IsEquipped);
            if (equipped == null) { return; }

            if (equipped.Moveset.UseVanillaChargedSlash == VanillaAttackType.WITCH)
                playAudio.enabled = true;
        }
        QueuedSpin.InsertMethod(0, _ => DisableWitchAudio(QueuedSpin));
        BeginSpin.InsertMethod(0, _ => DisableWitchAudio(BeginSpin));
        #endregion

        #region Default behaviour

        FsmState Kickoff = fsm.AddState("Needleforge Kickoff");

        AnticType.AddMethod(() => {
            var crest = NeedleforgePlugin.newCrestData.FirstOrDefault(x => x.IsEquipped);
            if (
                crest != null
                && crest.Moveset.HeroConfig!.ChargedSlashFsmEdit == null
            ) {
                var attack = crest.Moveset.ChargedSlash?.GameObject;
                if (attack && attack.transform.childCount <= 0)
                    ModHelper.LogWarning($"{crest.name}: {nameof(MovesetData.ChargedSlash)} has no steps; the attack won't do anything.");
                fsm.Fsm.Event(needleforgeDefaultEvent);
            }
        });
        AnticType.AddTransition(needleforgeDefaultEvent.Name, Kickoff.Name);

        Kickoff.AddAction(new CheckIsCharacterGrounded
        {
            Target = new() { OwnerOption = OwnerDefaultOption.UseOwner },
            RayCount = new() { Value = 3 },
            GroundDistance = new() { Value = 0.2f },
            SkinWidth = new() { Value = -0.05f },
            SkinHeight = new() { Value = 0.1f },
            StoreResult = new() { Value = false },
            NotGroundedEvent = FsmEvent.GetFsmEvent("FINISHED"),
            EveryFrame = false,
        });
        Kickoff.AddMethod(() => {
            var hc = HeroController.instance;
            if (hc.Config is HeroConfigNeedleforge x && x.ChargedSlashDoesKickoff)
                hc.rb2d.linearVelocityY = 10;
        });
        Kickoff.AddTransition("FINISHED", "Antic");

        // Allow crests other than Shaman to have charged slash recoil
        fsm.GetState("Slash Recoil?")?.DisableActionsOfType<CheckIfCrestEquipped>();

        #endregion

        IEnumerable<MovesetData>
            movesets = NeedleforgePlugin.newCrestData
                .Select(cd => cd.Moveset)
                .Where(m => m.HeroConfig && m.HeroConfig.ChargedSlashFsmEdit != null);

        if (!movesets.Any())
            return;

        int equipCheckIndex = 1 + Array.FindLastIndex(AnticType.Actions, x => x is CheckIfCrestEquipped);

        foreach(MovesetData m in movesets)
        {
            string name = m.Crest.name;
            ModHelper.Log($"{name} Charged Slash");

            FsmState AtkStart = fsm.AddState($"{name} Start");

            var equipCheckAction = CreateCrestEquipCheck(m.Crest);
            AnticType.InsertAction(equipCheckIndex, equipCheckAction);
            AnticType.AddTransition(equipCheckAction.trueEvent.Name, AtkStart.name);

            m.HeroConfig!.ChargedSlashFsmEdit!.Invoke(fsm, AtkStart, out FsmState[] AtkEnds);

            foreach(var end in AtkEnds)
                end.AddTransition("FINISHED", SetFinished.Name);
        }
    }

    #region Utils

    private static readonly FsmEvent
        needleforgeDefaultEvent = FsmEvent.GetFsmEvent("NEEDLEFORGE DEFAULT"),
        noEvent = FsmEvent.GetFsmEvent("");

    private static CheckIfCrestEquipped CreateCrestEquipCheck(CrestData crest) =>
        new()
        {
            Crest = new FsmObject() { Value = crest.ToolCrest },
            trueEvent = FsmEvent.GetFsmEvent(crest.name),
            falseEvent = noEvent,
            storeValue = false,
        };

    private static void CloneAnimationTo(tk2dSpriteAnimation libraryToCloneTo, string libraryNameToCloneFrom, string cloneAnimationName)
    {
        foreach (HeroController.ConfigGroup configGroup in HeroController.instance.configs)
        {
            HeroControllerConfig config = configGroup.Config;
            if (config == null) { continue; }

            tk2dSpriteAnimation library = config.heroAnimOverrideLib;
            if (library == null) { continue; } // Ignore default, we should never have to clone from it.

            if (library.name != libraryNameToCloneFrom) { continue; }

            tk2dSpriteAnimationClip clip = library.GetClipByName(cloneAnimationName);
            if (clip == null)
            {
                ModHelper.LogError($"Animation {cloneAnimationName} not found in library {libraryToCloneTo}." +
                "Failed to make clone.");
            }

            tk2dSpriteAnimationClip clone = new tk2dSpriteAnimationClip();
            clone.CopyFrom(clip);

            List<tk2dSpriteAnimationClip> list = libraryToCloneTo.clips.ToList<tk2dSpriteAnimationClip>();
            list.Add(clone);

            libraryToCloneTo.clips = list.ToArray();
            libraryToCloneTo.isValid = false;
            libraryToCloneTo.ValidateLookup();
            ModHelper.Log($"Cloned animation {cloneAnimationName} from {libraryNameToCloneFrom} to {libraryToCloneTo.name}.");
        }
    }

    private static void CloneAnimationsToIfNotExists(CrestData crestToCloneTo, string libraryNameToCloneFrom, string[] cloneAnimationNames)
    {
        HeroConfigNeedleforge? config = crestToCloneTo.Moveset.HeroConfig;
        if (config == null) { return; }

        tk2dSpriteAnimation? library = config.heroAnimOverrideLib;
        if (library == null) {
            GameObject libobj = new GameObject($"{crestToCloneTo.name}LibraryObject");
            GameObject.DontDestroyOnLoad(libobj);

            library = libobj.AddComponent<tk2dSpriteAnimation>();
            library.clips = Array.Empty<tk2dSpriteAnimationClip>();
            config.heroAnimOverrideLib = library;

            ModHelper.Log($"Crest {crestToCloneTo.name} has no animation library, but requests to clone vanilla animations." +
                "Created new library and assigned to crest, though this should really be done by you.");
        }

        foreach (string anim in cloneAnimationNames)
        {
            if (library.GetClipByName(anim) != null) { continue; }

            ModHelper.Log($"Required animation {anim} for crest {crestToCloneTo.name} not found." +
                $"Cloning from vanilla {libraryNameToCloneFrom}.");
            CloneAnimationTo(library, libraryNameToCloneFrom, anim);
        }
    }
    
    private static CrestData? GetEquippedCrestIfOfDownslashType(VanillaAttackType type)
    {
        var crest = NeedleforgePlugin.newCrestData.FirstOrDefault(x => x.IsEquipped);
        if (
            crest != null
            && crest.Moveset.UseVanillaDownSlash == type)
        {
            return crest;
        }
        return null;
    }

    private static CrestData? GetEquippedCrestIfOfDashslashType(VanillaAttackType type)
    {
        var crest = NeedleforgePlugin.newCrestData.FirstOrDefault(x => x.IsEquipped);
        if (
            crest != null
            && crest.Moveset.UseVanillaDashSlash == type)
        {
            return crest;
        }
        return null;
    }
    #endregion

}
