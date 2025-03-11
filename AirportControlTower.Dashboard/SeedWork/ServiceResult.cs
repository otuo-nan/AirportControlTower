namespace AirportControlTower.Dashboard.SeedWork
{
#nullable disable

    public class ServiceResultBase
    {
        public int Id { get; set; }
        public string Message { get; set; }

        //public bool IsSuccessful { get; set; }
        public List<string> Errors { get; set; } = [];

        public bool HasErrors => Errors.Count != 0;
    }

    public class ServiceResult<T> : ServiceResultBase
    {
        public T Entity { get; set; }

        public ServiceResult()
        {

        }

        public ServiceResult(string errorMessage)
        {
            Errors.Add(errorMessage);
        }

        public ServiceResult(IList<string> errors)
        {
            Errors.AddRange(errors);
        }

        public ServiceResult(T entity)
        {
            Entity = entity;
        }
    }

    public class ServiceResult : ServiceResultBase
    {
        public object Entity { get; set; }

        public ServiceResult()
        {

        }

        public ServiceResult(string errorMessage)
        {
            Errors.Add(errorMessage);
        }

        public ServiceResult(IList<string> errors)
        {
            Errors.AddRange(errors);
        }

        public ServiceResult(object entity)
        {
            Entity = entity;
        }
    }
}
