using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteraction : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null)
            {
                // Destroy Trash
                if (hit.collider.GetComponent<TrashBehaviour>())
                {
                    GameManager.Instance.AddScore(10);
                    Destroy(hit.collider.gameObject);
                    return;
                }

                // Trigger Fish
                if (hit.collider.GetComponent<FishBehaviour>())
                {
                    GameManager.Instance.MinScore(2);
                    hit.collider.GetComponent<FishBehaviour>().TriggerFear(worldPos);
                    return;
                }
            }

            // Spawn Food
            FoodObjectPooling.Instance.SpawnFood(worldPos);
        }
    }
}