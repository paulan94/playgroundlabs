namespace PaulLabs
{
    //pure c# class to hold data
    public class Node
    {
        public int NodeVal;
        public int Depth;
        public Node LeftNode;
        public Node RightNode;

        public Node(int val, int depth)
        {
            this.NodeVal = val;
            this.Depth = depth;
        }
    }
}
