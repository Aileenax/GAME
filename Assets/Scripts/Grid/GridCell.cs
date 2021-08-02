using GramGames.CraftingSystem.DataContainers;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private GridCell _left;
    [SerializeField] private GridCell _right;
    [SerializeField] private GridCell _up;
    [SerializeField] private GridCell _down;
    [SerializeField] private GridHandler _handler;
    
    [field: SerializeField] public MergableItem Item { get; private set; }

    private Game _game;

    private void Awake()
    {
        _game = Game.Instance;
    }

    public void SpawnItem(NodeContainer item)
    {
	    _handler.ClearCell(this);

        MergableItem obj = Instantiate(_game.DraggableObjectPrefab);
	    obj.Configure(item, this);
    }
    
    public void SetHandler(GridHandler handler)
    {
        _handler = handler;
    }

    public void SetItemAssigned(MergableItem item)
    {
	    if (_handler == null)
		    _handler = GetComponentInParent<GridHandler>();
       
        if (Item != null)
            _handler.SetCellState(Item.GetCurrentCell(), true);
        
        Item = item;
        _handler.SetCellState(this, Item != null);
    }

    public void SetNeighbor(GridCell neighbor, MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Left:
                _left = neighbor;
                break;
            case MoveDirection.Right:
                _right = neighbor;
                break;
            case MoveDirection.Up:
                _up = neighbor;
                break;
            case MoveDirection.Down:
                _down = neighbor;
                break;  
        }
    }

    public bool HasItem()
    {
        return Item != null;
    }

    public void ClearItem()
    {
        if (Item != null)
        {
            Destroy(Item.gameObject);
            Item = null;
        }
    }

    public void EmptyCell()
    {
        Item = null;
    }
}

public enum MoveDirection
{
    Left, 
    Right, 
    Up, 
    Down
}
