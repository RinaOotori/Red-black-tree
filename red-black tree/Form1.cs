using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private static Graphics g;
        private static RedBlackTree tree; //хранит само дерево

        public Form1()
        {
            InitializeComponent();
            g = CreateGraphics();
        }

        public enum ColorElement
        {
            Red,
            Black
        }

        /*
         * Класс экземпляра вершины
         * Содержит ключ вершины, её значение, ссылки на родителя и правого и 
левого детей
         */
        public class Node
        {
            public int Key { get; set; }
            public string Value { get; set; }
            public ColorElement ColorElement { get; set; }
            public Node LeftChild { get; set; }
            public Node RightChild { get; set; }
            public Node Parent { get; set; }

            public Node(int key, string value = null)
            {
                Key = key;
                Value = value;
                ColorElement = ColorElement.Red;
            }
        }

        /*
         * Класс красно-чёрного дерева
         * Содержит поле с корнем дерева, а также методы для работы с деревом
         */
        public class RedBlackTree
        {
            public Node Root { get; private set; }

            /*
             * Конструктор дерева. Строит дерево по элементам values
             */
            public RedBlackTree(params int[] values)
            {
                foreach (var value in values)
                {
                    Insert(value, $"{value}");
                }
            }

            /*
            * Метод, выполняющий вращение влево вокруг вершины node
            */
            private void RotateLeft(Node node)
            {
                var rightChild = node.RightChild;
                node.RightChild = rightChild.LeftChild;
                if (rightChild.LeftChild != null)
                {
                    rightChild.LeftChild.Parent = node;
                }

                rightChild.Parent = node.Parent;
                if (node.Parent == null)
                {
                    Root = rightChild;
                }
                else if (node == node.Parent.LeftChild)
                {
                    node.Parent.LeftChild = rightChild;
                }
                else
                {
                    node.Parent.RightChild = rightChild;
                }

                rightChild.LeftChild = node;
                node.Parent = rightChild;
            }

            /*
            * Метод, выполняющий вращение вправо вокруг вершины node
            */
            private void RotateRight(Node node)
            {
                var leftChild = node.LeftChild;
                node.LeftChild = leftChild.RightChild;
                if (leftChild.RightChild != null)
                {
                    leftChild.RightChild.Parent = node;
                }

                leftChild.Parent = node.Parent;
                if (node.Parent == null)
                {
                    Root = leftChild;
                }
                else if (node == node.Parent.LeftChild)
                {
                    node.Parent.LeftChild = leftChild;
                }
                else
                {
                    node.Parent.RightChild = leftChild;
                }

                leftChild.RightChild = node;
                node.Parent = leftChild;
            }

            /*
            * Метод, выполняющий вставку новой вершины со значением key
            */
            public void Insert(int key, string value = null)
            {
                var newNode = new Node(key, value);

                if (Root == null)
                {
                    Root = newNode;
                    newNode.ColorElement = ColorElement.Black;
                    return;
                }

                var currentNode = Root;
                while (true)
                {
                    if (newNode.Key < currentNode.Key)
                    {
                        if (currentNode.LeftChild == null)
                        {
                            currentNode.LeftChild = newNode;
                            newNode.Parent = currentNode;
                            break;
                        }

                        currentNode = currentNode.LeftChild;
                    }
                    else
                    {
                        if (currentNode.RightChild == null)
                        {
                            currentNode.RightChild = newNode;
                            newNode.Parent = currentNode;
                            break;
                        }

                        currentNode = currentNode.RightChild;
                    }
                }

                BalanceAfterInsert(newNode);
            }

            /*
            * Метод, выполняющий балансировку дерева после вставки нового 
узла
            */
            private void BalanceAfterInsert(Node newNode)
            {
                while (newNode != Root && newNode.Parent.ColorElement ==
                       ColorElement.Red)
                {
                    if (newNode.Parent == newNode.Parent.Parent.LeftChild)
                    {
                        var uncle = newNode.Parent.Parent.RightChild;
                        if (uncle != null && uncle.ColorElement ==
                            ColorElement.Red)
                        {
                            newNode.Parent.ColorElement = ColorElement.Black;
                            uncle.ColorElement = ColorElement.Black;
                            newNode.Parent.Parent.ColorElement =
                                ColorElement.Red;
                            newNode = newNode.Parent.Parent;
                        }
                        else
                        {
                            if (newNode == newNode.Parent.RightChild)
                            {
                                newNode = newNode.Parent;
                                RotateLeft(newNode);
                            }

                            newNode.Parent.ColorElement = ColorElement.Black;
                            newNode.Parent.Parent.ColorElement =
                                ColorElement.Red;
                            RotateRight(newNode.Parent.Parent);
                        }
                    }
                    else
                    {
                        var uncle = newNode.Parent.Parent.LeftChild;
                        if (uncle != null && uncle.ColorElement ==
                            ColorElement.Red)
                        {
                            newNode.Parent.ColorElement = ColorElement.Black;
                            uncle.ColorElement = ColorElement.Black;
                            newNode.Parent.Parent.ColorElement =
                                ColorElement.Red;
                            newNode = newNode.Parent.Parent;
                        }
                        else
                        {
                            if (newNode == newNode.Parent.LeftChild)
                            {
                                newNode = newNode.Parent;
                                RotateRight(newNode);
                            }

                            newNode.Parent.ColorElement = ColorElement.Black;
                            newNode.Parent.Parent.ColorElement =
                                ColorElement.Red;
                            RotateLeft(newNode.Parent.Parent);
                        }
                    }
                }

                if (Root.ColorElement == ColorElement.Red)
                {
                    Root.ColorElement = ColorElement.Black;
                }
            }

            /*
            * Метод, выполняющий поиск узла по значению key
            */
            public Node Search(int key)
            {
                var currentNode = Root;
                while (currentNode != null)
                {
                    if (key == currentNode.Key)
                    {
                        return currentNode;
                    }

                    if (key < currentNode.Key)
                    {
                        currentNode = currentNode.LeftChild;
                    }
                    else
                    {
                        currentNode = currentNode.RightChild;
                    }
                }

                return null;
            }

            /*
            * Метод, выполняющий удаление узла node
            */
            public void Delete(Node node)
            {
                if (node.LeftChild == null && node.RightChild == null)
                {
                    if (node == tree.Root)
                    {
                        tree = new RedBlackTree();
                        return;
                    }

                    var parent = node.Parent;
                    if (parent.LeftChild == node)
                    {
                        parent.LeftChild = null;
                        BalanceAfterDelete(node, parent.RightChild, parent,
                            true);
                        return;
                    }

                    parent.RightChild = null;
                    BalanceAfterDelete(node, parent.LeftChild, parent,
                        false);
                    return;
                }

                if (node.LeftChild == null || node.RightChild == null)
                {
                    var temp = node;
                    temp = node.LeftChild != null ? node.LeftChild : node.RightChild;
                    if (node == tree.Root)
                    {
                        temp.Parent = null;
                        tree.Root = temp;
                        tree.Root.ColorElement = ColorElement.Black;
                    }
                    else
                    {
                        if (node.Parent.LeftChild == node)
                        {
                            node.Parent.LeftChild = temp;
                            BalanceAfterDelete(node, node.Parent.RightChild,
                                node.Parent, true);
                            return;
                        }

                        node.Parent.RightChild = temp;
                        BalanceAfterDelete(node, node.Parent.LeftChild,
                            node.Parent, false);
                        return;
                    }
                }
                else
                {
                    var newNode = FindMinLeft();
                    if (node == tree.Root)
                    {
                        tree.Root.Value = newNode.Value;
                        tree.Root.Key = newNode.Key;
                        newNode.Parent.LeftChild = null;
                        tree.Root.ColorElement = ColorElement.Black;
                        BalanceAfterDelete(tree.Root.RightChild,
                            tree.Root.LeftChild, tree.Root, false);
                        return;
                    }

                    node.Value = newNode.Value;
                    node.Key = newNode.Key;
                    newNode.Parent.LeftChild = null;
                    if (node.Parent.LeftChild == node)
                    {
                        BalanceAfterDelete(node, node.Parent.RightChild,
                            node.Parent, true);
                    }
                    else
                    {
                        BalanceAfterDelete(node, node.Parent.LeftChild,
                            node.Parent, false);
                    }
                }

                tree.Root.ColorElement = ColorElement.Black;
            }

            /*
            * Метод, выполняющий балансировку метода после удаления узла node
            */
            private void BalanceAfterDelete(Node node, Node brother, Node
                parent, bool isCurrentLeft)
            {
                while ((node == null || node.ColorElement ==
                           ColorElement.Black) && node != tree.Root)
                {
                    if (isCurrentLeft)
                    {
                        if (brother == null)
                        {
                            return;
                        }

                        if (brother.ColorElement == ColorElement.Red)
                        {
                            brother.ColorElement = ColorElement.Black;
                            parent.ColorElement = ColorElement.Red;
                            RotateLeft(parent);
                        }

                        if ((brother.LeftChild == null ||
                             brother.LeftChild.ColorElement ==
                             ColorElement.Black) &&
                            (brother.RightChild == null ||
                             brother.RightChild.ColorElement ==
                             ColorElement.Black))
                        {
                            brother.ColorElement = ColorElement.Red;
                        }
                        else
                        {
                            if ((brother.RightChild == null ||
                                 brother.RightChild.ColorElement == ColorElement.Black) &&
                                (brother.LeftChild != null &&
                                 brother.LeftChild.ColorElement == ColorElement.Red))
                            {
                                if (brother.LeftChild != null)
                                    brother.LeftChild.ColorElement = ColorElement.Black;
                                brother.ColorElement = ColorElement.Red;
                                RotateRight(brother);
                            }

                            if (brother.RightChild != null &&
                                brother.RightChild.ColorElement == ColorElement.Red)
                            {
                                brother.ColorElement = parent.ColorElement;
                                parent.ColorElement = ColorElement.Black;
                                if (brother.RightChild != null)
                                    brother.RightChild.ColorElement =
                                        ColorElement.Black;
                                RotateLeft(parent);
                            }
                        }
                    }
                    else
                    {
                        if (brother == null)
                        {
                            return;
                        }

                        if (brother.ColorElement == ColorElement.Red)
                        {
                            brother.ColorElement = ColorElement.Black;
                            parent.ColorElement = ColorElement.Red;
                            RotateRight(parent);
                        }

                        if ((brother.LeftChild == null ||
                             brother.LeftChild.ColorElement ==
                             ColorElement.Black) &&
                            (brother.RightChild == null ||
                             brother.RightChild.ColorElement ==
                             ColorElement.Black))
                        {
                            brother.ColorElement = ColorElement.Red;
                        }
                        else
                        {
                            if ((brother.LeftChild == null ||
                                 brother.LeftChild.ColorElement == ColorElement.Black) &&
                                (brother.RightChild != null &&
                                 brother.RightChild.ColorElement == ColorElement.Red))
                            {
                                if (brother.RightChild != null)
                                    brother.RightChild.ColorElement = ColorElement.Black;
                                brother.ColorElement = ColorElement.Red;
                                RotateLeft(brother);
                            }

                            if (brother.LeftChild != null &&
                                brother.LeftChild.ColorElement == ColorElement.Red)
                            {
                                brother.ColorElement = parent.ColorElement;
                                parent.ColorElement = ColorElement.Black;
                                if (brother.LeftChild != null)
                                    brother.LeftChild.ColorElement =
                                        ColorElement.Black;
                                RotateRight(parent);
                            }
                        }
                    }

                    node = node.Parent;
                    node.ColorElement = ColorElement.Black;
                }
            }

            /*
            * Метод, ищущий самое правое значение в левом поддереве
            */
            private static Node FindMinLeft()
            {
                var currentNode = tree.Root.LeftChild;
                if (currentNode == null)
                {
                    return tree.Root;
                }

                while (currentNode.RightChild != null)
                {
                    currentNode = currentNode.RightChild;
                }

                return currentNode;
            }
        }

        /*
        * Метод, который выполняет отрисовку НЕ NIL-узла
        */
        private static void DrawEllipse(Color color, int x, int y, int value,
            int lvl, int offsetX, int offsetY)
        {
            g.DrawLine(new Pen(Color.Black, 3), x, y, x + offsetX / lvl, y +
                                                                         offsetY);
            g.DrawLine(new Pen(Color.Black, 3), x, y, x - offsetX / lvl, y +
                                                                         offsetY);
            g.DrawEllipse(new Pen(Color.Black, 3), x - 20, y - 20, 40, 40);
            g.FillEllipse(new SolidBrush(color), x - 20, y - 20, 40, 40);
            g.DrawString($"{value}", DefaultFont, Brushes.White, x - 5, y -
                                                                        5);
        }

        /*
        * Метод, выполняющийся при нажатии на кнопку "Построить дерево"
        */
        private void button1_Click(object sender, EventArgs e)
        {
            var temp = textBox1.Text.Split();
            var values = new List<int>();
            foreach (var value in temp)
            {
                if (int.TryParse(value, out var v))
                {
                    values.Add(v);
                }
                else
                {
                    button2.Enabled = false;
                    button3.Enabled = false;
                    MessageBox.Show("Некорректный ввод!");
                    return;
                }
            }

            button2.Enabled = true;
            button3.Enabled = true;
            tree = new RedBlackTree(temp.Select(s =>
                int.Parse(s)).ToArray());
            DrawTree();
        }

        /*
        * Метод, выполняющий отрисовку дерева
        */
        private static void DrawTree()
        {
            g.Clear(Color.White);
            const int offsetX = 200;
            const int offsetY = 60;
            const int level = 1;
            var q = new Queue<(Node, int, int, int)>();
            q.Enqueue((tree.Root, 700, 100, level));
            while (q.Count > 0)
            {
                var t = q.Dequeue();
                DrawEllipse(t.Item1.ColorElement == ColorElement.Black ? Color.Black : Color.Red, t.Item2, t.Item3,
                    t.Item1.Key, t.Item4, offsetX, offsetY);

                CheckChildren(q, t, offsetY, offsetX);
            }
        }

        /*
        * Метод, проверяющий наличие у текущей вершины детей
        */
        private static void CheckChildren(Queue<(Node, int, int, int)> q,
            (Node, int, int, int) element, int offsetY,
            int offsetX)
        {
            if (element.Item1.RightChild != null)
            {
                q.Enqueue((element.Item1.RightChild, element.Item2 + offsetX
                    / element.Item4, element.Item3 + offsetY,
                    element.Item4 + 1));
            }
            else
            {
                DrawNIL(element.Item2 + offsetX / element.Item4 - 10,
                    element.Item3 + offsetY - 10);
            }

            if (element.Item1.LeftChild != null)
            {
                q.Enqueue((element.Item1.LeftChild, element.Item2 - offsetX /
                    element.Item4, element.Item3 + offsetY,
                    element.Item4 + 1));
            }
            else
            {
                DrawNIL(element.Item2 - offsetX / element.Item4 - 10,
                    element.Item3 + offsetY - 10);
            }
        }

        /*
        * Метод, отрисовывающий NIL-узлы
        */
        private static void DrawNIL(int x, int y)
        {
            g.DrawEllipse(new Pen(Color.Black, 3), x, y, 20, 20);
            g.FillEllipse(new SolidBrush(Color.Black), x, y, 20, 20);
        }

        /*
        * Метод, выполняющийся при нажатии на кнопку "Добавить элемент"
        */
        private void button2_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox2.Text, out var value))
            {
                tree.Insert(value);
                textBox1.Text += $" {value}";
                DrawTree();
            }
            else
            {
                MessageBox.Show("Некорректный элемент!");
            }
        }

        /*
        * Метод, выполняющийся при нажатии на кнопку "Удалить элемент"
        */
        private void button3_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox3.Text, out var value) &&
                tree.Search(value) != null)
            {
                tree.Delete(tree.Search(value));
                var str = textBox1.Text.Replace($"{value.ToString()}", "");
                str = str.Replace("  ", " ");
                if (str[0] == ' ')
                {
                    str = str.Substring(1, str.Length - 1);
                }

                if (str.Last() == ' ')
                {
                    str = str.Substring(0, str.Length - 1);
                }

                textBox1.Text = str;
                DrawTree();
            }
            else
            {
                MessageBox.Show("Некорректный элемент!");
            }
        }

        /*
        * Метод, выполняющийся при загрузке формы
        */
        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
        }
    }
}