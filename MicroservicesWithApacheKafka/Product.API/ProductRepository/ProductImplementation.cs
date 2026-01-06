using ApplicationDataContext.DataBaseContext;
using Azure.Core.Serialization;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Product.API.DTOs.ResponseDTOs;
using Shared.DTOs.ProductDtos;
using Shared.Mapper;
using System.Text.Json;

namespace Product.API.ProductRepository
{
    public class ProductImplementation : IProductService
    {
        private const string ADD_PRODUCT_TOPIC = "Add-Product-Topic";
        private const string GET_PRODUCT_TOPIC = "Get-Product-Topic";
        private const string GET_PRODUCTS_TOPIC = "Get-Products-Topic";
        private const string DELETE_PRODUCT_TOPIC = "Delete-Product-Topic";
        private const string UPDATE_PRODUCT_TOPIC = "Update-Product-Topic";

        private readonly ProductDbContext _productDbContext;
        private readonly IProducer<Null, string> _producer;

        public ProductImplementation(ProductDbContext productDbContext, IProducer<Null, string> producer)
        {
            this._productDbContext = productDbContext;
            this._producer = producer;
        }

        public async Task<ResponseDto> CreateProductAsync(CreateProductDto newProductDto)
        {
            if (newProductDto is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product is null"
                };
            }

            ProductDTO productDto = new()
            {
                //Id = newProductDto.Id,
                Name = newProductDto.Name,
                Price = newProductDto.Price
            };

            var newProduct = productDto.ConvertProductDtoToProductExtension();

            /* Check if the new product is already existing in the database */
            var existingProduct = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.Name == newProduct.Name && product.Price == newProduct.Price);

            if (existingProduct is not null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product already exists"
                };
            }

            /* Send the new product to the kafka topic */
            var deliveryResult = await this._producer.ProduceAsync(topic: ADD_PRODUCT_TOPIC, message: new Message<Null, string>()
            {
                Value = JsonSerializer.Serialize(newProduct)
            });

            if (deliveryResult.Status is PersistenceStatus.NotPersisted)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product is not added!"
                };
            }

            await this._productDbContext.Products.AddAsync(newProduct);
            await this._productDbContext.SaveChangesAsync();

            var addedNewProductDto = newProduct.ConvertProductToProductDtoExtension();

            return new ResponseDto()
            {
                Result = addedNewProductDto,
                IsSuccess = true,
                Message = "New product is added!"
            };
        }

        public async Task<ResponseDto> DeleteProductAsync(int productId)
        {
            if (productId <= 0)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Invalid Product Id"
                };
            }

            var product = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.Id == productId);

            if (product is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product not found"
                };
            }

            this._productDbContext.Remove(product);
            await this._productDbContext.SaveChangesAsync();

            /* Publish product deletion event to Kafka for audit/logging */
            await PublishProductEventAsync(topic: DELETE_PRODUCT_TOPIC, message: new
            {
                EventType = "ProductDeleted",
                ProductId = productId,
                Timestamp = DateTime.UtcNow
            });

            return new ResponseDto()
            {
                Result = null,
                IsSuccess = true,
                Message = "Product is deleted!"
            };
        }

        public async Task<ResponseDto> GetProductByIdAsync(int productId)
        {
            if (productId <= 0)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Invalid Product Id"
                };
            }

            var product = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.Id == productId);

            if (product is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product not found"
                };
            }

            var productDto = product.ConvertProductToProductDtoExtension();

            /* Publish product retrieval event to Kafka for audit/logging */
            await PublishProductEventAsync(topic: GET_PRODUCT_TOPIC, message: new
            {
                EventType = "ProductRetrieved",
                ProductId = productId,
                Timestamp = DateTime.UtcNow
            });

            return new ResponseDto()
            {
                Result = productDto,
                IsSuccess = true,
                Message = "Product is found!",
            };
        }

        public async Task<ResponseDto> GetProductsAsync()
        {
            var products = await this._productDbContext.Products.ToListAsync();

            if (products is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Products not found"
                };
            }

            IList<ProductDTO> productsDto = [];

            foreach (var product in products)
            {
                var productDto = product.ConvertProductToProductDtoExtension();

                productsDto.Add(item: productDto);
            }

            /* Publish all products retrieval event to Kafka for audit/logging */
            await PublishProductEventAsync(topic: GET_PRODUCTS_TOPIC, message: new
            {
                EventType = "AllProductsRetrieved",
                ProductCount = productsDto.Count,
                Timestamp = DateTime.UtcNow
            });

            return new ResponseDto()
            {
                Result = productsDto,
                IsSuccess = true,
                Message = "Success"
            };
        }

        public async Task<ResponseDto> UpdateProductByIdAsync(int productId, UpdateProductDto updatedProductDto)
        {
            if (updatedProductDto is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product is null"
                };
            }

            if (productId <= 0)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Invalid Product Id"
                };
            }

            var fetchedProduct = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.Id == productId);

            if (fetchedProduct is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product not found"
                };
            }

            if (fetchedProduct.Name == updatedProductDto.Name && fetchedProduct.Price == updatedProductDto.Price)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product is not updated!"
                };
            }

            /* Send the updated product to the kafka topic */
            var deliveryResult = await this._producer.ProduceAsync(topic: UPDATE_PRODUCT_TOPIC, message: new Message<Null, string>()
            {
                Value = JsonSerializer.Serialize(updatedProductDto)
            });

            if (deliveryResult.Status is PersistenceStatus.NotPersisted)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product is not updated!"
                };
            }

            fetchedProduct?.Name = updatedProductDto.Name;
            fetchedProduct?.Price = updatedProductDto.Price;

            await this._productDbContext.SaveChangesAsync();

            var updatedProductAddedToDatabaseDto = fetchedProduct?.ConvertProductToProductDtoExtension();

            return new ResponseDto()
            {
                Result = updatedProductAddedToDatabaseDto,
                IsSuccess = true,
                Message = "Product is updated!"
            };
        }

        private async Task PublishProductEventAsync(string topic, object message)
        {
            try
            {
                await this._producer.ProduceAsync(topic: topic, message: new Message<Null, string>()
                {
                    Value = JsonSerializer.Serialize(message)
                });
            }
            catch (Exception ex)
            {
                /* Log error but don't fail the request since read operations shouldn't be blocked by Kafka issues */
                System.Diagnostics.Debug.WriteLine($"Failed to publish event to {topic}: {ex.Message}");
            }
        }
    }
}
