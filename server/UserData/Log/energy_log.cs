namespace Nanina.UserData.Log;

public class EnergyLog(float energy, float max_energy, float energy_spent, float gacha_currency)
{
    public float energy = energy;
    public float max_energy = max_energy;
    public float energy_spent = energy_spent;
    public float gacha_currency = gacha_currency;
    public long timestamp { get; set; } = Utils.GetTimestamp();

}