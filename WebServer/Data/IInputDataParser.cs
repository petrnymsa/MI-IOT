using System;

namespace WebServer.Data
{
    public interface IInputDataParser<T>
    {
        T Parse(string raw, DateTime dateRecord);
    }   
}
