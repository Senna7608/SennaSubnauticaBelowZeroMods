using BZCommon.Helpers.SMLHelpers;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArms.API
{
    public abstract class SeaTruckArmFragment : ModPrefab_Fragment
    {
        private static readonly Dictionary<ArmFragmentTemplate, string> ArmFragmentTypes = new Dictionary<ArmFragmentTemplate, string>()
        {
            { ArmFragmentTemplate.ClawArm, "WorldEntities/Alterra/Fragments/ExosuitDrillArmfragment.prefab" },
            { ArmFragmentTemplate.DrillArm, "WorldEntities/Alterra/Fragments/ExosuitDrillArmfragment.prefab" },
            { ArmFragmentTemplate.GrapplingArm, "WorldEntities/Alterra/Fragments/ExosuitGrapplingArmfragment.prefab" },
            { ArmFragmentTemplate.PropulsionArm, "WorldEntities/Alterra/Fragments/ExosuitPropulsionArmfragment.prefab" },
            { ArmFragmentTemplate.TorpedoArm, "WorldEntities/Alterra/Fragments/ExosuitTorpedoArmfragment.prefab" }
            
        };       
        
        public ArmFragmentTemplate ArmFragmentTemplate { get; private set; }        

        protected SeaTruckArmFragment(
            string techTypeName,
            string friendlyName,
            ArmFragmentTemplate fragmentTemplate,            
            LargeWorldEntity.CellLevel cellLevel = LargeWorldEntity.CellLevel.Medium,
            float scanTime = 3,
            int totalFragments = 3            
            )
            : base(
        
            techTypeName,
            friendlyName,
            template: TechType.None,
            prefabFilePath: ArmFragmentTypes[fragmentTemplate],
            slotType: EntitySlot.Type.Small,
            prefabZUp: false,
            cellLevel: cellLevel,
            localScale: new Vector3(0.8f, 0.8f, 0.8f),            
            scanTime: scanTime,
            totalFragments: totalFragments,
            destroyAfterScan: true            
            )
        {
            ArmFragmentTemplate = fragmentTemplate;
        }

        protected override void ModifyGameObject()
        {
            
            if (ArmFragmentTemplate == ArmFragmentTemplate.ClawArm)
            {
                Main.graphics.ArmsCache.TryGetValue(ArmTemplate.ClawArm, out GameObject armPrefab);

                SkinnedMeshRenderer smr = armPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
                Mesh clawMesh = smr.sharedMesh;

                MeshFilter mf = GameObjectClone.GetComponentInChildren<MeshFilter>();
                mf.sharedMesh = Object.Instantiate(clawMesh);
                mf.sharedMesh.name = "seatruck_hand_geo";

                MeshRenderer mr = GameObjectClone.GetComponentInChildren<MeshRenderer>();
                mr.materials = (Material[])smr.materials.Clone();
            }            

            GameObjectClone.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            PostModify();
        }

        protected abstract void PostModify();
    }
}
