//using Aspose.Imaging;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationProject.Data;
using WebApplicationProject.Models;
//using WebApplicationProject.Utilities;

namespace WebApplicationProject.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext context;

        public ImageController(UserManager<AppUser> userManager, AppDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        //public IActionResult Index()
        // {
        //  var userId = userManager.GetUserId(User);
        // var images = context.Images.Where(i => i.UserId == userId).ToList();
        // return View(images);
        //}

        [ResponseCache(Duration = 3600)]
        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            // Calculate the number of items to skip
            int skip = (page - 1) * pageSize;

            // Retrieve the specified number of images based on the selected page and page size
            var images = context.Images.Skip(skip).Take(pageSize).ToList();
            //var images = context.Images.FromSqlRaw($"SELECT * FROM Images ORDER BY Id OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY").ToList();

            // Total number of images in the database
            var totalImages = context.Images.Count();

            // Pass data to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalImages / pageSize);

            return View(images);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please select a file");
                return View(file);
            }

            var userId = userManager.GetUserId(User);
            var fileName = Path.GetFileName(file.FileName);

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                var image = new Models.Image
                {
                    FileName = fileName,
                    ImageData = memoryStream.ToArray(),
                    UserId = userId!
                };

                context.Images.Add(image);
                await context.SaveChangesAsync();
                ViewBag.Message = fileName + " stored successfully!";
            }
            return View();
        }

        [HttpPost]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var image = context.Images.SingleOrDefault(i => i.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            context.Images.Remove(image);
            context.SaveChanges();
            TempData["Success"] = "Image Deleted";
            return RedirectToAction("Index");
        }

        /* [HttpPost]
        [HttpGet]
        public IActionResult Rotate(int id)
        {
            var image = context.Images.Find(id);

            if (image == null)
            {
                return NotFound();
            }

            // Rotate the image (assuming the ImageUtils.Rotate method is available)
            //image.ImageData = ImageUtils.Rotate(image.ImageData, 90); // Rotate by 90 degrees (adjust as needed)

           // image.ImageData = image.RotateFlip(RotateFlipType.Rotate270FlipNone);

            context.SaveChanges();

            // Redirect back to the Index page after rotation
            return RedirectToAction("Index");
        } */

        [HttpGet]
        public IActionResult Show(int id)
        {
            var image = context.Images.AsNoTracking().SingleOrDefault(i => i.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Edit(int id)
        {
            var image = context.Images.Find(id);

            if (image == null)
            {
                return NotFound();
            }

            // Rotate the image by 90 degrees
            byte[] rotatedImageData = RotateImage90Degrees(image.ImageData);

            // Update the ImageData in the database
            image.ImageData = rotatedImageData;

            // Save changes to the database
            context.Entry(image).State = EntityState.Modified;
            context.SaveChanges();

            // Redirect back to the index page
            return RedirectToAction("Index");
        }

        private byte[] RotateImage90Degrees(byte[] imageData)
        {
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                using (var originalImage = System.Drawing.Image.FromStream(ms))
                {
                    using (var rotatedImage = new System.Drawing.Bitmap(originalImage.Height, originalImage.Width))
                    {
                        rotatedImage.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);

                        using (var graphics = System.Drawing.Graphics.FromImage(rotatedImage))
                        {
                            graphics.TranslateTransform(rotatedImage.Width / 2, rotatedImage.Height / 2);
                            graphics.RotateTransform(90);
                            graphics.TranslateTransform(-rotatedImage.Height / 2, -rotatedImage.Width / 2);

                            graphics.DrawImage(originalImage, new System.Drawing.Point(0, 0));
                        }

                        using (MemoryStream rotatedMs = new MemoryStream())
                        {
                            rotatedImage.Save(rotatedMs, System.Drawing.Imaging.ImageFormat.Png);
                            return rotatedMs.ToArray();
                        }
                    }
                }
            }
        }
    }
}
