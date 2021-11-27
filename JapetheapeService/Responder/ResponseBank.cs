﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JapeHttp;
using JapeService.Responder;

namespace JapeService
{
    public class ResponseBank<T> : IEnumerable
    {
        private Dictionary<T, Func<Responder<T>.Transfer, JsonData, object[], Task<Resolution>>> responses = new();

        public IEnumerator GetEnumerator() => responses.GetEnumerator();
        public void Add(T id, Func<Responder<T>.Transfer, JsonData, object[], Task<Resolution>> response) => responses.Add(id, response);
        public void Remove(T id) => responses.Remove(id);
    }
}