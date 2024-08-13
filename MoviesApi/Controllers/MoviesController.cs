using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos;
using MoviesApi.Models;
using System.Reflection;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MoviesController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private new List<string> _allowedExe = new List<string>() { ".jpg", ".png" };
		private long _maxPosterSize = 1048576;
		public MoviesController(ApplicationDbContext context)
		{
			_context = context;
		}
		[HttpGet]

		public async Task<IActionResult> GetAllAsync()
		{
			var movies = await _context.Movies.OrderByDescending(m => m.Rate).Include(m => m.Genre).Select(m => new MovieDetailsDto
			{
				Id = m.Id,
				GenreId = m.GenreId,
				GenreName = m.Genre.Name,
				Poster = m.Poster,
				Rate = m.Rate,
				StoryLine = m.StoryLine,
				Title = m.Title,
				Year = m.Year
			})
				.ToListAsync();
			return Ok(movies);
		}

		[HttpGet("{id}")]
		public async Task <IActionResult>GetByIdAsync(int id)
		{
			var movie= await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m=>m.Id==id);
			if (movie == null)
				return NotFound();

			var dto = new MovieDetailsDto
			{
				Id = movie.Id,
				GenreId = movie.GenreId,
				GenreName = movie.Genre.Name,
				Poster = movie.Poster,
				Rate = movie.Rate,
				StoryLine = movie.StoryLine,
				Title = movie.Title,
				Year = movie.Year
			};
			return Ok(dto);
		}

		[HttpGet("GetByGenreIdAsync")]
		public async Task<IActionResult> GetByGenreIdAsync(int genreId)
		{
			var movies = await _context.Movies.OrderByDescending(m => m.Rate)
				.Where(m=>m.GenreId==genreId)
				.Include(m => m.Genre)
				.Select(m => new MovieDetailsDto
			{
				Id = m.Id,
				GenreId = m.GenreId,
				GenreName = m.Genre.Name,
				Poster = m.Poster,
				Rate = m.Rate,
				StoryLine = m.StoryLine,
				Title = m.Title,
				Year = m.Year
			})
				.ToListAsync();
			return Ok(movies);
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync([FromForm]MovieDto dto)
		{
			if (!_allowedExe.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
				return BadRequest();
			if (dto.Poster.Length > _maxPosterSize)
				return BadRequest();

			var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);

			if (!isValidGenre) return BadRequest();	
			// Convert  to bytes array 
			using var dataStream = new MemoryStream();
			await dto.Poster.CopyToAsync(dataStream);

			var movie = new Movie() 
			{ 
				GenreId= dto.GenreId,
				Title = dto.Title,
				Poster= dataStream.ToArray(),
				Rate= dto.Rate,
				StoryLine= dto.StoryLine,
				Year= dto.Year

			};
			await _context.AddAsync(movie);
			_context.SaveChanges();
			return Ok(movie);
		}

		[HttpDelete("{id}")]
		public async Task <IActionResult>DeleteAsync(int id)
		{
			var movie= await _context.Movies.FindAsync(id);

			if(movie==null)
				return NotFound();

			_context.Movies.Remove(movie);
			_context.SaveChanges();
			return Ok(movie);
		}

		[HttpPut ("{id}")]
		public async Task<IActionResult>UpadteAsync(int id, [FromForm] MovieDto dto)
		{
			var movie=await _context.Movies.FindAsync(id);
			if (movie==null)
				return NotFound();

			var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);

			if (!isValidGenre) return BadRequest();

			if(dto.Poster != null)
			{
				if (!_allowedExe.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
					return BadRequest();
				if (dto.Poster.Length > _maxPosterSize)
					return BadRequest();
				using var dataStream = new MemoryStream();
				await dto.Poster.CopyToAsync(dataStream);

			}
			movie.Title= dto.Title;
			movie.StoryLine= dto.StoryLine;
			movie.Year= dto.Year;
			movie.Rate= dto.Rate;
			movie.Year=dto.Year;
			_context.SaveChanges();
			return Ok(movie);
		}
	}
}
