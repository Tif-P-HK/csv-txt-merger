using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Simulation
{
  /// <summary>
  /// Merge files, show result on a table and allow export
  /// </summary>
  public partial class MergeFiles : UserControl
  {
    //Data table to be populated on result data grid
    private SimulationDataTable FilesDataTable;

    private ObservableCollection<SimulationFile> simulationFiles = null;

    public MergeFiles()
    {
      InitializeComponent();

      simulationFiles = SimulationFiles.Files;
    }

    /// <summary>
    /// Fired when the page is loaded (whether on app init or when user clicks)
    /// </summary>
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (simulationFiles.Count == 0)
        return;

      //Create the data table with the columns
      CreateDataTable();

      //Populate from each of the fs. Pull one at a time
      try
      {
        PopulateDataTable();
      }
      catch (Exception)
      {        
      }

      //Show data table on UI
      grdDataGrid.DataContext = FilesDataTable;
    }

    /// <summary>
    /// Export the result as a csv file
    /// </summary>
    private void btnExport_Click(object sender, RoutedEventArgs e)
    {
      StringBuilder sb = ConvertTableToStringBuilder();

      SaveResult(sb);
    }

    /// <summary>
    /// Convert the data table to a string builder for save
    /// </summary>
    private StringBuilder ConvertTableToStringBuilder()
    {
      StringBuilder sb = new StringBuilder();

      //Headers
      var columnNames = FilesDataTable.Columns.Cast<DataColumn>().Select(col => col.ColumnName);
      sb.AppendLine(string.Join(",", columnNames));

      //Rows
      foreach (DataRow row in FilesDataTable.Rows)
        sb.AppendLine(string.Join(",", row.ItemArray));
      return sb;
    }

    /// <summary>
    /// Show a SaveFileDialog and save the file as .txt
    /// </summary>
    private async void SaveResult(StringBuilder sb)
    {
      SaveFileDialog dlg = new SaveFileDialog()
      {
        Filter = "Text Documents (*.txt)|*.txt",
        Title = "Save the text file"
      };

      dlg.ShowDialog();

      if (dlg.FileName == "")
        return;

      string filePath = dlg.FileName;

      try
      {
        using (StreamWriter sw = new StreamWriter(filePath, false))
        {
          await sw.WriteAsync(sb.ToString());
        }
      }
      catch (IOException)
      {
        MessageBox.Show("Error writing output to file. Please retry", "Error writing output", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    /// <summary>
    /// Create the data table with the columns
    /// </summary>
    private void CreateDataTable()
    {
      //Get the info from the first sf (which always has most fields)
      SimulationFile sf = simulationFiles.First();

      //Create the table based on the sf
      if (sf.HasHeader)
        FilesDataTable = new SimulationDataTable(sf.GetHeaderLine());
      else
        FilesDataTable = new SimulationDataTable(sf.FieldCount);
    }

    /// <summary>
    /// Populate from each of the fs. Pull one at a time
    /// </summary>
    private void PopulateDataTable()
    {
      //Get the number of loop (outer loop) from the file with the least number of lines
      SimulationFile file = simulationFiles.OrderBy(f => GetLineCountWithoutHeader(f)).First();
      int numberOfLoops = GetLineCountWithoutHeader(file);

      //Get the tables from each file, loop them through in the inner loop
      var individualTables = simulationFiles.Select<SimulationFile, SimulationDataTable>(sf => sf.DataTable );

      try
      {
        for (int i = 0; i < numberOfLoops; i++)
          for (int j = 0; j < individualTables.Count(); j++)
          {
            //Adding row from one table directly to another isn't allowed
            //FilesDataTable.Rows.ImportRow() doesn't handle the rows with less fields very well
            //Hence use ItemArray instead
            var itemArr = individualTables.ElementAt(j).Rows[i].ItemArray;
            FilesDataTable.Rows.Add(itemArr);
          }            
      }
      catch (Exception)
      {
      }
    }

    /// <summary>
    /// Get the number of lines in the file without the header
    /// </summary>
    private int GetLineCountWithoutHeader(SimulationFile f)
    {
      return f.HasHeader ? f.TotalLineCount - 1 : f.TotalLineCount;
    }
  }
}
