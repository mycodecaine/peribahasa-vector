﻿using Codecaine.Common.Primitives.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Authentication.Errors
{
    public static class AuthenticationError
    {
        /// <summary>
        /// Error indicating invalid username or password.
        /// </summary>
        public static Error InvalidUserNameOrPassword => new Error(
            "Authentication.InvalidUserNameOrPassword",
            "The specified username or password are incorrect.");

        /// <summary>
        /// Error indicating invalid reset password attempt.
        /// </summary>
        public static Error InvalidResetPassword => new Error(
            "Authentication.InvalidResetPassword",
            "The specified username or password are incorrect.");

        /// <summary>
        /// Error indicating invalid role assignment attempt.
        /// </summary>
        public static Error InvalidAssignRole => new Error(
            "Authentication.InvalidAssignRole",
            "The specified username or rolename are incorrect.");

        /// <summary>
        /// Error indicating that the passwords do not match.
        /// </summary>
        public static Error PasswordNotIdentical => new Error(
            "Authentication.PasswordNotIdentical",
            "Password is incorrect");

        /// <summary>
        /// Error indicating invalid admin token.
        /// </summary>
        public static Error InvalidAdminToken => new Error(
            "Authentication.InvalidAdminToken",
            "The specified token is incorrect.");

        /// <summary>
        /// Error indicating unsuccessful role assignment.
        /// </summary>
        public static Error AssignRoleNotSuccessfull => new Error(
                "Authentication.AssignRoleNotSuccessfull",
                "Assigning Role is not successfull.");

        /// <summary>
        /// Error indicating unsuccessful password reset.
        /// </summary>
        public static Error ResetPasswordError => new Error(
               "Authentication.ResetPasswordError",
               "Reset Password is not successfull.");

        /// <summary>
        /// Error indicating that the token SubId does not exist.
        /// </summary>
        public static Error SubIdNotExist => new Error(
            "Authentication.SubIdNotExist",
            "Token SubId is not valid");

        /// <summary>
        /// Error indicating that the username already exists.
        /// </summary>
        public static Error UserNameAlreadyExist => new Error(
            "Authentication.UserNameAlreadyExist",
            "The specified username already exist.");

        /// <summary>
        /// Error indicating that the username not exists.
        /// </summary>
        public static Error UserNameNotExist => new Error(
            "Authentication.UserNameNotExist",
            "The specified username is not exist.");

        /// <summary>
        /// Error indicating that the user is not authenticated.
        /// </summary>
        public static Error UnauthorizedUser => new Error(
            "Authentication.UnauthorizedUser",
            "User is not Authenticate.");
    }
}
