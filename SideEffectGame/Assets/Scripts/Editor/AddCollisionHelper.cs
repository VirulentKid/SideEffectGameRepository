//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AddCollisionBox : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem.Controls;

public class AddCollisionBox
{
    [MenuItem("Tools/添加碰撞体/一键添加所有碰撞盒")]
    private static void Anchor_AddCollisionBoxForMeshes()
    {
        if (Selection.activeObject)
        {
            AddColliderForMeshes((GameObject)Selection.activeObject);
        }

    }

    public static void AddColliderForMeshes(GameObject obj)
    {
        if (obj.transform.childCount > 0)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                if (child.GetComponent<MeshRenderer>())
                {
                    child.gameObject.AddComponent<MeshCollider>();
                }

                AddColliderForMeshes(child.gameObject);
            }
        }
    }
}

public class DestoryColliders : EditorWindow
{
    [MenuItem("Tools/删除碰撞体/一键清理所有Collider")]
    public static void Anchor_ClearTreeCollider()
    {
        if (Selection.activeObject)
        {
            ClearMeshColliderByChild((GameObject)Selection.activeObject, false);
        }
    }
    [MenuItem("Tools/删除碰撞体/一键清理骨骼的所有Collider")]
    public static void Anchor_ClearTreeSkeletonCollider()
    {
        if (Selection.activeObject)
        {
            ClearMeshColliderByChild((GameObject)Selection.activeObject, true);
        }
    }
    public static void ClearMeshColliderByChild(GameObject obj, bool destroyObject = false)
    {
        if (obj.transform.childCount > 0)
        {
            List<GameObject> colliderObjectsToDestroy = new List<GameObject>();
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                //MeshCollider meshCollider = child.GetComponent<MeshCollider>();
                //if (meshCollider != null)
                //{
                //    DestroyImmediate(meshCollider);
                //}
                //BoxCollider boxCollider = child.GetComponent<BoxCollider>();
                //if (boxCollider != null)
                //{
                //    Debug.Log(boxCollider.name);
                //    DestroyImmediate(boxCollider);
                //}
                ClearMeshColliderByChild(child, destroyObject);
                Collider[] colliders = child.GetComponents<Collider>();
                if (!destroyObject)
                {
                    foreach (Collider collider in colliders)
                    {
                        Debug.Log($"Clear {collider.name}");
                        DestroyImmediate(collider);
                    }
                }
                else if (colliders.Length > 0)
                {
                    colliderObjectsToDestroy.Add(child.gameObject);
                }
            }

            foreach (GameObject child in colliderObjectsToDestroy)
            {
                Debug.Log($"Clear {child.gameObject.name}");
                DestroyImmediate(child.gameObject);
            }
        }
    }
}

public class AddCollisionCapsule
{
    [MenuItem("Tools/添加碰撞体/一键给骨骼添加胶囊体，半径为20")]
    private static void Anchor_AddCollisionCapsuleForSkeleton()
    {
        if (Selection.activeObject)
        {
            AddColliderForSkeleton((GameObject)Selection.activeObject, 20f);
        }
    }

    public static void AddColliderForSkeleton(GameObject obj, float radius)
    {
        Transform parent = obj.transform.parent;
        if (parent != null && parent.childCount > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform sibling = parent.GetChild(i);
                SkinnedMeshRenderer mesh = sibling.GetComponent<SkinnedMeshRenderer>();
                if (mesh != null)
                {
                    AddColliderForSkeletonInternal(obj, mesh, radius);
                }
            }
        }
    }

    protected static void AddColliderForSkeletonInternal(GameObject obj, SkinnedMeshRenderer mesh, float radius)
    {
        if (obj.transform.childCount > 0)
        {
            int previousChildCount = obj.transform.childCount;
            for (int i = 0; i < previousChildCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                GameObject colliderObject = new GameObject($"Collider:{child.name}");
                colliderObject.transform.parent = obj.transform;
                colliderObject.transform.localPosition = Vector3.zero;
                colliderObject.transform.localRotation = Quaternion.identity;
                colliderObject.transform.localScale = Vector3.one;

                CapsuleCollider collider = colliderObject.AddComponent<CapsuleCollider>();
                collider.center = (child.localPosition) * 0.5f;
                collider.height = (child.localPosition).magnitude;
                collider.radius = radius;
                collider.direction = 0;

                AddColliderForSkeletonInternal(child.gameObject, mesh, radius);
            }
        }
    }
}

