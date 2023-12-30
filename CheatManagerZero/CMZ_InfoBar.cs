using System.Text;
using UnityEngine;
using System;
using CheatManagerZero.Configuration;
using BZCommon.Helpers.GUIHelper;

namespace CheatManagerZero
{
    public class CMZ_InfoBar : MonoBehaviour
    {
        private static Rect windowRect = new Rect(0, 0, Screen.width - (Screen.width / CMZ_Config.ASPECT) - 2 , Screen.height / 45);
        private Rect drawRect;
        
        private Int3 currentBatch = new Int3();
        private string currentBiome = "";
        private int day = 0;
        private string infoText;

        private readonly string[] daynightstr = { "Day", "Night" };
        private int isDay;
        private Vector3 currentWorldPos = new Vector3();       
        private StringBuilder stringBuilder;
        private float temperature = 0;

        float FPS;
        float totalmem;
        float diffTotalmem;
        private float timeNextSample = -1f;
        private float timeNextUpdate = -1f;
        private long lastTotalMem;
        private long diffTotalMem;
        private float accumulatedFrameTime;
        private int numAccumulatedFrames;
        private int lastCollectionCount1;
        private int lastCollectionCount2;
        private float lastCollectionTime;
        private float timeBetweenCollections;
        private float avgFrameTime;
        private int numCollections;
        private int numFixedUpdates;
        private int numUpdates;
        private float avgFixedUpdatesPerFrame;

        private Vector3 PlayerLastPosition = Vector3.zero;
        private float speed;
        private Vector3 currVel;
        private float timeCount = 0.0f;

        public void Awake()
        {   
            useGUILayout = false;            
            DontDestroyOnLoad(this);
            drawRect = new Rect(windowRect.x + 5, windowRect.y, windowRect.width, windowRect.height);            
        }

        public void Start()
        {
            stringBuilder = new StringBuilder();
        }

        public void OnDestroy()
        {
            stringBuilder = null;
        }

        public void Update()
        {
            if (!CMZ_Config.isInfoBarEnabled)
            {
                return;
            }

            UpdateFPS();

            if (Player.main != null && CMZ_Config.isInfoBarEnabled)
            {
                timeCount += Time.deltaTime;

                if (timeCount > 1.0f)
                {
                    SubRoot playerSubRoot = Player.main.GetCurrentSub();

                    if (playerSubRoot != null)
                    {

                        temperature = playerSubRoot.GetInsideTemperature();
                    }
                    else if (WeatherManager.main != null && Player.main.transform.position.y > 0)
                    {
                        temperature = WeatherManager.main.GetCurrentOutdoorTemperature();

                    }
                    else if (WaterTemperatureSimulation.main != null)
                    {
                        temperature = WaterTemperatureSimulation.main.GetTemperature(Utils.GetLocalPlayerPos());
                    }

                    currentBiome = Player.main.GetBiomeString();
                    currentBatch = LargeWorldStreamer.main.GetContainingBatch(Utils.GetLocalPlayerPos());
                    isDay = DayNightCycle.main.IsDay() ? 0 : 1;                    
                    day = (int)DayNightCycle.main.GetDay();
                    timeCount = 0.0f;                    
                }
                
                currentWorldPos = Player.main.transform.position;

                stringBuilder.Remove(0, stringBuilder.Length);
                
                stringBuilder.AppendFormat($"Biome: {currentBiome}" +
                    $"   {string.Format("World Position: {0,3:N0}, {1,3:N0}, {2,3:N0}", currentWorldPos.x, currentWorldPos.y, currentWorldPos.z)}" +                    
                    $"   Batch: {currentBatch}" +                    
                    $"   Day: {day}" +
                    $"   Time of Day: {daynightstr[isDay]}" +
                    $"   Temp.: {Mathf.CeilToInt(temperature)} \u00B0C" +
                    $"   Speed: {(int)speed} km/h" +
                    $"   {string.Format("FPS: {0,3:N0}", FPS)}" +
                    $"   {string.Format("MEM: {0,3:N0} MB (+{1,6:N2} MB/s)", totalmem, diffTotalmem)}" +
                    $"   {string.Format("GC: {0,2:N0} ms (Total:{1,3})",  timeBetweenCollections, numCollections)}" +
                    $"   Fixed Updates: {avgFixedUpdatesPerFrame}");

                infoText = stringBuilder.ToString();
            }
            else
            {
                timeCount = 0.0f;

                stringBuilder.Remove(0, stringBuilder.Length);

                stringBuilder.AppendFormat(
                    $"   {string.Format("FPS: {0,3:N0}", FPS)}" +
                    $"   {string.Format("MEM: {0,3:N0} MB (+{1,6:N2} MB/s)", totalmem, diffTotalmem)}" +
                    $"   {string.Format("GC: {0,2:N0} ms (Total:{1,3})", timeBetweenCollections, numCollections)}" +
                    $"   Fixed Updates: {avgFixedUpdatesPerFrame}");

                infoText = stringBuilder.ToString();
            }

        }

        public void OnGUI()
        {
            if (!CMZ_Config.isInfoBarEnabled)
            {
                return;
            }

            SNWindow.CreateWindow(windowRect, null);
            GUI.Label(drawRect, infoText, SNStyles.GetGuiItemStyle(GuiItemType.LABEL, textColor: GuiColor.Green, textAnchor: TextAnchor.MiddleLeft));
        }

        private void FixedUpdate()
        {
            if (!CMZ_Config.isInfoBarEnabled)
            {
                return;
            }

            if (Player.main != null)
            {
                if (Player.main.GetVehicle() != null)
                {
                    speed = Player.main.GetVehicle().useRigidbody.velocity.magnitude * 3.6f;
                }                
                else
                {
                    currVel = (Player.main.transform.position - PlayerLastPosition) / Time.fixedDeltaTime;
                    speed = currVel.magnitude * 3.6f;
                }

                PlayerLastPosition = Player.main.transform.position;
            }

            numFixedUpdates++;
        }

        private void UpdateFPS()
        {
            numAccumulatedFrames++;
            numUpdates++;
            accumulatedFrameTime += Time.unscaledDeltaTime;
            bool flag = false;

            if (Time.unscaledTime > timeNextSample)
            {
                SampleTotalMemory();
                timeNextSample = Time.unscaledTime + 1f;
                flag = true;
            }

            if (Time.unscaledTime > timeNextUpdate)
            {
                SampleFrameRate();
                flag = true;
                timeNextUpdate = Time.unscaledTime + 0.1f;
                if (numUpdates > 0)
                {
                    avgFixedUpdatesPerFrame = numFixedUpdates / (float)numUpdates;
                    numUpdates = 0;
                    numFixedUpdates = 0;
                }
            }

            int num = GC.CollectionCount(1);
            int num2 = GC.CollectionCount(2);

            if (num2 > lastCollectionCount2 || num > lastCollectionCount1)
            {
                float unscaledTime = Time.unscaledTime;
                timeBetweenCollections = unscaledTime - lastCollectionTime;
                lastCollectionTime = unscaledTime;
                numCollections++;
                flag = true;
            }

            lastCollectionCount1 = num;
            lastCollectionCount2 = num2;

            if (flag)
            {
                totalmem = lastTotalMem / 1048576f;
                diffTotalmem = diffTotalMem / 1048576f;
                FPS = 1f / avgFrameTime;                
            }
        }

        private void SampleTotalMemory()
        {
            long totalMemory = GC.GetTotalMemory(false);
            diffTotalMem = totalMemory - lastTotalMem;
            lastTotalMem = totalMemory;
        }

        private void SampleFrameRate()
        {
            avgFrameTime = accumulatedFrameTime / numAccumulatedFrames;
            numAccumulatedFrames = 0;
            accumulatedFrameTime = 0f;
        }        
    }
}
