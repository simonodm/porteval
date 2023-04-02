using System.Data;

namespace PortEval.Application.Core.Interfaces.Queries
{
    /// <summary>
    /// A factory for creating <see cref="IDbConnection"/> to the application's database.
    /// </summary>
    public interface IDbConnectionCreator
    {
        /// <summary>
        /// Creates an <see cref="IDbConnection"/> instance to the application's database.
        /// </summary>
        /// <returns>An <see cref="IDbConnection"/> to the application's database.</returns>
        public IDbConnection CreateConnection();
    }
}
