using System;
namespace SpaceBattle.Interfaces
{
    public interface IQueue<T>
    {
        void Push(T elem);

        T Pop();
    }
}
