using System;
using System.Data;
using System.Drawing;
using Dapper;

namespace PortEval.Application.Core.Queries.TypeHandlers
{
    /// <summary>
    /// Handles conversion of <see cref="Color">Color</see> values to an SQL database.
    /// </summary>
    public class ColorHandler : SqlMapper.TypeHandler<Color>
    {
        /// <inheritdoc />
        public override void SetValue(IDbDataParameter parameter, Color value)
        {
            parameter.Value = value.ToArgb();
        }

        /// <inheritdoc />
        public override Color Parse(object value)
        {
            if (value is int colorArgb)
            {
                return Color.FromArgb(colorArgb);
            }

            throw new ArgumentException($"Cannot map value of type {value.GetType()} to {typeof(Color)}");
        }
    }
}
