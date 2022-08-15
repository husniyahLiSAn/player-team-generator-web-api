// /////////////////////////////////////////////////////////////////////////////
// PLEASE DO NOT RENAME OR REMOVE ANY OF THE CODE BELOW. 
// YOU CAN ADD YOUR CODE TO THIS FILE TO EXTEND THE FEATURES TO USE THEM IN YOUR WORK.
// /////////////////////////////////////////////////////////////////////////////


using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities;

public class Player
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public List<PlayerSkill> PlayerSkills { get; set; }
}

public class ListofPlayer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public List<PlayerSkills> PlayerSkills { get; set; }
}

public class PlayerVM
{
    public string Name { get; set; }
    public string Position { get; set; }
    public PlayerSkillVM PlayerSkills { get; set; }
}

public class TeamProcessVM
{
    [Required]
    public string Position { get; set; }
    [Required]
    public string MainSkill { get; set; }
    [Required]
    public int NumberOfPlayers { get; set; }
}