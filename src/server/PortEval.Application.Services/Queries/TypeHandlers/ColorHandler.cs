using Dapper;
using System;
using System.Data;
using System.Drawing;

namespace PortEval.Application.Services.Queries.TypeHandlers
{
    /// <summary>
    /// Handles conversion of <see cref="Color">Color</see> values to an SQL database.
    /// </summary>
    public class ColorHandler : SqlMapper.TypeHandler<Color>
    {
        public override void SetValue(IDbDataParameter parameter, Color value)
        {
            parameter.Value = value.ToArgb();
        }

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
