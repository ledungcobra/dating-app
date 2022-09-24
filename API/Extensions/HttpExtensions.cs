using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace API.Extensions
{
  public static class HttpExtensions
  {
    public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
      var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
      var options = new JsonSerializerSettings();
      options.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
      response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, options));
      response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
  }
}