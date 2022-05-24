using UnityEngine;
using UnityEngine.InputSystem;
namespace PaulLabs
{
    public class NodeMB : MonoBehaviour
    {

        private InputAction leftMouseClick;
        public bool Selected;
        public Node Node;


        private void LeftMouseClicked()
        {
            if (Selected)
            {
                UnSelectNode();
            }
            else
            {
                SelectNode();
            }
        }


        public void UnSelectNode()
        {
            GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            this.Selected = false;
            Debug.Log($"unselected node w val : {this.Node.NodeVal}!");
        }

        public void SelectNode()
        {
            // GameManager.Instance.SelectNode(this.Node);
            GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            this.Selected = true;
            Debug.Log($"selected node w val : {this.Node.NodeVal}!");
        }

    }
}