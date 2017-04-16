using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Classes
{
    internal class Exchange
    {
        private long id;
        private String name;
        private String country;
        private String localAddress;
        private String owner;
        private String website;
        private long contactNumber;

        public Exchange(long id, String name)
        {
            this.name = name;
            this.id = id;
        }

        public Exchange(long id, String name, String country, String localAddress, String owner, String website, long contactNumber)
        {
            this.id = id;
            this.name = name;
            this.country = country;
            this.localAddress = LocalAddress;
            this.owner = owner;
            this.website = website;
            this.contactNumber = contactNumber;
        }

        public long Id
        {
            get
            {
                return id;
            }

        }

        public string Country
        {
            get
            {
                return country;
            }

        }

        public string LocalAddress
        {
            get
            {
                return localAddress;
            }

        }

        public string Owner
        {
            get
            {
                return owner;
            }

        }

        public string Website
        {
            get
            {
                return website;
            }

        }

        public long ContactNumber
        {
            get
            {
                return contactNumber;
            }

        }

        public string Name
        {
            get
            {
                return name;
            }

        }
    }
}