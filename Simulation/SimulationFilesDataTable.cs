using System.Data;

namespace Simulation
{
  /// <summary>
  /// Data table which shows a selected file from the list of loaded files
  /// </summary>
  public class SimulationFilesDataTable : DataTable
  {
    //Create a data table with a given header line
    public SimulationFilesDataTable(string headerLine)
    {
      var fields = headerLine.Split(',');

      foreach (string field in fields)
        this.Columns.Add(field);
    }

    //Create a data table with no header and only the number of fields
    public SimulationFilesDataTable(int fieldCount)
    {
      for (int i = 0; i < fieldCount; i++)
        this.Columns.Add();
    }
  }
}
