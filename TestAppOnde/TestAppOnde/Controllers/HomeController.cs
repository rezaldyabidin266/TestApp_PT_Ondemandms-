using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;
using System.Diagnostics;
using TestAppOnde.Models;

namespace TestAppOnde.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbContextSql _context;


        public HomeController(DbContextSql context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var dataTable = ReadExcelFile(filePath);
                SaveToDatabase(dataTable);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetTransaksiData()
        {
            var transaksiList = _context.transaksiContext.Select(t => new
            {
                t.Id,
                t.Invoice,
                t.Harga,
                t.Discount,
                t.User,
                t.Status,
                Tgl_Transaksi = t.Tgl_Transaksi.ToString("yyyy-MM-dd")
            }).ToList();

            return Json(transaksiList);
        }

        private DataTable ReadExcelFile(string filePath)
        {

            // Set the license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            FileInfo fileInfo = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                DataTable dt = new DataTable();

                // Create columns
                foreach (var header in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    dt.Columns.Add(header.Text);
                }

                // Load rows
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    DataRow dr = dt.NewRow();
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        dr[col - 1] = worksheet.Cells[row, col].Text;
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
        }

        private void SaveToDatabase(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                string invoice = row["Invoice"].ToString();

                // Cek apakah invoice sudah ada di database
                bool invoiceExists = _context.transaksiContext.Any(t => t.Invoice == invoice);

                if (!invoiceExists)
                {
                    var entity = new TransaksiModels
                    {
                        Invoice = row["Invoice"].ToString(),
                        Harga = Convert.ToInt32(row["Harga"]),
                        Discount = Convert.ToInt32(row["Discount"]),
                        User = row["User"].ToString(),
                        Status = row["Status"].ToString(),
                        Tgl_Transaksi = DateTime.TryParse(row["Tgl_Transaksi"].ToString(), out DateTime tglTransaksi) ? tglTransaksi : DateTime.MinValue
                    };
                    _context.transaksiContext.Add(entity);
                }

            }
            _context.SaveChanges();
        }

    }
}
