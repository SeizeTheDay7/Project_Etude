// using UnityEngine;

// public class MapEditor : MonoBehaviour
// {
//     public GameObject blockPF;
//     public GameObject dottedLinePF;

//     public void CreateNextBlock(GameObject currentBlock)
//     {
//         // 현재 블록의 NoteBlock 컴포넌트 가져오기
//         NoteBlock currentNote = currentBlock.GetComponent<NoteBlock>();

//         // 음표 길이에 따른 점선 길이
//         float lineLength = GetLineLength(currentNote.noteLength);

//         // 방향 설정
//         Vector3 direction = GetDirection(currentNote.direction);

//         // 현재 블록의 중심 계산
//         SpriteRenderer spriteRenderer = currentBlock.GetComponent<SpriteRenderer>();
//         Vector3 startPosition = spriteRenderer.bounds.center;

//         // 점선 생성 및 배치
//         GameObject dottedLine = Instantiate(dottedLinePF, startPosition, Quaternion.identity);
//         SetDottedLine(dottedLine, lineLength, direction);

//         // 다음 블록 생성 및 배치
//         Vector3 nextBlockPosition = startPosition + direction.normalized * lineLength;
//         GameObject nextBlock = Instantiate(blockPF, nextBlockPosition, Quaternion.identity);

//         // 새 블록의 방향과 음표 길이를 기본값으로 설정
//         NoteBlock nextNoteBlock = nextBlock.GetComponent<NoteBlock>();
//         nextNoteBlock.noteLength = currentNote.noteLength;
//         nextNoteBlock.direction = currentNote.direction;
//     }

//     private float GetLineLength(NoteBlock.NoteLength length)
//     {
//         switch (length)
//         {
//             case NoteBlock.NoteLength.Whole: return 2f;
//             case NoteBlock.NoteLength.Half: return 1f;
//             case NoteBlock.NoteLength.Quarter: return 0.5f;
//             case NoteBlock.NoteLength.Eighth: return 0.25f;
//             default: return 0.5f;
//         }
//     }

//     private Vector3 GetDirection(NoteBlock.Direction direction)
//     {
//         switch (direction)
//         {
//             case NoteBlock.Direction.Right: return Vector3.right;
//             case NoteBlock.Direction.Up: return Vector3.up;
//             case NoteBlock.Direction.Left: return Vector3.left;
//             case NoteBlock.Direction.Down: return Vector3.down;
//             default: return Vector3.right;
//         }
//     }

//     private void SetDottedLine(GameObject dottedLine, float length, Vector3 direction)
//     {
//         // 점선 길이 조정
//         SpriteRenderer lineRenderer = dottedLine.GetComponent<SpriteRenderer>();
//         Vector3 scale = lineRenderer.transform.localScale;
//         scale.x = length;
//         lineRenderer.transform.localScale = scale;

//         // 점선 방향 조정
//         float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
//         dottedLine.transform.rotation = Quaternion.Euler(0, 0, angle);
//     }
// }