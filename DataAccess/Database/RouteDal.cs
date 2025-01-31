using DataAccess.Context;
using Interface.Interface.Dal;
using Interface.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Database;

public class RouteDal(MyDbContext context) : IRouteDal
{
    public async Task<int> CreateRoute(int userId)
    {
        var userModel = await context.Users.FindAsync(userId);
        if (userModel == null)
        {
            throw new Exception("User not found");
        }
        var route = new RouteModel
        {
            UserId = userId,
            UserModel = userModel,
        };
        context.Routes.Add(route);
        await context.SaveChangesAsync();
        return route.Id;
    }

    public async Task CreateDataPoint(DataPointModel[] dataPoints)
    {
        var route = await context.Routes.FindAsync(dataPoints[0].RouteId);
        if (route == null)
        {
            throw new Exception("Route not found");
        }
        dataPoints.ToList().ForEach(x => x.RouteModel = route);
        context.DataPoints.AddRange(dataPoints);
        await context.SaveChangesAsync();
    }

    public async Task<double> GetMaxSpeed(int routeId)
    {
        var route = await context.Routes.Include(r => r.DataPoints).FirstOrDefaultAsync(r => r.Id == routeId);
        if (route == null)
        {
            throw new Exception("Route not found");
        }
        return (double)route.DataPoints.Max(dp => dp.Speed);
    }

    public async Task<double> GetMaxLean(int routeId)
    {
        var route = await context.Routes.Include(r => r.DataPoints).FirstOrDefaultAsync(r => r.Id == routeId);
        if (route == null)
        {
            throw new Exception("Route not found");
        }
        return (double)route.DataPoints.Max(dp => dp.Lean);
    }

    public async Task<double> GetMaxG(int routeId)
    {
        var route = await context.Routes.Include(r => r.DataPoints).FirstOrDefaultAsync(r => r.Id == routeId);
        if (route == null)
        {
            throw new Exception("Route not found");
        }
        var maxLateralG = (double)route.DataPoints.Max(dp => dp.LateralG);
        var maxAcceleration = (double)route.DataPoints.Max(dp => dp.Acceleration);
        
        return Math.Max(maxLateralG, maxAcceleration);
    }

    public async Task<RouteModel> GetRoute(int routeId)
    {
        var route = await context.Routes.Include(r => r.DataPoints).FirstOrDefaultAsync(r => r.Id == routeId);
        if (route == null)
        {
            throw new Exception("Route not found");
        }
        return route;
    }

    public async Task DeleteRoute(int routeId)
    {
        var route = await context.Routes.FindAsync(routeId);
        if (route == null)
        {
            throw new Exception("Route not found");
        }
        context.Routes.Remove(route);
        await context.SaveChangesAsync();
    }
}