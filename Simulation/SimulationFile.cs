using System;
using System.Collections.Generic;
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
    //Total number of lines (including header, if any)
    public int TotalLineCount { get; private set; }

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
    public SimulationDataTable DataTable { get; private set; }

    private static FileInfo file = null;
    private bool hasHeader = false;
    private static CsvDataParser parser = null;

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

      //Validation passed. Set up the parser
      parser = new CsvDataParser(filePath);

      return validation;
    }

    /// <summary>
    /// Validate if the file has malformated data
    /// </summary>
    public static string ValidateMalformatedData()
    {
      long lineWithError = parser.TryToParseFile();
      return lineWithError > 0 ? string.Format("Line {0} has some invalid data. File cannot be used.", lineWithError) : "";
    }

    public SimulationFile()
    {
      FilePath = file.FullName;
      FileName = file.Name;
      TotalLineCount = GetLineCount();
      FieldCount = GetFieldCount();
    }

    /// <summary>
    /// Populate the data table from the input csv
    /// </summary>
    private void PopulateDataTable()
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
      try
      {
        //Create data table using the number of fields
        DataTable = new SimulationDataTable(FieldCount);

        //Parse the data
        List<string[]> dataRows = parser.ParseFile();
        if (dataRows == null)
          return;

        for (int i = 0; i < dataRows.Count; i++)
        {
          //For each row, validate if fields looks good
          string[] dataArr = FixExtraOrMissingFields(dataRows[i]);

          //create a row from the dataArray and add to table
          DataTable.Rows.Add(dataArr);
        }
      }
      catch (Exception)
      {
      }
    }

    /// <summary>
    /// Populate the table when the first line is the header line
    /// </summary>
    private void PopulateTableWithHeader()
    {
      string headerLine = GetHeaderLine();

      //Fields on header line must not contain leading or trailing spaces, 
      //otherwise the corresponding column won't appear on the data grid
      headerLine = TrimFieldsLeadingTrailingSpaces(headerLine);

      //Populate data table
      try
      {
        //Create data table
        string[] fields = parser.ParseHeaderLine(headerLine);
        DataTable = new SimulationDataTable(fields);

        //Parse the data
        List<string[]> dataRows = parser.ParseFile();
        if (dataRows == null)
          return;

        for (int i = 0; i < dataRows.Count;i++ )
        {
          //skip the first line (header line)
          if (i == 0) 
            continue;

          //For each row, validate if fields looks good
          string[] dataArr = FixExtraOrMissingFields(dataRows[i]);

          //create a row from the dataArray and add to table
          DataTable.Rows.Add(dataArr);
        }

      }
      catch (IOException)
      {
      }
    }

    /// <summary>
    /// Check the field count of the data line. Remove any extra and add more fields if some are missing
    /// </summary>
    private string[] FixExtraOrMissingFields(string[] rowDataArray)
    {
      //Compare the count of fields (dataArr.Count) with the expected count
      //difference > 0: Given line has more fields than expected. Take only up to what the field count allows
      //difference < 0: Given line has less fields than expected. Add more empty strings to fill the space
      //difference = 0: Field count matched. No extra action needed. 
      int differnce = DataFieldCountCompareExpectedCount(rowDataArray);
      if (differnce > 0)
        rowDataArray = rowDataArray.Take(FieldCount).ToArray();
      else if (differnce < 0)
      {
        string[] tempArr = Enumerable.Repeat("", FieldCount).ToArray();
        for (int i = 0; i < rowDataArray.Count(); i++)
          tempArr[i] = rowDataArray[i];
        rowDataArray = tempArr;
      }
      return rowDataArray;
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
    public string GetHeaderLine()
    {
      return File.ReadLines(this.FilePath).Take(1).First();
    }

    /// <summary>
    /// Get the number of fields
    /// </summary>
    private int GetFieldCount()
    {
      string firstLine = File.ReadLines(FilePath).Take(1).First();
      string[] fields = parser.ParseHeaderLine(firstLine);
      return fields.Count();
    }

    /// <summary>
    /// Get the total number of lines
    /// </summary>
    private int GetLineCount()
    {
      return File.ReadLines(FilePath).Count();
    }

    /// <summary>
    /// Get each field from the header, then trim their leading and trailing spaces
    /// </summary>
    private string TrimFieldsLeadingTrailingSpaces(string header)
    {
      string outHeader = "";
      var fields = header.Split(',');
      string _field = "";
      foreach (string field in fields)
      {
        _field = field;
        while (_field.StartsWith(" "))
          _field = _field.TrimStart(' ');
        while (_field.EndsWith(" "))
          _field = _field.TrimEnd(' ');

        outHeader += _field + ",";
      }
      return outHeader.TrimEnd(',');
    }

    /// <summary>
    /// Validate that the number of fields matches the expected count
    /// (which might come from the header line or the first line of data)
    /// </summary>
    private int DataFieldCountCompareExpectedCount(string[] dataArr)
    {
      int difference = dataArr.Count() - FieldCount;
      return difference;
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
  }
}
