using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GramGames.CraftingSystem.DataContainers;

public class Game : Singleton<Game>
{
	[field: SerializeField] public MergableItem DraggableObjectPrefab { get; private set; }
	[field: SerializeField] public GridHandler MainGrid { get; private set; }

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
		float randomValue;
		NodeContainer chosenItem;
		NodeData chosenIngredient;
		NodeContainer itemToSpawn;
		System.Random randomItem = new System.Random();
		System.Random randomIngredient = new System.Random();

		// clear the board
		GridCell[] fullCells = MainGrid.GetFullCells.ToArray();

		for (int i = fullCells.Length - 1; i >= 0; i--)
			MainGrid.ClearCell(fullCells[i]);

		// choose new recipes
		_activeRecipes.Clear();
		difficulty = Mathf.Max(difficulty, 1);

		// populate the board
		GridCell[] emptyCells = MainGrid.GetEmptyCells.ToArray();

		foreach (GridCell cell in emptyCells)
		{
			randomValue = Random.value;

			// GridCell should spawn an item
			if (randomValue <= MainGrid.ItemDensity)
			{
				chosenItem = MainGrid.ItemsToSpawn.OrderBy(item => randomItem.Next()).FirstOrDefault();

				// a 'recipe' has more than 1 ingredient, else it is just a raw ingredient.
				if (chosenItem.NodeLinks.Count > 1)
				{
					// Choose a random ingredient from the recipe to spawn into the GridCell
					chosenIngredient = ItemUtils.RecipeMap[chosenItem.MainNodeData.NodeGUID].OrderBy(ingredients => randomIngredient.Next()).FirstOrDefault();
					itemToSpawn = ItemUtils.ItemsMap[chosenIngredient.NodeGUID];
				}
				// This is just a raw ingredient so spawn that into the GridCell
				else
				{
					itemToSpawn = chosenItem;
				}

				cell.SpawnItem(itemToSpawn);
			}
		}
	}
}
