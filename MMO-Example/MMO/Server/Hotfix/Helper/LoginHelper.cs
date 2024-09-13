using Fantasy;
using Fantasy.Network;

namespace BestGame;

public static class LoginHelper
{
    public static bool CheckSessionValid(Session session, long sessionRuntimeId)
    {
        if (session.RunTimeId != sessionRuntimeId)
        {
            return false;
        }

        return true;
    }

    public static int CheckSessionBindAccount(Session session)
    {
        if (session == null)
        {
            return ErrorCode.SessionNotBindAccount;
        }

        var sessionPlayer = session.GetComponent<SessionPlayerComponent>();

        if (sessionPlayer == null)
        {
            return ErrorCode.SessionNotBindAccount;
        }

        return ErrorCode.Success;
    }
}