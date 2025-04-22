namespace MealPlan;
using Spectre.Console;


public class Pantry {
        public List<Ingredient> Ingredients { get; set; }

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
                AnsiConsole.WriteLine(ingredient.name + " " + ingredient.amount + " " + ingredient.measurementType + " " + ingredient.foodCategory);
            }
        }

        public void ShowIngredientsByCategory()
        {
            if (this.Ingredients == null || !this.Ingredients.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Pantry contains no ingredients[/]");
                return;
            }

            AnsiConsole.MarkupLine("[underline bold blue]Pantry Contents:[/]"); 

            var groupedIngredients = this.Ingredients
                .GroupBy(ingredient => string.IsNullOrWhiteSpace(ingredient.foodCategory) ? "Uncategorized" : ingredient.foodCategory)
                .OrderBy(group => group.Key); 

            foreach (var categoryGroup in groupedIngredients)
            {
                AnsiConsole.MarkupLine($"\n[bold green]{categoryGroup.Key}:[/]"); 

                foreach (Ingredient ingredient in categoryGroup.OrderBy(ing => ing.name))
                {
                    AnsiConsole.MarkupLine($"  -- [white]{ingredient.name}[/] ({ingredient.amount} {ingredient.measurementType})");
                }
            }
            Console.WriteLine(); 
        }

        public Ingredient AddIngredientToPantry() {
            Ingredient ingredient = Ingredient.CreateInteractiveIngredient();
            this.Ingredients.Add(ingredient);
            return ingredient; 
        }

        public int AddIngredientsToPantryFromShoppingList(List<Ingredient> ingredientsToAdd)
        {
            int countAdded = 0;
            if (ingredientsToAdd == null || ingredientsToAdd.Count == 0)
            {
                return 0; 
            }

            this.Ingredients ??= new List<Ingredient>(); 

            foreach (Ingredient ingredient in ingredientsToAdd)
            {
                this.Ingredients.Add(ingredient);
                countAdded++;
            }

            return countAdded;
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

        public void SavePantryCollection(Pantry pantry) {
            var pantryFileSaver = new FileSaver(FileSaver.pantryFile);
            pantryFileSaver.SaveCollection(pantry.Ingredients);
        }

        public List<Ingredient> LoadPantryCollection() {
            var pantryFileSaver = new FileSaver(FileSaver.pantryFile); 
            return pantryFileSaver.LoadCollection<Ingredient>();
        }
    }

