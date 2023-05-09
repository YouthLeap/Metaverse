using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using agora_gaming_rtc;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Runtime.InteropServices;

public class SpatialAudio : MonoBehaviour
{
    public float radious;
    PhotonView view;

    static Dictionary<Player, SpatialAudio> spacialAudioTable = new Dictionary<Player, SpatialAudio>();

    IAudioEffectManager agoraAudioEffect;

    public float gainmultiplier = 100;

    public PlayerController controllar;

    PrivateZone zone;
    private void Awake()
    {
        view = GetComponent<PhotonView>();
        controllar = GetComponent<PlayerController>();
        if (GameManager.IngameScene())
            spacialAudioTable[view.Owner] = this;
    }
    private void Start()
    {
        if (GameManager.IngameScene())
            agoraAudioEffect = NetworkManager.instance.rtcEngine.GetAudioEffectManager();

        zone = GetComponent<PrivateZone>();
    }

    private void Update()
    {
        if (!view.IsMine || !GameManager.IngameScene())
            return;

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.IsLocal)
                continue;

            if (player.CustomProperties.TryGetValue("agoraID", out object agoraID))
            {
                if (spacialAudioTable.TryGetValue(player, out SpatialAudio other))
                {
                    float gain = GetGain(other.transform.position);
                    float pan = GetPan(other.transform.position);
                    //NetworkManager.instance.rtcEngine.AdjustUserPlaybackSignalVolume(uint.Parse((string)agoraID), 100);
                    if (!other.controllar.isblocked && !other.controllar.isotherblocked)
                    {

                        if(controllar.carcont.isactive && other.controllar.carcont.isactive)
                        {
                            NetworkManager.instance.rtcEngine.AdjustUserPlaybackSignalVolume(uint.Parse((string)agoraID), other.controllar.AudioOn ? 100:0);
                        }
                        else
                        {
                            NetworkManager.instance.rtcEngine.AdjustUserPlaybackSignalVolume(uint.Parse((string)agoraID), (int)(other.controllar.AdminAudioOn ? (gainmultiplier * PlayerPrefs.GetFloat("VoiceAudio", 1.0f) * PlayerPrefs.GetFloat("MasterAudio", 1.0f)) : zone.multiplier == 2 ? 0 : (other.controllar.AudioOn ? (other.controllar.Muted ? 0 : gain * PlayerPrefs.GetFloat("VoiceAudio", 1.0f) * PlayerPrefs.GetFloat("MasterAudio", 1.0f)) : 0)));
                        }

                        

                    }
                    else
                    {
                        NetworkManager.instance.rtcEngine.AdjustUserPlaybackSignalVolume(uint.Parse((string)agoraID), 0);
                    }
                    //NetworkManager.instance.rtcEngine.AdjustUserPlaybackSignalVolume(uint.Parse((string)agoraID), 100);
                }
            }
            else
            {
                
            }


        }


    }
    private void OnDestroy()
    {
        if (GameManager.IngameScene())
            spacialAudioTable.Remove(view.Owner);
    }


    float GetGain(Vector3 otherposition)
    {
        float distance = Vector3.Distance(transform.position,otherposition);

        float gain = Mathf.Max(1-(distance/radious),0)* gainmultiplier;
        //Debug.Log("GAin" + gain);
        return gain;
    }
    float GetPan(Vector3 otherposition)
    {
        Vector3 direction = otherposition - transform.position;
        direction.Normalize();
        float dotProduct = Vector3.Dot(transform.right,direction);
        return dotProduct;
    }
}
