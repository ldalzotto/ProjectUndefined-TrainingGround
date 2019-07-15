﻿using System;

namespace CoreGame
{
    public static class FunctionalExtensions
    {
        public static void IfNotNull<T>(this T input, Action<T> action)
        {
            if (input != null)
            {
                action.Invoke(input);
            }
        }

        public static void IfTypeEqual<COMPARISON_TYPE>(this object input, Action<COMPARISON_TYPE> action)
        {
            if (input != null && input.GetType() == typeof(COMPARISON_TYPE))
            {
                action.Invoke((COMPARISON_TYPE)input);
            }
        }
    }
}