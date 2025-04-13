namespace MealPlan;
    public class Ingredient {
        public string name { get; set; }
        public int amount { get; set; }
        public string measurementType { get; set; }

        public Ingredient() {
            name = string.Empty;
            measurementType = string.Empty;
            amount = 0;
        }

        public static Ingredient CreateInteractiveIngredient()
        {
            Console.WriteLine("Enter Ingredient Name");
            string name = Console.ReadLine();

            Console.WriteLine("Enter Unit of Measure");
            string measurementType = Console.ReadLine();

            int finalNumAmount;
            while (true)
            {
                Console.WriteLine("Enter Ingredient Quantity");
                string amountStr = Console.ReadLine();
                if (int.TryParse(amountStr, out int numAmount) && numAmount >= 0)
                {
                    finalNumAmount = numAmount;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid non-negative whole number. Try Again");
                }
            }
            Ingredient newIngredient = new Ingredient {
                name = name,
                measurementType = measurementType,
                amount = finalNumAmount
            };
            return newIngredient;
        }
    }
