using backend.Features.Students;
using backend.Infrastructure;
using backend.Models;
using backend.Enums;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace backend.Infrastructure.Services
{
    public class CsvImportService
    {
        private readonly AppDbContext _db;

        public CsvImportService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ImportResultDto> ImportStudentsAsync(IFormFile file)
        {
            var result = new ImportResultDto();

            if (file == null || file.Length == 0)
            {
                result.Errors.Add(new ImportErrorDto { RowNumber = 0, Message = "File is empty or missing." });
                return result;
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var headerLine = await reader.ReadLineAsync();
            if (headerLine == null)
            {
                result.Errors.Add(new ImportErrorDto { RowNumber = 0, Message = "File is empty." });
                return result;
            }

            var headers = headerLine.Split(',').Select(h => h.Trim()).ToArray();
            var expectedHeaders = new[] { "student_id", "student_name", "student_email", "student_gender" };

            if (headers.Length < 4 || !expectedHeaders.SequenceEqual(headers, StringComparer.OrdinalIgnoreCase))
            {
                result.Errors.Add(new ImportErrorDto { RowNumber = 1, Message = "Invalid header. Expected: student_id,student_name,student_email,student_gender" });
                return result;
            }

            var rows = new List<string[]>();
            int currentRowNum = 1;
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                currentRowNum++;

                if (string.IsNullOrWhiteSpace(line)) continue;

                var columns = line.Split(',').Select(c => c.Trim()).ToArray();
                if (columns.Length < 4)
                {
                    result.Errors.Add(new ImportErrorDto { RowNumber = currentRowNum, Message = "Row missing required columns." });
                    result.SkippedRows++;
                    continue;
                }

                rows.Add(new[] { columns[0], columns[1], columns[2], columns[3], currentRowNum.ToString() });
            }

            result.TotalRows = rows.Count;

            // Extract all incoming IDs and emails to check DB
            var incomingIds = rows.Select(r => r[0]).Where(id => !string.IsNullOrEmpty(id)).ToList();
            var incomingEmails = rows.Select(r => r[2]).Where(e => !string.IsNullOrEmpty(e)).ToList();

            var existingIds = await _db.Students.Where(s => incomingIds.Contains(s.StudentId)).Select(s => s.StudentId.ToLower()).ToListAsync();
            var existingEmails = await _db.Students.Where(s => incomingEmails.Contains(s.Email)).Select(s => s.Email.ToLower()).ToListAsync();

            var dbIds = new HashSet<string>(existingIds);
            var dbEmails = new HashSet<string>(existingEmails);

            var fileSeenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var fileSeenEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var studentsToInsert = new List<Student>();
            string defaultPasswordHash = SecurityHelper.HashPassword("Student@123");

            foreach (var row in rows)
            {
                string studentId = row[0];
                string name = row[1];
                string email = row[2];
                string genderStr = row[3];
                int rowNum = int.Parse(row[4]);

                if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(genderStr))
                {
                    result.Errors.Add(new ImportErrorDto { RowNumber = rowNum, Message = "Missing required fields." });
                    result.SkippedRows++;
                    continue;
                }

                if (!MailAddress.TryCreate(email, out _))
                {
                    result.Errors.Add(new ImportErrorDto { RowNumber = rowNum, Message = "Invalid email format." });
                    result.SkippedRows++;
                    continue;
                }

                if (!Enum.TryParse<Gender>(genderStr, true, out var gender))
                {
                    result.Errors.Add(new ImportErrorDto { RowNumber = rowNum, Message = $"Invalid gender '{genderStr}'. Expected Male, Female, or Other." });
                    result.SkippedRows++;
                    continue;
                }

                // File duplicate check
                if (fileSeenIds.Contains(studentId) || fileSeenEmails.Contains(email))
                {
                    result.DuplicateRows++;
                    result.SkippedRows++;
                    continue;
                }
                fileSeenIds.Add(studentId);
                fileSeenEmails.Add(email);

                // DB duplicate check
                if (dbIds.Contains(studentId.ToLower()) || dbEmails.Contains(email.ToLower()))
                {
                    result.DuplicateRows++;
                    result.SkippedRows++;
                    continue;
                }

                studentsToInsert.Add(new Student
                {
                    StudentId = studentId,
                    Name = name,
                    Email = email,
                    Gender = gender,
                    PasswordHash = defaultPasswordHash,
                    IsActive = true
                });
            }

            if (studentsToInsert.Any())
            {
                _db.Students.AddRange(studentsToInsert);
                await _db.SaveChangesAsync();
            }

            result.ImportedRows = studentsToInsert.Count;
            return result;
        }
    }
}
