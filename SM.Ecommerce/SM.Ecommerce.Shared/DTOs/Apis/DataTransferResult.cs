using System;

namespace SM.Ecommerce.Shared
{
    public class DataTransferResult <T>
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public T Result { get; set; }
    }
}
