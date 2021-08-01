
public class DishEffect : BaseEffect
{
    public override void ApplyEffect()
    {
        // repopulate
        _game.ReloadLevel();
    }
}
