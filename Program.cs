
DrawTrees();

void DrawTrees()
{
    // Input
    List<Tree> trees = TakeInputs(out Vector2 canvasSize);

    // Calculations and buffering draw data
    char[,] buffer = Draw(trees, canvasSize);

    // Output
    DrawToScreen(buffer);
}

#region Data input
List<Tree> TakeInputs(out Vector2 canvasSize)
{
    List<Tree> trees = new();
    canvasSize = new(0, 0);
    string inputLine = Console.ReadLine();

    while (inputLine != "")
    {
        string[] splitInput = inputLine.Split(' ');
        Tree t = new(int.Parse(splitInput[0]) - 1, int.Parse(splitInput[1]) - 1, int.Parse(splitInput[2]), int.Parse(splitInput[3]));
        Vector2 treeArea = t.GetTreeArea();

        if (canvasSize.x < treeArea.x)
            canvasSize.x = treeArea.x;
        if (canvasSize.y < treeArea.y)
            canvasSize.y = treeArea.y;

        trees.Add(t);
        
        inputLine = Console.ReadLine();
    }

    return trees;
}
#endregion

#region Calculations & Drawing
void DrawToScreen(char[,] buffer)
{
    Console.Clear();

    for (int y = 0; y < buffer.GetLength(1); y++)
    {
        for (int x = 0; x < buffer.GetLength(0); x++)
            Console.Write(buffer[x, y]);
        Console.WriteLine();
    }

    Console.WriteLine("Press any key to stop the program.");
    Console.ReadKey();
}

char[,] Draw(List<Tree> trees, Vector2 canvasSize)
{
    char[,] buffer = new char[canvasSize.x, canvasSize.y];

    // Fill the buffer with '.' characters
    for (int y = 0; y < buffer.GetLength(1); y++)
        for (int x = 0; x < buffer.GetLength(0); x++)
            buffer[x, y] = '.';

    foreach (Tree t in trees)
        DrawTree(buffer, t);

    return buffer;
}

void DrawTree(char[,] buffer, Tree t)
{
    // Draw the leaves of the tree
    for (int y = 0; y < t.treeHeight; y++)
    {
        int yPos = t.position.y + y;

        // The rows get wider as their y position rises, by 1 on every side when y increases by 1
        for (int x = t.position.x - y; x <= t.position.x + y; x++)
            buffer[x, yPos] = '*';
    }
    
    // Draw the trunk
    int trunkStartPos = t.position.y + t.treeHeight;

    for (int y = 0; y < t.trunkHeight; y++)
        buffer[t.position.x, trunkStartPos + y] = '*';
}
#endregion

#region Structs
public struct Tree
{
    public Vector2 position;
    public int treeHeight; // The height of the tree part with leaves
    public int trunkHeight;

    public Tree(Vector2 position, int treeHeight, int trunkHeight)
    {
        this.position = position;
        this.treeHeight = treeHeight;
        this.trunkHeight = trunkHeight;
    }

    public Tree(int xPos, int yPos, int treeHeight, int trunkHeight)
    {
        position = new(xPos, yPos);
        this.treeHeight= treeHeight;
        this.trunkHeight = trunkHeight;
    }

    // Calculates the area a tree instance occupies
    public Vector2 GetTreeArea()
    {
        // Calculate the y maximum of the tree (the bottommost position of its trunk)
        int yMaximum = position.y + (treeHeight - 1) + trunkHeight;

        // Calculate the x maximum of the tree (the rightmost position of the leaves)
        int xMaximum = position.x + (treeHeight - 1);

        // We have to add 1 to compensate working with indexing the buffer starting with 0 rather than 1
        return new(xMaximum + 1, yMaximum + 1);
    }
}

public struct Vector2
{
    public int x;
    public int y;

    public Vector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
#endregion