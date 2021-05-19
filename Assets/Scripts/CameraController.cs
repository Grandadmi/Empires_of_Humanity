using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    //connections
    [SerializeField] private GameManager manager;
    [SerializeField] private HexGrid grid;
    [SerializeField] private Transform swivelTrans;
    [SerializeField] private GameObject selectedCellUI;
    [SerializeField] private GameObject selectedSettlementUI;
    [SerializeField] private TimeControlManager timeManager;
    [SerializeField] private GameObject menuUI;

    //Pulbic Player Prefs Values
    public float cameraMoveSensitivity;
    public float zoomSensitivity;
    public bool isInSettingsMenu;

    //Serialized Private Fields
    [SerializeField] private float movementTime;
    [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private float cameraPanBorderThickness;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float rotSpeed;

    //private Clamp Values
    private float xMin, xMax, zMin, zMax;
    private float zoomMin = 20f, zoomMax = 80f;
    private float zoomRotMin, zoomRotMax;

    //camera aperature positions
    private Vector3 newCamPos;
    private Vector3 newZoom;
    private Quaternion newCamRot;

    //Mouse Drag Vector3
    private Vector3 dragStartPos;
    private Vector3 dragCurrentPos;
    private Vector3 rotStartPos;
    private Vector3 rotCurrentPos;

    //private Map Hits
    private HexCell selectedCell;

    private void Awake()
    {
        newCamPos = transform.position;
        newZoom = swivelTrans.localPosition;
        newCamRot = transform.rotation;
        CalculateClampValues();
    }

    void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
    }

    void HandleMouseInput()
    {
        if (!manager.editMode && manager.allowEdgeScroll && !isInSettingsMenu)
        {

            if (Input.mousePosition.y >= Screen.height - cameraPanBorderThickness)
            {
                newCamPos += (transform.forward * cameraMoveSpeed * cameraMoveSensitivity);
            }
            if (Input.mousePosition.y <= cameraPanBorderThickness)
            {
                newCamPos -= (transform.forward * cameraMoveSpeed * cameraMoveSensitivity);
            }
            if (Input.mousePosition.x >= Screen.width - cameraPanBorderThickness)
            {
                newCamPos += (transform.right * cameraMoveSpeed * cameraMoveSensitivity);
            }
            if (Input.mousePosition.x <= cameraPanBorderThickness)
            {
                newCamPos -= (transform.right * cameraMoveSpeed * cameraMoveSensitivity);
            }
        }
        if (!manager.editMode)
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(inputRay, out hit))
                    {
                        if (selectedCell != null)
                        {
                            HexCell previousCell = selectedCell;
                            previousCell.IsSelected = false;
                        }
                        selectedCell = grid.GetCell(hit.point);
                        selectedCell.IsSelected = false;
                        if (!selectedCell.isSettled)
                        {
                            selectedCellUI.SetActive(true);
                            CellInfoDisplay cellUI = selectedCellUI.GetComponent<CellInfoDisplay>();
                            cellUI.UpdateUIInformation(selectedCell);
                        }
                        //if (selectedCell.isSettled)
                        //{
                        //    SettlementInfoScreen infoScreen = selectedSettlementUI.GetComponent<SettlementInfoScreen>();
                        //    infoScreen.OpenUI(selectedCell.settlement);
                        //}

                        //Debug.Log("Selected Cell" + selectedCell.coordinates);
                    }
                }

            }
            if (Input.GetMouseButtonDown(2))
            {
                rotStartPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(2))
            {
                rotCurrentPos = Input.mousePosition;

                Vector3 difference = rotStartPos - rotCurrentPos;

                rotStartPos = rotCurrentPos;

                newCamRot *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (selectedCell != null)
                {
                    //cell Selection and UI UPDATE
                    selectedCell.IsSelected = false;
                    selectedCellUI.SetActive(false);
                }

                //Handle mouse pan
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float entry;
                if (plane.Raycast(ray, out entry))
                {
                    dragStartPos = ray.GetPoint(entry);
                }
            }
            if (Input.GetMouseButton(1))
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float entry;
                if (plane.Raycast(ray, out entry))
                {
                    dragCurrentPos = ray.GetPoint(entry);

                    newCamPos = transform.position + dragStartPos - dragCurrentPos;
                }
            }
        }
        if (Input.mouseScrollDelta.y != 0 && !isInSettingsMenu)
        {
            newZoom -= Input.mouseScrollDelta.y * new Vector3(0, zoomSpeed, 0);
        }
    }

    void HandleKeyboardInput()
    {

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newCamPos += (transform.forward * cameraMoveSpeed * cameraMoveSensitivity);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newCamPos += (transform.forward * -cameraMoveSpeed * cameraMoveSensitivity);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newCamPos += (transform.right * -cameraMoveSpeed * cameraMoveSensitivity);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newCamPos += (transform.right * cameraMoveSpeed * cameraMoveSensitivity);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            newCamRot.y += rotSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            newCamRot.y -= rotSpeed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Home))
        {
            newCamRot.y = 0;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            newZoom.y -= zoomSpeed * Time.deltaTime * zoomSensitivity;
        }
        if (Input.GetKey(KeyCode.X))
        {
            newZoom.y += zoomSpeed * Time.deltaTime * zoomSensitivity;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            timeManager.Pause();
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus))
        {
            timeManager.IncreaseTimeStep();
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
        {
            timeManager.DecreaseTimeStep();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectedSettlementUI.activeInHierarchy == true)
            {
                selectedSettlementUI.SetActive(false);
            }
            else
            {
                menuUI.SetActive(true);
                Button menubuttion = menuUI.GetComponent<Button>();
                menubuttion.onClick.Invoke();
            }
        }

        //CLAMP CAMERA APATURE
        newCamPos.x = Mathf.Clamp(newCamPos.x, xMin, xMax);
        newCamPos.z = Mathf.Clamp(newCamPos.z, zMin, zMax);
        newZoom.y = Mathf.Clamp(newZoom.y, zoomMin, zoomMax);
        newCamRot.y = Mathf.Clamp(newCamRot.y, -90f, 90f);

        //move the "stick"
        transform.position = Vector3.Lerp(transform.position, newCamPos, Time.deltaTime * movementTime);
        //zoom the "swivel"
        swivelTrans.localPosition = Vector3.Lerp(swivelTrans.localPosition, newZoom, Time.deltaTime * movementTime);
        //rotate the "stick"
        transform.rotation = Quaternion.Lerp(transform.rotation, newCamRot, Time.deltaTime * movementTime);
        //rotate the "swivel" this will the camera rotates as zoom
    }

    void CalculateClampValues()
    {
        xMin = 0f;
        zMin = 0f;
        xMax = (grid.chunkCountX * HexMetrics.chunkSizeX - 0.5f) * (2f * HexMetrics.innerRadius);
        zMax = (grid.chunkCountZ * HexMetrics.chunkSizeZ - 1) * (1.5f * HexMetrics.outerRadius);
    }
}
