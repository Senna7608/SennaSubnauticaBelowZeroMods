﻿using System;
using System.Reflection;
using UnityEngine;
using Harmony;
using SeaTruckArms.ArmPrefabs;

namespace SeaTruckArms
{
    public static class Main
    {        
        public static SeatruckArms_Graphics graphics;

        public static bool isGraphicsReady = false;

        public static void Load()
        {
            try
            {
                if (!isGraphicsReady)
                {
                    graphics = new SeatruckArms_Graphics();
                }

                new SeaTruckDrillArmPrefab().Patch();
                new SeaTruckClawArmPrefab().Patch();
                new SeaTruckGrapplingArmPrefab().Patch();                
                new SeaTruckTorpedoArmPrefab().Patch();

                HarmonyInstance.Create("BelowZero.SeaTruckArms.mod").PatchAll(Assembly.GetExecutingAssembly());                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }        
    }
}