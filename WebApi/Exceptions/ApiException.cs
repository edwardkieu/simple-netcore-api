﻿using System;
using System.Globalization;

namespace WebApi.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException() : base("An error has occurred.")
        {
        }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, params object[] args) : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}