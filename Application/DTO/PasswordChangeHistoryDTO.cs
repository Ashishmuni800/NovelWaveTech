﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class PasswordChangeHistoryDTO
    {
        public int Id { get; set; }
        public string UserPassword { get; set; }
        public string OldPassword { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
