using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    // Variables control the various actions the player can perform at any time.

    public bool isMoving;
    public bool isJumping;
    public bool isDashing;
    public bool isRecoilingX;
    public bool isRecoilingY;
    public bool isInvinsible;
    public bool isWallJumping;
    public bool isSliding;
    public bool isFacingRight = true;
    public bool isEnteringCutscene = false;
    public bool isAlive = true;
    public bool isAttackingMelee;
    public bool isAttackingBow;
}
