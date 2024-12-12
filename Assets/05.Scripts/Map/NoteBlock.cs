using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBlock : MonoBehaviour
{
    public enum NoteLength
    {
        dottedWhole, // 점온음표, 6박자
        Whole,    // 온음표, 4박자
        dottedHalf, // 점2분음표, 3박자
        Half,     // 2분음표, 2박자
        dottedQuarter, // 점4분음표, 3/2박자
        Quarter,  // 4분음표, 1박자
        dottedEighth, // 점8분음표, 3/4박자
        Eighth,    // 8분음표, 1/2박자
        dottedSixteenth, // 점16분음표, 3/8박자
        Sixteenth, // 16분음표, 1/4박자

    }

    // 방향 드롭다운
    public enum Direction
    {
        Right,
        Up,
        Left,
        Down
    }

    public NoteLength noteLength;
    public Direction direction;
}
