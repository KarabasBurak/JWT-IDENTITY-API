﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class ErrorDto
    {
        public List<string> Errors { get; private set; } // Bu sınıf içinde Başka bir metodda kullanabilmek için private yaptık
        public bool IsShow { get; private set; }

        public ErrorDto()
        {
            Errors = new List<string>();

        }

        public ErrorDto(string error, bool isShow)
        {
            Errors?.Add(error);
            isShow = true;
        }

        public ErrorDto(List<string> errors, bool isShow)
        {
            Errors = Errors;
            IsShow = isShow;
        }

        
    }
}
