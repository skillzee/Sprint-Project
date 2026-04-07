using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Hotel.Data;
using TravelApp.Services.Hotel.DTOs;
using TravelApp.Services.Hotel.Interfaces;
using TravelApp.Services.Hotel.Models;

namespace TravelApp.Services.Hotel.Controllers
{
    [Route("api/hotels")]
    [ApiController]
    public class HotelsController : ControllerBase
    {

        private readonly IHotelService _hotelService;
        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetAll([FromQuery] string? city)
        {
            var result = await _hotelService.GetHotelsAsync(city);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> Get(int id)
        {
            var result = await _hotelService.GetHotelByIdAsync(id);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(CreateHotelDto dto)
        {

            var result = await _hotelService.CreateHotelAsync(dto);

            if(result == null)
            {
                return BadRequest();
            }

            return Ok(result);


        }

        [HttpPost("{id}/rooms")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddRoom(int id, CreateRoomDto dto)
        {
            var result = await _hotelService.AddRoomToHotelAsync(id, dto);

            if(result == null)
            {
                return BadRequest();    
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _hotelService.DeleteHotelAsync(id);

            if(result == false)
            {
                return BadRequest();
            }

            return Ok();
        }






    }
}
