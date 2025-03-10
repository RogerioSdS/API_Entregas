var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient(); // Para usar HttpClient
builder.Services.AddSession(); // Para usar sessões
builder.Services.AddControllersWithViews(); // Para Razor views
builder.Services.AddRazorPages(); // Opcional, se usar páginas Razor

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Opcional, para autenticação
app.UseAuthorization();
app.UseSession(); // Habilita sessões

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();