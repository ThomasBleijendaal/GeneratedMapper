﻿namespace Example
{
    //}

    //namespace Random
    //{
    public static class StringExtensions
    {
        public static string ConvertToGreeting(this string someString)
        {
            return $"Hi {someString};";
        }
    }
}