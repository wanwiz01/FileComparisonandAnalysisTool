using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileComparisonandAnalysisTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // --- ส่วนที่ต้องกำหนดค่า ---
            string file1Path = @"C:\Users\wanwisa.r\Downloads\first_file.txt";
            string file2Path = @"C:\Users\wanwisa.r\Downloads\second_file.txt";
            string outputFilePath = @"C:\path\to\your\analysis_report.md";

            // ระบุชื่อคอลัมน์ (Header) ที่ต้องการตรวจสอบ
            string columnToAnalyze = "serial_number";
            // -------------------------

            try
            {
                // เรียกฟังก์ชันเพื่อวิเคราะห์และรับผลลัพธ์เป็น Markdown
                string markdownResult = AnalyzeAndCompareFiles(file1Path, file2Path, columnToAnalyze);

                // บันทึกผลลัพธ์ลงไฟล์ .md
                File.WriteAllText(outputFilePath, markdownResult, Encoding.UTF8);

                Console.OutputEncoding = Encoding.UTF8;
                Console.WriteLine("✅ การวิเคราะห์เสร็จสมบูรณ์");
                Console.WriteLine($"ผลลัพธ์ถูกบันทึกที่: {outputFilePath}");
                Console.WriteLine("\n--- เนื้อหาไฟล์ผลลัพธ์ ---\n");
                Console.WriteLine(markdownResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        public static string AnalyzeAndCompareFiles(string file1Path, string file2Path, string columnName)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t", // หรือเปลี่ยนเป็น "," หากไฟล์ของคุณใช้ comma
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null
            };

            // อ่านข้อมูลจากไฟล์ทั้งสอง
            var records1 = ReadCsvFile(file1Path, config);
            var records2 = ReadCsvFile(file2Path, config);

            // ดึงข้อมูลเฉพาะคอลัมน์ที่ต้องการมาวิเคราะห์
            var serials1 = records1.Select(r => r[columnName]).ToList();
            var serials2 = records2.Select(r => r[columnName]).ToList();

            // 1. หาข้อมูลที่ซ้ำกันภายในไฟล์ที่ 1
            var duplicatesInFile1 = serials1
                .GroupBy(s => s)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            // 2. หาข้อมูลที่ซ้ำกันภายในไฟล์ที่ 2
            var duplicatesInFile2 = serials2
                .GroupBy(s => s)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            // 3. หาข้อมูลที่มีร่วมกันระหว่างสองไฟล์
            var commonSerials = new HashSet<string>(serials1)
                .Intersect(new HashSet<string>(serials2))
                .ToList();

            // 4. สร้างผลลัพธ์ด้วย StringBuilder ในรูปแบบ Markdown
            var report = new StringBuilder();
            report.AppendLine($"# ผลการวิเคราะห์ไฟล์: คอลัมน์ `{columnName}`");
            report.AppendLine($"* **ไฟล์ที่ 1:** `{Path.GetFileName(file1Path)}`");
            report.AppendLine($"* **ไฟล์ที่ 2:** `{Path.GetFileName(file2Path)}`");
            report.AppendLine($"* **วิเคราะห์เมื่อ:** `{DateTime.Now:dd MMMM yyyy HH:mm:ss}`");
            report.AppendLine("\n---");

            // ผลลัพธ์ส่วนที่ 1
            report.AppendLine($"## 🧐 ข้อมูล `{columnName}` ที่ซ้ำกันภายใน **ไฟล์ที่ 1**");
            if (duplicatesInFile1.Any())
            {
                foreach (var serial in duplicatesInFile1)
                {
                    report.AppendLine($"- `{serial}`");
                }
            }
            else
            {
                report.AppendLine("ไม่พบข้อมูลซ้ำ");
            }
            report.AppendLine("\n---");

            // ผลลัพธ์ส่วนที่ 2
            report.AppendLine($"## 🧐 ข้อมูล `{columnName}` ที่ซ้ำกันภายใน **ไฟล์ที่ 2**");
            if (duplicatesInFile2.Any())
            {
                foreach (var serial in duplicatesInFile2)
                {
                    report.AppendLine($"- `{serial}`");
                }
            }
            else
            {
                report.AppendLine("ไม่พบข้อมูลซ้ำ");
            }
            report.AppendLine("\n---");

            // ผลลัพธ์ส่วนที่ 3
            report.AppendLine($"## 🔄️ ข้อมูล `{columnName}` ที่มีอยู่ **ทั้งสองไฟล์**");
            if (commonSerials.Any())
            {
                foreach (var serial in commonSerials)
                {
                    report.AppendLine($"- `{serial}`");
                }
            }
            else
            {
                report.AppendLine("ไม่พบข้อมูลที่มีร่วมกัน");
            }

            return report.ToString();
        }

        // ฟังก์ชันสำหรับอ่านไฟล์ (เหมือนกับโค้ดก่อนหน้า)
        private static List<Dictionary<string, string>> ReadCsvFile(string filePath, CsvConfiguration config)
        {
             var reader = new StreamReader(filePath, Encoding.UTF8);
             var csv = new CsvReader(reader, config);

            var records = new List<Dictionary<string, string>>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var header in csv.HeaderRecord)
                {
                    record[header] = csv.GetField(header);
                }
                records.Add(record);
            }
            return records;
        }
    }
}
