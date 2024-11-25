using BTL_WebManga.Controllers;
using BTL_WebManga.Services;

var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ vào container
builder.Services.AddControllersWithViews();

// Đăng ký HttpClient
builder.Services.AddHttpClient();
builder.Services.AddScoped<CategoryService>();

var app = builder.Build();

// Cấu hình request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
