using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCollideable : EntityCollideable<CharacterMovement>
{
    public static bool IsCollisionPlayer(Collider2D other, out CharacterMovement entity)
    {
        return IsCollisionEntity(other, out entity, GameConstants.PLAYER_TAG);
    }
}
