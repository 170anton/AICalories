using System;
namespace AICalories.Models
{
	public static class InternetConnection
	{
        public static bool CheckInternetConnection()
        {
            var current = Connectivity.Current.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

