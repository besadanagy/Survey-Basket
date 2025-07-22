
namespace Survey_Basket
{
    public static class DependencyInjection
    {
        

        public static IServiceCollection AddDependancies(this IServiceCollection services
            , IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers();
            
            services
                .AddSwaggerServices()
                .AddMapsterConfigrations()
                .AddFluentConfigrations()
                .AddAuthConfigrations(configuration)
                .AddCors(configuration);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            services.AddDbContext<EntityContext>(options=>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            //dependency Injection
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            //


            return services;
        }
        private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }
        private static IServiceCollection AddCors(this IServiceCollection services,IConfiguration configuration)
        {
            var AllowOrigins = configuration.GetSection("AllowOrigins").Get<string[]>();

            services.AddCors(
                option =>
                {
                option.AddPolicy("FirstPolicy", builder =>
                builder.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins(AllowOrigins!)
                );
                option.AddPolicy("SecondPolicy", builder =>
                builder.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins(AllowOrigins!)
                );
                });
            return services;
        }
        private static IServiceCollection AddMapsterConfigrations(this IServiceCollection services)
        {
            //add mapster
            //services.AddMapster();
            var mappingconfig = TypeAdapterConfig.GlobalSettings;
            mappingconfig.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton<IMapper>(new Mapper(mappingconfig));

            return services;
        }
        private static IServiceCollection AddFluentConfigrations(this IServiceCollection services)
        {
            services
                .AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
        private static IServiceCollection AddAuthConfigrations(this IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<EntityContext>();
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();
            var JwtSetting = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
            services.AddSingleton<IJwtProvider, JwtProvider>();
            services.AddAuthentication(option => { 
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o => {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSetting!.Key)),
                    ValidIssuer = JwtSetting.issuer,
                    ValidAudience =JwtSetting.audience                    
                };
                })
            ;

            return services;
        }

    }
}
