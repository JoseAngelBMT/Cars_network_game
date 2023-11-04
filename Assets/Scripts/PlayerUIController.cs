using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerUIController : MonoBehaviourPun
{

    public Texture texture;

    private int position;
    private int laps;
    private int maxLaps;
    private float turbo;
    private float maxTurbo;

    private CarController script;
    // Start is called before the first frame update
    void Start()
    {
        script = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        maxLaps = script.maxLaps;
        laps = script.getLap();
        position = script.getPosition();
        turbo = script.turbo;
        maxTurbo = script.maxTurbo;
    }

    // Interface of this player
    void OnGUI()
    {
        if (photonView.IsMine)
        {
            Font myFont = (Font)Resources.Load("Fonts/ShoottoKill", typeof(Font));

            GUIStyle turboStyle = new GUIStyle();
            turboStyle.normal.background = TurboTexture(2, 2, new Color(0f, 1f, 0f, 0.5f));
            float percentageTurbo = (turbo / maxTurbo) * 100;
            GUI.Box(new Rect(10, Screen.height - 160, 50, percentageTurbo * 1.5f), "", turboStyle); // Print the turbo
            string labelText = percentageTurbo.ToString("0");

            GUIStyle numberStyle = new GUIStyle();
            numberStyle.fontSize = 25;
            numberStyle.normal.textColor = Color.black;
            numberStyle.font = myFont;
            GUI.Label(new Rect(10, Screen.height - 200, 200, 175), labelText + " %", numberStyle); // Shows the percentage of boost

            //Turbo GUI           
            //GUI.Box(new Rect(10, Screen.height-160, 50, 150), ""); // Print the box
            GUI.DrawTexture(new Rect(-42, Screen.height - 215, 150, 240), texture);
            GUI.backgroundColor = Color.white;

            //Laps GUI      
            GUIStyle lapsStyle1 = new GUIStyle();
            lapsStyle1.fontSize = 40;
            lapsStyle1.normal.textColor = Color.blue;
            lapsStyle1.font = myFont;
            GUIStyle lapsStyle2 = new GUIStyle();
            lapsStyle2.fontSize = 25;
            lapsStyle2.normal.textColor = Color.blue;
            lapsStyle2.font = myFont;
            GUI.Label(new Rect(10, 10, 100, 100), "Lap ", lapsStyle2);
            GUI.Label(new Rect(110, 15, 100, 100), laps + "/" + maxLaps, lapsStyle1);

            // Position GUI

            GUIStyle positionStyle = new GUIStyle();
            positionStyle.fontSize = 100;
            if (position == 1)
            {
                positionStyle.normal.textColor = new Color(1f, 0.8432f, 0f, 1f); // Gold
            }
            else if (position == 2)
            {
                positionStyle.normal.textColor = new Color(0.753f, 0.753f, 0.753f, 1f); // Silver
            }
            else if (position == 3)
            {
                positionStyle.normal.textColor = new Color(0.804f, 0.498f, 0.196f, 1f); // Bronze
            }
            else
            {
                positionStyle.normal.textColor = Color.white;
            }
            positionStyle.font = myFont;
            GUI.Label(new Rect(Screen.width - 100, 0, 100, 100), "" + position, positionStyle);
        }
    }

    private Texture2D TurboTexture(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
