using System.Collections.Generic;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Models.DTOs.Converters;

internal class ImportStatusStringValues
{
    public static readonly Dictionary<ImportStatus, string> EnumToStringMap = new()
    {
        [ImportStatus.Error] = "Error",
        [ImportStatus.Finished] = "Finished",
        [ImportStatus.InProgress] = "In progress",
        [ImportStatus.Pending] = "Pending"
    };

    public static readonly Dictionary<string, ImportStatus> StringToEnumMap = new()
    {
        ["Error"] = ImportStatus.Error,
        ["Finished"] = ImportStatus.Finished,
        ["In progress"] = ImportStatus.InProgress,
        ["Pending"] = ImportStatus.Pending
    };
}