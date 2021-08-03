namespace GeneratedMapper.Enums
{
    internal enum MappingType
    {
        From = 0,
        To = 1,
        Map = 2,
        Extension = 4,
        Project = 8,
        MapFrom = Map | From,
        MapTo = Map | To,
        ExtensionMapTo = Extension | To,
        ExtensionProjectTo = Project | To
    }
}
