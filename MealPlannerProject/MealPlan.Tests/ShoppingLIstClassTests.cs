namespace MealPlan.Tests
{
    using Xunit;
    using MealPlan; 
    using System.Collections.Generic;
    using System.Linq; 

    public class ShoppingListTests
    {
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
        public void ShoppingList_Constructor_ShouldInitializeEmptyIngredientsList()
        {
            var shoppingList = new ShoppingList();

            Assert.NotNull(shoppingList.Ingredients); 
            Assert.Empty(shoppingList.Ingredients);   
        }

        [Fact]
        public void ShoppingList_IngredientsList_AllowsAddingItems()
        {
            var shoppingList = new ShoppingList();
            var milk = CreateIngredient("Milk", 1, "gallon", "Dairy");
            var eggs = CreateIngredient("Eggs", 12, "count", "Protein");

            shoppingList.Ingredients.Add(milk);
            shoppingList.Ingredients.Add(eggs);

            Assert.Equal(2, shoppingList.Ingredients.Count);
            Assert.Contains(milk, shoppingList.Ingredients);
            Assert.Contains(eggs, shoppingList.Ingredients);
        }

        [Fact]
        public void ShoppingList_IngredientsList_AllowsClearingItems()
        {
            var shoppingList = new ShoppingList();
            var bread = CreateIngredient("Bread", 1, "loaf", "Bakery");
            shoppingList.Ingredients.Add(bread); 

            shoppingList.Ingredients.Clear();

            Assert.NotNull(shoppingList.Ingredients);
            Assert.Empty(shoppingList.Ingredients); 
        }
    }
}