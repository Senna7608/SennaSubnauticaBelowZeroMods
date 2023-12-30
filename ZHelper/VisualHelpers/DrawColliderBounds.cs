using System.Collections.Generic;
using UnityEngine;
using ZHelper.Helpers;

namespace ZHelper.VisualHelpers
{
    
    public class DrawColliderBounds : MonoBehaviour
    {
        public int cInstanceID;
        public Collider colliderBase;
        public ColliderInfo colliderInfo;

        public IDictionary<int, ColliderInfo> colliderOriginals = new Dictionary<int, ColliderInfo>();

        public GameObject Box_Base, Capsule_Base, Sphere_Base, Mesh_Base;

        private List<GameObject> lineContainers_Box = new List<GameObject>();
        private List<GameObject> lineContainers_Capsule = new List<GameObject>();
        private List<GameObject> lineContainers_Sphere = new List<GameObject>();
        private List<GameObject> triangleContainers_Mesh = new List<GameObject>();
        private bool isMeshReadable = true;

        public void Awake()
        {
            Box_Base = new GameObject("Box_Base");
            Box_Base.transform.SetParent(transform);
            Utils.ZeroTransform(Box_Base.transform);

            Capsule_Base = new GameObject("Capsule_Base");
            Capsule_Base.transform.SetParent(transform);
            Utils.ZeroTransform(Capsule_Base.transform);

            Sphere_Base = new GameObject("Sphere_Base");
            Sphere_Base.transform.SetParent(transform);
            Utils.ZeroTransform(Sphere_Base.transform);

            Mesh_Base = new GameObject("Mesh_Base");
            Mesh_Base.transform.SetParent(transform);
            Utils.ZeroTransform(Mesh_Base.transform);

            Box_Base.CreateLineContainers(ref lineContainers_Box, ContainerType.Box, 0.008f, Color.red, false);

            Capsule_Base.CreateLineContainers(ref lineContainers_Capsule, ContainerType.Capsule, 0.004f, Color.red, false);

            Sphere_Base.CreateLineContainers(ref lineContainers_Sphere, ContainerType.Sphere, 0.004f, Color.red, false);
        }

        public void DrawCollider(int colliderID)
        {
            if (colliderID != 0)
            {
                foreach (Collider collider in transform.parent.parent.GetComponents<Collider>())
                {
                    int cInstanceID = collider.GetInstanceID();

                    if (!colliderOriginals.ContainsKey(cInstanceID))
                    {
                        colliderOriginals.Add(cInstanceID, new ColliderInfo(collider));
                    }

                    if (cInstanceID == colliderID)
                    {
                        colliderBase = collider;
                    }
                }
            }
        }

        public void EnableColliderDrawing()
        {
            DisableColliderDrawing();

            switch (colliderInfo.ColliderType)
            {
                case ColliderType.BoxCollider:
                    Box_Base.SetActive(true);
                    break;
                case ColliderType.CapsuleCollider:
                    Capsule_Base.SetActive(true);
                    break;
                case ColliderType.SphereCollider:
                    Sphere_Base.SetActive(true);
                    break;
                case ColliderType.MeshCollider:
                    Mesh_Base.SetActive(isMeshReadable);
                    break;
            }
        }

        public void DisableColliderDrawing()
        {
            Box_Base.SetActive(false);
            Capsule_Base.SetActive(false);
            Sphere_Base.SetActive(false);
            Mesh_Base.SetActive(false);
        }


        private void CheckTriangleContainers()
        {
            MeshCollider meshCollider = (MeshCollider)colliderBase;

            if (triangleContainers_Mesh.Count != meshCollider.sharedMesh.triangles.Length)
            {
                DestroyTriangleContainers();

                CreateTriangleContainers(meshCollider.sharedMesh.triangles.Length);
            }
        }

        private void DestroyTriangleContainers()
        {
            //RHZLogger.Debug($"Destroying [{triangleContainers_Mesh.Count}] triangle containers...");

            foreach (GameObject triangleContainer in triangleContainers_Mesh)
            {
                DestroyImmediate(triangleContainer);
            }

            triangleContainers_Mesh.Clear();
        }

        private void CreateTriangleContainers(int containerCount)
        {
            int i;

            for (i = 0; i < containerCount; i++)
            {
                triangleContainers_Mesh.Add(new GameObject { name = $"RHZ_MESH_triangle_{i}" });
                triangleContainers_Mesh[i].SetContainerTransform(Mesh_Base);
                triangleContainers_Mesh[i].AddLineRendererToContainer(0.008f, Color.red, false);
            }

            //RHZLogger.Debug($"Created [{i}] triangle container.");
        }

        public void Update()
        {
            if (!Main.Instance.isColliderSelected)
            {
                DisableColliderDrawing();
                return;
            }

            colliderBase.SetColliderInfo(ref colliderInfo);

            EnableColliderDrawing();

            switch (colliderInfo.ColliderType)
            {
                case ColliderType.BoxCollider:
                    Box_Base.DrawBoxColliderBounds(ref lineContainers_Box, colliderInfo);
                    break;
                case ColliderType.CapsuleCollider:
                    Capsule_Base.DrawCapsuleColliderBounds(ref lineContainers_Capsule, colliderInfo);
                    break;
                case ColliderType.SphereCollider:
                    Sphere_Base.DrawSphereColliderBounds(ref lineContainers_Sphere, colliderInfo);
                    break;

                case ColliderType.MeshCollider:

                    MeshCollider meshCollider = (MeshCollider)colliderBase;

                    if (!meshCollider.sharedMesh.isReadable)
                    {
                        if (isMeshReadable)
                        {
                            //RHZLogger.Error("Cannot draw mesh collider! Mesh is not readable!");
                        }

                        isMeshReadable = false;
                        return;
                    }

                    isMeshReadable = true;
                    CheckTriangleContainers();
                    Mesh_Base.DrawMeshColliderBounds(ref triangleContainers_Mesh, (MeshCollider)colliderBase);
                    break;
            }
        }
    }
}