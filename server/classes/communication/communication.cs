using LiteDB;
using WebSocketSharp.Server;

public static class Communication {

    public static void UpdateSessionWithUserId(SessionDBEntry session, string userId) {
        session.hasUserAssociatedWithSession = true;
        session.userId = userId;
        using var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}");
        var sessionCol = db.GetCollection<SessionDBEntry>("sessiondb");
        sessionCol.Update(session);
    }

    public static void UpdateSessionLanguage(string sessionId, string lang) {
        
    }

    

    public static SessionDBEntry GetSession(string sessionId){
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var sessionCol = db.GetCollection<SessionDBEntry>("sessiondb");
            return sessionCol.Find(x => x.id == sessionId).First();
        }
    }

    
}