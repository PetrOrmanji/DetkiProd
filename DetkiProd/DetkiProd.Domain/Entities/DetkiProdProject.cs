using DetkiProd.Domain.Exceptions;

namespace DetkiProd.Domain.Entities;

public class DetkiProdProject : BaseEntity
{
    public string Name { get; set; }
    public string Tools { get; set; }
    public string Year { get; set; }

    public string VideoUrl { get; set; }

    public static DetkiProdProject Create(string name, string tools, string year, string videoUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidProjectDataException("Name param is null or whitespace");

        if (string.IsNullOrWhiteSpace(tools))
            throw new InvalidProjectDataException("Tools param is null or whitespace");

        if (!int.TryParse(year, out var _))
            throw new InvalidProjectDataException("Year param cannot be parsed");

        if (string.IsNullOrWhiteSpace(videoUrl))
            throw new InvalidProjectDataException("Video url param is null or whitespace");

        return new DetkiProdProject()
        {
            Name = name,
            Tools = tools,
            Year = year,
            VideoUrl = videoUrl,
        };
    }
}
