using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    // Domain/Common/Result.cs
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T Value { get; }
        public string Error { get; }
        public string ErrorCode { get; }

        protected Result(T value, bool isSuccess, string error, string errorCode)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
            ErrorCode = errorCode;
        }

        public static Result<T> Success(T value) => new(value, true, null, null);
        public static Result<T> Failure(string error, string errorCode = null) => new(default, false, error, errorCode);
    }
}
