using System;
namespace AICalories.Models
{
    public static class RequestData
    {
        private static object firstPrompt;
        private static object secondPrompt;


        public static object GetFirstPrompt(string base64Image)
        {
            firstPrompt = new
            {
                response_format = new { type = "json_object" },
                tool_choice = "required",  //function calling
                max_tokens = 500,
                temperature = 0,
                model = "gpt-4o",
                messages = new object[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a gastronomic expert"
                    },

                    //new
                    //{
                    //    role = "user",
                    //    content = new object[]
                    //    {
                    //        new
                    //        {
                    //            type = "text",
                    //            text = "Analyze this dish, its engredients, size, weight, and calories "
                    //        },
                    //        //new
                    //        //{
                    //        //    type = "text",
                    //        //    text = "What is this dish and how much calories in total in has?"
                    //        //},
                    //        new
                    //        {
                    //            type = "image_url",
                    //            image_url = new
                    //            {
                    //                url = $"data:image/jpeg;base64,{base64Image}"
                    //            }
                    //        }
                    //    }
                    //},
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "text",
                                text = "What ingredients are there, what size, what weight, and how many calories it has?" +
                                "Calories must be calculated as precise as possible, considering every unit" +
                                "Output result in a JSON format"
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
                            description = "Analyze image",
                            parameters = new
                            {
                                type = "object",
                                required = new[] { "dish_name", "calories" },
                                properties = new
                                {
                                    dish_name = new
                                    {
                                        type = "string",
                                        description = "Name of the dish"
                                    },
                                    calories = new
                                    {
                                        type = "string",
                                        description = "Amount of calories"
                                    }
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

