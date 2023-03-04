using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace AutoFindBot.Web.Configuration.Swagger
{
	public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
	{
		private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

		public ConfigureSwaggerOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider) =>
			_apiVersionDescriptionProvider = apiVersionDescriptionProvider;

		public void Configure(SwaggerGenOptions options)
		{
			foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
			{
				options.SwaggerDoc(description.GroupName, new OpenApiInfo
				{
					Title = $"Application API {description.GroupName}",
					Version = description.ApiVersion.ToString(),
					Description = "API сервиса создания заявок на подключение пакетов услуг",
				});
			}
		}
	}
}