using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Persistence
{
    public sealed class ConnectionString
    {
        /// <summary>
        /// The connection strings key.
        /// </summary>
        public const string SettingsKey = "DbConnectionString";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionString"/> class.
        /// </summary>
        /// <param name="value">The connection string value.</param>
        public ConnectionString(string value) => Value = value;

        /// <summary>
        /// Gets the connection string value.
        /// </summary>
        public string Value { get; }

        public static implicit operator string(ConnectionString connectionString) => connectionString.Value;
    }
}
