#  File Comparison and Analysis Tool

## How It Works ⚙️

This C\# application provides a robust solution for comparing data across two input files, specifically focusing on a user-defined column (e.g., `serial_number`). It leverages the `CsvHelper` library for efficient parsing of delimited files and generates a comprehensive Markdown report summarizing its findings.

### Key Features:

1.  **Duplicate Detection within File 1:** Identifies and lists all values that appear more than once in the specified column within the first input file.
2.  **Duplicate Detection within File 2:** Identifies and lists all values that appear more than once in the specified column within the second input file.
3.  **Common Data Identification:** Finds and lists all values that are present in the specified column of *both* input files.
4.  **Markdown Report Generation:** Outputs the analysis results into a structured and easy-to-read Markdown file, including clear headings and bullet points for each section.

### Technical Breakdown:

  * **File Reading:** The `ReadCsvFile` function, using `CsvHelper`, reads data from the specified text/CSV files. It's configured to handle tab-delimited files by default but can be easily adjusted for comma-delimited files. It reads the header and then iterates through each record, storing it as a `Dictionary<string, string>` where keys are column headers.
  * **Duplicate Finding (within a single file):**
      * The `GroupBy(s => s)` LINQ method is used to group identical `serial_number` values.
      * `.Where(g => g.Count() > 1)` filters these groups, retaining only those that contain more than one instance, indicating a duplicate.
      * `.Select(g => g.Key)` extracts the actual duplicate `serial_number` value.
  * **Common Data Finding (between two files):**
      * `new HashSet<string>(serials1)` creates a **HashSet** from the `serial_number` values of the first file. HashSets are highly optimized for fast lookups and set operations as they only store unique values.
      * `.Intersect(new HashSet<string>(serials2))` is then used on these two HashSets. This method efficiently returns a new collection containing only the elements that are common to both sets.
  * **Markdown Report Generation:**
      * A `StringBuilder` is used to efficiently construct the Markdown report string.
      * Markdown syntax such as `#` (for main headings), `##` (for subheadings), `-` (for bullet points), and `` ` `` (backticks for inline code/highlighting specific values like serial numbers) are programmatically added to format the output beautifully.

## How to Use 📝

1.  **Install CsvHelper:** If you haven't already, install the `CsvHelper` NuGet package in your Console App project:
    ```bash
    Install-Package CsvHelper
    ```
2.  **Copy and Paste the Code:** Place the entire C\# code into your Console App project (e.g., in `Program.cs`).
3.  **Configure Paths and Column:**
      * Modify the `file1Path`, `file2Path`, and `outputFilePath` variables in the `Main` method to point to your specific input files and desired output location.
        ```csharp
        string file1Path = @"C:\path\to\your\first_file.txt";
        string file2Path = @"C:\path\to\your\second_file.txt";
        string outputFilePath = @"C:\path\to\your\analysis_report.md";
        ```
      * **Crucially**, update the `columnToAnalyze` variable to the exact name of the column you wish to analyze (e.g., `"serial_number"`, `"ID"`, `"product_code"`). The application assumes your files have headers.
        ```csharp
        string columnToAnalyze = "serial_number"; 
        ```
4.  **Run the Program:** Press `F5` in Visual Studio (or run from the command line). The program will execute, display progress in the console, and generate a Markdown report (`.md` file) at the specified `outputFilePath`. This `.md` file can be opened with any text editor or a Markdown-compatible viewer like VS Code.

-----
