using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DiscordJoinEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordSpectateEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordJoinRequestEvent : UnityEngine.Events.UnityEvent<DiscordRpc.DiscordUser> { }

public class DiscordController : MonoBehaviour
{
    public static DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();
    public string applicationId;
    public string optionalSteamId;
    public int clickCounter;
    public DiscordRpc.DiscordUser joinRequest;
    public UnityEngine.Events.UnityEvent onConnect;
    public UnityEngine.Events.UnityEvent onDisconnect;
    public UnityEngine.Events.UnityEvent hasResponded;
    public DiscordJoinEvent onJoin;
    public DiscordJoinEvent onSpectate;
    public DiscordJoinRequestEvent onJoinRequest;

    DiscordRpc.EventHandlers handlers;



    public void RequestRespondYes()
    {
        Debug.Log("Discord: responding yes to Ask to Join request");
        DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.Yes);
        hasResponded.Invoke();
    }

    public void RequestRespondNo()
    {
        Debug.Log("Discord: responding no to Ask to Join request");
        DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.No);
        hasResponded.Invoke();
    }

    public void ReadyCallback(ref DiscordRpc.DiscordUser connectedUser)
    {
        Debug.Log(string.Format("Discord: connected to {0}#{1}: {2}", connectedUser.username, connectedUser.discriminator, connectedUser.userId));
        onConnect.Invoke();
    }

    public void DisconnectedCallback(int errorCode, string message)
    {
        Debug.Log(string.Format("Discord: disconnect {0}: {1}", errorCode, message));
        onDisconnect.Invoke();
    }

    public void ErrorCallback(int errorCode, string message)
    {
        Debug.Log(string.Format("Discord: error {0}: {1}", errorCode, message));
    }

    public void JoinCallback(string secret)
    {
        Debug.Log(string.Format("Discord: join ({0})", secret));
        onJoin.Invoke(secret);
    }

    public void SpectateCallback(string secret)
    {
        Debug.Log(string.Format("Discord: spectate ({0})", secret));
        onSpectate.Invoke(secret);
    }

    public void RequestCallback(ref DiscordRpc.DiscordUser request)
    {
        Debug.Log(string.Format("Discord: join request {0}#{1}: {2}", request.username, request.discriminator, request.userId));
        joinRequest = request;
        onJoinRequest.Invoke(request);
    }

    void Start()
    {
    }

    void Update()
    {
            DiscordRpc.RunCallbacks();
            DiscordRpc.UpdatePresence(presence);
        presence.largeImageKey = "mainwfade";
        if (SceneManager.GetActiveScene().name == "game") presence.details = "Ingame";
        else if (SceneManager.GetActiveScene().name == "menu") presence.details = "In main menu";
        else if (SceneManager.GetActiveScene().name == "credits") presence.details = "Watching credits";

    }

    void OnEnable()
    {
        Debug.Log("Discord: init");
        handlers = new DiscordRpc.EventHandlers();
        handlers.readyCallback += ReadyCallback;
        handlers.disconnectedCallback += DisconnectedCallback;
        handlers.errorCallback += ErrorCallback;
        handlers.joinCallback += JoinCallback;
        handlers.spectateCallback += SpectateCallback;
        handlers.requestCallback += RequestCallback;
        DiscordRpc.Initialize(applicationId, ref handlers, true, optionalSteamId);
    }

    void OnDisable()
    {
        Debug.Log("Discord: shutdown");
        DiscordRpc.Shutdown();
    }

    void OnDestroy()
    {

    }
}
