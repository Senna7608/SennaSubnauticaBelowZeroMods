using System.Collections;
using UnityEngine;

namespace SeaTruckFlyModule
{
    public partial class FlyManager
    {
        private readonly Vector3 takeOffVelocity = new Vector3(0, 10f, 0);

        IEnumerator OnTakeOff()
        {
            while (distanceFromSurface < 20)
            {
                SeatruckState = TruckState.TakeOff;                

                timer += 0.01f;
                                
                rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, takeOffVelocity, timer);               

                //print($"[TakeOff] timer: {timer}, velocity: {rigidbody.velocity.y}");                

                yield return null;
            }
            
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

            yield break;
        }
    }
}
