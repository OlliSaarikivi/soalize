using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Soalize
{
    /// <summary>
    /// An array with value semantics. Null values are not supported. Assignment into the array creates shallow copies.
    /// Expect this class to be slow, as semantics are implemented through reflection, and it is intended to be
    /// transpiled away into a flat memory representation.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "'Array' is the most appropriate suffix.")]
    public sealed class ValueArray<T> : IReadOnlyList<T> where T : class, new()
    {
        readonly T[] values;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification = "This static constructor is for extended generic type constraint checking.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
            Justification = "Ideally this would be a generic type constraint, but this is the next best thing.")]
        static ValueArray()
        {
            Contract.Requires<TypeConstraintException>(typeof(T).IsSealed, "Generic argument type must be sealed.");
        }

        /// <summary>
        /// Initializes an array of the specified length and calls the default constructor for each element.
        /// 
        /// </summary>
        public ValueArray(int length)
        {
            Contract.Requires(0 <= length, "length must be positive.");

            Utils.WarnConstructNonTranspiled<ValueArray<T>>();
            
            values = new T[length];
            for (int i = 0; i < length; ++i)
                values[i] = new T();
        }

        /// <summary>
        /// The setter copies the value to the position at the index. The getter returns a reference to the value at
        /// the index.
        /// </summary>
        public T this[int index]
        {
            get
            {
                return values[index];
            }
            set
            {
                Contract.Requires(0 <= index, "index must be positive.");
                Contract.Requires(null != value, "value may not be null.");

                values[index] = value.ShallowCloneNonNull();
            }
        }

        /// <summary>
        /// The length of the array.
        /// </summary>
        public int Count { get { return values.Length; } }

        /// <summary>
        /// The length of the array.
        /// </summary>
        public int Length { get { return values.Length; } }

        /// <summary>
        /// Returns an enumerator that iterates through the array.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)values).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the array.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }
}
