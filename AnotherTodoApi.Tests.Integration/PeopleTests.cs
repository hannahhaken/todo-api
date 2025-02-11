// using System.Net;
// using System.Net.Http.Json;
// using Microsoft.AspNetCore.Mvc.Testing;
//
// namespace AnotherTodoApi.Tests.Integration;
//
// public class PeopleTests
// {
//     [Fact]
//     public async Task CreatePerson()
//     {
//         await using var application = new WebApplicationFactory<IPeopleService>();
//
//         var client = application.CreateClient();
//
//         var result = await client.PostAsJsonAsync("/people", new Person
//         {
//             FirstName = "Maarten",
//             LastName = "Balliauw",
//             Email = "maarten@jetbrains.com"
//         });
//
//         Assert.Equal(HttpStatusCode.OK, result.StatusCode);
//         Assert.Equal("\"Maarten Balliauw created.\"", await result.Content.ReadAsStringAsync());
//     }
// }