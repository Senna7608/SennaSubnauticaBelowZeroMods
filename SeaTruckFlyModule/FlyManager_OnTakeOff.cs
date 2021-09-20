using BZCommon;
using System.Collections;
using UnityEngine;

namespace SeaTruckFlyModule
{
    public partial class FlyManager
    {
        private readonly Vector3 takeOffVelocity = new Vector3(0, 10f, 0);
        private float prevSurfacePos;

        private IEnumerator OnTakeOff()
        {
            BZLogger.Debug("[OnTakeOff] Take off started");

            if (SeatruckState == TruckState.TakeOff)
                yield break;

            prevSurfacePos = mainCab.transform.position.y;
            rigidbody.isKinematic = false;

            while (distanceFromSurface < 20 || mainCab.transform.position.y < prevSurfacePos + 20)
            {
                SeatruckState = TruckState.TakeOff;                
                                
                rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, takeOffVelocity, timer);

                BZLogger.Debug($"[OnTakeOff] timer: {timer}, velocity: {rigidbody.velocity.y}, distanceFromSurface: {distanceFromSurface}, prevDistance: {prevSurfacePos}, mainCab.position.y: {mainCab.transform.position.y}");

                timer += 0.001f;

                if (mainCab.transform.position.y > 150)
                {
                    rigidbody.velocity = Vector3.zero;
                    BZLogger.Debug("[OnTakeOff] Take off Breaked by position.y!");

                    OnTakeOffFinished();

                    yield break;
                }

                yield return null;
            }

            OnTakeOffFinished();

            yield break;
        }

        private void OnTakeOffFinished()
        {
            if (landingFoots.activeSelf)
            {
                SetLandingFoots(false);
            }

            if (!isFlying.value)
            {
                isFlying.Update(true);
            }

            SeatruckState = TruckState.Flying;

            ErrorMessage.AddDebug("Seatruck has take off");
        }
    }
}
