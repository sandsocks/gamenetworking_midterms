using Fusion;
using UnityEngine;

public class Hammer : Item
{
    public float Damage = 30f;
    public float StunDuration = 1.5f;

    protected override void ExecuteEffect(PlayerController holder)
    {
        RaycastHit hit;
        if (Physics.Raycast(holder.transform.position + Vector3.up, holder.transform.forward, out hit, 1.5f))
        {
            if (hit.collider.TryGetComponent<PlayerController>(out var otherPlayer))
            {
                otherPlayer.TakeDamage((int)Damage, true);
                otherPlayer.ApplyStun(StunDuration);
            }
        }
    }
}
