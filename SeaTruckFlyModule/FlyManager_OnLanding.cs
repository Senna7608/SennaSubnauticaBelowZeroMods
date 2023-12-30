extern alias SEZero;
using BZCommon;
using SEZero::SlotExtenderZero.API;
using System.Collections;
using UnityEngine;

namespace SeaTruckFlyModule
{
    public partial class FlyManager //OnLanding
    {
        private readonly Vector3 landingVelocity = new Vector3(0, -1f, 0);
           
        IEnumerator OnLanding(Vector3 surface)
        {
            while (!IsSafePitch())
            {
                StabilizePitch();
                BZLogger.Debug("[OnLanding] Stabilizing Pitch...");
                yield return null;
            }

            if (!landingFoots.activeSelf)
            {
                BZLogger.Debug("[OnLanding] Extending landing foots");
                SetLandingFoots(true);
            }

            ErrorMessage.AddDebug("Landing sequence started");
            engineSound.Stop();

            //helper.TruckWorldForces.aboveWaterGravity = 9.81f;
            timer = 0.0f;

            while (!IsFootsContactWithTerrain())
            {
                if (telemetry.SeatruckState != TruckState.Landing)
                {
                    telemetry.ForceStateChange(TruckState.Landing);
                }
                
                rigidbody.velocity = Vector3.down * Mathf.Clamp(telemetry.distanceFromSurface, 0.1f, 10f);

                BZLogger.Debug($"[OnLanding] rigidbody velocity: {rigidbody.velocity}");

                /*
                telemetry.ForceStateChange(TruckState.Landing);

                timer += 0.01f;//Mathf.Max(Mathf.Abs(rigidbody.velocity.y) * 0.009f, 0.008f);

                //timer += altitude / 1000;

                if (timer > 5.0f)
                {
                    ErrorMessage.AddDebug("Landing sequence failed");
                    OnLandingFailed();
                    yield break;
                }
                else
                {
                    rigidbody.velocity = Vector3.Lerp(surface, landingVelocity, timer);

                    BZLogger.Debug($"[OnLanding] timer: {timer}, velocity: {rigidbody.velocity}");
                }
                */


                yield return null;
            }

            rigidbody.velocity = new Vector3(0, -0.1f, 0);

            yield return new WaitForSeconds(1);

            if (!IsSafePitch())
            {
                ErrorMessage.AddDebug("Pitch angle unsafe!");
                OnLandingFailed();
                yield break;
            }

            if (!IsSafeRoll())
            {
                ErrorMessage.AddDebug("Roll angle unsafe!");
                OnLandingFailed();
                yield break;
            }            

            rigidbody.velocity = Vector3.zero;

            rigidbody.isKinematic = true;

            telemetry.ForcePositionChange(TruckPosition.OnSurface);
            telemetry.ForceStateChange(TruckState.Landed);

            ErrorMessage.AddDebug("Seatruck has landed");

            SetExitPosition();

            if (isFlying.value)
            {
                isFlying.Update(false);
            }

            yield break;
        }

        public void OnLandingFailed()
        {
            BZLogger.Debug("[OnLanding] Landing failed!");
            if (telemetry.SeatruckState == TruckState.Landing)
            StartCoroutine(OnTakeOff());
            /*
            rigidbody.AddForce(jump, ForceMode.Impulse);

            if (!isFlying.value)
            {
                isFlying.Update(true);
            }
            */
        }
    }
}
