using UnityEngine;

public class SceneGenerator: MonoBehaviour
{
    [Header("Pyramid Settings (3 - 10)")]
    [Range(3, 10)]
    public int pyramidBaseSize = 6;

    [Header("Forest Settings")]
    public int numberOfTrees = 25;
    public float forestRadius = 40f;
    public float treeSpacing = 3f;

    [Header("Celestial Settings")]
    public float rotationSpeed = 20f;
    public float dayNightDuration = 10f;

    private GameObject sceneRoot;
    private GameObject celestialBody;
    private Light sun;
    private float timer;

    void Start()
    {
        CreateHierarchyRoot();
        CreateGround();
        CreatePyramid();
        CreateForest();
        CreateCelestialSystem();
    }

    void Update()
    {
        HandleCelestialRotation();
        HandleDayNightCycle();
    }

    void CreateHierarchyRoot()
    {
        sceneRoot = new GameObject("SceneRoot");
    }

    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.parent = sceneRoot.transform;
        ground.transform.localScale = new Vector3(10, 1, 10);
    }

    void CreatePyramid()
    {
        pyramidBaseSize = Mathf.Clamp(pyramidBaseSize, 3, 10);

        GameObject pyramidParent = new GameObject("Pyramid");
        pyramidParent.transform.parent = sceneRoot.transform;

        for (int level = 0; level < pyramidBaseSize; level++)
        {
            int size = pyramidBaseSize - level;

            GameObject levelParent = new GameObject("Level_" + level);
            levelParent.transform.parent = pyramidParent.transform;

            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = levelParent.transform;

                    cube.transform.position = new Vector3(
                        x - size / 2f,
                        level + 0.5f,
                        z - size / 2f
                    );

                    Renderer renderer = cube.GetComponent<Renderer>();
                    renderer.material = new Material(Shader.Find("Standard"));
                    renderer.material.color = Color.HSVToRGB((float)level / pyramidBaseSize, 0.8f, 0.9f);
                }
            }
        }
    }

    void CreateForest()
    {
        GameObject forestParent = new GameObject("Forest");
        forestParent.transform.parent = sceneRoot.transform;

        int treesPlaced = 0;
        int attempts = 0;

        while (treesPlaced < numberOfTrees && attempts < numberOfTrees * 10)
        {
            attempts++;

            Vector3 randomPos = new Vector3(
                Random.Range(-forestRadius, forestRadius),
                0,
                Random.Range(-forestRadius, forestRadius)
            );

            if (Vector3.Distance(randomPos, Vector3.zero) < pyramidBaseSize + 5)
                continue;

            bool tooClose = false;
            foreach (Transform child in forestParent.transform)
            {
                if (Vector3.Distance(child.position, randomPos) < treeSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose) continue;

            GameObject tree = CreateTree();
            tree.name = "Tree_" + treesPlaced;
            tree.transform.position = randomPos;
            tree.transform.parent = forestParent.transform;

            treesPlaced++;
        }
    }

    GameObject CreateTree()
    {
        GameObject tree = new GameObject("Tree");

        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.parent = tree.transform;
        trunk.transform.localPosition = new Vector3(0, 1, 0);
        trunk.transform.localScale = new Vector3(0.5f, 1, 0.5f);
        trunk.GetComponent<Renderer>().material.color = new Color(0.4f, 0.2f, 0.1f);

        GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.transform.parent = tree.transform;
        leaves.transform.localPosition = new Vector3(0, 2.5f, 0);
        leaves.transform.localScale = new Vector3(2, 2, 2);
        leaves.GetComponent<Renderer>().material.color = Color.green;

        return tree;
    }

    void CreateCelestialSystem()
    {
        GameObject celestialSystem = new GameObject("CelestialSystem");
        celestialSystem.transform.parent = sceneRoot.transform;

        celestialBody = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        celestialBody.name = "CelestialBody";
        celestialBody.transform.parent = celestialSystem.transform;
        celestialBody.transform.position = new Vector3(0, 30, 0);
        celestialBody.transform.localScale = Vector3.one * 5;

        sun = FindObjectOfType<Light>();
    }

    void HandleCelestialRotation()
    {
        if (celestialBody != null)
        {
            celestialBody.transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
    }

    void HandleDayNightCycle()
    {
        if (sun == null) return;

        timer += Time.deltaTime;
        float cycle = Mathf.PingPong(timer / dayNightDuration, 1);

        sun.intensity = Mathf.Lerp(0.2f, 1.2f, cycle);
        RenderSettings.ambientLight = Color.Lerp(Color.black, Color.white, cycle);
    }
}