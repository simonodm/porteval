using System;
using PortEval.Domain.Models.Enums;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents a user-initiated bulk data import.
/// </summary>
public class DataImport : VersionedEntity, IAggregateRoot
{
    /// <summary>
    ///     ID of the data import.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    ///     Time at which the import was created.
    /// </summary>
    public DateTime Time { get; private set; }

    /// <summary>
    ///     Template using which the data was imported.
    /// </summary>
    public TemplateType TemplateType { get; private set; }

    /// <summary>
    ///     Processing status of the import.
    /// </summary>
    public ImportStatus Status { get; private set; }

    /// <summary>
    ///     An additional message describing the <see cref="Status" /> of the import.
    /// </summary>
    public string StatusDetails { get; private set; }

    /// <summary>
    ///     Whether an error log is available for this data import.
    /// </summary>
    public bool ErrorLogAvailable { get; private set; }

    /// <summary>
    ///     The file system path of the error log file.
    /// </summary>
    public string ErrorLogPath { get; private set; }

    internal DataImport(Guid id, DateTime time, TemplateType templateType, ImportStatus status = ImportStatus.Pending,
        string statusDetails = "",
        bool errorLogAvailable = false, string errorLogPath = "")
    {
        Id = id;
        TemplateType = templateType;
        Status = status;
        StatusDetails = statusDetails;
        ErrorLogAvailable = errorLogAvailable;
        ErrorLogPath = errorLogPath;
        Time = time;
    }

    /// <summary>
    ///     Creates the bulk data import.
    /// </summary>
    /// <param name="id">ID of the import.</param>
    /// <param name="time">Time at which the import was created.</param>
    /// <param name="templateType">Template using which the data was imported.</param>
    /// <param name="status">Processing status of the import.</param>
    /// <param name="statusDetails">An additional message describing the <paramref name="status" /> of the import.</param>
    /// <returns></returns>
    public static DataImport Create(Guid id, DateTime time, TemplateType templateType,
        ImportStatus status = ImportStatus.Pending, string statusDetails = "")
    {
        return new DataImport(id, time, templateType, status, statusDetails);
    }

    /// <summary>
    ///     Changes the processing status of the import.
    /// </summary>
    /// <param name="status">New processing status.</param>
    /// <param name="statusDetails">An additional message describing the status.</param>
    public void ChangeStatus(ImportStatus status, string statusDetails = "")
    {
        Status = status;
        StatusDetails = statusDetails;
    }

    /// <summary>
    ///     Adds an error log file to the import.
    /// </summary>
    /// <param name="errorLogPath">Path to the error log file.</param>
    public void AddErrorLog(string errorLogPath)
    {
        ErrorLogPath = errorLogPath;
        ErrorLogAvailable = true;
    }
}