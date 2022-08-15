// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly DataContext _context;

    public PlayerController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ListofPlayer>>> GetAll()
    {
        try
        {

            #region List of Players
            IEnumerable<ListofPlayer> data = 
                await _context.Players
                        .Select(c =>
                            new ListofPlayer
                            {
                                Id = c.Id,
                                Name = c.Name,
                                Position = c.Position,
                            }
                        )
                        .ToListAsync();
            #endregion
            foreach (var item in data)
            {
                List<PlayerSkills> res = await _context.PlayerSkills
                        .Where(x => x.PlayerId.Equals(item.Id))
                        .Select(c =>
                            new PlayerSkills
                            {
                                Id = c.Id,
                                Skill = c.Skill,
                                Value = c.Value,
                                PlayerId = c.PlayerId,
                            }
                        )
                        .ToListAsync();
                item.PlayerSkills = res;
            }
            return Ok(data);
        }
        catch (Exception e)
        {
            ErrorResponse error = new ErrorResponse(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ListofPlayer>> PostPlayer(Player player)
    {
        try
        {
            if (player == null) { return NoContent(); }
            ValidationData checkPosition = new ValidationData(player.Position, null);
            if (checkPosition.Position == null)
            {
                ErrorResponse error = new ErrorResponse("Invalid value for position: " + player.Position);
                return BadRequest(error);
            }

            foreach (var item in player.PlayerSkills)
            {
                ValidationData check = new ValidationData(null, item.Skill);
                if (check.Skill == null)
                {
                    ErrorResponse error = new ErrorResponse("Invalid value for skill: " + item.Skill);
                    return BadRequest(error);
                }
            }

            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();

            #region List of Created Player
            ListofPlayer data = await _context.Players.Where(x => x.Id.Equals(player.Id))
                .Select(c =>
                    new ListofPlayer
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Position = c.Position,
                    }
                ).SingleOrDefaultAsync();

            List<PlayerSkills> res = await _context.PlayerSkills
                .Where(x => x.PlayerId.Equals(player.Id))
                .Select(c =>
                    new PlayerSkills
                    {
                        Id = c.Id,
                        Skill = c.Skill,
                        Value = c.Value,
                        PlayerId = c.PlayerId,
                    }
                )
                .ToListAsync();
            data.PlayerSkills = res;
            #endregion
            return Ok(data);
        }
        catch (Exception e)
        {
            ErrorResponse error = new ErrorResponse(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
    }

    [HttpPut("{playerId}")]
    public async Task<ActionResult<ListofPlayer>> PutPlayer(int playerId, Player player)
    {
        try
        {
            var data = await _context.Players.SingleOrDefaultAsync(x => x.Id == playerId);
            if (data != null)
            {
                ValidationData checkPosition = new ValidationData(player.Position, null);
                if (checkPosition.Position == null)
                {
                    ErrorResponse error = new ErrorResponse("Invalid value for position: " + player.Position);
                    return BadRequest(error);
                }
                data.Name = player.Name;
                data.Position = player.Position;
                _context.Entry(data).State = EntityState.Modified;

                foreach (var item in player.PlayerSkills)
                {
                    ValidationData check = new ValidationData(null, item.Skill);
                    if (check.Skill == null)
                    {
                        ErrorResponse error = new ErrorResponse("Invalid value for skill: " + item.Skill);
                        return BadRequest(error);
                    }

                    item.PlayerId = data.Id;
                    await _context.PlayerSkills.AddAsync(item);
                    await _context.SaveChangesAsync();
                }

                #region List of Updated Player
                ListofPlayer result = await _context.Players.Where(x => x.Id.Equals(data.Id))
                    .Select(c =>
                        new ListofPlayer
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Position = c.Position,
                        }
                    ).SingleOrDefaultAsync();

                List<PlayerSkills> res = await _context.PlayerSkills
                    .Where(x => x.PlayerId.Equals(data.Id))
                    .Select(c =>
                        new PlayerSkills
                        {
                            Id = c.Id,
                            Skill = c.Skill,
                            Value = c.Value,
                            PlayerId = c.PlayerId,
                        }
                    )
                    .ToListAsync();
                result.PlayerSkills = res;
                #endregion
                return Ok(result);
            }
            return BadRequest(new ErrorResponse("Data Not Found!"));
        }
        catch (Exception e)
        {
            ErrorResponse error = new ErrorResponse(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
    }

    // [Authorize]
    [HttpDelete("{playerId}")]
    public async Task<IActionResult> DeletePlayer(int playerId)
    {
        if (this.HttpContext.Request.Headers["Authorization"].ToString()
            .Equals("Bearer SkFabTZibXE1aE14ckpQUUxHc2dnQ2RzdlFRTTM2NFE2cGI4d3RQNjZmdEFITmdBQkE="))
        {
            try
            {
                var data = await _context.Players.FirstOrDefaultAsync(x => x.Id == playerId);
                if (data != null)
                {
                    _context.Players.Remove(data);
                }
                await _context.SaveChangesAsync();
                return Ok(new ErrorResponse("Data player has been deleted"));
            }
            catch (Exception e)
            {
                ErrorResponse error = new ErrorResponse(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, error);
            }
        }
        return StatusCode(StatusCodes.Status401Unauthorized, new ErrorResponse("Unauthorized"));
    }
}