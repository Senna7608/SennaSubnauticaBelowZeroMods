using UnityEngine;

namespace SlotExtenderZero
{
    internal class SlotPosHoverbike : SlotPosLayout
    {
        public override Vector2 VehicleImgPos => new Vector2(120, 305);
        
        public override Vector2[] SlotPos => new Vector2[4]
        {
            new Vector2(-140f, -293), // Hoverbike slot 1
            new Vector2(20f, -293),    // Hoverbike slot 2
            new Vector2(140f, -184),  // Hoverbike slot 3
            new Vector2(140f, -30)   // Hoverbike slot 4
        };

        public override Vector2[] ArmSlotPos => null;             
    }
}
