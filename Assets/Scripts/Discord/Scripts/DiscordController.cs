using SajberSim.Helper;
using Steamworks;
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
        presence.largeImageKey = "rpc_logo_2";
    }

    void Update()
    {
        string details;
        string state;
        string steamkey;
        string steamarg = " ";
        switch (SceneManager.GetActiveScene().name)
        {
            case "menu":

                if (StartStory.creatingStory)
                {
                    steamkey = "#EditNovel";
                    steamarg = CreateStory.editName;
                    details = "Editing a story";
                    state = "" + CreateStory.editName;
                }
                else if (StartStory.storymenuOpen)
                {
                    steamkey = "#LookingForStory";
                    details = "Looking for a story";
                    state = "";
                }
                else
                {
                    steamkey = "#InMain";
                    details = "In the main menu";
                    state = "";
                }
                break;
            case "game":
                steamkey = "#PlayingNovel";
                steamarg = Helper.currentStoryName;
                details = $"Playing \"{GameManager.storyName}\"";
                state = $"Published by {GameManager.storyAuthor}";
                break;
            case "credits":
                steamkey = "#InCredits";
                details = "Watching credits";
                state = $"\"{GameManager.storyName}\" by {GameManager.storyAuthor}";
                break;
            case "characterpos":
                steamkey = "#InCharacters";
                details = "Setting up characters";
                state = "Novel: " + CreateStory.editName;
                break;
            default:
                steamarg = " ";
                steamkey = "#Unknown";
                details = "Unknown state";
                state = "";
                break;
        }
        if (presence.details != details || presence.state != state)
        {
            presence.details = details;
            presence.state = state;
            if (Helper.loggedin)
            {
                SteamFriends.SetRichPresence("status", details);
                SteamFriends.SetRichPresence("steam_display", steamkey);
                if (steamkey == "#PlayingNovel" || steamkey == "#EditNovel") SteamFriends.SetRichPresence("novel", steamarg);
            }

            DiscordRpc.UpdatePresence(presence);
            DiscordRpc.RunCallbacks();
        }
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