using GameFlow.Data;

namespace GameFlow.Services.Date;

public class ActionValidityChecker
{
    private readonly DataContext _dataContext;
    
    public ActionValidityChecker(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public void CheckActionValidity()
    {
        var actions = _dataContext.Actions.Where(a => a.EndDate < DateTime.Now);
        IQueryable<Product>? products = null;
        if (actions.Any())
        {
            foreach (var action in actions)
            {
                products = _dataContext.Products.Where(p => p.ActionId == action.Id);
            }
            
            if (products != null)
            {
                foreach (var product in products)
                {
                    product.ActionId = null;
                }
                _dataContext.SaveChanges();
            }
        }
    }
}