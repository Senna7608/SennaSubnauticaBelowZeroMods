using System;
using System.Collections.Generic;
using BZCommon.Helpers;
using BZCommon.Helpers.SMLHelpers;

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
        ALLTECH,
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


        public void InitFullTechMatrixList(ref List<TechTypeData> TechMatrix)
        {
            int[] techTypeArray = (int[])Enum.GetValues(typeof(TechType));

            for (int i = 0; i < techTypeArray.Length; i++)
            {
                TechType techType = (TechType)techTypeArray[i];

                string name = Language.main.Get(TechTypeExtensions.AsString((TechType)techTypeArray[i], false));

                TechMatrix.Add(new TechTypeData(techType, name));
            }
        }        
        
        public void InitTechMatrixList(ref List<TechTypeData>[] TechnologyMatrix)
        {
            int i = 0;

            foreach (KeyValuePair<TechCategory, TechType[]> kvp in baseTechMatrix)
            {
                if (Enum.IsDefined(typeof(Categories), (int)kvp.Key))
                {                    
                    TechnologyMatrix[i] = new List<TechTypeData>();

                    for (int j = 0; j < kvp.Value.Length; j++)
                    {
                        string name;
                        TechType techType = kvp.Value[j];

                        name = Language.main.Get(TechTypeExtensions.AsString(kvp.Value[j], false));

                        TechnologyMatrix[i].Add(new TechTypeData(techType, name));
                    }

                    i++;
                }                
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
            ModdedTechTypeHelper mHelper = new ModdedTechTypeHelper();

            foreach (KeyValuePair<string, TechType> kvp in mHelper.FoundModdedTechTypes)
            {
                EquipmentType equipmentType = mHelper.TypeDefCache[kvp.Value];

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

        }
        
        public static readonly Dictionary<TechCategory, TechType[]> baseTechMatrix = new Dictionary<TechCategory, TechType[]>()
        {
            #region Vehicles
            {
                TechCategory.Vehicles,

                new TechType[]
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

                new TechType[]
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

                new TechType[]
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

                new TechType[]
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

                new TechType[]
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

                new TechType[]
                {   
                    TechType.SeaTruckUpgradeHull1,
                    TechType.SeaTruckUpgradeHull2,
                    TechType.SeaTruckUpgradeHull3,
                    TechType.SeaTruckUpgradePerimeterDefense,
                    TechType.SeaTruckUpgradeThruster,                    
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

                new TechType[]
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

                new TechType[]
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

                new TechType[]
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

                new TechType[]
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

                new TechType[]
                {                    
                    TechType.Rockgrub,                    
                    TechType.PrecursorDroid
                }
            },
            #endregion

            #region Leviathan
            {
                TechCategory.Leviathan,

                new TechType[]
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

                new TechType[]
                {                    
                    TechType.ShockerEgg                    
                }
            },
            #endregion

            #region Sea: Seed
            {
                TechCategory.SeaSeed,

                new TechType[]
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

                new TechType[]
                {                    
                    TechType.PurpleVegetable,
                    TechType.MelonSeed                    
                }
            },
            #endregion

            #region Flora: Item
            {
                TechCategory.FloraItem,

                new TechType[]
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

                new TechType[]
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

                new TechType[]
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

                new TechType[]
                {
                    TechType.RocketBaseLadder,
                    TechType.RocketStage1,
                    TechType.RocketStage2,
                    TechType.RocketStage3
                }
            }
            #endregion
        };        
    }
}
