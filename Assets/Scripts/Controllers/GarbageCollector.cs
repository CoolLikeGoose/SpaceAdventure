using UnityEngine;

/// <summary>
/// Delete all garbage from scene
/// </summary>
public class GarbageCollector : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
    }
}
