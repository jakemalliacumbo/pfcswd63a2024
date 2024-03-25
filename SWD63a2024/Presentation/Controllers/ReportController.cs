using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Presentation.Repositories;
using System.Drawing;
using System.Xml.Linq;

namespace Presentation.Controllers
{
    public class ReportController : Controller
    {

        PubSubRepository _pubSubRepository;
        PostsRepository _postsRepository;
        public ReportController(PubSubRepository pubSubRepository, 
            PostsRepository postsRepository) { 
        _pubSubRepository= pubSubRepository;
            _postsRepository= postsRepository;
        }

        public async Task< IActionResult> Index()
        {
           string blogId = _pubSubRepository.PullMessagesSync("pfc-jmc-2024-sub", true);
            if (string.IsNullOrEmpty(blogId))
            {
                return Content("None to convert");
            }
            else
            {

                // Create a new PDF document
                PdfDocument document = new PdfDocument();

                // Add a new page to the document
                PdfPage page = document.AddPage();

                var myPosts = await _postsRepository.GetPosts(blogId);

                int yPosition = 10;
                // Get an XGraphics object for drawing
                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                {

                    foreach (var post in myPosts)
                    {

                        // Create a font
                        XFont font = new XFont("Verdana", 12, XFontStyleEx.Regular);

                        // Draw the text on the page
                        gfx.DrawString(post.Title, font, XBrushes.Black, new XRect(10, yPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        // Move to the next line (increase Y-coordinate position)
                        yPosition += font.Height;
                        gfx.DrawString(post.Content, font, XBrushes.Black, new XRect(10, yPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        // Move to the next line (increase Y-coordinate position)
                        yPosition += font.Height;
                        gfx.DrawString("-----------------------------------------------", font, XBrushes.Black, new XRect(10, yPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        // Move to the next line (increase Y-coordinate position)
                        yPosition += (font.Height*3);

                    }
                }

                MemoryStream msOut = new MemoryStream();
                // Save the PDF document
                document.Save(msOut, true);
           
               //save memorystream into bucket
            
            
            }

           


            return View();
        }
    }
}
