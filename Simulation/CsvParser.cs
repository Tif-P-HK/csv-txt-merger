using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simulation
{
  /// <summary>
  /// A simple CSV parser which validates the input csv file and parses it
  /// </summary>
  class CsvDataParser
  {
    private string _filePath;
    public CsvDataParser(string filePath)
    {
      _filePath = filePath;
    }

    /// <summary>
    /// Create a TextFieldParser. 
    /// </summary>
    private TextFieldParser GetParser()
    {
      if (string.IsNullOrEmpty(_filePath))
        return null;

      return new TextFieldParser(_filePath)
      {
        TextFieldType = FieldType.Delimited,
        Delimiters = new string[] { "," },
        HasFieldsEnclosedInQuotes = true,
        TrimWhiteSpace = false
      };
    }

    /// <summary>
    /// Try to parse the input file to see if it has any malformated data. 
    /// </summary>
    public long TryToParseFile()
    {
      long lineWithError = -1;
      TextFieldParser parser = GetParser();
      if (parser == null)
        return lineWithError;

      using(parser)
      {
        while(!parser.EndOfData)
        {
          try
          {
            parser.ReadFields();
          }
          catch (MalformedLineException mle)
          {
            lineWithError = mle.LineNumber;
          }
        }
      }
      return lineWithError;
    }

    /// <summary>
    /// Parse the csv file. 
    /// </summary>
    public List<string[]> ParseFile()
    {
      List<string[]> rows = new List<string[]>();
      TextFieldParser parser = GetParser();
      if (parser == null)
        return null;

      using(parser)
      {
        try
        {
          while (!parser.EndOfData)
            rows.Add(parser.ReadFields().Select(f => f.Trim(new[] { ' ', '"' })).ToArray<string>());
        }
        catch (Exception ex)
        {
          //There should be no MalformedLineException at this point given that the file has been validated
          throw ex;
        }
      }
      return rows;
    }

    /// <summary>
    /// Same as parse file except that only the first line will be read.
    /// </summary>
    public string[] ParseHeaderLine(string headerLine)
    {
      string[] fields = null;
      TextFieldParser parser = GetParser();
      if (parser == null)
        return null;

      using (parser)
      {
        try
        {
          while (!parser.EndOfData)
          { 
            fields = parser.ReadFields().Select(f => f.Trim(new[] { ' ', '"' })).ToArray<string>();
            break;
          }
        }
        catch (Exception ex)
        {
          //There should be no MalformedLineException at this point given that the file has been validated
          throw ex;
        }
      }
      return fields;
    }
  }
}
