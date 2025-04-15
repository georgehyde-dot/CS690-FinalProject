namespace MealPlan.Tests
{
    using Xunit;
    using MealPlan;
    using System; 

    public class IngredientTests
    {
        [Fact]
        public void Ingredient_Constructor_ShouldInitializePropertiesToDefaults()
        {
            var ingredient = new Ingredient();

            Assert.Equal(string.Empty, ingredient.name);
            Assert.Equal(0, ingredient.amount);
            Assert.Equal(string.Empty, ingredient.measurementType);
            Assert.Equal(string.Empty, ingredient.foodCategory);
        }

        [Fact]
        public void Ingredient_Properties_ShouldSetAndGetCorrectly()
        {
            var ingredient = new Ingredient();
            string expectedName = "Chicken Breast";
            int expectedAmount = 2;
            string expectedMeasurement = "pieces";
            string expectedCategory = "Poultry";

            ingredient.name = expectedName;
            ingredient.amount = expectedAmount;
            ingredient.measurementType = expectedMeasurement;
            ingredient.foodCategory = expectedCategory;

            Assert.Equal(expectedName, ingredient.name);
            Assert.Equal(expectedAmount, ingredient.amount);
            Assert.Equal(expectedMeasurement, ingredient.measurementType);
            Assert.Equal(expectedCategory, ingredient.foodCategory);
        }

    }
}