using BZCommon;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZHelper
{
    public class ModdedSupplyCrateManager : MonoBehaviour
    {
        public static ModdedSupplyCrateManager Instance;

        //GameObject treasureChest;

        LargeWorldStreamer streamer;

        CellManager cellManager;       

        private bool complete = false;

        public ModdedSupplyCrateManager()
        {
            if (!Instance)
            {
                Instance = FindObjectOfType(typeof(ModdedSupplyCrateManager)) as ModdedSupplyCrateManager;

                if (!Instance)
                {
                    GameObject ZHelper = new GameObject("ZHelper");
                    Instance = ZHelper.EnsureComponent<ModdedSupplyCrateManager>();
                }
            }
        }

        public bool SearchChestInRange(Vector3 position, GameObject ignoreObj = null, bool ignoreLiving = true)
        {

            RaycastHit[] hits = Physics.SphereCastAll(position, 5f, Vector3.up);

            BZLogger.Debug("ZHelper", $"SphereCastAll hits: {hits.Length}");

            for (int i = 0; i < hits.Length; i++)
            {
                GameObject gameObject = hits[i].collider.gameObject;

                BZLogger.Debug("ZHelper", $"Gameobjects found on position: {gameObject}");

                if (gameObject.transform.root.gameObject.name.StartsWith("shipwreck_supplycrate", StringComparison.CurrentCultureIgnoreCase))
                {
                    BZLogger.Debug("ZHelper", $"Treasure Chest found on position: {position}");
                    return true;
                }
            }
            return false;
        }


        private static bool IsOtherEntity(UniqueIdentifier uid)
        {
            return !IsSpecialIdentifier(uid) && !uid.GetComponent<LargeWorldEntityCell>() && !uid.GetComponent<EntitySlot>() && !uid.GetComponent<EntityTag>();
        }

        private static bool IsSpecialIdentifier(UniqueIdentifier uid)
        {
            return uid is SceneObjectIdentifier || uid is ChildObjectIdentifier || uid is TemporaryObjectIdentifier;
        }


        private void CheckBatchCell()
        {
            cellManager = streamer.cellManager;

            BZLogger.Debug("ZHelper", $"CellManager: [{cellManager.ToString()}] ");


            Dictionary<Int3, BatchCells> batch2cells = cellManager.GetPrivateField("batch2cells") as Dictionary<Int3, BatchCells>;

            BZLogger.Debug("ZHelper", $"batch2cells count: [{batch2cells.Count}] ");

            foreach (KeyValuePair<Int3, BatchCells> kvp in batch2cells)
            {
                BZLogger.Debug("ZHelper", $"Int3: [{kvp.Key}] ");
            }
            



            Vector3 position = new Vector3(-290, -16.05f, 39.04f);
            Int3 block = streamer.GetBlock(position);
            Int3 key = block / streamer.blocksPerBatch;
            Int3 u = block % streamer.blocksPerBatch;
            int cellLevel = 1;


            if (batch2cells.TryGetValue(key, out BatchCells batchCells))
            {
                Int3 cellSize = BatchCells.GetCellSize(cellLevel, streamer.blocksPerBatch);
                Int3 cellId = u / cellSize;
                EntityCell entityCell = batchCells.EnsureCell(cellId, cellLevel);

                entityCell.ContainsEntity(new Func<UniqueIdentifier, bool>(IsOtherEntity));

                BZLogger.Debug("ZHelper", $"EntityCell.liveRoot: [{entityCell.liveRoot.name}] ");
            }

            complete = true;
        }

        public void Update()
        {
            if (!streamer.IsReady())
            {                
                return;
            }

            if (complete)
            {
                return;
            }
            else
            {
                CheckBatchCell();
            }
        }





        public void Awake()
        {
            DevConsole.RegisterConsoleCommand(this, "searchgo", false, false);

            BZLogger.Debug("ZHelper", "Awake called");

            streamer = LargeWorldStreamer.main;

            BZLogger.Debug("ZHelper", $"Streamer: [{streamer.name}] ");

            

            

            

            /*
            if (SearchChestInRange(new Vector3(-290, -16.05f, 39.04f)))
                return;

            treasureChest = Instantiate(Resources.Load<GameObject>("worldentities/alterra/supplies/shipwreck_supplycrate_battery"));

            treasureChest.transform.position = new Vector3(-290, -16.05f, 39.04f);            

            
            GameObject placeHolder = treasureChest.FindChild("Battery(Placeholder)");

            Pickupable pickupable = placeHolder.transform.GetComponentInChildren<Pickupable>();

            if (pickupable == null)
            {
                BZLogger.Debug("ZHelper", "Pickupable item not found!");
            }
            else
            {
                BZLogger.Debug("ZHelper", $"Pickupable item found: [{pickupable.name}]");
            }

            if (TechTypeHandler.TryGetModdedTechType("SeaTruckArmorMK1", out TechType techtype))
           {
                GameObject armorMK1 = Instantiate(CraftData.GetPrefabForTechType(techtype));

                armorMK1.transform.SetParent(treasureChest.transform);

                UWE.Utils.ZeroTransform(armorMK1);

                treasureChest.transform.position = new Vector3(-290, -16.05f, 39.04f);
           }
           */



        }

        private void OnConsoleCommand_searchgo(NotificationCenter.Notification n)
        {
            if (n != null && n.data != null && n.data.Count > 0)
            {
                string text = (string)n.data[0];

                if (FindActiveGameObject(text, out GameObject go))
                {
                    ErrorMessage.AddDebug($"Found gameobject with name [{text}]\n" +
                        $"name: [{go.name}]\n" +
                        $"root: [{go.transform.root.name}]");
                }
                else
                {
                    ErrorMessage.AddDebug($"Could not find gameobject with name [{text}]");
                }                
            }
        }

        public void Start()
        {
            //StartCoroutine(RegisterChest());
        }
        /*
        private IEnumerator RegisterChest()
        {
            while (LargeWorldStreamer.main == null)
            {
                yield return null;
            }            
            while (LargeWorldStreamer.main.cellManager == null)
            {
                yield return null;
            }
            LargeWorldStreamer.main.cellManager.RegisterEntity(treasureChest);

            yield break;
        }
        */
        private bool FindActiveGameObject(string goName, out GameObject go)
        {
            GameObject[] array = FindObjectsOfType(typeof(GameObject)) as GameObject[];

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].name.StartsWith(goName, StringComparison.CurrentCultureIgnoreCase))
                {
                    string fullPath = array[i].GetFullHierarchyPath();

                    BZLogger.Debug("ZHelper", $"Found gameobject with name [{goName}]\n" +
                        $"path: [{fullPath}]");

                    go = array[i];

                    return true;
                }
            }

            go = null;

            return false;
        }

        
    }
}
