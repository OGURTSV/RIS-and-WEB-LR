namespace backend.Models;

/// <summary>Сервисная учётная запись AD — справочник (docs/data-model.md, 2.2).</summary>
public class ServiceAccount
{
    public int Id { get; set; }

    /// <summary>Логин учётки в AD, например svc_backup.</summary>
    public string SamAccountName { get; set; } = "";

    /// <summary>ФИО владельца; пусто — «ничейная» учётка (это фактор риска).</summary>
    public string? OwnerName { get; set; }

    /// <summary>Связанные системы, например «1С, СУБД бухгалтерии».</summary>
    public string? RelatedSystems { get; set; }

    public bool IsPrivileged { get; set; }
    public bool PasswordNeverExpires { get; set; }
    public bool IsGmsaCompatible { get; set; }
    public bool IsEnabled { get; set; }
}
