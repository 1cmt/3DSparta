using UnityEngine;

public class JumpGate : MonoBehaviour
{
    public float jumpPower = 80.0f;

    private void OnTriggerStay(Collider other)
    {
        CharacterManager.Instance.Player.controller.Rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
    }
}
