﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Tailspin.Infrastructure;

namespace Tailspin.Model {
    
    public class ShoppingCart {
        
        public ShoppingCart():this("") { }
        
        public ShoppingCart(string userName){

            Items = new List<ShoppingCartItem>();
            this.UserName = userName;
            this.ShippingService = "";
        }
        
        public string UserName { get; set; }
        public List<ShoppingCartItem> Items { get; set; }
        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
        public CreditCard CreditCard { get; set; }
        public string ShippingService { get; set; }
        public decimal ShippingAmount { get; set; }
        public int ShippingMethodID { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }

        public decimal SubTotal {
            get {
                return Items.Sum(x => x.LineTotal);
            }
        }
        public int TotalItems {
            get {
                return Items.Sum(x=>x.Quantity);
            }
        }
        public decimal Total {
            get {
                return SubTotal + TaxAmount+ShippingAmount;
            }
        }

        ShoppingCartItem _itemLastAdded;
        public ShoppingCartItem ItemLastAdded {
            get {
                if (this.Items.Count > 0) {
                    _itemLastAdded = Items.OrderByDescending(x => x.DateAdded).FirstOrDefault();
                }
                return _itemLastAdded;
            }
        }

        /// <summary>
        /// Adds a product to the cart
        /// </summary>
        public void AddItem(Product product) {
            AddItem(product, 1);
        }
        /// <summary>
        /// Adds a product to the cart
        /// </summary>
        public void AddItem(Product product,int quantity) {
            AddItem(product, quantity,DateTime.Now);
        }


        /// <summary>
        /// Removes all items from cart
        /// </summary>
        public void ClearItems() {
            if (Items != null)
                Items.Clear();
        }

        /// <summary>
        /// Adds a product to the cart
        /// </summary>
        public void AddItem(Product product, int quantity, DateTime dateAdded) {

            //see if this item is in the cart already
            ShoppingCartItem item = FindItem(product);

            if (quantity != 0) {
                if (item != null) {
                    //if the passed in amount is 0, do nothing
                    //as we're assuming "add 0 of this item" means
                    //do nothing
                    if (quantity != 0)
                        AdjustQuantity(product, item.Quantity);
                } else {
                    if (quantity > 0) {
                        item = new ShoppingCartItem(product, quantity, dateAdded);

                        //add to list
                        Items.Add(item);
                    }
                }

            }

        }
        /// <summary>
        /// Adjusts the quantity of an item in the cart
        /// </summary>
        public void AdjustQuantity(string sku, int newQuantity) {
            ShoppingCartItem itemToAdjust = FindItem(sku);
            AdjustQuantity(itemToAdjust.Product, newQuantity);

        }
        /// <summary>
        /// Adjusts the quantity of an item in the cart
        /// </summary>
        public void AdjustQuantity(Product product, int newQuantity) {
            ShoppingCartItem itemToAdjust = FindItem(product);
            if (itemToAdjust != null) {
                if (newQuantity <= 0) {
                    this.RemoveItem(product);
                } else {
                    itemToAdjust.AdjustQuantity(newQuantity);
                }

            }

        }

        /// <summary>
        /// Remmoves a product from the cart
        /// </summary>
        public void RemoveItem(Product product) {
            RemoveItem(product.SKU);
        }

        /// <summary>
        /// Remmoves a product from the cart
        /// </summary>
        public void RemoveItem(string sku) {
            var itemToRemove = FindItem(sku);
            if (itemToRemove != null) {
                Items.Remove(itemToRemove);
            }
        }


        /// <summary>
        /// Finds an item in the cart
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public ShoppingCartItem FindItem(Product product) {
            //see if this item is in the cart already
            return (from items in Items
                    where items.Product.Equals(product)
                    select items).SingleOrDefault();

        }
        /// <summary>
        /// Finds an item in the cart
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public ShoppingCartItem FindItem(string sku) {
            //see if this item is in the cart already
            return (from items in Items
                    where items.Product.SKU == sku
                    select items).SingleOrDefault();

        }

    }
}
