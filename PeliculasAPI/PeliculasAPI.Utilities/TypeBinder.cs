using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace PeliculasAPI.PeliculasAPI.Utilities
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propName = bindingContext.ModelName;
            var valueProvider = bindingContext.ValueProvider.GetValue(propName);
            if(valueProvider == ValueProviderResult.None) { return Task.CompletedTask; }
            try
            {
                var desValue = JsonConvert.DeserializeObject<T>(valueProvider.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(desValue);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(propName, "Valor invalido");
            }
            return Task.CompletedTask;
        }
    }
}
