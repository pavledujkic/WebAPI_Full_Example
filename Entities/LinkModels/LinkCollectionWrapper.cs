namespace Entities.LinkModels;

public class LinkCollectionWrapper<T> where T : class
{
    public List<T> Value { get; set; } = new();
    public Link? Links { get; set; }

    public LinkCollectionWrapper()
    {
    }

    public LinkCollectionWrapper(List<T> value) => Value = value;
}