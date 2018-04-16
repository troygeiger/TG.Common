using System;
using System.Collections.Generic;
using System.Text;

namespace TG.Common
{
    /// <summary>
    /// Determines how the values of a <see cref="SearchFormBase"/> are applied to the value text box of <see cref="InputBox"/>.
    /// </summary>
    public enum ValueOptions
    {
        /// <summary>
        /// Completely replace the value of the <see cref="InputBox"/> text box with the value from <see cref="SearchFormBase"/>.
        /// </summary>
        Replace,
        /// <summary>
        /// Directly appends the value to the text box.
        /// </summary>
        Append,
        /// <summary>
        /// Keeps any existing text and separates the value from <see cref="SearchFormBase"/> with a semicolon.
        /// </summary>
        AppendSemiColonSeparated
    }
}
