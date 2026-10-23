using Microsoft.OpenApi;

namespace SCA_MVC.Api
{
    // Swagger (OpenAPI): la documentación de la API, generada por la propia aplicación a partir de
    // los controladores y de sus comentarios. Se ve en /swagger y permite probar los endpoints
    // desde el navegador, con el botón Authorize para cargar la clave del lector.
    public static class DocumentacionApi
    {
        public static IServiceCollection AddDocumentacionApi(this IServiceCollection servicios)
        {
            servicios.AddSwaggerGen(opciones =>
            {
                opciones.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API de lectores RFID · Sistema Control de Almuerzos",
                    Version = "v1",
                    Description = "Registro de comensales desde un lector RFID de red. " +
                                  "Cada lector se identifica con su propia clave en la cabecera X-Api-Key " +
                                  "y registra en el servicio activo del lugar donde está instalado."
                });

                // Solo la API: las pantallas de la aplicación son MVC y no van en esta documentación
                opciones.DocInclusionPredicate((_, api) => api.RelativePath?.StartsWith("api/") == true);

                // El texto de cada endpoint y de cada respuesta sale de los comentarios /// del código
                var archivoXml = Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml");
                opciones.IncludeXmlComments(archivoXml);

                // Cómo se autentica: la clave del lector en una cabecera. Con esto Swagger UI muestra
                // el botón Authorize y manda la clave en cada prueba.
                opciones.AddSecurityDefinition(ApiKeyAuthenticationHandler.Esquema, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = ApiKeyAuthenticationHandler.Cabecera,
                    Description = "La clave del lector, tal cual se mostró al crearlo (empieza con sca_)."
                });

                opciones.AddSecurityRequirement(documento => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(ApiKeyAuthenticationHandler.Esquema, documento)] = []
                });
            });

            return servicios;
        }

        public static WebApplication UseDocumentacionApi(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(opciones =>
            {
                opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "API de lectores v1");
                opciones.DocumentTitle = "API de lectores · SCA";
            });

            return app;
        }
    }
}
