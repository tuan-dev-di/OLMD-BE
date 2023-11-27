using System;
namespace OptimizingLastMile.Models.Commons
{
    public class GenericResult
    {
        public ErrorObject Error { get; private set; }
        public bool IsSuccess { get; private set; }
        public bool IsFail { get; private set; }

        protected GenericResult(ErrorObject error)
        {
            Error = error;
            IsSuccess = error is null;
            IsFail = error is not null;
        }

        public static GenericResult Ok()
        {
            return new(null);
        }

        public static GenericResult Fail(ErrorObject error)
        {
            return new(error);
        }
    }

    public class GenericResult<T> : GenericResult where T : class
    {
        public T Data { get; private set; }

        private GenericResult(T data, ErrorObject error) : base(error)
        {
            Data = data;
        }

        public static GenericResult<T> Ok(T data)
        {
            return new(data, null);
        }

        public static GenericResult<T> Fail(ErrorObject error)
        {
            return new(null, error);
        }
    }
}