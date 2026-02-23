using Fusion;
using UnityEngine;

public enum ItemType { Hammer, Helmet, Slipper }

public class Item : NetworkBehaviour
{
    public ItemType type;
    [Networked] public int RemainingUses { get; set; }
    [Networked] public NetworkBool IsHeld { get; set; }
    
    public virtual void Use(PlayerController holder)
    {
        if (RemainingUses <= 0) 
        {
            Runner.Despawn(Object);
            return;
        }
        
        RemainingUses--;
        ExecuteEffect(holder);

        if (RemainingUses <= 0)
        {
            Runner.Despawn(Object);
        }
    }

    protected virtual void ExecuteEffect(PlayerController holder) { }
}
