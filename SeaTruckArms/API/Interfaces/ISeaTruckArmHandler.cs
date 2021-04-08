using UnityEngine;

namespace SeaTruckArms.API.Interfaces
{
    public interface ISeaTruckArmHandler
    {
        GameObject GetGameObject();

        bool HasClaw();

        bool HasDrill();

        void SetSide(SeaTruckArm arm);

        void SetRotation(SeaTruckArm arm, bool isDocked);

        bool OnUseDown(out float cooldownDuration);

        bool OnUseHeld(out float cooldownDuration);

        bool OnUseUp(out float cooldownDuration);

        bool OnAltDown();

        void Update(ref Quaternion aimDirection);

        void Reset();

        float GetEnergyCost();
    }
}