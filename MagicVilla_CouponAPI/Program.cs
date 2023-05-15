using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//////////////////////////////////////////////////////////////

app.MapGet("/api/coupons", (IMapper _mapper, ILogger<Program> _logger) =>
{
    if (CouponStore.CouponList.Count == 0)
    {
        return Results.NotFound();
    }

    // Happy flow:
    _logger.Log(LogLevel.Information, "Now retrieving all coupons...");
    List<Coupon> coupons = CouponStore.CouponList;
    IEnumerable<CouponDto> cds = coupons.Select(c => _mapper.Map<CouponDto>(c));
    return Results.Ok(cds);
})
    .WithName("GetCoupons")
    .Produces<CouponDto>(200)
    .Produces(404);

///////////////////////////////////////////////////////////////

app.MapGet("/api/coupons/{id:int}", (IMapper _mapper, int id, ILogger<Program> _logger) =>
{
    if (CouponStore.CouponList.FirstOrDefault(c => c.Id == id) == null)
    {
        return Results.NotFound();
    }

    //Happy flow:
    _logger.Log(LogLevel.Information, $"Now retrieving coupon {id}...");
    Coupon coupon = CouponStore.CouponList.FirstOrDefault(c => c.Id == id);
    CouponDto cd = _mapper.Map<CouponDto>(coupon);
    return Results.Ok(cd);
})
    .WithName("GetCoupon")
    .Produces<CouponDto>(200)
    .Produces(404);

/////////////////////////////////////////////////////////////////

app.MapPost("/api/coupons", async (IMapper _mapper, ILogger<Program> _logger, IValidator<CouponInputDto> _validator, [FromBody] CouponInputDto cid) =>
{

    ValidationResult validationResult = await _validator.ValidateAsync(cid);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }
    if (CouponStore.CouponList.FirstOrDefault(c => c.Name.ToLower() == cid.Name.ToLower()) != null)
    {
        return Results.BadRequest($"A coupon with name {cid.Name} already exists...");
    }

    //Happy flow:
    _logger.Log(LogLevel.Information, $"Now adding the new coupon...");
    Coupon coupon = _mapper.Map<Coupon>(cid);

    coupon.Id = CouponStore.CouponList.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
    _logger.Log(LogLevel.Information, $"The new coupon's ID will be {coupon.Id}.");
    coupon.Created = DateTime.Now;
    coupon.LastUpdated = coupon.Created;
    CouponStore.CouponList.Add(coupon);

    CouponDto couponDTO = _mapper.Map<CouponDto>(coupon);

    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id, couponDTO });
})
    .WithName("CreateCoupon")
    .Accepts<CouponInputDto>("application/json")
    .Produces<CouponDto>(201)
    .Produces(400);

///////////////////////////////////////////////////////////////////

app.MapPut("/api/coupons/{id}:int", async (IMapper _mapper, ILogger<Program> _logger, IValidator<CouponInputDto> _validator, [FromBody] CouponInputDto cid, int id) =>
{

    ValidationResult validationResult = await _validator.ValidateAsync(cid);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }
    if (CouponStore.CouponList.FirstOrDefault(c => c.Id == id) == null)
    {
        return Results.NotFound($"A coupon with id {id} does not exist");
    }

    //Happy flow:   
    _logger.Log(LogLevel.Information, $"Now updating coupon {id}...");
    Coupon coupon = CouponStore.CouponList.FirstOrDefault(c => c.Id == id);
    coupon.Name = cid.Name;
    coupon.Percent = cid.Percent;
    coupon.IsActive = cid.IsActive;
    coupon.LastUpdated = DateTime.Now;

    CouponDto cd = _mapper.Map<CouponDto>(coupon);

    return Results.Ok(cd);

})
    .WithName("UpdateCoupon")
    .Accepts<CouponInputDto>("application/json")
    .Produces<CouponDto>(200)
    .Produces(400).Produces(404);

///////////////////////////////////////////////////////////////////

app.MapDelete("/api/coupons/{id:int}", (int id) =>
{

    if (CouponStore.CouponList.FirstOrDefault(c => c.Id == id) == null)
    {
        return Results.NotFound("Coupon not found");
    }

    //Happy flow: 
    //_logger.Log(LogLevel.Information, $"Now deleting coupon {id}");
    Coupon coupon = CouponStore.CouponList.FirstOrDefault(c => c.Id == id);
    CouponStore.CouponList.Remove(coupon);

    return Results.NoContent();
})
    .WithName("DeleteCoupon")
    .Produces(204)
    .Produces(404);

////////////////////////////////////////////////////////////////////
app.UseHttpsRedirection();
app.Run();
