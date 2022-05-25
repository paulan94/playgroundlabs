using UnityEngine;
namespace PaulLabs
{
    //Node MonoBehaviour class to store node reference and handle selection events.
    public class NodeMB : MonoBehaviour
    {
        public bool Selected;
        public Node Node;

        public void UnSelectNode()
        {
            GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            this.Selected = false;
        }

        public void SelectNode()
        {
            GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            this.Selected = true;
        }
    }
}