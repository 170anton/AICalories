using System;

namespace AICalories.Models
{
    public static class RequestData
    {
        private static object firstPrompt;
        private static object secondPrompt;


        public static object GetFirstPrompt(string base64Image, string mealType, string userInfo)
        {
            firstPrompt = new
            {
                response_format = new { type = "json_object" },
                tool_choice = "required",  //function calling
                max_tokens = 3000,
                temperature = 0,
                model = "gpt-4o-mini",
                messages = new object[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a gastronomic expert"
                    },

                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "text",
                                text = "Is there any meal or food in this image? " +
                                "If no, set is_meal to false and set other properties to 0 or null. " +
                                "Follow nutritional information if it is present. " +
                                $"{mealType}. What ingredients are in this meal? " +
                                "Weight, calories, proteins, fats, carbohydrates, sugar of ingredients must be calculated as precise as possible in grams. " +
                                //"If you are not sure about weight of ingredient, take a lower-value. " +
                                "Then precisely summarize all calories and weight of all ingredients. " +
                                "Output result in a JSON format. " +
                                $"{userInfo}. "
                            },
                            new
                            {
                                type = "image_url",
                                image_url = new
                                {
                                    url = $"data:image/jpeg;base64,{base64Image}",
                                    detail = "high"
                                }
                            }
                        }
                    }
                },
                tools = new[]
                {
                    new
                    {
                        type = "function",
                        function = new
                        {
                            name = "gastronomic_expert",
                            //description = "Analyze this dish and its amount of calories",
                            parameters = new
                            {
                                type = "object",
                                required = new[] { "is_meal", "ingredients", "meal_name", "weight", "calories", "proteins", "fats", "carbohydrates", "sugar" },
                                properties = new
                                {
                                    is_meal = new
                                    {
                                        type = "boolean",
                                        description = "Is this a meal or a type of food"
                                    },
                                    has_nutrition_information = new
                                    {
                                        type = "boolean",
                                        description = "Is this a meal has nutrition information"
                                    },
                                    ingredients = new
                                    {
                                        type = "array",
                                        description = "List of ingredients and their calories",
                                        items = new
                                        {
                                            type = "object",
                                            required = new[] { "ingredient_name", "ingredient_weight", "ingredient_calories",
                                                "ingredient_proteins", "ingredient_fats", "ingredient_carbohydrates", "ingredient_sugar" },
                                            properties = new
                                            {
                                                ingredient_name = new
                                                {
                                                    type = "string",
                                                    description = "Name of the ingredient"
                                                },
                                                //ingredient_amount = new
                                                //{
                                                //    type = "string",
                                                //    description = "Amount of the ingredient"
                                                //},
                                                ingredient_weight = new
                                                {
                                                    type = "integer",
                                                    description = "Weight of the ingredient"
                                                },
                                                //ingredient_volume = new
                                                //{
                                                //    type = "string",
                                                //    description = "Volume of the ingredient"
                                                //},
                                                ingredient_calories = new
                                                {
                                                    type = "integer",
                                                    description = "Calories of the ingredient"
                                                },
                                                ingredient_proteins = new
                                                {
                                                    type = "integer",
                                                    description = "Protein of the ingredient"
                                                },
                                                ingredient_fats = new
                                                {
                                                    type = "integer",
                                                    description = "Fat of the ingredient"
                                                },
                                                ingredient_carbohydrates = new
                                                {
                                                    type = "integer",
                                                    description = "Carbohydrates of the ingredient"
                                                },
                                                ingredient_sugar = new
                                                {
                                                    type = "integer",
                                                    description = "Sugar of the ingredient"
                                                }
                                            },
                                        },
                                    },
                                    meal_name = new
                                    {
                                        type = "string",
                                        description = "Summarize and give the name of the meal"
                                    },
                                    weight = new
                                    {
                                        type = "integer",
                                        description = "Precisely summarize all ingredient_weight "
                                    },
                                    calories = new
                                    {
                                        type = "integer",
                                        description = "Precisely summarize all ingredient_calories"
                                    },
                                    proteins = new
                                    {
                                        type = "integer",
                                        description = "Precisely summarize all ingredient_proteins"
                                    },
                                    fats = new
                                    {
                                        type = "integer",
                                        description = "Precisely summarize all ingredient_fats"
                                    },
                                    carbohydrates = new
                                    {
                                        type = "integer",
                                        description = "Precisely summarize all ingredient_carbohydrates"
                                    },
                                    sugar = new
                                    {
                                        type = "integer",
                                        description = "Precisely summarize all ingredient_sugar"
                                    },
                                }
                            },
                        }
                    },

                }
            //functions = new
            //{
            //    name = "gastronomic_expert",
            //    description = "Name this dish",
            //    parameters = new
            //    {
            //        type = "object",
            //        required = new string[] { "dish_name" },
            //        properties = new
            //        {
            //            dish_name = new
            //            {
            //                type = "string",
            //                description = "Name of the dish"
            //            }
            //        }
            //    }
            //}
        };
            return firstPrompt;
        }

        public static object GetSecondPrompt(string imageUrl)
        {
            secondPrompt = new
            {
                max_tokens = 100,
                //temperature = 0.1,
                model = "gpt-4o",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "text",
                                text = "How much calories is this dish?"
                            },
                            new
                            {
                                type = "image_url",
                                image_url = new
                                {
                                    url = imageUrl
                                }
                            }
                        }
                    }
                }
            };
            return secondPrompt;
        }
    }
}

