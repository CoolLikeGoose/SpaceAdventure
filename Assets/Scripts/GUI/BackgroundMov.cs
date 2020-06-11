using UnityEngine;

/// <summary>
/// Background looping
/// </summary>
public class BackgroundMov : MonoBehaviour
{
    private void Update()
    {
        transform.Translate(new Vector2(0, -2.5f * Time.deltaTime * GameController.Instance.gameSpeed));
        if (transform.position.y < -5)
        {
            transform.position = new Vector2(transform.position.x, 5);
        }   
    }
}
