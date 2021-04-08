namespace SeaTruckArms.API
{
    public enum ArmTemplate
    {
        ClawArm,
        DrillArm,
        GrapplingArm,
        PropulsionArm,
        TorpedoArm
    };

    public enum ArmFragmentTemplate
    {
        ClawArm,
        DrillArm,
        GrapplingArm,
        PropulsionArm,
        TorpedoArm
    };

    public enum SeaTruckArm
    {
        None,
        Left,
        Right
    }

    public enum ObjectType
    {
        None,
        Pickupable,
        Drillable
    }
}
