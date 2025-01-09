using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "Scriptable Objects/MovementConfig")]
public class MovementConfig : ScriptableObject
{
    public float speed = 5f;
    public float turnSmoothTime = 0.2f;
    public float rotationSmoothTime = 0.2f;
    public float drag = 0.2f;
    public Vector3 initPos = new Vector3(0,0.901f,0);
}
