using System;
using PortEval.Domain.Models.Enums;

namespace PortEval.Domain.Models.Entities
{
    public class DataImport : IAggregateRoot
    {
        public Guid Id { get; private set; }
        public DateTime Time { get; private set; }
        public TemplateType TemplateType { get; private set; }
        public ImportStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public bool ErrorLogAvailable { get; private set; }
        public string ErrorLogPath { get; private set; }

        public DataImport(Guid id, TemplateType templateType, ImportStatus status = ImportStatus.Pending, string statusDetails = "")
        {
            Id = id;
            TemplateType = templateType;
            Status = status;
            StatusDetails = statusDetails;
            ErrorLogAvailable = false;
            Time = DateTime.UtcNow;
        }

        public DataImport(Guid id, TemplateType templateType, DateTime time, ImportStatus status = ImportStatus.Pending, string statusDetails = "")
        {
            Id = id;
            TemplateType = templateType;
            Status = status;
            StatusDetails = statusDetails;
            ErrorLogAvailable = false;
            Time = time;
        }

        public void ChangeStatus(ImportStatus status, string statusDetails = "")
        {
            Status = status;
            StatusDetails = statusDetails;
        }

        public void AddErrorLog(string errorLogPath)
        {
            ErrorLogPath = errorLogPath;
            ErrorLogAvailable = true;
        }
    }
}
