using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;

namespace PaulLabs
{
    //this class manages game states, starts/ends/resets the game state.
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance;

        [Range(3, 10)] public int TreeDepthMin = 3;
        [Range(3, 10)] public int TreeDepthMax = 10;

        [Range(1, 100)] public int NodeValMin = 1;
        [Range(1, 100)] public int NodeValMax = 100;

        public int TargetSum;
        public List<Node> SelectedNodeList = new List<Node>();
        public List<Node> NodeList = new List<Node>();
        public List<NodeMB> NodeGOList = new List<NodeMB>();

        public AudioClip VictorySound;
        public GameObject NodeGO;
        public TMP_Text TargetSumText;
        public int nodeLayer;

        AudioSource audioSource;

        private PlayerControls _playerControls;


        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }

        private void Update()
        {
            if (_playerControls.Node.Click.triggered)
            {
                Debug.Log("triggered click!");
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("There is more than one Instance of GameManager.");
            }
            _playerControls = new PlayerControls();
            Instance = this;
        }

        private void Start()
        {
            audioSource = this.GetComponent<AudioSource>();
            _playerControls.Node.Click.performed += SelectNodeClick;
            StartGame();
        }

        private void SelectNodeClick(InputAction.CallbackContext context)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << nodeLayer))
            {
                NodeMB nodeMB = hit.transform.GetComponent<NodeMB>();
                foreach (NodeMB n in NodeGOList)
                {
                    if (n.Node.Depth == nodeMB.Node.Depth)
                    {
                        n.UnSelectNode();
                    }
                }
                SelectedNodeList.RemoveAll(n => n.Depth == nodeMB.Node.Depth);
                SelectedNodeList.Add(nodeMB.Node);
                nodeMB.SelectNode();
            }
        }

        public void StartGame()
        {
            int TreeDepth = Random.Range(TreeDepthMin, TreeDepthMax);
            Node root = CreateNode(TreeDepth, 1);
            PrintPreOrder(root);
            InstantiateNodes(root, new Vector3(0, 0, 0));
            SetTargetSum();

        }

        public Node CreateNode(int depth, int val)
        {
            Node node = new Node(val, depth);
            NodeList.Add(node);

            if (depth == 1)
            {
                return node;
            }
            node.LeftNode = CreateNode(depth - 1, Random.Range(NodeValMin, NodeValMax));
            node.RightNode = CreateNode(depth - 1, Random.Range(NodeValMin, NodeValMax));
            return node;
        }

        public void SetTargetSum()
        {
            int minIdx = 0;
            int maxIdx = 0;
            int power = 0;

            while (maxIdx < NodeList.Count())
            {
                TargetSum += NodeList[Random.Range(minIdx, maxIdx)].NodeVal;
                power++;
                minIdx = maxIdx + 1;
                maxIdx = maxIdx + 1 + (int)(Mathf.Pow(2, power));
            }

            TargetSumText.text = $"TargetSum: {TargetSum}";
        }

        public void InstantiateNodes(Node node, Vector3 spawnLocation)
        {
            if (node != null)
            {
                GameObject nodeGO = Instantiate(NodeGO, spawnLocation, Quaternion.identity);
                nodeGO.GetComponent<NodeMB>().Node = node;
                nodeGO.transform.GetComponentInChildren<TMP_Text>().text = node.NodeVal.ToString(); //move this to nodemb
                NodeGOList.Add(nodeGO.GetComponent<NodeMB>());
                InstantiateNodes(node.LeftNode, spawnLocation + new Vector3(-2.5f, -1.5f, 0)); //todo figure out how to instantiate these split apart
                InstantiateNodes(node.RightNode, spawnLocation + new Vector3(2.5f, -1.5f, 0));
            }
        }

        public void PrintPreOrder(Node node)
        {
            if (node != null)
            {
                Debug.Log($"node val: {node.NodeVal}");
                PrintPreOrder(node.LeftNode);
                PrintPreOrder(node.RightNode);
            }
        }

        //check selected nodes sum to the target
        public void CheckVictory()
        {
            if (SelectedNodeList.Sum(node => node.NodeVal) == TargetSum)
            {
                PlayVictorySound();
                //show success ui
            }
            return;
        }

        public void ResetGame()
        {
            //reset game
        }

        public void PlayVictorySound()
        {
            audioSource.PlayOneShot(VictorySound);
        }

    }

}
