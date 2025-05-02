using Microsoft.EntityFrameworkCore;
using ProductConsoleAPI.Business;
using ProductConsoleAPI.Business.Contracts;
using ProductConsoleAPI.Data.Models;
using ProductConsoleAPI.DataAccess;
using System.ComponentModel.DataAnnotations;

namespace ProductConsoleAPI.IntegrationTests.NUnit
{
    public  class IntegrationTests
    {
        private TestProductsDbContext dbContext;
        private IProductsManager productsManager;

        [SetUp]
        public void SetUp()
        {
            this.dbContext = new TestProductsDbContext();
            this.productsManager = new ProductsManager(new ProductsRepository(this.dbContext));
        }


        [TearDown]
        public void TearDown()
        {
            this.dbContext.Database.EnsureDeleted();
            this.dbContext.Dispose();
        }


        //positive test
        [Test]
        public async Task AddProductAsync_ShouldAddNewProduct()
        {
            var newProduct = new Product()
            {
                OriginCountry = "Bulgaria",
                ProductName = "TestProduct",
                ProductCode = "AB12C",
                Price = 1.25m,
                Quantity = 100,
                Description = "Anything for description"
            };
            await productsManager.AddAsync(newProduct);

            var dbProduct = await this.dbContext.Products.FirstOrDefaultAsync(p => p.ProductCode == newProduct.ProductCode);

            Assert.NotNull(dbProduct);
            Assert.That(dbProduct.ProductName, Is.EqualTo(newProduct.ProductName));
            Assert.That(dbProduct.Description, Is.EqualTo(newProduct.Description));
            Assert.That(dbProduct.Price, Is.EqualTo(newProduct.Price));
            Assert.That(dbProduct.Quantity, Is.EqualTo(newProduct.Quantity));
            Assert.That(dbProduct.OriginCountry, Is.EqualTo(newProduct.OriginCountry));
            Assert.That(dbProduct.ProductCode, Is.EqualTo(newProduct.ProductCode));
        }

        //Negative test
        [Test]
        public async Task AddProductAsync_TryToAddProductWithInvalidCredentials_ShouldThrowException()
        {
            var newProduct = new Product()
            {
                OriginCountry = "Bulgaria",
                ProductName = "TestProduct",
                ProductCode = "AB12C",
                Price = -1m,    // невалидна цена на продукта
                Quantity = 100,
                Description = "Anything for description"
            };

            var ex = Assert.ThrowsAsync<ValidationException>(async () => await productsManager.AddAsync(newProduct));
            var actual = await dbContext.Products.FirstOrDefaultAsync(c => c.ProductCode == newProduct.ProductCode);

            Assert.IsNull(actual);
            Assert.That(ex?.Message, Is.EqualTo("Invalid product!"));

        }

        [Test]
        public async Task DeleteProductAsync_WithValidProductCode_ShouldRemoveProductFromDb()
        {
            // Arrange
            var newProduct = new Product()
            {
                OriginCountry = "Bulgaria",
                ProductName = "TestProduct",
                ProductCode = "AB12C",
                Price = 1.25m,
                Quantity = 100,
                Description = "Anything for description"
            };

            await productsManager.AddAsync(newProduct);

            // Act
            await productsManager.DeleteAsync(newProduct.ProductCode);

            // Assert
            var productInDb = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductCode == newProduct.ProductCode);

            Assert.IsNull(productInDb);  
        }

        
        [Test]
        public async Task DeleteProductAsync_TryToDeleteWithNullOrWhiteSpaceProductCode_ShouldThrowException()
        {

            // Act
         var exception = Assert.ThrowsAsync<ArgumentException>(() => productsManager.DeleteAsync(null));
         var exception2 = Assert.ThrowsAsync<ArgumentException>(() => productsManager.DeleteAsync("  "));

            // Assert
          Assert.That(exception.Message, Is.EqualTo("Product code cannot be empty."));
          Assert.That(exception2.Message, Is.EqualTo("Product code cannot be empty."));
        }

        [Test]
        public async Task GetAllAsync_WhenProductsExist_ShouldReturnAllProducts()
        {
            // Arrange
            var firstProduct = new Product()
            {
                OriginCountry = "Bulgaria",
                ProductName = "TestProduct",
                ProductCode = "AB12C",
                Price = 1.25m,
                Quantity = 100,
                Description = "Anything for description"
            };

            await productsManager.AddAsync(firstProduct);

            var secondProduct = new Product()
            {
                OriginCountry = "Russia",
                ProductName = "TestProduct2",
                ProductCode = "AB542C",
                Price = 1.10m,
                Quantity = 15,
                Description = "Some description"
            };
            await productsManager.AddAsync(secondProduct);

            // Act
            var result = await productsManager.GetAllAsync();
     
            // Assert
            Assert.That(result.Count(),Is.EqualTo(2));  // В резултат има 2 продукта

          // взимаме първия продукт
          var firstItem = result.FirstOrDefault(p => p.ProductCode == firstProduct.ProductCode);
            Assert.NotNull(firstItem);   
    
            Assert.That(firstItem.OriginCountry, Is.EqualTo(firstProduct.OriginCountry));
            Assert.That(firstItem.ProductName, Is.EqualTo(firstProduct.ProductName));
            Assert.That(firstItem.ProductCode, Is.EqualTo(firstProduct.ProductCode));
            Assert.That(firstItem.Price, Is.EqualTo(firstProduct.Price));
            Assert.That(firstItem.Quantity, Is.EqualTo(firstProduct.Quantity));
            Assert.That(firstItem.Description, Is.EqualTo(firstProduct.Description));
        }

        [Test]
        public async Task GetAllAsync_WhenNoProductsExist_ShouldThrowKeyNotFoundException()
        {

            // Act
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => productsManager.GetAllAsync());

            // Assert
              Assert.That(exception.Message, Is.EqualTo($"No product found."));   
        }

        [Test]
        public async Task SearchByOriginCountry_WithExistingOriginCountry_ShouldReturnMatchingProducts()
        {
            // Arrange
            var newProduct = new Product()
            {
                OriginCountry = "Bulgaria",
                ProductName = "TestProduct",
                ProductCode = "AB12C",
                Price = 1.25m,
                Quantity = 100,
                Description = "Anything for description"
            };

            await productsManager.AddAsync(newProduct);

            // Act
            var result = await productsManager.SearchByOriginCountry(newProduct.OriginCountry);   

            // Assert
            Assert.NotNull(result);

            // взимам продукта, за проверя всяко негово пропърти дали правилно се е запазило
            var resultProduct = result.First(); 
            
            Assert.That(resultProduct.OriginCountry,Is.EqualTo (newProduct.OriginCountry)); 
            Assert.That(resultProduct.ProductName, Is.EqualTo (newProduct.ProductName)); 
            Assert.That(resultProduct.ProductCode, Is.EqualTo (newProduct.ProductCode)); 
            Assert.That(resultProduct.Price, Is.EqualTo (newProduct.Price)); 
            Assert.That(resultProduct.Quantity, Is.EqualTo (newProduct.Quantity)); 
            Assert.That(resultProduct.Description, Is.EqualTo (newProduct.Description)); 
        }

        [Test]
        public async Task SearchByOriginCountryAsync_WithNonExistingOriginCountry_ShouldThrowKeyNotFoundException()
        {
            // Act
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => productsManager.SearchByOriginCountry("NonExistingOriginCountry"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No product found with the given country of origin."));
        }

        [Test]
        public async Task GetSpecificAsync_WithValidProductCode_ShouldReturnProduct()
        {
            // Arrange
            var newProduct = new Product()
            {
                OriginCountry = "Bulgaria",
                ProductName = "TestProduct",
                ProductCode = "AB12C",
                Price = 1.25m,
                Quantity = 100,
                Description = "Anything for description"
            };

            await productsManager.AddAsync(newProduct);

            // Act

           var result = await productsManager.GetSpecificAsync(newProduct.ProductCode);    

            // Assert
            Assert.NotNull(result);

            Assert.That(result.OriginCountry, Is.EqualTo(newProduct.OriginCountry));
            Assert.That(result.ProductName, Is.EqualTo(newProduct.ProductName));
            Assert.That(result.ProductCode, Is.EqualTo(newProduct.ProductCode));
            Assert.That(result.Price, Is.EqualTo(newProduct.Price));
            Assert.That(result.Quantity, Is.EqualTo(newProduct.Quantity));
            Assert.That(result.Description, Is.EqualTo(newProduct.Description));
        }

        [Test]
        public async Task GetSpecificAsync_WithInvalidProductCode_ShouldThrowKeyNotFoundException()
        {
            //Arrange
            const string invalidProductCode = "InvalidCode";

            // Act
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => productsManager.GetSpecificAsync(invalidProductCode));

            // Assert
            Assert.That(exception.Message, Is.EqualTo($"No product found with product code: {invalidProductCode}"));
        }

        [Test]
        public async Task UpdateAsync_WithValidProduct_ShouldUpdateProduct()
        {
            // Arrange
            var firstProduct = new Product()
            {
                OriginCountry = "Bulgaria",
                ProductName = "TestProduct",
                ProductCode = "AB12C",
                Price = 1.25m,
                Quantity = 100,
                Description = "Anything for description"
            };

            await productsManager.AddAsync(firstProduct);

            var secondProduct = new Product()
            {
                OriginCountry = "Russia",
                ProductName = "TestProduct2",
                ProductCode = "AB542C",
                Price = 1.10m,
                Quantity = 15,
                Description = "Some description"
            };
            await productsManager.AddAsync(secondProduct);
            // Act
            secondProduct.ProductName = "UPDATED NAME!";
            await productsManager.UpdateAsync(secondProduct); 

            // Assert
           
           var productInDb = await productsManager.GetSpecificAsync(secondProduct.ProductCode);

            Assert.That(productInDb.OriginCountry, Is.EqualTo(secondProduct.OriginCountry));
            Assert.That(productInDb.ProductName, Is.EqualTo(secondProduct.ProductName));
            Assert.That(productInDb.ProductCode, Is.EqualTo(secondProduct.ProductCode));
            Assert.That(productInDb.Price, Is.EqualTo(secondProduct.Price));
            Assert.That(productInDb.Quantity, Is.EqualTo(secondProduct.Quantity));
            Assert.That(productInDb.Description, Is.EqualTo(secondProduct.Description));
        }

        [Test]
        public async Task UpdateAsync_WithInvalidProduct_ShouldThrowValidationException()
        {
            // Arrange
            var invalidProduct = new Product()
            {
                OriginCountry = "Bulgaria",
                ProductName = "TestProduct",
                ProductCode = "AB12C",
                Price = -100m,
                Quantity = 100,
                Description = "Anything for description"
            };

            // await productsManager.AddAsync(invalidProduct);
            // Act
            var exception = Assert.ThrowsAsync<ValidationException>(() => productsManager.UpdateAsync(invalidProduct));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid product!"));
        }
    }
}
