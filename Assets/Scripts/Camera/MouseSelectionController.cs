using System;
using System.Collections.Generic;
using UnityEngine;

class MouseSelectionController : MonoBehaviour
{
    public Camera Camera;
    public LayerMask OverLapLayerMask;

    private bool isSelecting;
    private Vector3 initialMousePos;
    private SelectionBox selectionBox;
    private Rect currentSelectionRect;
    private List<Unit> selectedUnits;

    private Vector3 currentCenter;
    private Vector3 currentScale;

    void Start()
    {
        selectionBox = gameObject.AddComponent<SelectionBox>();
        selectedUnits = new List<Unit>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnLeftMouseDown();
        if (Input.GetMouseButtonUp(0))
            OnLeftMouseUp();
        if (Input.GetMouseButtonUp(1))
            OnRightMouseUp();
    }

    private void OnRightMouseUp()
    {
        var ray = Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit))
            return;
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            Unit unit = selectedUnits[i];
            unit.SetDestination(hit.point, true);
        }
    }

    private void OnLeftMouseUp()
    {
        isSelecting = false;

        var ray1 = Camera.ScreenPointToRay(initialMousePos);
        var ray2 = Camera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit1;
        RaycastHit hit2;
        Vector3 v1 = Vector3.zero;
        Vector3 v2 = Vector3.zero;

        if (Physics.Raycast(ray1, out hit1))
            v1 = hit1.point;
        if (Physics.Raycast(ray2, out hit2))
            v2 = hit2.point;

        if (v1 == Vector3.zero && v2 == Vector3.zero)
            return;
        var center = Vector3.Lerp(v1, v2, 0.5f);
        currentCenter = center;
        currentScale = new Vector3(Math.Abs(v2.x - v1.x), 10f, Math.Abs(v2.z - v1.z));
        Collider[] overlappingColliders = Physics.OverlapBox(center, currentScale * 0.5f, Quaternion.identity, OverLapLayerMask);

        for (int i = 0; i < overlappingColliders.Length; i++)
        {
            var collider = overlappingColliders[i];
            if (collider.isTrigger)
                continue;
            Unit unit = collider.GetComponent<Unit>();
            if (unit == null)
                continue;
            unit.SetSelection(true);
            selectedUnits.Add(unit);
            unit.OnUnitDead += OnUnitDead;
        }
    }

    private void OnUnitDead(Unit unit)
    {
        selectedUnits.Remove(unit);
    }

    private void OnLeftMouseDown()
    {
        initialMousePos = Input.mousePosition;
        isSelecting = true;
        for (int i = 0; i < selectedUnits.Count; i++)
            selectedUnits[i].SetSelection(false);
        selectedUnits.Clear();
    }

    void OnGUI()
    {
        if (!isSelecting)
            return;
        currentSelectionRect = selectionBox.GetScreenRect(initialMousePos, Input.mousePosition);
        selectionBox.DrawScreenRect(currentSelectionRect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
        selectionBox.DrawScreenRectBorder(currentSelectionRect, 2, new Color(0.8f, 0.8f, 0.95f));
    }
}
