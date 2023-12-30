﻿namespace ModdedArmsHelperBZ.API
{
    /// <summary>
    /// The ArmType enumerator.
    /// </summary>
    /// <remarks>
    /// This determines which vehicles can use the arm.
    /// </remarks>
    public enum ArmType
    {
#pragma warning disable CS1591
        ExosuitArm = 15,
        SeatruckArm = 200
#pragma warning restore CS1591
    }

    /// <summary>
    /// The ArmTemplate enumerator.
    /// </summary>
    /// <remarks>
    /// This determines which vanilla arm should be cloned.
    /// </remarks>
    public enum ArmTemplate
    {
#pragma warning disable CS1591
        ClawArm,
        DrillArm,
        GrapplingArm,
        PropulsionArm,
        TorpedoArm
#pragma warning restore CS1591
    };

    /// <summary>
    /// The ArmBaseAbility enumerator.
    /// </summary>
    /// <remarks>
    /// Not currently in use.
    /// </remarks>
    public enum ArmBaseAbility
    {
#pragma warning disable CS1591
        None,
        Claw,
        Drill,
        Propulsion,
        GrapplingHook,
        Torpedo
#pragma warning restore CS1591
    }

    /// <summary>
    /// The SeatruckArm enumerator.
    /// </summary>
    /// <remarks>
    /// Used to query the Seatruck arm.
    /// </remarks>
    public enum SeatruckArm
    {
#pragma warning disable CS1591
        None,
        Left,
        Right
#pragma warning restore CS1591
    }

    /// <summary>
    /// The TargetObjectType enumerator.
    /// </summary>
    /// <remarks>
    /// Used to query the active target.
    /// </remarks>
    public enum TargetObjectType
    {
#pragma warning disable CS1591
        None,
        Pickupable,
        Drillable
#pragma warning restore CS1591
    }
}
