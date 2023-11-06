using System;
using System.Collections.Generic;
using System.Globalization;

public class Program
{
    public static void Main()
    {
        var graph = new Dictionary<string, Dictionary<string, int>>(StringComparer.InvariantCultureIgnoreCase);

        // Adiciona conexões (caminhos) no grafo de forma bidirecional para um grafo não-dirigido
        AddBidirectionalEdge(graph, "Uberlandia", "Araguari", 30);
        AddBidirectionalEdge(graph, "Uberlandia", "Romaria", 78);
        AddBidirectionalEdge(graph, "Uberlandia", "Indianopolis", 45);
        AddBidirectionalEdge(graph, "Uberlandia", "Douradinho", 63);
        AddBidirectionalEdge(graph, "Uberlandia", "Monte Alegre de Minas", 60);
        AddBidirectionalEdge(graph, "Uberlandia", "Tupaciguara", 60);
        AddBidirectionalEdge(graph, "Araguari", "Estrela do Sul", 34);
        AddBidirectionalEdge(graph, "Araguari", "Cascalho Rico", 28);
        AddBidirectionalEdge(graph, "Cascalho Rico", "Grupiara", 32);
        AddBidirectionalEdge(graph, "Grupiara", "Estrela do Sul", 38);
        AddBidirectionalEdge(graph, "Estrela do Sul", "Romaria", 27);
        AddBidirectionalEdge(graph, "Romaria", "Santa Juliana", 28);
        AddBidirectionalEdge(graph, "Santa Juliana", "Indianopolis", 40);
        AddBidirectionalEdge(graph, "Tupaciguara", "Monte Alegre de Minas", 44);
        AddBidirectionalEdge(graph, "Tupaciguara", "Itumbiara", 55);
        AddBidirectionalEdge(graph, "Monte Alegre de Minas", "Centralina", 75);
        AddBidirectionalEdge(graph, "Monte Alegre de Minas", "Ituiutaba", 85);
        AddBidirectionalEdge(graph, "Monte Alegre de Minas", "Douradinho", 28);
        AddBidirectionalEdge(graph, "Ituiutaba", "Capinopolis", 30);
        AddBidirectionalEdge(graph, "Capinopolis", "Centralina", 40);
        AddBidirectionalEdge(graph, "Centralina", "Itumbiara", 20);

        bool exit = false;
        while (!exit)
        {
            Console.Write("Digite a cidade de origem (ou 'sair' para encerrar): ");
            string start = Console.ReadLine().Trim();

            if (string.Equals(start, "sair", StringComparison.InvariantCultureIgnoreCase)) break;

            Console.Write("Digite a cidade de destino (ou 'sair' para encerrar): ");
            string target = Console.ReadLine().Trim();

            if (string.Equals(target, "sair", StringComparison.InvariantCultureIgnoreCase)) break;

            // Verifica se as cidades inseridas existem no grafo
            if (!graph.ContainsKey(start) || !graph.ContainsKey(target))
            {
                Console.WriteLine("Uma ou ambas as cidades não foram encontradas no grafo.");
            }
            else
            {
                // Executa o algoritmo de Dijkstra
                var (path, distance) = Dijkstra(graph, start, target);
                if (path != null)
                {
                    Console.WriteLine($"Caminho mais curto de {start} para {target}: {string.Join(" -> ", path)}");
                    Console.WriteLine($"Distância total: {distance} km");
                }
                else
                {
                    Console.WriteLine($"Não há caminho disponível de {start} para {target}.");
                }
            }

            Console.WriteLine("\nDeseja fazer outra pesquisa? (Digite 'sair' para encerrar ou Enter para continuar)");
            if (string.Equals(Console.ReadLine().Trim(), "sair", StringComparison.InvariantCultureIgnoreCase))
            {
                exit = true;
            }
        }
    }
    // Método para adicionar arestas bidirecionais ao grafo
    public static void AddBidirectionalEdge(Dictionary<string, Dictionary<string, int>> graph, string city1, string city2, int distance)
    {
        if (!graph.ContainsKey(city1))
            graph[city1] = new Dictionary<string, int>();

        if (!graph.ContainsKey(city2))
            graph[city2] = new Dictionary<string, int>();

        graph[city1][city2] = distance;
        graph[city2][city1] = distance;
    }

    public static (List<string>, int) Dijkstra(Dictionary<string, Dictionary<string, int>> graph, string start, string target)
    {
        var distances = new Dictionary<string, int>();
        var previous = new Dictionary<string, string>();
        var nodes = new List<string>();

        List<string> path = null;

        foreach (var vertex in graph)
        {
            if (vertex.Key == start)
            {
                distances[vertex.Key] = 0;
            }
            else
            {
                distances[vertex.Key] = int.MaxValue;
            }

            nodes.Add(vertex.Key);
        }

        while (nodes.Count != 0)
        {
            nodes.Sort((x, y) => distances[x] - distances[y]);

            var smallest = nodes[0];
            nodes.Remove(smallest);

            if (smallest == target)
            {
                path = new List<string>();
                while (previous.ContainsKey(smallest))
                {
                    path.Add(smallest);
                    smallest = previous[smallest];
                }

                break;
            }

            if (distances[smallest] == int.MaxValue)
            {
                break;
            }

            foreach (var neighbor in graph[smallest])
            {
                var alt = distances[smallest] + neighbor.Value;
                if (alt < distances[neighbor.Key])
                {
                    distances[neighbor.Key] = alt;
                    previous[neighbor.Key] = smallest;
                }
            }
        }

        if (path != null)
        {
            path.Reverse();
            path.Insert(0, start);
            return (path, distances[target]);
        }
        else
        {
            return (null, 0);
        }
    }
}