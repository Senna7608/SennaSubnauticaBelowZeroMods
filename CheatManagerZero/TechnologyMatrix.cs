using System;
using System.Collections.Generic;
using BZCommon.Helpers;
using BZHelper;
using BZHelper.NautilusHelpers;

namespace CheatManagerZero
{
    public enum TechCategory
    {
        Vehicles,
        Tools,
        Equipment,
        Materials,
        Electronics,
        Upgrades,
        FoodAndWater,
        LootAndDrill,
        Herbivores,
        Carnivores,
        Parasites,
        Leviathan,
        Eggs,
        SeaSeed,
        LandSeed,
        FloraItem,
        SeaSpawn,
        LandSpawn,
        Blueprints,
        Warp,
        BaseModule,
    };      

    public class TechnologyMatrix
    {
        public class TechTypeSearch
        {
            readonly TechType _techType;

            public TechTypeSearch(TechType techType)
            {
                _techType = techType;
            }

            public bool EqualsWith(TechTypeData techTypeData)
            {
                return techTypeData.TechType == _techType;
            }
        }

        public int GetTotalCount(ref List<TechTypeData>[] technologyMatrix)
        {
            int count = 0;

            foreach (List<TechTypeData> entry in technologyMatrix)
            {
                count += entry.Count;
            }

            return count;
        }

        public void InitTechMatrixList(ref List<TechTypeData>[] TechnologyMatrix)
        {
            int i = 0;

            foreach (KeyValuePair<TechCategory, List<TechType>> kvp in baseTechMatrix)
            {
                if (Enum.IsDefined(typeof(Categories), (int)kvp.Key))
                {                    
                    TechnologyMatrix[i] = new List<TechTypeData>();

                    for (int j = 0; j < kvp.Value.Count; j++)
                    {
                        string name;
                        TechType techType = kvp.Value[j];

                        name = Language.main.Get(TechTypeExtensions.AsString(kvp.Value[j], false));

                        TechnologyMatrix[i].Add(new TechTypeData(techType, name));
                    }

                    i++;
                }                
            }

            BZLogger.Debug("Base Tech matrix created.");
        }

        public void InitializeBlueprints()
        {
            HashSet<TechType> unlockables = KnownTech.GetAllUnlockables();

            int moddedCount = 0;

            foreach (TechType techType in unlockables)
            {
                baseTechMatrix[TechCategory.Blueprints].Add(techType);

                if (ModdedTechTypeHelper.Main.IsModdedTechTypeExists(techType))
                {
                    BZLogger.Debug($"Modded blueprint found for techtype: [{techType}]");

                    moddedCount++;
                }
            }

            if (moddedCount > 0)
            {
                BZLogger.Log($"Found [{moddedCount}] modded blueprint(s).");
                BZLogger.Log($"Dynamic blueprint list created and expanded with modded blueprint(s). Number of entries: [{unlockables.Count}]");
            }

        }

        public void SortTechLists(ref List<TechTypeData>[] TechnologyMatrix)
        {
            foreach (List<TechTypeData> item in TechnologyMatrix)
            {
                item.Sort();
            }
        }

        public void GetModdedTechTypes(ref List<TechTypeData>[] TechnologyMatrix)
        {
            foreach (KeyValuePair<string, TechType> kvp in ModdedTechTypeHelper.Main.ModdedTechTypes)
            {
                EquipmentType equipmentType = ModdedTechTypeHelper.Main.TypeDefCache[kvp.Value];

                switch (equipmentType)
                {                    
                    case EquipmentType.ExosuitArm:
                    case EquipmentType.ExosuitModule:
                    case EquipmentType.HoverbikeModule:
                    case EquipmentType.SeamothModule:
                    case EquipmentType.SeaTruckModule:
                    case EquipmentType.VehicleModule:
                    case (EquipmentType)200:
                        TechnologyMatrix[(int)TechCategory.Upgrades].Add(new TechTypeData(kvp.Value, Language.main.Get(TechTypeExtensions.AsString(kvp.Value, false))));
                        break;                    
                    case EquipmentType.Body:
                    case EquipmentType.Chip:
                    case EquipmentType.Foots:
                    case EquipmentType.Gloves:                    
                    case EquipmentType.Head:
                    case EquipmentType.Tank:                        
                        TechnologyMatrix[(int)TechCategory.Equipment].Add(new TechTypeData(kvp.Value, Language.main.Get(TechTypeExtensions.AsString(kvp.Value, false))));
                        break;

                    case EquipmentType.Hand:
                        TechnologyMatrix[(int)TechCategory.Tools].Add(new TechTypeData(kvp.Value, Language.main.Get(TechTypeExtensions.AsString(kvp.Value, false))));
                        break;

                    case EquipmentType.BatteryCharger:
                    case EquipmentType.PowerCellCharger:
                        TechnologyMatrix[(int)TechCategory.Electronics].Add(new TechTypeData(kvp.Value, Language.main.Get(TechTypeExtensions.AsString(kvp.Value, false))));
                        break;
                }

                if (kvp.Key.Equals("SeaTruckScannerModule"))
{
                    TechnologyMatrix[(int)TechCategory.Vehicles].Add(new TechTypeData(kvp.Value, Language.main.Get(TechTypeExtensions.AsString(kvp.Value, false))));
                }
            }

            if (ModdedTechTypeHelper.Main.ModdedTechTypes.Count > 0)
            {
                BZLogger.Log($"Found [{ModdedTechTypeHelper.Main.ModdedTechTypes.Count}] modded TechType(s).");
                BZLogger.Log($"Dynamic techmatrix created. Modded TechType(s) added to appropriate categories. Number of entries: [{GetTotalCount(ref TechnologyMatrix)}]");
            }
        }
        
        public readonly Dictionary<TechCategory, List<TechType>> baseTechMatrix = new Dictionary<TechCategory, List<TechType>>()
        {
            #region Vehicles
            {
                TechCategory.Vehicles,

                new List<TechType>()
                {                    
                    TechType.Exosuit,
                    TechType.SeaTruck,
                    TechType.SeaTruckAquariumModule,
                    TechType.SeaTruckDockingModule,
                    TechType.SeaTruckFabricatorModule,
                    //TechType.SeaTruckPlanterModule,
                    TechType.SeaTruckSleeperModule,
                    TechType.SeaTruckStorageModule,
                    TechType.SeaTruckTeleportationModule,
                    TechType.Hoverbike
                }
            },
            #endregion

            #region Tools
            {
                TechCategory.Tools,

                new List<TechType>()
                {                    
                    TechType.Thumper,                    
                    TechType.SpyPenguin,
                    TechType.Knife,                    
                    TechType.HeatBlade,
                    TechType.Flashlight,
                    TechType.Beacon,
                    TechType.Builder,
                    TechType.AirBladder,                    
                    TechType.DiveReel,
                    TechType.Scanner,                    
                    TechType.PipeSurfaceFloater,
                    TechType.Welder,
                    TechType.Seaglide,
                    TechType.Constructor,                    
                    TechType.Flare,                    
                    TechType.PropulsionCannon,                    
                    TechType.Gravsphere,
                    TechType.SmallStorage,
                    TechType.LaserCutter,
                    TechType.LEDLight,
                    TechType.TeleportationTool,
                    TechType.SpyPenguinRemote,
                    TechType.QuantumLocker,
                    TechType.MetalDetector

                }
            },
            #endregion

            #region Equipment
            {
                TechCategory.Equipment,

                new List<TechType>()
                {                    
                    TechType.ReinforcedDiveSuit,
                    TechType.ReinforcedGloves,                    
                    TechType.Fins,
                    TechType.UltraGlideFins,
                    TechType.SwimChargeFins,
                    TechType.Tank,
                    TechType.DoubleTank,
                    TechType.PlasteelTank,
                    TechType.HighCapacityTank,
                    TechType.Rebreather,
                    TechType.Compass,
                    TechType.MapRoomHUDChip,
                    TechType.WhirlpoolTorpedo,
                    TechType.GasTorpedo,
                    TechType.MetalDetector,
                    TechType.FlashlightHelmet,
                    TechType.ColdSuit,
                    TechType.ColdSuitGloves,
                    TechType.ColdSuitHelmet,
                    TechType.SuitBoosterTank                    
                }
            },
            #endregion

            #region Materials
            {
                TechCategory.Materials,

                new List<TechType>()
                {
                    TechType.Aerogel,
                    TechType.AluminumOxide,
                    TechType.AramidFibers,
                    TechType.Benzene,                    
                    TechType.Copper,
                    TechType.CrashPowder,
                    TechType.Diamond,
                    TechType.EnameledGlass,
                    TechType.FiberMesh,
                    TechType.Glass,
                    TechType.Gold,                    
                    TechType.HydrochloricAcid,
                    TechType.JeweledDiskPiece,
                    TechType.Kyanite,
                    TechType.Lead,
                    TechType.Lithium,
                    TechType.Lubricant,                    
                    TechType.Magnetite,                    
                    TechType.Nickel,
                    TechType.PlasteelIngot,
                    TechType.Polyaniline,
                    TechType.PrecursorIonCrystal,
                    TechType.Quartz,                    
                    TechType.Salt,
                    TechType.ScrapMetal,                    
                    TechType.Silicone,
                    TechType.Silver,                    
                    TechType.Sulphur,
                    TechType.Titanium,                    
                    TechType.TitaniumIngot,                                      
                    TechType.UraniniteCrystal,
                    TechType.HydraulicFluid
                }
            },
            #endregion

            #region Electronics
            {
                TechCategory.Electronics,

                new List<TechType>()
                {
                    TechType.AdvancedWiringKit,
                    TechType.Battery,
                    TechType.ComputerChip,
                    TechType.CopperWire,
                    TechType.DepletedReactorRod,                    
                    TechType.PowerCell,
                    TechType.PrecursorIonBattery,                    
                    TechType.PrecursorIonPowerCell,                    
                    TechType.RadioTowerPPU,
                    TechType.RadioTowerTOM,
                    TechType.ReactorRod,
                    TechType.WiringKit
                }
            },
            #endregion

            #region Upgrades
            {
                TechCategory.Upgrades,

                new List<TechType>()
                {   
                    TechType.SeaTruckUpgradeHull1,
                    TechType.SeaTruckUpgradeHull2,
                    TechType.SeaTruckUpgradeHull3,
                    TechType.SeaTruckUpgradePerimeterDefense,                                     
                    TechType.SeaTruckUpgradeEnergyEfficiency,
                    TechType.SeaTruckUpgradeHorsePower,
                    TechType.SeaTruckUpgradeAfterburner,
                    TechType.HoverbikeJumpModule,
                    TechType.HoverbikeIceWormReductionModule,
                    TechType.MapRoomUpgradeScanRange,
                    TechType.MapRoomUpgradeScanSpeed,                    
                    TechType.LootSensorFragment,                    
                    TechType.ExosuitJetUpgradeModule,
                    TechType.ExosuitDrillArmModule,
                    TechType.ExosuitThermalReactorModule,
                    TechType.ExosuitClawArmModule,
                    TechType.ExosuitPropulsionArmModule,
                    TechType.ExosuitGrapplingArmModule,
                    TechType.ExosuitTorpedoArmModule,
                    TechType.ExoHullModule1,
                    TechType.ExoHullModule2                    
                }
            },
            #endregion
            
            #region Food and Water
            {
                TechCategory.FoodAndWater,

                new List<TechType>()
                {
                    TechType.SpicyFruitSalad,
                    TechType.NutrientBlock,                    
                    TechType.Coffee,
                    TechType.FirstAidKit,
                    TechType.FilteredWater,
                    TechType.DisinfectedWater,                    
                    TechType.BigFilteredWater,                    
                    TechType.CookedArcticPeeper,
                    TechType.CookedArrowRay,
                    TechType.CookedNootFish,
                    TechType.CookedSymbiote,                    
                    TechType.CookedBladderfish,                    
                    TechType.CookedBoomerang,                    
                    TechType.CookedHoopfish,
                    TechType.CookedSpinefish,
                    TechType.CookedSpinnerfish,
                    TechType.CookedTriops,
                    TechType.CookedFeatherFish,
                    TechType.CookedFeatherFishRed,
                    TechType.CookedDiscusFish,                    
                    TechType.CuredArcticPeeper,
                    TechType.CuredArrowRay,
                    TechType.CuredNootFish,
                    TechType.CuredSymbiote,                    
                    TechType.CuredBladderfish,                    
                    TechType.CuredBoomerang,                    
                    TechType.CuredHoopfish,
                    TechType.CuredSpinefish                    
                }
            },
            #endregion

            #region Loot and Drill
            {
                TechCategory.LootAndDrill,

                new List<TechType>()
                {
                    TechType.LimestoneChunk,                                    
                    TechType.DrillableSalt,
                    TechType.DrillableQuartz,
                    TechType.DrillableCopper,
                    TechType.DrillableTitanium,
                    TechType.DrillableLead,
                    TechType.DrillableSilver,
                    TechType.DrillableDiamond,
                    TechType.DrillableGold,
                    TechType.DrillableMagnetite,
                    TechType.DrillableLithium,
                    TechType.DrillableMercury,
                    TechType.DrillableUranium,
                    TechType.DrillableAluminiumOxide,
                    TechType.DrillableNickel,
                    TechType.DrillableSulphur,
                    TechType.DrillableKyanite
                }
            },
            #endregion

            #region Herbivores
            {
                TechCategory.Herbivores,

                new List<TechType>()
                {                    
                    TechType.TitanHolefish,
                    TechType.SeaMonkey,                    
                    TechType.Penguin,
                    TechType.PenguinBaby,
                    TechType.ArcticPeeper,
                    TechType.ArcticRay,                    
                    TechType.Skyray,                    
                    TechType.Boomerang,                    
                    TechType.Bladderfish,                   
                    TechType.Hoopfish,
                    TechType.HoopfishSchool,                    
                    TechType.Spinefish                    
                }
            },
            #endregion

            #region Carnivores
            {
                TechCategory.Carnivores,

                new List<TechType>()
                {
                    TechType.Brinewing, 
                    TechType.Symbiote,                    
                    TechType.Snowman,
                    TechType.RockPuncher,
                    TechType.Brinicle,
                    TechType.BruteShark,                    
                    TechType.Crash                    
                }
            },
            #endregion

            #region Parasites
            {
                TechCategory.Parasites,

                new List<TechType>()
                {                    
                    TechType.Rockgrub,                    
                    TechType.PrecursorDroid
                }
            },
            #endregion

            #region Leviathan
            {
                TechCategory.Leviathan,

                new List<TechType>()
                {    
                    TechType.ShadowLeviathan,                    
                    TechType.GlowWhale,
                    TechType.Chelicerate,                    
                    TechType.SeaEmperorJuvenile                    
                }
            },
            #endregion

            #region Eggs
            {
                TechCategory.Eggs,

                new List<TechType>()
                {                    
                    TechType.ShockerEgg                    
                }
            },
            #endregion

            #region Sea: Seed
            {
                TechCategory.SeaSeed,

                new List<TechType>()
                {                   
                    TechType.PurpleStalkSeed,                    
                    TechType.RedBushSeed,                   
                    TechType.CreepvineSeedCluster
                }
            },
            #endregion

            #region Land: Seed
            {
                TechCategory.LandSeed,

                new List<TechType>()
                {                    
                    TechType.PurpleVegetable,
                    TechType.MelonSeed                    
                }
            },
            #endregion

            #region Flora: Item
            {
                TechCategory.FloraItem,

                new List<TechType>()
                {
                    TechType.GenericRibbon,
                    TechType.JellyPlant,                    
                    TechType.JeweledDiskPiece,                    
                    TechType.JellyPlantSeed,                    
                    TechType.HangingFruit,
                    TechType.SmallMelon,
                    TechType.Melon                    
                }
            },
            #endregion

            #region Sea: Spawn
            {
                TechCategory.SeaSpawn,

                new List<TechType>()
                {
                    TechType.BlueLostRiverLilly,                    
                    TechType.PurpleTentacle,                    
                    TechType.BigCoralTubes,
                    TechType.TreeMushroom,
                    TechType.BloodRoot,
                    TechType.BloodVine,                    
                    TechType.EyesPlant,
                    TechType.RedGreenTentacle,
                    TechType.PurpleStalk,
                    TechType.RedBasketPlant,
                    TechType.RedBush,                    
                    TechType.ShellGrass,
                    TechType.SpottedLeavesPlant,                    
                    TechType.PurpleBranches,                    
                    TechType.GenericJeweledDisk,                   
                    TechType.BallClusters,
                    TechType.BarnacleSuckers,
                    TechType.BlueBarnacle,
                    TechType.BlueBarnacleCluster,
                    TechType.BlueCoralTubes,
                    TechType.RedGrass,
                    TechType.GreenGrass,
                    TechType.Mohawk,
                    TechType.GreenReeds,
                    TechType.RedSeaweed,
                    TechType.CoralShellPlate,
                    TechType.BlueCluster,
                    TechType.BrownTubes,
                    TechType.BloodGrass,
                    TechType.FloatingStone,
                    TechType.BlueAmoeba,
                    TechType.RedTipRockThings,
                    TechType.BlueTipLostRiverPlant,
                    TechType.BlueLostRiverLilly,
                    TechType.HangingStinger,
                    TechType.BrainCoral,
                    TechType.CoveTree
                }
            },
            #endregion

            #region Land: Spawn
            {
                TechCategory.LandSpawn,

                new List<TechType>()
                {
                    TechType.PinkFlower,                    
                    TechType.FernPalm,
                    TechType.HangingFruitTree,
                    TechType.PurpleVegetablePlant,
                    TechType.MelonPlant,
                    TechType.OrangePetalsPlant
                }
            },
            #endregion

            #region Blueprints
            {
                TechCategory.Blueprints,

                new List<TechType>()                
            }
            #endregion
        };        
    }
}
