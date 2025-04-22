namespace MealPlan;
using Spectre.Console;

public class ShoppingList {
        public List<Ingredient> Ingredients { get; set; }

        public ShoppingList() {
            List<Ingredient> ingredients = new List<Ingredient>();
            this.Ingredients = ingredients;
        }

        public Ingredient AddIngredientToShoppingList() {
            Ingredient ingredient = Ingredient.CreateInteractiveIngredient();
            this.Ingredients.Add(ingredient);
            return ingredient;
        }

        public void ShowIngredientsInShoppingList() {
            if (this.Ingredients.Count == 0) {
                Console.WriteLine("ShoppingList contains no ingredients");
                return;
            }
            foreach (Ingredient ingredient in Ingredients) {
                AnsiConsole.WriteLine(ingredient.name + " " + ingredient.amount + " " + ingredient.measurementType);
            }
        }

        public void RemoveIngredientsFromShoppingList() {
            if (this.Ingredients.Count == 0) {
                Console.WriteLine("ShoppingList contains no ingredients");
                return;
            }
            var removeIngredients = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Select [green]ingredients to remove[/]?")
                    .NotRequired() 
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more ingredients)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle an ingredient, " + 
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(this.Ingredients.Select(ingredient => ingredient.name)));
                            

            if (removeIngredients != null && removeIngredients.Count > 0)
            {
                var namesToRemoveSet = new HashSet<string>(removeIngredients);

                int removedCount = this.Ingredients.RemoveAll(ingredient =>
                    namesToRemoveSet.Contains(ingredient.name)
                );

                if (removedCount > 0)
                {
                    Console.WriteLine($"Removed {removedCount} ingredient(s).");
                }
                else
                {
                    Console.WriteLine("Selected ingredient(s) could not be found or removed.");
                }
            }
            else
            {
                Console.WriteLine("No ingredients selected for removal.");
            }
        }

        public void AddRecipeIngredientsToShoppingList(RecipeList recipeList)
        {
            if (recipeList == null || recipeList.Recipes == null || recipeList.Recipes.Count == 0)
            {
                Console.WriteLine("There are no recipes available to select from.");
                return;
            }

            var selectedRecipeName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a Recipe to add its ingredients to the shopping list")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more recipes)[/]")
                    .AddChoices(recipeList.Recipes.Select(recipe => recipe.Name ?? "Unnamed Recipe"))
            );

            Recipe foundRecipe = recipeList.Recipes.FirstOrDefault(recipe => recipe.Name == selectedRecipeName);

            if (foundRecipe != null) 
            {
                if (foundRecipe.Ingredients != null && foundRecipe.Ingredients.Count > 0)
                {
                    foreach (Ingredient ingredientFromRecipe in foundRecipe.Ingredients)
                    {
                        this.Ingredients.Add(ingredientFromRecipe);
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]Recipe '{foundRecipe.Name}' has no ingredients to add.[/]");
                }
            }
        }

public void MoveIngredientsFromShoppingListToPantry(Pantry pantry)
{
    if (this.Ingredients == null || this.Ingredients.Count == 0)
    {
        Console.WriteLine("ShoppingList contains no ingredients");
        return;
    }

    var selectedIngredientNames = AnsiConsole.Prompt(
        new MultiSelectionPrompt<string>()
            .Title("Select [green]ingredients to move[/]?")
            .NotRequired()
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more ingredients)[/]")
            .InstructionsText(
                "[grey](Press [blue]<space>[/] to toggle an ingredient, " +
                "[green]<enter>[/] to accept)[/]")
            .AddChoices(this.Ingredients.Select(ingredient => ingredient.name)));

    if (selectedIngredientNames != null && selectedIngredientNames.Count > 0)
    {
        var namesToMoveSet = new HashSet<string>(selectedIngredientNames);

        List<Ingredient> ingredientsToMove = this.Ingredients
            .Where(ingredient => namesToMoveSet.Contains(ingredient.name)) 
            .ToList();

        if (ingredientsToMove.Count > 0)
        {
            int moveCount = pantry.AddIngredientsToPantryFromShoppingList(ingredientsToMove);

            if (moveCount > 0)
            {
                Console.WriteLine($"Successfully added {moveCount} ingredient(s) to Pantry.");

                int removedCount = this.Ingredients.RemoveAll(ingredient => namesToMoveSet.Contains(ingredient.name));
                Console.WriteLine($"Removed {removedCount} ingredient(s) from Shopping List.");
                 if(moveCount != removedCount) {
                     AnsiConsole.MarkupLine($"[yellow]Warning:[/]");
                     Console.WriteLine($" Mismatch between count added to pantry ({moveCount}) and count removed from shopping list ({removedCount}).");
                 }
            }
            else
            {
                Console.WriteLine("Selected ingredient(s) were not added to the pantry (perhaps duplicates?).");
            }
        }
        else
        {
             Console.WriteLine("Could not find ingredient objects matching the selected names in the shopping list.");
        }
    }
    else
    {
        Console.WriteLine("No ingredients selected to move.");
    }
}
    public void SaveShoppingListCollection(ShoppingList shoppingList) {
        var shoppingListFileSaver = new FileSaver(FileSaver.shoppingListFile);
        shoppingListFileSaver.SaveCollection(shoppingList.Ingredients);
    }

    public List<Ingredient> LoadShoppingListCollection() {
        var shoppingListFileSaver = new FileSaver(FileSaver.shoppingListFile);
        return shoppingListFileSaver.LoadCollection<Ingredient>();
    }

}
