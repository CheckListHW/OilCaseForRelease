namespace OilCaseApi.resources;

public class GroupTagAttribute: Attribute
{
    public string Name { get; }

    public GroupTagAttribute(string name)
    {
        Name = name;
    }
}