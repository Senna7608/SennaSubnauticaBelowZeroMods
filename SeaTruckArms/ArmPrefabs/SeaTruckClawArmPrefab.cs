using System.Collections.Generic;
using UnityEngine;
using SMLHelper.V2.Crafting;
using BZCommon.Helpers.SMLHelpers;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckClawArmPrefab : CraftableModItem
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckClawArmPrefab()
            : base(nameID: "SeaTruckClawArmModule",
                  iconFilePath: null,
                  iconTechType: TechType.ExosuitClawArmModule,
                  friendlyName: "Seatruck claw arm",
                  description: "Allows Seatruck to use claw arm.",
                  template: TechType.ExosuitDrillArmModule,                  
                  newTabNode: new TabNode(CraftTree.Type.Workbench, "SeaTruckWBUpgrades", "Seatruck Upgrades", SpriteManager.Get(TechType.SeaTruck)),
                  fabricatorTypes: new CraftTree.Type[] { CraftTree.Type.SeaTruckFabricator, CraftTree.Type.SeamothUpgrades, CraftTree.Type.Workbench },
                  fabricatorTabs: new string[][] { new string[] { "Upgrades" }, new string[] { "SeaTruckUpgrade" }, new string[] { "SeaTruckWBUpgrades" } },
                  requiredAnalysis: TechType.Exosuit,
                  groupForPDA: TechGroup.VehicleUpgrades,
                  categoryForPDA: TechCategory.VehicleUpgrades,
                  equipmentType: (EquipmentType)200,
                  quickSlotType: QuickSlotType.Selectable,
                  backgroundType: CraftData.BackgroundType.ExosuitArm,
                  itemSize: new Vector2int(1, 1),
                  gamerResourceFileName: null
                  )
        {
        }

        public override void Patch()
        {
            base.Patch();
            TechTypeID = TechType;
        }

        protected override RecipeData GetRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[3]
                {
                    new Ingredient(TechType.TitaniumIngot, 2),
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.ComputerChip, 1)
                })
            };
        }

        public override GameObject GetGameObject()
        {
            base.GetGameObject();

            var exosuit = Main.graphics.ExosuitResource;

            GameObject armPrefab = null;

            for (int i = 0; i < exosuit.armPrefabs.Length; i++)
            {
                if (exosuit.armPrefabs[i].techType == TechType.ExosuitClawArmModule)
                {
                    armPrefab = exosuit.armPrefabs[i].prefab;
                    break;
                }
            }
            
            SkinnedMeshRenderer smr = armPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
            Mesh clawMesh = smr.sharedMesh;

            MeshFilter mf = _GameObject.GetComponentInChildren<MeshFilter>();
            mf.sharedMesh = Object.Instantiate(clawMesh);
            mf.sharedMesh.name = "seamoth_hand_geo";

            MeshRenderer mr = _GameObject.GetComponentInChildren<MeshRenderer>();
            mr.materials = (Material[])smr.materials.Clone();

            _GameObject.transform.Find("model/exosuit_rig_armLeft:exosuit_drill_geo").gameObject.name = "seatruck_claw_arm_geo";

            Object.Destroy(_GameObject.GetComponentInChildren<CapsuleCollider>());

            BoxCollider bc_1 = _GameObject.FindChild("collider").AddComponent<BoxCollider>();

            bc_1.size = new Vector3(1.29f, 0.33f, 0.42f);
            bc_1.center = new Vector3(-0.53f, 0f, 0.04f);

            GameObject collider2 = new GameObject("collider2");
            collider2.transform.SetParent(_GameObject.transform, false);
            collider2.transform.localPosition = new Vector3(-1.88f, 0.07f, 0.50f);
            collider2.transform.localRotation = Quaternion.Euler(0, 34, 0);

            BoxCollider bc_2 = collider2.AddComponent<BoxCollider>();
            bc_2.size = new Vector3(1.06f, 0.23f, 0.31f);
            bc_2.center = new Vector3(0, -0.08f, 0);

            _GameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            
            return _GameObject;
        }
    }
}
