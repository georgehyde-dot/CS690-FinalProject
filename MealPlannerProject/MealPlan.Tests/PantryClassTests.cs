namespace MealPlan.Tests
{
    using Xunit;
    using MealPlan; // Assuming Pantry and Ingredient classes are in this namespace
    using System.Collections.Generic;
    using System.Linq; // Used for Assert.Empty, Assert.Single, Assert.Contains

    public class PantryTests
    {
        // Helper method to create sample ingredients easily for tests
        private Ingredient CreateIngredient(string name, int amount = 1, string measure = "unit", string category = "TestCategory")
        {
            return new Ingredient
            {
                name = name,
                amount = amount,
                measurementType = measure,
                foodCategory = category
            };
        }

        [Fact]
        public void AddIngredientsToPantryFromShoppingList_NullList_ReturnsZero()
        {
            var pantry = new Pantry();
            List<Ingredient> listToAdd = null;

            int countAdded = pantry.AddIngredientsToPantryFromShoppingList(listToAdd);

            Assert.Equal(0, countAdded); 
            Assert.Empty(pantry.Ingredients);
        }

        [Fact]
        public void AddIngredientsToPantryFromShoppingList_SingleItem_AddsItemAndReturnsOne()
        {
            var pantry = new Pantry();
            var ingredient = CreateIngredient("Milk", 1, "gallon", "Dairy");
            var listToAdd = new List<Ingredient> { ingredient };

            int countAdded = pantry.AddIngredientsToPantryFromShoppingList(listToAdd);

            Assert.Equal(1, countAdded);
            Assert.Single(pantry.Ingredients); 
            Assert.Contains(ingredient, pantry.Ingredients); 
        }

        [Fact]
        public void AddIngredientsToPantryFromShoppingList_ToExistingPantry_AppendsItems()
        {
            var pantry = new Pantry();
            var initialIngredient = CreateIngredient("Butter", 1, "stick", "Dairy");
            pantry.Ingredients.Add(initialIngredient); 

            var ingredientToAdd1 = CreateIngredient("Flour", 5, "lbs", "Baking");
            var ingredientToAdd2 = CreateIngredient("Sugar", 2, "lbs", "Baking");
            var listToAdd = new List<Ingredient> { ingredientToAdd1, ingredientToAdd2 };

            int countAdded = pantry.AddIngredientsToPantryFromShoppingList(listToAdd);

            Assert.Equal(2, countAdded); 
            Assert.Equal(3, pantry.Ingredients.Count); 
            Assert.Contains(initialIngredient, pantry.Ingredients); 
            Assert.Contains(ingredientToAdd1, pantry.Ingredients); 
            Assert.Contains(ingredientToAdd2, pantry.Ingredients); 
        }

    }
}