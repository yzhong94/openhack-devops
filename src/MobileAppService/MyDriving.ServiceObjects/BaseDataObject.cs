using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;

namespace MyDriving.ServiceObjects
{
    public class BaseDataObject : System.Data.Entity.
    {
        public string Id;

        public BaseDataObject()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
