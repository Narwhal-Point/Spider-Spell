using UnityEngine;

public class MovementIdleSOBASE : ScriptableObject
{
    protected PlayerMovement playerMovement;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;

    public virtual void Initialize(GameObject gameObject, PlayerMovement playerMovement)
    {
        this.gameObject = gameObject;
        this.playerMovement = playerMovement;
        this.gameObject = gameObject;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    public virtual void DoEnterLogic() {}
    public virtual void DoExitLogic() {}
    public virtual void DoUpdateLogic() {}
    public virtual void DoFixedUpdateLogic() {}
    
}
