using PortEval.Domain.Models.Enums;
using System.Collections.Generic;

namespace PortEval.Application.Models.DTOs.Converters
{
    internal class ImportStatusStringValues
    {
        public static readonly Dictionary<ImportStatus, string> EnumToStringMap = new Dictionary<ImportStatus, string>()
        {
            [ImportStatus.Error] = "Error",
            [ImportStatus.Finished] = "Finished",
            [ImportStatus.InProgress] = "In progress",
            [ImportStatus.Pending] = "Pending"
        };

        public static readonly Dictionary<string, ImportStatus> StringToEnumMap = new Dictionary<string, ImportStatus>()
        {
            ["Error"] = ImportStatus.Error,
            ["Finished"] = ImportStatus.Finished,
            ["In progress"] = ImportStatus.InProgress,
            ["Pending"] = ImportStatus.Pending
        };
    }
}
