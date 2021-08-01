using UnityEngine;

public class BaseEffect : MonoBehaviour
{
    [SerializeField] protected GridHandler _grid;

    protected Game _game;

    private void Awake()
    {
        _game = Game.Instance;
    }

    public virtual void ApplyEffect()
    {
    }
}
