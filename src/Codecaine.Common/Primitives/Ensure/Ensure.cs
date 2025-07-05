﻿using Codecaine.Common.Domain.Extensions;

namespace Codecaine.Common.Primitives.Ensure
{
    /// <summary>
    /// The Ensure Pattern is a design pattern used to validate that a certain condition holds true, and if it doesn't, 
    /// it ensures that an exception is thrown or another action is taken to handle the violation of the condition. 
    /// Notes : <a href="https://chatgpt.com/share/6799750c-f478-8007-9f45-b93ba7dcd6df"></a>
    /// </summary>
    public static class Ensure
    {
        /// <summary>
        /// Ensures that the specified <see cref="string"/> value is not empty.        
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="message">The message to show if the check fails.</param>
        /// <param name="argumentName">The name of the argument being checked.</param>
        /// <exception cref="ArgumentException"> if the specified value is empty.</exception>
        public static void NotEmpty(string value, string message, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>
        /// Ensures that the specified <see cref="Guid"/> value is not empty.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="message">The message to show if the check fails.</param>
        /// <param name="argumentName">The name of the argument being checked.</param>
        /// <exception cref="ArgumentException"> if the specified value is empty.</exception>
        public static void NotEmpty(Guid value, string message, string argumentName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>
        /// Ensures that the specified <see cref="DateTime"/> value is not empty.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="message">The message to show if the check fails.</param>
        /// <param name="argumentName">The name of the argument being checked.</param>
        /// <exception cref="ArgumentException"> if the specified value is the default value for the type.</exception>
        public static void NotEmpty(DateTime value, string message, string argumentName)
        {
            if (value == default)
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>
        /// Ensures that the specified value is not null.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="message">The message to show if the check fails.</param>
        /// <param name="argumentName">The name of the argument being checked.</param>
        /// <exception cref="ArgumentNullException"> if the specified value is null.</exception>
        public static void NotNull<T>(T value, string message, string argumentName)
            where T : class
        {
            if (value is null)
            {
                throw new ArgumentNullException(argumentName, message);
            }
        }


        /// <summary>
        /// Ensures that the specified value is  null.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="message">The message to show if the check fails.</param>
        /// <param name="argumentName">The name of the argument being checked.</param>
        /// <exception cref="ArgumentNullException"> if the specified value is null.</exception>
        public static void Null<T>(T value, string message, string argumentName)
            where T : class
        {
            if (value is not null)
            {
                throw new ArgumentNullException(argumentName, message);
            }
        }
        public static void NotInvalidEnum<T>(T value, string message, string argumentName) where T : struct, Enum
        {

            var isValid = value.IsValidEnumValue();
            if (!isValid)
            {
                throw new ArgumentException(message, argumentName);

            }

        }
    }
}
