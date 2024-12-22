using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ObjectSelector : MonoBehaviour
{
    public GameObject selectedObject; // 현재 선택된 오브젝트
    private GameObject ClickedObject; // 클릭된 오브젝트

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 감지
        if (Input.GetMouseButtonDown(0))
            SelectObject();
    }

    void SelectObject()
    {
        // 카메라에서 마우스 위치로 Ray 발사
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycast로 충돌 검사
        if (Physics.Raycast(ray, out hit))
        {
            ClickedObject = GetRootParent(hit.collider.gameObject);

            // 버튼 누른게 아니라면 일단 선택 해제
            if (!ClickedObject.CompareTag("Button"))
                selectedObject = null;

            // 블럭 클릭했으면 해당 블럭 선택
            if (ClickedObject.CompareTag("Note"))
            {
                selectedObject = ClickedObject;
                Debug.Log("Selected Note : " + selectedObject.name);
            }
        }
    }

    GameObject GetRootParent(GameObject obj)
    {
        while (obj.transform.parent.name != "MapRoot")
        {
            obj = obj.transform.parent.gameObject;
        }
        return obj; // 최상위 부모 반환
    }
}
