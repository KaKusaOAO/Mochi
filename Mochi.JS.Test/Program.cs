using System;
using Mochi.JS;

Console.WriteLine("Hello, Browser!");
Window.Current.SetTimeout(() =>
{
    Console.WriteLine("Hello from timeout!");
}, 3000);