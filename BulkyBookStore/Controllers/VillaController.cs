using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace BulkyBookStore.Controllers
{

    public class VillaController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment  _webHostHostEnvironment;
        public VillaController(ApplicationDbContext db, IWebHostEnvironment  webHostHostEnvironment)
        {
            _db = db;
            this._webHostHostEnvironment = webHostHostEnvironment;
        }
        public IActionResult Index()
        {
            var villas = _db.Villas.ToList();
            return View(villas);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if (obj.Name == obj.Description)
            {
                ModelState.AddModelError("", "  The description can not be the same as the Name. ");
            }


            if (ModelState.IsValid)
            {
                 if(obj.Image != null)
                {
                   string fileName = Guid.NewGuid()+Path.GetExtension(obj.Image.Name);
                    string imagePath = Path.Combine(_webHostHostEnvironment.WebRootPath, @"images\VillaImage");

                    using (var fileStream = new FileStream(Path.Combine(imagePath,fileName), FileMode.Create))
                    {
                        obj.Image.CopyTo(fileStream);
                         
                        obj.ImageUrl = @"\images\VillaImage\" + fileName;
                    }

                }
                 else
                {
                    obj.ImageUrl = "https://placehold.co/600x400";
                }

                _db.Villas.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "The villa has been created successfully";

                return RedirectToAction("Index", "Villa");
            }
            return View();
        }

        public IActionResult Update(int villaId)
        {
            Villa? obj = _db.Villas.FirstOrDefault(x => x.Id == villaId);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(Villa obj)
        {
         
            if (ModelState.IsValid)
            {

                _db.Villas.Update(obj);
                _db.SaveChanges ();
                TempData["success"] = "The Villa has been updated successfully";

                return RedirectToAction("Index", "Villa");
            }
            return View();
        }

        public IActionResult Delete(int villaId)
        {
            Villa? obj = _db.Villas.FirstOrDefault(x => x.Id == villaId);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }


        [HttpPost]
        public IActionResult Delete(Villa obj)
        {

            Villa objFromDb = _db.Villas.FirstOrDefault( u => u.Id == obj.Id);  
            if (objFromDb is not null )
            {

                _db.Villas.Remove(objFromDb);
                _db.SaveChanges();
                TempData["success"] = "The villa has been deleted successfully.";
                return RedirectToAction("Index", "Villa");
            }
            TempData["error"] = " The villa could not be deleted";
            return View();
        }



    }
}
