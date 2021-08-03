using GramGames.CraftingSystem.DataContainers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
	#region fields

	[SerializeField] private int _numOfRows;
	[SerializeField] private int _numOfColumns;
	[SerializeField] private float _tileSpacing = 1f;
	[SerializeField] private GameObject _cellPrefab;
	[SerializeField] private GameObject _gridPlayArea;

	// caching
	[SerializeField] protected List<GridCell> _emptyCells;
	[SerializeField] protected List<GridCell> _fullCells;
	public List<GridCell> GetFullCells => _fullCells;
	public List<GridCell> GetEmptyCells => _emptyCells;

	// chance of the grid cell spawning an item (0 = 0%, 1 = 100%)
	[Range(0, 1)]
	[field: SerializeField] public float ItemDensity = 1;
	[field: SerializeField] public List<NodeContainer> ItemsToSpawn = new List<NodeContainer>();

	#endregion

	#region initialization

	private void Awake()
	{
		ClearExistingCells();

		RectTransform rectTransform = _gridPlayArea.GetComponent<RectTransform>();
		BoxCollider2D cellBoxCollider = _cellPrefab.GetComponent<BoxCollider2D>();
		GameObject cell;
		GridCell gridCell;
		List<GridCell> gridCells = new List<GridCell>();
		float posX;
		float posY;

		// We want the upper left position of the grid's play area to start instantiating the cells from
		// https://stackoverflow.com/questions/43864931/find-the-upper-left-position-of-a-canvas/43865340
		float minX = rectTransform.position.x + rectTransform.rect.xMin;
		float maxY = rectTransform.position.y + rectTransform.rect.yMax;

		// Need to convert to world space then offset it by half the size of the cell so it fits inside the grid
		Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(minX, maxY, 0)) + new Vector3(cellBoxCollider.size.x / 2, -cellBoxCollider.size.y / 2, 0);
		topLeft.z = 0;
		
		for (int i = 0; i < _numOfRows; i++)
		{
			for (int j = 0; j < _numOfColumns; j++)
			{
				cell = Instantiate(_cellPrefab, transform);

				posX = j * _tileSpacing;
				posY = i * _tileSpacing;

				// Start placing the cells from the top left corner
				cell.transform.position = new Vector2(topLeft.x + posX, topLeft.y - posY);

				gridCell = cell.GetComponent<GridCell>();

				gridCell.SetRowIndex(i);
				gridCells.Add(gridCell);
			}
		}

		for (int i = 0; i < _numOfRows; i++)
		{
			//each row
			List<GridCell> currentRow = gridCells.Where(cell => cell.RowIndex == i).ToList();
			List<GridCell> upperRow = i > 0 ? gridCells.Where(cell => cell.RowIndex == i - 1).ToList() : null;
			List<GridCell> lowerRow = i + 1 < _numOfRows ? gridCells.Where(cell => cell.RowIndex == i + 1).ToList() : null;

			for (int j = 0; j < currentRow.Count; j++)
			{
				// each cell
				currentRow[j].SetNeighbor(upperRow?[j], MoveDirection.Up);
				currentRow[j].SetNeighbor(lowerRow?[j], MoveDirection.Down);

				var leftN = j > 0 ? currentRow[j - 1] : null;
				currentRow[j].SetNeighbor(leftN, MoveDirection.Left);

				var rightN = j < currentRow.Count - 1 ? currentRow?[j + 1] : null;
				currentRow[j].SetNeighbor(rightN, MoveDirection.Right);
				currentRow[j].SetHandler(this);

				// cache the cell as empty
				_emptyCells.Add(currentRow[j]);
			}
		}
	}

	private void ClearExistingCells()
	{
		_emptyCells = new List<GridCell>();
		_fullCells = new List<GridCell>();
	}

	#endregion

	#region helpers

	public void AddMergeableItemToEmpty(MergableItem item)
	{
		var cell = _emptyCells.FirstOrDefault();
		if (cell != null)
		{
			item.AssignToCell(cell);
		}
	}

	public void ClearCell(GridCell cell)
	{
		if (_fullCells.Contains(cell))
		{
			_fullCells.Remove(cell);
			cell.ClearItem();
		}
		
		if (!_emptyCells.Contains(cell))
			_emptyCells.Add(cell);
	}

	public void SetCellState(GridCell cell, bool empty)
	{
		if (cell == null) return;
		if (empty)
		{
			if (_fullCells.Contains(cell))
			{
				_fullCells.Remove(cell);
			}

			if (_emptyCells.Contains(cell) == false)
			{
				_emptyCells.Add(cell);
			}
		}
		else
		{
			if (_emptyCells.Contains(cell))
			{
				_emptyCells.Remove(cell);
			}

			if (_fullCells.Contains(cell) == false)
			{
				_fullCells.Add(cell);
			}
		}
	}

	#endregion
}
