﻿using UnityEngine;
using CheatManagerZero.Configuration;
using BZHelper;

namespace CheatManagerZero.NewCommands
{
    public class OverPowerConsoleCommand : MonoBehaviour
    {
        public static OverPowerConsoleCommand main;
        private bool OverPowerCheat;
        private float overPowerMultiplier;
        private float hungerAndThristInterval;
        private const float defaultOxygenCapacity = 45f;
        private const float defaultHealth = 100f;

        public void Awake()
        {
            main = this;
            DontDestroyOnLoad(this);
            DevConsole.RegisterConsoleCommand(this, "overpower", false, false);
        }

        private void OnConsoleCommand_overpower(NotificationCenter.Notification n)
        {
            OverPowerCheat = !OverPowerCheat;

            if (OverPowerCheat)
            {
                overPowerMultiplier = CMZ_Config.overPowerMultiplier;
                hungerAndThristInterval = CMZ_Config.hungerAndThirstInterval;
            }
            else
            {
                overPowerMultiplier = 1;
                hungerAndThristInterval = 10;
            }

            SetOverPowerCheat();
            ErrorMessage.AddMessage($"overpower cheat is now {OverPowerCheat}\nOverPower Multiplier: {overPowerMultiplier}x\nHunger and thirst interval set to: {hungerAndThristInterval}");
        }

        public bool GetOverPowerCheat()
        {
            return OverPowerCheat;
        }

        internal void SetOverPowerCheat()
        {
            Oxygen o2 = Player.main.GetComponent<OxygenManager>().GetComponent<Oxygen>();

            Player.main.GetComponent<Survival>().SetPrivateField("kUpdateHungerInterval", hungerAndThristInterval);

            if (o2.isPlayer)
            {
                o2.oxygenCapacity = 45 * overPowerMultiplier;
            }

            Player.main.liveMixin.data.maxHealth = 100 * overPowerMultiplier;
            Player.main.liveMixin.health = 100 * overPowerMultiplier;

        }

    }
}
