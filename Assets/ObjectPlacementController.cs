using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectPlacementController : MonoBehaviour
{
    public GameObject[] objectPrefabs;
    public LayerMask substrateLayer;
    public Camera placementCamera;
    public Material collidedMaterial;
    public Material hoverMaterial;
    public float rotationSpeed = 100f;
    public float scaleSpeed = 1f;
    public float minScale = 0.1f;
    public float maxScale = 10f;

    public GameObject shopPanel;
    public List<Button> plantButtons = new List<Button>();
    public Collider[] tankColliders;

    private GameObject spawnedObject;
    private bool isObjectSelected;
    private bool isLocked;
    private bool canInteract;
    private Renderer objectRenderer;
    private int selectedPrefabIndex = 0;
    private Material originalMaterial;
    private bool isObjectPlaced = false;
    private bool hasCollided = false;

    private Dictionary<string, Fish> spawnedFishStats = new Dictionary<string, Fish>();
    private Dictionary<string, Plant> spawnedPlantStats = new Dictionary<string, Plant>();

    public int SelectedPrefabIndex
    {
        get { return selectedPrefabIndex; }
        set { selectedPrefabIndex = value; }
    }

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        foreach (Button plantButton in plantButtons)
        {
            plantButton.onClick.AddListener(OnPlantButtonClick);
        }
    }

    private void Update()
    {
        if (isObjectSelected && canInteract && !isLocked)
        {
            if (spawnedObject != null)
            {
                Ray ray = placementCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, substrateLayer))
                {
                    Vector3 targetPosition = hitInfo.point;
                    spawnedObject.transform.position = targetPosition;
                }

                float rotationY = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
                spawnedObject.transform.Rotate(Vector3.up, rotationY);

                float rotationX = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;
                spawnedObject.transform.Rotate(Vector3.right, rotationX);

                float scale = 0f;
                if (Input.GetKey(KeyCode.Q))
                    scale = -scaleSpeed * Time.deltaTime;
                else if (Input.GetKey(KeyCode.E))
                    scale = scaleSpeed * Time.deltaTime;

                Vector3 newScale = spawnedObject.transform.localScale + new Vector3(scale, scale, scale);
                newScale = Vector3.ClampMagnitude(newScale, maxScale);
                newScale = Vector3.Max(newScale, new Vector3(minScale, minScale, minScale));
                spawnedObject.transform.localScale = newScale;

                if (Input.GetMouseButtonDown(0))
                {
                    if (!isLocked)
                    {
                        isLocked = true;
                        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.isKinematic = true;
                        }
                    }
                    canInteract = false;
                    isObjectPlaced = true;
                    if (objectRenderer != null)
                    {
                        objectRenderer.material = originalMaterial;
                    }

                    if (hasCollidedWithTank())
                    {
                        canInteract = false;
                    }
                    string uniqueName = "Item_" + System.Guid.NewGuid().ToString();
                    if (selectedPrefabIndex >= 0 && selectedPrefabIndex < objectPrefabs.Length)
                    {
                        GameObject prefab = objectPrefabs[selectedPrefabIndex];
                        Fish fishStats = FindObjectOfType<JSONLoader>().GetFishDataByName(prefab.name);
                        if (fishStats != null)
                        {
                            FindObjectOfType<JSONLoader>().spawnedFishStats[uniqueName] = fishStats;
                        }
                        Plant plantStats = FindObjectOfType<JSONLoader>().GetPlantDataByName(prefab.name);
                        if (plantStats != null)
                        {
                            FindObjectOfType<JSONLoader>().spawnedPlantStats[uniqueName] = plantStats;
                        }
                    }

                }

            }
        }
        else
        {
            isObjectSelected = false;
        }

        if (isObjectPlaced && spawnedObject != null && spawnedObject.layer == LayerMask.NameToLayer("Selectable") && canInteract)
        {
            if (hasCollided)
            {
                if (objectRenderer != null && collidedMaterial != null)
                {
                    objectRenderer.material = collidedMaterial;
                }
                canInteract = false;

                Debug.Log("Object collided with the tank.");
            }
            else
            {
                if (objectRenderer != null && hoverMaterial != null)
                {
                    objectRenderer.material = hoverMaterial;
                }
                canInteract = true;
            }
        }
    }

    public void OnPlantButtonClick()
    {
        int buttonIndex = plantButtons.IndexOf(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        if (buttonIndex >= 0 && buttonIndex < plantButtons.Count)
        {
            selectedPrefabIndex = buttonIndex;
            SpawnObject();
        }

        shopPanel.SetActive(false);
    }

    public void SpawnObject()
    {
        if (selectedPrefabIndex < 0 || selectedPrefabIndex >= objectPrefabs.Length)
            return;

        GameObject prefab = objectPrefabs[selectedPrefabIndex];
        prefab.transform.localScale = Vector3.one;

        Ray ray = placementCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
        RaycastHit hitInfo;
        Vector3 spawnPosition = Vector3.zero;

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, substrateLayer))
        {
            spawnPosition = hitInfo.point;
        }

        Quaternion spawnRotation = Quaternion.identity;

        GameObject newObject = Instantiate(prefab, spawnPosition, spawnRotation);
        newObject.transform.localScale = Vector3.one;

        spawnedObject = newObject;

        isObjectSelected = true;
        isLocked = false;
        canInteract = true;
        isObjectPlaced = false;
        hasCollided = false;

        objectRenderer = spawnedObject.GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
            objectRenderer.material = hoverMaterial;
        }

        shopPanel.SetActive(false);
    }

    private bool hasCollidedWithTank()
    {
        Collider[] colliders = spawnedObject.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            foreach (var tankCollider in tankColliders)
            {
                if (collider == tankCollider)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
