using UnityEngine;
using SeaTruckArms.API;
using SeaTruckArms.API.ArmHandlers;
using SeaTruckArms.API.Interfaces;

namespace SeaTruckArms.InternalArmHandlers
{
    internal class InternalDrillArmHandler : DrillArmHandler, ISeaTruckArmHandler
    {
        GameObject ISeaTruckArmHandler.GetGameObject()
        {
            return gameObject;
        }

        public void SetSide(SeaTruckArm arm)
        {
            if (arm == SeaTruckArm.Right)
            {
                transform.localScale = new Vector3(-0.80f, 0.80f, 0.80f);
            }
            else
            {
                transform.localScale = new Vector3(0.80f, 0.80f, 0.80f);
            }
        }

        bool ISeaTruckArmHandler.OnUseDown(out float cooldownDuration)
        {
            animator.SetBool("use_tool", true);
            drilling = true;
            loop.Play();
            cooldownDuration = 0f;
            drillTarget = null;
            return true;
        }

        bool ISeaTruckArmHandler.OnUseHeld(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return false;
        }


        bool ISeaTruckArmHandler.OnUseUp(out float cooldownDuration)
        {
            animator.SetBool("use_tool", false);
            drilling = false;
            StopEffects();
            cooldownDuration = 0f;
            return true;
        }

        bool ISeaTruckArmHandler.OnAltDown()
        {
            return false;
        }

        private bool isResetted = false;

        void ISeaTruckArmHandler.Update(ref Quaternion aimDirection)
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

        void ISeaTruckArmHandler.Reset()
        {
            animator.SetBool("use_tool", false);
            drilling = false;
            StopEffects();
        }

        public void OnHit()
        {
#if DEBUG
            if (true)
#else
            if (TruckHelper.IsPiloted())
#endif
            {
                Vector3 pos = Vector3.zero;
                GameObject hitObject = null;
                drillTarget = null;

                UWE.Utils.TraceFPSTargetPosition(TruckHelper.MainCab, attackDist, ref hitObject, ref pos, out Vector3 normal, true);

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
                    var drillable = hitObject.FindAncestor<SeaTruckDrillable>();

                    loopHit.Play();

                    if (drillable)
                    {
                        drillable.OnDrill(fxSpawnPoint.position, TruckHelper, out GameObject gameObject2);

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

        bool ISeaTruckArmHandler.HasClaw()
        {
            return false;
        }

        bool ISeaTruckArmHandler.HasDrill()
        {
            return true;
        }

        float ISeaTruckArmHandler.GetEnergyCost()
        {
            return energyCost;
        }

        void ISeaTruckArmHandler.SetRotation(SeaTruckArm arm, bool isDocked)
        {
            if (isDocked)
            {
                BaseRoot baseRoot = TruckHelper.MainCab.GetComponentInParent<BaseRoot>();

                if (baseRoot.isBase)
                {
                    if (arm == SeaTruckArm.Right)
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
                if (arm == SeaTruckArm.Right)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
    }

}

