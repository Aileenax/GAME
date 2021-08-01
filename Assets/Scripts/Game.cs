using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GramGames.CraftingSystem.DataContainers;

public class Game : Singleton<Game>
{
	public MergableItem DraggableObjectPrefab;
	public GridHandler MainGrid;

	private List<string> _activeRecipes = new List<string>();

	protected override void Awake()
	{
		base.Awake();

		Screen.fullScreen =
			false; // https://issuetracker.unity3d.com/issues/game-is-not-built-in-windowed-mode-when-changing-the-build-settings-from-exclusive-fullscreen

		// load all item definitions
		ItemUtils.InitializeMap();
	}

	private void Start()
	{
		ReloadLevel(1);
	}

	public void ReloadLevel(int difficulty = 1)
	{
		// clear the board
		GridCell[] fullCells = MainGrid.GetFullCells.ToArray();

		for (int i = fullCells.Length - 1; i >= 0; i--)
			MainGrid.ClearCell(fullCells[i]);

		// choose new recipes
		_activeRecipes.Clear();
		difficulty = Mathf.Max(difficulty, 1);

		for (int i = 0; i < difficulty; i++)
		{
			// a 'recipe' has more than 1 ingredient, else it is just a raw ingredient.
			string recipe = ItemUtils.RecipeMap.FirstOrDefault(kvp => kvp.Value.Count > 1).Key;
			_activeRecipes.Add(recipe);
		}

		// populate the board
		GridCell[] emptyCells = MainGrid.GetEmptyCells.ToArray();

		foreach (GridCell cell in emptyCells)
		{
			string chosenRecipe = _activeRecipes[0];
			NodeData[] ingredients = ItemUtils.RecipeMap[chosenRecipe].ToArray();
			NodeData ingredient = ingredients[0];
			NodeContainer item = ItemUtils.ItemsMap[ingredient.NodeGUID];

			cell.SpawnItem(item);
		}
	}
}
