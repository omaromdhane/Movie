using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace MovieStoreMvc.Controllers
{
    [Route("admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IFileService _fileService;
        private readonly IGenreService _genreService;
        public AdminController(IGenreService genService, IMovieService MovieService, IFileService fileService)
        {
            _movieService = MovieService;
            _fileService = fileService;
            _genreService = genService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Views/Admin_Views/Index.cshtml");
        }
        [HttpGet("genres")]
        public IActionResult GenreList()
        {
            var data = this._genreService.List().ToList();
            return View("~/Views/Admin_Views/Genre/GenreList.cshtml", data);
        }

        [HttpGet("genres/add")]
        public IActionResult AddGenre()
        {
            return View("~/Views/Admin_Views/Genre/Add.cshtml");
        }

        [HttpPost("genres/add")]
        public IActionResult AddGenrePost(Genre model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = _genreService.Add(model);
            if (result)
            {
                TempData["msg"] = "Added Successfully";
                return RedirectToAction(nameof(GenreList));
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View("~/Views/Admin_Views/Genre/Add.cshtml", model);
            }
        }

        [HttpGet("genres/edit")]
        public IActionResult EditGenre(int id)
        {
            var data = _genreService.GetById(id);
            return View("~/Views/Admin_Views/Genre/Edit.cshtml", data);
        }

        [HttpPost("genres/edit")]
        public IActionResult EditGenrePost(Genre model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = _genreService.Update(model);
            if (result)
            {
                TempData["msg"] = "Modified Successfully";
                return RedirectToAction("EditGenre", new { id = model.Id });
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View("~/Views/Admin_Views/Genre/Edit.cshtml", model);
            }
        }

        [HttpGet("genres/delete")]
        public IActionResult DeleteGenre(int id)
        {
            var result = _genreService.Delete(id);
            return RedirectToAction(nameof(GenreList));
        }

        /////////////////////////////////////////////////////////////////////// genre 
        ///

        [HttpGet("movies")]
        public IActionResult MovieList()
        {
            var data = this._movieService.List();
            return View("~/Views/Admin_Views/Movie/MovieList.cshtml", data);
        }



        [HttpGet("movies/add")]
        public IActionResult AddMovie()
        {
            var model = new Movie();
            model.GenreList = _genreService.List().Select(a => new SelectListItem { Text = a.GenreName, Value = a.Id.ToString() });
            return View("~/Views/Admin_Views/Movie/Add.cshtml",model);
        }




        [HttpPost("movies/add")]
        public IActionResult AddMoviePost(Movie model)
        {
            model.GenreList = _genreService.List().Select(a => new SelectListItem { Text = a.GenreName, Value = a.Id.ToString() });
            if (!ModelState.IsValid)
                return View(model);
            if (model.ImageFile != null)
            {
                var fileReult = this._fileService.SaveImage(model.ImageFile);
                if (fileReult.Item1 == 0)
                {
                    TempData["msg"] = "File could not saved";
                    return View(model);
                }
                var imageName = fileReult.Item2;
                model.MovieImage = imageName;
            }
            var result = _movieService.Add(model);
            if (result)
            {
                TempData["msg"] = "Added Successfully";
                return RedirectToAction(nameof(AddMovie));
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View("~/Views/Admin_Views/Movie/Add.cshtml", model);
            }

        }

        [HttpGet("movies/edit")]
        public IActionResult EditMovie(int id)
        {
            var model = _movieService.GetById(id);
            var selectedGenres = _movieService.GetGenreByMovieId(model.Id);
            MultiSelectList multiGenreList = new MultiSelectList(_genreService.List(), "Id", "GenreName", selectedGenres);
            model.MultiGenreList = multiGenreList;
            return View("~/Views/Admin_Views/Movie/Edit.cshtml", model);
        }




        [HttpPost("movies/edit")]
        public IActionResult EditMoviePost(Movie model)
        {
            var selectedGenres = _movieService.GetGenreByMovieId(model.Id);
            MultiSelectList multiGenreList = new MultiSelectList(_genreService.List(), "Id", "GenreName", selectedGenres);
            model.MultiGenreList = multiGenreList;
            if (!ModelState.IsValid)
                return View("~/Views/Admin_Views/Movie/Edit.cshtml",model);
            if (model.ImageFile != null)
            {
                var fileReult = this._fileService.SaveImage(model.ImageFile);
                if (fileReult.Item1 == 0)
                {
                    TempData["msg"] = "File could not saved";
                    return View(model);
                }
                var imageName = fileReult.Item2;
                model.MovieImage = imageName;
            }
            var result = _movieService.Update(model);
            if (result)
            {
                TempData["msg"] = "Modified Successfully";
                return RedirectToAction("EditMovie", new { id = model.Id });
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View(model);
            }
        }



        [HttpGet("movies/delete")]
        public IActionResult Delete(int id)
        {
            var result = _movieService.Delete(id);
            return RedirectToAction(nameof(MovieList));
        }


    }
}
