﻿using System.Net.Http;

namespace Mockaco
{
    public class RequestTemplate
    {
        public string Method { get; set; }
        public string Route { get; set; }
        public string Condition { get; set; }
    }
}