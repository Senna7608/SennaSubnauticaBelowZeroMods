using UnityEngine;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;
using ModdedArmsHelperBZ.API.ArmHandlers;

namespace SeaTruckArms.InternalArmHandlers
{
    internal class InternalDrillArmHandler : DrillArmHandler, ISeatruckArm
    {
        public override void Start()
        {
        }

        GameObject ISeatruckArm.GetGameObject()
        {
            return gameObject;
        }

        GameObject ISeatruckArm.GetInteractableRoot(GameObject target)
        {
            SeatruckDrillable seatruckDrillable = target.GetComponent<SeatruckDrillable>();

            if (seatruckDrillable != null)
            {
                return seatruckDrillable.gameObject;
            }

            return null;
        }

        void ISeatruckArm.SetSide(SeatruckArm arm)
        {
            if (arm == SeatruckArm.Right)
            {
                transform.localScale = new Vector3(-0.80f, 0.80f, 0.80f);
            }
            else
            {
                transform.localScale = new Vector3(0.80f, 0.80f, 0.80f);
            }
        }

        bool ISeatruckArm.OnUseDown(out float cooldownDuration)
        {
            animator.SetBool("use_tool", true);
            drilling = true;
            loop.Play();
            cooldownDuration = 0f;
            drillTarget = null;
            return true;
        }

        bool ISeatruckArm.OnUseHeld(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return false;
        }


        bool ISeatruckArm.OnUseUp(out float cooldownDuration)
        {
            animator.SetBool("use_tool", false);
            drilling = false;
            StopEffects();
            cooldownDuration = 0f;
            return true;
        }

        bool ISeatruckArm.OnAltDown()
        {
            return false;
        }

        private bool isResetted = false;

        void ISeatruckArm.Update(ref Quaternion aimDirection)
        {
            if (!isResetted)
            {
                animator.SetBool("use_tool", false);
                drilling = false;
                StopEffects();
                isResetted = true;
            }

            if (drillTarget != null)
            {
                Quaternion b = Quaternion.LookRotation(Vector3.Normalize(UWE.Utils.GetEncapsulatedAABB(drillTarget, -1).center - MainCamera.camera.transform.position), Vector3.up);
                smoothedDirection = Quaternion.Lerp(smoothedDirection, b, Time.deltaTime * 6f);
                aimDirection = smoothedDirection;
            }
            else
            {
                smoothedDirection = Quaternion.Lerp(smoothedDirection, aimDirection, Time.deltaTime * 15f);
                aimDirection = smoothedDirection;
            }
        }

        void ISeatruckArm.ResetArm()
        {
            animator.SetBool("use_tool", false);
            drilling = false;
            StopEffects();
        }

        public void OnHit()
        {
            if (seatruck.seatruckHelper.IsPiloted())
            {
                Vector3 pos = Vector3.zero;
                GameObject hitObject = null;
                drillTarget = null;

                UWE.Utils.TraceFPSTargetPosition(seatruck.mainCab, attackDist, ref hitObject, ref pos, out Vector3 normal, true);

                if (hitObject == null)
                {
                    InteractionVolumeUser component = Player.main.gameObject.GetComponent<InteractionVolumeUser>();

                    if (component != null && component.GetMostRecent() != null)
                    {
                        hitObject = component.GetMostRecent().gameObject;
                    }
                }
                if (hitObject && drilling)
                {
                    var drillable = hitObject.FindAncestor<SeatruckDrillable>();

                    loopHit.Play();

                    if (drillable)
                    {
                        drillable.OnDrill(fxSpawnPoint.position, seatruck, out GameObject gameObject2);

                        if (!gameObject2)
                        {
                            StopEffects();
                        }

                        drillTarget = gameObject2;

                        if (fxControl.emitters[0].fxPS != null && !fxControl.emitters[0].fxPS.emission.enabled)
                        {
                            fxControl.Play(0);
                        }
                    }
                    else
                    {
                        LiveMixin liveMixin = hitObject.FindAncestor<LiveMixin>();

                        if (liveMixin)
                        {
                            bool flag = liveMixin.IsAlive();
                            liveMixin.TakeDamage(4f, pos, DamageType.Drill, null);
                            drillTarget = hitObject;
                        }

                        VFXSurface component2 = hitObject.GetComponent<VFXSurface>();

                        if (drillFXinstance == null)
                        {
                            drillFXinstance = VFXSurfaceTypeManager.main.Play(component2, vfxEventType, fxSpawnPoint.position, fxSpawnPoint.rotation, fxSpawnPoint);
                        }
                        else if (component2 != null && prevSurfaceType != component2.surfaceType)
                        {
                            VFXLateTimeParticles component3 = drillFXinstance.GetComponent<VFXLateTimeParticles>();
                            component3.Stop();
                            Destroy(drillFXinstance.gameObject, 1.6f);
                            drillFXinstance = VFXSurfaceTypeManager.main.Play(component2, vfxEventType, fxSpawnPoint.position, fxSpawnPoint.rotation, fxSpawnPoint);
                            prevSurfaceType = component2.surfaceType;
                        }

                        hitObject.SendMessage("BashHit", this, SendMessageOptions.DontRequireReceiver);
                    }
                }
                else
                {
                    StopEffects();
                }
            }
        }

        private void StopEffects()
        {
            if (drillFXinstance != null)
            {
                VFXLateTimeParticles component = drillFXinstance.GetComponent<VFXLateTimeParticles>();
                component.Stop();
                Destroy(drillFXinstance.gameObject, 1.6f);
                drillFXinstance = null;
            }

            if (fxControl.emitters[0].fxPS != null && fxControl.emitters[0].fxPS.emission.enabled)
            {
                fxControl.Stop(0);
            }
            loop.Stop();
            loopHit.Stop();
        }

        bool ISeatruckArm.HasClaw()
        {
            return false;
        }

        bool ISeatruckArm.HasDrill()
        {
            return true;
        }

        bool ISeatruckArm.HasPropCannon()
        {
            return false;
        }

        float ISeatruckArm.GetEnergyCost()
        {
            return energyCost;
        }

        void ISeatruckArm.SetRotation(SeatruckArm arm, bool isDocked)
        {
            if (isDocked && !seatruck.seatruckHelper.TruckDockable.isInTransition)
            {
                BaseRoot baseRoot = seatruck.seatruckHelper.MainCab.GetComponentInParent<BaseRoot>();

                if (baseRoot.isBase)
                {
                    if (arm == SeatruckArm.Right)
                    {
                        transform.localRotation = Quaternion.Euler(20, 6, 0);
                    }
                    else
                    {
                        transform.localRotation = Quaternion.Euler(20, 354, 0);
                    }
                }
            }
            else
            {
                if (arm == SeatruckArm.Right)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }

        bool ISeatruckArm.GetCustomUseText(out string customText)
        {
            customText = string.Empty;
            return false;
        }
    }

}

