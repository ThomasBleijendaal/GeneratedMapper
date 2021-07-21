namespace GeneratedMapper.Enums
{
    internal enum MappingType
    {
        From = 0,
        To = 1,
        Map = 2,
        Project = 4,
        MapFrom = Map | From,
        MapTo = Map | To,
        ExtensionMapTo = To,
        ExtensionProjectTo = Project | MapTo
    }
}
