namespace RecipeClass;
using Ingredient;
using PantryClass;

public class Recipe {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }

        public Recipe()
        {
            // Initialize lists/strings if necessary, or leave empty if setters handle nulls
            Name = string.Empty; 
            Ingredients = new List<Ingredient>(); 
        }

        public Recipe(PantryClass.Pantry pantry) {
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
                Ingredient ingredient = Ingredient.CreateInteractiveIngredient();
                listIngredients.Add(ingredient);
                
                Console.WriteLine("Type (add) to add another ingredient, or (quit) to finish recipe");
                state = Console.ReadLine();
            }

            this.Ingredients = listIngredients;
        }
    }
