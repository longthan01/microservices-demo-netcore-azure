using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SM.Ecommerce.Api.Products.GraphQL.Schemas;
using SM.Ecommerce.Api.Products.Interfaces;
using SM.Ecommerce.Api.Products.Providers;
using GraphQL.Server.Ui.Playground;
using GraphQL;
using GraphQL.Types;
using GraphQL.Server;
using SM.Ecommerce.Api.Products.GraphQL.Queries;
using SM.Ecommerce.Api.Products.GraphQL.Types;
using GraphiQl;

namespace SM.Ecommerce.Api.Products
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AddDbContext<DataAccess.ProductsDbContext>(options =>
            {
                options.UseInMemoryDatabase("Products");
            }); 
            services.AddScoped<IProductsProvider, ProductsProvider>();
            services.AddControllers();
            // graphql configurations
            services.AddScoped<ISchema, ProductSchema>();
            services.AddScoped<ProductGraphQuery>();
            services.AddScoped<ProductGraphType>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddGraphQL(x =>
            {
                x.EnableMetrics = true;
                x.UnhandledExceptionDelegate = ctx => { };
            })
            .AddSystemTextJson(deserializerSettings => { }, serializerSettings => { })
            .AddDataLoader() // Add required services for DataLoader support
            .AddGraphTypes(typeof(ProductSchema));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            // graphql configurations
            app.UseGraphQL<ISchema>("/api/products");
            app.UseGraphiQl("/graphql", "/api/products");
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions()
            {
                Path = "/ui/playground"
            });
        }
    }
}
