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
    /// Вычислить исполнителей по матрице согласования.
    /// </summary>
    /// <param name="task">Документ по которому ищутся критерии.</param>
    /// <returns>Список исполнителей.</returns>
    [ExpressionElement("GetMatrixPerformersName")]
    public static List<Sungero.Company.IEmployee> GetMatrixPerformers(Sungero.Docflow.IOfficialDocument document)
    {
      return GetMatrixPerformers(document, null);
    }
    
    /// <summary>
    /// Вычислить исполнителей по матрице согласования.
    /// </summary>
    /// <param name="task">Документ по которому ищутся критерии.</param>
    /// /// <param name="roleType">Тип роли, null если для no-code.</param>
    /// <returns>Список исполнителей.</returns>
    public static List<Sungero.Company.IEmployee> GetMatrixPerformers(Sungero.Docflow.IOfficialDocument document, Enumeration? roleType)
    {
      var recipients = Functions.Module.GetMatrixApprovalRoleRecipients(document, roleType);
      return Sungero.Company.PublicFunctions.Module.GetEmployeesFromRecipients(recipients);
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
    /// Получить матрицы, соответствующие критериям.
    /// </summary>
    /// <param name="doc">Согласуемый документ.</param>
    /// <param name="employee">Сотрудник.</param>
    /// <param name="roleType">Тип роли.</param>
    /// <returns>Матрицы согласований.</returns>
    [Public]
    public virtual System.Collections.Generic.IEnumerable<IApprovalMatrix> GetMatchingMatrices(Sungero.Docflow.IOfficialDocument doc, Sungero.Company.IEmployee employee, Enumeration? roleType)
    {
      // Найти участников роли согласования по записям матрицы согласования по критериям.
      // Полученные записи сортируются по количеству совпавших критериев, при этом поля матрицы с отсутствующим значеним считаются релевантными, но с низким приоритетом.
      // Затем записи сортируются по приоритету (соотвествующее поле матрицы согласования).
      var documentKind = doc.DocumentKind;
      var category = doc.DocumentGroup;
      var businessUnit = employee.Department.BusinessUnit;
      var department = employee.Department;
      var jobTitle = employee.JobTitle;
      
      return ApprovalMatrices.GetAll(matrix => (roleType == null ? (matrix.ApprovalRole == null) : (matrix.ApprovalRole.Type == roleType)) &&
                                     (matrix.Status == Sungero.CoreEntities.DatabookEntry.Status.Active) &&
                                     (matrix.DocumentKinds.Any(kinds => Equals(kinds.DocumentKind, documentKind))) &&
                                     (matrix.Categories.Any(categories => Equals(categories.Category, category)) || !matrix.Categories.Any() || category == null) &&
                                     (matrix.BusinessUnits.Any(units => Equals(units.BusinessUnit, businessUnit)) || !matrix.BusinessUnits.Any()) &&
                                     (matrix.Departments.Any(departments => Equals(departments.Department, department)) || !matrix.Departments.Any()) &&
                                     (matrix.JobTitles.Any(titles => Equals(titles.JobTitle, jobTitle)) || !matrix.JobTitles.Any())
                                    )
        // Отсортировать найденные записи по релевантности.
        .ToDictionary(x => x,
                      x => (x.Categories.Any(c => Equals(c.Category, category)) ? 1 : 0)
                      + (x.BusinessUnits.Any(b => Equals(b.BusinessUnit, businessUnit)) ? 1 : 0)
                      + (x.Departments.Any(d => Equals(d.Department, department)) ? 1 : 0)
                      + (x.JobTitles.Any(t => Equals(t.JobTitle, jobTitle)) ? 1 : 0)
                     )
        .OrderByDescending(x => x.Value)
        .ThenByDescending(x => x.Key.Priority)
        .Select(x => x.Key);
    }
    
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

      var matrice = GetMatchingMatrices(doc, employee, roleType).FirstOrDefault();
      
      if (matrice == null)
        return new List<IRecipient>();
      
      return matrice.Members.Select(m => m.Member).Distinct().ToList();
    }
  }
}