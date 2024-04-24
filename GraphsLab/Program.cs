class Vertice
{
    public int index;
    public List<Edge> edges;

    public Vertice(int index)
    {
        this.index = index;
        edges = new List<Edge>();
    }

    public void addEdge(int weight, Vertice destination) 
    { 
        edges.Add(new Edge(weight, destination));
    }

    public Edge getMinWeightEdge()
    {
        if (edges == null)
        {
            return null;
        }

        int minWeight = 999999999;
        Edge minEdge = null;

        foreach (Edge edge in edges)
        {
            if (edge.weight < minWeight)
            {
                minWeight = edge.weight;
                minEdge = edge;
            }
        }

        return minEdge;
    }
}

class Edge
{
    public int weight;
    public Vertice destination;

    public Edge(int weight, Vertice destination)
    {
        this.weight = weight;
        this.destination = destination;
    }
}

class Point
{
    public int a;
    public int b;

    public Point(int a, int b)
    {
        this.a = a;
        this.b = b;
    }

    public int calculateDistance(Point to)
    {
        return Math.Abs(a - to.a) + Math.Abs(b - to.b);
    }
}

class SpanningTreeCalculator
{
    private static int weight;
    private static int diameter;

    public static void starPlatinum(int graphSize, Dictionary<int, Vertice> vertices, int startIndex)
    {
        weight = 0;
        diameter = graphSize / 16;
        List<int> visitedVertices = new List<int>();
        visitedVertices.Add(startIndex);
        Vertice currentVertice = vertices[startIndex];
        int i = 1;
        Edge minEdge;
        int diameterCount = 0;
        List<string> tree = new List<string>();

        while (i < graphSize)
        {
            if (diameterCount == diameter / 2)
            {
                currentVertice = vertices[startIndex];
                diameterCount = 0;
                continue;
            }

            minEdge = currentVertice.getMinWeightEdge();
            if (visitedVertices.Contains(minEdge.destination.index))
            {
                currentVertice.edges.Remove(currentVertice.edges.Single(r => r.destination.index == minEdge.destination.index));
                continue;
            }

            if (currentVertice.index < minEdge.destination.index)
            {
                tree.Add(currentVertice.index + " " + minEdge.destination.index);
            }
            else
            {
                tree.Add(minEdge.destination.index + " " + currentVertice.index);
            }

            currentVertice.edges.Remove(currentVertice.edges.Single(r => r.destination.index == minEdge.destination.index));
            currentVertice = minEdge.destination;
            weight += minEdge.weight;
            visitedVertices.Add(currentVertice.index);
            diameterCount++;
            i++;
        }

        if (weight < Program.minTreeSize) 
        {
            var sorted = from str in tree orderby Int32.Parse(str.Split(" ")[0]) select str;

            StreamWriter writer = new StreamWriter("E:\\GraphsLab\\GraphsLab\\Graphs\\Answers\\StarPlatinum" + graphSize + ".txt");
            writer.WriteLine("c Вес дерева = " + weight + ", диаметр = " + diameter + ",");
            writer.WriteLine("p edge " + graphSize + " " + (graphSize - 1));
            foreach (string line in sorted)
            {
                writer.WriteLine("e " + line);
            }

            writer.Close();

            Program.minTreeSize = weight;
        }
    }
}

class Program
{
    private static int graphSize;
    private static Point[] points;
    private static Dictionary<int, Vertice> vertices = new Dictionary<int, Vertice>();
    public static int minTreeSize;

    static void parseFile(string filePath)
    {
        StreamReader reader = new StreamReader(filePath);

        String line = reader.ReadLine();
        graphSize = Int32.Parse(line.Split("=")[1].Trim());
        points = new Point[graphSize];
        line = reader.ReadLine();

        for (int i = 0; line != null; i++)
        {
            string[] coordinates;
            if (line.Contains("\t"))
            {
                coordinates = line.Split("\t");
            }
            else
            {
                coordinates = line.Split(" ");
            }

            Point point = new Point(Int32.Parse(coordinates[0].Trim()), Int32.Parse(coordinates[1].Trim()));
            points[i] = point;
            vertices.Add(i + 1, new Vertice((i + 1)));

            line = reader.ReadLine();
        }
        reader.Close();

        fillVertices();
    }

    static void fillVertices()
    {
        for (int i = 0; i < graphSize; i++)
        {
            for (int j = 0; j < graphSize; j++)
            {
                if (i == j)
                {
                    continue;
                }
                
                vertices[i + 1].addEdge(points[i].calculateDistance(points[j]), vertices[j + 1]);
            }
        }
    }

    static void Main(string[] args)
    {
        int[] fileSizes = { 64, 128, 512, 2048, 4096 };
        foreach (int size in fileSizes)
        {
            minTreeSize = 999999999;
            parseFile("E:\\GraphsLab\\GraphsLab\\Graphs\\Benchmark\\Taxicab_" + size + ".txt");
            for (int i = 1; i <= graphSize; i++) {
                SpanningTreeCalculator.starPlatinum(graphSize, vertices, i);
                vertices.Clear();
                if (i != graphSize)
                {
                    for (int j = 0; j < graphSize; j++)
                    {
                        vertices.Add(j + 1, new Vertice((j + 1)));
                    }
                    fillVertices();
                }
            }
        }
    }
}