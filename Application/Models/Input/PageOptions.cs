namespace Application.Models.Input
{
    /// <summary>
    /// Options for pagination
    /// </summary>
    public record PageOptions
    {
        /// <summary>
        /// Min number is 1. If page number is greater than total pages count then empty collection will be returned
        /// </summary>
        public required uint Page { get; init; }
        /// <summary>
        /// Min size is 1. Number of items per page
        /// </summary>
        public required uint PageSize { get; init; }
    }
}
