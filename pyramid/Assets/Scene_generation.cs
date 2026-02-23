using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class Scene_generation : MonoBehaviour
{
    [Range(3, 10)]
    public int pyramidScale = 5;

    public int trees = 15;
    public float forestScale = 30f;
    public float treeSpace = 3f;

    public float rotationSpeed = 10f;
    public float dayNightRotation = 5f;

    private GameObject sceneRoot;
    private GameObject celestialObject;
    private Light sun;
    private float timer;


    private void Start()
    {
        CreateHirearchyRoot();
        CreateGround();
        CreatePyramid();
        CreateForest();
        CreateCelestialSystem();

    }

    private void Update()
    {
        HandleCelestialRotaion();
        HandleDayNightCycle();
    }

    void CreateHirearchyRoot()
    {
        sceneRoot = new GameObject("SceneRoot");
    }
    
    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.parent = sceneRoot.transform;
        ground.transform.localScale = new Vector3(10, 1, 10);   
        ground.GetComponent<Renderer>().material.color = Color.green;
    }


    void CreatePyramid()
    {
        pyramidScale = Mathf.Clamp(pyramidScale, 3, 10);

        GameObject pyramidParent = new GameObject("Pyramid");
        pyramidParent.transform.parent = sceneRoot.transform;

        for (int level = 0; level < pyramidScale; level++)
        {
            int size = pyramidScale - level;

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
                    renderer.material.color = Color.HSVToRGB((float)level / pyramidScale, 0.8f, 0.9f);
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

        while (treesPlaced < trees && attempts < trees * 10)
        {
            attempts++;

            Vector3 randomPos = new Vector3(
                Random.Range(-forestScale, forestScale),
                0,
                Random.Range(-forestScale, forestScale)
                );

            if (Vector3.Distance(randomPos, Vector3.zero) < pyramidScale + 5)
                continue;

            bool tooClose = false;
            foreach (Transform child in forestParent.transform)
            {
                if (Vector3.Distance(child.position, randomPos) < treeSpace)
                {
                    tooClose = true; break;
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
            GameObject celestialSystem = new GameObject("Celestial System");
            celestialSystem.transform.parent = sceneRoot.transform;

            celestialObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            celestialObject.name = "Celestial Body";
            celestialObject.transform.parent = celestialSystem.transform;
            celestialObject.transform.position = new Vector3(0, 30, 0);
            celestialObject.transform.localScale = Vector3.one * 5;

            sun = FindObjectOfType<Light>();
    }

        void HandleCelestialRotaion()
        {
            if (celestialObject != null)
            {
                celestialObject.transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);   
            }
        }

        void HandleDayNightCycle()
        {
            if (sun == null) return;
            timer += Time.deltaTime;
            float cycle = Mathf.PingPong(timer / dayNightRotation, 1);

            sun.intensity = Mathf.Lerp(0.2f, 1.2f, cycle);
            RenderSettings.ambientLight = Color.Lerp(Color.black, Color.white, cycle);

        }


}
