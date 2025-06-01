using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Player/Player Config", order = 1)]
public class PlayerConfig : ScriptableObject
{
    public GameObject player;
    public Vector3 offset;
}