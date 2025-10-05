namespace Nanina.UserData.Log;

public class EnergyLog(double energy, double max_energy, double energy_spent, double gacha_currency)
{
    public double energy = energy;
    public double max_energy = max_energy;
    public double energy_spent = energy_spent;
    public double gacha_currency = gacha_currency;
    public long timestamp { get; set; } = Utils.GetTimestamp();

}