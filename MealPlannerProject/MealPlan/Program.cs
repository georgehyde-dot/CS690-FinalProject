using Spectre.Console;
namespace MealPlan;
using System;
using System.Collections.Generic; 
using System.Linq;              
using FileSaver;
using Ingredient;
using RecipeListClass;
using ShoppingListClass; 


class Program
{
    static void Main(string[] args)
    {
        // Manage Data at start up
        const string pantryFile = "pantry-data.json";
        const string recipesFile = "recipes-data.json";
        const string shoppingListFile = "shopping-list-data.json";

        var pantryFileSaver = new FileSaver(pantryFile);
        var recipeListFileSaver = new FileSaver(recipesFile);
        var shoppingListFileSaver = new FileSaver(shoppingListFile);

        var pantry = new PantryClass.Pantry();
        var recipeList = new RecipeList();
        var shoppingList = new ShoppingList();

        Console.WriteLine("Loading data from files...");
        // LoadCollection returns a List<T>, assign it to the corresponding property
        pantry.Ingredients = pantryFileSaver.LoadCollection<Ingredient>();
        recipeList.Recipes = recipeListFileSaver.LoadCollection<RecipeClass.Recipe>();
        shoppingList.Ingredients = shoppingListFileSaver.LoadCollection<Ingredient>();

        Console.WriteLine($" -> Loaded {pantry.Ingredients.Count} pantry items.");
        Console.WriteLine($" -> Loaded {recipeList.Recipes.Count} recipes.");
        Console.WriteLine($" -> Loaded {shoppingList.Ingredients.Count} shopping list items.");
        Console.WriteLine("------------------------------------");

        string mode = "Main-Menu";

        while (mode != "Quit") {
            Console.WriteLine("Please Select Mode (shopping-list, pantry, recipes), or (quit) to exit program");
            mode = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please Select Mode")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to see modes)[/]")
                    .AddChoices(new[] {
                        "Shopping-List", "Pantry", "Recipes", "Quit",
            }));
    

            if (mode=="Shopping-List") {
                // TODO use ShoppingList class entrypoint
                
                while(mode != "Main-Menu") {
                    string action = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select action")
                            .PageSize(10)
                            .MoreChoicesText("[grey](Move up and down to see modes)[/]")
                            .AddChoices(new[] {
                                "Add-Ingredient", "Add-Recipe", "Show-List", 
                                "Remove-Ingredient", "Save-List", "Main-Menu",

                    }));
                    switch (action)
                    {
                        case "Add-Ingredient":
                            Ingredient ingredient = shoppingList.AddIngredientToShoppingList();
                            break;
                        case "Add-Recipe":
                            shoppingList.AddRecipeIngredientsToShoppingList(recipeList);
                            break;
                        case "Show-List":
                            shoppingList.ShowIngredientsInShoppingList();
                            break;
                        case "Remove-Ingredient":
                            shoppingList.RemoveIngredientsFromShoppingList();
                            break;
                        case "Save-List":
                            shoppingListFileSaver.SaveCollection(shoppingList.Ingredients);
                            break;
                        case "Main-Menu":
                            mode = "Main-Menu";
                            break;
                    }
                }
            } else if (mode=="Pantry") {
                // add, modify, remove, list, select mode
                while (mode != "Main-Menu") {
                    string action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select action")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to see modes)[/]")
                        .AddChoices(new[] {
                            "Add-Ingredient", "Show-Ingredients", 
                            "Remove-Ingredient", "Save-Pantry", "Main-Menu",

                    }));
                    switch (action)
                    {
                        case "Add-Ingredient":
                            pantry.AddIngredientToPantry();
                            break;
                        case "Show-Ingredients":
                            pantry.ShowIngredientsInPantry();
                            break;
                        case "Remove-Ingredient":
                            pantry.RemoveIngredientsFromPantry();
                            break;
                        case "Save-Pantry":
                            pantryFileSaver.SaveCollection(pantry.Ingredients);
                            break;
                        case "Main-Menu":
                            mode = "Main-Menu";
                            break;
                    }
                }
                
            } else if (mode=="Recipes") {
                // create, modify, remove, list, select mode
                while (mode != "Main-Menu") {
                    string action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select action")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to see modes)[/]")
                        .AddChoices(new[] {
                            "Create-Recipe", "Remove-Recipe", "List-Recipes",
                            "Update-Recipe", "Save-Recipes", "Main-Menu",

                    }));
                    switch (action)
                    {
                        case "Create-Recipe":
                            RecipeClass.Recipe recipe = new RecipeClass.Recipe(pantry);
                            recipeList.AddRecipeToRecipeList(recipe);
                            break;
                        case "Remove-Recipe":
                            recipeList.RemoveRecipeFromRecipeList();
                            break;
                        case "List-Recipes":
                            recipeList.ShowRecipesInRecipeList();
                            break;
                        case "Update-Recipe":
                            recipeList.UpdateRecipe(pantry);
                            break;
                        case "Save-Recipes":
                            recipeListFileSaver.SaveCollection(recipeList.Recipes);
                            break;
                        case "Main-Menu":
                            mode = "Main-Menu";
                            break;
                    }
                }
            }
        }
        Console.WriteLine("Thanks for visiting!");
    }    
}
