using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Simulation
{
  /// <summary>
  /// Allow users to add csv/txt files, and view the data of each of the file on a table
  /// </summary>
  public partial class LoadFiles : UserControl
  {
    public ObservableCollection<SimulationFile> SimulationFiles
    {
      get { return Simulation.SimulationFiles.Files; }
      private set { if (value != null) Simulation.SimulationFiles.Files = value; }
    }

    public SimulationDataTable DataTable { get; set; }
    
    public LoadFiles()
    {
      InitializeComponent();

      SimulationFiles = new ObservableCollection<SimulationFile>();

      DataContext = this;
    }

    /// <summary>
    /// Remove a file based on the selectd index
    /// Then select the file following the deleted one
    /// </summary>
    private void DeleteMenuItem_Clicked(object sender, RoutedEventArgs e)
    {
      int lastSelectedIndex = lstFiles.SelectedIndex;
      if (lastSelectedIndex == -1)
      {
        MessageBox.Show("Nothing to remove");
        return;
      }
      SimulationFiles.RemoveAt(lastSelectedIndex);
      UpdateSelectedIndexAfterRemove(lastSelectedIndex);
    }

    /// <summary>
    /// Event when changing between files and data tabs
    /// </summary>
    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      //Show a table representing the data on the Data tab
      //If there's no no file, don't show anything on Data tab
      //Otherwise, get the datatable from sf and set it as the data context of the data grid
      if ((sender as TabControl).SelectedIndex == 1)
      {
        int selectedIndex = lstFiles.SelectedIndex;

        if (selectedIndex == -1)
          DataTable = null;
        else
        {
          SimulationFile sf = SimulationFiles.ElementAt(selectedIndex);
          DataTable = sf.DataTable;
        }
        grdFileDataGrid.DataContext = DataTable;
      }
    }

    /// <summary>
    /// Browse for a csv/txt file, then validate and load it
    /// </summary>
    private void btnBrowseAndLoad_Click(object sender, RoutedEventArgs e)
    {
      string filePath = BrowseFile();

      if (filePath == "")
        return;

      //Do some simple validation on the file before adding it to the file list
      string validation;
      if ((validation = SimulationFile.ValidateFile(filePath)) != "")
      {
        //Validate that file is a txt/csv and has at least one line 
        ShowValidationError(validation);
      }
      else if((validation = SimulationFile.ValidateMalformatedData()) != "")
      {
        //Validate that file doesn't have malformated data
        ShowValidationError(validation);
      }
      else
      {        
        //Check the field count
        bool fieldCountMatched = Simulation.SimulationFiles.ValidateFieldCount(filePath);
        if (fieldCountMatched)
          AddSimulationFile();
        else
        {
          //Field count doesn't match with one or more of the other files. Prompt to confirm or cancel
          MessageBoxResult result = MessageBox.Show("Field count doesn't seem to match other files. Continue using this file?",
            "Field count not match",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

          if (result == MessageBoxResult.No)
            filePath = "";
          else if (result == MessageBoxResult.Yes)
            AddSimulationFile();
        }
      }
    }

    private void ShowValidationError(string validation)
    {
      //File has problem. Show error
      MessageBox.Show(validation,
        "File has problem",
        MessageBoxButton.OK,
        MessageBoxImage.Exclamation);
    }

    private void AddSimulationFile()
    {
      //Add file to the file list and select it
      SimulationFile sf = new SimulationFile();
      if (SimulationFiles.Count > 0 && sf.FieldCount > SimulationFiles[0].FieldCount)
        SimulationFiles.Insert(0, sf);
      else
        SimulationFiles.Add(sf);

      UpdateSelectedIndexAfterAdd();
    }

    /// <summary>
    /// Update the selected file index after adding a new one
    /// to select the latest file
    /// </summary>
    private void UpdateSelectedIndexAfterAdd()
    {
      lstFiles.SelectedIndex = SimulationFiles.Count - 1;
    }

    /// <summary>
    /// Update the selected file index after removal:
    /// - If there's only one file left, select that one
    /// - If the deleted file is the last one, select the one before it
    /// - Else select the one after the one being deleted
    /// </summary>
    private void UpdateSelectedIndexAfterRemove(int lastSelectedIndex)
    {
      if (SimulationFiles.Count == 1)
        lstFiles.SelectedIndex = 0;
      else if (lastSelectedIndex >= SimulationFiles.Count)
        lstFiles.SelectedIndex = SimulationFiles.Count - 1;
      else
        lstFiles.SelectedIndex = lastSelectedIndex;
    }

    /// <summary>
    /// Browse for a csv/txt file
    /// </summary>
    private string BrowseFile()
    {
      string filePath;

      OpenFileDialog dialog = new OpenFileDialog()
      {
        DefaultExt = ".txt",
        Filter = "Text Documents (*.txt)|*.txt|CSV (Comma delimited)|*.csv"
      };

      bool? dlgResult = dialog.ShowDialog();
      if (dlgResult == true)
        filePath = dialog.FileName;
      else
        filePath = "";

      return filePath;
    }

    /// <summary>
    /// Transition to the next flip view item to handle the merging
    /// </summary>
    private void btnMerge_Click(object sender, RoutedEventArgs e)
    {
      var flipView = this.Parent as FlipView;
      if (flipView != null)
        flipView.SelectedIndex = flipView.SelectedIndex + 1;
    }
  }
}