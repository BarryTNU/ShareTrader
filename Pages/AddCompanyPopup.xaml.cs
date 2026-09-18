namespace ShareTrader;

using ShareTrader.Services;
using ShareTrader.Helpers;

public partial class AddCompanyPopup : ContentPage
{
    string fPath = AppGlobals.PortfolioFile;
    public event Action? CompanyAdded;
    public AppGlobals.CompanyItem NewCompany { get; set; } = new AppGlobals.CompanyItem();
    private List<AppGlobals.CompanyItem> CompaniesPopup = new();

    private readonly List<string> Countries = new()
{
    "Australia",
    "USA",
    "UK",
    "NZ"
};

    private string SelectedCountry = "Australia";


    public AddCompanyPopup()
    {
        InitializeComponent();
        cvCountries.ItemsSource = Countries;
        cvCountries.SelectedItem = Countries[0];    

    }
    


    private void cvCountries_SelectionChanged(
      object sender,
      SelectionChangedEventArgs e)
    {   
        SelectedCountry = e.CurrentSelection.FirstOrDefault()?.ToString() ?? "";
    }


  
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(100);
        txtName.Focus();
    }


    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text) ||
            string.IsNullOrWhiteSpace(txtSymbol.Text) ||
            string.IsNullOrWhiteSpace(SelectedCountry))
        {
            CustomMessageBox.DefaultFocus = DefaultButton.OK;
            await CustomMessageBox.ShowAsync(
                "Missing Information",
                "Please enter Company Name, Symbol and Country.",
                MessageType.Information);
            return;
        }

        NewCompany = new AppGlobals.CompanyItem
        {
            Name = txtName.Text?.Trim() ?? "",
            Symbol = txtSymbol.Text?.Trim().ToUpper() ?? "",
            Country = SelectedCountry
        };


        FileManager.SaveCompany(NewCompany);
        FileManager.SavePortfolio(NewCompany);


        CustomMessageBox.DefaultFocus = DefaultButton.OK;
        await CustomMessageBox.ShowAsync(
            "Company Added",
            NewCompany.Name + " " + NewCompany.Symbol + " has been added.",
            MessageType.Information );
    
        CompanyAdded?.Invoke(); // Tell MainPage to refresh.
     // await  PortfolioManager.UpdatePortfolio(); // Refresh the portfolio data.

        await Navigation.PopModalAsync();
       
    }

    private bool CompanyExists(string name, string symbol)
    {
        foreach (var company in Dictionaries.AllCompanies)
        {
            // Check either the company name or the symbol.
            if (company.Key.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                company.Value.Equals(symbol, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private async void TestDownload_Clicked(object sender, EventArgs e)
    {
        btnTest.IsEnabled = false;
        this.IsVisible = false;
        await Navigation.PopModalAsync();


        if (string.IsNullOrWhiteSpace(txtName.Text) ||
     string.IsNullOrWhiteSpace(txtSymbol.Text) ||
     string.IsNullOrWhiteSpace(SelectedCountry))

        {
            CustomMessageBox.DefaultFocus = DefaultButton.OK;
            await CustomMessageBox.ShowAsync(
                "Missing Information",
                "Please enter Company Name, Symbol and Country.",
                MessageType.Information);
            return;
        }

        string companyName = txtName.Text.Trim();
        string symbol = txtSymbol.Text.Trim().ToUpper();

        string country = SelectedCountry;

        if (CompanyExists(companyName, symbol))
        {
            CustomMessageBox.DefaultFocus = DefaultButton.OK;
            await CustomMessageBox.ShowAsync(
                "Company Already Exists",
                $"{companyName} ({symbol}) already exists in the {country} list.",
                MessageType.Information);
            return;
        }
        string provider = AppGlobals.ConfigurationManager.APIProvider;
        

        try
        {
            bool success = await DownloadService.DownloadData(symbol, companyName);

                 if (success)
            

            {
                    // Write new company
                    string record = $"{companyName},{symbol}";

                    File.AppendAllText(fPath, record + Environment.NewLine);

                CustomMessageBox.DefaultFocus = DefaultButton.OK;
                await CustomMessageBox.ShowAsync(
                        "Portfolio",
                        companyName + " added to Portfolio.",
                        MessageType.Information);
                              

               // CompanyAdded?.Invoke(); // Tell MainPage to refresh.
                await  PortfolioManager.UpdatePortfolio(); // Refresh the portfolio data.
                                                           //  await Navigation.PopModalAsync();
              


                // Add to Log file.
                string logEntry = $"{companyName} added to Portfolio.";
                    FileManager.SaveLogFile(logEntry);

                }
                else
                {
                    string message = $"Your API Providor may not support this symbol ({symbol}) or the symbol is invalid. Please check and try again.";
                CustomMessageBox.DefaultFocus = DefaultButton.OK;
                await CustomMessageBox.ShowAsync(
                        "Download Failed",
                        $"{message}",
                        MessageType.Error);
                    success = false;
                }

        }
        finally
        {
            
        }     

    }
}

      