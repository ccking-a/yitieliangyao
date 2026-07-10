[System.Serializable]
public class Prescription
{
    public HerbData jun;    // 君药（固定）
    public HerbData chen;   // 臣药
    public HerbData zuo;    // 佐药
    public HerbData shi;    // 使药

    public int CalculateTotalScore()
    {
        int score = 100;  // 基础分

        // 角色基础分
        score += jun.baseScore * 3;
        score += chen.baseScore * 2;
        score += zuo.baseScore * 1;
        score += shi.baseScore * 1;

        // 配伍加分
        score += GetInteractionBonus(jun, chen);
        score += GetInteractionBonus(jun, zuo);
        score += GetInteractionBonus(jun, shi);

        return score;
    }

    private int GetInteractionBonus(HerbData a, HerbData b)
    {
        var interaction = a.interactions.Find(i => i.targetHerbId == b.herbId);
        if (interaction != null)
        {
            if (interaction.type == InteractionType.相须) return 15;
            if (interaction.type == InteractionType.相使) return 10;
            if (interaction.type == InteractionType.相恶) return -10;
            if (interaction.type == InteractionType.相反) return -20;
        }
        return 0;
    }
}