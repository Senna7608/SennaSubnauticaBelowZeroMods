using System;
using UnityEngine;
using UWE;
using BZCommon;
using CheatManagerZero.Configuration;

namespace CheatManagerZero
{
    public class HoverbikeOverDrive : MonoBehaviour
    {
        public HoverbikeOverDrive Instance { get; private set; }
        public Hoverbike ThisHoverBike { get; private set; }
        public Equipment ThisEquipment { get; private set; }        

        private const float HoverBike_Default_forwardAccel = 4250f;
        private const float HoverBike_Default_forwardBoostForce = 22000f;
        private const float HoverBike_Default_fovMaxVelocity = 12f;
        private const float HoverBike_Default_fovSmoothSpeed = 2f;
        private const float HoverBike_Default_fovSpeedPower = 3f;
        private const float HoverBike_Default_verticalBoostForce = 6000f;
        private const float HoverBike_Default_hoverForce = 3500f;
        private const float HoverBike_Default_verticalDampening = 0.0007f;
        private const float HoverBike_Default_waterDampening = 10f;
        private const float HoverBike_Default_waterLevelOffset = 0;

        internal void PrintForces(Hoverbike hoverbike)
        {
            BZLogger.Log($"[{Config.PROGRAM_NAME}] DebugHoverBike:\n" +
                $"forwardAccel: {hoverbike.forwardAccel}\n" +
                $"forwardBoostForce: {hoverbike.forwardBoostForce}\n" +
                $"fovMaxVelocity: {hoverbike.fovMaxVelocity}\n" +
                $"fovSmoothSpeed: {hoverbike.fovSmoothSpeed}\n" +
                $"fovSpeedPower: {hoverbike.fovSpeedPower}\n" +
                $"verticalBoostForce: {hoverbike.verticalBoostForce}\n" +
                $"hoverForce: {hoverbike.hoverForce}\n" +
                $"verticalDampening: { hoverbike.verticalDampening}\n" +
                $"waterDampening: {hoverbike.waterDampening}\n" +
                $"waterLevelOffset: {hoverbike.waterLevelOffset}");
        }

        public void Awake()
        {
            Instance = this;
            ThisHoverBike = GetComponent<Hoverbike>();            
        }

        public void Start()
        {            
            ThisEquipment = ThisHoverBike.modules;
            ThisEquipment.onAddItem += OnAddItem;
            ThisEquipment.onRemoveItem += OnRemoveItem;
            Main.Instance.onHoverbikeSpeedValueChanged.AddHandler(this, new Event<object>.HandleFunction(OnHoverBikeSpeedValueChanged));
            Main.Instance.onPlayerMotorModeChanged.AddHandler(this, new Event<Player.MotorMode>.HandleFunction(OnPlayerMotorModeChanged));
            Main.Instance.isHoverBikeMoveOnWater.changedEvent.AddHandler(this, new Event<Utils.MonitoredValue<bool>>.HandleFunction(IsHoverBikeMoveOnWater));
        }

        private void IsHoverBikeMoveOnWater(Utils.MonitoredValue<bool> hoverBikeMoveOnWater)
        {
            SetHoverBikeMoveOnWater(hoverBikeMoveOnWater.value);
        }

        private void OnHoverBikeSpeedValueChanged(object newValue)
        {
            Main.Instance.hoverbikeSpeedMultiplier = (float)newValue;
            SetHoverbikeOverDrive(ThisHoverBike, (float)newValue);
            //PrintForces(ThisHoverBike);
        }

        public void OnDestroy()
        {
            ThisEquipment.onAddItem -= OnAddItem;
            ThisEquipment.onRemoveItem -= OnRemoveItem;
            Main.Instance.onHoverbikeSpeedValueChanged.RemoveHandler(this, OnHoverBikeSpeedValueChanged);
            Main.Instance.onPlayerMotorModeChanged.RemoveHandler(this, OnPlayerMotorModeChanged);
            Destroy(Instance);
        }

        private void OnPlayerMotorModeChanged(Player.MotorMode newMotorMode)
        {            
            if (newMotorMode == Player.MotorMode.Vehicle)
            {
                if (Player.main.inHovercraft)
                {                    
                    SetHoverbikeOverDrive(ThisHoverBike, Main.Instance.hoverbikeSpeedMultiplier);
                }
            }
        }

        private void OnAddItem(InventoryItem invItem)
        {            
            SetHoverbikeOverDrive(ThisHoverBike, Main.Instance.hoverbikeSpeedMultiplier);
        }

        private void OnRemoveItem(InventoryItem invItem)
        {            
            SetHoverbikeOverDrive(ThisHoverBike, Main.Instance.hoverbikeSpeedMultiplier);
        }

        internal void SetHoverbikeOverDrive(Hoverbike hoverbike, float multiplier)
        {
            hoverbike.forwardAccel = multiplier == 1 ? HoverBike_Default_forwardAccel : HoverBike_Default_forwardAccel * multiplier;
            SetHoverBikeMoveOnWater(Main.Instance.isHoverBikeMoveOnWater.value);
        }               

        internal void SetHoverBikeMoveOnWater(bool isEnabled)
        {
            if (isEnabled)
            {
                ThisHoverBike.waterLevelOffset = 2f;
                ThisHoverBike.waterDampening = 1;
            }
            else
            {
                ThisHoverBike.waterLevelOffset = HoverBike_Default_waterLevelOffset;
                ThisHoverBike.waterDampening = HoverBike_Default_waterDampening;
            }
        }
    }
}
