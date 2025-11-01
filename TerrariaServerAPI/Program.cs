using TerrariaServerSystem;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(lb => {
    lb.AddConsole();
    lb.AddFilter(f => true);
});
builder.Services.AddSingleton<ServerManager>(); //添加管理器为单例
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy
            .SetIsOriginAllowed(d => true)
            //.AllowCredentials()
            .AllowAnyOrigin()    // 允许任何来源
            .AllowAnyMethod()    // 允许任何 HTTP 方法（GET、POST、PUT 等）
            .AllowAnyHeader();   // 允许任何请求头
        ;
    });
});
var app = builder.Build();
app.Urls.Add("http://127.0.0.1:3000");
app.Urls.Add("http://0.0.0.0:3000");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
