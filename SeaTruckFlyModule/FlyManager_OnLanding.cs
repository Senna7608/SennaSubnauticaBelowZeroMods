using System.Collections;
using UnityEngine;

namespace SeaTruckFlyModule
{
    public partial class FlyManager
    {
        private readonly Vector3 landingVelocity = new Vector3(0, -1f, 0);

        IEnumerator OnLanding(Vector3 velocity)
        {
            while (!IsSafePitch())
            {
                StabilizePitch();

                yield return null;
            }

            while (!IsFootsContactWithTerrain())
            {
                SeatruckState = TruckState.Landing;

                timer += 0.01f;//Mathf.Max(Mathf.Abs(rigidbody.velocity.y) * 0.009f, 0.008f);

                if (timer > 5.0f)
                {
                    ErrorMessage.AddDebug("Landing sequence failed");
                    OnLandingFailed();
                    yield break;
                }
                else
                {
                    rigidbody.velocity = Vector3.Lerp(velocity, landingVelocity, timer);

                    print($"timer: {timer}, velocity: {rigidbody.velocity}");
                }

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

            SeatruckPosition = TruckPosition.OnSurface;
            SeatruckState = TruckState.Landed;

            ErrorMessage.AddDebug("Seatruck has landed");

            SetExitPosition();

            if (isFlying.value)
            {
                isFlying.Update(false);
            }

            yield break;
        }
    }
}
