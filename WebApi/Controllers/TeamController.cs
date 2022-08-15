// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly DataContext _context;

        public TeamController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("process")]
        public async Task<ActionResult<List<PlayerVM>>> Process(List<TeamProcessVM> players)
        {
            try
            {
                List<PlayerVM> list = new List<PlayerVM>();
                var countPosition = players.GroupBy(s => s.Position)
                        .Select(x => new { x, count = x.Count() })
                        .ToList();
                foreach (var item in countPosition)
                {
                    var countSkill = item.x.GroupBy(b => b.MainSkill).Select(s => new { s, count = s.Count() }).ToList();
                    foreach (var c in countSkill)
                    {
                        if (c.count > 1)
                            return BadRequest(new ErrorResponse("The request just allow one skill for one position. The same skill can use in different position."));
                    }
                    if (item.count > 1)
                    {
                        return BadRequest(new ErrorResponse("The same position doesn't allow in the request."));
                    }
                }
                foreach (TeamProcessVM item in players)
                {
                    ValidationData validation = new ValidationData(item.Position, item.MainSkill);
                    if (validation.Position == null)
                    {
                        ErrorResponse error = new ErrorResponse("Insufficient number of players for position: " + item.Position);
                        return BadRequest(error);
                    }
                    if (validation.Skill == null)
                    {
                        ErrorResponse error = new ErrorResponse("Insufficient number of players for mainskill: " + item.MainSkill);
                        return BadRequest(error);
                    }

                    var checkData = await _context.PlayerSkills
                            .Where(x => x.Player.Position.ToLower().Equals(item.Position.ToLower()))
                            .Where(s => s.Skill.ToLower().Equals(item.MainSkill.ToLower())).ToListAsync();
                    if (checkData.Count == 0)
                    {
                        var list1 = await _context.PlayerSkills
                            .Where(x => x.Player.Position.ToLower().Equals(item.Position.ToLower()))
                            .OrderByDescending(a => a.Value)
                            .Select(c =>
                                new PlayerVM
                                {
                                    Name = c.Player.Name,
                                    Position = c.Player.Position,
                                    PlayerSkills = new PlayerSkillVM
                                    {
                                        Skill = c.Skill,
                                        Value = c.Value,
                                    },
                                }
                            )
                            .Take(item.NumberOfPlayers)
                            .ToListAsync();
                        list.AddRange(list1);
                    }
                    else
                    {
                        var list2 = await _context.PlayerSkills
                            .Where(x => x.Player.Position.ToLower().Equals(item.Position.ToLower()))
                            .Where(s => s.Skill.ToLower().Equals(item.MainSkill.ToLower()))
                            .OrderByDescending(a => a.Value)
                            .Select(c =>
                                new PlayerVM
                                {
                                    Name = c.Player.Name,
                                    Position = c.Player.Position,
                                    PlayerSkills = new PlayerSkillVM
                                    {
                                        Skill = c.Skill,
                                        Value = c.Value,
                                    },
                                }
                            )
                            .Take(item.NumberOfPlayers)
                            .ToListAsync();
                        list.AddRange(list2);
                    }
                }
                return Ok(list);
            }
            catch (Exception e)
            {
                ErrorResponse error = new ErrorResponse(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
