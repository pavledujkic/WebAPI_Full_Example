namespace Entities.LinkModels;

public class LinkCollectionWrapper<T>
{
    public List<T> Value { get; set; } = new();

    public LinkCollectionWrapper()
    {
    }
    
    public LinkCollectionWrapper(List<T> value) => Value = value;
}