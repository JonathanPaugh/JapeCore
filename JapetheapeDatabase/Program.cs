﻿using System;
using System.Threading;
using JapeHttp;

namespace JapeDatabase
{
    public class Program
    {
        public static void Main()
        {
            Log.Init();
            Database database = new Database();
            database.Start();
            SpinWait.SpinUntil(() => false);
        }
    }
}
