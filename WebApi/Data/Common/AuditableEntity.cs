﻿using System;

namespace WebApi.Data.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
