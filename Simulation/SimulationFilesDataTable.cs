using System.Data;

namespace Simulation
{
  /// <summary>
  /// Data table showing preview of data from either a simulation file or a merged file
  /// </summary>
  public class SimulationDataTable : DataTable
  {
    //Create a data table with a given header line
    public SimulationDataTable(string[] fields)
    {
      foreach (string field in fields)
        this.Columns.Add(field);
    }

    //Create a data table with no header and only the number of fields
    public SimulationDataTable(int fieldCount)
    {
      for (int i = 0; i < fieldCount; i++)
        this.Columns.Add();
    }
  }
}
