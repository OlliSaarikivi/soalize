using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soalize
{
    /// <summary>
    /// Specifies grouping of members in a structure-of-arrays memory layout. Members are by default in a single
    /// unnamed group.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Optional column group to place this member in. If not set the member is placed in a group of its own.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="ColumnAttribute"/>.
        /// </summary>
        public ColumnAttribute()
        {
        }
    }
}
