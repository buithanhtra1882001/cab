using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CabPaymentService.Infrastructures.Conventions
{
    public class ApiControllerConvention : IControllerModelConvention
    {
        private readonly Type _targetedControllerType;

        public ApiControllerConvention(Type targetedControllerType)
        {
            _targetedControllerType = targetedControllerType;
        }

        public void Apply(ControllerModel controller)
        {
            var type = controller.ControllerType;
            if(type.BaseType?.GetGenericTypeDefinition() != _targetedControllerType &&
                controller.Selectors.Any(selector => selector.AttributeRouteModel != null))
            {
                foreach (var controllerSelector in controller.Selectors.Where(selector => selector.AttributeRouteModel != null))
                {
                    var originalTemp = controllerSelector.AttributeRouteModel.Template;

                    var newName = type.Name;
                    if (newName.Contains("Admin"))
                    {
                        newName = "admin/" + type.Name.Split("Admin")[0];
                    }
                    else
                    {
                        newName = type.Name.Split("Controller")[0];
                    }

                    controllerSelector.AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = originalTemp.Replace("[controller]", newName)
                    };
                }
            }
        }
    }
}
