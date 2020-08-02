/*
using BZCommon;
using UnityEngine;

namespace SeaTruckArms.ArmControls
{
    public class SeaTruckPropulsionArmControl : MonoBehaviour, ISeaTruckArm
    {
        public SeaTruck ThisSeaTruck
        {
            get
            {
                return GetComponentInParent<SeaTruck>();
            }
        }

        public PropulsionCannon propulsionCannon;

        private bool usingTool;

        private void Start()
        {
            ThisSeaTruck.gameObject.AddIfNeedComponent<ImmuneToPropulsioncannon>();
            propulsionCannon = GetComponent<PropulsionCannon>();
            propulsionCannon.energyInterface = ThisSeaTruck.GetComponent<EnergyInterface>();
            propulsionCannon.shootForce = 60f;
            propulsionCannon.attractionForce = 145f;
            propulsionCannon.massScalingFactor = 0.005f;
            propulsionCannon.pickupDistance = 25f;
            propulsionCannon.maxMass = 1800f;
            propulsionCannon.maxAABBVolume = 400f;
        }


        GameObject ISeaTruckArm.GetGameObject()
        {
            return gameObject;
        }

        void ISeaTruckArm.SetSide(Arm arm)
        {
            if (propulsionCannon == null)
            {
                propulsionCannon = GetComponent<PropulsionCannon>();
            }

            if (arm == Arm.Right)
            {
                transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
                propulsionCannon.localObjectOffset = new Vector3(0.75f, 0f, 0f);
            }
            else
            {
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                propulsionCannon.localObjectOffset = new Vector3(-0.75f, 0f, 0f);
            }
        }

        bool ISeaTruckArm.OnUseDown(out float cooldownDuration)
        {
            usingTool = true;
            cooldownDuration = 1f;
            return propulsionCannon.OnShoot();
        }

        bool ISeaTruckArm.OnUseHeld(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return false;
        }

        bool ISeaTruckArm.OnUseUp(out float cooldownDuration)
        {
            usingTool = false;
            cooldownDuration = 0f;
            return true;
        }

        bool ISeaTruckArm.OnAltDown()
        {
            if (propulsionCannon.IsGrabbingObject())
            {
                propulsionCannon.ReleaseGrabbedObject();
            }

            return true;
        }

        void ISeaTruckArm.Update(ref Quaternion aimDirection)
        {
            propulsionCannon.usingCannon = usingTool;
            propulsionCannon.UpdateActive();
        }

        void ISeaTruckArm.Reset()
        {
            propulsionCannon.usingCannon = (usingTool = false);
            propulsionCannon.ReleaseGrabbedObject();
        }

        bool ISeaTruckArm.HasClaw()
        {
            return false;
        }

        bool ISeaTruckArm.HasDrill()
        {
            return false;
        }

        void ISeaTruckArm.SetRotation(Arm arm)
        {            
            transform.localRotation = Quaternion.Euler(0, 0, 0);           
        }

        float ISeaTruckArm.GetEnergyCost()
        {
            return 0;
        }
    }
}
*/