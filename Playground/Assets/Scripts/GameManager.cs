using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PaulLabs
{
    //this class manages game states, starts/ends/resets the game state.
    public class GameManager : MonoBehaviour
    {
        //Tunables
        [Range(1, 10)] public int TreeDepthMin = 1;
        [Range(1, 10)] public int TreeDepthMax = 10;

        [Range(1, 100)] public int NodeValMin = 1;
        [Range(1, 100)] public int NodeValMax = 100;

        //Publics
        public Vector3 SpawnLocation = new Vector3(0, 3, 0);

        public int TargetSum = 0;

        public GameObject NodeParent;
        public GameObject NodePrefab;
        public TMP_Text TargetSumText;
        public int NodeLayer;

        public GameObject SuccessPanel;
        public AudioClip VictorySound;
        public GameObject FireWorksFX;

        //Privates
        AudioSource _audioSource;
        PlayerControls _playerControls;

        List<Node> _selectedNodeList = new List<Node>(); //List to track selected nodes.
        List<NodeMB> _nodeGOList = new List<NodeMB>();

        //Statics
        public static GameManager Instance;
        static float moveMultiplier = .25f;

        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Node.Click.performed -= SelectNodeClick;
            _playerControls.Disable();
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
            _audioSource = this.GetComponent<AudioSource>();
            _playerControls.Node.Click.performed += SelectNodeClick;
            StartGame();
        }

        private void Update()
        {
            HandleMovement();
        }

        //WASD movement for camera and mouse scroll to zoom in and out.
        private void HandleMovement()
        {
            Vector2 move = _playerControls.Player.Move.ReadValue<Vector2>();
            Vector2 zoom = _playerControls.Player.Zoom.ReadValue<Vector2>();
            float zoomVal = 1f;
            if (zoom.y == -120f)
            {
                zoomVal = -1f;
                Camera.main.transform.position += new Vector3(0, 0, zoomVal);
            }
            else if (zoom.y == 120f)
            {
                Camera.main.transform.position += new Vector3(0, 0, zoomVal);
            }

            float xMove = move.x * moveMultiplier;
            float yMove = move.y * moveMultiplier;
            Camera.main.transform.position += new Vector3(xMove, yMove, 0);
        }

        private void SelectNodeClick(InputAction.CallbackContext context)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << NodeLayer))
            {
                NodeMB nodeMB = hit.transform.GetComponent<NodeMB>();
                if (nodeMB.Selected)
                {
                    nodeMB.UnSelectNode();
                    _selectedNodeList.Remove(nodeMB.Node);
                }
                else
                {
                    foreach (NodeMB n in _nodeGOList)
                    {
                        if (n.Node.Depth == nodeMB.Node.Depth)
                        {
                            n.UnSelectNode();
                        }
                    }
                    _selectedNodeList.RemoveAll(n => n.Depth == nodeMB.Node.Depth);
                    _selectedNodeList.Add(nodeMB.Node);
                    nodeMB.SelectNode();
                }
            }
        }

        public void StartGame()
        {
            int TreeDepth = Random.Range(TreeDepthMin, TreeDepthMax);
            Node root = CreateNode(TreeDepth, Random.Range(NodeValMin, NodeValMax), SpawnLocation);
            SetTargetSum();

        }

        public Node CreateNode(int depth, int val, Vector3 spawnLocation)
        {
            Node node = new Node(val, depth);
            GameObject nodeGO = Instantiate(NodePrefab, spawnLocation, Quaternion.identity);
            nodeGO.transform.SetParent(NodeParent.transform);
            nodeGO.GetComponent<NodeMB>().Node = node;
            nodeGO.transform.GetComponentInChildren<TMP_Text>().text = node.NodeVal.ToString();
            _nodeGOList.Add(nodeGO.GetComponent<NodeMB>());

            if (depth == 1)
            {
                return node;
            }
            node.LeftNode = CreateNode(depth - 1, Random.Range(NodeValMin, NodeValMax), spawnLocation + new Vector3(-Mathf.Pow(2, node.Depth) / 2, -1.5f, 0));
            node.RightNode = CreateNode(depth - 1, Random.Range(NodeValMin, NodeValMax), spawnLocation + new Vector3(Mathf.Pow(2, node.Depth) / 2, -1.5f, 0));
            return node;
        }

        public void SetTargetSum()
        {
            int minIdx = 0;
            int maxIdx = 0;
            int power = 0;

            while (maxIdx < _nodeGOList.Count())
            {
                TargetSum += _nodeGOList[Random.Range(minIdx, maxIdx)].Node.NodeVal;
                power++;
                minIdx = maxIdx + 1;
                maxIdx = maxIdx + 1 + (int)(Mathf.Pow(2, power));
            }
            TargetSumText.text = $"TargetSum: {TargetSum}";
        }

        //Used for debugging
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
            if (_selectedNodeList.Sum(node => node.NodeVal) == TargetSum)
            {
                PlayVictorySound();
                SuccessPanel.SetActive(true);
                GameObject fx = Instantiate(FireWorksFX, SpawnLocation, Quaternion.identity);
                Destroy(fx, 1f);
            }
            return;
        }

        //reset game,clear all lists,reset ui
        public void ResetGame()
        {
            TargetSum = 0;
            _nodeGOList.Clear();
            _selectedNodeList.Clear();
            SuccessPanel.SetActive(false);
            foreach (Transform t in NodeParent.transform)
            {
                Destroy(t.gameObject);
            }
            StartGame();
        }

        void PlayVictorySound()
        {
            _audioSource.clip = VictorySound;
            _audioSource.Play();
        }

    }

}
