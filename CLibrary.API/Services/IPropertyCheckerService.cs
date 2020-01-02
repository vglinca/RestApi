namespace CLibrary.API.Services{
    public interface IPropertyCheckerService{
        bool CheckIfValid<T>(string fields);
    }
}