using UnityEngine;
using System;

public class GameInput : MonoBehaviour
{

    // Etkinlik tanýmý
    public event EventHandler OnInteractAction;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Eðer birisi bu etkinliði dinliyorsa (null deðilse) ateþle
            OnInteractAction?.Invoke(this, EventArgs.Empty);
        }
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W)) inputVector.y = +1;
        if (Input.GetKey(KeyCode.S)) inputVector.y = -1;
        if (Input.GetKey(KeyCode.A)) inputVector.x = -1;
        if (Input.GetKey(KeyCode.D)) inputVector.x = +1;

        return inputVector.normalized;
    }
}