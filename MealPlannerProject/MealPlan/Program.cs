using System.Linq;
using Spectre.Console;
namespace MealPlan;


class Program
{

    public struct Ingredient {
        public string name;
        public int amount;
        public string measmentType;
    }

    public struct Pantry {
        List<Ingredient> Ingredients;
    }

    public struct Recipe {
        public string Name;
        public List<Ingredient> Ingredients;

    }

    public struct RecipeList {
        public List<Recipe> Recipes { get; set; }

        public void AddRecipeToRecipeList(Recipe recipe) {
            this.Recipes.Append(recipe);
        }
    }

    public struct ShoppingList {
        public List<Ingredient> Ingredients;

        public void AddIngredientToShoppingList(Ingredient ingredient) {
            this.Ingredients.Append(ingredient);
        }

        public void AddRecipeIngredientsToShoppingList(Recipe recipe) {
            foreach (Ingredient ingredient in recipe.Ingredients) {
                Ingredients.Append(ingredient);
            }
        }

        public void ShowIngredientsInShoppingList() {
            foreach (Ingredient ingredient in Ingredients) {
                AnsiConsole.WriteLine(ingredient.name + " " + ingredient.amount + " " + ingredient.measmentType);
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
        // Pantry pantry = new Pantry();
        RecipeList recipeList = new RecipeList();
        string mode = "start";

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
                            "Remove-Ingredient", "Main Menu",

                 }));
                switch (action)
                {
                    case "Add-Ingrdient":
                        Console.WriteLine("Enter Ingredient Name");
                        string name = Console.ReadLine();
                        Console.WriteLine("Enter Unit of Measure");
                        string measmentType = Console.ReadLine();
                        Console.WriteLine("Enter Ingredient Quantity");
                        string quantity = Console.ReadLine();
                        shoppingListFileSaver.AppendLine(name + " " + quantity + measmentType);
                        break;
                    case "Add-Recipe":
                         var selectedRecipeName = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Select a Recipe")       
                                .PageSize(10)                  
                                .MoreChoicesText("[grey](Move up and down to reveal more recipes)[/]") 
                                .AddChoices(recipeList.Recipes.Select(recipe => recipe.Name))
                        );
                        break;
                    case "Show-List":
                        shoppingList.ShowIngredientsInShoppingList();
                        break;
                    case "Remove-Ingredient":
                        var removeIngredients = AnsiConsole.Prompt(
                            new MultiSelectionPrompt<Ingredient>()
                                .Title("Select [green]ingredients to remove[/]?")
                                .NotRequired() 
                                .PageSize(10)
                                .MoreChoicesText("[grey](Move up and down to reveal more ingredients)[/]")
                                .InstructionsText(
                                    "[grey](Press [blue]<space>[/] to toggle an ingredient, " + 
                                    "[green]<enter>[/] to accept)[/]")
                                .AddChoices(shoppingList.Ingredients.Select(ingredient => ingredient)));
                        

                        foreach(Ingredient ingredient in removeIngredients) {
                            removeIngredients.Remove(ingredient);
                        }
                        break;
              }


            } else if (mode=="Pantry") {
                // TODO Ingredients entrypoint
                // add, modify, remove, list, select mode
            } else if (mode=="Recipes") {
                // TODO Recipes entrypoint
                // create, modify, remove, list, select mode
            } else {
                Console.WriteLine("Mode must be from list (shopping-list, pantry, recipes), or (quit) to exit program");
                continue;
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
