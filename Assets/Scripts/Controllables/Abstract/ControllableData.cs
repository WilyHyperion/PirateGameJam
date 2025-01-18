using UnityEngine;
/// <summary>
/// Represents all the data attached to a given entity
/// general design philosophy is that this should only hold relatively static data that is not going to change often, ie name age etc
/// why a seperate object? good question(easier to pass into functions and generate)
/// </summary>
public class ControllableData
{
    public string name;
    public int age;
    public float height;
    public float weight;
    //really an acceleration value
    public float baseSpeed;
    public float maxSpeed;
    public static ControllableData getRandomizedControllableData()
    {
        ControllableData data = new ControllableData();
        data.name = "John Test";//todo name generation
        data.age = Random.Range(18, 100);
        data.height = Random.Range(1.5f, 2.5f);
        data.weight = Random.Range(50, 100);
        data.baseSpeed = Random.Range(5, 6);
        data.maxSpeed = data.baseSpeed;
        return data;
    }
    public float CalcBaseSpeed()
    {
        return baseSpeed;
    }
    public override string ToString()
    {
        return $"Name: {name}, Age: {age}, Height: {height}, Weight: {weight}, Base Speed: {baseSpeed}, Max Speed: {maxSpeed}";
    }
}
