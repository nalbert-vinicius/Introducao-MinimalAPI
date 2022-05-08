using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
   opt.UseInMemoryDatabase("TarefasDB")
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.MapGet("/tarefas", async (AppDbContext db) =>
{
    return await db.Tarefas.ToListAsync();
});

app.MapGet("/tarefas/{id}", async (int id, AppDbContext db) =>{
    var tarefas = await db.Tarefas.FirstOrDefaultAsync(t => t.Id == id);
    if (tarefas != null) {
        return Results.Ok(tarefas);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapGet("/tarefas/concluida", async (AppDbContext db) =>
{
    var tarefas = await db.Tarefas.FirstOrDefaultAsync(t => t.IsConcluido == true);
    if (tarefas != null)
    {
        return Results.Ok(tarefas);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapPost("/tarefas", async (Tarefa tarefa, AppDbContext db) =>
{
    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();
    return Results.Created($"/tarefas/{ tarefa.Id }", tarefa);
});

app.Run();

class Tarefa
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public bool IsConcluido { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
}