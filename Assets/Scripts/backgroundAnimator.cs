using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundAnimator : MonoBehaviour {
    [System.Serializable]
    public class TextureList
    {
        public Texture2D[] textures;
        public int Length;
    }
    public TextureList[] maps;
    public int selection;
    float framesPerSecond = 10;
    void Update()
    {
        int index = (int)(Time.time * framesPerSecond) % maps[selection].Length;
        GetComponent<Renderer>().material.mainTexture = maps[selection].textures[index];
    }
}
