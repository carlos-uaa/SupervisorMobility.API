
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SupervisorMobility.API
{
    public class SwaggerGroupFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc == null) throw new ArgumentNullException(nameof(swaggerDoc));
            if (context == null) throw new ArgumentNullException(nameof(context));

            foreach (var apiDesc in context.ApiDescriptions)
            {
                var relativePath = apiDesc?.RelativePath;
                if (string.IsNullOrWhiteSpace(relativePath))
                    continue;

                // Normalize: ensure leading slash and trim trailing slash
                var path = "/" + relativePath.TrimEnd('/');
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length >= 2)
                {
                    var tag = segments[1]; // second segment after the initial '/'
                    var pathKey = "/" + relativePath.TrimEnd('/');

                    if (swaggerDoc.Paths != null && swaggerDoc.Paths.TryGetValue(pathKey, out var pathItem))
                    {
                        foreach (var operation in pathItem.Operations)
                        {
                            // Ensure Tags set exists
                            if (operation.Value.Tags == null)
                            {
                                operation.Value.Tags = new HashSet<OpenApiTagReference>();
                            }

                            // Clear existing tag references and add a reference with the computed name
                            operation.Value.Tags.Clear();

                            // Fix for diagnostics:
                            // - CS7036: OpenApiTagReference requires a 'referenceId' parameter.
                            // - CS0200: 'Name' is read-only and cannot be set via object initializer.
                            // Use the constructor that accepts the referenceId and pass null for optional args.
                            operation.Value.Tags.Add(new OpenApiTagReference(tag, null, null));
                        }
                    }
                }
            }
        }
    }
}