# Blazor Pivot Table SQL Server Databinding using SqlClient data provider

This examples demonstrates, how to consume data from SQL Server using Microsoft SqlClient and bound it to Blazor Pivot Table. You can achieve this requirement by using [Custom Adaptor](https://blazor.syncfusion.com/documentation/datagrid/custom-binding/). 

Before the implementation, we need to add required NuGet like **Microsoft.Data.SqlClient** and Syncfusion.Blazor in your application. In the below sample, Custom Adaptor can be created as a Component.

In custom adaptor `Read` method you can get grid action details like paging, filtering, sorting information etc., using `DataManagerRequest`.
Based on the `DataManagerRequest`, you can form SQL query string (to perform paging) and execute the SQL query and retrieve the data from database using SqlDataAdapter. 
The Fill method of the DataAdapter is used to populate a DataSet with the results of the SelectCommand of the DataAdapter, then converted the DataSet into List and return Result and Count pair object in `Read` method to bind the data to Grid.

```xml
<SfPivotView TValue="Order">
    <PivotViewDataSourceSettings TValue="Order" ExpandAll="true" EnableSorting="true">
        <SfDataManager Adaptor="Adaptors.CustomAdaptor">
            <CustomAdaptorComponent></CustomAdaptorComponent>
        </SfDataManager>
        <PivotViewRows>
            <PivotViewRow Name=@nameof(Order.CustomerID) Caption="Customer ID"></PivotViewRow>
        </PivotViewRows>
        <PivotViewValues>
            <PivotViewValue Name=@nameof(Order.OrderID) Caption="Order ID"></PivotViewValue>
        </PivotViewValues>
    </PivotViewDataSourceSettings>
</SfPivotView>
@code{ 
	SfPivotView<Order> Pivot { get; set; }
	public static List<Order> Orders { get; set; }
    public class Order
    {
        public int? OrderID { get; set; }
        public string CustomerID { get; set; }
    }
}
```

```xml
@using Syncfusion.Blazor;
@using Syncfusion.Blazor.Data;
@using Newtonsoft.Json
@using static EFGrid.Pages.Index;
@using Microsoft.Data.SqlClient;
@using System.Data;
@using Microsoft.AspNetCore.Hosting;
@inject IHostingEnvironment _env

@inherits DataAdaptor<Order>

<CascadingValue Value="@this">
    @ChildContent
</CascadingValue>

@code {
    [Parameter]
    [JsonIgnore]
    public RenderFragment ChildContent { get; set; }
    public static DataSet CreateCommand(string queryString, string connectionString)
    {
        using (SqlConnection connection = new SqlConnection(
                   connectionString))
        {

            SqlDataAdapter adapter = new SqlDataAdapter(queryString, connection);
            DataSet dt = new DataSet();
            try
            {
                connection.Open();
                adapter.Fill(dt);// using sqlDataAdapter we process the query string and fill the data into dataset
            }
            catch (SqlException se)
            {
                Console.WriteLine(se.ToString());
            }
            finally
            {
                connection.Close();
            }
            return dt;
        }
    }
    // Performs data Read operation
    public override object Read(DataManagerRequest dm, string key = null)
    {
        string appdata = _env.ContentRootPath;
        string path = Path.Combine(appdata, "App_Data\\NORTHWND.MDF");
        string str = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename='{path}';Integrated Security=True;Connect Timeout=30";        // based on the skip and take count from DataManagerRequest here we formed SQL query string    
        string qs = "SELECT OrderID, CustomerID FROM dbo.Orders ORDER BY OrderID OFFSET " + dm.Skip + " 
ROWS FETCH NEXT " + dm.Take + " ROWS ONLY;";
        DataSet data = CreateCommand(qs, str);
        Orders = data.Tables[0].AsEnumerable().Select(r => new Order
        {
            OrderID = r.Field<int>("OrderID"),
            CustomerID = r.Field<string>("CustomerID")
        }).ToList();  // here we convert dataset into list
        IEnumerable<Order> DataSource = Orders;
        SqlConnection conn = new SqlConnection(str);
        conn.Open();
        SqlCommand comm = new SqlCommand("SELECT COUNT(*) FROM dbo.Orders", conn);
        Int32 count = (Int32)comm.ExecuteScalar();
        return dm.RequiresCounts ? new DataResult() { Result = DataSource, Count = count } : (object)DataSource;
    }
}
```