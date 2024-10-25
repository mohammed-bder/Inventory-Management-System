using Inventory_Management_System.Filters;
using Inventory_Management_System.Repository;
using Inventory_Management_System.Repository.repo;
using Inventory_Management_System.ViewModel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text.pdf.draw;
using OfficeOpenXml;
using System.Drawing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Management_System.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IProductRepository productRepository;
        private readonly IProductTransactionRepository productTransactionRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionController(ITransactionRepository transactionRepository,
                                    IProductRepository productRepository,
                                    IProductTransactionRepository productTransactionRepository,
                                    IEmployeeRepository employeeRepository,
                                    UserManager<ApplicationUser> userManager)
        {
            this.transactionRepository = transactionRepository;
            this.productRepository = productRepository;
            this.productTransactionRepository = productTransactionRepository;
            this.employeeRepository = employeeRepository;
            this._userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Transaction> transactions = transactionRepository.GetAll();
            ViewBag.TopSellingProducts = productTransactionRepository.GetTopSelling();
            ViewBag.TopEmployees = transactionRepository.GetTopEmployees();
            

            return View("Index", transactions);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            List<Product> availableProducts = productRepository.GetAllAvailable();

            TransactionWithProducts transactionWithProducts = new TransactionWithProducts
            {
                AvailableProducts = availableProducts
            };
            var myUser = await _userManager.GetUserAsync(User);
            ViewBag.EmpId = myUser.Employee_id;
            return View(transactionWithProducts);
        }

        [HttpPost]
        public async Task<IActionResult> ShowBill(TransactionWithProducts transactionWithProducts)
        {
            var myUser = await _userManager.GetUserAsync(User);
            ViewBag.EmpId = myUser.Employee_id;
            if (!ModelState.IsValid)
            {
                transactionWithProducts.AvailableProducts = productRepository.GetAllAvailable();
                return View("Add", transactionWithProducts);
            }

            List<int> productIds = transactionWithProducts.ProductDetails.Select(p => p.ProductId).ToList();//takes the products ids
            List<Product> products = productRepository.GetByIds(productIds);// fetch the specific products from db

            //check for available quantites 


            transactionWithProducts.AvailableProducts = productRepository.GetAllAvailable();

            double total = 0;
            for (int i = 0; i < products.Count; i++)//calculate the price 
            {
                total += transactionWithProducts.ProductDetails[i].Quantity * products[i].UnitPrice;
            }

            transactionWithProducts.SelectedProducts = products;
            transactionWithProducts.TotalPrice = total;
            return View(transactionWithProducts);
        }

        //list<products> , empid , quantity , total price
        [HttpPost]
        public IActionResult FinalizeTransaction(TransactionWithProducts transactionWithProducts)
        {
            // Step 1: Fetch Products and Update Stock Quantities
            List<int> productIds = transactionWithProducts.ProductDetails
                .Select(p => p.ProductId).ToList();
            List<Product> products = productRepository.GetByIds(productIds);

            for (int i = 0; i < transactionWithProducts.ProductDetails.Count; i++)//decrease quantity from db(update products)
            {
                if (productIds[i] == products[i].ID)
                {
                    products[i].StockQuantity -= transactionWithProducts.ProductDetails[i].Quantity;
                    productRepository.Update(products[i]);
                    productRepository.Save();
                }
            }

            // Step 2: Create a new Transaction Record
            Transaction transaction = new Transaction
            {
                EmployeeId = transactionWithProducts.EmployeeId,
                Date = DateTime.Now,
                Type = TransactionType.Sale,
                TotalPrice = transactionWithProducts.TotalPrice ?? 0
            };

            transactionRepository.Add(transaction);
            transactionRepository.Save();

            int transactionId = transactionRepository.GetLastTransactionId();
            for (int i = 0; i < transactionWithProducts.ProductDetails.Count; i++)//add productTransaction record (many to many relation)
            {
                ProductTransaction productTransaction = new ProductTransaction();
                productTransaction.ProductId = productIds[i];
                productTransaction.TransactionId = transactionId; // realtion with the employee
                productTransaction.Quantity = transactionWithProducts.ProductDetails[i].Quantity;

                productTransactionRepository.Add(productTransaction);
                productTransactionRepository.Save();
            }

            // Step 3: Generate PDF with Transaction and Product Data
            string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", $"Transaction_{transactionId}.pdf");
            Directory.CreateDirectory(Path.GetDirectoryName(pdfPath)); // Ensure directory exists

            using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter.GetInstance(pdfDoc, fs);
                pdfDoc.Open();

                // Add Icon at the Top Right
                string iconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "electronic.jpg");
                if (System.IO.File.Exists(iconPath))
                {
                    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(iconPath);
                    logo.ScaleToFit(50f, 50f);  // Adjust icon size
                    logo.SetAbsolutePosition(pdfDoc.PageSize.Width - 80f, pdfDoc.PageSize.Height - 80f);
                    pdfDoc.Add(logo);
                }

                // Add Title
                pdfDoc.Add(new Paragraph("Transaction Receipt",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, BaseColor.DarkGray))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                });

                Employee employee = employeeRepository.GetById(transaction.EmployeeId);
                // Add Transaction Details
                pdfDoc.Add(new Paragraph($"Transaction ID: {transactionId}\n" +
                    $"Date: {transaction.Date}\n" +
                    $"Employee : {employee.FName+" "+employee.LName}\n",
                    FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.Black))
                {
                    SpacingAfter = 10
                });

                AddSectionDivider(pdfDoc);

                // Create a Product Table
                PdfPTable table = new PdfPTable(4); // 4 columns: Product Name, Quantity, Price, Subtotal
                table.WidthPercentage = 100;
                table.AddCell(CreateCell("Product Name", Element.ALIGN_CENTER, true));
                table.AddCell(CreateCell("Quantity", Element.ALIGN_CENTER, true));
                table.AddCell(CreateCell("Price", Element.ALIGN_CENTER, true));
                table.AddCell(CreateCell("Subtotal", Element.ALIGN_CENTER, true));

                // Populate the Table with Product Data
                decimal grandTotal = 0;
                foreach (var detail in transactionWithProducts.ProductDetails)
                {
                    var product = products.FirstOrDefault(p => p.ID == detail.ProductId);
                    if (product != null)
                    {
                        decimal subtotal = detail.Quantity * (decimal)product.UnitPrice;
                        grandTotal += subtotal;

                        table.AddCell(CreateCell(product.Name, Element.ALIGN_CENTER));
                        table.AddCell(CreateCell(detail.Quantity.ToString(), Element.ALIGN_CENTER));
                        table.AddCell(CreateCell($"${product.UnitPrice:F2}", Element.ALIGN_CENTER));
                        table.AddCell(CreateCell($"${subtotal:F2}", Element.ALIGN_CENTER));
                    }
                }

                // Add the Total Price Row
                PdfPCell totalCell = new PdfPCell(new Phrase("Total", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                {
                    Colspan = 3,  // Span the first three columns
                    HorizontalAlignment = Element.ALIGN_LEFT,  // Align to the left
                    Border = iTextSharp.text.Rectangle.NO_BORDER,
                    Padding = 8
                };
                table.AddCell(totalCell);

                PdfPCell totalValueCell = new PdfPCell(new Phrase($"${grandTotal:F2}", FontFactory.GetFont(FontFactory.HELVETICA, 12)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,  // Keep total value aligned to the right
                    Border = iTextSharp.text.Rectangle.NO_BORDER,
                    Padding = 8
                };
                table.AddCell(totalValueCell);


                pdfDoc.Add(table);

                AddSectionDivider(pdfDoc);

                pdfDoc.Add(new Paragraph("Thank you for your purchase!",
                    FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 14, BaseColor.DarkGray))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 30
                });

                pdfDoc.Close();
            }

            // Step 4: Store PDF Path in TempData and Redirect to Index
            TempData["PdfPath"] = $"/pdfs/Transaction_{transactionId}.pdf";
            return RedirectToAction("Index");
        }

        // Helper Method to Create Table Cells
        private PdfPCell CreateCell(string text, int alignment, bool isHeader = false)
        {
            iTextSharp.text.Font font = isHeader ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)
                                 : FontFactory.GetFont(FontFactory.HELVETICA, 10);
            return new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = alignment,
                Border = iTextSharp.text.Rectangle.BOTTOM_BORDER,
                Padding = 8,
                PaddingBottom = 12
            };
        }

        // Helper Method to Add Section Divider
        private void AddSectionDivider(Document doc)
        {
            doc.Add(new Paragraph(new Chunk(new LineSeparator(1f, 100f, BaseColor.LightGray, Element.ALIGN_CENTER, -1)))
            {
                SpacingBefore = 10,
                SpacingAfter = 10
            });
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            var transactions = transactionRepository.GetAll(); // Fetch transactions

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Transactions");

                // Set the title with styling
                var titleCell = worksheet.Cells[1, 1, 1, 5]; // Merge across five columns
                titleCell.Merge = true;
                titleCell.Value = "Transactions List";
                titleCell.Style.Font.Color.SetColor(Color.White); // Font color
                titleCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Set fill pattern
                titleCell.Style.Fill.BackgroundColor.SetColor(Color.Blue); // Background color
                titleCell.Style.Font.Size = 16; // Increase font size
                titleCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Center text
                titleCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Center vertically
                titleCell.Style.Font.Bold = true; // Bold title

                // Set column headers
                var headerRow = worksheet.Cells[2, 1, 2, 5]; // Set the range for headers
                headerRow.Style.Font.Bold = true; // Make headers bold
                headerRow.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRow.Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Header background color
                headerRow.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); // Add border

                // Set headers text
                worksheet.Cells[2, 1].Value = "Transaction ID";
                worksheet.Cells[2, 2].Value = "Employee Name";
                worksheet.Cells[2, 3].Value = "Date";
                worksheet.Cells[2, 4].Value = "Total Price";
                worksheet.Cells[2, 5].Value = "Transaction Type";

                // Fill transaction data with alternating row colors and borders
                for (int i = 0; i < transactions.Count; i++)
                {
                    int rowIndex = i + 3; // Starting from the third row
                    var transaction = transactions[i];

                    // Fetch the employee details for the transaction
                    Employee employee = employeeRepository.GetById(transaction.EmployeeId);

                    worksheet.Cells[rowIndex, 1].Value = transaction.ID;
                    worksheet.Cells[rowIndex, 2].Value = $"{employee.FName} {employee.LName}";
                    worksheet.Cells[rowIndex, 3].Value = transaction.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[rowIndex, 4].Value = transaction.TotalPrice;
                    worksheet.Cells[rowIndex, 5].Value = transaction.Type.ToString();

                    // Set border for each cell in the row
                    for (int j = 1; j <= 5; j++)
                    {
                        worksheet.Cells[rowIndex, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    // Alternate row colors
                    if (i % 2 == 0)
                    {
                        worksheet.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.BackgroundColor.SetColor(Color.LightCyan);
                    }
                }

                // AutoFit columns
                worksheet.Cells.AutoFitColumns();

                // Set the width for the first column to prevent overflow
                worksheet.Column(1).Width = 15; // Set the width for Transaction ID

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = "Transactions.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        public IActionResult Delete(string selectedIds)// this action for admin only to save history of transaction
        {
            if (!string.IsNullOrEmpty(selectedIds))
            {
                var ids = selectedIds.Split(',').Select(int.Parse).ToList();

                foreach (int id in ids)
                {
                    Transaction transaction = transactionRepository.GetById(id);
                    transactionRepository.Delete(transaction);
                }
                transactionRepository.Save();
            }
            return RedirectToAction("Index");
        }


    }
}
