using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopBridge.Data;
using ShopBridge.Helper;
using ShopBridge.Models;
using ShopBridge.Models.Enum;
using ShopBridge.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopBridge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<object> GetAllProducts(int pageNumber=1,int pageSize=3)
        {
            try
            {
                var products = _context.Products.AsNoTracking();
                var paginatedData = await PaginatedList<Product>.CreateAsync(products,pageNumber,pageSize);
                return await Task.FromResult(new ResponseModel(ResponseCode.OK, "", paginatedData));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, ex.Message, null));
            }
        }

        [HttpGet]
        [Route("GetProductByID/{id:int}")]
        public async Task<object> GetProductByID(int id)
        {
            try
            {
                if (id != 0)
                {
                    var product = _context.Products.Where(x => x.ID == id).FirstOrDefault();
                    return await Task.FromResult(new ResponseModel(ResponseCode.OK, "", product));
                }
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, "", ModelState.Values));


            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, ex.Message, null));
            }

        }

        [HttpPost]
        [Route("AddProduct")]
        public async Task<object> AddProduct([FromBody] ProductViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var product = new Product()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Price = model.Price,
                    };
                    await _context.Products.AddAsync(product);
                    await _context.SaveChangesAsync();
                    return await Task.FromResult(new ResponseModel(ResponseCode.OK, "Product has been saved successfully.", null));
                }
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, "", ModelState.Values));


            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, ex.Message, null));
            }

        }

        [HttpPut]
        [Route("UpdateProduct/{id:int}")]
        public async Task<object> UpdateProduct(int id,[FromBody] ProductViewModel model)
        {
            try
            {
                if (id != model.ID)
                {
                    return await Task.FromResult(new ResponseModel(ResponseCode.Error, "Data is not sent for editing.", null));
                }

                if (ModelState.IsValid)
                {
                    var product = new Product()
                    {
                        ID=model.ID,
                        Name = model.Name,
                        Description = model.Description,
                        Price = model.Price,
                    };
                     _context.Update(product);
                    await _context.SaveChangesAsync();
                    return await Task.FromResult(new ResponseModel(ResponseCode.OK, "Product has been updated successfully.", null));
                }
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, "", ModelState.Values));


            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, ex.Message, null));
            }

        }

        [HttpDelete]
        [Route("DeleteProduct/{id:int}")]
        public async Task<object> DeleteProduct(int id)
        {
            try
            {
                if (id!=0)
                {
                    var product = _context.Products.Where(x => x.ID == id).FirstOrDefault();
                    _context.Remove(product);
                    await _context.SaveChangesAsync();
                    return await Task.FromResult(new ResponseModel(ResponseCode.OK, "Product has been deleted successfully.", null));
                }
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, "", ModelState.Values));


            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, ex.Message, null));
            }

        }
    }
}
