using SajberSim.Helper;
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
    public DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();
    public string applicationId;
    public string optionalSteamId;
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
        Debug.Log("RPC: responding yes to Ask to Join request");
        DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.Yes);
        hasResponded.Invoke();
    }

    public void RequestRespondNo()
    {
        Debug.Log("RPC: responding no to Ask to Join request");
        DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.No);
        hasResponded.Invoke();
    }

    public void ReadyCallback(ref DiscordRpc.DiscordUser connectedUser)
    {
        Debug.Log(string.Format("RPC: connected to {0}", connectedUser.userId));
        onConnect.Invoke();
    }

    public void DisconnectedCallback(int errorCode, string message)
    {
        Debug.Log(string.Format("RPC: disconnect {0}: {1}", errorCode, message));
        onDisconnect.Invoke();
    }

    public void ErrorCallback(int errorCode, string message)
    {
        Debug.Log(string.Format("RPC: error {0}: {1}", errorCode, message));
    }

    public void JoinCallback(string secret)
    {
        Debug.Log(string.Format("RPC: join ({0})", secret));
        onJoin.Invoke(secret);
    }

    public void SpectateCallback(string secret)
    {
        Debug.Log(string.Format("RPC: spectate ({0})", secret));
        onSpectate.Invoke(secret);
    }

    public void RequestCallback(ref DiscordRpc.DiscordUser request)
    {
        Debug.Log(string.Format("RPC: join request {0}#{1}: {2}", request.username, request.discriminator, request.userId));
        joinRequest = request;
        onJoinRequest.Invoke(request);
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        presence.largeImageText = Helper.UsernameCache();
    }

    void Update()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "menu":

                if (StartStory.creatingStory)
                {
                    presence.details = "Editing a story";
                    presence.state = "" + CreateStory.currentlyEditingName;
                }
                else if (StartStory.storymenuOpen)
                {
                    presence.details = "Looking for a story";
                    presence.state = "";
                }
                else
                {
                    presence.details = "In the main menu";
                    presence.state = "";
                }
                break;
            case "game":
                presence.details = $"Playing \"{GameManager.storyName}\"";
                presence.state = $"Published by {GameManager.storyAuthor}";
                break;
            case "credits":
                presence.details = "Watching credits";
                presence.state = $"\"{GameManager.storyName}\" by {GameManager.storyAuthor}";
                break;
            case "characterpos":
                presence.details = "Setting up characters";
                presence.state = "Novel: " + CreateStory.currentlyEditingName;
                break;
            default:
                presence.details = "Unknown state";
                presence.state = "";
                break;
        }
        presence.largeImageKey = "rpc_logo_2";
        DiscordRpc.UpdatePresence(presence);
        DiscordRpc.RunCallbacks();
    }

    void OnEnable()
    {
        Debug.Log("RPC: init");
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
        Debug.Log("RPC: shutdown");
        DiscordRpc.Shutdown();
    }

    void OnDestroy()
    {

    }
}