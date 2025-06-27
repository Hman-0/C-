using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Data;
using ProductCatalogApi.DTOs;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductCatalogContext _context;
        private readonly IMapper _mapper;
        public ProductsController(ProductCatalogContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm với phân trang và lọc
        /// </summary>
        /// <param name="name">Tên sản phẩm (tìm kiếm gần đúng)</param>
        /// <param name="category">Danh mục sản phẩm</param>
        /// <param name="minPrice">Giá tối thiểu</param>
        /// <param name="maxPrice">Giá tối đa</param>
        /// <param name="page">Trang hiện tại (mặc định: 1)</param>
        /// <param name="pageSize">Số sản phẩm mỗi trang (mặc định: 10, tối đa: 100)</param>
        /// <param name="sortBy">Sắp xếp theo (name, price, stock, created)</param>
        /// <param name="sortOrder">Thứ tự sắp xếp (asc, desc)</param>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<ProductDto>>> GetAll(
            [FromQuery] string? name,
            [FromQuery] string? category,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "name",
            [FromQuery] string sortOrder = "asc")
        {
            // Validate pagination parameters
            page = Math.Max(1, page);
            pageSize = Math.Min(100, Math.Max(1, pageSize));

            var query = _context.Products.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(x => x.Name.ToLower().Contains(name.ToLower()));
            
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(x => x.Category.ToLower() == category.ToLower());
            
            if (minPrice.HasValue)
                query = query.Where(x => x.Price >= minPrice);
            
            if (maxPrice.HasValue)
                query = query.Where(x => x.Price <= maxPrice);

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "price" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
                "stock" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(x => x.Stock) : query.OrderBy(x => x.Stock),
                "created" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
                _ => sortOrder.ToLower() == "desc" ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name)
            };

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResultDto<ProductDto>
            {
                Data = _mapper.Map<List<ProductDto>>(products),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, _mapper.Map<ProductDto>(product));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            _mapper.Map(dto, product);
            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Bán sản phẩm - giảm số lượng tồn kho
        /// </summary>
        /// <param name="id">ID sản phẩm</param>
        /// <param name="sellDto">Thông tin bán hàng</param>
        [HttpPost("{id}/sell")]
        public async Task<IActionResult> Sell(int id, [FromBody] SellProductDto sellDto)
        {
            if (sellDto.Quantity <= 0)
                return BadRequest("Số lượng phải lớn hơn 0");

            var product = await _context.Products.FindAsync(id);
            if (product == null) 
                return NotFound($"Không tìm thấy sản phẩm với ID: {id}");
            
            if (product.Stock < sellDto.Quantity) 
                return BadRequest($"Không đủ tồn kho! Hiện có: {product.Stock}, yêu cầu: {sellDto.Quantity}");
            
            product.Stock -= sellDto.Quantity;
            product.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return Ok(new 
            {
                message = $"Đã bán {sellDto.Quantity} sản phẩm thành công",
                product = _mapper.Map<ProductDto>(product),
                note = sellDto.Note
            });
        }

        /// <summary>
        /// Báo cáo tổng giá trị kho hàng chi tiết
        /// </summary>
        [HttpGet("report/stock-value")]
        public async Task<ActionResult<StockReportDto>> GetStockValue()
        {
            var products = await _context.Products.ToListAsync();
            
            var totalValue = products.Sum(x => x.Price * x.Stock);
            var totalProducts = products.Count;
            var totalStock = products.Sum(x => x.Stock);
            
            var categoryBreakdown = products
                .GroupBy(x => x.Category)
                .Select(g => new CategoryStockDto
                {
                    Category = g.Key,
                    TotalValue = g.Sum(x => x.Price * x.Stock),
                    ProductCount = g.Count(),
                    TotalStock = g.Sum(x => x.Stock)
                })
                .OrderByDescending(x => x.TotalValue)
                .ToList();
            
            var report = new StockReportDto
            {
                TotalValue = totalValue,
                TotalProducts = totalProducts,
                TotalStock = totalStock,
                CategoryBreakdown = categoryBreakdown
            };
            
            return Ok(report);
        }

        /// <summary>
        /// Lấy danh sách các danh mục sản phẩm
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            var categories = await _context.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
            
            return Ok(categories);
        }

        /// <summary>
        /// Kiểm tra tồn kho của sản phẩm
        /// </summary>
        /// <param name="id">ID sản phẩm</param>
        [HttpGet("{id}/stock")]
        public async Task<IActionResult> CheckStock(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) 
                return NotFound($"Không tìm thấy sản phẩm với ID: {id}");
            
            return Ok(new 
            {
                productId = id,
                productName = product.Name,
                currentStock = product.Stock,
                stockStatus = product.Stock > 0 ? "Còn hàng" : "Hết hàng",
                lowStock = product.Stock < 10
            });
        }
    }
}