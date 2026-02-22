using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    [SerializeField] private float _speed = 5f;
    
    [Networked] public int Health { get; set; } = 100;
    [Networked] public TickTimer StunTimer { get; set; }
    [Networked] public bool IsGrounded { get; set; }
    [Networked] public NetworkBool HasHelmet { get; set; }
    [Networked] public int HelmetHP { get; set; }
    
    [Networked] public NetworkObject GrabbedObject { get; set; }
    [Networked] public NetworkId GrabbedObjectId { get; set; }
    [Networked] public NetworkId GrabbedPlayerId { get; set; }

    public override void Spawned()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (StunTimer.ExpiredOrNotRunning(Runner))
        {
            if (GetInput(out NetworkInputData data))
            {
                data.direction.Normalize();
                _cc.Move(_speed * data.direction * Runner.DeltaTime);

                if (data.direction != Vector3.zero)
                {
                    transform.forward = data.direction;
                }

                var buttons = data.buttons;
                var prevButtons = GetInput(out NetworkInputData prevData) ? prevData.buttons : default;
                
                if (buttons.WasPressed(prevButtons, NetworkInputData.BUTTON_PUNCH))
                {
                    if (GrabbedObjectId.IsValid) UseHeldItem();
                    else if (GrabbedPlayerId.IsValid) ThrowGrabbedPlayer();
                    else Punch();
                }
                if (buttons.WasPressed(prevButtons, NetworkInputData.BUTTON_KICK)) Kick();
                if (buttons.WasPressed(prevButtons, NetworkInputData.BUTTON_GRAB)) Grab();
            }
        }

        if (GrabbedPlayerId.IsValid && Object.HasStateAuthority)
        {
            if (Runner.TryFindObject(GrabbedPlayerId, out var playerObj))
            {
                playerObj.transform.position = transform.position + transform.forward * 1f + Vector3.up * 1f;
            }
        }
    }

    void ThrowGrabbedPlayer()
    {
        if (Runner.TryFindObject(GrabbedPlayerId, out var playerObj))
        {
            if (playerObj.TryGetComponent<PlayerController>(out var otherPlayer))
            {
                otherPlayer.ApplyStun(1.0f);
                if (playerObj.TryGetComponent<NetworkCharacterController>(out var ncc))
                {
                    ncc.Move(transform.forward * 10f);
                }
            }
        }
        GrabbedPlayerId = default;
    }

    void UseHeldItem()
    {
        if (Runner.TryFindObject(GrabbedObjectId, out var netObj))
        {
            if (netObj.TryGetComponent<Item>(out var item))
            {
                item.Use(this);
                if (item.RemainingUses <= 0 && item.type != ItemType.Slipper)
                {
                    GrabbedObjectId = default;
                }
            }
        }
    }

    void Punch()
    {
        Debug.Log("Punching!");
        CheckHit(10, 1f);
    }

    void Kick()
    {
        Debug.Log("Kicking!");
        CheckHit(15, 1.5f);
    }

    void Grab()
    {
        if (GrabbedObjectId.IsValid || GrabbedPlayerId.IsValid)
        {
            // Drop it
            GrabbedObjectId = default;
            GrabbedPlayerId = default;
            return;
        }

        Debug.Log("Trying to grab!");
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, 1.5f))
        {
            if (hit.collider.TryGetComponent<Item>(out var item))
            {
                GrabbedObjectId = item.Object.Id;
                item.IsHeld = true;
                item.transform.parent = transform;
                item.transform.localPosition = new Vector3(0, 1, 0.5f);
            }
            else if (hit.collider.TryGetComponent<PlayerController>(out var otherPlayer))
            {
                GrabbedPlayerId = otherPlayer.Object.Id;
                otherPlayer.ApplyStun(2.0f);
            }
        }
    }

    void CheckHit(int damage, float range)
    {
        if (!Object.HasStateAuthority) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, range))
        {
            if (hit.collider.TryGetComponent<PlayerController>(out var otherPlayer))
            {
                otherPlayer.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damage, bool canStun = true)
    {
        if (HasHelmet)
        {
            damage /= 2;
            HelmetHP -= 10;
            if (HelmetHP <= 0)
            {
                HasHelmet = false;
                Debug.Log("Helmet Broke!");
            }
            if (!canStun) return; // Helmet prevents some stuns but if it's forced...
            // User said helmet prevents stuns
            return; 
        }

        Health -= damage;
        if (canStun)
        {
            StunTimer = TickTimer.CreateFromSeconds(Runner, 0.5f);
        }
    }
    
    public void ApplyStun(float duration)
    {
        if (HasHelmet) return;
        StunTimer = TickTimer.CreateFromSeconds(Runner, duration);
    }
}
