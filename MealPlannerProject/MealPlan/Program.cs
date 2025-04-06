using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using Spectre.Console;
namespace MealPlan;


class Program
{

    public struct Ingredient {
        public string name;
        public int amount;
        public string measmentType;

        public Ingredient(){
            Console.WriteLine("Enter Ingredient Name");
            string name = Console.ReadLine();
            Console.WriteLine("Enter Unit of Measure");
            string measmentType = Console.ReadLine();
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

            this.name = name;
            this.measmentType = measmentType;
            this.amount = finalNumAmount;
        }
    }

    public struct Pantry {
        List<Ingredient> Ingredients;

        public void ShowIngredientsInPantry() {
            if (this.Ingredients.Count == 0) {
                Console.WriteLine("Pantry contains no ingredients");
                return;
            }
            foreach (Ingredient ingredient in Ingredients) {
                AnsiConsole.WriteLine(ingredient.name + " " + ingredient.amount + " " + ingredient.measmentType);
            }
        }

        public Ingredient AddIngredientToPantry() {
            Ingredient ingredient = new Ingredient();
            this.Ingredients.Append(ingredient);
            return ingredient; 
        }

        public void RemoveIngredientsFromPantry() {
            var removeIngredients = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Ingredient>()
                    .Title("Select [green]ingredients to remove[/]?")
                    .NotRequired() 
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more ingredients)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle an ingredient, " + 
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(this.Ingredients.Select(ingredient => ingredient)));
                            

            foreach(Ingredient ingredient in removeIngredients) {
                this.Ingredients.Remove(ingredient);
            }
        }
    }

    public struct Recipe {
        public string Name;
        public List<Ingredient> Ingredients;

        public Recipe() {

        }
    }

    public struct RecipeList {
        // "Create-Recipe", "Remove-Recipe", "List-Recipes",
                            // "Update-Recipe",
        public List<Recipe> Recipes { get; set; }

        public void AddRecipeToRecipeList(Recipe recipe) {
            this.Recipes.Append(recipe);
        }

    }

    public struct ShoppingList {
        public List<Ingredient> Ingredients;

        public Ingredient AddIngredientToShoppingList() {
            Ingredient ingredient = new Ingredient();
            this.Ingredients.Append(ingredient);
            return ingredient;
        }

        public void AddRecipeIngredientsToShoppingList(Recipe recipe) {
            if (recipe.Ingredients.Count == 0) {
                Console.WriteLine("Recipe " + recipe.Name + "contains no ingredients");
                return;
            }
            foreach (Ingredient ingredient in recipe.Ingredients) {
                Ingredients.Append(ingredient);
            }
        }

        public void ShowIngredientsInShoppingList() {
            if (this.Ingredients.Count == 0) {
                Console.WriteLine("ShoppingList contains no ingredients");
                return;
            }
            foreach (Ingredient ingredient in Ingredients) {
                AnsiConsole.WriteLine(ingredient.name + " " + ingredient.amount + " " + ingredient.measmentType);
            }
        }

        public void RemoveIngredientsFromShoppingList() {
            if (this.Ingredients.Count == 0) {
                Console.WriteLine("ShoppingList contains no ingredients");
                return;
            }
            var removeIngredients = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Ingredient>()
                    .Title("Select [green]ingredients to remove[/]?")
                    .NotRequired() 
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more ingredients)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle an ingredient, " + 
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(this.Ingredients.Select(ingredient => ingredient)));
                            

            foreach(Ingredient ingredient in removeIngredients) {
                this.Ingredients.Remove(ingredient);
            }
        }

        public void AddRecipeIngredientsToShoppingList(RecipeList recipeList) {
            if (recipeList.Recipes.Count == 0) {
                Console.WriteLine("RecipeList contains no Recipes");
                return;
            }
            var selectedRecipeName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a Recipe")       
                    .PageSize(10)                  
                    .MoreChoicesText("[grey](Move up and down to reveal more recipes)[/]") 
                    .AddChoices(recipeList.Recipes.Select(recipe => recipe.Name))
            );
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
                string action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select action")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to see modes)[/]")
                        .AddChoices(new[] {
                            "Add-Ingredient", "Add-Recipe", "Show-List", 
                            "Remove-Ingredient", "Main-Menu",

                 }));
                while(mode != "Main-Menu") {
                    switch (action)
                    {
                        case "Add-Ingredient":
                            Ingredient ingredient = shoppingList.AddIngredientToShoppingList();
                            shoppingListFileSaver.AppendLine(ingredient.name + " " + ingredient.amount + " " + ingredient.measmentType);
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
                        case "Remove-Ingredients":
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
                            Recipe recipe = new Recipe();
                            recipeList.AddRecipeToRecipeList(recipe);
                            break;
                        case "Remove-Recipe":
                            break;
                        case "List-Recipes":
                            break;
                        case "Update-Recipe":
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
