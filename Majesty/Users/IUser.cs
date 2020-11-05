namespace Majesty.Users
{
    interface IUser<T> : IUserBase
    {
        T DestinationTo { get; }
        T DestinationFrom { get; }
    }
}
