namespace MealPlan.Tests
{
    using Xunit;
    using MealPlan; 
    using System.Collections.Generic;
    using System.Linq;

    public class RecipeListTests
    {
        private Recipe CreateRecipe(string name)
        {
            return new Recipe
            {
                Name = name,
            };
        }

        [Fact]
        public void RecipeList_Constructor_ShouldInitializeEmptyList()
        {
            var recipeList = new RecipeList();

            Assert.NotNull(recipeList.Recipes); 
            Assert.Empty(recipeList.Recipes);   
        }

        [Fact]
        public void AddRecipeToRecipeList_AddsRecipeToEmptyList()
        {
            var recipeList = new RecipeList();
            var recipe = CreateRecipe("Pasta Bake");

            recipeList.AddRecipeToRecipeList(recipe);

            Assert.Single(recipeList.Recipes); 
            Assert.Contains(recipe, recipeList.Recipes); 
            Assert.Same(recipe, recipeList.Recipes[0]); 
        }

        [Fact]
        public void AddRecipeToRecipeList_AddsRecipeToExistingList()
        {
            var recipeList = new RecipeList();
            var initialRecipe = CreateRecipe("Omelette");
            recipeList.Recipes.Add(initialRecipe); 

            var recipeToAdd = CreateRecipe("Salad");

            recipeList.AddRecipeToRecipeList(recipeToAdd);

            Assert.Equal(2, recipeList.Recipes.Count); 
            Assert.Contains(initialRecipe, recipeList.Recipes);
            Assert.Contains(recipeToAdd, recipeList.Recipes);
            Assert.Same(recipeToAdd, recipeList.Recipes[1]); 
        }
    }
}