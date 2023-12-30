using BZHelper;
using UnityEngine;

namespace SlotExtenderZero.API
{
    public enum TruckState
    {
        None,
        Diving,
        TakeOff,
        Flying,
        AutoFly,
        Landing,
        Landed
    };

    public enum TruckPosition
    {
        None,
        BelowWater,
        AboveWater,
        AboveSurface,
        NearSurface,
        OnSurface
    };

    [DisallowMultipleComponent]
    public class SeaTruckTelemetry : MonoBehaviour
    {
        private SeatruckHelper helper = null;
        //private ObjectHelper objectHelper = new ObjectHelper();
        private GameObject mainCab;

        public GameObject altitudeMeter { get; private set; }
        public float depth;
        public float altitude;
        public float speed;
        public float distanceFromSurface, FLeftDist, FRightDist;


        //public float AGC_MaximumRange = 2000f;
        //public float AGC_VerticalOffset = -1f;
        //public LayerMask AGC_LayerMask = ~0;
        public LayerMask layerMask = 1 << LayerID.TerrainCollider;

        private static readonly Vector3 LDown = new Vector3(-1f, -1f, 0f);
        private static readonly Vector3 RDown = new Vector3(1f, -1f, 0f);

        public Vector3 LeftDown => LDown;
        public Vector3 RightDown => RDown;

        //private Vector3 mainCabLastPosition = Vector3.zero;

        public delegate void OnSeatruckStateChanged(TruckState newState);
        public event OnSeatruckStateChanged onSeatruckStateChanged;

        public delegate void OnSeatruckPositionChanged(TruckPosition newPosition);
        public event OnSeatruckPositionChanged onSeatruckPositionChanged;

        private TruckState _seatruckState = TruckState.None;
        public TruckState SeatruckState
        {
            get
            {
                return _seatruckState;
            }
            private set
            {
                if (_seatruckState != value)
                {
                    _seatruckState = value;

                    onSeatruckStateChanged?.Invoke(_seatruckState);

                    BZLogger.Debug($"API/Telemetry: SeaTruck state changed: [{_seatruckState}]");
                }
            }
        }

        private TruckPosition _seatruckPosition;
        public TruckPosition SeatruckPosition
        {
            get
            {
                return _seatruckPosition;
            }
            private set
            {
                if (_seatruckPosition != value)
                {
                    _seatruckPosition = value;

                    onSeatruckPositionChanged?.Invoke(_seatruckPosition);

                    BZLogger.Debug($"API/Telemetry: SeaTruck position changed: [{_seatruckPosition}]");
                }
            }
        }        

        private void Awake()
        {
            BZLogger.Debug($"API/Telemetry: Awake started, ID: [{gameObject.GetInstanceID()}]");

            helper = SeatruckServices.Main.GetSeaTruckHelper(gameObject);
            mainCab = helper.MainCab;

            altitudeMeter = UnityHelper.CreateGameObject("altitudeMeter", transform);
            Utils.ZeroTransform(altitudeMeter.transform);
            altitudeMeter.transform.localPosition = new Vector3(0f, -3f, 0.94f);            

            BZLogger.Log($"API/Telemetry: SeaTruckTelemetry is ready for this SeaTruck, ID: [{mainCab.GetInstanceID()}]");

            BZLogger.Debug($"API/Telemetry: Awake finished, ID: [{mainCab.GetInstanceID()}]");
        }

        public void ForceStateChange(TruckState newState)
        {
            BZLogger.Debug($"API/Telemetry: ForceStateChange called with parameter [{newState}]");
            SeatruckState = newState;
        }

        public void ForcePositionChange(TruckPosition newPosition)
        {
            BZLogger.Debug($"API/Telemetry: ForcePositionChange called with parameter [{newPosition}]");
            SeatruckPosition = newPosition;
        }


        private void FixedUpdate()
        {
            if (helper == null)
            {
                return;
            }

            if (helper.IsDocked)
            {
                return;
            }

            speed = helper.TruckWorldForces.useRigidbody.velocity.magnitude * 3.6f;            

            altitude = helper.MainCab.transform.position.y;

            depth = altitude < 0 ? altitude : 0;

            altitudeMeter.transform.localRotation = Quaternion.AngleAxis(360 - mainCab.transform.eulerAngles.x, Vector3.right);

            if (Physics.Raycast(altitudeMeter.transform.position, altitudeMeter.transform.TransformDirection(Vector3.down), out RaycastHit raycastDown, 100f, layerMask, QueryTriggerInteraction.Ignore))
            {
                GameObject gameObject = raycastDown.collider.gameObject;

                if (gameObject != null && gameObject.GetComponent<LiveMixin>() == null)
                {
                    distanceFromSurface = (altitude - raycastDown.point.y) - 3;
                }
                else
                {
                    distanceFromSurface = altitude;
                }
            }
            else
            {
                distanceFromSurface = altitude;
            }

            if (Physics.Raycast(altitudeMeter.transform.position, altitudeMeter.transform.TransformDirection(LDown), out RaycastHit raycastLeft, 100f, layerMask, QueryTriggerInteraction.Ignore))
            {
                GameObject gameObject = raycastLeft.collider.gameObject;

                if (gameObject != null && gameObject.GetComponent<LiveMixin>() == null)
                {
                    FLeftDist = (altitude - raycastLeft.point.y) - 3;
                }

            }
            else
            {
                FLeftDist = altitude;
            }

            if (Physics.Raycast(altitudeMeter.transform.position, altitudeMeter.transform.TransformDirection(RDown), out RaycastHit raycastRight, 100f, layerMask, QueryTriggerInteraction.Ignore))
            {
                GameObject gameObject = raycastRight.collider.gameObject;

                if (gameObject != null && gameObject.GetComponent<LiveMixin>() == null)
                {
                    FRightDist = (altitude - raycastRight.point.y) - 3;
                }

            }
            else
            {
                FRightDist = altitude;
            }
            
            if (SeatruckState != TruckState.Landing || SeatruckState != TruckState.TakeOff)
            {
                if (altitude < 0)
                {
                    SeatruckPosition = TruckPosition.BelowWater;
                    SeatruckState = TruckState.Diving;
                }
                else if (altitude > 0 && altitude <= distanceFromSurface)
                {
                    SeatruckPosition = TruckPosition.AboveWater;
                    SeatruckState = TruckState.Flying;
                }
                else if (altitude > distanceFromSurface && distanceFromSurface > 20.0f)
                {
                    SeatruckPosition = TruckPosition.AboveSurface;
                    SeatruckState = TruckState.Flying;
                }
                else if (altitude > 0 && distanceFromSurface > 1.0f && distanceFromSurface < 15.0f)
                {
                    SeatruckPosition = TruckPosition.NearSurface;
                    SeatruckState = TruckState.Flying;
                }
            }
        }        
    }    
}
