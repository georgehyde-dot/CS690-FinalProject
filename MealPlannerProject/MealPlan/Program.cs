using Spectre.Console;
namespace MealPlan;
using System;
using System.Collections.Generic; 
using System.IO;                
using System.Linq;              
using System.Text.Json;  


class Program
{

    public class Ingredient {
        public string name;
        public int amount;
        public string measurementType;

        public Ingredient(){
            Console.WriteLine("Enter Ingredient Name");
            string name = Console.ReadLine();
            Console.WriteLine("Enter Unit of Measure");
            string measurementType = Console.ReadLine();
            bool success = false;
            int finalNumAmount;
            while(true)
            {
                Console.WriteLine("Enter Ingredient Quantity");
                string amount = Console.ReadLine();
                success = int.TryParse(amount, out int numAmount);
                if (success)
                {
                    finalNumAmount = numAmount;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. That was not a valid whole number. Try Again");
                }
            }

            Console.WriteLine("Name " + name);
            Console.WriteLine("MT " + measurementType);

            this.name = name;
            this.measurementType = measurementType;
            this.amount = finalNumAmount;
        }
    }

    public class Pantry {
        public List<Ingredient> Ingredients;

        public Pantry() {
            List<Ingredient> ingredients = new List<Ingredient>();
            this.Ingredients = ingredients;
        }

        public void ShowIngredientsInPantry() {
            if (this.Ingredients.Count == 0) {
                Console.WriteLine("Pantry contains no ingredients");
                return;
            }
            foreach (Ingredient ingredient in Ingredients) {
                AnsiConsole.WriteLine(ingredient.name + " " + ingredient.amount + " " + ingredient.measurementType);
            }
        }

        public Ingredient AddIngredientToPantry() {
            Ingredient ingredient = new Ingredient();
            this.Ingredients.Add(ingredient);
            return ingredient; 
        }

        public void RemoveIngredientsFromPantry() {
            if (this.Ingredients.Count == 0) {
                Console.WriteLine("Pantry contains no ingredients");
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

        public List<Ingredient> SelectIngredientsFromPantry()
        {
            if (this.Ingredients == null || this.Ingredients.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Pantry contains no ingredients to select.[/]");
                return new List<Ingredient>(); 
            }
            var selectedNames = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Select [green]ingredients [/]?")
                    .NotRequired() 
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more ingredients)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle an ingredient, " +
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(this.Ingredients.Select(ingredient => ingredient.name))
            );
            if (selectedNames == null || selectedNames.Count == 0)
            {
                Console.WriteLine("No ingredients selected.");
                return new List<Ingredient>(); 
            }
            var selectedNamesSet = new HashSet<string>(selectedNames);

            List<Ingredient> resultList = this.Ingredients
                .Where(ingredient => selectedNamesSet.Contains(ingredient.name))
                .ToList();
            return resultList;
        }
    }

    public class Recipe {
        public string Name;
        public List<Ingredient> Ingredients;

        public Recipe(Pantry pantry) {
            Console.WriteLine("Enter Recipe Name");
            string name = Console.ReadLine();
            this.Name = name;

            List<Ingredient> listIngredients = new List<Ingredient>();
            
            if (pantry.Ingredients.Count != 0) {
                Console.WriteLine("Select Ingredients from pantry to add");
                foreach(Ingredient ingredient in pantry.SelectIngredientsFromPantry()) {
                    listIngredients.Add(ingredient);
                }
            }
            

            Console.WriteLine("Enter new ingredients to add to recipe");
            Console.WriteLine("Type (add) to add another ingredient, or (quit) to finish recipe");
            string state = Console.ReadLine();

            while(state!="quit") {
                Ingredient ingredient = new Ingredient();
                listIngredients.Add(ingredient);
                
                Console.WriteLine("Type (add) to add another ingredient, or (quit) to finish recipe");
                state = Console.ReadLine();
            }

            this.Ingredients = listIngredients;
        }
    }

    public class RecipeList {
        public List<Recipe> Recipes;

        public RecipeList() {
            List<Recipe> recipes = new List<Recipe>();
            this.Recipes = recipes;
        }

        public void AddRecipeToRecipeList(Recipe recipe) {
            this.Recipes.Add(recipe);
        }

        public void ShowRecipesInRecipeList() {
            if (this.Recipes == null || this.Recipes.Count == 0) {
                Console.WriteLine("RecipeList contains no recipes to show.");
                return;
            }
            var selectedRecipeName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a Recipe to view its ingredients")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more recipes)[/]")
                    .AddChoices(this.Recipes.Select(recipe => recipe.Name ?? "Unnamed Recipe"))
            );

            Recipe foundRecipe = this.Recipes.FirstOrDefault(recipe => recipe.Name == selectedRecipeName);

            if (foundRecipe != null)
            {
                AnsiConsole.MarkupLine($"\n[underline bold cyan]Ingredients for {foundRecipe.Name}:[/]");

                if (foundRecipe.Ingredients != null && foundRecipe.Ingredients.Count > 0)
                {
                    foreach (Ingredient ingredient in foundRecipe.Ingredients)
                    {
                        AnsiConsole.WriteLine($"- {ingredient.name}");
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]This recipe has no ingredients listed.[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error: Could not find recipe details for '{selectedRecipeName}'.[/]");
            }
        }

        public void RemoveRecipeFromRecipeList() {
            if (this.Recipes.Count == 0) {
                Console.WriteLine("RecipeList contains no Recipes");
                return;
            }
            var removeRecipes = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Select [green]recipes to remove[/]?")
                    .NotRequired() 
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more ingredients)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle a recipe, " + 
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(this.Recipes.Select(recipe => recipe.Name)));
                            

            if (removeRecipes != null && removeRecipes.Count > 0)
            {
                var namesToRemoveSet = new HashSet<string>(removeRecipes);

                int removedCount = this.Recipes.RemoveAll(recipe =>
                    namesToRemoveSet.Contains(recipe.Name)
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

        public void UpdateRecipe(Pantry pantry) {
            if (this.Recipes.Count == 0) {
                    Console.WriteLine("RecipeList contains no Recipes");
                    return;
                }
            var selectedRecipe = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select a Recipe")       
                        .PageSize(10)                  
                        .MoreChoicesText("[grey](Move up and down to reveal more recipes)[/]") 
                        .AddChoices(this.Recipes.Select(recipe => recipe.Name))
                ); 

            if (selectedRecipe != null)
            {

            this.Recipes.RemoveAll(recipe =>
                    selectedRecipe == recipe.Name
                );
            }

            Recipe recipe = new Recipe(pantry);

            this.AddRecipeToRecipeList(recipe);
        }

    }

    public class ShoppingList {
        public List<Ingredient> Ingredients;

        public ShoppingList() {
            List<Ingredient> ingredients = new List<Ingredient>();
            this.Ingredients = ingredients;
        }

        public Ingredient AddIngredientToShoppingList() {
            Ingredient ingredient = new Ingredient();
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
    }

    static void Main(string[] args)
    {
        // Manage Data at start up
        const string pantryFile = "pantry-data.json";
        const string recipesFile = "recipes-data.json";
        const string shoppingListFile = "shopping-list-data.json";

        var pantryFileSaver = new FileSaver(pantryFile);
        var recipeListFileSaver = new FileSaver(recipesFile);
        var shoppingListFileSaver = new FileSaver(shoppingListFile);

        var pantry = new Pantry();
        var recipeList = new RecipeList();
        var shoppingList = new ShoppingList();

        Console.WriteLine("Loading data from files...");
        // LoadCollection returns a List<T>, assign it to the corresponding property
        pantry.Ingredients = pantryFileSaver.LoadCollection<Ingredient>();
        recipeList.Recipes = recipeListFileSaver.LoadCollection<Recipe>();
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
                            Recipe recipe = new Recipe(pantry);
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

    public class FileSaver
    {
        private readonly string _filename;

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        public FileSaver(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("Filename cannot be null or whitespace.", nameof(filename));
            }
            this._filename = filename;

            try
            {
                string directory = Path.GetDirectoryName(this._filename);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(this._filename))
                {
                    File.WriteAllText(this._filename, "[]");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to initialize file '{this._filename}': {ex.Message}");
            }
        }

        public void SaveCollection<T>(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                collection = Enumerable.Empty<T>();
            }

            try
            {
                string jsonString = JsonSerializer.Serialize(collection, _options);
                File.WriteAllText(_filename, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to save data to '{_filename}': {ex.Message}");
            }
        }
        public List<T> LoadCollection<T>()
        {
            try
            {
                if (!File.Exists(_filename))
                {
                    Console.WriteLine($"[Warning] File not found: '{_filename}'. Returning empty list.");
                    return new List<T>();
                }

                string jsonString = File.ReadAllText(_filename);

                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    return new List<T>(); 
                }

                List<T> loadedData = JsonSerializer.Deserialize<List<T>>(jsonString, _options);
                return loadedData ?? new List<T>(); 
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"[Error] Invalid JSON format in '{_filename}': {jsonEx.Message}. Returning empty list.");
                return new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to load data from '{_filename}': {ex.Message}. Returning empty list.");
                return new List<T>();
            }
        }
    }

    
}
