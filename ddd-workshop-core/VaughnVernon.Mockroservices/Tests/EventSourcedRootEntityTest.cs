//   Copyright © 2017 Vaughn Vernon. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using Xunit;
using System.Collections.Generic;

namespace VaughnVernon.Mockroservices
{
    public class EventSourcedRootEntityTest
    {
        [Fact]
        public void TestProductDefinedEventKept()
        {
            Product product = new Product("dice-fuz-1", "Fuzzy dice.", 999);
            Assert.Equal(1, product.MutatingEvents.Count);
            Assert.Equal("dice-fuz-1", product.Name);
            Assert.Equal("Fuzzy dice.", product.Description);
            Assert.Equal(999, product.Price);
            Assert.Equal(new ProductDefined("dice-fuz-1", "Fuzzy dice.", 999), product.MutatingEvents[0]);
        }

        [Fact]
        public void TestProductNameChangedEventKept()
        {
            Product product = new Product("dice-fuz-1", "Fuzzy dice.", 999);

            product.MutatingEvents.Clear();

            product.ChangeName("dice-fuzzy-1");
            Assert.Equal(1, product.MutatingEvents.Count);
            Assert.Equal("dice-fuzzy-1", product.Name);
            Assert.Equal(new ProductNameChanged("dice-fuzzy-1"), product.MutatingEvents[0]);
        }

        [Fact]
        public void TestProductDescriptionChangedEventsKept()
        {
            Product product = new Product("dice-fuz-1", "Fuzzy dice.", 999);

            product.MutatingEvents.Clear();

            product.ChangeDescription("Fuzzy dice, and all.");
            Assert.Equal(1, product.MutatingEvents.Count);
            Assert.Equal("Fuzzy dice, and all.", product.Description);
            Assert.Equal(new ProductDescriptionChanged("Fuzzy dice, and all."), product.MutatingEvents[0]);
        }

        [Fact]
        public void TestProductPriceChangedEventKept()
        {
            Product product = new Product("dice-fuz-1", "Fuzzy dice.", 999);

            product.MutatingEvents.Clear();

            product.ChangePrice(995);
            Assert.Equal(1, product.MutatingEvents.Count);
            Assert.Equal(995, product.Price);
            Assert.Equal(new ProductPriceChanged(995), product.MutatingEvents[0]);
        }

        [Fact]
        public void TestReconstitution()
        {
            Product product = new Product("dice-fuz-1", "Fuzzy dice.", 999);
            product.ChangeName("dice-fuzzy-1");
            product.ChangeDescription("Fuzzy dice, and all.");
            product.ChangePrice(995);

            Product productAgain = new Product(product.MutatingEvents, product.MutatedVersion);
            Assert.Equal(product, productAgain);
        }
    }

    public class Product : EventSourcedRootEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public long Price { get; private set; }

        public Product(string name, string description, long price)
        {
            Apply(new ProductDefined(name, description, price));
        }

        public Product(List<DomainEvent> eventStream, int streamVersion)
            : base(eventStream, streamVersion)
        {
        }

        public void ChangeDescription(string description)
        {
            Apply(new ProductDescriptionChanged(description));
        }

        public void ChangeName(string name)
        {
            Apply(new ProductNameChanged(name));
        }

        public void ChangePrice(long price)
        {
            Apply(new ProductPriceChanged(price));
        }

        public override bool Equals(Object other)
        {
            if (other == null || other.GetType() != typeof(Product))
            {
                return false;
            }

            Product otherProduct = (Product)other;

            return this.Name.Equals(otherProduct.Name) &&
                this.Description.Equals(otherProduct.Description) &&
                this.Price == otherProduct.Price;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public void When(ProductDefined e)
        {
            this.Name = e.Name;
            this.Description = e.Description;
            this.Price = e.Price;
        }

        public void When(ProductDescriptionChanged e)
        {
            this.Description = e.Description;
        }

        public void When(ProductNameChanged e)
        {
            this.Name = e.Name;
        }

        public void When(ProductPriceChanged e)
        {
            this.Price = e.Price;
        }
    }

    public class ProductDefined : DomainEvent
    {
        public string Description { get; private set; }
        public string Name { get; private set; }
        public long Price { get; private set; }

        public ProductDefined(string name, string description, long price)
            : base()
        {
            this.Name = name;
            this.Description = description;
            this.Price = price;
        }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(ProductDefined))
            {
                return false;
            }

            ProductDefined otherProductDefined = (ProductDefined)other;

            return this.Name.Equals(otherProductDefined.Name) &&
                this.Description.Equals(otherProductDefined.Description) &&
                this.Price == otherProductDefined.Price &&
                this.EventVersion == otherProductDefined.EventVersion;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Description.GetHashCode() + (int)Price;
        }
    }

    public class ProductDescriptionChanged : DomainEvent
    {
        public string Description { get; private set; }

        public ProductDescriptionChanged(string description)
            : base()
        {
            this.Description = description;
        }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(ProductDescriptionChanged))
            {
                return false;
            }

            ProductDescriptionChanged otherProductDescriptionChanged = (ProductDescriptionChanged)other;

            return this.Description.Equals(otherProductDescriptionChanged.Description) &&
                this.EventVersion == otherProductDescriptionChanged.EventVersion;
        }

        public override int GetHashCode()
        {
            return Description.GetHashCode();
        }
    }

    public class ProductNameChanged : DomainEvent
    {
        public string Name { get; private set; }

        public ProductNameChanged(string name)
            : base()
        {
            this.Name = name;
        }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(ProductNameChanged))
            {
                return false;
            }

            ProductNameChanged otherProductNameChanged = (ProductNameChanged)other;

            return this.Name.Equals(otherProductNameChanged.Name) &&
                this.EventVersion == otherProductNameChanged.EventVersion;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public class ProductPriceChanged : DomainEvent
    {
        public long Price { get; private set; }

        public ProductPriceChanged(long price)
            : base()
        {
            this.Price = price;
        }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(ProductPriceChanged))
            {
                return false;
            }

            ProductPriceChanged otherProductPriceChanged = (ProductPriceChanged)other;

            return this.Price == otherProductPriceChanged.Price &&
                this.EventVersion == otherProductPriceChanged.EventVersion;
        }

        public override int GetHashCode()
        {
            return (int)Price;
        }
    }
}
