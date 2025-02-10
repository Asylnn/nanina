using LiteDB;
using Nanina.Database;

namespace Nanina.Communication {
    public static class Communication {

        public static void UpdateSessionWithUserId(SessionDBEntry session, string userId) 
        {
            session.hasUserAssociatedWithSession = true;
            session.userId = userId;
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var sessionCol = db.GetCollection<SessionDBEntry>("sessiondb");
            sessionCol.Update(session);
        }

        public static SessionDBEntry CreateNewSession()
        {
            var sessionId = Guid.NewGuid().ToString();
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var sessionCol = db.GetCollection<SessionDBEntry>("sessiondb");
            Console.WriteLine("Entering new session ID into database! id : " + sessionId);
            var session = new SessionDBEntry {
                userId = null,
                hasUserAssociatedWithSession = false,
                id = sessionId,
                date = Utils.GetTimestamp(),
                locale = Global.config.default_locale,
            };
            sessionCol.Insert(session);
            sessionCol.EnsureIndex(x => x.id);
            
            
            return session;
        }

        public static SessionDBEntry GetSession(string sessionId){
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var sessionCol = db.GetCollection<SessionDBEntry>("sessiondb");
                return sessionCol.Find(x => x.id == sessionId).First();
            }
        }
    }   
}
