
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SupervisorMobility.API
{
    public class SwaggerGroupFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var apiDesc in context.ApiDescriptions)
            {
                var path = "/" + apiDesc.RelativePath.TrimEnd('/');
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length >= 2)
                {
                    var tag = segments[1]; // Segundo segmento después de /api/
                    var pathKey = "/" + apiDesc.RelativePath.TrimEnd('/');

                    if (swaggerDoc.Paths.TryGetValue(pathKey, out var pathItem))
                    {
                        foreach (var operation in pathItem.Operations)
                        {
                            operation.Value.Tags.Clear();
                            operation.Value.Tags.Add(new OpenApiTag { Name = tag });
                        }
                    }
                }
            }
        }
    }
}