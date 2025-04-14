namespace MealPlan.Tests;
using MealPlan;
using System.IO;
using System.Reflection;
using System;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;

public class FileSaverTests : IDisposable
{

    // filename for save collection test
    public string saveTestFileName = "save-test-data.json";
    private readonly FileSaver _filesaver; 
    private readonly string _tempFilePath; 

    // Helper function to read embedded resources
    // This is how I got the test json files in the test
    private string ReadEmbeddedResourceText(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string defaultNamespace = assembly.GetName().Name;
        string fullResourceName = $"{defaultNamespace}.{resourceName.Replace('\\', '.').Replace('/', '.')}";

        using (Stream stream = assembly.GetManifestResourceStream(fullResourceName))
        {
            if (stream == null)
            {
                throw new FileNotFoundException($"Embedded resource '{fullResourceName}' not found. Check Build Action is 'Embedded resource' and the name (including namespace) is correct.");
            }
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }


    public FileSaverTests() {

        string resourceFileName = "filesaver-load-test.json";

        string jsonContent = ReadEmbeddedResourceText(resourceFileName);

        _tempFilePath = Path.GetTempFileName();

        File.WriteAllText(_tempFilePath, jsonContent);

        _filesaver = new FileSaver(_tempFilePath);
    }

    [Fact]
    public void Test_FileSaver_LoadCollection()
    {
        RecipeList test_Recipe_List = new RecipeList();
        test_Recipe_List.Recipes = _filesaver.LoadCollection<Recipe>();

        // Confirm that IEnumerable contains only 1 recipe
        Assert.Single(test_Recipe_List.Recipes);

        // Confirm the type of the object is Recipe
        Assert.IsType<Recipe>(test_Recipe_List.Recipes[0]);

        // Confirm the name of the recipe is as expected
        Assert.Equal("Chicken Parmesean", test_Recipe_List.Recipes[0].Name);
    }

    [Fact]
    public void Test_FileSaves_SaveCollection()
    {
        ShoppingList shoppingList = new ShoppingList();
        FileSaver testFileSaver = new FileSaver(saveTestFileName);

        // Ensure initialized shoppingList is empty
        Assert.Empty(shoppingList.Ingredients);

        // create test ingredient

        Ingredient test_ingredient = new Ingredient();
        test_ingredient.amount = 12;
        test_ingredient.measurementType = "cups";
        test_ingredient.name = "flour";

        shoppingList.Ingredients.Add(test_ingredient);

        // Ensure an ingredient was added to the in memory list
        Assert.Single(shoppingList.Ingredients);

        // Save collection to file
        testFileSaver.SaveCollection<Ingredient>(shoppingList.Ingredients);

        string jsonString = File.ReadAllText(saveTestFileName);

        List<Ingredient> loadedData = JsonSerializer.Deserialize<List<Ingredient>>(jsonString);

        // Check all the values after extracting form the collection
        Assert.Equal("flour", loadedData[0].name);

        Assert.Equal("cups", loadedData[0].measurementType);

        Assert.Equal(12, loadedData[0].amount);

    }

    // Dispose: Runs *after each* test method in this class finishes
    // This is needed to do cleanup between tests
    public void Dispose()
    {
        try
        {
            if (_tempFilePath != null && File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Warning: Failed to delete temporary test file '{_tempFilePath}'. Reason: {ex.Message}");
        }
        GC.SuppressFinalize(this);
    }
}