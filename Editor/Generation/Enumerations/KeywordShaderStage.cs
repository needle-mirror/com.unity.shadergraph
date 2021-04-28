namespace UnityEditor.ShaderGraph
{
    [GenerationAPI]
    internal enum KeywordShaderStage
    {
        Vertex = (1 << 0),
        Fragment = (1 << 1),
        Geometry = (1 << 2),
        Hull = (1 << 3),
        Domain = (1 << 4),
        RayTracing = (1 << 5),

        // Common aggregates
        FragmentAndRaytracing = (Fragment | RayTracing),
        All = (Vertex | Fragment | Geometry | Hull | Domain | RayTracing)
    }
}