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
      TotalLineCount = GetLineCount();
      FieldCount = GetFieldCount();
    }

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
        using (StreamReader sr = new StreamReader(FilePath))
        {
          string data = sr.ReadLine();

          //Create data table using the number of fields
          DataTable = new SimulationDataTable(FieldCount);

          //Populate the table
          while (data != null)
          {
            //Get the data line, validate if fields looks good and split it into string array
            string[] dataArr = FixExtraOrMissingFields(data);

            //create a row from the dataArray and add to table
            DataTable.Rows.Add((object[])dataArr);

            data = sr.ReadLine();
          }
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

      //Create data table
      DataTable = new SimulationDataTable(headerLine);

      //Populate data table
      try
      {
        using (StreamReader sr = new StreamReader(FilePath))
        {
          //Read off the header line
          sr.ReadLine();

          string data = "";
          while ((data = sr.ReadLine()) != null)
          {
            //Get the data line, validate if fields looks good and split it into string array
            string[] dataArr = FixExtraOrMissingFields(data);

            //create a row from the dataArray and add to table
            DataTable.Rows.Add((object[])dataArr);
          }
        }
      }
      catch (IOException)
      {
      }
    }

    /// <summary>
    /// Check the field count of the data line. Remove any extra and add more fields if some are missing
    /// </summary>
    private string[] FixExtraOrMissingFields(string data)
    {
      string[] dataArr = data.Split(',');

      //Compare the count of fields (dataArr.Count) with the expected count
      //difference > 0: Given line has more fields than expected. Take only up to what the field count allows
      //difference < 0: Given line has less fields than expected. Add more empty strings to fill the space
      //difference = 0: Field count matched. No extra action needed. 
      int differnce = DataFieldCountCompareExpectedCount(dataArr);
      if (differnce > 0)
        dataArr = dataArr.Take(FieldCount).ToArray();
      else if (differnce < 0)
      {
        string[] tempArr = Enumerable.Repeat("", FieldCount).ToArray();
        for (int i = 0; i < dataArr.Count(); i++)
          tempArr[i] = dataArr[i];
        dataArr = tempArr;
      }
      return dataArr;
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
      return firstLine.Count(c => c == ',') + 1;
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
      //return (lineOfData.Count(c => c == ',') + 1 == FieldCount) ? true : false;

      //int countLineOfData = lineOfData.Count(c => c == ',') + 1;
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
