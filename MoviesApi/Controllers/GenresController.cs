﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos;
using MoviesApi.Models;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GenresController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		
		public GenresController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllAsync()
		{
			var genres= await _context.Genres.OrderBy(g=>g.Name).ToListAsync();
			return Ok(genres);

		}
		[HttpPost]
		public async Task<IActionResult> CreateAsync(CreateGenreDto dto)
		{
			
			var genre = new Genre() { Name=dto.Name };
			await _context.Genres.AddAsync(genre);
			_context.SaveChanges();
			return Ok(genre);
		}
		[HttpPut ("{id}")]
		public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateGenreDto dto)
		{
			var genre= await _context.Genres.SingleOrDefaultAsync(g=>g.Id == id);
			if (genre == null) { return NotFound($"No genre was found with id {id}"); }
			
			genre.Name = dto.Name;
			_context.SaveChangesAsync();
			return Ok(genre);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult>DeleteAsync(int id)
		{
			var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
			if (genre == null) { return NotFound($"No genre was found with id {id}"); }

			_context.Genres.Remove(genre);
			_context.SaveChanges();
			return Ok(genre);
		}
	}
}
