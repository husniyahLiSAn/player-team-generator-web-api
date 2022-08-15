namespace WebApi.Helpers
{
    public class ValidationData
    {
        public ValidationData(string position, string skill)
        {
            if (position != null)
            {
                if (position.ToLower().Equals("defender"))
                {
                    Position = position;
                }
                else if (position.ToLower().Equals("midfielder"))
                {
                    Position = position;
                }
                else if (position.ToLower().Equals("forward"))
                {
                    Position = position;
                }
            }
            else
                Position = null;

            if (skill != null)
            {
                if (skill.ToLower().Equals("defense"))
                {
                    Skill = skill;
                }
                else if (skill.ToLower().Equals("attack"))
                {
                    Skill = skill;
                }
                else if (skill.ToLower().Equals("speed"))
                {
                    Skill = skill;
                }
                else if (skill.ToLower().Equals("strength"))
                {
                    Skill = skill;
                }
                else if (skill.ToLower().Equals("stamina"))
                {
                    Skill = skill;
                }
            }
            else
                Skill = null;
        }

        public string Position { get; set; }
        public string Skill { get; set; }
    }
}
