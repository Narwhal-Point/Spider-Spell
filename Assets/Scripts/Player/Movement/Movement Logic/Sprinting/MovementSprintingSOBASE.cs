using Player.Movement;
using UnityEngine;

public class MovementSprintingSOBASE : ScriptableObject
{
    protected PlayerMovement player;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;

    public virtual void Initialize(GameObject gameObject, PlayerMovement player)
    {
        this.gameObject = gameObject;
        this.player = player;
        this.transform = gameObject.transform;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    public virtual void DoEnterLogic() {}
    public virtual void DoExitLogic() {}
    public virtual void DoUpdateLogic() {}
    public virtual void DoFixedUpdateLogic() {}
}
