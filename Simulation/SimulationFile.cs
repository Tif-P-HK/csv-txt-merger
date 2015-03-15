using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Simulation
{
    /// <summary>
    /// A class representing a simulation file
    /// </summary>
  public class SimulationFile
  {
    //File name to be listed on list view
    public string FileName { get; private set; }
    //Field count discovered from the first line
    public int FieldCount { get; private set; }
    //Full file path 
    public string FilePath { get; private set; }

    //Indicate if the file includes a header line
    //This property is set by the UI
    public bool HasHeader 
    { 
      get { return hasHeader; }
      set
      {
        hasHeader = value;
        PopulateDataTable();
      } 
    }

    //Data table to be populated on data grid
    public SimulationFilesDataTable DataTable { get; private set; }

    static FileInfo file = null;
    //static int fieldCount = 0;
    private bool hasHeader = false;

    #region Static methods
    /// <summary>
    /// Validate the file extension and number of lines
    /// </summary>
    public static string ValidateFile(string filePath)
    {
      string validation = "";

      //Validate file extension
      if ((validation = ValidateFileExtension(filePath)) != "")
        return validation;

      //Validate the file is not empty
      if ((validation = ValidateLineCount(filePath)) != "")
        return validation;

      return validation;
    }
  
    #endregion

    public SimulationFile()
    {
      FilePath = file.FullName;
      FileName = file.Name;
      FieldCount = GetFieldCount();
    }

    //Create the ObservableCollection for binding
    public static ObservableCollection<SimulationFile> CreateFiles()
    {
      return new ObservableCollection<SimulationFile>();
    }

    public void PopulateDataTable()
    {
      if (hasHeader)
        PopulateTableWithHeader();
      else
        PopulateTableWithNoHeader();
    }

    #region Helper methods
    /// <summary>
    /// Populate the table when there's no header line
    /// </summary>
    private void PopulateTableWithNoHeader()
    {
      StreamReader sr = new StreamReader(FilePath);
      try
      {
        string data = "";
        string[] dataArr = null;

        data = sr.ReadLine();
        //if (!string.IsNullOrEmpty(data))
        //  fieldCount = data.Count(c => c == ',') + 1;

        //Create data table using the number of fields
        DataTable = new SimulationFilesDataTable(FieldCount);

        //Populate the table
        while (data != null)
        {
          //Won't trim the data line because the first/last field might just be blank
          //data = TrimLeadingTrailingComma(data);

          //Compare the count of fields with the first line of data
          //If the counts don't match, skip the line
          //TODO - Throw error if counts don't match
          if (!ValidateNumberOfFields(data))
          {
            data = sr.ReadLine();
            continue;
          }

          //create a row and add to table
          dataArr = data.Split(',');
          DataTable.Rows.Add((object[])dataArr);

          data = sr.ReadLine();
        }
      }
      catch (Exception)
      {
      }
      finally
      {
        sr.Close();
        sr.Dispose();
      }
    }

    /// <summary>
    /// Populate the table when the first line is the header line
    /// </summary>
    private void PopulateTableWithHeader()
    {
      string headerLine = GetHeaderLine();

      //Create data table
      DataTable = new SimulationFilesDataTable(headerLine);

      //Populate data table
      StreamReader sr = new StreamReader(FilePath);
      try
      {
        sr.ReadLine();
        string data = "";
        string[] dataArr = null;
        while ((data = sr.ReadLine()) != null)
        {
          //Won't trim the data line because the first/last field might just be blank
          //data = TrimLeadingTrailingComma(data);

          //Compare the count of fields with header
          //If the counts don't match, skip the line
          //TODO - Throw error if counts don't match
          if (!ValidateNumberOfFields(data))
            continue;

          //create a row and add to table
          dataArr = data.Split(',');
          DataTable.Rows.Add((object[])dataArr);
        }
      }
      catch (IOException)
      {
      }
      finally
      {
        sr.Close();
        sr.Dispose();
      }
    }
    
    /// <summary>
    // Validate file is either csv or txt
    /// </summary>
    private static string ValidateFileExtension(string filePath)
    {
      file = new FileInfo(filePath);
      if (file.Extension != ".csv" && file.Extension != ".txt")
        return "File must be csv or txt";
      else
        return "";
    }

    /// <summary>
    /// Validate the file is not empty
    /// </summary>
    private static string ValidateLineCount(string filePath)
    {
      int lineCount = File.ReadLines(filePath).Count();
      return (lineCount == 0) ? "File is empty" : "";
    }

    /// <summary>
    /// Get the header line
    /// </summary>
    private string GetHeaderLine()
    {
      return File.ReadLines(file.FullName).Take(1).First();
    }

    /// <summary>
    /// Get the number of fields
    /// </summary>
    private int GetFieldCount()
    {
      string firstLine = File.ReadLines(FilePath).Take(1).First();
      return firstLine.Count(c => c == ',') + 1;
    }

    private static string TrimLeadingTrailingComma(string line)
    {
      if (line.StartsWith(","))
        line = line.TrimStart(',');
      if (line.EndsWith(","))
        line = line.TrimEnd(',');
      return line;
    }

    /// <summary>
    /// Validate the number of fields matches the expected count
    /// (which might come from the header line or the first line of data)
    /// </summary>
    private bool ValidateNumberOfFields(string lineOfData)
    {
      return (lineOfData.Count(c => c == ',') + 1 == FieldCount) ? true : false;
    }
    #endregion
  }

  /// <summary>
  /// A collection of simulation files with functions for validation 
  /// </summary>
  public class SimulationFiles
  {
    public static ObservableCollection<SimulationFile> Files { get; set; }

    //Create the ObservableCollection for binding
    public static ObservableCollection<SimulationFile> CreateFiles()
    {
      return new ObservableCollection<SimulationFile>();
    }

    /// <summary>
    /// Get the field count of the new file based on the data from the first line
    /// Then compare the field count with the rest of the files
    /// </summary>
    public static bool ValidateFieldCount(string filePath)
    {
      string firstLine = File.ReadLines(filePath).Take(1).First();
      int newFileFieldCount = firstLine.Count(c => c == ',') + 1;

      return Files.All(f => f.FieldCount == newFileFieldCount);      
    }

    /// <summary>
    /// Get the header line of the new file 
    /// Then compare with the rest of the files
    /// </summary>
    /// <returns></returns>
    public static bool ValidateHeaders()
    {

      return false;
    }
  }
}
