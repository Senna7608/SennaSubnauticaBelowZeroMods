namespace ModdedArmsHelperBZ.API.Interfaces
{
    /// <summary>
    /// The modded Exosuit arm can be controlled via this and base interface. 
    /// </summary>    
    public interface IExosuitModdedArm : IExosuitArm
    {
        /// <summary>
        ///  The arm manager calls this method to query the arm power consumption.
        /// </summary>
        float GetEnergyCost();

        /// <summary>
        ///  The arm manager calls this method for change the ui text.
        /// </summary>
        bool GetCustomUseText(out string customText);
    }
}