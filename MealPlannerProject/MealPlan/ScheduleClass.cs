namespace MealPlan;
using Spectre.Console;

public class Schedule {

    public List<Recipe> Recipes { get; set; }


    public Schedule()
    {
        Recipes = new List<Recipe>();
        this.Recipes = Recipes;
    }

    public Schedule(List<Recipe> recipes) {
        this.Recipes = recipes;
    }

    public void AddRecipeToSchedule(RecipeList recipeList)
        {
            if (recipeList == null || recipeList.Recipes == null || recipeList.Recipes.Count == 0)
            {
                Console.WriteLine("There are no recipes available to select from.");
                return;
            }

            var selectedRecipeName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a Recipe to add it to your schedule")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more recipes)[/]")
                    .AddChoices(recipeList.Recipes.Select(recipe => recipe.Name ?? "Unnamed Recipe"))
            );

            Recipe foundRecipe = recipeList.Recipes.FirstOrDefault(recipe => recipe.Name == selectedRecipeName);

            if (foundRecipe != null) 
            {
                this.Recipes.Add(foundRecipe);
                Console.WriteLine($"Added {foundRecipe.Name} to Schedule");
            }
        }

    public void ShowRecipesInSchedule() {
            if (this.Recipes == null || this.Recipes.Count == 0) {
                Console.WriteLine("Schedule contains no recipes to show.");
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
                    AnsiConsole.WriteLine($"NOTES- {foundRecipe.Notes}");
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

    public void RemoveRecipeFromSchedule() {
            if (this.Recipes.Count == 0) {
                Console.WriteLine("Schedule contains no Recipes");
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
                    Console.WriteLine($"Removed {removedCount} recipe(s).");
                }
                else
                {
                    Console.WriteLine("Selected recipe(s) could not be found or removed.");
                }
            }
            else
            {
                Console.WriteLine("No recipes selected for removal.");
            }
        }

    public void SaveScheduleCollection(Schedule schedule){
        var scheduleFileSaver = new FileSaver(FileSaver.scheduleFileSaver);
        scheduleFileSaver.SaveCollection(schedule.Recipes);
    }

    public List<Recipe> LoadScheduleCollection() {
        var scheduleFileSaver = new FileSaver(FileSaver.scheduleFileSaver);
        return scheduleFileSaver.LoadCollection<Recipe>();    
    }
}