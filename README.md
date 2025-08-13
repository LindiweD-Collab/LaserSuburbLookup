# Suburb Looker WinForms Application

This is a C# Windows Forms application built in .NET that demonstrates API integration and UI data display. The application fetches a list of suburbs from a specified API based on a user's search query, sorts the results alphabetically, and displays them in a grid.

---

### Features

- **API Authentication**: Retrieves a bearer token before making data requests.
- **Asynchronous API Calls**: Uses `async/await` and `HttpClient` for non-blocking network requests, ensuring the UI remains responsive.
- **Dynamic Search**: Users can type a search term to find matching suburbs.
- **Data Display**: Results are sorted and shown in a clean `DataGridView`.
- **Error Handling**: Gracefully handles API and network errors with user-friendly messages.
- **Configurable Settings**: API URL and credentials can be changed easily without modifying the code.

---

### How to Run the Application

1. **Open the Project**: Open the `LaserSuburbLookup.sln` file in Visual Studio 2022 or later.
2. **Restore Dependencies**: The required NuGet packages (`Microsoft.Extensions.Configuration.*`) should restore automatically. If not, right-click the solution in Solution Explorer and select **Restore NuGet Packages**.
3. **Build**: Build the solution by pressing `Ctrl+Shift+B` or from the **Build** menu.
4. **Run**: Press `F5` or the **Start** button to run the application.

---

### How to Change API Credentials

All API-related settings are stored in the `appsettings.json` file. This file is the single source of truth for configuration.

#### 1. In Solution Explorer, find the `appsettings.json` file.
#### 2. Open the file to edit the values.

```
{
  "ApiSettings": {
    "BaseUrl": "https://laserrest.laserlogistics.co.za/api/Laser/",
    "Username": "Cash",
    "Password": "Cash123"
  }
}
```
#### 3. Modify the BaseUrl, Username, or Password fields as needed.

#### 4. Save the file and restart the application for the changes to take effect.

## Assumptions Made

JSON Structure: The suburb data is expected to be a JSON array of objects, where each object contains a suburbName property (camelCase), as this is a common C#/.NET convention. The JsonPropertyName attribute is used to map this to the SuburbName C# property.

HttpClient Lifetime: A single, static HttpClient instance is used throughout the application's lifetime to prevent socket exhaustion and improve performance, which is a recommended best practice.

## Project Structure
```
LaserSuburbLookup/
├── LaserSuburbLookup.sln
├── LaserSuburbLookup/
│   ├── LaserSuburbLookup.csproj
|   ├──Resources/
|   |   └──laser-logistics.png
|   ├──Models/
|   |    └──Suburb.cs
│   ├── UI/
│   │   └── MainForm.cs
│   ├── Services/
│   │   └── ApiClient.cs
│   ├── appsettings.json
│   └── Program.cs
├── .gitignore
└── README.md
```
Project Repository: https://github.com/LindiweD-Collab/LaserSuburbLookup

