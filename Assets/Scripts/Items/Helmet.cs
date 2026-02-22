using Fusion;
using UnityEngine;

public class Helmet : Item
{
    public int HP = 50;

    protected override void ExecuteEffect(PlayerController holder)
    {
        holder.HasHelmet = true;
        holder.HelmetHP = HP;
        // The item is consumed when put on
        RemainingUses = 0;
    }
}
