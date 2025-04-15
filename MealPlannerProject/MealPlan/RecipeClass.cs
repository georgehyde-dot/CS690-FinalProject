namespace MealPlan;


public class Recipe {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }

        public string Notes { get; set; }

        public Recipe()
        {
            Name = string.Empty; 
            Ingredients = new List<Ingredient>(); 
            Notes = string.Empty;
        }

        public Recipe(Pantry pantry) {
            Console.WriteLine("Enter Recipe Name");
            string name = Console.ReadLine();
            this.Name = name;

            Console.WriteLine("Add Notes for Recipe");
            string notes = Console.ReadLine();
            this.Notes = notes;

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
                Ingredient ingredient = Ingredient.CreateInteractiveIngredient();
                listIngredients.Add(ingredient);
                
                Console.WriteLine("Type (add) to add another ingredient, or (quit) to finish recipe");
                state = Console.ReadLine();
            }

            this.Ingredients = listIngredients;
        }
    }
