using GramGames.CraftingSystem.DataContainers;
using UnityEngine;

public class MergableItem : DraggableObject
{
    [SerializeField] private LayerMask _mask;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private GridCell _parentCell;
    private RayCastHandler<GridCell> _rayCast;

    public NodeContainer ItemData { get; private set; }
    
    private void Awake()
    {
	    _rayCast = new RayCastHandler<GridCell>(transform);
    }

    /// <summary>
    /// Should be called immediately after instantiation!
    /// </summary>
    public void Configure(NodeContainer item, GridCell current)
    {
	    ItemData = item;
	    
	    if (ItemData != null)
	        _spriteRenderer.sprite = ItemData.MainNodeData.Sprite;
        else
	        _spriteRenderer.sprite = null;
	    
	    AssignToCell(current);
    }
    
    public void AssignToCell(GridCell current)
    {
        _parentCell = current;
        transform.SetParent(current.transform);
        transform.position = current.transform.position;
        current.SetItemAssigned(this);
    }

    protected override void DoBeginDrag()
    {
    }

    protected override void DoEndDrag()
    {
        if (_rayCast.RayCastDown(_mask))
        {
            //we hit a slot
            var targets = _rayCast.GetTargets();
            foreach (var slot in targets)
            {
                if (slot.HasItem() == false)
                {
                    // Remove the item from the original cell before moving it to the new one
                    _parentCell.EmptyCell();
                    AssignToCell(slot);
                    return;
                }
            }
            // TODO: else what do we do?
        }
        else
        {
            //return to previous slot
            if (_parentCell != null)
            {
                AssignToCell(_parentCell);
            }
        }
    }

    public GridCell GetCurrentCell()
    {
        return _parentCell;
    }
}
