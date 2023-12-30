using BZCommon.Helpers.GUIHelper;
using BZHelper;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CheatManagerZero
{
    public partial class CheatManagerZeroControl
    {
        protected readonly Dictionary<IntVector3, string> WarpTargets_Internal = new Dictionary<IntVector3, string>()
        {
            { new IntVector3(-265, -0, 93),       "Shallow Twisty Bridges"},
            { new IntVector3(39, 0, 300),         "Research Base Zero entry pool"},
            
            { new IntVector3(-224, -99, -250),    "Twisty Bridges"},
            { new IntVector3(333, -111, -1075),   "Lilypad Islands"},
            { new IntVector3(-498, -35, -60),     "Arctic Kelp Forest"},
            { new IntVector3(-532, -80, -89),     "Arctic Kelp Caves"},
            { new IntVector3(-1289, 48, 62),      "Gandalf's hats (Ice)"},
            
            { new IntVector3(-1621, 25, -737),    "Frozen Creature Cave"},            
            { new IntVector3(550, -197, -1080),   "Research Base Omega"},
            { new IntVector3(-254, 42, -770),     "Research Base Delta"},
            { new IntVector3(-587, -27, 20),      "Emergency Supply Cache 1"},
            { new IntVector3(-377, -164, -334),   "Emergency Supply Cache 2"},            
            { new IntVector3(-248, -117, -265),   "Emergency Supply Cache 3"},
            { new IntVector3(-136, -55, -182),    "Emergency Supply Cache 4"},
            { new IntVector3(-557, -199, -489),   "Precursor Computer Core Entrance"},            
            { new IntVector3(49, -104, -916),     "Starship Wreck 1"},
            { new IntVector3(289, -221, -1228),   "Starship Wreck 2"},
            { new IntVector3(446, -145, -636),    "Starship Wreck 3"},
            { new IntVector3(-830, 49, 706),      "Iceworm cemetery: Arctic Spires"},

            { new IntVector3(95, -20, -16),       "Research Platform: Sparse Arctic"},
            { new IntVector3(-1195, 0, -1053),    "Glacial Basin Exit Pool"},
            { new IntVector3(-1310, 0, -1068),    "Glacial Basin Exit Pool 2"},
            { new IntVector3(-391, -143, -825),   "Alterra Mining Site Entrance"},
            { new IntVector3(-80, -405, -940),    "Crystal Cave Entrance (Tree Spires)"},
            { new IntVector3(92, -365, -940),     "Maghda's Base"},
            { new IntVector3(998, 30, -873),      "Maghda's Greenhouse"},
            { new IntVector3(284, -173, -904),    "Cave entrance (Lily Pads)"},
            
            { new IntVector3(1221, -945, -308),   "Precursor Base (Fabricator Caverns)"},            
            
            { new IntVector3(545, -619, -1092),   "Precursor Base (Lily Pads Deep)"},
            { new IntVector3(-508, -139, -64),    "Precursor Modular Monument (Arctic Kelp - Cave Inner)"},
            { new IntVector3(-260, -298, -612),   "Precursor Statue (Water)"},
            { new IntVector3(-721, -220, -601),   "Precursor Obelisk (Arctic Kelp - Cave Inner)"},
            
            { new IntVector3(-112, -569, -1424),  "Precursor Ground Sampler 2 (Tree Spires)"},
            { new IntVector3(792, -491, -676),    "Precursor Ground Sampler 3 (Arctic Caldera)"},
            { new IntVector3(23, -120, -473),     "Precursor Mineral Distiller (Thermal Spires)"},
            { new IntVector3(242, -149, -426),    "Precursor Water Analyzer 1 (Purple Vents)"},
            { new IntVector3(195, -170, -31),     "Precursor Water Analyzer 2 (Arctic Kelp - Cave Inner)"},
            { new IntVector3(906, 0, -700),       "Precursor Satellite (Land)"},
            { new IntVector3(-245, 7, -787),      "Precursor Cave (Land)"},
            
            { new IntVector3(1525, 1, -376),      "The Beach"},
            { new IntVector3(-322, -330, -369),   "Twisty Bridges Deep"},
            
        };

        public readonly Dictionary<IntVector3, string> WarpTargets_User = new Dictionary<IntVector3, string>();

        public List<string> GetWarpTargetNames()
        {
            List<string> targets = new List<string>();

            foreach (string name in WarpTargets_Internal.Values)
            {
                targets.Add(name);
            }

            foreach (string name in WarpTargets_User.Values)
            {
                targets.Add(name);
            }

            return targets;
        }

        public IntVector3 GetIntVector(int index)
        {  
            if (index >= WarpTargets_Internal.Count)
            {
                return WarpTargets_User.Keys.ElementAt(index - WarpTargets_Internal.Count);
            }
            else
            {
                return WarpTargets_Internal.Keys.ElementAt(index);
            }            
        }

        public Vector3 ConvertStringPosToVector3(string target)
        {
            string[] numbers = target.Split(' ');

            return numbers.Length != 3 ? Vector3.zero : new Vector3(float.Parse(numbers[0]), float.Parse(numbers[1]), float.Parse(numbers[2]));
        }        

        public string Teleport(string targetName, string vector3string)
        {
            Vector3 currentWorldPos = Player.main.transform.position;

            string prevCwPos = string.Format("{0:D} {1:D} {2:D}", (int)currentWorldPos.x, (int)currentWorldPos.y, (int)currentWorldPos.z);

            if (IsPlayerInVehicle())
            {
                Player.main.GetVehicle().TeleportVehicle(ConvertStringPosToVector3(vector3string), Quaternion.identity);
                Player.main.CompleteTeleportation();
                ErrorMessage.AddMessage($"Vehicle and Player Warped to:\n{targetName}\n({vector3string})");
            }
            else if (Player.main.GetComponentInParent<SeaTruckMotor>() != null)
            {
                SeaTruckMotor motor = Player.main.GetComponentInParent<SeaTruckMotor>();
                Rigidbody useRigidbody = motor.useRigidbody;

                useRigidbody.isKinematic = true;
                motor.transform.position = ConvertStringPosToVector3(vector3string);
                motor.transform.rotation = Quaternion.identity;
                motor.transform.Translate(new Vector3(0f, 2f, 4f), Space.Self);
                useRigidbody.isKinematic = false;
                ErrorMessage.AddMessage($"Seatruck and Player Warped to:\n{targetName}\n({vector3string})");
            }
            else
            {
                Player.main.SetPosition(ConvertStringPosToVector3(vector3string));
                Player.main.OnPlayerPositionCheat();
                ErrorMessage.AddMessage($"Player Warped to:\n{targetName}\n({vector3string})");
            }

            return prevCwPos;
        }


        public Vector3 Teleport(string targetName, Vector3 targetPos)
        {
            Vector3 currentWorldPos = Player.main.transform.position;

            if (IsPlayerInVehicle())
            {
                Player.main.GetVehicle().TeleportVehicle(targetPos, Quaternion.identity);
                Player.main.CompleteTeleportation();
                ErrorMessage.AddMessage($"Vehicle and Player Warped to:\n{targetName}\n({targetPos.ToString()})");
            }
            else if (Player.main.GetComponentInParent<SeaTruckMotor>() != null)
            {
                SeaTruckMotor motor = Player.main.GetComponentInParent<SeaTruckMotor>();
                Rigidbody useRigidbody = motor.useRigidbody;

                useRigidbody.isKinematic = true;
                motor.transform.position = targetPos;
                motor.transform.rotation = Quaternion.identity;
                motor.transform.Translate(new Vector3(0f, 2f, 4f), Space.Self);
                useRigidbody.isKinematic = false;
                ErrorMessage.AddMessage($"Seatruck and Player Warped to:\n{targetName}\n({targetPos.ToString()})");
            }
            else
            {
                Player.main.SetPosition(targetPos);
                Player.main.OnPlayerPositionCheat();
                ErrorMessage.AddMessage($"Player Warped to:\n{targetName}\n({targetPos.ToString()})");
            }

            return currentWorldPos;
        }

        public IntVector3 Teleport(string targetName, IntVector3 targetPos)
        {
            IntVector3 currentWorldPos = Player.main.transform.position;

            if (IsPlayerInVehicle())
            {
                Player.main.GetVehicle().TeleportVehicle(targetPos.ToVector3(), Quaternion.identity);
                Player.main.CompleteTeleportation();
                ErrorMessage.AddMessage($"Vehicle and Player Warped to:\n{targetName}\n({targetPos.ToString()})");
            }
            else if (Player.main.GetComponentInParent<SeaTruckMotor>() != null)
            {
                SeaTruckMotor motor = Player.main.GetComponentInParent<SeaTruckMotor>();
                Rigidbody useRigidbody = motor.useRigidbody;

                useRigidbody.isKinematic = true;
                motor.transform.position = targetPos;
                motor.transform.rotation = Quaternion.identity;
                motor.transform.Translate(new Vector3(0f, 2f, 4f), Space.Self);
                useRigidbody.isKinematic = false;
                ErrorMessage.AddMessage($"Seatruck and Player Warped to:\n{targetName}\n({targetPos.ToString()})");
            }
            else
            {
                Player.main.SetPosition(targetPos.ToVector3());
                Player.main.OnPlayerPositionCheat();
                ErrorMessage.AddMessage($"Player Warped to:\n{targetName}\n({targetPos.ToString()})");
            }

            return currentWorldPos;
        }

        public void AddToList(IntVector3 worldPosition)
        {
            if (WarpTargets_Internal.ContainsKey(worldPosition))
            {
                ErrorMessage.AddMessage("CheatManagerZero message:\nThis position is already exist in internal Warp list.");
            }
            else if (WarpTargets_User.ContainsKey(worldPosition))
            {
                ErrorMessage.AddMessage("CheatManagerZero message:\nThis position is already exist in user Warp list.");
            }
            else
            {
                string name = $"User_{WarpTargets_User.Count + 1}_{Player.main.GetBiomeString()}";

                WarpTargets_User.Add(worldPosition, name);

                scrollItemsList[tMatrix.Length].AddGuiItemToGroup(name);

                ErrorMessage.AddMessage($"CheatManagerZero message:\nPosition added to user Warp list with name:\n{name}.");
            }
        }

        public void RemoveFormList(IntVector3 key)
        {
            if (WarpTargets_User.ContainsKey(key))
            {
                WarpTargets_User.Remove(key);

                ErrorMessage.AddMessage($"CheatManagerZero message:\nTarget position [{key}] removed from user Warp list.");
            }
        }

        public bool IsPositionWithinRange(IntVector3 position, out string nearestWarpPoint)
        {
            foreach (KeyValuePair<IntVector3, string> kvpInternal in WarpTargets_Internal)
            {
                if (IntVector3.Distance(kvpInternal.Key, position) < 50)
                {
                    nearestWarpPoint = kvpInternal.Value;
                    return true;
                }
            }

            foreach (KeyValuePair<IntVector3, string> kvpUser in WarpTargets_User)
            {
                if (IntVector3.Distance(kvpUser.Key, position) < 50)
                {
                    nearestWarpPoint = kvpUser.Value;
                    return true;
                }
            }

            nearestWarpPoint = string.Empty;
            return false;
        }
    }
}
