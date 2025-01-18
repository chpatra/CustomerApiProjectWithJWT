using CutomerRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CustomerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;

        public UploadController(IRepository<Customer> repository)
        {
            _customerRepository = repository;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var customers = new List<Customer>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, false))
                {
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();

                   if(sheetData==null)
                    {
                        NotFound("File Not uploaded with Data");
                    }
                    foreach (Row row in sheetData.Elements<Row>().Skip(1))
                    {
                        var customer = new Customer
                        {
                            Name = GetCellValue(row, 1, workbookPart),
                            Age = int.Parse(GetCellValue(row, 2, workbookPart)),
                            Email = GetCellValue(row, 3, workbookPart),
                            Address = GetCellValue(row, 4, workbookPart)
                        };

                        customers.Add(customer);
                    }
                }
            }

            await _customerRepository.AddCustomersAsync(customers);
            return Ok(new { message = "File uploaded and data inserted successfully" });
        }

        private string GetCellValue(Row row, int columnIndex, WorkbookPart workbookPart)
        {
            var cell = row.Elements<Cell>().ElementAt(columnIndex - 1);
            var value = cell.InnerText;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (stringTable != null)
                {
                    value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                }
            }

            return value;
        }
    }
}

