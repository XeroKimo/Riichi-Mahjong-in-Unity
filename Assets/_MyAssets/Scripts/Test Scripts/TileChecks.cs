using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChecks : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Tile tile = new Tile(Tile.Suit.Character, Tile.Face.One);

        Debug.Assert(tile.face == Tile.Face.One, "Expects " + Tile.Face.One + " Got " + tile.face);
        Debug.Assert(tile.suit == Tile.Suit.Character);

        tile = new Tile(Tile.Suit.Honor, Tile.Face.East);
        Debug.Assert(tile.face == Tile.Face.East, "Expects " + Tile.Face.East + " Got " + tile.face);
        Debug.Assert(tile.suit == Tile.Suit.Honor);
        Debug.Assert((3 << 6) == 0b11000000, "Expects " + 0b11000000 + " Got " + (3 << 6));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
