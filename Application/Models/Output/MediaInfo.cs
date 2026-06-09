using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Output
{
    public record MediaInfo
    {
        public required string ContentType { get; init; }
        public required string FileName { get; init; }
    }
}
