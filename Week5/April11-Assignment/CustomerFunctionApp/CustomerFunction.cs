using System.Net;
using System.Text.Json;
using CustomerFunctionApp.Data;
using CustomerFunctionApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;

namespace CustomerFunctionApp
{
    public class CustomerFunction
    {
        private readonly AppDbContext _context;

        public CustomerFunction(AppDbContext context)
        {
            _context = context;
        }

        [Function("GetCustomers")]
        public async Task<HttpResponseData> GetCustomers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers")] HttpRequestData req)
        {
            var customers = await _context.Customers.ToListAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(customers);

            return response;
        }

        [Function("GetCustomerById")]
        public async Task<HttpResponseData> GetCustomerById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers/{id:int}")] HttpRequestData req,
            int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(customer);

            return response;
        }

        [Function("CreateCustomer")]
        public async Task<HttpResponseData> CreateCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] HttpRequestData req)
        {
            var customer = await JsonSerializer.DeserializeAsync<Customer>(req.Body);

            _context.Customers.Add(customer!);
            await _context.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(customer);

            return response;
        }

        [Function("UpdateCustomer")]
        public async Task<HttpResponseData> UpdateCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "customers/{id:int}")] HttpRequestData req,
            int id)
        {
            var existing = await _context.Customers.FindAsync(id);

            if (existing == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var updated = await JsonSerializer.DeserializeAsync<Customer>(req.Body);

            existing.Name = updated!.Name;
            existing.Email = updated.Email;
            existing.City = updated.City;

            await _context.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(existing);

            return response;
        }

        [Function("DeleteCustomer")]
        public async Task<HttpResponseData> DeleteCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "customers/{id:int}")] HttpRequestData req,
            int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
