namespace MealPlan.Tests
{
    using Xunit;
    using MealPlan;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RecipeTests
    {
        private Ingredient CreateSampleIngredient(string name, int amount = 1, string measure = "unit", string category = "Test")
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
        public void Recipe_ParameterlessConstructor_ShouldInitializeProperties()
        {
            var recipe = new Recipe();

            Assert.Equal(string.Empty, recipe.Name);
            Assert.NotNull(recipe.Ingredients);
            Assert.Empty(recipe.Ingredients);  
            Assert.Equal(string.Empty, recipe.Notes);
        }

        [Fact]
        public void Recipe_Properties_ShouldSetAndGetCorrectly()
        {
            var recipe = new Recipe(); 

            string expectedName = "Spaghetti Bolognese";
            string expectedNotes = "Simmer sauce for at least 1 hour.";
            var ingredient1 = CreateSampleIngredient("Ground Beef", 1, "lb", "Meat");
            var ingredient2 = CreateSampleIngredient("Tomato Sauce", 24, "oz", "Canned");
            var expectedIngredients = new List<Ingredient> { ingredient1, ingredient2 };

            recipe.Name = expectedName;
            recipe.Notes = expectedNotes;
            recipe.Ingredients = expectedIngredients;

            Assert.Equal(expectedName, recipe.Name);
            Assert.Equal(expectedNotes, recipe.Notes);
            Assert.NotNull(recipe.Ingredients);
            Assert.Equal(2, recipe.Ingredients.Count);
            Assert.Same(expectedIngredients, recipe.Ingredients); 
            Assert.Contains(ingredient1, recipe.Ingredients);
            Assert.Contains(ingredient2, recipe.Ingredients);
        }

        [Fact]
        public void Recipe_IngredientsProperty_CanBeModified()
        {
            var recipe = new Recipe(); 
            var ingredient1 = CreateSampleIngredient("Pasta", 1, "box", "Pantry");
            var ingredient2 = CreateSampleIngredient("Olive Oil", 2, "tbsp", "Pantry");

             recipe.Ingredients.Add(ingredient1); 
             recipe.Ingredients.Add(ingredient2);

             Assert.Equal(2, recipe.Ingredients.Count);
             Assert.Contains(ingredient1, recipe.Ingredients);
             Assert.Contains(ingredient2, recipe.Ingredients);
        }
    }
}