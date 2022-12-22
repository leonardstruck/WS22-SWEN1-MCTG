namespace Models;

public class GenericCard
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public int Damage { get; set; }
    
    public GenericCard(string name, int damage, Guid? id)
    {
        Name = name;
        Damage = damage;
        Id = id;
    }
}