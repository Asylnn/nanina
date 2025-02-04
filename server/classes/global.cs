using System.Net.Http;
using RestSharp;
public static class Global {
    public static Config config;
}

public class Config {
    public long time_for_allowing_another_fight_in_milliseconds;
    public long time_limit_for_osu_code_verification_in_milliseconds;
    public long time_for_allowing_another_claim_in_milliseconds;
    public bool first_time_running;
}