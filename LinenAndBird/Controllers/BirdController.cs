using LinenAndBird.DataAccess;
using LinenAndBird.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;

namespace LinenAndBird.Controllers
{
    [Route("api/birds")]
    [Authorize]
    [ApiController]
    public class BirdController : ControllerBase
    {
        BirdRepository _repo;

        //this is asking asp.net for a bird repo
        //this is known as Dependency Injection
        public BirdController(BirdRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllBirds()
        {
            var fbUserId = User.FindFirst(claim => claim.Type == "user_id").Value;
            return Ok(_repo.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetBirdById(Guid id)
        {
            var bird = _repo.GetById(id);

            if (bird == null)
            {
                return NotFound($"No bird with the id {id} was found.");
            }

            return Ok(bird);
        }

        [HttpPost]
        public IActionResult AddBird(Bird newBird)
        {
            if (string.IsNullOrEmpty(newBird.Name) || string.IsNullOrEmpty(newBird.Color))
            {
                return BadRequest("Name and Color are required fields");
            }

            _repo.Add(newBird);

            return Created($"/api/birds/{newBird.Id}", newBird);
        }

        //api/birds/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteBird(Guid id)
        {
            _repo.Remove(id);

            return Ok();
        }

        //api/birds/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateBird(Guid id, Bird bird)
        {
            var birdToUpdate = _repo.GetById(id);

            if (birdToUpdate == null)
            {
                return NotFound($"Could not find bird with the id {id} for updating");
            }

            var updatedBird = _repo.Update(id, bird);

            return Ok(updatedBird);

        }
    }
}
