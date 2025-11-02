
namespace Survey_Basket
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependancies(this IServiceCollection services
            , IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers();
            services.AddHybridCache();
            services
                .AddSwaggerServices()
                .AddMapsterConfigrations()
                .AddFluentConfigrations()
                .AddAuthConfigrations(configuration)
                .AddCors(configuration)
                .AddHangfireConfigrations(configuration);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            var connectionString = configuration.GetConnectionString("DefaultConnection")??
                throw  new InvalidOperationException("connection string 'DefaultConnection' not found");

            services.AddDbContext<EntityContext>(options=>
                options.UseSqlServer(connectionString));

            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            services.AddHttpContextAccessor();

            //dependency Injection
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IEmailSender, MailSenderServices>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            //
            services.AddHealthChecks()
                .AddSqlServer(name: "Database", connectionString: connectionString, tags: ["database"])
                .AddSqlServer(name: "Database 2", connectionString: "test", tags: ["database"])
                .AddHangfire(option => { option.MinimumAvailableServers = 1; })
                .AddCheck<MailProviderHealthCheck>(name:"Mail Server");
            //.AddUrlGroup(name:"external api",uri:new Uri ("https://www.google.com"));

            services.AddRateLimiter(option => {
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                option.AddConcurrencyLimiter("concurrency",
                op =>
                {
                    op.PermitLimit = 1000;
                    op.QueueLimit = 100;
                    op.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
                //option.AddTokenBucketLimiter("token",
                //op =>
                //{
                //    op.TokenLimit = 2;
                //    op.QueueLimit = 1;
                //    op.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                //    op.TokensPerPeriod = 1;
                //    op.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
                //    op.AutoReplenishment = true;
                //}
                //option.AddFixedWindowLimiter("fixed",
                //op =>
                //{
                //    op.PermitLimit = 2;
                //    op.QueueLimit = 1;
                //    op.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                //    op.Window = TimeSpan.FromSeconds(30);
                //}
                //option.AddSlidingWindowLimiter("Sliding",
                //op =>
                //{
                //    op.PermitLimit = 2;
                //    op.QueueLimit = 1;
                //    op.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                //    op.Window = TimeSpan.FromSeconds(30);
                //    op.SegmentsPerWindow = 2;
                //}
                option.AddPolicy("iplimit", httpcontext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpcontext.Connection.RemoteIpAddress?.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(20)
                        //QueueLimit = 2,
                        //QueueProcessingOrder = QueueProcessingOrder.OldestFirst

                    }));
            option.AddPolicy("userlimit", httpcontext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpcontext.User.GetUserId(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 2,
                    Window = TimeSpan.FromSeconds(20)
                    //QueueLimit = 2,
                    //QueueProcessingOrder = QueueProcessingOrder.OldestFirst

                }
                )
            );
            });

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
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
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<EntityContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

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
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });
            return services;
        }
        private static IServiceCollection AddHangfireConfigrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));
                    services.AddHangfireServer();

            return services;
        }
    }
}
