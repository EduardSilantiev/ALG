namespace ALG.Application
{
    public static class AppConsts
    {
        public enum HttpStatusCodes : int
        {
            Status200OK = 200,
            Status204NoContent = 204,
            Status400BadRequest = 400,
            Status401Unauthorized = 401,
            Status409Conflict = 409,
            Status422UnprocessableEntity = 422
        }
    }
}
