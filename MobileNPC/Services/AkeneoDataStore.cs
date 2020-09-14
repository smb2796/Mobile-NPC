﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akeneo;
using Akeneo.Client;
using Akeneo.Model;
using Akeneo.Search;
using MobileNPC.Models;

namespace MobileNPC.Services
{
    public class AkeneoDataStore : IDataStore<Item>
    {
        private readonly IAkeneoClient _client;
        private readonly IEnumerable<string> _categories;

        public AkeneoDataStore()
        {
            _categories = App.AkeneoConfig.Categories;
            var options = new AkeneoOptions
            {
                ApiEndpoint = new Uri(App.AkeneoConfig.AkeneoUrl),
                ClientId = App.AkeneoConfig.ClientId,
                ClientSecret = App.AkeneoConfig.ClientSecret,
                UserName = App.AkeneoConfig.Username,
                Password = App.AkeneoConfig.Password
            };
            _client = new AkeneoClient(options);
        }

        public Task<bool> AddItemAsync(Item item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        async public Task<Item> GetItemAsync(string id)
        {
            var product = await _client.GetAsync<Product>(id);
            return ToItem(product);
        }

        async public Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            var categories = _categories.Select(c => Akeneo.Search.Category.In(c));

            var products = await _client.SearchAsync<Product>(categories);
            return products.GetItems().Select(ToItem);
        }

        public Task<bool> UpdateItemAsync(Item item)
        {
            throw new NotImplementedException();
        }

        static Item ToItem(Product product)
        {
            List<Akeneo.Model.ProductValue> brandName, functionalName, manufacturer;
            if (product == null) return null;
            var item = new Item();
            item.Id = product.Identifier;
            item.BrandName = product.Values.TryGetValue(Constants.ProductAttributes.BrandName, out brandName) ? brandName.FirstOrDefault()?.Data?.ToString() : "N/A";
            item.Text = item.BrandName;
            item.Description = product.Identifier;
            item.FunctionalName = product.Values.TryGetValue(Constants.ProductAttributes.FunctionalName, out functionalName) ? functionalName.FirstOrDefault()?.Data?.ToString() : "N/A";
            item.Manufacturer = product.Values.TryGetValue(Constants.ProductAttributes.ManufacturerGLN, out manufacturer) ? manufacturer.FirstOrDefault()?.Data?.ToString() : "N/A";
            item.GTIN = product.Identifier;
            return item;
        }
    }
}
