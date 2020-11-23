using System;
using System.ComponentModel.DataAnnotations;

namespace CG.DataAnnotations.QuickStart
{
    class TestClass : ValidatableObject
    {
        [Required]
        public string A { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var obj = new TestClass();
            obj.ThrowIfInvalid(); // <-- throws due to empty A property.
        }
    }
}
