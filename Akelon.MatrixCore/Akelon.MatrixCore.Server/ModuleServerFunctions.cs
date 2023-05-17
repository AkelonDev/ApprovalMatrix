using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Akelon.MatrixCore.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Получить исполнителей роли согласования по матрице согласования.
    /// </summary>
    /// <param name="doc">Согласуемый документ.</param>
    /// <param name="roleType">Тип роли.</param>
    /// <returns>Исполнители роли согласования.</returns>
    [Public]
    public virtual List<IRecipient> GetMatrixApprovalRoleRecipients(Sungero.Docflow.IOfficialDocument doc, Enumeration? roleType)
    {
      var employee = GetEmployeeByDoc(doc);
      if (employee == null)
        return new List<IRecipient>();

      return GetMatrixApprovalRoleRecipients(roleType, doc.DocumentKind, doc.DocumentGroup, employee.Department.BusinessUnit, employee.Department, employee.JobTitle);
    }
    
    /// <summary>
    /// Получить сотрудника по документу.
    /// </summary>
    /// <param name="doc">Документ.</param>
    /// <returns>Сотрудник или null.</returns>
    public virtual Sungero.Company.IEmployee GetEmployeeByDoc(Sungero.Docflow.IOfficialDocument doc)
    {
      // Договорные документы.
      if (Sungero.Contracts.ContractualDocuments.Is(doc))
        return Sungero.Contracts.ContractualDocuments.As(doc).ResponsibleEmployee;
      // Внутренние документы.
      if (Sungero.Docflow.InternalDocumentBases.Is(doc))
        return Sungero.Docflow.InternalDocumentBases.As(doc).PreparedBy;
      // Исходящие документы.
      if (Sungero.Docflow.OutgoingDocumentBases.Is(doc))
        return Sungero.Docflow.OutgoingDocumentBases.As(doc).PreparedBy;
      // Входящие документы.
      if (Sungero.Docflow.IncomingDocumentBases.Is(doc))
        return Sungero.Docflow.IncomingDocumentBases.As(doc).Addressee;
      
      // Оффициальный документ.
      return Sungero.Company.Employees.As(doc.Author);
    }
    
    /// <summary>
    /// Получить исполнителей роли согласования по матрице согласования.
    /// </summary>
    /// <param name="roleType">Тип роли.</param>
    /// <param name="documentKind">Вид документа.</param>
    /// <param name="category">Категория документа.</param>
    /// <param name="unit">Наша организация.</param>
    /// <param name="department">Подразделение.</param>
    /// <param name="jobTitle">Должность.</param>
    /// <returns>Исполнители роли согласования.</returns>
    [Public]
    public virtual List<IRecipient> GetMatrixApprovalRoleRecipients(Enumeration? roleType,
                                                                    Sungero.Docflow.IDocumentKind documentKind,
                                                                    Sungero.Docflow.IDocumentGroupBase category,
                                                                    Sungero.Company.IBusinessUnit unit,
                                                                    Sungero.Company.IDepartment department,
                                                                    Sungero.Company.IJobTitle jobTitle)
    {
      var matrice = GetMatchingMatrices(roleType, documentKind, category, unit, department, jobTitle).FirstOrDefault();
      
      if (matrice == null)
        return new List<IRecipient>();
      
      return matrice.Members.Select(m => m.Member).Distinct().ToList();
    }
    
    /// <summary>
    /// Получить матрицы, соответствующие критериям.
    /// </summary>
    /// <param name="roleType">Тип роли.</param>
    /// <param name="documentKind">Вид документа.</param>
    /// <param name="category">Категория документа.</param>
    /// <param name="unit">Наша организация.</param>
    /// <param name="department">Подразделение.</param>
    /// <param name="jobTitle">Должность.</param>
    /// <returns>Матрицы согласований.</returns>
    [Public]
    public virtual System.Collections.Generic.IEnumerable<IApprovalMatrix> GetMatchingMatrices(Enumeration? roleType,
                                                                                               Sungero.Docflow.IDocumentKind documentKind,
                                                                                               Sungero.Docflow.IDocumentGroupBase category,
                                                                                               Sungero.Company.IBusinessUnit unit,
                                                                                               Sungero.Company.IDepartment department,
                                                                                               Sungero.Company.IJobTitle jobTitle)
    {
      // Найти участников роли согласования по записям матрицы согласования по критериям.
      // Полученные записи сортируются по количеству совпавших критериев, при этом поля матрицы с отсутствующим значеним считаются релевантными, но с низким приоритетом.
      // Затем записи сортируются по приоритету (соотвествующее поле матрицы согласования).

      return ApprovalMatrices.GetAll(matrix => (matrix.ApprovalRole.Type == roleType) &&
                                     (matrix.Status == Sungero.CoreEntities.DatabookEntry.Status.Active) &&
                                     (matrix.DocumentKinds.Any(kinds => Equals(kinds.DocumentKind, documentKind))) &&
                                     (matrix.Categories.Any(categories => Equals(categories.Category, category)) || !matrix.Categories.Any() || category == null) &&
                                     (matrix.BusinessUnits.Any(units => Equals(units.BusinessUnit, unit)) || !matrix.BusinessUnits.Any()) &&
                                     (matrix.Departments.Any(departments => Equals(departments.Department, department)) || !matrix.Departments.Any()) &&
                                     (matrix.JobTitles.Any(titles => Equals(titles.JobTitle, jobTitle)) || !matrix.JobTitles.Any())
                                    )
        // Отсортировать найденные записи по релевантности.
        .ToDictionary(x => x,
                      x => (x.Categories.Any(c => Equals(c.Category, category)) ? 1 : 0)
                      + (x.BusinessUnits.Any(b => Equals(b.BusinessUnit, unit)) ? 1 : 0)
                      + (x.Departments.Any(d => Equals(d.Department, department)) ? 1 : 0)
                      + (x.JobTitles.Any(t => Equals(t.JobTitle, jobTitle)) ? 1 : 0)
                     )
        .OrderByDescending(x => x.Value)
        .ThenByDescending(x => x.Key.Priority)
        .Select(x => x.Key);
    }
  }
}