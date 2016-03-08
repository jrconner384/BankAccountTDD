namespace EffinghamLibrary
{
    /// <summary>
    /// Represents the allowable currency types.
    /// </summary>
    public enum CurrencyType
    {
        // Can mark each enum element with any integer value.
        // Might want to set them equal to 2^x, use [Flags] on the enum. Then you can do bitwise operators:
        // [Flags]
        // enum Things
        // {
        //     oneThing = 1,
        //     otherThing = 2,
        //     thisThing = 4
        // }
        // Things thing = Things.oneThing | Things.thisThing;
        Dollar,
        Peso,
        Yen
    }
}
