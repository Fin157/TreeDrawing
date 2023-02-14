
DrawTrees();

// The main function of this program
void DrawTrees()
{
    // Gathering input
    List<Tree> trees = TakeInputs(out Vector2 canvasSize);

    // Calculations and buffering draw data
    char[,] buffer = Draw(trees, canvasSize);

    // Output to the console
    DrawToScreen(buffer);
}

#region Data input
// Gathers inputs from the user and validates them
List<Tree> TakeInputs(out Vector2 canvasSize)
{
    List<Tree> trees = new();
    
    // Used for determining the size of the drawing canvas we need
    // (to fit all the trees there but keep it as small as possible)
    canvasSize = new(0, 0);

    string inputLine = Console.ReadLine();

    while (inputLine != "")
    {
        // Load a tree from the inputs we got

        // Chop up the input to distill just the numbers from it (still in string format)
        string[] splitInput = inputLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Validate the current input (gets rid of any formatting errors)
        if (!IsInputValid(splitInput, out int[] parsedInputs))
        {
            inputLine = Console.ReadLine();
            continue;
        }

        // Subtract one from the coordinate X and Y to convert them to 0-based ones
        Tree t = new(parsedInputs[0] - 1, parsedInputs[1] - 1, parsedInputs[2], parsedInputs[3]);
        Vector2 treeArea = t.GetRequiredCanvasSize();

        // Check if the tree is inside the buffer and thus valid
        // Gets rid of buffer index overflow errors (negative index values)
        if (t.IsTreeValid())
        {
            // Increase the size of the canvas if the current tree doesn't fit there
            if (canvasSize.x < treeArea.x)
                canvasSize.x = treeArea.x;
            if (canvasSize.y < treeArea.y)
                canvasSize.y = treeArea.y;

            trees.Add(t);
        }
        // Alert the user that these input coordinates are invalid and continue to the next input
        else
            Console.WriteLine("These input coordinates are invalid. Please try again.");
        
        inputLine = Console.ReadLine();
    }

    return trees;
}

// Checks if a single input set (one line/four numbers) is valid and also outputs the parsed inputs if it is
bool IsInputValid(string[] inputs, out int[] parsedInputs)
{
    // We need four elements of the array, otherwise the input is invalid
    if (inputs.Length != 4)
    {
        Console.WriteLine("The input must consist of four numbers. Please try again.");
        parsedInputs = Array.Empty<int>();
        return false;
    }

    // Check for parseability of all of the inputs
    parsedInputs = new int[inputs.Length];
    for (int i = 0; i < inputs.Length; i++)
    {
        if (!int.TryParse(inputs[i], out parsedInputs[i]))
        {
            Console.WriteLine("The input is in incorrect format. Please try again.");
            return false;
        }
    }

    return true;
}
#endregion

#region Calculations & Drawing
// Draws a buffer (a 2D array of chars) to the screen
void DrawToScreen(char[,] buffer)
{
    Console.Clear();

    for (int y = 0; y < buffer.GetLength(1); y++)
    {
        for (int x = 0; x < buffer.GetLength(0); x++)
            Console.Write(buffer[x, y]);
        Console.WriteLine();
    }

    Console.WriteLine("\nPress any key to stop the program.");
    Console.ReadKey();
}

// Creates a buffer and draws all the trees into it, then returns it
char[,] Draw(List<Tree> trees, Vector2 canvasSize)
{
    char[,] buffer = new char[canvasSize.x, canvasSize.y];

    // Fill the buffer with '.' characters (represents an empty character on the canvas)
    for (int y = 0; y < buffer.GetLength(1); y++)
        for (int x = 0; x < buffer.GetLength(0); x++)
            buffer[x, y] = '.';

    foreach (Tree t in trees)
        DrawTree(buffer, t);

    return buffer;
}

// Draws one tree to the buffer
void DrawTree(char[,] buffer, Tree t)
{
    // Draw the crown of the tree
    for (int y = 0; y < t.treeCrownHeight; y++)
    {
        int yPos = t.position.y + y;

        // The rows get wider as their y position rises, by 1 on every side when y increases by 1
        for (int x = t.position.x - y; x <= t.position.x + y; x++)
            buffer[x, yPos] = '*';
    }
    
    // Draw the trunk
    int trunkStartPos = t.position.y + t.treeCrownHeight;

    for (int y = 0; y < t.trunkHeight; y++)
        buffer[t.position.x, trunkStartPos + y] = '*';
}
#endregion

#region Structs
/// <summary>
/// A simple structure whose one instance can store data representing one tree
/// </summary>
public struct Tree
{
    public Vector2 position; // Position of the top of the tree (the topmost asterisk character)
    public int treeCrownHeight;
    public int trunkHeight;

    public Tree(int xPos, int yPos, int treeHeight, int trunkHeight)
    {
        position = new(xPos, yPos);
        this.treeCrownHeight= treeHeight;
        this.trunkHeight = trunkHeight;
    }

    /// <summary>
    /// Gets the bottommost and rightmost position this tree reaches to.
    /// We can find out if the current canvas size is enough for this tree from that data
    /// </summary>
    /// <returns>The said coordinates</returns>
    public Vector2 GetRequiredCanvasSize()
    {
        // Calculate the y maximum of the tree (the bottommost position of its trunk)
        int yMaximum = position.y + (treeCrownHeight - 1) + trunkHeight;

        // Calculate the x maximum of the tree (the rightmost position of the tree crown)
        int xMaximum = position.x + (treeCrownHeight - 1);

        // We have to add 1 to compensate working with indexing the buffer starting with 0 rather than 1
        return new(xMaximum + 1, yMaximum + 1);
    }
    
    /// <summary>
    /// Checks if this tree instance is valid
    /// (doesn't overlap to negative coordinates on the canvas)
    /// </summary>
    /// <returns>True if the tree stays in positive coordinates, otherwise false</returns>
    public bool IsTreeValid()
    {
        bool isXValid = position.x - (treeCrownHeight - 1) >= 0;
        bool isYValid = position.y >= 0;

        return isXValid && isYValid;
    }
}

/// <summary>
/// A struct representing a mathematical 2D vector
/// </summary>
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