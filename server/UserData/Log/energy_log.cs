namespace Nanina.UserData.Log;

public class EnergyLog(double energy, double max_energy, double energy_spent, double gacha_currency)
{
    public double energy { get; set; } = energy;
    public double max_energy { get; set; } = max_energy;
    public double energy_spent { get; set; } = energy_spent;
    public double gacha_currency { get; set; } = gacha_currency;
    public long timestamp { get; set; } = Utils.GetTimestamp();

}