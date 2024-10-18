using LiteDB;
public static class Communication {

    public static ServerWebSocketResponse UpdateSessionId(string userId = "", bool hasUserAssociatedWithSession = false) {
        var sessionId = Guid.NewGuid().ToString();
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var session_col = db.GetCollection<SessionDBEntry>("sessiondb");
            Console.WriteLine("Entering new session ID into database! id : " + sessionId);
            session_col.Insert(new SessionDBEntry {
                userId = userId,
                hasUserAssociatedWithSession = hasUserAssociatedWithSession,
                sessionId = sessionId,
                date = long.Parse(Utils.GetTimestamp())
            });
            session_col.EnsureIndex(x => x.sessionId);
        }
        return new ServerWebSocketResponse {
            type = "session_id",
            data = sessionId
        };
    }
}