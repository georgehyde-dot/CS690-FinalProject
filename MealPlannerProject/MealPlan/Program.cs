using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using Spectre.Console;
namespace MealPlan;


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

        // Initialize Storage for a session
        FileSaver recipeFileSaver = new FileSaver("recipe-data.txt");
        FileSaver ingredientFileSaver = new FileSaver("pantry-data.txt");
        FileSaver shoppingListFileSaver = new FileSaver("shopping-list-data.txt");

        // Initialize in memory storage
        ShoppingList shoppingList = new ShoppingList();
        Pantry pantry = new Pantry();
        RecipeList recipeList = new RecipeList();
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
                                "Remove-Ingredient", "Main-Menu",

                    }));
                    switch (action)
                    {
                        case "Add-Ingredient":
                            Ingredient ingredient = shoppingList.AddIngredientToShoppingList();
                            shoppingListFileSaver.AppendLine(ingredient.name + " " + ingredient.amount + " " + ingredient.measurementType);
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
                        case "Main-Menu":
                            mode = "Main-Menu";
                            break;
                    }
                }
            } else if (mode=="Pantry") {
                // TODO Ingredients entrypoint
                // add, modify, remove, list, select mode
                while (mode != "Main-Menu") {
                    string action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select action")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to see modes)[/]")
                        .AddChoices(new[] {
                            "Add-Ingredient", "Show-Ingredients", 
                            "Remove-Ingredient", "Main-Menu",

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
                        case "Main-Menu":
                            mode = "Main-Menu";
                            break;
                    }
                }
                
            } else if (mode=="Recipes") {
                // TODO Recipes entrypoint
                // create, modify, remove, list, select mode
                while (mode != "Main-Menu") {
                    string action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select action")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to see modes)[/]")
                        .AddChoices(new[] {
                            "Create-Recipe", "Remove-Recipe", "List-Recipes",
                            "Update-Recipe", "Main-Menu",

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
                        case "Main-Menu":
                            mode = "Main-Menu";
                            break;
                    }
                }
            }
        }
        Console.WriteLine("Thanks for visiting!");
    } 

    public class FileSaver {
        string filename;

        public FileSaver(string filename) {
            this.filename = filename;
            File.Create(this.filename).Close();
        }

        public void AppendLine(string line) {            
            File.AppendAllText(this.filename, line + Environment.NewLine);
        }


    }

    
}
