using System;
using System.Collections.Generic;
using Assets.Scripts.Entity;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    class AbilityAstralbelebung : AbstractMagicAbility
    {
        public static readonly int VER_SINGLE_TARGET = 0;
        public static readonly string PRE = "<Astralbelebung>: "; // Log Prefix
        public static float maxRange = 30f;

        public override string name
        {
            get
            {
                return "Astralbelebung";
            }
        }

        public override void applyFailure(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets)
        {
            // TODO: well - just remove 1/2 of the mana costs? if critical, remove full mana costs
            int minutes = 0;

            int MaPcost = (luck == ELuck.slipup) ? minutes * 3 : (minutes * 3) / 2;
            user.drainMana(MaPcost);
        }

        public override void applySuccess(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets)
        {
            int IN = ((rp+1)/2 == rp/2) ? rp / 2 : (rp/2) + 1; // if rp is uneven, add +1 to rp/2
            int LO = rp / 2;
            Debug.Assert(IN + LO == rp, PRE + "calculation error: IN+LO == rp invalidated!");

            // TODO: animate object with these stats
            Debug.Assert(version == VER_SINGLE_TARGET, "verison bug?");
            int minutes = UnityEngine.Random.Range(1,10) * Constants.gameTimeMultiplier;
            DateTime endtime = DateTime.Now.AddMinutes(minutes);
            Conversations.InteractionComponent c = targets[0].AddInteractionComponent();

            Constants.gameLogic.AddTimedInteraction(c, endtime);
            Debug.Log("Astrahlbelebung. Object will come to life for " + minutes + "minutes: " + targets[0]);

            // drain costs from user
            int MaPcost = minutes * 3;
            user.drainMana(MaPcost);
        }

        /**
         * We need:
         * - at least 1 target
         * - target[s] must be liveless
         */
        public override bool canUse(int version, EntityController user, List<PraeObject> targets)
        {
            if (version == VER_SINGLE_TARGET)
                if (targets.Count == 1)
                    if (targets[0].inanimate)
                        if (targets[0].weight <= fw)
                            if (!targets[0].HasInteraction())
                                return true; // all conditions met
                            else
                                Debug.Log("Object already has interaction!");
                        else
                            Debug.Log(PRE + "Object is to heavy");
                    else
                        Debug.Log(PRE + "Object is alive!");
                else
                    Debug.Log(PRE + "Zero or more than 1 target given. Exactly 1 expected!");
            else
                Debug.LogError(PRE + "ERROR: Version " + version + " not existing!");

            return false;
        }

        public override int getTestModifier(int version, EntityController user, List<PraeObject> targets)
        {
            if (version == VER_SINGLE_TARGET)
                return 0;
            else
                throw new NotImplementedException();
        }

        public override void makeVisuals(int version, int rp, ELuck luck, EntityController user, List<PraeObject> targets)
        {
            //throw new NotImplementedException();
        }

        public override void useOverride()
        {

        }

        public override List<PraeObject> getTargets(int version, EntityController user)
        {
            if (version == VER_SINGLE_TARGET)
            {
                List<PraeObject> targets = null;
                RaycastHit hitinfo;
                Debug.DrawLine(user.transform.position, user.transform.position + user.lookDir * maxRange, Color.green, 10f);
                if (Physics.Raycast(user.transform.position, user.lookDir, out hitinfo, 30f))
                {
                    PraeObject obj = hitinfo.transform.gameObject.GetComponent<PraeObject>();
                    if (obj)
                    {
                        targets = new List<PraeObject>(2);
                        targets.Add(obj);
                        Debug.Log("Found target: " + obj);
                    }
                    else
                    {
                        Debug.Log("Could not find targets in range.");
                    }
                }

                return targets;
            }
            else
                throw new NotImplementedException();
        }
    }
}
