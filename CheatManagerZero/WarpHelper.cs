using BZCommon;
using BZCommon.Helpers.GUIHelper;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CheatManagerZero
{
    public partial class CheatManagerZero
    {
        protected readonly Dictionary<IntVector, string> WarpTargets_Internal = new Dictionary<IntVector, string>()
        {
            { new IntVector(-265, -0, 93),       "Shallow Twisty Bridges"},
            { new IntVector(39, 0, 300),         "Research Base Zero entry pool"},
            //{ new IntVector(-209, 26, 523),      "Alien Research Site Zero"},
            { new IntVector(-224, -99, -250),    "Twisty Bridges"},
            { new IntVector(333, -111, -1075),   "Lilypad Islands"},
            { new IntVector(-498, -35, -60),     "Arctic Kelp Forest"},
            { new IntVector(-532, -80, -89),     "Arctic Kelp Caves"},
            { new IntVector(-1289, 48, 62),      "Gandalf's hats (Ice)"},
            { new IntVector(-1889, 98, 21),      "Gandalf's hat (Rock)"},
            { new IntVector(-1336, 44, -709),    "The Bridge"},
            { new IntVector(-1607, 32, -727),    "The Bridge 2"},
            { new IntVector(561, -198, -1051),   "Research Base Omega"},
            { new IntVector(-254, 42, -770),     "Cargo Rocket Island"},
            { new IntVector(-583, -26, -19),     "Emergency Supply Cache 1"},
            { new IntVector(-377, -164, -334),   "Emergency Supply Cache 2"},            
            { new IntVector(-248, -117, -265),   "Emergency Supply Cache 3"},
            { new IntVector(-136, -55, -182),    "Emergency Supply Cache 4"},
            { new IntVector(-557, -199, -489),   "Precursor Computer Core Entrance"},
            { new IntVector(544, -607, -1050),   "Precursor Cave (Lilypads Deep)"},
            { new IntVector(49, -104, -916),     "Starship Wreck 1"},
            { new IntVector(289, -221, -1228),   "Starship Wreck 2"},
            { new IntVector(446, -145, -636),    "Starship Wreck 3"},            
            
            
            { new IntVector(95, -20, -16),       "Research Platform: Sparse Arctic"},
            { new IntVector(-1195, 0, -1053),    "Glacier Basin Exit Pool"},
            { new IntVector(-391, -143, -825),   "Human Mine Entrance"},
            { new IntVector(-80, -405, -940),    "Crystal Cave Entrance (Tree Spires)"},
            { new IntVector(92, -365, -940),     "Maghda's Base"},
            { new IntVector(284, -173, -904),    "Cave entrance (Lily Pads)"},
            { new IntVector(776, 0, 241),        "Glacial Connection"},
            { new IntVector(1221, -945, -308),   "Precursor Base (Fabricator Caverns)"},            
            { new IntVector(-1859, 21, -151),    "Precursor Base (Tundra Void)"},
            { new IntVector(554, -609, -1054),   "Precursor Base (Lily Pads Deep)"},
            { new IntVector(-508, -139, -64),    "Precursor Modular Monument (Arctic Kelp - Cave Inner)"},
            { new IntVector(-260, -298, -612),   "Precursor Statue (Water)"},
            { new IntVector(-721, -220, -601),   "Precursor Obelisk (Arctic Kelp - Cave Inner)"},
            { new IntVector(829, -200, -1107),   "Precursor Ground Sampler 1 (Lily Pads)"},
            //{ new IntVector(-1796, 46, -257),    "Precursor Technology 3"},
            //{ new IntVector(-1650, 35, -520),    "Precursor Technology 4"},
            { new IntVector(-112, -569, -1424),  "Precursor Ground Sampler 2 (Tree Spires)"},
            { new IntVector(23, -120, -473),     "Precursor Mineral Distiller (Thermal Spires)"},
            { new IntVector(242, -149, -426),    "Precursor Water Analyzer 1 (Purple Vents)"},
            { new IntVector(195, -170, -31),     "Precursor Water Analyzer 2 (Arctic Kelp - Cave Inner)"},
            { new IntVector(906, 0, -700),       "Precursor Satellite (Land)"},
            //{ new IntVector(-1821, 37, -746),    "Human Tech Site"},
            //{ new IntVector(-1570, 61, -149),    "Human Tech Site 2"},
            { new IntVector(1525, 1, -376),      "The Beach"},
            { new IntVector(-334, -330, -373),   "Twisty Bridges Deep"},
            
        };

        public readonly Dictionary<IntVector, string> WarpTargets_User = new Dictionary<IntVector, string>();

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

        public IntVector GetIntVector(int index)
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

        public IntVector Teleport(string targetName, IntVector targetPos)
        {
            IntVector currentWorldPos = Player.main.transform.position;

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

        public void AddToList(IntVector worldPosition)
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

        public void RemoveFormList(IntVector key)
        {
            if (WarpTargets_User.ContainsKey(key))
            {
                //print($"WarpTargets_User.Count before: {WarpTargets_User.Count}");

                WarpTargets_User.Remove(key);

                ErrorMessage.AddMessage($"CheatManagerZero message:\nTarget position [{key}] removed from user Warp list.");

                //print($"WarpTargets_User.Count after: {WarpTargets_User.Count}");
            }
        }

        public bool IsPositionWithinRange(IntVector position, out string nearestWarpPoint)
        {
            foreach (KeyValuePair<IntVector, string> kvpInternal in WarpTargets_Internal)
            {
                if (IntVector.Distance(kvpInternal.Key, position) < 50)
                {
                    nearestWarpPoint = kvpInternal.Value;
                    return true;
                }
            }

            foreach (KeyValuePair<IntVector, string> kvpUser in WarpTargets_User)
            {
                if (IntVector.Distance(kvpUser.Key, position) < 50)
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
