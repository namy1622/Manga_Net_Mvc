
using System.Net;
using App.Data;
using Manga.Models;
using App.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Manga.Data;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();

// cau hinh chuoi ket noi den SqlServer
builder.Services.AddDbContext<MangaContext>(options =>
{
    //string connectString = builder.Configuration.GetConnectionString("AppMvcConnectionString");
    options.UseSqlServer(builder.Configuration.GetConnectionString("Web_Manga"));
});
// Đăng ký dịch vụ của bạn, ví dụ:
builder.Services.AddControllersWithViews();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDistributedMemoryCache(); // Thêm bộ nhớ cache phân phối (Distributed Cache)
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Đặt thời gian hết hạn của session
});

//---
builder.Services.AddRazorPages();

//---
builder.Services.AddHttpClient();
//--
builder.Services.AddMemoryCache();
builder.Services.AddHostedService<PreloadService>();


//--- Dang Ki Lop Nhu 1 Dich Vu -------------------------------
// builder.Services.AddSingleton<ProductService>();
// cach khac
// builder.Services.AddSingleton<ProductService, ProductService>();
// builder.Services.AddSingleton(typeof(ProductService));
// builder.Services.AddSingleton(typeof(ProductService), typeof(ProductService));

// builder.Services.AddSingleton<PlanetService>();
//----------------------------------------------------------
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    // /View/Controller/Action.cshtml
    // /MyView/Controller/Action.cshtml

    // {0} -> ten Action
    // {1} -> ten Controller
    // {2} -> ten Area

    // app tu dong build theo duong dan mac dinh la /View/Controller/Action.cshtml
    // neu ko co se chuyen huowng tim theo /MyView/...
    options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);
    // + RazorViewEngine.ViewExtension : la cac duoi nhu .cs .cshtml,...

});
//---------------------------------------------------------

//-------
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("ViewManageMenu", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.RequireClaim(RoleName.Administrator);
    });
});

// dang ki Identity
builder.Services.AddIdentity<MangaUser, IdentityRole>()
    .AddEntityFrameworkStores<MangaContext>()
    .AddDefaultTokenProviders();

//---------------
// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{

    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;
});

//-------------------------------------------------------
//**********************************************************
//
// cấu hinhf cài đặt Email
builder.Services.AddOptions();
var mailsetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsetting);
builder.Services.AddSingleton<IEmailSender, SendMailService>();

// Đăng ký IActionContextAccessor
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
// builder.Services.AddTransient<AdminSidebarService>();
//
// gọi dv google,facebook
builder.Services.AddAuthentication()
        // theem provider từ google
        .AddGoogle(options =>
        {
            var gconfig = builder.Configuration.GetSection("Authentication:Google");
#pragma warning disable CS8601 // Possible null reference assignment.
            options.ClientId = gconfig["ClientId"];
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
            options.ClientSecret = gconfig["ClientSecret"];
#pragma warning restore CS8601 // Possible null reference assignment.
            // https://localhost:5001/signin-google
            options.CallbackPath = "/dang-nhap-tu-google";
        })
         // thêm provider từ FB
         .AddFacebook(fb_options =>
         {
             var fbAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");
             fb_options.AppId = fbAuthNSection["AppId"];
             fb_options.AppSecret = fbAuthNSection["AppSecret"];
             fb_options.CallbackPath = "/dang-nhap-tu-facebook";
         })

        // .AddTwitter() // thêm provider từ TW
        // ...        
        ;

builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();
//---------------

var app = builder.Build();

// IWebHostEnvironment envProgram;


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseSession(); // Đảm bảo gọi UseSession trước các middleware khác

//-------------

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // xac dinh danh tinhs 
app.UseAuthorization(); // xac thuc quyen truy cap 

// app.AddStatusCodePage(); // tuy bien response khi cos loi tu 400 - 599

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoint =>
{

    endpoint.MapControllers();
    // Đặt trang splash làm mặc định
    // endpoint.MapControllerRoute(
    //     name: "default",
    //     pattern: "{area=Manga}/{controller=Home}/{action=Splash}/{id?}");

    // endpoint.MapAreaControllerRoute(
    //     name: "default",
    //     pattern: "/{controller=Home}/{action=Splash}/{id?}",
    //     areaName:"Manga");
        

    // endpoint.MapControllerRoute(
    //     name: "default",
    //     pattern: "{area=Identity}/{controller=User}/{action=Index}/{id?}");

    
 endpoint.MapControllerRoute(
        name: "default",
        pattern: "{area=Manga}/{controller=Home}/{action=Index}/{id?}");

    //Area
        // endpoint.MapAreaControllerRoute(
        //     name: "product",
        //     pattern: "/{controller=User}/{action=Index}/{id?}",
        //     areaName: "Identity"
        // );
// endpoint.MapControllerRoute(
//         name: "default",
//         pattern: "Identity/{controller=Home}/{action=Index}/{id?}");

        endpoint.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
});
#pragma warning restore ASP0014 // Suggest using top level route registrations




app.Run();

//========================================================
/*
    endpoint.MapControllers
    endpoint.MapControllerRoute
    endpoint.MapDefaultControllerRoute
    endpoint.MapAreaControllerRoute

    * Cac Attribute
    [AcceptVerbs] - duoc dung cho cac action 

    [Route]

    [HttpGet]
    [HttpPost]
    [HttpPut]
    [HttpDelete]
    [HttpHead]
    [HttpPatch]
*/
