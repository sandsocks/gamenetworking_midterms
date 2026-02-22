using Fusion;
using UnityEngine;

public class Slipper : Item
{
    public float ThrowForce = 20f;
    [Networked] public NetworkBool IsThrown { get; set; }

    protected override void ExecuteEffect(PlayerController holder)
    {
        IsThrown = true;
        IsHeld = false;
        transform.parent = null;
        
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.AddForce(holder.transform.forward * ThrowForce, ForceMode.Impulse);
        }
        
        // Slipper is usually a one-time throw until picked up or destroyed
        RemainingUses = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsThrown || !Runner.IsServer) return;

        if (collision.collider.TryGetComponent<PlayerController>(out var otherPlayer))
        {
            otherPlayer.ApplyStun(2.0f);
            // Throw back effect
            if (collision.collider.TryGetComponent<NetworkCharacterController>(out var ncc))
            {
                ncc.Move(transform.forward * 5f);
            }
        }
        
        // Destroy or reset slipper
        Runner.Despawn(Object);
    }
}
