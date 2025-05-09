﻿namespace MealPlan;
using Spectre.Console;

public class RecipeList {
        public List<Recipe> Recipes { get; set; }

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

        public void SaveRecipeListCollection(RecipeList recipeList) {
            var recipeListFileSaver = new FileSaver(FileSaver.recipesFile);
            recipeListFileSaver.SaveCollection(recipeList.Recipes);
        }

        public List<Recipe> LoadRecipeListCollection() {
           var recipeListFileSaver = new FileSaver(FileSaver.recipesFile);
           return recipeListFileSaver.LoadCollection<Recipe>(); 
        }
    }
