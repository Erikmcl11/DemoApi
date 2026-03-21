using DemoApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using DemoApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

async Task<List<Contacto>> GetContactosAsync(DataContext context)=> await context.Contactos.ToListAsync();

// Endpoints nuevos
app.MapGet("/contactos", async (DataContext context) => {
    var contactos = await GetContactosAsync(context);
    return Results.Ok(contactos);
});

app.MapGet("/contactos/{id}", async (int id, DataContext context) => {
    var contacto = await context.Contactos.FindAsync(id);
    return contacto != null ? Results.Ok(contacto) : Results.NotFound();
});

app.MapPost("/contactos", async (Contacto contacto, DataContext context) => {
    context.Contactos.Add(contacto);
    await context.SaveChangesAsync();
    return Results.Created($"/contactos/{contacto.Id}", contacto);
});

app.MapPut("/contactos/{id}", async (int id, Contacto updatedContacto, DataContext context) => {
    var existingContacto = await context.Contactos.FindAsync(id);
    if (existingContacto is null) return Results.NotFound();

    existingContacto.Name = updatedContacto.Name;
    existingContacto.Lastname = updatedContacto.Lastname;
    existingContacto.Birthday = updatedContacto.Birthday;
    existingContacto.PhoneNumber = updatedContacto.PhoneNumber;
    existingContacto.Email = updatedContacto.Email;

    await context.SaveChangesAsync();
    return Results.Ok(existingContacto);
});

app.MapDelete("/contactos/{id}", async (int id, DataContext context) => {
    var contacto = await context.Contactos.FindAsync(id);
    if (contacto is null) return Results.NotFound();

    context.Contactos.Remove(contacto);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

