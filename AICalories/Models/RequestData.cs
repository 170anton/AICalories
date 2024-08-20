﻿using System;
namespace AICalories.Models
{
    public static class RequestData
    {
        private static object firstPrompt;
        private static object secondPrompt;


        public static object GetFirstPrompt(string base64Image, string dishType)
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
                                text = $"What ingredients are in this meal? There are {dishType} ingredients. " +
                                "Weight and calories of ingredients must be calculated as precise as possible. " +
                                //"If you are not sure about weight of ingredient, take a lower-value. " +
                                "Then summarize all calories of all ingredients as precise as possible. " +
                                "Output result in a JSON format. "
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
                                required = new[] { "ingredients", "meal_name", "calories",  },
                                properties = new
                                {
                                    ingredients = new
                                    {
                                        type = "array",
                                        description = "List of ingredients and their calories",
                                        items = new
                                        {
                                            type = "object",
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
                                                }
                                            },
                                            required = new[] { "ingredient_name", "ingredient_weight", "ingredient_calories" }
                                        },
                                    },
                                    meal_name = new
                                    {
                                        type = "string",
                                        description = "Summarize and give the name of the meal"
                                    },
                                    calories = new
                                    {
                                        type = "integer",
                                        description = "Summarize all ingredient_calories which you calculated"
                                    },
                                }
                            },
                        }
                    }
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

