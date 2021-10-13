using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleTables;

namespace OneListClient
{
    class Program
    {
        static async Task ShowAllItems(string token)
        {
            var client = new HttpClient();
            var url = $"https://one-list-api.herokuapp.com/items?access_token={token}";
            var responseBodyAsStream = await client.GetStreamAsync(url);
            //                                              Describe the Shape of the data (array in JSON => List, Object in JSON => Item)
            //                                                  v   v
            var items = await JsonSerializer.DeserializeAsync<List<Item>>(responseBodyAsStream);

            var table = new ConsoleTable("Id", "Description", "Created At", "Completed?");

            // Back in the world of List/LINQ/C#
            foreach (var item in items)
            {
                table.AddRow(item.Id, item.Text, item.CreatedAt, item.CompletedStatus);
            }

            table.Write();
        }

        static async Task GetOneItem(string token, int id)
        {
            try
            {
                var client = new HttpClient();

                var url = $"https://one-list-api.herokuapp.com/items/{id}?access_token={token}";

                var responseBodyAsStream = await client.GetStreamAsync(url);

                var item = await JsonSerializer.DeserializeAsync<Item>(responseBodyAsStream);

                var table = new ConsoleTable("Id", "Description", "Created At", "Updated At", "Completed?");

                table.AddRow(item.Id, item.Text, item.CreatedAt, item.UpdatedAt, item.CompletedStatus);
                table.Write();
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("I could not find that item!");
            }
        }

        static async Task AddOneItem(string token, Item newItem)
        {
            var client = new HttpClient();

            var url = $"https://one-list-api.herokuapp.com/items?access_token={token}";

            var jsonBody = JsonSerializer.Serialize(newItem);

            var jsonBodyAsContent = new StringContent(jsonBody);
            jsonBodyAsContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(url, jsonBodyAsContent);

            var responseJson = await response.Content.ReadAsStreamAsync();

            var item = await JsonSerializer.DeserializeAsync<Item>(responseJson);

            var table = new ConsoleTable("Id", "Descriptions", "Created At", "Updated At", "Completed?");

            table.AddRow(item.Id, item.Text, item.CreatedAt, item.UpdatedAt, item.CompletedStatus);
            table.Write();
        }

        static async Task UpdateOneItem(string token, int id, Item updateItem)
        {
            var client = new HttpClient();

            var url = $"https://one-list-api.herokuapp.com/items/{id}?access_token={token}";

            var jsonBody = JsonSerializer.Serialize(updateItem);

            var jsonBodyAsContent = new StringContent(jsonBody);
            jsonBodyAsContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PutAsync(url, jsonBodyAsContent);

            var responseJson = await response.Content.ReadAsStreamAsync();

            var item = await JsonSerializer.DeserializeAsync<Item>(responseJson);

            var table = new ConsoleTable("Id", "Description", "Created At", "Updated At", "Completed?");

            table.AddRow(item.Id, item.Text, item.CreatedAt, item.UpdatedAt, item.CompletedStatus);
            table.Write();
        }

        static async Task DeleteOneItem(string token, int id)
        {
            try
            {
                var client = new HttpClient();

                var url = $"https://one-list-api.herokuapp.com/items/{id}?access_token={token}";

                var response = await client.DeleteAsync(url);

                await response.Content.ReadAsStreamAsync();
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("I could not find that item!");
            }
        }
        static async Task Main(string[] args)
        {
            var token = "";

            if (args.Length == 0)
            {
                Console.Write("What list would you like? ");
                token = Console.ReadLine();
            }
            else
            {
                token = args[0];
            }

            var keepGoing = true;
            while (keepGoing)
            {
                Console.Clear();
                Console.Write("Get (A)ll Todo, (O)ne Todo, (C)reate a new item, (U)pdate an item, (D)elete an item, or (Q)uit: ");
                var choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "Q":
                        keepGoing = false;
                        break;

                    case "A":
                        await ShowAllItems(token);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;

                    case "C":
                        Console.Write("Enter the description of your new todo: ");
                        var text = Console.ReadLine();

                        var newItem = new Item
                        {
                            Text = text
                        };

                        await AddOneItem(token, newItem);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;

                    case "O":
                        Console.Write("Enter the ID: ");
                        var id = int.Parse(Console.ReadLine());

                        await GetOneItem(token, id);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;

                    case "U":
                        Console.Write("Enter the ID of the item to update: ");
                        var existingId = int.Parse(Console.ReadLine());

                        Console.Write("Enter the new description: ");
                        var newText = Console.ReadLine();

                        Console.Write("Enter yes or no to indicate if the item is complete: ");
                        var newComplete = Console.ReadLine().ToLower() == "yes";

                        var updatedItem = new Item
                        {
                            Text = newText,
                            Complete = newComplete
                        };

                        await UpdateOneItem(token, existingId, updatedItem);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;

                    case "D":
                        Console.Write("Enter the ID of the item to delete: ");
                        var idToDelete = int.Parse(Console.ReadLine());

                        await DeleteOneItem(token, idToDelete);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
