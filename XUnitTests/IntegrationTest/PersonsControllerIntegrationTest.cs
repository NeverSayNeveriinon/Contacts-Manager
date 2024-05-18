using FluentAssertions;
using HtmlAgilityPack;


namespace xUnit_Tests.IntegrationTest;

public class HomeControllerIntegrationTest
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
        
    public HomeControllerIntegrationTest()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }
        
    #region Index

    // When we request to "~/home/index", it should return '2xx' status code and proper 'div html tag with grid class'    
    [Fact]
    public async Task Index_Valid_ReturnIndexView()
    {
        // Arrange
            
        // Act
        HttpResponseMessage response = await _client.GetAsync("~/home/index");
            
        // Assert
        // any '2xx' status code
        response.Should().BeSuccessful(); 

        // check the 'response important html tag' to be not empty (here we'll check table)
        string responseBody = await response.Content.ReadAsStringAsync();
        HtmlDocument html = new HtmlDocument();
        html.LoadHtml(responseBody);
        var document = html.DocumentNode;

        // document.QuerySelectorAll(".grid").Should().NotBeNullOrEmpty();
    }

    #endregion 
        
}